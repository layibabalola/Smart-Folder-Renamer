using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Management;

namespace Smart_Folder_Renamer
{
    public partial class SmartFolderRenamer : Form
    {
        int errorCount = 0;
        int foldersRenamed = 0;
        int foldersMoved = 0;

        long maxDirQueue = 0;
        long dirQueueBeforeEmpty = 0;

        double percentComplete = 0;

        bool moveRenamedFolders = false;
        bool customRename = false;
        bool formClosed = false;
        bool preparing = false;
        bool setControlsBusy = false;
        bool workCompleted = false;
        bool isResting = true;

        static ConcurrentQueue<DirectoryInfo> dirQueue = new ConcurrentQueue<DirectoryInfo>();
        static ConcurrentQueue<DirectoryInfo> previouslyRenamedQueue = new ConcurrentQueue<DirectoryInfo>();
        static ConcurrentQueue<DirectoryInfo> recentlyRenamedQueue = new ConcurrentQueue<DirectoryInfo>();

        BackgroundWorker bwBusiness;
        Stopwatch swCanceling = new System.Diagnostics.Stopwatch();
        Stopwatch swPreparing = new System.Diagnostics.Stopwatch();
        Stopwatch cwStopwatch = new Stopwatch();
        Stopwatch swPBar = new Stopwatch();
        BackgroundWorker bwProgressBar = new BackgroundWorker();
        BackgroundWorker bwPreparing = new BackgroundWorker();
        BackgroundWorker bwCumulativeStopWatch = new BackgroundWorker();
        BackgroundWorker bwCancel = new BackgroundWorker();
        BackgroundWorker bwThreads = new BackgroundWorker();

        string elapsedTime = "00:00:00";
        string timeRemaining = "00:00:00";
        string durationPerFolder = "00:00:00.000";
        string rename = "Preparing";
        string initialDirectory = string.Empty;
        string destDirectory = string.Empty;
        string currentDirectory = string.Empty;

        string searchName = string.Empty;
        string customRenameText = string.Empty;

        Dictionary<string, int> parentFolders = new Dictionary<string, int>();

        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(String sClassName, String sAppName);
        [DllImport("user32.dll")]
        static extern IntPtr FindWindow(IntPtr classname, string title);


        public SmartFolderRenamer()
        {
            InitializeComponent();

            this.CenterToScreen();

            if (!string.IsNullOrEmpty(Smart_Folder_Renamer.Properties.Settings.Default.SearchName))
            {
                txtSearchName.Text = Smart_Folder_Renamer.Properties.Settings.Default.SearchName;
            }
            if (!string.IsNullOrEmpty(Smart_Folder_Renamer.Properties.Settings.Default.SourceFolder))
            {
                txtSourceFolder.Text = Smart_Folder_Renamer.Properties.Settings.Default.SourceFolder;
            }
            if (!string.IsNullOrEmpty(Smart_Folder_Renamer.Properties.Settings.Default.DestinationFolder))
            {
                txtDestFolder.Text = Smart_Folder_Renamer.Properties.Settings.Default.DestinationFolder;
            }
            if (!string.IsNullOrEmpty(Smart_Folder_Renamer.Properties.Settings.Default.CustomRenameText))
            {
                txtCustomRename.Text = Smart_Folder_Renamer.Properties.Settings.Default.CustomRenameText;
            }
            chkMoveFolders.Checked = Smart_Folder_Renamer.Properties.Settings.Default.MoveRenamedFolders;
            moveRenamedFolders = chkMoveFolders.Checked;
            chkCustomRename.Checked = Smart_Folder_Renamer.Properties.Settings.Default.CustomRename;
            customRename = chkCustomRename.Checked;

            chkCustomRename_CheckedChanged(this, new EventArgs());

            bwCumulativeStopWatch.DoWork += BwCumulativeStopWatch_DoWork;
            bwCumulativeStopWatch.WorkerSupportsCancellation = true;

            bwProgressBar.DoWork += BwProgressBar_DoWork;
            bwProgressBar.WorkerSupportsCancellation = true;

            bwPreparing.DoWork += BwPreparing_DoWork;
            bwPreparing.WorkerSupportsCancellation = true;

            bwCancel.DoWork += BwCancel_DoWork;
            bwCancel.WorkerSupportsCancellation = true;

            bwThreads.DoWork += BwThreads_DoWork;
            bwThreads.ProgressChanged += BwThreads_ProgressChanged;
            bwThreads.WorkerReportsProgress = true;
            bwThreads.WorkerSupportsCancellation = true;

            bwBusiness = new BackgroundWorker();
            bwBusiness.DoWork += BwBusiness_DoWork;
            bwBusiness.WorkerSupportsCancellation = true;

            this.FormClosing += SmartFolderRenamer_FormClosing;
            this.FormClosed += SmartFolderRenamer_FormClosed;

            foreach (Control c in this.Controls)
            {
                c.EnabledChanged += Control_EnabledChanged;
            }

            bwThreads.RunWorkerAsync();
        }

        void FindAndMoveMsgBox(string title, int x, int y)
        {
            //Moves a messagebox to the desired position
            Thread thr = new Thread(() => // create a new thread
            {
                IntPtr msgBox = IntPtr.Zero;
                // while there's no MessageBox, FindWindow returns IntPtr.Zero
                while ((msgBox = FindWindow(IntPtr.Zero, title)) == IntPtr.Zero) ;
                // after the while loop, msgBox is the handle of your MessageBox
                Rectangle r = new Rectangle();

                ManagedWinapi.Windows.SystemWindow msgBoxWindow = new ManagedWinapi.Windows.SystemWindow(msgBox);
                msgBoxWindow.Location = new Point(x, y);
            });
            thr.IsBackground = true;
            thr.Start(); // starts the thread
        }

        private void Control_EnabledChanged(object sender, EventArgs e)
        {
            Control c = (Control)sender;

            if (c.GetType() == typeof(Button))
            {
                Button b = ((Button)sender);
                if (b.Enabled)
                {
                    b.ForeColor = Button.DefaultForeColor;
                    b.BackColor = Button.DefaultBackColor;
                }
                else
                {
                    b.BackColor = Color.AliceBlue;
                }
            }
        }

