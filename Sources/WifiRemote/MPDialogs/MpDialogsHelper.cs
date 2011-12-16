using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MediaPortal.GUI.Library;
using MediaPortal.Dialogs;
using System.Threading;
using Deusty.Net;
using WifiRemote.Messages;

namespace WifiRemote.MPDialogs
{
    public class MpDialogsHelper
    {
        /// <summary>
        /// Id of TvSeries rating dialog
        /// </summary>
        private const int TVSERIES_RATING_ID = 9814;

        /// <summary>
        /// Id of TvSeries pin dialog
        /// </summary>
        private const int TVSERIES_PIN_ID = 9815;

        /// <summary>
        /// Id of MovingPictures rating dialog
        /// </summary>
        private const int MOPI_RATING_ID = 28380;

        /// <summary>
        /// Id of MovingPictures pin dialog
        /// </summary>
        private const int MOPI_PIN_ID = 9915;

        /// <summary>
        /// Is a dialog currently shown
        /// </summary>
        public static bool IsDialogShown { get; set; }

        /// <summary>
        /// The currently shown dialog
        /// </summary>
        public static GUIDialogWindow CurrentDialog { get; set; }

        /// <summary>
        /// Get the internal dialog object from the given GUIDialogWindow
        /// </summary>
        /// <param name="dialog">Dialog</param>
        /// <returns>WifiRemote MP Dialog</returns>
        public static MpDialog GetDialog(GUIDialogWindow dialog)
        {
            if (dialog.GetType().Equals(typeof(GUIDialogOK)))
            {
                return GetDialogOk();
            }
            else if (dialog.GetType().Equals(typeof(GUIDialogYesNo)))
            {
                return GetDialogYesNo();
            }
            else if (dialog.GetType().Equals(typeof(GUIDialogMenu)))
            {
                return GetDialogMenu();
            }
            else if (dialog.GetType().Equals(typeof(GUIDialogNotify)))
            {
                return GetDialogNotify();
            }
            else if (dialog.GetType().Equals(typeof(GUIDialogSetRating)))
            {
                return GetDialogRating();
            }
            else if (dialog.GetType().Equals(typeof(GUIDialogSelect)))
            {
                return GetDialogSelect();
            }
            if (WifiRemote.IsAvailableTVSeries)
            {
                if (TVSeriesHelper.IsTvSeriesRatingDialog(dialog))
                {
                    return GetDialogMpTvSeriesRating();
                }
                if (TVSeriesHelper.IsTvSeriesPinDialog(dialog))
                {
                    return GetDialogMpTvSeriesPin();
                }
            }
            if (WifiRemote.IsAvailableMovingPictures)
            {
                if (MovingPicturesHelper.IsMovingPictureRatingDialog(dialog))
                {
                    return GetDialogMovingPicturesRating();
                }
                else if (MovingPicturesHelper.IsMovingPicturePinDialog(dialog))
                {
                    return GetDialogMovingPicturesPin();
                }
            }
            return null;
        }

        /// <summary>
        /// Get WifiRemote representation of the MpTvSeries rating dialog
        /// </summary>
        /// <returns>WifiRemote Dialog Instance</returns>
        public static MpDialogTvSeriesRating GetDialogMpTvSeriesRating()
        {
            WindowPlugins.GUITVSeries.GUIUserRating menu = (WindowPlugins.GUITVSeries.GUIUserRating)GUIWindowManager.GetWindow(TVSERIES_RATING_ID);
            MpDialogTvSeriesRating ratingDialog = new MpDialogTvSeriesRating(menu);
            return ratingDialog;
        }

        /// <summary>
        /// Get WifiRemote representation of the MpTvSeries pin dialog
        /// </summary>
        /// <returns>WifiRemote Dialog Instance</returns>
        public static MpDialogTvSeriesPin GetDialogMpTvSeriesPin()
        {
            WindowPlugins.GUITVSeries.GUIPinCode menu = (WindowPlugins.GUITVSeries.GUIPinCode)GUIWindowManager.GetWindow(TVSERIES_PIN_ID);
            MpDialogTvSeriesPin ratingDialog = new MpDialogTvSeriesPin(menu);
            return ratingDialog;
        }

        /// <summary>
        /// Get WifiRemote representation of the MpTvSeries rating dialog
        /// </summary>
        /// <returns>WifiRemote Dialog Instance</returns>
        public static MpDialogMovingPicturesRating GetDialogMovingPicturesRating()
        {
            Cornerstone.MP.GUIGeneralRating menu = (Cornerstone.MP.GUIGeneralRating)GUIWindowManager.GetWindow(MOPI_RATING_ID);
            MpDialogMovingPicturesRating ratingDialog = new MpDialogMovingPicturesRating(menu);
            return ratingDialog;
        }

        /// <summary>
        /// Get WifiRemote representation of the MpTvSeries pin dialog
        /// </summary>
        /// <returns>WifiRemote Dialog Instance</returns>
        public static MpDialogMovingPicturesPin GetDialogMovingPicturesPin()
        {
            Cornerstone.MP.GUIPinCodeDialog menu = (Cornerstone.MP.GUIPinCodeDialog)GUIWindowManager.GetWindow(MOPI_PIN_ID);
            MpDialogMovingPicturesPin pinDialog = new MpDialogMovingPicturesPin(menu);
            return pinDialog;
        }

