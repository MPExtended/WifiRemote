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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SetupForm));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.buttonDownloadBonjour = new System.Windows.Forms.Button();
            this.progressBarBonjourDownload = new System.Windows.Forms.ProgressBar();
            this.backgroundWorkerBonjourDownload = new System.ComponentModel.BackgroundWorker();
            this.textBoxPort = new System.Windows.Forms.TextBox();
            this.checkBoxDisableBonjour = new System.Windows.Forms.CheckBox();
            this.tabControlNavigation = new System.Windows.Forms.TabControl();
            this.tabPageNetwork = new System.Windows.Forms.TabPage();
            this.textBoxName = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tabPagePlugins = new System.Windows.Forms.TabPage();
            this.dataGridViewPluginList = new System.Windows.Forms.DataGridView();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.label5 = new System.Windows.Forms.Label();
            this.cbAuthMethod = new System.Windows.Forms.ComboBox();
            this.groupPasscode = new System.Windows.Forms.GroupBox();
            this.txtPasscode = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.groupUsernamePassword = new System.Windows.Forms.GroupBox();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.txtUsername = new System.Windows.Forms.TextBox();
            this.lblPassword = new System.Windows.Forms.Label();
            this.lblUser = new System.Windows.Forms.Label();
            this.setupFormBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.setupFormBindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            this.tabControlNavigation.SuspendLayout();
            this.tabPageNetwork.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabPagePlugins.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewPluginList)).BeginInit();
            this.tabPage1.SuspendLayout();
            this.groupPasscode.SuspendLayout();
            this.groupUsernamePassword.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.setupFormBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.setupFormBindingSource1)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(19, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Port:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(169, 13);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "(Default: 8017)";
            // 
            // buttonDownloadBonjour
            // 
            this.buttonDownloadBonjour.Location = new System.Drawing.Point(7, 48);
            this.buttonDownloadBonjour.Name = "buttonDownloadBonjour";
            this.buttonDownloadBonjour.Size = new System.Drawing.Size(236, 23);
            this.buttonDownloadBonjour.TabIndex = 4;
            this.buttonDownloadBonjour.Text = "Download and install Bonjour";
            this.buttonDownloadBonjour.UseVisualStyleBackColor = true;
            this.buttonDownloadBonjour.Click += new System.EventHandler(this.buttonDownloadBonjour_Click);
            // 
            // progressBarBonjourDownload
            // 
            this.progressBarBonjourDownload.Location = new System.Drawing.Point(7, 19);
            this.progressBarBonjourDownload.Name = "progressBarBonjourDownload";
            this.progressBarBonjourDownload.Size = new System.Drawing.Size(236, 23);
            this.progressBarBonjourDownload.TabIndex = 5;
            this.progressBarBonjourDownload.Visible = false;
            // 
            // backgroundWorkerBonjourDownload
            // 
            this.backgroundWorkerBonjourDownload.WorkerReportsProgress = true;
            this.backgroundWorkerBonjourDownload.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerBonjourDownload_DoWork);
            this.backgroundWorkerBonjourDownload.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorkerBonjourDownload_ProgressChanged);
            this.backgroundWorkerBonjourDownload.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorkerBonjourDownload_RunWorkerCompleted);
            // 
            // textBoxPort
            // 
            this.textBoxPort.Location = new System.Drawing.Point(55, 10);
            this.textBoxPort.Name = "textBoxPort";
            this.textBoxPort.Size = new System.Drawing.Size(108, 20);
            this.textBoxPort.TabIndex = 6;
            // 
            // checkBoxDisableBonjour
            // 
            this.checkBoxDisableBonjour.AutoSize = true;
            this.checkBoxDisableBonjour.Enabled = false;
            this.checkBoxDisableBonjour.Location = new System.Drawing.Point(7, 223);
            this.checkBoxDisableBonjour.Name = "checkBoxDisableBonjour";
            this.checkBoxDisableBonjour.Size = new System.Drawing.Size(197, 17);
            this.checkBoxDisableBonjour.TabIndex = 7;
            this.checkBoxDisableBonjour.Text = "Disable Bonjour (not recommended!)";
            this.checkBoxDisableBonjour.UseVisualStyleBackColor = true;
            // 
            // tabControlNavigation
            // 
            this.tabControlNavigation.Controls.Add(this.tabPageNetwork);
            this.tabControlNavigation.Controls.Add(this.tabPagePlugins);
            this.tabControlNavigation.Controls.Add(this.tabPage1);
            this.tabControlNavigation.Location = new System.Drawing.Point(1, 1);
            this.tabControlNavigation.Name = "tabControlNavigation";
            this.tabControlNavigation.SelectedIndex = 0;
            this.tabControlNavigation.Size = new System.Drawing.Size(263, 361);
            this.tabControlNavigation.TabIndex = 8;
            // 
            // tabPageNetwork
            // 
            this.tabPageNetwork.Controls.Add(this.textBoxName);
            this.tabPageNetwork.Controls.Add(this.label4);
            this.tabPageNetwork.Controls.Add(this.groupBox1);
            this.tabPageNetwork.Controls.Add(this.label1);
            this.tabPageNetwork.Controls.Add(this.label2);
            this.tabPageNetwork.Controls.Add(this.textBoxPort);
            this.tabPageNetwork.Location = new System.Drawing.Point(4, 22);
            this.tabPageNetwork.Name = "tabPageNetwork";
            this.tabPageNetwork.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageNetwork.Size = new System.Drawing.Size(255, 335);
            this.tabPageNetwork.TabIndex = 0;
            this.tabPageNetwork.Text = "Network";
            this.tabPageNetwork.UseVisualStyleBackColor = true;
            // 
            // textBoxName
            // 
            this.textBoxName.Location = new System.Drawing.Point(55, 36);
            this.textBoxName.Name = "textBoxName";
            this.textBoxName.Size = new System.Drawing.Size(191, 20);
            this.textBoxName.TabIndex = 10;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(10, 39);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(38, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "Name:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.progressBarBonjourDownload);
            this.groupBox1.Controls.Add(this.buttonDownloadBonjour);
            this.groupBox1.Controls.Add(this.checkBoxDisableBonjour);
            this.groupBox1.Location = new System.Drawing.Point(3, 81);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(249, 246);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Bonjour";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.SystemColors.GrayText;
            this.label3.Location = new System.Drawing.Point(7, 78);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(231, 130);
            this.label3.TabIndex = 8;
            this.label3.Text = resources.GetString("label3.Text");
            // 
            // tabPagePlugins
            // 
            this.tabPagePlugins.Controls.Add(this.dataGridViewPluginList);
            this.tabPagePlugins.Location = new System.Drawing.Point(4, 22);
            this.tabPagePlugins.Name = "tabPagePlugins";
            this.tabPagePlugins.Padding = new System.Windows.Forms.Padding(3);
            this.tabPagePlugins.Size = new System.Drawing.Size(255, 335);
            this.tabPagePlugins.TabIndex = 1;
            this.tabPagePlugins.Text = "Plugins";
            this.tabPagePlugins.UseVisualStyleBackColor = true;
            // 
            // dataGridViewPluginList
            // 
            this.dataGridViewPluginList.AllowUserToAddRows = false;
            this.dataGridViewPluginList.AllowUserToDeleteRows = false;
            this.dataGridViewPluginList.AllowUserToOrderColumns = true;
            this.dataGridViewPluginList.AllowUserToResizeColumns = false;
            this.dataGridViewPluginList.AllowUserToResizeRows = false;
            this.dataGridViewPluginList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewPluginList.ColumnHeadersVisible = false;
            this.dataGridViewPluginList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewPluginList.Location = new System.Drawing.Point(3, 3);
            this.dataGridViewPluginList.Name = "dataGridViewPluginList";
            this.dataGridViewPluginList.ReadOnly = true;
            this.dataGridViewPluginList.RowHeadersVisible = false;
            this.dataGridViewPluginList.Size = new System.Drawing.Size(249, 329);
            this.dataGridViewPluginList.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.label5);
            this.tabPage1.Controls.Add(this.cbAuthMethod);
            this.tabPage1.Controls.Add(this.groupPasscode);
            this.tabPage1.Controls.Add(this.groupUsernamePassword);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(255, 335);
            this.tabPage1.TabIndex = 2;
            this.tabPage1.Text = "Authentification";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(7, 21);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(119, 13);
            this.label5.TabIndex = 2;
            this.label5.Text = "Authentification Method";
            // 
            // cbAuthMethod
            // 
            this.cbAuthMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbAuthMethod.FormattingEnabled = true;
            this.cbAuthMethod.Items.AddRange(new object[] {
            "No Authentification",
            "Username/Password",
            "Passcode",
            "Both"});
            this.cbAuthMethod.Location = new System.Drawing.Point(132, 18);
            this.cbAuthMethod.Name = "cbAuthMethod";
            this.cbAuthMethod.Size = new System.Drawing.Size(111, 21);
            this.cbAuthMethod.TabIndex = 1;
            this.cbAuthMethod.SelectedIndexChanged += new System.EventHandler(this.cbAuthMethod_SelectedIndexChanged);
            // 
            // groupPasscode
            // 
            this.groupPasscode.Controls.Add(this.txtPasscode);
            this.groupPasscode.Controls.Add(this.label7);
            this.groupPasscode.Location = new System.Drawing.Point(7, 171);
            this.groupPasscode.Name = "groupPasscode";
            this.groupPasscode.Size = new System.Drawing.Size(240, 63);
            this.groupPasscode.TabIndex = 0;
            this.groupPasscode.TabStop = false;
            this.groupPasscode.Text = "Passcode";
            // 
            // txtPasscode
            // 
            this.txtPasscode.Location = new System.Drawing.Point(87, 25);
            this.txtPasscode.Name = "txtPasscode";
            this.txtPasscode.Size = new System.Drawing.Size(147, 20);
            this.txtPasscode.TabIndex = 2;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(15, 28);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(54, 13);
            this.label7.TabIndex = 0;
            this.label7.Text = "Passcode";
            // 
            // groupUsernamePassword
            // 
            this.groupUsernamePassword.Controls.Add(this.txtPassword);
            this.groupUsernamePassword.Controls.Add(this.txtUsername);
            this.groupUsernamePassword.Controls.Add(this.lblPassword);
            this.groupUsernamePassword.Controls.Add(this.lblUser);
            this.groupUsernamePassword.Location = new System.Drawing.Point(9, 55);
            this.groupUsernamePassword.Name = "groupUsernamePassword";
            this.groupUsernamePassword.Size = new System.Drawing.Size(240, 97);
            this.groupUsernamePassword.TabIndex = 0;
            this.groupUsernamePassword.TabStop = false;
            this.groupUsernamePassword.Text = "Username / Password";
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(87, 56);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(147, 20);
            this.txtPassword.TabIndex = 2;
            // 
            // txtUsername
            // 
            this.txtUsername.Location = new System.Drawing.Point(87, 25);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Size = new System.Drawing.Size(147, 20);
            this.txtUsername.TabIndex = 2;
            // 
            // lblPassword
            // 
            this.lblPassword.AutoSize = true;
            this.lblPassword.Location = new System.Drawing.Point(15, 59);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(53, 13);
            this.lblPassword.TabIndex = 1;
            this.lblPassword.Text = "Password";
            // 
            // lblUser
            // 
            this.lblUser.AutoSize = true;
            this.lblUser.Location = new System.Drawing.Point(15, 28);
            this.lblUser.Name = "lblUser";
            this.lblUser.Size = new System.Drawing.Size(55, 13);
            this.lblUser.TabIndex = 0;
            this.lblUser.Text = "Username";
            // 
            // SetupForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(264, 362);
            this.Controls.Add(this.tabControlNavigation);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(280, 400);
            this.MinimumSize = new System.Drawing.Size(280, 400);
            this.Name = "SetupForm";
            this.ShowIcon = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "WifiRemote Setup";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SetupForm_FormClosing);
            this.tabControlNavigation.ResumeLayout(false);
            this.tabPageNetwork.ResumeLayout(false);
            this.tabPageNetwork.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabPagePlugins.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewPluginList)).EndInit();
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.groupPasscode.ResumeLayout(false);
            this.groupPasscode.PerformLayout();
            this.groupUsernamePassword.ResumeLayout(false);
            this.groupUsernamePassword.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.setupFormBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.setupFormBindingSource1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button buttonDownloadBonjour;
        private System.Windows.Forms.ProgressBar progressBarBonjourDownload;
        private System.ComponentModel.BackgroundWorker backgroundWorkerBonjourDownload;
        private System.Windows.Forms.TextBox textBoxPort;
        private System.Windows.Forms.CheckBox checkBoxDisableBonjour;
        private System.Windows.Forms.TabControl tabControlNavigation;
        private System.Windows.Forms.TabPage tabPageNetwork;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TabPage tabPagePlugins;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBoxName;
        private System.Windows.Forms.DataGridView dataGridViewPluginList;
        private System.Windows.Forms.BindingSource setupFormBindingSource;
        private System.Windows.Forms.BindingSource setupFormBindingSource1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cbAuthMethod;
        private System.Windows.Forms.GroupBox groupPasscode;
        private System.Windows.Forms.TextBox txtPasscode;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.GroupBox groupUsernamePassword;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.TextBox txtUsername;
        private System.Windows.Forms.Label lblPassword;
        private System.Windows.Forms.Label lblUser;
    }
}