        private void SmartFolderRenamer_FormClosing(object sender, FormClosingEventArgs e)
        {
            Smart_Folder_Renamer.Properties.Settings.Default.SearchName = txtSearchName.Text;

            Smart_Folder_Renamer.Properties.Settings.Default.SourceFolder = txtSourceFolder.Text;

            Smart_Folder_Renamer.Properties.Settings.Default.DestinationFolder = txtDestFolder.Text;

            Smart_Folder_Renamer.Properties.Settings.Default.CustomRename = chkCustomRename.Checked;

            Smart_Folder_Renamer.Properties.Settings.Default.CustomRenameText = txtCustomRename.Text;

            Smart_Folder_Renamer.Properties.Settings.Default.MoveRenamedFolders = chkMoveFolders.Checked;

            Smart_Folder_Renamer.Properties.Settings.Default.Save();

            formClosed = true;
        }

        private void SmartFolderRenamer_FormClosed(object sender, FormClosedEventArgs e)
        {
            formClosed = true;
        }

        private void SetControlValues(ProgressChangedEventArgs e)
        {
            try
            {
                setControlsBusy = true;
                progressBar1.Value = e.ProgressPercentage;

                if (e.UserState.GetType() != typeof(List<Control>))
                {
                    Control c = (Control)e.UserState;

                    if (c.GetType() == typeof(Button))
                    {
                        Button btnSource = ((Button)c);
                        if (((Button)(Controls.Find(btnSource.Name, true).FirstOrDefault())) != null)
                        {
                            Button btnDest = ((Button)(Controls.Find(btnSource.Name, true).FirstOrDefault()));
                            btnDest.Text = btnSource.Text;
                            btnDest.Enabled = btnSource.Enabled;
                            btnDest.ForeColor = btnSource.ForeColor;
                        }
                    }
                    else if (c.GetType() == typeof(Label))
                    {
                        Label lblSource = ((Label)c);
                        if (((Label)(Controls.Find(lblSource.Name, true).FirstOrDefault())) != null)
                        {
                            Label lblDest = ((Label)(Controls.Find(lblSource.Name, true).FirstOrDefault()));
                            lblDest.Text = lblSource.Text;
                            lblDest.Enabled = lblSource.Enabled;
                            lblDest.ForeColor = lblSource.ForeColor;
                        }
                    }
                    else if (c.GetType() == typeof(TextBox))
                    {
                        TextBox txtSource = ((TextBox)c);
                        if (((TextBox)(Controls.Find(txtSource.Name, true).FirstOrDefault())) != null)
                        {
                            TextBox txtDest = ((TextBox)(Controls.Find(txtSource.Name, true).FirstOrDefault()));
                            txtDest.Text = txtSource.Text;
                            txtDest.Enabled = txtSource.Enabled;
                            txtDest.ForeColor = txtSource.ForeColor;

                            if (!isResting)
                            {
                                txtDest.SelectionStart = txtSource.SelectionStart;
                                txtDest.SelectionLength = txtSource.SelectionLength;
                            }
                        }
                    }
                }
                else if (e.UserState.GetType() == typeof(List<Control>))
                {
                    List<Control> controlList = ((List<Control>)(e.UserState));
                    if (controlList.Count() > 0)
                    {
                        foreach (Control c in controlList)
                        {
                            if (c.GetType() == typeof(Button))
                            {
                                Button btnSource = ((Button)c);
                                if (((Button)(Controls.Find(btnSource.Name, true).FirstOrDefault())) != null)
                                {
                                    Button btnDest = ((Button)(Controls.Find(btnSource.Name, true).FirstOrDefault()));
                                    btnDest.Text = btnSource.Text;
                                    btnDest.Enabled = btnSource.Enabled;
                                    btnDest.Enabled = btnSource.Enabled;
                                    btnDest.ForeColor = btnSource.ForeColor;
                                }
                            }
                            else if (c.GetType() == typeof(Label))
                            {
                                Label lblSource = ((Label)c);
                                if (((Label)(Controls.Find(lblSource.Name, true).FirstOrDefault())) != null)
                                {
                                    Label lblDest = ((Label)(Controls.Find(lblSource.Name, true).FirstOrDefault()));
                                    lblDest.Text = lblSource.Text;
                                    lblDest.Enabled = lblSource.Enabled;
                                    lblDest.ForeColor = lblSource.ForeColor;
                                }
                            }
                            else if (c.GetType() == typeof(TextBox))
                            {
                                TextBox txtSource = ((TextBox)c);
                                if (((TextBox)(Controls.Find(txtSource.Name, true).FirstOrDefault())) != null)
                                {
                                    TextBox txtDest = ((TextBox)(Controls.Find(txtSource.Name, true).FirstOrDefault()));
                                    txtDest.Text = txtSource.Text;
                                    txtDest.Enabled = txtSource.Enabled;
                                    txtDest.ForeColor = txtSource.ForeColor;

                                    if (!isResting)
                                    {
                                        txtDest.SelectionStart = txtSource.SelectionStart;
                                        txtDest.SelectionLength = txtSource.SelectionLength;
                                    }
                                }
                            }
                        }
                    }

                    setControlsBusy = false;
                }
            }
            catch (Exception ex)
            {
                errorCount++;
                if (Directory.Exists(txtDestFolder.Text))
                {
                    File.WriteAllText(txtDestFolder.Text + "\\ExceptionLog." + Guid.NewGuid().ToString() + ".log", ex.Message + "\r\n" + ex.StackTrace);
                }
            }
        }

        private void BwCancel_DoWork(object sender, DoWorkEventArgs e)
        {
            percentComplete = 1;
            swCanceling.Start();

            while (!((BackgroundWorker)(sender)).CancellationPending)
            {
                if (percentComplete < 100)
                {
                    percentComplete = percentComplete = .1;
                }
            }

            if (bwCumulativeStopWatch.IsBusy)
            {
                bwCumulativeStopWatch.CancelAsync();
            }

            //Set all globals to original state
            dirQueue = new ConcurrentQueue<DirectoryInfo>();
            maxDirQueue = 0; ;
            currentDirectory = initialDirectory;

            if (moveRenamedFolders && previouslyRenamedQueue.Count() > 0)
            {
                btnHandlePreviousRenamed_Click(this, new EventArgs());
            }
        }