        /// <summary>
        /// Get WifiRemote representation of the select dialog
        /// </summary>
        /// <returns>WifiRemote Dialog Instance</returns>
        public static MpDialogSelect GetDialogSelect()
        {
            GUIDialogSelect menu = (GUIDialogSelect)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_SELECT);
            MpDialogSelect notifyDialog = new MpDialogSelect(menu);
            return notifyDialog;
        }

        /// <summary>
        /// Get WifiRemote representation of the rating dialog
        /// </summary>
        /// <returns>WifiRemote Dialog Instance</returns>
        public static MpDialogRating GetDialogRating()
        {
            GUIDialogSetRating menu = (GUIDialogSetRating)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_RATING);
            MpDialogRating notifyDialog = new MpDialogRating(menu);
            return notifyDialog;
        }

        /// <summary>
        /// Get WifiRemote representation of the notify dialog
        /// </summary>
        /// <returns>WifiRemote Dialog Instance</returns>
        public static MpDialogNotify GetDialogNotify()
        {
            GUIDialogNotify menu = (GUIDialogNotify)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_NOTIFY);
            MpDialogNotify notifyDialog = new MpDialogNotify(menu);
            return notifyDialog;
        }

        /// <summary>
        /// Get WifiRemote representation of the menu dialog
        /// </summary>
        /// <returns>WifiRemote Dialog Instance</returns>
        public static MpDialogMenu GetDialogMenu()
        {
            GUIDialogMenu menu = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
            MpDialogMenu notifyMenu = new MpDialogMenu(menu);
            return notifyMenu;
        }

        /// <summary>
        /// Get WifiRemote representation of the yesno dialog
        /// </summary>
        /// <returns>WifiRemote Dialog Instance</returns>
        public static MpDialogYesNo GetDialogYesNo()
        {
            GUIDialogYesNo menu = (GUIDialogYesNo)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_YES_NO);
            MpDialogYesNo okDialog = new MpDialogYesNo(menu);
            return okDialog;
        }

        /// <summary>
        /// Get WifiRemote representation of the ok dialog
        /// </summary>
        /// <returns>WifiRemote Dialog Instance</returns>
        public static MpDialogOk GetDialogOk()
        {
            GUIDialogOK menu = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
            MpDialogOk okDialog = new MpDialogOk(menu);
            return okDialog;
        }

        /// <summary>
        /// Handle the dialog action
        /// </summary>
        /// <param name="message">Message from Client</param>
        internal static void HandleDialogAction(Newtonsoft.Json.Linq.JObject message, SocketServer server, AsyncSocket client)
        {
            String action = (string)message["ActionType"];
            String dialogType = (string)message["DialogType"];
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
                    MpDialogMenu diag = GetDialogMenu();
                    diag.HandleAction(action, index);
                }
                else if (dialogId == (int)GUIWindow.Window.WINDOW_DIALOG_OK)
                {
                    MpDialogOk diag = GetDialogOk();
                    diag.HandleAction(action, index);
                }
                else if (dialogId == (int)GUIWindow.Window.WINDOW_DIALOG_YES_NO)
                {
                    MpDialogYesNo diag = GetDialogYesNo();
                    diag.HandleAction(action, index);
                }
                else if (dialogId == (int)GUIWindow.Window.WINDOW_DIALOG_NOTIFY)
                {
                    MpDialogNotify diag = GetDialogNotify();
                    diag.HandleAction(action, index);
                }
                else if (dialogId == (int)GUIWindow.Window.WINDOW_DIALOG_RATING)
                {
                    MpDialogRating diag = GetDialogRating();
                    diag.HandleAction(action, index);
                }
                else if (dialogId == TVSERIES_RATING_ID)
                {
                    if (WifiRemote.IsAvailableTVSeries)
                    {
                        MpDialogTvSeriesRating diag = GetDialogMpTvSeriesRating();
                        diag.HandleAction(action, index);
                    }
                }
                else if (dialogId == TVSERIES_PIN_ID)
                {
                    if (WifiRemote.IsAvailableTVSeries)
                    {
                        MpDialogTvSeriesPin diag = GetDialogMpTvSeriesPin();
                        diag.HandleAction(action, index);
                    }
                }
                else if (dialogId == MOPI_RATING_ID)
                {
                    if (WifiRemote.IsAvailableMovingPictures)
                    {
                        MpDialogMovingPicturesRating diag = GetDialogMovingPicturesRating();
                        diag.HandleAction(action, index);
                    }
                }
                else if (dialogId == MOPI_PIN_ID)
                {
                    if (WifiRemote.IsAvailableMovingPictures)
                    {
                        MpDialogMovingPicturesPin diag = GetDialogMovingPicturesPin();
                        diag.HandleAction(action, index);
                    }
                }
            }
        }

        /// <summary>
        /// Get the WifiRemote message from the given dialog
        /// </summary>
        /// <param name="dialog">The MP dialog</param>
        /// <returns>WifiRemote dialog message</returns>
        internal static Messages.MessageDialog GetDialogMessage(GUIDialogWindow dialog)
        {
            MpDialog diag = MpDialogsHelper.GetDialog(dialog);
            MessageDialog message = new MessageDialog();
            message.DialogShown = true;
            message.Dialog = diag;

            return message;
        }
    }
}
