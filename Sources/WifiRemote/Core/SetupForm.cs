﻿using System;
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
using System.Collections;
using System.Reflection;
using MediaPortal.GUI.Library;
using MediaPortal.Profile;
using System.Net;
using System.Net.Sockets;
using Newtonsoft.Json;
using System.Net.NetworkInformation;
using System.Xml;


namespace WifiRemote
{
  public partial class SetupForm : Form
  {
    private bool is64bit = false;

    private String originalPort;

    private String downloadUrl32Bit = "http://download.info.apple.com/Mac_OS_X/061-5788.20081215.5t9Uk/BonjourSetup.exe";
    private String downloadUrl64Bit = "http://download.info.apple.com/Mac_OS_X/061-5788.20081215.5t9Uk/Bonjour64Setup.exe";
    private String downloadTarget;

    private ArrayList availablePlugins;
    private ArrayList plugins;
    private Dictionary<int, String> savedPlugins;
    private List<int> ignoredPluginsList;
    private ImageList pluginIcons;

    private Rectangle dragBoxFromMouseDown;
    private int rowIndexFromMouseDown;
    private int rowIndexOfItemUnderMouseToDrop;

    private BindingSource pluginsDataSource = new BindingSource();

    private class ItemTag
    {
      public string DllName;
      public ISetupForm SetupForm;
      public string Type = string.Empty;
      public int WindowId = -1;
      private Image activeImage = null;
      public bool IsEnabled;

      public Image ActiveImage
      {
        get { return activeImage; }
        set { activeImage = value; }
      }
    }

