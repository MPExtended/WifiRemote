using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;
using MediaPortal.Configuration;

namespace WifiRemote
{
    public partial class SetupForm : Form
    {
        private bool is64bit = false;

        private String originalPort;

        private String downloadUrl32Bit = "http://support.apple.com/downloads/DL755/en_US/BonjourSetup.exe";
        private String downloadUrl64Bit = "http://download.info.apple.com/Mac_OS_X/061-5788.20081215.5t9Uk/Bonjour64Setup.exe";
        private String downloadTarget;


        public SetupForm()
        {
            InitializeComponent();
            label2.Text = String.Format("(Default: {0})", WifiRemote.DEFAULT_PORT);

            // load port from settings
            using (MediaPortal.Profile.Settings reader = new MediaPortal.Profile.Settings(Config.GetFile(Config.Dir.Config, "MediaPortal.xml")))
            {
                originalPort = reader.GetValue(WifiRemote.PLUGIN_NAME, "port");
                checkBoxDisableBonjour.Checked = reader.GetValueAsBool(WifiRemote.PLUGIN_NAME, "disableBonjour", false);
                textBoxName.Text = reader.GetValueAsString(WifiRemote.PLUGIN_NAME, "serviceName", WifiRemote.GetServiceName());
        
                resetPort();
            }

            // Test if Bonjour is installed
            try
            {
                //float bonjourVersion = ZeroconfService.NetService.GetVersion();
                Version bonjourVersion = ZeroconfService.NetService.DaemonVersion;
                buttonDownloadBonjour.Enabled = false;
                checkBoxDisableBonjour.Enabled = false;
                buttonDownloadBonjour.Text = "Bonjour already installed";
            }
            catch
            {
                if (Is64Bit() || Is32BitProcessOn64BitProcessor())
                {
                    // 64 bit windows
                    is64bit = true;
                    buttonDownloadBonjour.Enabled = true;
                    checkBoxDisableBonjour.Enabled = true;
                    buttonDownloadBonjour.Text = "Download and install Bonjour (64 bit)";
                }
                else
                {
                    // 32 bit windows
                    is64bit = false;
                    buttonDownloadBonjour.Enabled = true;
                    checkBoxDisableBonjour.Enabled = true;
                    buttonDownloadBonjour.Text = "Download and install Bonjour (32 bit)";
                }
            }
        }


        /// <summary>
        /// Download Bonjour button clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonDownloadBonjour_Click(object sender, EventArgs e)
        {
            buttonDownloadBonjour.Enabled = false;
            progressBarBonjourDownload.Visible = true;
            backgroundWorkerBonjourDownload.RunWorkerAsync();
        }

        /// <summary>
        /// Save changed vars to mediaportal settings file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SetupForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                UInt16 portCheck = UInt16.Parse(textBoxPort.Text);
                if (portCheck == 0)
                {
                    resetPort();
                }
            }
            catch (Exception)
            {
                resetPort();
            }

            using (MediaPortal.Profile.Settings xmlwriter = new MediaPortal.Profile.Settings(Config.GetFile(Config.Dir.Config, "MediaPortal.xml")))
            {
                xmlwriter.SetValue(WifiRemote.PLUGIN_NAME, "port", textBoxPort.Text);
                xmlwriter.SetValueAsBool(WifiRemote.PLUGIN_NAME, "disableBonjour", checkBoxDisableBonjour.Checked);
                xmlwriter.SetValue(WifiRemote.PLUGIN_NAME, "serviceName", textBoxName.Text);
            }
        }


        /// <summary>
        /// Set the port box to defaults
        /// </summary>
        private void resetPort()
        {
            textBoxPort.Text = (originalPort != String.Empty) ? originalPort : WifiRemote.DEFAULT_PORT.ToString();
        }


        #region Windows 64bit check

        [DllImport("kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsWow64Process([In] IntPtr hProcess, [Out] out bool lpSystemInfo);

        private bool Is64Bit()
        {
            if (IntPtr.Size == 8 || (IntPtr.Size == 4 && Is32BitProcessOn64BitProcessor()))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool Is32BitProcessOn64BitProcessor()
        {
            bool retVal;

            IsWow64Process(System.Diagnostics.Process.GetCurrentProcess().Handle, out retVal);

            return retVal;
        }

        #endregion


        #region Background Worker events

        /// <summary>
        /// Get the file with progress bar updates
        /// 
        /// Inspired by:
        /// http://www.devtoolshed.com/content/c-download-file-progress-bar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backgroundWorkerBonjourDownload_DoWork(object sender, DoWorkEventArgs e)
        {
            Uri url = new Uri((is64bit) ? downloadUrl64Bit : downloadUrl32Bit);
            downloadTarget = Path.GetTempPath() + "BonjourSetup.exe";

            // Get filesize
            System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(url);
            System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)request.GetResponse();
            response.Close();

            // gets the size of the file in bytes
            Int64 iSize = response.ContentLength;

            // keeps track of the total bytes downloaded so we can update the progress 
            Int64 iRunningByteTotal = 0;

            // use the webclient object to download the file
            using (System.Net.WebClient client = new System.Net.WebClient())
            {
                // open the file at the remote URL for reading
                using (System.IO.Stream streamRemote = client.OpenRead(url))
                {
                    // using the FileStream object, we can write the downloaded bytes to the file system
                    using (Stream streamLocal = new FileStream(downloadTarget, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        // loop the stream and get the file into the byte buffer
                        int iByteSize = 0;
                        byte[] byteBuffer = new byte[iSize];

                        while ((iByteSize = streamRemote.Read(byteBuffer, 0, byteBuffer.Length)) > 0)
                        {
                            // write the bytes to the file system at the file path specified
                            streamLocal.Write(byteBuffer, 0, iByteSize);
                            iRunningByteTotal += iByteSize;

                            // calculate the progress out of a base "100"
                            double dIndex = (double)(iRunningByteTotal);
                            double dTotal = (double)byteBuffer.Length;
                            double dProgressPercentage = (dIndex / dTotal);
                            int iProgressPercentage = (int)(dProgressPercentage * 100);

                            // update the progress bar
                            backgroundWorkerBonjourDownload.ReportProgress(iProgressPercentage);
                        }

                        // clean up the file stream
                        streamLocal.Close();
                    }

                    // close the connection to the remote server
                    streamRemote.Close();
                }
            }
        }

        /// <summary>
        /// Update the download progress bar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backgroundWorkerBonjourDownload_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBarBonjourDownload.Value = e.ProgressPercentage;
        }

        /// <summary>
        /// Install bonjour
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backgroundWorkerBonjourDownload_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(downloadTarget);
            }
            catch (Exception) {}
        }

        #endregion

    }
}
