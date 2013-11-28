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
        public const int TVSERIES_RATING_ID = 9814;

        /// <summary>
        /// Id of TvSeries pin dialog
        /// </summary>
        public const int TVSERIES_PIN_ID = 9815;

        /// <summary>
        /// Id of MovingPictures rating dialog
        /// </summary>
        public const int MOPI_RATING_ID = 28380;

        /// <summary>
        /// Id of MovingPictures pin dialog
        /// </summary>
        public const int MOPI_PIN_ID = 9915;

        /// <summary>
        /// Id of the Trakt rating dialog
        /// </summary>
        public const int TRAKT_RATING_ID = 87300;

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
            else if (dialog.GetType().Equals(typeof(GUIDialogProgress)))
            {
                return GetDialogProgress();
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
            if (WifiRemote.IsAvailableTrakt)
            {
                if (TraktHelper.IsTraktRatingDialog(dialog))
                {
                    return GetDialogTraktRating();
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
        /// Get WifiRemote representation of the MpMovingPictures rating dialog
        /// </summary>
        /// <returns>WifiRemote Dialog Instance</returns>
        public static MpDialogMovingPicturesRating GetDialogMovingPicturesRating()
        {
            Cornerstone.MP.GUIGeneralRating menu = (Cornerstone.MP.GUIGeneralRating)GUIWindowManager.GetWindow(MOPI_RATING_ID);
            MpDialogMovingPicturesRating ratingDialog = new MpDialogMovingPicturesRating(menu);
            return ratingDialog;
        }

        /// <summary>
        /// Get WifiRemote representation of the MpMovingPictures pin dialog
        /// </summary>
        /// <returns>WifiRemote Dialog Instance</returns>
        public static MpDialogMovingPicturesPin GetDialogMovingPicturesPin()
        {
            Cornerstone.MP.GUIPinCodeDialog menu = (Cornerstone.MP.GUIPinCodeDialog)GUIWindowManager.GetWindow(MOPI_PIN_ID);
            MpDialogMovingPicturesPin pinDialog = new MpDialogMovingPicturesPin(menu);
            return pinDialog;
        }

        /// <summary>
        /// Get WifiRemote representation of the MpTrakt rating dialog
        /// </summary>
        /// <returns>WifiRemote Dialog Instance</returns>
        public static MpDialogTraktRating GetDialogTraktRating()
        {
            TraktPlugin.GUI.GUIRateDialog menu = (TraktPlugin.GUI.GUIRateDialog)GUIWindowManager.GetWindow(TRAKT_RATING_ID);
            MpDialogTraktRating ratingDialog = new MpDialogTraktRating(menu);
            return ratingDialog;
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
        /// Get WifiRemote representation of the progress dialog
        /// </summary>
        /// <returns></returns>
        public static MpDialogProgress GetDialogProgress()
        {
            GUIDialogProgress menu = (GUIDialogProgress)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_PROGRESS);
            MpDialogProgress progressDialog = new MpDialogProgress(menu);
            return progressDialog;
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
