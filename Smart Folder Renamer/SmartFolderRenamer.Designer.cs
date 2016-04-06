using System;

namespace Smart_Folder_Renamer
{
    partial class SmartFolderRenamer
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing && (components != null))
                {
                    components.Dispose();
                }
                base.Dispose(disposing);
            }
            catch(Exception ex)
            {

            }
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnSetSourceFolder = new System.Windows.Forms.Button();
            this.txtSearchName = new System.Windows.Forms.TextBox();
            this.txtDestFolder = new System.Windows.Forms.TextBox();
            this.btnSetDestFolder = new System.Windows.Forms.Button();
            this.btnRenameFolders = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.dlgFolderBrowser = new System.Windows.Forms.FolderBrowserDialog();
            this.lblElapsed = new System.Windows.Forms.Label();
            this.lblElapsedValue = new System.Windows.Forms.Label();
            this.lblDurationPerFolderValue = new System.Windows.Forms.Label();
            this.lblDurationPerFolder = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lblFoldersProcessedVal = new System.Windows.Forms.Label();
            this.lblFoldersProcessed = new System.Windows.Forms.Label();
            this.lblTimeRemainingValue = new System.Windows.Forms.Label();
            this.lblTimeRemaining = new System.Windows.Forms.Label();
            this.lblErrorsVal = new System.Windows.Forms.Label();
            this.lblErrors = new System.Windows.Forms.Label();
            this.btnBusiness = new System.Windows.Forms.Button();
            this.chkMoveFolders = new System.Windows.Forms.CheckBox();
            this.txtSourceFolder = new System.Windows.Forms.TextBox();
            this.lblSearchName = new System.Windows.Forms.Label();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.btnHandlePreviousRenamed = new System.Windows.Forms.Button();
            this.txtCustomRename = new System.Windows.Forms.TextBox();
            this.chkCustomRename = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // btnSetSourceFolder
            // 
            this.btnSetSourceFolder.Location = new System.Drawing.Point(8, 48);
            this.btnSetSourceFolder.Margin = new System.Windows.Forms.Padding(2);
            this.btnSetSourceFolder.Name = "btnSetSourceFolder";
            this.btnSetSourceFolder.Size = new System.Drawing.Size(114, 30);
            this.btnSetSourceFolder.TabIndex = 0;
            this.btnSetSourceFolder.Text = "Set Source Folder";
            this.btnSetSourceFolder.UseVisualStyleBackColor = true;
            this.btnSetSourceFolder.Click += new System.EventHandler(this.btnSetSourceFolder_Click);
            // 
            // txtSearchName
            // 
            this.txtSearchName.Location = new System.Drawing.Point(148, 11);
            this.txtSearchName.Margin = new System.Windows.Forms.Padding(2);
            this.txtSearchName.Name = "txtSearchName";
            this.txtSearchName.Size = new System.Drawing.Size(365, 20);
            this.txtSearchName.TabIndex = 1;
            // 
            // txtDestFolder
            // 
            this.txtDestFolder.Enabled = false;
            this.txtDestFolder.Location = new System.Drawing.Point(127, 114);
            this.txtDestFolder.Margin = new System.Windows.Forms.Padding(2);
            this.txtDestFolder.Name = "txtDestFolder";
            this.txtDestFolder.Size = new System.Drawing.Size(387, 20);
            this.txtDestFolder.TabIndex = 3;
            // 
            // btnSetDestFolder
            // 
            this.btnSetDestFolder.Enabled = false;
            this.btnSetDestFolder.Location = new System.Drawing.Point(8, 109);
            this.btnSetDestFolder.Margin = new System.Windows.Forms.Padding(2);
            this.btnSetDestFolder.Name = "btnSetDestFolder";
            this.btnSetDestFolder.Size = new System.Drawing.Size(114, 30);
            this.btnSetDestFolder.TabIndex = 2;
            this.btnSetDestFolder.Text = "Set Dest. Folder";
            this.btnSetDestFolder.UseVisualStyleBackColor = true;
            this.btnSetDestFolder.Click += new System.EventHandler(this.btnSetDestFolder_Click);
            // 
            // btnRenameFolders
            // 
            this.btnRenameFolders.Location = new System.Drawing.Point(8, 197);
            this.btnRenameFolders.Margin = new System.Windows.Forms.Padding(2);
            this.btnRenameFolders.Name = "btnRenameFolders";
            this.btnRenameFolders.Size = new System.Drawing.Size(248, 24);
            this.btnRenameFolders.TabIndex = 6;
            this.btnRenameFolders.Text = "Rename Folders";
            this.btnRenameFolders.UseVisualStyleBackColor = true;
            this.btnRenameFolders.Click += new System.EventHandler(this.btnRenameFolders_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(9, 250);
            this.progressBar1.Margin = new System.Windows.Forms.Padding(2);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(504, 19);
            this.progressBar1.TabIndex = 7;
            // 
            // lblElapsed
            // 
            this.lblElapsed.AutoSize = true;
            this.lblElapsed.Location = new System.Drawing.Point(48, 172);
            this.lblElapsed.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblElapsed.Name = "lblElapsed";
            this.lblElapsed.Size = new System.Drawing.Size(74, 13);
            this.lblElapsed.TabIndex = 8;
            this.lblElapsed.Text = "Elapsed Time:";
            // 
            // lblElapsedValue
            // 
            this.lblElapsedValue.AutoSize = true;
            this.lblElapsedValue.Location = new System.Drawing.Point(124, 172);
            this.lblElapsedValue.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblElapsedValue.Name = "lblElapsedValue";
            this.lblElapsedValue.Size = new System.Drawing.Size(49, 13);
            this.lblElapsedValue.TabIndex = 9;
            this.lblElapsedValue.Text = "00:00:00";
            // 
            // lblDurationPerFolderValue
            // 
            this.lblDurationPerFolderValue.AutoSize = true;
            this.lblDurationPerFolderValue.Location = new System.Drawing.Point(423, 172);
            this.lblDurationPerFolderValue.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblDurationPerFolderValue.Name = "lblDurationPerFolderValue";
            this.lblDurationPerFolderValue.Size = new System.Drawing.Size(70, 13);
            this.lblDurationPerFolderValue.TabIndex = 11;
            this.lblDurationPerFolderValue.Text = "00:00:00.000";
            // 
            // lblDurationPerFolder
            // 
            this.lblDurationPerFolder.AutoSize = true;
            this.lblDurationPerFolder.Location = new System.Drawing.Point(318, 172);
            this.lblDurationPerFolder.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblDurationPerFolder.Name = "lblDurationPerFolder";
            this.lblDurationPerFolder.Size = new System.Drawing.Size(101, 13);
            this.lblDurationPerFolder.TabIndex = 10;
            this.lblDurationPerFolder.Text = "Duration Per Folder:";
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(265, 197);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(2);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(248, 24);
            this.btnCancel.TabIndex = 12;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // lblFoldersProcessedVal
            // 
            this.lblFoldersProcessedVal.AutoSize = true;
            this.lblFoldersProcessedVal.Location = new System.Drawing.Point(126, 151);
            this.lblFoldersProcessedVal.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblFoldersProcessedVal.Name = "lblFoldersProcessedVal";
            this.lblFoldersProcessedVal.Size = new System.Drawing.Size(24, 13);
            this.lblFoldersProcessedVal.TabIndex = 14;
            this.lblFoldersProcessedVal.Text = "0/0";
            // 
            // lblFoldersProcessed
            // 
            this.lblFoldersProcessed.AutoSize = true;
            this.lblFoldersProcessed.Location = new System.Drawing.Point(25, 151);
            this.lblFoldersProcessed.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblFoldersProcessed.Name = "lblFoldersProcessed";
            this.lblFoldersProcessed.Size = new System.Drawing.Size(97, 13);
            this.lblFoldersProcessed.TabIndex = 13;
            this.lblFoldersProcessed.Text = "Folders Processed:";
            // 
            // lblTimeRemainingValue
            // 
            this.lblTimeRemainingValue.AutoSize = true;
            this.lblTimeRemainingValue.Location = new System.Drawing.Point(95, 234);
            this.lblTimeRemainingValue.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblTimeRemainingValue.Name = "lblTimeRemainingValue";
            this.lblTimeRemainingValue.Size = new System.Drawing.Size(70, 13);
            this.lblTimeRemainingValue.TabIndex = 18;
            this.lblTimeRemainingValue.Text = "00:00:00.000";
            // 
            // lblTimeRemaining
            // 
            this.lblTimeRemaining.AutoSize = true;
            this.lblTimeRemaining.Location = new System.Drawing.Point(8, 234);
            this.lblTimeRemaining.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblTimeRemaining.Name = "lblTimeRemaining";
            this.lblTimeRemaining.Size = new System.Drawing.Size(86, 13);
            this.lblTimeRemaining.TabIndex = 17;
            this.lblTimeRemaining.Text = "Time Remaining:";
            // 
            // lblErrorsVal
            // 
            this.lblErrorsVal.AutoSize = true;
            this.lblErrorsVal.Location = new System.Drawing.Point(423, 149);
            this.lblErrorsVal.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblErrorsVal.Name = "lblErrorsVal";
            this.lblErrorsVal.Size = new System.Drawing.Size(13, 13);
            this.lblErrorsVal.TabIndex = 24;
            this.lblErrorsVal.Text = "0";
            // 
            // lblErrors
            // 
            this.lblErrors.AutoSize = true;
            this.lblErrors.Location = new System.Drawing.Point(380, 149);
            this.lblErrors.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblErrors.Name = "lblErrors";
            this.lblErrors.Size = new System.Drawing.Size(37, 13);
            this.lblErrors.TabIndex = 23;
            this.lblErrors.Text = "Errors:";
            // 
            // btnBusiness
            // 
            this.btnBusiness.Enabled = false;
            this.btnBusiness.Location = new System.Drawing.Point(368, 151);
            this.btnBusiness.Margin = new System.Windows.Forms.Padding(2);
            this.btnBusiness.Name = "btnBusiness";
            this.btnBusiness.Size = new System.Drawing.Size(8, 11);
            this.btnBusiness.TabIndex = 25;
            this.btnBusiness.UseVisualStyleBackColor = true;
            this.btnBusiness.Visible = false;
            this.btnBusiness.Click += new System.EventHandler(this.btnBusiness_Click);
            // 
            // chkMoveFolders
            // 
            this.chkMoveFolders.AutoSize = true;
            this.chkMoveFolders.Location = new System.Drawing.Point(12, 84);
            this.chkMoveFolders.Name = "chkMoveFolders";
            this.chkMoveFolders.Size = new System.Drawing.Size(139, 17);
            this.chkMoveFolders.TabIndex = 26;
            this.chkMoveFolders.Text = "Move Renamed Folders";
            this.chkMoveFolders.UseVisualStyleBackColor = true;
            this.chkMoveFolders.CheckedChanged += new System.EventHandler(this.chkMoveFolders_CheckedChanged);
            // 
            // txtSourceFolder
            // 
            this.txtSourceFolder.Location = new System.Drawing.Point(129, 54);
            this.txtSourceFolder.Name = "txtSourceFolder";
            this.txtSourceFolder.Size = new System.Drawing.Size(385, 20);
            this.txtSourceFolder.TabIndex = 27;
            // 
            // lblSearchName
            // 
            this.lblSearchName.AutoSize = true;
            this.lblSearchName.Location = new System.Drawing.Point(11, 14);
            this.lblSearchName.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblSearchName.Name = "lblSearchName";
            this.lblSearchName.Size = new System.Drawing.Size(133, 13);
            this.lblSearchName.TabIndex = 28;
            this.lblSearchName.Text = "Matching SubFolder Name";
            // 
            // btnHandlePreviousRenamed
            // 
            this.btnHandlePreviousRenamed.Enabled = false;
            this.btnHandlePreviousRenamed.Location = new System.Drawing.Point(356, 149);
            this.btnHandlePreviousRenamed.Margin = new System.Windows.Forms.Padding(2);
            this.btnHandlePreviousRenamed.Name = "btnHandlePreviousRenamed";
            this.btnHandlePreviousRenamed.Size = new System.Drawing.Size(8, 11);
            this.btnHandlePreviousRenamed.TabIndex = 29;
            this.btnHandlePreviousRenamed.UseVisualStyleBackColor = true;
            this.btnHandlePreviousRenamed.Visible = false;
            this.btnHandlePreviousRenamed.Click += new System.EventHandler(this.btnHandlePreviousRenamed_Click);
            // 
            // txtCustomRename
            // 
            this.txtCustomRename.Enabled = false;
            this.txtCustomRename.Location = new System.Drawing.Point(321, 82);
            this.txtCustomRename.Name = "txtCustomRename";
            this.txtCustomRename.Size = new System.Drawing.Size(193, 20);
            this.txtCustomRename.TabIndex = 30;
            // 
            // chkCustomRename
            // 
            this.chkCustomRename.AutoSize = true;
            this.chkCustomRename.Enabled = false;
            this.chkCustomRename.Location = new System.Drawing.Point(157, 84);
            this.chkCustomRename.Name = "chkCustomRename";
            this.chkCustomRename.Size = new System.Drawing.Size(158, 17);
            this.chkCustomRename.TabIndex = 31;
            this.chkCustomRename.Text = "Custom Rename and Merge";
            this.chkCustomRename.UseVisualStyleBackColor = true;
            this.chkCustomRename.CheckedChanged += new System.EventHandler(this.chkCustomRename_CheckedChanged);
            // 
            // SmartFolderRenamer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(524, 282);
            this.Controls.Add(this.chkCustomRename);
            this.Controls.Add(this.txtCustomRename);
            this.Controls.Add(this.btnHandlePreviousRenamed);
            this.Controls.Add(this.lblSearchName);
            this.Controls.Add(this.txtSourceFolder);
            this.Controls.Add(this.chkMoveFolders);
            this.Controls.Add(this.btnBusiness);
            this.Controls.Add(this.lblErrorsVal);
            this.Controls.Add(this.lblErrors);
            this.Controls.Add(this.lblTimeRemainingValue);
            this.Controls.Add(this.lblTimeRemaining);
            this.Controls.Add(this.lblFoldersProcessedVal);
            this.Controls.Add(this.lblFoldersProcessed);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.lblDurationPerFolderValue);
            this.Controls.Add(this.lblDurationPerFolder);
            this.Controls.Add(this.lblElapsedValue);
            this.Controls.Add(this.lblElapsed);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.btnRenameFolders);
            this.Controls.Add(this.txtDestFolder);
            this.Controls.Add(this.btnSetDestFolder);
            this.Controls.Add(this.txtSearchName);
            this.Controls.Add(this.btnSetSourceFolder);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.Name = "SmartFolderRenamer";
            this.Text = "Smart Folder Renamer";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSetSourceFolder;
        private System.Windows.Forms.TextBox txtSearchName;
        private System.Windows.Forms.TextBox txtDestFolder;
        private System.Windows.Forms.Button btnSetDestFolder;
        private System.Windows.Forms.Button btnRenameFolders;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.FolderBrowserDialog dlgFolderBrowser;
        private System.Windows.Forms.Label lblElapsed;
        private System.Windows.Forms.Label lblElapsedValue;
        private System.Windows.Forms.Label lblDurationPerFolderValue;
        private System.Windows.Forms.Label lblDurationPerFolder;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label lblFoldersProcessedVal;
        private System.Windows.Forms.Label lblFoldersProcessed;
        private System.Windows.Forms.Label lblTimeRemainingValue;
        private System.Windows.Forms.Label lblTimeRemaining;
        private System.Windows.Forms.Label lblErrorsVal;
        private System.Windows.Forms.Label lblErrors;
        private System.Windows.Forms.Button btnBusiness;
        private System.Windows.Forms.CheckBox chkMoveFolders;
        private System.Windows.Forms.TextBox txtSourceFolder;
        private System.Windows.Forms.Label lblSearchName;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Button btnHandlePreviousRenamed;
        private System.Windows.Forms.TextBox txtCustomRename;
        private System.Windows.Forms.CheckBox chkCustomRename;
    }
}

