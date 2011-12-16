using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindowPlugins.GUITVSeries;
using MediaPortal.GUI.Library;

namespace WifiRemote.MPDialogs
{
    public class MpDialogTvSeriesRating : MpDialog
    {
        GUIUserRating mpDialog;
        public MpDialogTvSeriesRating(GUIUserRating dialog)
            : base(dialog)
        {
            this.mpDialog = dialog;
            this.DialogType = dialog.GetModuleName();
            this.DialogId = dialog.GetID;
            this.Rating = dialog.Rating;
            this.RatingMax = (dialog._displayStars == GUIUserRating.StarDisplay.FIVE_STARS) ? 5 : 10;
            GetHeading(dialog, 1);
            GetText(dialog, 2);

            this.AvailableActions.Add("cancel");
            this.AvailableActions.Add("setrating");
            this.AvailableActions.Add("confirmrating");
        }

        /// <summary>
        /// Current Rating
        /// </summary>
        public int Rating { get; set; }

        /// <summary>
        /// Max Value for Ratings
        /// </summary>
        public int RatingMax { get; set; }

        /// <summary>
        /// Handle actions which are available on this dialog
        /// </summary>
        /// <param name="action">Action to execute</param>
        /// <param name="index">Index (e.g. needed for lists)</param>
        public override void HandleAction(String action, int index)
        {
            base.HandleAction(action, index);

            if (action.Equals("setrating"))
            {
                SetRating(index);
            }

            if (action.Equals("confirmrating"))
            {
                ConfirmRating();
            }
        }

        private void ConfirmRating()
        {
            MediaPortal.GUI.Library.Action confirmAction = new MediaPortal.GUI.Library.Action(MediaPortal.GUI.Library.Action.ActionType.ACTION_SELECT_ITEM, 0, 0);
            mpDialog.OnAction(confirmAction);
        }

        /// <summary>
        /// Set Rating for this dialog
        /// </summary>
        /// <param name="rating"></param>
        public void SetRating(int rating)
        {
            MediaPortal.GUI.Library.Action ratingAction = null; ;
            switch (rating)
            {
                case 1:
                    ratingAction = new MediaPortal.GUI.Library.Action(MediaPortal.GUI.Library.Action.ActionType.REMOTE_1, 0, 0);
                    break;
                case 2:
                    ratingAction = new MediaPortal.GUI.Library.Action(MediaPortal.GUI.Library.Action.ActionType.REMOTE_2, 0, 0);
                    break;
                case 3:
                    ratingAction = new MediaPortal.GUI.Library.Action(MediaPortal.GUI.Library.Action.ActionType.REMOTE_3, 0, 0);
                    break;
                case 4:
                    ratingAction = new MediaPortal.GUI.Library.Action(MediaPortal.GUI.Library.Action.ActionType.REMOTE_4, 0, 0);
                    break;
                case 5:
                    ratingAction = new MediaPortal.GUI.Library.Action(MediaPortal.GUI.Library.Action.ActionType.REMOTE_5, 0, 0);
                    break;
                case 6:
                    ratingAction = new MediaPortal.GUI.Library.Action(MediaPortal.GUI.Library.Action.ActionType.REMOTE_6, 0, 0);
                    break;
                case 7:
                    ratingAction = new MediaPortal.GUI.Library.Action(MediaPortal.GUI.Library.Action.ActionType.REMOTE_7, 0, 0);
                    break;
                case 8:
                    ratingAction = new MediaPortal.GUI.Library.Action(MediaPortal.GUI.Library.Action.ActionType.REMOTE_8, 0, 0);
                    break;
                case 9:
                    ratingAction = new MediaPortal.GUI.Library.Action(MediaPortal.GUI.Library.Action.ActionType.REMOTE_9, 0, 0);
                    break;
                case 10:
                    ratingAction = new MediaPortal.GUI.Library.Action(MediaPortal.GUI.Library.Action.ActionType.REMOTE_0, 0, 0);
                    break;
            }

            if (ratingAction != null)
            {
                mpDialog.OnAction(ratingAction);
            }
        }
    }
}
