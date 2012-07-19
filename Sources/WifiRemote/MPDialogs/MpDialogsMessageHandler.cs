using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MediaPortal.GUI.Library;
using WifiRemote.Messages;
using Deusty.Net;

namespace WifiRemote.MPDialogs
{
    /// <summary>
    /// Handler for dialog actions
    /// </summary>
    class MpDialogsMessageHandler
    {
        /// <summary>
        /// Handle the dialog action
        /// </summary>
        /// <param name="message">Message from Client</param>
        internal static void HandleDialogAction(Newtonsoft.Json.Linq.JObject message, SocketServer server, AsyncSocket client)
        {
            String action = (string)message["ActionType"];
            int dialogId = (int)message["DialogId"];
            int index = (int)message["Index"];

            if (action.Equals("get"))
            {
                if (MpDialogsHelper.IsDialogShown)
                {
                    MessageDialog msg = MpDialogsHelper.GetDialogMessage(MpDialogsHelper.CurrentDialog);
                    server.SendMessageToClient(msg, client);
                }
                else
                {
                    MessageDialog msg = new MessageDialog();
                    msg.DialogShown = false;
                    server.SendMessageToClient(msg, client);
                }
            }
            else
            {
                if (dialogId == (int)GUIWindow.Window.WINDOW_DIALOG_MENU)
                {
                    MpDialogMenu diag = MpDialogsHelper.GetDialogMenu();
                    diag.HandleAction(action, index);
                }
                else if (dialogId == (int)GUIWindow.Window.WINDOW_DIALOG_OK)
                {
                    MpDialogOk diag = MpDialogsHelper.GetDialogOk();
                    diag.HandleAction(action, index);
                }
                else if (dialogId == (int)GUIWindow.Window.WINDOW_DIALOG_YES_NO)
                {
                    MpDialogYesNo diag = MpDialogsHelper.GetDialogYesNo();
                    diag.HandleAction(action, index);
                }
                else if (dialogId == (int)GUIWindow.Window.WINDOW_DIALOG_NOTIFY)
                {
                    MpDialogNotify diag = MpDialogsHelper.GetDialogNotify();
                    diag.HandleAction(action, index);
                }
                else if (dialogId == (int)GUIWindow.Window.WINDOW_DIALOG_PROGRESS)
                {
                    MpDialogProgress diag = MpDialogsHelper.GetDialogProgress();
                    diag.HandleAction(action, index);
                }
                else if (dialogId == (int)GUIWindow.Window.WINDOW_DIALOG_RATING)
                {
                    MpDialogRating diag = MpDialogsHelper.GetDialogRating();
                    diag.HandleAction(action, index);
                }
                else if (dialogId == MpDialogsHelper.TVSERIES_RATING_ID)
                {
                    if (WifiRemote.IsAvailableTVSeries)
                    {
                        MpDialogTvSeriesRating diag = MpDialogsHelper.GetDialogMpTvSeriesRating();
                        diag.HandleAction(action, index);
                    }
                }
                else if (dialogId == MpDialogsHelper.TVSERIES_PIN_ID)
                {
                    if (WifiRemote.IsAvailableTVSeries)
                    {
                        MpDialogTvSeriesPin diag = MpDialogsHelper.GetDialogMpTvSeriesPin();
                        diag.HandleAction(action, index);
                    }
                }
                else if (dialogId == MpDialogsHelper.MOPI_RATING_ID)
                {
                    if (WifiRemote.IsAvailableMovingPictures)
                    {
                        MpDialogMovingPicturesRating diag = MpDialogsHelper.GetDialogMovingPicturesRating();
                        diag.HandleAction(action, index);
                    }
                }
                else if (dialogId == MpDialogsHelper.MOPI_PIN_ID)
                {
                    if (WifiRemote.IsAvailableMovingPictures)
                    {
                        MpDialogMovingPicturesPin diag = MpDialogsHelper.GetDialogMovingPicturesPin();
                        diag.HandleAction(action, index);
                    }
                }
            }
        }
    }
}