    public SetupForm()
    {
      InitializeComponent();
      labelDefaultPort.Text = String.Format("(Default: {0})", WifiRemote.DEFAULT_PORT);

      // load port from settings
      using (MediaPortal.Profile.Settings reader = new MediaPortal.Profile.Settings(Config.GetFile(Config.Dir.Config, "MediaPortal.xml")))
      {
        originalPort = reader.GetValue(WifiRemote.PLUGIN_NAME, "port");
        checkBoxDisableBonjour.Checked = reader.GetValueAsBool(WifiRemote.PLUGIN_NAME, "disableBonjour", false);
        textBoxName.Text = reader.GetValueAsString(WifiRemote.PLUGIN_NAME, "serviceName", WifiRemote.GetServiceName());
        checkBoxShowConnectionMessage.Checked = reader.GetValueAsBool(WifiRemote.PLUGIN_NAME, "showNotifications", false);

        txtUsername.Text = WifiRemote.DecryptString(reader.GetValueAsString(WifiRemote.PLUGIN_NAME, "username", ""));
        txtPassword.Text = WifiRemote.DecryptString(reader.GetValueAsString(WifiRemote.PLUGIN_NAME, "password", ""));
        txtPasscode.Text = WifiRemote.DecryptString(reader.GetValueAsString(WifiRemote.PLUGIN_NAME, "passcode", ""));

        cbAuthMethod.SelectedIndex = reader.GetValueAsInt(WifiRemote.PLUGIN_NAME, "auth", 0);
        numericUpDownAutologin.Value = reader.GetValueAsInt(WifiRemote.PLUGIN_NAME, "autologinTimeout", 0);



        resetPort();

        // Read plugin ids and convert them to int
        String[] savedPluginStrings = reader.GetValueAsString(WifiRemote.PLUGIN_NAME, "savedPlugins", "").Split('|');
        savedPlugins = new Dictionary<int, string>();

        for (int j = 0; j + 1 < savedPluginStrings.Length; j += 2)
        {
          // Add plugin id and name
          int i;
          if (int.TryParse(savedPluginStrings[j], out i))
          {
            try
            {
              savedPlugins.Add(i, savedPluginStrings[j + 1]);
            }
            catch (Exception e)
            {
              WifiRemote.LogMessage("Adding saved plugin from config failed: " + e.Message, WifiRemote.LogType.Debug);
            }
          }
        }

        // Read ignored plugins
        // Ignored by default: 
        //     -1: 
        //      0: home
        //   3005: GUITopbar
        // 730716: fanart handler
        String[] ignoredPluginsString = reader.GetValueAsString(WifiRemote.PLUGIN_NAME, "ignoredPlugins", "-1|0|3005|730716").Split('|');
        ignoredPluginsList = new List<int>();

        foreach (String pluginId in ignoredPluginsString)
        {
          int i;
          if (int.TryParse(pluginId, out i))
          {
            try
            {
              ignoredPluginsList.Add(i);
            }
            catch (Exception e)
            {
              WifiRemote.LogMessage("Adding ignored plugin from config failed: " + e.Message, WifiRemote.LogType.Debug);
            }
          }
        }

      }

      // Test if Bonjour is installed
      try
      {
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

      // Setup plugins list
      availablePlugins = new ArrayList();
      plugins = new ArrayList();
      pluginIcons = new ImageList
      {
        ImageSize = new Size(20, 20)
      };

      EnumerateWindowPlugins();
      LoadPlugins();
      LoadSettings();

      // Add saved plugins to list for ordering
      foreach (var aSavedPlugin in savedPlugins)
      {
        // Find saved plugin with this window id
        var query = from ItemTag p in plugins
                    where p.WindowId == aSavedPlugin.Key
                    select p;

        // Add the first found plugin to the list
        foreach (ItemTag plugin in query)
        {
          if (plugin.IsEnabled)
          {
            pluginsDataSource.Add(new WindowPlugin(aSavedPlugin.Value,
                                                   aSavedPlugin.Key,
                                                   (plugin.ActiveImage != null)
                                                        ? ImageHelper.imageToByteArray(plugin.ActiveImage, System.Drawing.Imaging.ImageFormat.Png)
                                                        : ImageHelper.imageToByteArray(Properties.Resources.NoPluginImage, System.Drawing.Imaging.ImageFormat.Png),
                                                   !ignoredPluginsList.Contains(aSavedPlugin.Key)));
          }
          break;
        }
      }

      // Add rest of the plugins to the list
      foreach (ItemTag plugin in plugins)
      {
        if (!savedPlugins.ContainsKey(plugin.WindowId))
        {
          addPluginToList(plugin);
        }
      }

      dataGridViewPluginList.AutoGenerateColumns = false;
      dataGridViewPluginList.AutoSize = true;
      dataGridViewPluginList.DataSource = pluginsDataSource;
      dataGridViewPluginList.CurrentCellDirtyStateChanged += new EventHandler(dataGridViewPluginList_CurrentCellDirtyStateChanged);

      DataGridViewCheckBoxColumn displayColumn = new DataGridViewCheckBoxColumn
      {
        ReadOnly = false,
        AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCellsExceptHeader,
        DataPropertyName = "DisplayPlugin",
        Name = ""
      };
      dataGridViewPluginList.Columns.Add(displayColumn);

      DataGridViewImageColumn iconColumn = new DataGridViewImageColumn(false)
      {
        ReadOnly = true,
        ImageLayout = DataGridViewImageCellLayout.Zoom,
        Width = 20,
        DataPropertyName = "Icon",
        Name = ""
      };
      dataGridViewPluginList.Columns.Add(iconColumn);

      DataGridViewColumn nameColumn = new DataGridViewTextBoxColumn
      {
        ReadOnly = false,
        MinimumWidth = 200,
        AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
        DataPropertyName = "Name",
        Name = "Plugin"
      };
      dataGridViewPluginList.Columns.Add(nameColumn);

    }

    /// <summary>
    /// Apparently checkboxes fire changed events only after the cell was left
    /// Let's do it manually.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void dataGridViewPluginList_CurrentCellDirtyStateChanged(object sender, EventArgs e)
    {
      if (dataGridViewPluginList.IsCurrentCellDirty && dataGridViewPluginList.CurrentCell is DataGridViewCheckBoxCell)
      {
        dataGridViewPluginList.CommitEdit(DataGridViewDataErrorContexts.Commit);
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
        xmlwriter.SetValue(WifiRemote.PLUGIN_NAME, "username", WifiRemote.EncryptString(txtUsername.Text));
        xmlwriter.SetValue(WifiRemote.PLUGIN_NAME, "password", WifiRemote.EncryptString(txtPassword.Text));
        xmlwriter.SetValue(WifiRemote.PLUGIN_NAME, "passcode", WifiRemote.EncryptString(txtPasscode.Text));
        xmlwriter.SetValue(WifiRemote.PLUGIN_NAME, "auth", cbAuthMethod.SelectedIndex);
        xmlwriter.SetValue(WifiRemote.PLUGIN_NAME, "autologinTimeout", numericUpDownAutologin.Value);
        xmlwriter.SetValueAsBool(WifiRemote.PLUGIN_NAME, "showNotifications", checkBoxShowConnectionMessage.Checked);
        // Save plugins order, custom names and if they should be displayed
        List<string> pluginIdsToSave = new List<String>();

        foreach (WindowPlugin plugin in pluginsDataSource)
        {
          pluginIdsToSave.Add(plugin.WindowId.ToString());
          pluginIdsToSave.Add(plugin.Name);

          // Don't display plugin
          if (!plugin.DisplayPlugin && !ignoredPluginsList.Contains(plugin.WindowId))
          {
            // Plugin disabled, add to ignored plugins list
            ignoredPluginsList.Add(plugin.WindowId);
          }
          else if (plugin.DisplayPlugin && ignoredPluginsList.Contains(plugin.WindowId))
          {
            // Plugin not disabled but on disabled list. Remove it.
            ignoredPluginsList.Remove(plugin.WindowId);
          }
        }
        xmlwriter.SetValue(WifiRemote.PLUGIN_NAME, "savedPlugins", String.Join("|", pluginIdsToSave.ToArray()));
        xmlwriter.SetValue(WifiRemote.PLUGIN_NAME, "ignoredPlugins", String.Join("|", ignoredPluginsList.ConvertAll<string>(x => x.ToString()).ToArray()));
      }
    }

    /// <summary>
    /// Reset the port to defaults onClick trigger
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void triggerPortReset(object sender, LinkLabelLinkClickedEventArgs e)
    {
      resetPort(true);
    }

    /// <summary>
    /// Set the port box to defaults
    /// </summary>
    private void resetPort()
    {
      resetPort(false);
    }

    /// <summary>
    /// Reset the port box to defaults
    /// </summary>
    /// <param name="toPluginDefaults">Use plugin defaults or saved value</param>
    private void resetPort(bool toPluginDefaults)
    {
      textBoxPort.Text = (originalPort != String.Empty && !toPluginDefaults) ? originalPort : WifiRemote.DEFAULT_PORT.ToString();
      checkPortBox();
    }

    /// <summary>
    /// Checks if a given port is already in use.
    /// </summary>
    /// <param name="port">The port to check</param>
    /// <returns>true if the port is in use, false otherwise</returns>
    private bool isPortInUse(int port)
    {
      IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
      TcpConnectionInformation[] tcpConnInfoArray = ipGlobalProperties.GetActiveTcpConnections();

      foreach (TcpConnectionInformation tcpi in tcpConnInfoArray)
      {
        if (tcpi.LocalEndPoint.Port == port)
        {
          return true;
        }
      }

      return false;
    }

    /// <summary>
    /// Check if the port textbox contains a port
    /// that is already in use and display a 
    /// warning label.
    /// </summary>
    private void checkPortBox()
    {
      try
      {
        labelPortInUse.Visible = isPortInUse(Int32.Parse(textBoxPort.Text));
      }
      catch (Exception)
      {
        labelPortInUse.Visible = false;
      }
    }

    /// <summary>
    /// The port box was left, check its contents
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void textBoxPort_Leave(object sender, EventArgs e)
    {
      checkPortBox();
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
      catch (Exception) { }
    }

    #endregion

    #region Plugin List

    /// <summary>
    /// Add a plugin to the display list if it is active
    /// </summary>
    /// <param name="plugin"></param>
    private void addPluginToList(ItemTag plugin)
    {
      if (plugin.IsEnabled)
      {
        pluginsDataSource.Add(
            new WindowPlugin(plugin.SetupForm.PluginName(),
                             plugin.WindowId,
                             (plugin.ActiveImage != null) ? ImageHelper.imageToByteArray(plugin.ActiveImage, System.Drawing.Imaging.ImageFormat.Png)
                                                          : ImageHelper.imageToByteArray(Properties.Resources.NoPluginImage, System.Drawing.Imaging.ImageFormat.Png),
                             !ignoredPluginsList.Contains(plugin.WindowId)));
      }
    }

    /// <summary>
    /// List all window plugin dll's
    /// </summary>
    private void EnumerateWindowPlugins()
    {
      try
      {
        string directory = Config.GetSubFolder(Config.Dir.Plugins, "windows");

        if (Directory.Exists(directory))
        {
          //
          // Enumerate files
          //
          string[] files = Directory.GetFiles(directory, "*.dll");

          //
          // Add to list
          //
          foreach (string file in files)
          {
            availablePlugins.Add(file);
          }
        }
      }
      catch (Exception e)
      {
        Log.Error("[WifiRemote Setup] error enumerating windows plugins: " + e.Message);
      }
    }

    /// <summary>
    /// Load available window plugins in a list
    /// 
    /// Copied and modified from MediaPortal config: PluginsNew.cs
    /// </summary>
    private void LoadPlugins()
    {
      try
      {
        foreach (string pluginFile in availablePlugins)
        {
          Assembly pluginAssembly = null;
          try
          {
            pluginAssembly = Assembly.LoadFrom(pluginFile);
          }
          catch (BadImageFormatException)
          {
            Log.Warn("[WifiRemote Setup] {0} has a bad image format", pluginFile);
          }

          if (pluginAssembly != null)
          {
            try
            {
              Type[] exportedTypes = pluginAssembly.GetExportedTypes();

              foreach (Type type in exportedTypes)
              {
                bool isPlugin = (type.GetInterface("MediaPortal.GUI.Library.ISetupForm") != null);
                bool isGuiWindow = ((type.IsClass) && (type.IsSubclassOf(typeof(GUIWindow))));

                // an abstract class cannot be instanciated
                if (type.IsAbstract)
                {
                  continue;
                }

                // Try to locate the interface we're interested in
                if (isPlugin || isGuiWindow)
                {
                  // Create instance of the current type
                  object pluginObject;
                  try
                  {
                    pluginObject = Activator.CreateInstance(type);
                  }
                  catch (TargetInvocationException)
                  {
                    // Plugin is incompatible with current MediaPortal
                    Log.Error("[WifiRemote Setup] Plugin " + pluginFile + " incompatible");
                    continue;
                  }

                  if (isPlugin)
                  {

                    ISetupForm pluginForm = pluginObject as ISetupForm;

                    if (pluginForm != null)
                    {
                      ItemTag tag = new ItemTag
                      {
                        SetupForm = pluginForm,
                        DllName = pluginFile.Substring(pluginFile.LastIndexOf(@"\") + 1),
                        WindowId = pluginForm.GetWindowId()
                      };

                      if (isGuiWindow)
                      {
                        GUIWindow win = (GUIWindow)pluginObject;
                        if (tag.WindowId == win.GetID)
                        {
                          tag.Type = win.GetType().ToString();
                        }
                      }

                      LoadPluginImages(type, tag);
                      plugins.Add(tag);
                    }
                  }
                }
              }
            }
            catch (Exception ex)
            {
              // plugin broken or incompatible
              Log.Error("[WifiRemote Setup] Exception: {0}", ex);
            }
          }
        }
      }
      catch (Exception excep)
      {
        Log.Error("[WifiRemote Setup] Error loading plugins: " + excep.Message);
      }
    }


    /// <summary>
    /// Checks whether the a plugin has a <see cref="PluginIconsAttribute"/> defined.  If it has, the images that are indicated
    /// in the attribute are loaded
    /// 
    /// Copied from MediaPortal setup, see PluginsNew.cs
    /// </summary>
    /// <param name="type">The <see cref="Type"/> to examine.</param>
    /// <param name="tag">The <see cref="ItemTag"/> to store the images in.</param>
    private static void LoadPluginImages(Type type, ItemTag tag)
    {
      PluginIconsAttribute[] icons =
        (PluginIconsAttribute[])type.GetCustomAttributes(typeof(PluginIconsAttribute), false);
      if (icons == null || icons.Length == 0)
      {
        return;
      }
      string resourceName = icons[0].ActivatedResourceName;
      if (!string.IsNullOrEmpty(resourceName))
      {
        tag.ActiveImage = LoadImageFromResource(type, resourceName);
      }
    }

    /// <summary>
    /// Load an image from a plugin ressource
    /// 
    /// Copied from MediaPortal setup, see PluginsNew.cs
    /// </summary>
    /// <param name="type"></param>
    /// <param name="resourceName"></param>
    /// <returns></returns>
    private static Image LoadImageFromResource(Type type, string resourceName)
    {
      try
      {
        return Image.FromStream(type.Assembly.GetManifestResourceStream(resourceName));
      }
      catch (ArgumentException aex)
      {
        Log.Error("[WifiRemote Setup] Argument Exception loading the image - {0}, {1}", resourceName, aex.Message);
        //Thrown when the stream does not seem to contain a valid image
      }
      catch (FileLoadException lex)
      {
        Log.Error("[WifiRemote Setup] FileLoad Exception loading the image - {0}, {1}", resourceName, lex.Message);
        //Throw when the resource could not be loaded
      }
      catch (FileNotFoundException fex)
      {
        Log.Error("[WifiRemote Setup] FileNotFound Exception loading the image - {0}, {1}", resourceName, fex.Message);
        //Thrown when the resource could not be found
      }
      return null;
    }

    /// <summary>
    /// Check in config if plugin is enabled
    /// </summary>
    private void LoadSettings()
    {
      using (Settings xmlreader = new MPSettings())
      {
        foreach (ItemTag itemTag in plugins)
        {
          if (itemTag.SetupForm != null)
          {
            if (itemTag.SetupForm.CanEnable() || itemTag.SetupForm.DefaultEnabled())
            {
              itemTag.IsEnabled =
                xmlreader.GetValueAsBool("plugins", itemTag.SetupForm.PluginName(), itemTag.SetupForm.DefaultEnabled());
            }
            else
            {
              itemTag.IsEnabled = itemTag.SetupForm.DefaultEnabled();
            }
          }
        }
      }
    }

    #endregion

    #region Authentication

    private void cbAuthMethod_SelectedIndexChanged(object sender, EventArgs e)
    {
      switch (cbAuthMethod.SelectedIndex)
      {
        case 0:
          groupUsernamePassword.Enabled = false;
          groupPasscode.Enabled = false;
          break;
        case 1:
          groupUsernamePassword.Enabled = true;
          groupPasscode.Enabled = false;
          break;
        case 2:
          groupUsernamePassword.Enabled = false;
          groupPasscode.Enabled = true;
          break;
        case 3:
          groupUsernamePassword.Enabled = true;
          groupPasscode.Enabled = true;
          break;

      }
    }

    #endregion

    #region Barcode
    /// <summary>
    /// Save the generated barcode as image file to harddisk
    /// </summary>
    /// <param name="sender">sender</param>
    /// <param name="e">event args</param>
    private void btnSaveBarcode_Click(object sender, EventArgs e)
    {
      SaveFileDialog diag = new SaveFileDialog
      {
        Filter = "JPEG Image|*.jpg",
        Title = "Save Barcode as Image File"
      };
      if (diag.ShowDialog() == System.Windows.Forms.DialogResult.OK)
      {
        pbQrCode.Image.Save(diag.FileName);
      }
    }

    /// <summary>
    /// Generate a QR Barcode with the server information
    /// </summary>
    private void GenerateBarcode()
    {
      try
      {
        ServerDescription desc = new ServerDescription
        {
          Port = Int32.Parse(textBoxPort.Text),
          Name = textBoxName.Text,
          HardwareAddresses = WifiRemote.GetHardwareAddresses(),
          Hostname = WifiRemote.GetServiceName()
        };

        IPHostEntry host;
        String localIP = "?";
        StringBuilder localIPs = new StringBuilder();
        host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (IPAddress ip in host.AddressList)
        {
          if (ip.AddressFamily == AddressFamily.InterNetwork || ip.AddressFamily == AddressFamily.InterNetworkV6)
          {
            // Single address field
            localIP = ip.ToString();

            // Multiple addresses field
            if (localIPs.Length > 0)
            {
              localIPs.Append(";");
            }

            localIPs.Append(ip.ToString());
          }
        }

        desc.Address = localIP;
        desc.Addresses = (localIPs.Length > 0) ? localIPs.ToString() : "?";

        desc.AuthOptions = cbAuthMethod.SelectedIndex;
        if (checkBoxIncludeAuth.Checked)
        {
          desc.User = txtUsername.Text;
          desc.Password = txtPassword.Text;
          desc.Passcode = txtPasscode.Text;
        }

        Bitmap bm = QRCodeGenerator.Generate(JsonConvert.SerializeObject(desc));

        pbQrCode.Image = bm;
      }
      catch (Exception ex)
      {
        Log.Error("[WifiRemote Setup] Error generating barcode: {0}", ex.Message);
      }
    }

    /// <summary>
    /// Checkbox that controls if we integrated authentication information in the barcode
    /// </summary>
    /// <param name="sender">sender</param>
    /// <param name="e">event args</param>
    private void checkBoxIncludeAuth_CheckedChanged(object sender, EventArgs e)
    {
      GenerateBarcode();
    }


    /// <summary>
    /// Tab changed
    /// </summary>
    /// <param name="sender">sender</param>
    /// <param name="e">event args</param>
    private void tabControlNavigation_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (tabControlNavigation.SelectedTab == tabPageQRCode)
      {
        //tab changed to (QR Code), generate barcode
        GenerateBarcode();
      }
    }
    #endregion


    #region Plugin sorting
    // See: http://stackoverflow.com/questions/1620947/how-could-i-drag-and-drop-datagridview-rows-under-each-other/1623968#1623968

    private void dataGridViewPluginList_MouseDown(object sender, MouseEventArgs e)
    {
      // Get the index of the item the mouse is below.
      rowIndexFromMouseDown = dataGridViewPluginList.HitTest(e.X, e.Y).RowIndex;
      if (rowIndexFromMouseDown != -1)
      {
        // Remember the point where the mouse down occurred. 
        // The DragSize indicates the size that the mouse can move 
        // before a drag event should be started.                
        Size dragSize = SystemInformation.DragSize;

        // Create a rectangle using the DragSize, with the mouse position being
        // at the center of the rectangle.
        dragBoxFromMouseDown = new Rectangle(new Point(e.X - (dragSize.Width / 2),
                                                       e.Y - (dragSize.Height / 2)),
                                                dragSize);
      }
      else
      {
        // Reset the rectangle if the mouse is not over an item in the ListBox.
        dragBoxFromMouseDown = Rectangle.Empty;
      }
    }

    private void dataGridViewPluginList_MouseMove(object sender, MouseEventArgs e)
    {
      if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
      {
        // If the mouse moves outside the rectangle, start the drag.
        if (dragBoxFromMouseDown != Rectangle.Empty &&
            !dragBoxFromMouseDown.Contains(e.X, e.Y))
        {

          // Proceed with the drag and drop, passing in the list item.                    
          DragDropEffects dropEffect = dataGridViewPluginList.DoDragDrop(
              dataGridViewPluginList.Rows[rowIndexFromMouseDown],
              DragDropEffects.Move);
        }
      }
    }

    private void dataGridViewPluginList_DragOver(object sender, DragEventArgs e)
    {
      e.Effect = DragDropEffects.Move;
    }

    private void dataGridViewPluginList_DragDrop(object sender, DragEventArgs e)
    {
      // The mouse locations are relative to the screen, so they must be 
      // converted to client coordinates.
      Point clientPoint = dataGridViewPluginList.PointToClient(new Point(e.X, e.Y));

      // Get the row index of the item the mouse is below. 
      rowIndexOfItemUnderMouseToDrop =
          dataGridViewPluginList.HitTest(clientPoint.X, clientPoint.Y).RowIndex;

      // If the drag operation was a move then remove and insert the row.
      if (e.Effect == DragDropEffects.Move && pluginsDataSource.Count > rowIndexFromMouseDown)
      {
        WindowPlugin pluginToMove = (WindowPlugin)pluginsDataSource[rowIndexFromMouseDown];
        pluginsDataSource.RemoveAt(rowIndexFromMouseDown);
        pluginsDataSource.Insert(rowIndexOfItemUnderMouseToDrop, pluginToMove);
      }

    }

    #endregion
  }
}
