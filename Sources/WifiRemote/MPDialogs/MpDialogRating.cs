using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MediaPortal.GUI.Library;

namespace WifiRemote.MPDialogs
{
    public class MpDialogRating : MpDialog
    {
        MediaPortal.Dialogs.GUIDialogSetRating mpDialog;
        public MpDialogRating(MediaPortal.Dialogs.GUIDialogSetRating dialog)
            : base(dialog)
        {
            this.mpDialog = dialog;
            this.DialogType = dialog.GetModuleName();
            this.DialogId = dialog.GetID;
            this.Rating = dialog.Rating;
            GetHeading(dialog, 2);
            GetText(dialog, 4);

            this.AvailableActions.Add("ok");
            this.AvailableActions.Add("cancel");
            this.AvailableActions.Add("setrating");
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
            if (action.Equals("yes"))
            {
                GUIMessage msg = new GUIMessage(GUIMessage.MessageType.GUI_MSG_CLICKED, 0, 11, 11, 0, 0, null);
                mpDialog.OnMessage(msg);
            }

            if (action.Equals("setrating"))
            {
                SetRating(index);
            }
        }

        /// <summary>
        /// Set Rating for this dialog
        /// </summary>
        /// <param name="rating"></param>
        public void SetRating(int rating)
        {
            if (mpDialog != null)
            {
                mpDialog.Rating = rating;
            }
        }
    }
}
