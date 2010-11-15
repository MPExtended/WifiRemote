namespace WifiRemote
{
    partial class SetupForm
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
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.buttonDownloadBonjour = new System.Windows.Forms.Button();
            this.progressBarBonjourDownload = new System.Windows.Forms.ProgressBar();
            this.backgroundWorkerBonjourDownload = new System.ComponentModel.BackgroundWorker();
            this.textBoxPort = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Port:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(155, 13);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "(Default: 8017)";
            // 
            // buttonDownloadBonjour
            // 
            this.buttonDownloadBonjour.Location = new System.Drawing.Point(16, 72);
            this.buttonDownloadBonjour.Name = "buttonDownloadBonjour";
            this.buttonDownloadBonjour.Size = new System.Drawing.Size(216, 23);
            this.buttonDownloadBonjour.TabIndex = 4;
            this.buttonDownloadBonjour.Text = "Download and install Bonjour";
            this.buttonDownloadBonjour.UseVisualStyleBackColor = true;
            this.buttonDownloadBonjour.Click += new System.EventHandler(this.buttonDownloadBonjour_Click);
            // 
            // progressBarBonjourDownload
            // 
            this.progressBarBonjourDownload.Location = new System.Drawing.Point(16, 43);
            this.progressBarBonjourDownload.Name = "progressBarBonjourDownload";
            this.progressBarBonjourDownload.Size = new System.Drawing.Size(216, 23);
            this.progressBarBonjourDownload.TabIndex = 5;
            this.progressBarBonjourDownload.Visible = false;
            // 
            // backgroundWorkerBonjourDownload
            // 
            this.backgroundWorkerBonjourDownload.WorkerReportsProgress = true;
            this.backgroundWorkerBonjourDownload.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerBonjourDownload_DoWork);
            this.backgroundWorkerBonjourDownload.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorkerBonjourDownload_RunWorkerCompleted);
            this.backgroundWorkerBonjourDownload.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorkerBonjourDownload_ProgressChanged);
            // 
            // textBoxPort
            // 
            this.textBoxPort.Location = new System.Drawing.Point(48, 10);
            this.textBoxPort.Name = "textBoxPort";
            this.textBoxPort.Size = new System.Drawing.Size(100, 20);
            this.textBoxPort.TabIndex = 6;
            // 
            // SetupForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(248, 107);
            this.Controls.Add(this.textBoxPort);
            this.Controls.Add(this.progressBarBonjourDownload);
            this.Controls.Add(this.buttonDownloadBonjour);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.MaximizeBox = false;
            this.Name = "SetupForm";
            this.ShowIcon = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "WifiRemote Setup";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SetupForm_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button buttonDownloadBonjour;
        private System.Windows.Forms.ProgressBar progressBarBonjourDownload;
        private System.ComponentModel.BackgroundWorker backgroundWorkerBonjourDownload;
        private System.Windows.Forms.TextBox textBoxPort;
    }
}