        private void BwThreads_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            SetControlValues(e);
        }

        private void BwThreads_DoWork(object sender, DoWorkEventArgs e)
        {
            do
            {
                bool businessBusy = false;

                if (preparing)
                {
                    List<Control> controlList = new List<Control>();
                    isResting = false;

                    Button btnP = new Button();
                    btnP.Name = btnRenameFolders.Name;
                    btnP.Text = rename;
                    btnP.Enabled = false;

                    controlList.Add(btnP);

                    Button btnC = new Button();
                    btnC.Name = btnCancel.Name;
                    btnC.Text = btnCancel.Text;
                    btnC.Enabled = btnCancel.Enabled;

                    controlList.Add(btnC);

                    Label lblImgProc = new Label();
                    lblImgProc.Name = lblFoldersProcessedVal.Name;
                    lblImgProc.Text = lblFoldersProcessedVal.Text;
                    lblImgProc.Enabled = lblFoldersProcessedVal.Enabled;

                    controlList.Add(lblImgProc);

                    Label lblElapVal = new Label();
                    lblElapVal.Name = lblElapsedValue.Name;
                    lblElapVal.Text = lblElapsedValue.Text;
                    lblElapVal.Enabled = lblElapsedValue.Enabled;

                    controlList.Add(lblElapVal);

                    Label lblDurationPerImg = new Label();
                    lblDurationPerImg.Name = lblDurationPerFolderValue.Name;
                    lblDurationPerImg.Text = lblDurationPerFolderValue.Text;
                    lblDurationPerImg.Enabled = lblDurationPerFolderValue.Enabled;

                    controlList.Add(lblDurationPerImg);

                    Label lblTRemainVal = new Label();
                    lblTRemainVal.Name = lblTimeRemainingValue.Name;
                    lblTRemainVal.Text = lblTimeRemainingValue.Text;
                    lblTRemainVal.Enabled = lblTimeRemainingValue.Enabled;

                    controlList.Add(lblTRemainVal);

                    ProgressBar pBar = new ProgressBar();

                    if (!btnRenameFolders.Text.Contains("Preparing"))
                    {
                        btnP.Text = "Preparing";
                    }
                    else
                    {
                        //Change the progress bar once a second
                        if (swPBar.ElapsedMilliseconds > 1000)
                        {
                            int pBarVal = progressBar1.Value;

                            if (rename.Length < 30)
                            {
                                //Add dots every 5 seconds
                                if (pBarVal % 5 == 0)
                                {
                                    btnP.Text = rename += ".";
                                }
                            }

                            pBarVal -= 1;

                            if (pBarVal < 5)
                            {
                                pBarVal = 5;
                            }

                            pBar.Value = pBarVal;

                            swPBar.Reset();
                            swPBar.Start();
                        }
                        else
                        {
                            pBar.Value = progressBar1.Value;
                        }
                    }

                    if (progressBar1.Value == 0)
                    {
                        pBar.Value = 100;
                    }

                    lblImgProc.Text = "0/0";
                    btnP.Enabled = false;
                    btnC.Enabled = true;

                    Button btnSetDest = new Button();
                    btnSetDest.Name = btnSetDestFolder.Name;
                    btnSetDest.Text = btnSetDestFolder.Text.Replace("Set ", "");
                    btnSetDest.Enabled = false;

                    controlList.Add(btnSetDest);

                    Button btnSetSource = new Button();
                    btnSetSource.Name = btnSetSourceFolder.Name;
                    btnSetSource.Text = btnSetSourceFolder.Text.Replace("Set ", "");
                    btnSetSource.Enabled = false;

                    controlList.Add(btnSetSource);

                    TextBox txt = new TextBox();
                    txt.Name = txtDestFolder.Name;
                    txt.Text = txtDestFolder.Text;
                    txt.Enabled = false;

                    controlList.Add(txt);

                    TextBox txt3 = new TextBox();
                    txt3.Name = txtSearchName.Name;
                    txt3.Text = txtSearchName.Text;
                    txt3.Enabled = false;

                    controlList.Add(txt3);

                    ((BackgroundWorker)(sender)).ReportProgress(pBar.Value, controlList);
                }
                else
                {
                    //Set Buttons enabled or disabled
                    try
                    {
                        if (bwCancel.IsBusy)
                        {
                            //Cancelling
                            List<Control> controlList = new List<Control>();
                            isResting = false;

                            Label lblErr = new Label();
                            lblErr.Name = lblErrorsVal.Name;
                            lblErr.Text = errorCount.ToString();
                            if (errorCount > 0)
                            {
                                lblErr.ForeColor = Color.Red;
                            }

                            controlList.Add(lblErr);

                            Label lblImgProc = new Label();
                            lblImgProc.Name = lblFoldersProcessedVal.Name;
                            lblImgProc.Text = foldersRenamed.ToString();
                            lblImgProc.Enabled = lblFoldersProcessedVal.Enabled;

                            controlList.Add(lblImgProc);

                            Label lblElapVal = new Label();
                            lblElapVal.Name = lblElapsedValue.Name;
                            lblElapVal.Text = elapsedTime;
                            lblElapVal.Enabled = lblElapsedValue.Enabled;

                            controlList.Add(lblElapVal);

                            Label lblDurationPerImg = new Label();
                            lblDurationPerImg.Name = lblDurationPerFolderValue.Name;
                            lblDurationPerImg.Text = durationPerFolder;
                            lblDurationPerImg.Enabled = lblDurationPerFolderValue.Enabled;

                            controlList.Add(lblDurationPerImg);

                            Label lblTRemainVal = new Label();
                            lblTRemainVal.Name = lblTimeRemainingValue.Name;
                            lblTRemainVal.Text = timeRemaining;
                            lblTRemainVal.Enabled = lblTimeRemainingValue.Enabled;

                            controlList.Add(lblTRemainVal);

                            Button btnP = new Button();
                            btnP.Name = btnRenameFolders.Name;
                            btnP.Text = btnRenameFolders.Text;
                            btnP.Enabled = false;

                            controlList.Add(btnP);

                            ProgressBar pBar = new ProgressBar();
                            pBar.Name = progressBar1.Name;
                            pBar.Enabled = progressBar1.Enabled;
                            pBar.Value = progressBar1.Value;

                            Button btnCncl = new Button();
                            btnCncl.Name = btnCancel.Name;
                            btnCncl.Enabled = false;

                            if (!btnCancel.Text.Contains("Canceling"))
                            {
                                pBar.Value = 99;
                                btnCncl.Text = "Canceling";
                            }
                            else
                            {
                                string processText = "Rename Folders";
                                double valueSlices = 100;
                                int processSubStrSlices = (int)Math.Round((double)processText.Length);
                                pBar.Value = (int)(valueSlices);
                                btnP.Text = processText.Substring(
                                    (processSubStrSlices) > processText.Length ? processText.Length - 1 : (processSubStrSlices),
                                    processText.Length - (processSubStrSlices) < 0 ? 0 : processText.Length - (processSubStrSlices));

                                btnCncl.Text = "Canceling";
                            }

                            controlList.Add(btnCncl);
                            controlList.Add(pBar);

                            Button btnSetDest = new Button();
                            btnSetDest.Name = btnSetDestFolder.Name;
                            btnSetDest.Text = btnSetDestFolder.Text.Replace("Set ", "");
                            btnSetDest.Enabled = false;

                            controlList.Add(btnSetDest);

                            Button btnSetSource = new Button();
                            btnSetSource.Name = btnSetSourceFolder.Name;
                            if (maxDirQueue > 0)
                            {
                                btnSetSource.Text = (1 - ((double)dirQueue.Count() / (double)maxDirQueue)).ToString("##0.###%");
                            }
                            else
                            {
                                btnSetSource.Text = "0%";
                            }
                            btnSetSource.Enabled = false;

                            controlList.Add(btnSetSource);

                            TextBox txt = new TextBox();
                            txt.Name = txtDestFolder.Name;
                            txt.Text = txtDestFolder.Text;
                            if (initialDirectory != string.Empty)
                            {
                                txt.Text = initialDirectory;
                            }
                            txt.Enabled = false;

                            controlList.Add(txt);

                            TextBox txt3 = new TextBox();
                            txt3.Name = txtSearchName.Name;
                            txt3.Text = txtSearchName.Text;
                            txt3.Enabled = false;

                            controlList.Add(txt3);

                            ((BackgroundWorker)(sender)).ReportProgress(pBar.Value, controlList);
                        }
                        //Doing work. Set relevant UI disabled
                        else if ((bwCumulativeStopWatch.IsBusy || bwPreparing.IsBusy || bwProgressBar.IsBusy || businessBusy || setControlsBusy) &&
                            !bwCancel.IsBusy)
                        {
                            List<Control> controlList = new List<Control>();
                            isResting = false;

                            Label lblErr = new Label();
                            lblErr.Name = lblErrorsVal.Name;
                            lblErr.Text = errorCount.ToString();
                            if (errorCount > 0)
                            {
                                lblErr.ForeColor = Color.Red;
                            }

                            controlList.Add(lblErr);

                            Label lblImgProc = new Label();
                            lblImgProc.Name = lblFoldersProcessedVal.Name;
                            lblImgProc.Text = foldersRenamed.ToString() + "/" + maxDirQueue;
                            lblImgProc.Enabled = lblFoldersProcessedVal.Enabled;

                            controlList.Add(lblImgProc);

                            Label lblElapVal = new Label();
                            lblElapVal.Name = lblElapsedValue.Name;
                            lblElapVal.Text = elapsedTime;
                            lblElapVal.Enabled = lblElapsedValue.Enabled;

                            controlList.Add(lblElapVal);

                            Label lblDurationPerImg = new Label();
                            lblDurationPerImg.Name = lblDurationPerFolderValue.Name;
                            lblDurationPerImg.Text = durationPerFolder;
                            lblDurationPerImg.Enabled = lblDurationPerFolderValue.Enabled;

                            controlList.Add(lblDurationPerImg);

                            Label lblTRemainVal = new Label();
                            lblTRemainVal.Name = lblTimeRemainingValue.Name;
                            lblTRemainVal.Text = timeRemaining;
                            lblTRemainVal.Enabled = lblTimeRemainingValue.Enabled;

                            controlList.Add(lblTRemainVal);

                            Button btnP = new Button();
                            btnP.Name = btnRenameFolders.Name;
                            btnP.Text = percentComplete.ToString("##0.###%");
                            btnP.Enabled = false;

                            controlList.Add(btnP);

                            Button btnCncl = new Button();
                            btnCncl.Name = btnCancel.Name;
                            btnCncl.Text = btnCancel.Text;
                            btnCncl.Enabled = true;

                            controlList.Add(btnCncl);

                            Button btnSetDest = new Button();
                            btnSetDest.Name = btnSetDestFolder.Name;
                            btnSetDest.Text = btnSetDestFolder.Text.Replace("Set ", "");
                            btnSetDest.Enabled = false;

                            controlList.Add(btnSetDest);

                            Button btnSetSource = new Button();
                            btnSetSource.Name = btnSetSourceFolder.Name;
                            if (maxDirQueue > 0)
                            {
                                btnSetSource.Text = (1 - ((double)dirQueue.Count() / (double)maxDirQueue)).ToString("##0.###%");
                            }
                            else
                            {
                                btnSetSource.Text = "0%";
                            }
                            btnSetSource.Enabled = false;

                            controlList.Add(btnSetSource);

                            TextBox txt = new TextBox();
                            txt.Name = txtDestFolder.Name;
                            txt.Text = txtDestFolder.Text;
                            txt.Enabled = false;

                            controlList.Add(txt);

                            TextBox txt3 = new TextBox();
                            txt3.Name = txtSearchName.Name;
                            txt3.Text = txtSearchName.Text;
                            txt3.SelectionStart = txt3.Text.Length - 1;
                            txt3.SelectionLength = 0;
                            txt3.Enabled = false;

                            controlList.Add(txt3);

                            ((BackgroundWorker)(sender)).ReportProgress(Convert.ToInt32(percentComplete * 100), controlList);
                        }
                        else
                        {
                            //No longer doing work. Set everything back to resting state
                            List<Control> controlList = new List<Control>();
                            isResting = true;

                            Label lblErr = new Label();
                            lblErr.Name = lblErrorsVal.Name;
                            lblErr.Text = lblErrorsVal.Text;
                            lblErr.ForeColor = lblErrorsVal.ForeColor;

                            controlList.Add(lblErr);

                            Label lblImgProc = new Label();
                            lblImgProc.Name = lblFoldersProcessedVal.Name;
                            lblImgProc.Text = lblFoldersProcessedVal.Text;
                            lblImgProc.Enabled = lblFoldersProcessedVal.Enabled;

                            controlList.Add(lblImgProc);

                            Label lblElapVal = new Label();
                            lblElapVal.Name = lblElapsedValue.Name;
                            lblElapVal.Text = lblElapsedValue.Text;
                            lblElapVal.Enabled = lblElapsedValue.Enabled;

                            controlList.Add(lblElapVal);

                            Label lblDurationPerImg = new Label();
                            lblDurationPerImg.Name = lblDurationPerFolderValue.Name;
                            lblDurationPerImg.Text = lblDurationPerFolderValue.Text;
                            lblDurationPerImg.Enabled = lblDurationPerFolderValue.Enabled;

                            controlList.Add(lblDurationPerImg);

                            Label lblTRemainVal = new Label();
                            lblTRemainVal.Name = lblTimeRemainingValue.Name;
                            lblTRemainVal.Text = lblTimeRemainingValue.Text;
                            lblTRemainVal.Enabled = lblTimeRemainingValue.Enabled;

                            controlList.Add(lblTRemainVal);

                            Button btnP = new Button();
                            btnP.Name = btnRenameFolders.Name;
                            btnP.Text = "Rename Folders";
                            btnP.Enabled = true;

                            controlList.Add(btnP);

                            Button btnC = new Button();
                            btnC.Name = btnCancel.Name;
                            btnC.Text = "Cancel";
                            btnC.Enabled = false;

                            controlList.Add(btnC);

                            Button btn = new Button();
                            btn.Name = btnSetDestFolder.Name;
                            btn.Text = "Set Dest. Folder";
                            if (moveRenamedFolders)
                            {
                                btn.Enabled = true;
                            }
                            else
                            {
                                btn.Enabled = false;
                            }

                            controlList.Add(btn);

                            Button btn3 = new Button();
                            btn3.Name = btnSetSourceFolder.Name;
                            btn3.Text = "Set Source Folder";
                            btn3.Enabled = true;

                            controlList.Add(btn3);

                            TextBox txt = new TextBox();
                            txt.Name = txtDestFolder.Name;
                            txt.Text = txtDestFolder.Text;
                            if (moveRenamedFolders)
                            {
                                txt.Enabled = true;
                            }
                            else
                            {
                                txt.Enabled = false;
                            }

                            controlList.Add(txt);

                            TextBox txt3 = new TextBox();
                            txt3.Name = txtSearchName.Name;
                            txt3.Text = txtSearchName.Text;
                            txt3.Enabled = true;

                            controlList.Add(txt3);

                            ((BackgroundWorker)(sender)).ReportProgress(0, controlList);

                            elapsedTime = "00:00:00";
                            durationPerFolder = "00:00:00.000";

                            if (workCompleted)
                            {
                                workCompleted = false;
                                string msg = "Work Completed. All folders have been renamed.";

                                if (errorCount > 0)
                                {
                                    msg += "\r\n\r\nThere were errors.";
                                }

                                //Now that the work is completed, the user can try again and if successful, cleanup can happen.
                                errorCount = 0;

                                BeginInvoke((MethodInvoker)delegate
                                {
                                    FindAndMoveMsgBox("Work Status", this.Location.X + this.Height / 3, this.Location.Y + this.Width / 8);
                                    MessageBox.Show(this, msg, "Work Status");
                                });
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        errorCount++;
                        BeginInvoke((MethodInvoker)delegate
                        {
                            if (Directory.Exists(txtDestFolder.Text))
                            {
                                File.WriteAllText(txtDestFolder.Text + "\\ExceptionLog." + Guid.NewGuid().ToString() + ".log", ex.Message + "\r\n" + ex.StackTrace);
                            }
                        });
                    }
                }
                Thread.Sleep(250);
            } while (!formClosed);
        }


        private void BwCumulativeStopWatch_DoWork(object sender, DoWorkEventArgs e)
        {
            do
            {
                elapsedTime = cwStopwatch.Elapsed.Milliseconds > 0 ? cwStopwatch.Elapsed.ToString(@"hh\:mm\:ss\.FFF", null) :
                    cwStopwatch.Elapsed.ToString(@"hh\:mm\:ss", null);

                long dirQueueCount = dirQueue.Count();
                long milliDurationPerFolder = 0;

                if (foldersRenamed > 0)
                {
                    milliDurationPerFolder = cwStopwatch.ElapsedMilliseconds / foldersRenamed;
                }

                if (dirQueueCount > 0)
                {
                    dirQueueBeforeEmpty = dirQueueCount;
                }

                TimeSpan durationTimeSpan = TimeSpan.FromMilliseconds(milliDurationPerFolder);
                durationPerFolder = durationTimeSpan.Milliseconds > 0 ? durationTimeSpan.ToString(@"hh\:mm\:ss\.FFF", null) :
                    durationTimeSpan.ToString(@"hh\:mm\:ss", null);

                TimeSpan timeRemainingTimeSpan = TimeSpan.FromMilliseconds(milliDurationPerFolder * (maxDirQueue - foldersRenamed));
                timeRemaining = string.Format("{0:D2} Days, {1:D2} Hours, {2:D2} Minutes, {3:D2} Seconds, {4:D2} Milliseconds", timeRemainingTimeSpan.Days,
                    timeRemainingTimeSpan.Hours, timeRemainingTimeSpan.Minutes, timeRemainingTimeSpan.Seconds, timeRemainingTimeSpan.Milliseconds);

            } while (!((BackgroundWorker)(sender)).CancellationPending && !formClosed);

            cwStopwatch.Stop();

            e.Cancel = true;
        }

        private void GetParentDirectories(DirectoryInfo dInfo, Dictionary<string, int> dictionaryIn)
        {
            DirectoryInfo parent = dInfo.Parent;
            if (parent != null)
            {
                string parentName = parent.Name;

                //Build list of all parent folders
                if (dictionaryIn.ContainsKey(parentName))
                {
                    dictionaryIn[parentName]++;
                }
                else
                {
                    dictionaryIn.Add(parentName, 1);
                }

                GetParentDirectories(parent, dictionaryIn);
            }
        }

        private void BwPreparing_DoWork(object sender, DoWorkEventArgs e)
        {
            preparing = true;
            rename = "Preparing";
            swPreparing.Start();
            DirectoryInfo di = new DirectoryInfo(txtSourceFolder.Text);
            Stopwatch diListStopwatch = new Stopwatch();

            //Prepare and Setup File Queues for Processing
            if (di.Exists)
            {
                try
                {
                    diListStopwatch.Start();
                    swPBar.Start();
                    //Exclude children of matches
                    //Exclude children of previously matched
                    //Exclude destination folder from search list
                    List<DirectoryInfo> diList = di.EnumerateDirectories("*", SearchOption.AllDirectories).Where(
                        d => d.Name == searchName && !d.FullName.Contains("\\" + searchName + "\\") && !d.FullName.Contains(" - " + searchName) && (destDirectory.Length > 0 ? !d.FullName.Contains(destDirectory) : true)).ToList();

                    List<DirectoryInfo> previousRenamedDiList = new List<DirectoryInfo>();

                    //Search for previously renamed folders in case the user wants to move them to a new directory
                    //Exclude destination folder from search list
                    previousRenamedDiList = di.EnumerateDirectories("*", SearchOption.AllDirectories).Where(d => d.Name.Contains(" - " + searchName) && (destDirectory.Length > 0 ? !d.FullName.Contains(destDirectory) : true)).ToList();

                    
                    swPBar.Stop();
                    swPBar.Reset();

                    foreach (DirectoryInfo dInfo in diList)
                    {
                        GetParentDirectories(dInfo, parentFolders);
                    }

                    dirQueue = new ConcurrentQueue<DirectoryInfo>(diList);
                    if (moveRenamedFolders)
                    {
                        previouslyRenamedQueue = new ConcurrentQueue<DirectoryInfo>(previousRenamedDiList);
                    }

                    maxDirQueue = dirQueue.Count;

                    diListStopwatch.Stop();
                    //How long did it take to get a directory listing and build dir queue?
                    string diListDuration = diListStopwatch.Elapsed.ToString();

                    btnBusiness_Click(this, new EventArgs());
                }
                catch (Exception ex)
                {
                    errorCount++;
                    if (di != null)
                    {
                        File.WriteAllText(di.FullName + "\\ExceptionLog." + Guid.NewGuid().ToString() + ".log", ex.Message + "\r\n" + ex.StackTrace);
                    }
                }
            }
            else
            {
                BeginInvoke((MethodInvoker)delegate
                {
                    FindAndMoveMsgBox("Directory Error", this.Location.X + this.Height / 2, this.Location.Y + this.Width / 8);
                    MessageBox.Show(this, "Folder Path Invalid", "Directory Error");
                });
            }

            e.Cancel = true;
        }

        private void BwBusiness_DoWork(object sender, DoWorkEventArgs e)
        {
            Stopwatch directoryStopwatch = new Stopwatch();

            Dictionary<string, int> renamedFolders = new Dictionary<string, int>();

            while (dirQueue.Count() > 0 && !((BackgroundWorker)(sender)).CancellationPending && !formClosed)
            {
                string output = string.Empty;

                if (!directoryStopwatch.IsRunning)
                {
                    directoryStopwatch.Start();
                }

                DirectoryInfo dir;
                dirQueue.TryDequeue(out dir);

                try
                {
                    if (dir != null && Directory.Exists(dir.FullName))
                    {
                        string directoryPath = dir.FullName;
                        currentDirectory = directoryPath;

                        //Get initial subdirectory path pre-move
                        string initialSubDirPath = dir.FullName;

                        //Check all parent folders of current dir.
                        Dictionary<string, int> dirFolders = new Dictionary<string, int>();
                        GetParentDirectories(dir, dirFolders);

                        //Find lowest occurrence in master folder dictionary
                        KeyValuePair<string, int> lowestOccurrence = new KeyValuePair<string, int>();

                        foreach (KeyValuePair<string, int> item in dirFolders)
                        {
                            if (null == lowestOccurrence.Key || parentFolders[item.Key] < lowestOccurrence.Value)
                            {
                                lowestOccurrence = new KeyValuePair<string, int>(item.Key, parentFolders[item.Key]);
                            }
                        }

                        string newFolderName = string.Empty;

                        //Perform custom rename or smart dynamic rename
                        if (customRename)
                        {
                            //Rename based on specified custom rename
                            newFolderName = customRenameText;
                        }
                        else
                        {
                            //Rename based on hit with the lowest number of duplicates
                            int numOccur = 0;
                            if (renamedFolders.TryGetValue(lowestOccurrence.Key, out numOccur))
                            {
                                //Mark dictionary value of hit, incrementing duplicate by one
                                renamedFolders[lowestOccurrence.Key] = numOccur++;
                                newFolderName = string.Format("{0} - {1}{2}", lowestOccurrence.Key, searchName, numOccur);
                            }
                            else
                            {
                                //Add new occurrence to renamed folders dictionary
                                renamedFolders.Add(lowestOccurrence.Key, 1);
                                newFolderName = string.Format("{0} - {1}", lowestOccurrence.Key, searchName);
                            }
                        }

                        try
                        {
                            string source = dir.FullName;
                            string dest = dir.Parent.FullName + "\\" + newFolderName;
                            dir = new DirectoryInfo(dir.Root.FullName);
                            
                            Directory.Move(source, dest);
                            recentlyRenamedQueue.Enqueue(new DirectoryInfo(dest));

                            //Renaming was Successful?
                            foldersRenamed++;
                        }
                        catch(IOException iex)
                        {

                        }
                        catch (Exception ex)
                        {
                            errorCount++;
                            BeginInvoke((MethodInvoker)delegate
                            {
                                FindAndMoveMsgBox("Folder Rename Error", this.Location.X + this.Width / 8, this.Location.Y + this.Height / 2);
                                MessageBox.Show(this, ex.Message, "Folder Rename Error");
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    errorCount++;
                    if (dir != null)
                    {
                        File.WriteAllText(dir.FullName + "\\ExceptionLog." + dir.Name + "." +
                            Guid.NewGuid().ToString() + ".log", ex.Message + "\r\n" + ex.StackTrace);
                    }
                }
            }

            if (!formClosed)
            {
                //Move directory if moving renamed folders is enabled
                if (moveRenamedFolders)
                {
                    try
                    {
                        while (recentlyRenamedQueue.Count() > 0)
                        {
                            DirectoryInfo dir;
                            recentlyRenamedQueue.TryDequeue(out dir);

                            string source = dir.FullName;
                            string dest = destDirectory + "\\" + dir.Name;

                            //If custom rename and merge activated, move all files in directory to destination and delete directory
                            if (customRename)
                            {
                                //Create destination if it doesnt exist yet
                                if (!Directory.Exists(destDirectory + "\\" + dir.Name))
                                {
                                    Directory.CreateDirectory(destDirectory + "\\" + dir.Name);
                                }

                                //Move all files in directory to destination and delete directory
                                var files = dir.EnumerateFiles().Where(f => !f.Name.Contains(".log"));

                                foreach (FileInfo fi in files)
                                {
                                    if (!File.Exists(destDirectory + "\\" + dir.Name + "\\" + fi.Name))
                                    {
                                        File.Move(fi.FullName, destDirectory + "\\" + dir.Name + "\\" + fi.Name);
                                    }
                                    else
                                    {
                                        //Overwrite smaller duplicate
                                        if (fi.Length > new FileInfo(destDirectory + "\\" + dir.Name + "\\" + fi.Name).Length)
                                        {
                                            File.Delete(destDirectory + "\\" + dir.Name + "\\" + fi.Name);
                                            File.Move(fi.FullName, destDirectory + "\\" + dir.Name + "\\" + fi.Name);
                                            File.Create(dir.FullName + "\\DeletionLog." + fi.Name + "." +
                                                Guid.NewGuid().ToString() + ".log");
                                        }
                                        else
                                        {
                                            File.Delete(fi.FullName);
                                            File.Create(dir.FullName + "\\DeletionLog." + fi.Name + "." +
                                                Guid.NewGuid().ToString() + ".log");
                                        }
                                    }
                                }

                                if (dir.EnumerateFiles().Count() == 0)
                                {
                                    dir.Delete();
                                }
                            }
                            else
                            {
                                if (Directory.Exists(source))
                                {
                                    Directory.Move(source, dest);

                                    //Moving was Successful?
                                    foldersMoved++;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        errorCount++;
                        BeginInvoke((MethodInvoker)delegate
                        {
                            FindAndMoveMsgBox("Folder Move Error", this.Location.X + this.Height / 2, this.Location.Y + this.Width / 8);
                            MessageBox.Show(this, ex.Message, "Folder Move Error");
                        });
                    }
                }
                else
                {
                    //Blow away the queue since "recent" time is over, my friend.
                    //We're in previous territory now, bub.
                    recentlyRenamedQueue = new ConcurrentQueue<DirectoryInfo>();
                }
            }

            //Queue empty and last worker? Time to clean up logs and signal work completed.
            if (dirQueue.Count() == 0)
            {
                workCompleted = true;

                if (!bwCancel.IsBusy)
                {
                    bwCancel.RunWorkerAsync();
                    Cancel();
                }
            }
        }

        private void BwProgressBar_DoWork(object sender, DoWorkEventArgs e)
        {
            do
            {
                if (maxDirQueue > 0)
                {
                    double newPercentComplete = ((double)foldersRenamed / (double)maxDirQueue);
                    if (newPercentComplete != percentComplete)
                    {
                        percentComplete = newPercentComplete;
                    }
                }
            } while (!((BackgroundWorker)(sender)).CancellationPending && !formClosed);

            percentComplete = 0;
            e.Cancel = true;
        }

        private void btnSetSourceFolder_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(this.txtSourceFolder.Text))
            {
                dlgFolderBrowser.SelectedPath = this.txtSourceFolder.Text;
            }
            else
            {
                dlgFolderBrowser.SelectedPath = Application.StartupPath;
            }

            if (DialogResult.OK == dlgFolderBrowser.ShowDialog())
            {
                txtSourceFolder.Text = dlgFolderBrowser.SelectedPath;
                Smart_Folder_Renamer.Properties.Settings.Default.SourceFolder = txtSourceFolder.Text;
                Smart_Folder_Renamer.Properties.Settings.Default.Save();
            }
        }

        private void btnSetDestFolder_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(this.txtDestFolder.Text))
            {
                folderBrowserDialog1.SelectedPath = this.txtDestFolder.Text;
            }
            else
            {
                folderBrowserDialog1.SelectedPath = Application.StartupPath;
            }

            if (DialogResult.OK == folderBrowserDialog1.ShowDialog())
            {
                txtDestFolder.Text = folderBrowserDialog1.SelectedPath;
                Smart_Folder_Renamer.Properties.Settings.Default.DestinationFolder = txtDestFolder.Text;
                Smart_Folder_Renamer.Properties.Settings.Default.Save();
            }
        }

        private void btnRenameFolders_Click(object sender, EventArgs e)
        {
            bool error = false;
            moveRenamedFolders = chkMoveFolders.Checked;

            if (moveRenamedFolders)
            {
                if (customRename)
                {
                    if (txtCustomRename.Text.Trim() == string.Empty)
                    {
                        error = true;
                        FindAndMoveMsgBox("Custom Rename Error", this.Location.X + this.Height / 2, this.Location.Y + this.Width / 8);
                        MessageBox.Show(this, "Invalid Custom Rename Text.", "Custom Rename Error");
                    }
                }

                if (txtDestFolder.Text.Trim() == string.Empty || !Directory.Exists(txtDestFolder.Text))
                {
                    if (txtDestFolder.Text.Trim() == string.Empty)
                    {
                        error = true;

                        FindAndMoveMsgBox("Destination Error", this.Location.X + this.Height / 2, this.Location.Y + this.Width / 8);
                        MessageBox.Show(this, "Invalid destination path.", "Destination Error");
                    }
                    else
                    {
                        try
                        {
                            if (Directory.CreateDirectory(txtDestFolder.Text) == null)
                            {
                                error = true;
                                FindAndMoveMsgBox("Destination Error", this.Location.X + this.Height / 2, this.Location.Y + this.Width / 8);
                                MessageBox.Show(this, "Invalid destination path.", "Destination Error");
                            }
                        }
                        catch (Exception ex)
                        {
                            errorCount++;
                            error = true;
                            FindAndMoveMsgBox("Destination Error", this.Location.X + this.Height / 2, this.Location.Y + this.Width / 8);
                            MessageBox.Show(this, "Invalid destination path. " + ex.Message, "Destination Error");
                        }
                    }
                }
            }
            if (txtSourceFolder.Text.Trim() == string.Empty || !Directory.Exists(txtSourceFolder.Text))
            {
                error = true;
                FindAndMoveMsgBox("Source Error", this.Location.X + this.Height / 2, this.Location.Y + this.Width / 8);
                MessageBox.Show(this, "Invalid source folder path.", "Source Error");
            }
            if (txtSearchName.Text.Trim() == string.Empty)
            {
                error = true;
                FindAndMoveMsgBox("Matching Subfolder Error", this.Location.X + this.Height / 2, this.Location.Y + this.Width / 8);
                MessageBox.Show(this, "Invalid Matching Subfolder Name.", "Matching Subfolder Error");
            }

            if (!error)
            {
                btnRenameFolders.Enabled = false;

                if (!bwPreparing.IsBusy && !bwProgressBar.IsBusy)
                {
                    //Reset all counters
                    foldersRenamed = 0;
                    elapsedTime = string.Empty;
                    durationPerFolder = string.Empty;
                    timeRemaining = string.Empty;

                    //Reset Stopwatch
                    cwStopwatch.Stop();
                    cwStopwatch.Reset();
                    cwStopwatch.Start();

                    //Start workers
                    bwPreparing.RunWorkerAsync();
                    bwProgressBar.RunWorkerAsync();

                    initialDirectory = txtSourceFolder.Text;

                    searchName = txtSearchName.Text;

                    if(chkMoveFolders.Checked)
                    {
                        destDirectory = txtDestFolder.Text;
                    }
                    else
                    {
                        destDirectory = string.Empty;
                    }

                    if(chkCustomRename.Checked)
                    {
                        customRenameText = txtCustomRename.Text.Trim();
                    }
                    else
                    {
                        customRenameText = string.Empty;
                    }
                }
            }
        }

        private void Cancel(bool prepareCanceled = false)
        {
            preparing = false;

            if (bwBusiness != null && bwBusiness.IsBusy)
            {
                bwBusiness.CancelAsync();
            }

            if (bwPreparing.IsBusy && prepareCanceled != true)
            {
                bwPreparing.CancelAsync();
            }

            if (bwProgressBar.IsBusy)
            {
                bwProgressBar.CancelAsync();
            }

            bwCancel.CancelAsync();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            btnCancel.Enabled = false;

            if (bwPreparing.IsBusy)
            {
                bwPreparing.CancelAsync();
            }

            if (!bwCancel.IsBusy)
            {
                bwCancel.RunWorkerAsync();
                Cancel();
            }
        }

        private void btnBusiness_Click(object sender, EventArgs e)
        {
            bool quitPreparing = false;
            preparing = false;

            rename = "Renaming";
            swPreparing.Stop();
            swPreparing.Reset();

            //Begin cumulative stopwatch worker
            bwCumulativeStopWatch.RunWorkerAsync();

            //Wait for business worker to stop being busy
            if (bwBusiness != null && bwBusiness.IsBusy && !quitPreparing)
            {
                Stopwatch swBusyWorker = new Stopwatch();
                swBusyWorker.Start();

                while (bwBusiness != null && bwBusiness.IsBusy)
                {
                    //Keep checking if worker is busy for 5 seconds
                    if (swBusyWorker.ElapsedMilliseconds > 5000)
                    {
                        //If worker is still busy after 5 seconds, toggle quit preparing flag
                        quitPreparing = true;
                        break;
                    }
                }
            }
            //If quit preparing flag is true cancel Processing, otherwise run business worker
            if (quitPreparing || bwPreparing.CancellationPending)
            {
                if (!bwCancel.IsBusy)
                {
                    bwCancel.RunWorkerAsync();
                }

                Cancel(quitPreparing);
            }
            else
            {
                bwBusiness.RunWorkerAsync();
            }
        }

        private void btnHandlePreviousRenamed_Click(object sender, EventArgs e)
        {
            BeginInvoke((MethodInvoker)delegate
            {
                FindAndMoveMsgBox("Renamed Folders Detected", this.Location.X + this.Height / 6, this.Location.Y + this.Width / 8);

                if (
                MessageBox.Show(this, string.Format("{0} previously renamed folders detected. Move them to destination folder?", previouslyRenamedQueue.Count()),
                    "Renamed Folders Detected", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    try
                    {
                        string source = string.Empty;
                        string dest = string.Empty;

                        while(previouslyRenamedQueue.Count() > 0)
                        {
                            DirectoryInfo di;
                            previouslyRenamedQueue.TryDequeue(out di);

                            source = di.FullName;
                            dest = destDirectory + "\\" + di.Name;
                            
                            Directory.Move(source, dest);
                        }
                    }
                    catch (Exception ex)
                    {
                        errorCount++;
                        FindAndMoveMsgBox("Folder Move Error", this.Location.X + this.Height / 2, this.Location.Y + this.Width / 8);
                        MessageBox.Show(this, ex.Message, "Folder Move Error");
                    }
                }
            });
        }

        private void chkMoveFolders_CheckedChanged(object sender, EventArgs e)
        {
            if (chkMoveFolders.Checked)
            {
                moveRenamedFolders = true;
                chkCustomRename.Enabled = true;
            }
            else
            {
                moveRenamedFolders = false;
                chkCustomRename.Enabled = false;
                txtCustomRename.Enabled = false;
                chkCustomRename.Checked = false;
            }
        }

        private void chkCustomRename_CheckedChanged(object sender, EventArgs e)
        {
            if (chkCustomRename.Checked)
            {
                customRename = true;
                txtCustomRename.Enabled = true;
            }
            else
            {
                customRename = false;
                txtCustomRename.Enabled = false;
            }
        }
    }
}