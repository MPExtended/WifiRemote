using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MediaPortal.Dialogs;
using MediaPortal.GUI.Library;
using WifiRemote.Messages;

namespace WifiRemote.MPDialogs
{
    /// <summary>
    /// Helper class for showing dialogs on mp
    /// </summary>
    class ShowDialogHelper
    {
        /// <summary>
        /// Show a yes no dialog in MediaPortal and send the result to the sender
        /// </summary>
        /// <param name="dialogId">Id of dialog</param>
        /// <param name="title">Dialog title</param>
        /// <param name="text">Dialog text</param>
        /// <param name="socketServer">Server</param>
        /// <param name="sender">Sender of the request</param>
        internal static void ShowYesNoDialog(string dialogId, string title, string text, SocketServer socketServer, Deusty.Net.AsyncSocket sender)
        {
            GUIDialogYesNo dlg = (GUIDialogYesNo)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_YES_NO);
            if (dlg != null)
            {
                dlg.Reset();
                dlg.SetHeading(title);
                dlg.SetLine(1, text);
                dlg.DoModal(GUIWindowManager.ActiveWindow);

                MessageDialogResult result = new MessageDialogResult();
                result.YesNoResult = dlg.IsConfirmed;
                result.DialogId = dialogId;

                socketServer.SendMessageToClient(result, sender);
            }
        }

        /// <summary>
        /// Show an ok dialog in MediaPortal
        /// </summary>
        /// <param name="title">Dialog title</param>
        /// <param name="text">Dialog text</param>
        internal static void ShowOkDialog(string title, string text)
        {
            GUIDialogOK dlg = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
            if (dlg != null)
            {
                dlg.Reset();
                dlg.SetHeading(title);
                dlg.SetLine(1, text);
                dlg.DoModal(GUIWindowManager.ActiveWindow);
            }
        }

        /// <summary>
        /// Show a select dialog in MediaPortal. After that, send the result to the sender.
        /// </summary>
        /// <param name="dialogId">Id of dialog</param>
        /// <param name="title">Dialog title</param>
        /// <param name="text">Dialog text</param>
        /// <param name="options">Options for the user to choose from</param>
        /// <param name="socketServer">Server</param>
        /// <param name="sender">Sender of the request</param>
        internal static void ShowSelectDialog(string dialogId, string title,  List<string> options, SocketServer socketServer, Deusty.Net.AsyncSocket sender)
        {
            GUIDialogMenu dlgMenu = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
                       MessageDialogResult result = new MessageDialogResult();
            result.DialogId = dialogId;
            result.YesNoResult = false;

            if (dlgMenu != null)
            {
                dlgMenu.Reset();
                dlgMenu.SetHeading(title);

                if (options != null)
                {
                    foreach (string o in options)
                    {
                        dlgMenu.Add(o);
                    }
                }

                dlgMenu.DoModal(GUIWindowManager.ActiveWindow);

                if (dlgMenu.SelectedId != -1)
                {
                    result.YesNoResult = true;
                    result.SelectedOption = dlgMenu.SelectedLabelText;
                }
            }
        }

        /// <summary>
        /// Show a yes/no dialog and if the user accepts, show a select dialog. After that, send the result to the sender.
        /// </summary>
        /// <param name="dialogId">Id of dialog</param>
        /// <param name="title">Dialog title</param>
        /// <param name="text">Dialog text</param>
        /// <param name="options">Options for the user to choose from</param>
        /// <param name="socketServer">Server</param>
        /// <param name="sender">Sender of the request</param>
        internal static void ShowYesNoThenSelectDialog(string dialogId, string title, string text, List<string> options, SocketServer socketServer, Deusty.Net.AsyncSocket sender)
        {
            GUIDialogYesNo dlg = (GUIDialogYesNo)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_YES_NO);
            MessageDialogResult result = new MessageDialogResult();
            result.DialogId = dialogId;
            result.YesNoResult = false;

            if (dlg != null)
            {
                dlg.Reset();
                dlg.SetHeading(title);
                dlg.SetLine(1, text);
                dlg.DoModal(GUIWindowManager.ActiveWindow);
            }

            if (dlg.IsConfirmed && options != null && options.Count > 0)
            {
                if (options.Count == 1)
                {
                    //only one option, no need to show select dialog
                    result.SelectedOption = options[0];
                    result.YesNoResult = true;
                }
                else
                {
                    //multiple options available, show select menu to user
                    GUIDialogMenu dlgMenu = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
                    if (dlgMenu != null)
                    {
                        dlgMenu.Reset();
                        
                        dlgMenu.SetHeading(title);

                        if (options != null)
                        {
                            foreach (string o in options)
                            {
                                dlgMenu.Add(o);
                            }
                        }


                        //dlg.SetLine(1, text);
                        dlgMenu.DoModal(GUIWindowManager.ActiveWindow);

                        if (dlgMenu.SelectedId != -1)
                        {
                            result.YesNoResult = true;
                            result.SelectedOption = dlgMenu.SelectedLabelText;
                        }
                    }
                }
            }

            socketServer.SendMessageToClient(result, sender);
        }
    }
}
