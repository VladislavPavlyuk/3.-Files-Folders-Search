using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FileSearchApp
{
    public partial class MainForm : Form
    {
        private CancellationTokenSource? _cancellationTokenSource;
        private bool _isSearching = false;

        public MainForm()
        {
            InitializeComponent();
            InitializeDisks();
            InitializeListView();
        }

        private void InitializeDisks()
        {
            comboBoxDisk.Items.Clear();
            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                if (drive.IsReady)
                {
                    string label = string.IsNullOrEmpty(drive.VolumeLabel) ? "Локальный диск" : drive.VolumeLabel;
                    string displayText = $"{drive.Name.TrimEnd('\\')} ({label})";
                    comboBoxDisk.Items.Add(displayText);
                }
            }
            if (comboBoxDisk.Items.Count > 0)
            {
                comboBoxDisk.SelectedIndex = 0;
            }
        }

        private void InitializeListView()
        {
            listViewResults.View = View.Details;
            listViewResults.FullRowSelect = true;
            listViewResults.Columns.Add("Имя", 300);
            listViewResults.Columns.Add("Путь", 500);
            listViewResults.Columns.Add("Тип", 100);
            listViewResults.Columns.Add("Размер", 100);
        }

        private void buttonSearch_Click(object sender, EventArgs e)
        {
            if (_isSearching)
            {
                CancelSearch();
                return;
            }

            string mask = textBoxMask.Text.Trim();
            if (string.IsNullOrEmpty(mask))
            {
                MessageBox.Show("Введите маску для поиска", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (comboBoxDisk.SelectedItem == null)
            {
                MessageBox.Show("Выберите диск для поиска", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string rootPath = "";
            string selectedText = comboBoxDisk.SelectedItem?.ToString() ?? "";
            
            // Извлекаем путь диска из текста (формат: "C: (System)" -> "C:\")
            int colonIndex = selectedText.IndexOf(':');
            if (colonIndex >= 0)
            {
                rootPath = selectedText.Substring(0, colonIndex + 1) + "\\";
            }
            else
            {
                // Пытаемся найти путь через DriveInfo
                foreach (DriveInfo drive in DriveInfo.GetDrives())
                {
                    if (drive.IsReady)
                    {
                        string label = string.IsNullOrEmpty(drive.VolumeLabel) ? "Локальный диск" : drive.VolumeLabel;
                        string displayText = $"{drive.Name.TrimEnd('\\')} ({label})";
                        if (displayText == selectedText)
                        {
                            rootPath = drive.Name;
                            break;
                        }
                    }
                }
            }

            if (string.IsNullOrEmpty(rootPath) || !Directory.Exists(rootPath))
            {
                MessageBox.Show("Неверный путь к диску", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            StartSearch(rootPath, mask);
        }

        private void StartSearch(string rootPath, string mask)
        {
            _isSearching = true;
            buttonSearch.Text = "Остановить";
            listViewResults.Items.Clear();
            labelStatus.Text = "Поиск...";
            progressBar.Visible = true;
            progressBar.Style = ProgressBarStyle.Marquee;

            _cancellationTokenSource = new CancellationTokenSource();
            var token = _cancellationTokenSource.Token;

            Task.Run(() => PerformSearch(rootPath, mask, token), token);
        }

        private void CancelSearch()
        {
            _cancellationTokenSource?.Cancel();
            buttonSearch.Text = "Поиск";
            labelStatus.Text = "Поиск отменен";
            progressBar.Visible = false;
            _isSearching = false;
        }

        private void PerformSearch(string rootPath, string mask, CancellationToken cancellationToken)
        {
            try
            {
                if (!Directory.Exists(rootPath))
                {
                    UpdateUI(() =>
                    {
                        MessageBox.Show($"Директория не существует: {rootPath}", "Ошибка", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        labelStatus.Text = "Ошибка";
                        progressBar.Visible = false;
                        buttonSearch.Text = "Поиск";
                        _isSearching = false;
                    });
                    return;
                }

                string pattern = ConvertMaskToRegex(mask);
                var regex = new Regex(pattern, RegexOptions.IgnoreCase);

                int foundCount = 0;
                SearchDirectory(rootPath, regex, ref foundCount, cancellationToken);

                if (!cancellationToken.IsCancellationRequested)
                {
                    UpdateUI(() =>
                    {
                        labelStatus.Text = $"Найдено: {foundCount}";
                        progressBar.Visible = false;
                        buttonSearch.Text = "Поиск";
                        _isSearching = false;
                    });
                }
            }
            catch (OperationCanceledException)
            {
                // Поиск отменен - ничего не делаем
            }
            catch (Exception ex)
            {
                if (!cancellationToken.IsCancellationRequested)
                {
                    UpdateUI(() =>
                    {
                        MessageBox.Show($"Ошибка при поиске: {ex.Message}\n\n{ex.StackTrace}", "Ошибка", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        labelStatus.Text = "Ошибка";
                        progressBar.Visible = false;
                        buttonSearch.Text = "Поиск";
                        _isSearching = false;
                    });
                }
            }
        }

        private void UpdateUI(Action action)
        {
            if (this.InvokeRequired && !this.IsDisposed)
            {
                try
                {
                    this.Invoke((MethodInvoker)delegate { action(); });
                }
                catch (ObjectDisposedException)
                {
                    // Форма закрыта, игнорируем
                }
            }
            else if (!this.IsDisposed)
            {
                action();
            }
        }

        private void SearchDirectory(string directory, Regex pattern, ref int foundCount, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            try
            {
                // Поиск файлов
                string[] files;
                try
                {
                    files = Directory.GetFiles(directory);
                }
                catch (UnauthorizedAccessException)
                {
                    return;
                }
                catch (DirectoryNotFoundException)
                {
                    return;
                }

                foreach (string file in files)
                {
                    if (cancellationToken.IsCancellationRequested)
                        return;

                    try
                    {
                        string fileName = Path.GetFileName(file);
                        if (!string.IsNullOrEmpty(fileName) && pattern.IsMatch(fileName))
                        {
                            foundCount++;
                            AddResultToListView(file, true);
                        }
                    }
                    catch
                    {
                        // Пропускаем проблемные файлы
                    }
                }

                // Поиск папок
                string[] directories;
                try
                {
                    directories = Directory.GetDirectories(directory);
                }
                catch (UnauthorizedAccessException)
                {
                    return;
                }
                catch (DirectoryNotFoundException)
                {
                    return;
                }

                foreach (string dir in directories)
                {
                    if (cancellationToken.IsCancellationRequested)
                        return;

                    try
                    {
                        string dirName = Path.GetFileName(dir);
                        if (!string.IsNullOrEmpty(dirName) && pattern.IsMatch(dirName))
                        {
                            foundCount++;
                            AddResultToListView(dir, false);
                        }

                        // Рекурсивный поиск в подпапках
                        SearchDirectory(dir, pattern, ref foundCount, cancellationToken);
                    }
                    catch (UnauthorizedAccessException)
                    {
                        // Пропускаем папки без доступа
                    }
                    catch (DirectoryNotFoundException)
                    {
                        // Пропускаем несуществующие папки
                    }
                    catch
                    {
                        // Пропускаем другие ошибки
                    }
                }
            }
            catch (UnauthorizedAccessException)
            {
                // Пропускаем папки без доступа
            }
            catch (DirectoryNotFoundException)
            {
                // Пропускаем несуществующие папки
            }
            catch
            {
                // Пропускаем другие ошибки
            }
        }

        private void AddResultToListView(string path, bool isFile)
        {
            UpdateUI(() =>
            {
                try
                {
                    if (this.IsDisposed || listViewResults.IsDisposed)
                        return;

                    string name = Path.GetFileName(path);
                    if (string.IsNullOrEmpty(name))
                        name = path;

                    string directory = Path.GetDirectoryName(path) ?? "";
                    string type = isFile ? "Файл" : "Папка";
                    string size = "";

                    if (isFile)
                    {
                        try
                        {
                            var fileInfo = new FileInfo(path);
                            if (fileInfo.Exists)
                            {
                                size = FormatFileSize(fileInfo.Length);
                            }
                            else
                            {
                                size = "N/A";
                            }
                        }
                        catch
                        {
                            size = "N/A";
                        }
                    }

                    var item = new ListViewItem(name);
                    item.SubItems.Add(directory);
                    item.SubItems.Add(type);
                    item.SubItems.Add(size);
                    item.Tag = path;

                    listViewResults.Items.Add(item);
                }
                catch
                {
                    // Игнорируем ошибки при добавлении в список
                }
            });
        }

        private string FormatFileSize(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            double len = bytes;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }
            return $"{len:0.##} {sizes[order]}";
        }

        private string ConvertMaskToRegex(string mask)
        {
            // Экранируем специальные символы regex, кроме * и ?
            string escaped = Regex.Escape(mask);
            // Заменяем экранированные \* и \? на regex паттерны
            escaped = escaped.Replace(@"\*", ".*").Replace(@"\?", ".");
            return "^" + escaped + "$";
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (_isSearching)
            {
                _cancellationTokenSource?.Cancel();
            }
            base.OnFormClosing(e);
        }
    }
}

