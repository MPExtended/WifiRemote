using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Threading;

namespace WifiRemote.MPDialogs
{
    /// <summary>
    /// Handler for showdialog messages
    /// </summary>
    public class ShowDialogMessageHandler
    {
         /// <summary>
        /// Handle a "showdialog" message received from a client
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="socketServer">Socket server</param>
        /// <param name="sender">Sender</param>
        internal static void HandleShowDialogMessage(Newtonsoft.Json.Linq.JObject message, SocketServer socketServer, Deusty.Net.AsyncSocket sender)
        {
            String dialogType = (String)message["DialogType"];
            String dialogId = (String)message["DialogId"];

            if (dialogType != null)
            {
                String title = (String)message["Title"];
                String text = (String)message["Text"];

                if (dialogType.Equals("yesno"))
                {
                    //show dialog in new thread so we don't block the tcp thread
                    new Thread(new ParameterizedThreadStart(ShowYesNoThreaded)).Start(new object[] { dialogId, title, text, socketServer, sender });
                }
                else if (dialogType.Equals("yesnoselect"))
                {
                    List<String> options = new List<String>();
                    JArray array = (JArray)message["Options"];
                    if (array != null)
                    {
                        foreach (JValue v in array)
                        {
                            options.Add((string)v.Value);
                        }
                    }

                    //show dialog in new thread so we don't block the tcp thread
                    new Thread(new ParameterizedThreadStart(ShowYesNoThenSelectThreaded)).Start(new object[] { dialogId, title, text, options, socketServer, sender });    
                }
                else if (dialogType.Equals("select"))
                {
                    List<String> options = new List<String>();
                    JArray array = (JArray)message["Options"];
                    if (array != null)
                    {
                        foreach (JValue v in array)
                        {
                            options.Add((string)v.Value);
                        }
                    }

                    //show dialog in new thread so we don't block the tcp thread
                    new Thread(new ParameterizedThreadStart(ShowSelectThreaded)).Start(new object[] { dialogId, title, options, socketServer, sender });
                }
                else
                {
                    WifiRemote.LogMessage("Dialog type " + dialogType + " not supported yet", WifiRemote.LogType.Warn);
                }
            }
            else
            {
                WifiRemote.LogMessage("No dialog type specified", WifiRemote.LogType.Warn);
            }
        }

        /// <summary>
        /// Show yes/no dialog from a new thread
        /// </summary>
        /// <param name="pars">parameters for ParameterizedThreadStart</param>
        private static void ShowYesNoThreaded(object pars)
        {
            object[] parameters = (object[])pars;
            ShowDialogHelper.ShowYesNoDialog((string)parameters[0], (string)parameters[1], (string)parameters[2], (SocketServer)parameters[3], (Deusty.Net.AsyncSocket)parameters[4]);
        }

        /// <summary>
        /// Show yes/no-then-select dialog from a new thread
        /// </summary>
        /// <param name="pars">parameters for ParameterizedThreadStart</param>
        private static void ShowYesNoThenSelectThreaded(object pars)
        {
            object[] parameters = (object[])pars;
            ShowDialogHelper.ShowYesNoThenSelectDialog((string)parameters[0], (string)parameters[1], (string)parameters[2], (List<String>)parameters[3], (SocketServer)parameters[4], (Deusty.Net.AsyncSocket)parameters[5]);
        }

        /// <summary>
        /// Show select dialog from a new thread
        /// </summary>
        /// <param name="pars">parameters for ParameterizedThreadStart</param>
        private static void ShowSelectThreaded(object pars)
        {
            object[] parameters = (object[])pars;
            ShowDialogHelper.ShowSelectDialog((string)parameters[0], (string)parameters[1], (List<String>)parameters[2], (SocketServer)parameters[3], (Deusty.Net.AsyncSocket)parameters[4]);
        }
    }
}
