namespace FileSearchApp
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.TextBox textBoxMask;
        private System.Windows.Forms.ComboBox comboBoxDisk;
        private System.Windows.Forms.ListView listViewResults;
        private System.Windows.Forms.Button buttonSearch;
        private System.Windows.Forms.Label labelMask;
        private System.Windows.Forms.Label labelDisk;
        private System.Windows.Forms.Label labelStatus;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.TextBox textBoxContent;
        private System.Windows.Forms.Label labelContent;
        private System.Windows.Forms.CheckBox checkBoxSearchInContent;
        private System.Windows.Forms.Button buttonPause;
        private System.Windows.Forms.CheckBox checkBoxSearchInSubdirectories;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.textBoxMask = new System.Windows.Forms.TextBox();
            this.comboBoxDisk = new System.Windows.Forms.ComboBox();
            this.listViewResults = new System.Windows.Forms.ListView();
            this.buttonSearch = new System.Windows.Forms.Button();
            this.labelMask = new System.Windows.Forms.Label();
            this.labelDisk = new System.Windows.Forms.Label();
            this.labelStatus = new System.Windows.Forms.Label();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.textBoxContent = new System.Windows.Forms.TextBox();
            this.labelContent = new System.Windows.Forms.Label();
            this.checkBoxSearchInContent = new System.Windows.Forms.CheckBox();
            this.buttonPause = new System.Windows.Forms.Button();
            this.checkBoxSearchInSubdirectories = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // textBoxMask
            // 
            this.textBoxMask.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxMask.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.textBoxMask.Location = new System.Drawing.Point(12, 30);
            this.textBoxMask.Name = "textBoxMask";
            this.textBoxMask.Size = new System.Drawing.Size(600, 21);
            this.textBoxMask.TabIndex = 0;
            this.textBoxMask.Text = "*.*";
            // 
            // comboBoxDisk
            // 
            this.comboBoxDisk.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxDisk.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxDisk.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.comboBoxDisk.FormattingEnabled = true;
            this.comboBoxDisk.Location = new System.Drawing.Point(12, 75);
            this.comboBoxDisk.Name = "comboBoxDisk";
            this.comboBoxDisk.Size = new System.Drawing.Size(600, 23);
            this.comboBoxDisk.TabIndex = 1;
            // 
            // textBoxContent
            // 
            this.textBoxContent.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxContent.Enabled = false;
            this.textBoxContent.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.textBoxContent.Location = new System.Drawing.Point(12, 120);
            this.textBoxContent.Name = "textBoxContent";
            this.textBoxContent.Size = new System.Drawing.Size(600, 21);
            this.textBoxContent.TabIndex = 2;
            // 
            // labelContent
            // 
            this.labelContent.AutoSize = true;
            this.labelContent.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.labelContent.Location = new System.Drawing.Point(12, 102);
            this.labelContent.Name = "labelContent";
            this.labelContent.Size = new System.Drawing.Size(133, 15);
            this.labelContent.TabIndex = 8;
            this.labelContent.Text = "Text to search in file:";
            this.labelContent.Visible = false;
            // 
            // checkBoxSearchInContent
            // 
            this.checkBoxSearchInContent.AutoSize = true;
            this.checkBoxSearchInContent.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.checkBoxSearchInContent.Location = new System.Drawing.Point(630, 120);
            this.checkBoxSearchInContent.Name = "checkBoxSearchInContent";
            this.checkBoxSearchInContent.Size = new System.Drawing.Size(158, 19);
            this.checkBoxSearchInContent.TabIndex = 9;
            this.checkBoxSearchInContent.Text = "Search in content";
            this.checkBoxSearchInContent.UseVisualStyleBackColor = true;
            this.checkBoxSearchInContent.CheckedChanged += new System.EventHandler(this.checkBoxSearchInContent_CheckedChanged);
            // 
            // buttonPause
            // 
            this.buttonPause.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonPause.Enabled = false;
            this.buttonPause.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.buttonPause.Location = new System.Drawing.Point(630, 75);
            this.buttonPause.Name = "buttonPause";
            this.buttonPause.Size = new System.Drawing.Size(158, 35);
            this.buttonPause.TabIndex = 10;
            this.buttonPause.Text = "Pause";
            this.buttonPause.UseVisualStyleBackColor = true;
            this.buttonPause.Click += new System.EventHandler(this.buttonPause_Click);
            // 
            // checkBoxSearchInSubdirectories
            // 
            this.checkBoxSearchInSubdirectories.AutoSize = true;
            this.checkBoxSearchInSubdirectories.Checked = true;
            this.checkBoxSearchInSubdirectories.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxSearchInSubdirectories.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.checkBoxSearchInSubdirectories.Location = new System.Drawing.Point(12, 102);
            this.checkBoxSearchInSubdirectories.Name = "checkBoxSearchInSubdirectories";
            this.checkBoxSearchInSubdirectories.Size = new System.Drawing.Size(158, 19);
            this.checkBoxSearchInSubdirectories.TabIndex = 11;
            this.checkBoxSearchInSubdirectories.Text = "Search in subdirectories";
            this.checkBoxSearchInSubdirectories.UseVisualStyleBackColor = true;
            // 
            // listViewResults
            // 
            this.listViewResults.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewResults.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.listViewResults.FullRowSelect = true;
            this.listViewResults.GridLines = true;
            this.listViewResults.HideSelection = false;
            this.listViewResults.Location = new System.Drawing.Point(12, 150);
            this.listViewResults.Name = "listViewResults";
            this.listViewResults.Size = new System.Drawing.Size(776, 288);
            this.listViewResults.TabIndex = 3;
            // 
            // checkBoxSearchInSubdirectories
            // 
            this.checkBoxSearchInSubdirectories.AutoSize = true;
            this.checkBoxSearchInSubdirectories.Checked = true;
            this.checkBoxSearchInSubdirectories.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxSearchInSubdirectories.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.checkBoxSearchInSubdirectories.Location = new System.Drawing.Point(180, 102);
            this.checkBoxSearchInSubdirectories.Name = "checkBoxSearchInSubdirectories";
            this.checkBoxSearchInSubdirectories.Size = new System.Drawing.Size(158, 19);
            this.checkBoxSearchInSubdirectories.TabIndex = 11;
            this.checkBoxSearchInSubdirectories.Text = "Search in subdirectories";
            this.checkBoxSearchInSubdirectories.UseVisualStyleBackColor = true;
            this.listViewResults.UseCompatibleStateImageBehavior = false;
            // 
            // buttonSearch
            // 
            this.buttonSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSearch.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.buttonSearch.Location = new System.Drawing.Point(630, 30);
            this.buttonSearch.Name = "buttonSearch";
            this.buttonSearch.Size = new System.Drawing.Size(158, 68);
            this.buttonSearch.TabIndex = 4;
            this.buttonSearch.Text = "Search";
            this.buttonSearch.UseVisualStyleBackColor = true;
            this.buttonSearch.Click += new System.EventHandler(this.buttonSearch_Click);
            // 
            // labelMask
            // 
            this.labelMask.AutoSize = true;
            this.labelMask.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.labelMask.Location = new System.Drawing.Point(12, 12);
            this.labelMask.Name = "labelMask";
            this.labelMask.Size = new System.Drawing.Size(40, 15);
            this.labelMask.TabIndex = 4;
            this.labelMask.Text = "Mask:";
            // 
            // labelDisk
            // 
            this.labelDisk.AutoSize = true;
            this.labelDisk.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.labelDisk.Location = new System.Drawing.Point(12, 57);
            this.labelDisk.Name = "labelDisk";
            this.labelDisk.Size = new System.Drawing.Size(33, 15);
            this.labelDisk.TabIndex = 5;
            this.labelDisk.Text = "Disk:";
            // 
            // labelStatus
            // 
            this.labelStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.labelStatus.Location = new System.Drawing.Point(12, 448);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(600, 23);
            this.labelStatus.TabIndex = 6;
            this.labelStatus.Text = "Ready to search";
            // 
            // progressBar
            // 
            this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar.Location = new System.Drawing.Point(12, 448);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(600, 23);
            this.progressBar.TabIndex = 7;
            this.progressBar.Visible = false;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 480);
            this.Controls.Add(this.checkBoxSearchInSubdirectories);
            this.Controls.Add(this.buttonPause);
            this.Controls.Add(this.checkBoxSearchInContent);
            this.Controls.Add(this.labelContent);
            this.Controls.Add(this.textBoxContent);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.labelStatus);
            this.Controls.Add(this.labelDisk);
            this.Controls.Add(this.labelMask);
            this.Controls.Add(this.buttonSearch);
            this.Controls.Add(this.listViewResults);
            this.Controls.Add(this.comboBoxDisk);
            this.Controls.Add(this.textBoxMask);
            this.MinimumSize = new System.Drawing.Size(600, 400);
            this.Name = "MainForm";
            this.Text = "File and Folder Search";
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}

