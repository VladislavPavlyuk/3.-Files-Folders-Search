using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FileSearchApp
{
    public partial class MainForm : Form
    {
        private CancellationTokenSource? _cancellationTokenSource;
        private ManualResetEventSlim? _pauseEvent;
        private bool _isSearching = false;
        private bool _isPaused = false;

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
                    string label = string.IsNullOrEmpty(drive.VolumeLabel) ? "Local Disk" : drive.VolumeLabel;
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
            listViewResults.GridLines = true;
            listViewResults.MultiSelect = false;
            listViewResults.Columns.Add("Name", 300);
            listViewResults.Columns.Add("Path", 500);
            listViewResults.Columns.Add("Type", 100);
            listViewResults.Columns.Add("Size", 100);
            listViewResults.ColumnClick += ListViewResults_ColumnClick;
            listViewResults.DoubleClick += ListViewResults_DoubleClick;
            
            // Enable sorting
            listViewResults.Sorting = SortOrder.Ascending;
        }

        private void ListViewResults_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            ListView listView = (ListView)sender;
            
            // Determine if clicked column is already the column that is being sorted.
            if (e.Column == listView.Tag as int?)
            {
                // Reverse the current sort direction for this column.
                if (listView.Sorting == SortOrder.Ascending)
                {
                    listView.Sorting = SortOrder.Descending;
                }
                else
                {
                    listView.Sorting = SortOrder.Ascending;
                }
            }
            else
            {
                // Set the column number that is to be sorted; default to ascending.
                listView.Tag = e.Column;
                listView.Sorting = SortOrder.Ascending;
            }
            
            // Create a comparer.
            listView.ListViewItemSorter = new ListViewItemComparer(e.Column, listView.Sorting);
        }

        private void ListViewResults_DoubleClick(object sender, EventArgs e)
        {
            if (listViewResults.SelectedItems.Count > 0)
            {
                string path = listViewResults.SelectedItems[0].Tag?.ToString() ?? "";
                if (!string.IsNullOrEmpty(path))
                {
                    try
                    {
                        if (File.Exists(path))
                        {
                            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                            {
                                FileName = path,
                                UseShellExecute = true
                            });
                        }
                        else if (Directory.Exists(path))
                        {
                            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                            {
                                FileName = "explorer.exe",
                                Arguments = path,
                                UseShellExecute = true
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Cannot open: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
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
                MessageBox.Show("Enter search mask", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (comboBoxDisk.SelectedItem == null)
            {
                MessageBox.Show("Select a disk for search", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string rootPath = "";
            string selectedText = comboBoxDisk.SelectedItem?.ToString() ?? "";
            
            // Extract disk path from text (format: "C: (System)" -> "C:\")
            int colonIndex = selectedText.IndexOf(':');
            if (colonIndex >= 0)
            {
                rootPath = selectedText.Substring(0, colonIndex + 1) + "\\";
            }
            else
            {
                // Try to find path through DriveInfo
                foreach (DriveInfo drive in DriveInfo.GetDrives())
                {
                    if (drive.IsReady)
                    {
                        string label = string.IsNullOrEmpty(drive.VolumeLabel) ? "Local Disk" : drive.VolumeLabel;
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
                MessageBox.Show("Invalid disk path", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string searchText = checkBoxSearchInContent.Checked ? textBoxContent.Text.Trim() : "";
            bool searchInSubdirectories = checkBoxSearchInSubdirectories.Checked;
            StartSearch(rootPath, mask, searchText, searchInSubdirectories);
        }

        private void StartSearch(string rootPath, string mask, string searchText, bool searchInSubdirectories)
        {
            _isSearching = true;
            _isPaused = false;
            buttonSearch.Text = "Stop";
            buttonPause.Text = "Pause";
            buttonPause.Enabled = true;
            listViewResults.Items.Clear();
            labelStatus.Text = "Searching...";
            progressBar.Visible = true;
            progressBar.Style = ProgressBarStyle.Marquee;

            _cancellationTokenSource = new CancellationTokenSource();
            _pauseEvent = new ManualResetEventSlim(true); // Initially not paused
            var token = _cancellationTokenSource.Token;

            Task.Run(() => PerformSearch(rootPath, mask, searchText, searchInSubdirectories, token), token);
        }

        private void CancelSearch()
        {
            _cancellationTokenSource?.Cancel();
            _pauseEvent?.Dispose();
            _pauseEvent = null;
            buttonSearch.Text = "Search";
            buttonPause.Text = "Pause";
            buttonPause.Enabled = false;
            labelStatus.Text = "Search cancelled";
            progressBar.Visible = false;
            _isSearching = false;
            _isPaused = false;
        }

        private void buttonPause_Click(object sender, EventArgs e)
        {
            if (!_isSearching)
                return;

            if (_isPaused)
            {
                // Resume
                _isPaused = false;
                _pauseEvent?.Set();
                buttonPause.Text = "Pause";
                labelStatus.Text = "Searching...";
            }
            else
            {
                // Pause
                _isPaused = true;
                _pauseEvent?.Reset();
                buttonPause.Text = "Resume";
                labelStatus.Text = "Paused";
            }
        }

        private void PerformSearch(string rootPath, string mask, string searchText, bool searchInSubdirectories, CancellationToken cancellationToken)
        {
            try
            {
                if (!Directory.Exists(rootPath))
                {
                    UpdateUI(() =>
                    {
                        MessageBox.Show($"Directory does not exist: {rootPath}", "Error", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        labelStatus.Text = "Error";
                        progressBar.Visible = false;
                        buttonSearch.Text = "Search";
                        buttonPause.Enabled = false;
                        _isSearching = false;
                    });
                    return;
                }

                string pattern = ConvertMaskToRegex(mask);
                var regex = new Regex(pattern, RegexOptions.IgnoreCase);

                int foundCount = 0;
                bool searchInContent = !string.IsNullOrEmpty(searchText);
                SearchDirectory(rootPath, regex, searchText, searchInContent, searchInSubdirectories, ref foundCount, cancellationToken);

                if (!cancellationToken.IsCancellationRequested)
                {
                    UpdateUI(() =>
                    {
                        labelStatus.Text = $"Found: {foundCount}";
                        progressBar.Visible = false;
                        buttonSearch.Text = "Search";
                        buttonPause.Enabled = false;
                        _isSearching = false;
                    });
                }
            }
            catch (OperationCanceledException)
            {
                // Search cancelled - do nothing
            }
            catch (Exception ex)
            {
                if (!cancellationToken.IsCancellationRequested)
                {
                    UpdateUI(() =>
                    {
                        MessageBox.Show($"Search error: {ex.Message}\n\n{ex.StackTrace}", "Error", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        labelStatus.Text = "Error";
                        progressBar.Visible = false;
                        buttonSearch.Text = "Search";
                        buttonPause.Enabled = false;
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
                    // Form closed, ignore
                }
            }
            else if (!this.IsDisposed)
            {
                action();
            }
        }

        private void WaitIfPaused()
        {
            _pauseEvent?.Wait();
        }

        private void SearchDirectory(string directory, Regex pattern, string searchText, bool searchInContent, bool searchInSubdirectories, ref int foundCount, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            WaitIfPaused();

            try
            {
                // Search files
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

                    WaitIfPaused();

                    try
                    {
                        string fileName = Path.GetFileName(file);
                        if (!string.IsNullOrEmpty(fileName) && pattern.IsMatch(fileName))
                        {
                            // If content search is enabled, check file content
                            if (searchInContent)
                            {
                                if (FileContainsText(file, searchText, cancellationToken))
                                {
                                    foundCount++;
                                    AddResultToListView(file, true);
                                }
                            }
                            else
                            {
                                foundCount++;
                                AddResultToListView(file, true);
                            }
                        }
                    }
                    catch
                    {
                        // Skip problematic files
                    }
                }

                // Search folders
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

                    WaitIfPaused();

                    try
                    {
                        string dirName = Path.GetFileName(dir);
                        if (!string.IsNullOrEmpty(dirName) && pattern.IsMatch(dirName))
                        {
                            foundCount++;
                            AddResultToListView(dir, false);
                        }

                        // Recursive search in subdirectories
                        if (searchInSubdirectories)
                        {
                            SearchDirectory(dir, pattern, searchText, searchInContent, searchInSubdirectories, ref foundCount, cancellationToken);
                        }
                    }
                    catch (UnauthorizedAccessException)
                    {
                        // Skip folders without access
                    }
                    catch (DirectoryNotFoundException)
                    {
                        // Skip non-existent folders
                    }
                    catch
                    {
                        // Skip other errors
                    }
                }
            }
            catch (UnauthorizedAccessException)
            {
                // Skip folders without access
            }
            catch (DirectoryNotFoundException)
            {
                // Skip non-existent folders
            }
            catch
            {
                // Skip other errors
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
                    string type = isFile ? "File" : "Folder";
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
                    // Ignore errors when adding to list
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
            // Escape regex special characters except * and ?
            string escaped = Regex.Escape(mask);
            // Replace escaped \* and \? with regex patterns
            escaped = escaped.Replace(@"\*", ".*").Replace(@"\?", ".");
            return "^" + escaped + "$";
        }

        private bool FileContainsText(string filePath, string searchText, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return false;

            WaitIfPaused();

            try
            {
                var fileInfo = new FileInfo(filePath);
                
                // Skip files that are too large (> 10 MB)
                if (fileInfo.Length > 10 * 1024 * 1024)
                    return false;

                // Skip binary files by extension
                string extension = fileInfo.Extension.ToLower();
                string[] binaryExtensions = { ".exe", ".dll", ".bin", ".so", ".dylib", ".lib", ".obj", ".o", ".a" };
                if (Array.IndexOf(binaryExtensions, extension) >= 0)
                    return false;

                // Try to read file with different encodings
                Encoding[] encodings = { Encoding.UTF8, Encoding.Default, Encoding.ASCII, Encoding.Unicode, Encoding.BigEndianUnicode };

                foreach (var encoding in encodings)
                {
                    try
                    {
                        if (cancellationToken.IsCancellationRequested)
                            return false;

                        WaitIfPaused();

                        // Read file line by line to save memory
                        using (var reader = new StreamReader(filePath, encoding, detectEncodingFromByteOrderMarks: true))
                        {
                            string? line;
                            int lineCount = 0;
                            const int maxLines = 10000; // Limit for very large text files

                            while ((line = reader.ReadLine()) != null)
                            {
                                if (cancellationToken.IsCancellationRequested)
                                    return false;

                                WaitIfPaused();

                                if (lineCount++ > maxLines)
                                    break;

                                if (line.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0)
                                {
                                    return true;
                                }
                            }
                        }
                        // If file was successfully read, stop trying other encodings
                        break;
                    }
                    catch (DecoderFallbackException)
                    {
                        // Wrong encoding, try next one
                        continue;
                    }
                    catch (OutOfMemoryException)
                    {
                        // File too large to read
                        return false;
                    }
                    catch
                    {
                        // Other errors - try next encoding
                        continue;
                    }
                }
            }
            catch (UnauthorizedAccessException)
            {
                // No access to file
                return false;
            }
            catch (IOException)
            {
                // I/O error
                return false;
            }
            catch
            {
                // Other errors
                return false;
            }

            return false;
        }

        private void checkBoxSearchInContent_CheckedChanged(object sender, EventArgs e)
        {
            textBoxContent.Enabled = checkBoxSearchInContent.Checked;
            if (checkBoxSearchInContent.Checked)
            {
                textBoxContent.Focus();
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (_isSearching)
            {
                _cancellationTokenSource?.Cancel();
            }
            _pauseEvent?.Dispose();
            base.OnFormClosing(e);
        }
    }

    // Comparer for ListView sorting
    public class ListViewItemComparer : IComparer
    {
        private int _column;
        private SortOrder _order;

        public ListViewItemComparer(int column, SortOrder order)
        {
            _column = column;
            _order = order;
        }

        public int Compare(object? x, object? y)
        {
            if (x == null || y == null)
                return 0;

            ListViewItem itemX = (ListViewItem)x;
            ListViewItem itemY = (ListViewItem)y;

            string textX = _column < itemX.SubItems.Count ? itemX.SubItems[_column].Text : "";
            string textY = _column < itemY.SubItems.Count ? itemY.SubItems[_column].Text : "";

            int result = 0;

            // Try to parse as numbers for Size column (column 3)
            if (_column == 3)
            {
                if (double.TryParse(textX.Replace(" B", "").Replace(" KB", "").Replace(" MB", "").Replace(" GB", "").Replace(" TB", ""), out double numX) &&
                    double.TryParse(textY.Replace(" B", "").Replace(" KB", "").Replace(" MB", "").Replace(" GB", "").Replace(" TB", ""), out double numY))
                {
                    // Convert to bytes for proper comparison
                    double bytesX = ConvertToBytes(textX, numX);
                    double bytesY = ConvertToBytes(textY, numY);
                    result = bytesX.CompareTo(bytesY);
                }
                else
                {
                    result = string.Compare(textX, textY, StringComparison.OrdinalIgnoreCase);
                }
            }
            else
            {
                result = string.Compare(textX, textY, StringComparison.OrdinalIgnoreCase);
            }

            return _order == SortOrder.Ascending ? result : -result;
        }

        private double ConvertToBytes(string text, double value)
        {
            if (text.Contains(" TB"))
                return value * 1024 * 1024 * 1024 * 1024;
            else if (text.Contains(" GB"))
                return value * 1024 * 1024 * 1024;
            else if (text.Contains(" MB"))
                return value * 1024 * 1024;
            else if (text.Contains(" KB"))
                return value * 1024;
            else
                return value;
        }
    }
}

