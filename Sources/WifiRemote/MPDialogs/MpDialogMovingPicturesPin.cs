using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WifiRemote.MPDialogs
{
    public class MpDialogMovingPicturesPin : MpDialog
    {
        Cornerstone.MP.GUIPinCodeDialog mpDialog;
        public MpDialogMovingPicturesPin(Cornerstone.MP.GUIPinCodeDialog dialog)
            : base(dialog)
        {
            this.mpDialog = dialog;
            this.DialogType = dialog.GetModuleName();
            this.DialogId = dialog.GetID;
            GetHeading(dialog, 1);
            GetText(dialog, 2, 3, 4, 5);

            this.AvailableActions.Add("cancel");
            this.AvailableActions.Add("setpin");
            this.AvailableActions.Add("deletepin");
            this.AvailableActions.Add("confirmpin");
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

            if (action.Equals("setpin"))
            {
                SetPin(index);
            }

            if (action.Equals("deletepin"))
            {
                RemovePin();
            }
        }

        public void RemovePin()
        {
            MediaPortal.GUI.Library.Action pinAction = new MediaPortal.GUI.Library.Action(MediaPortal.GUI.Library.Action.ActionType.ACTION_DELETE_ITEM, 0, 0);
            mpDialog.OnAction(pinAction);
        }

        public void SetPin(int pin)
        {
            MediaPortal.GUI.Library.Action.ActionType type = MediaPortal.GUI.Library.Action.ActionType.REMOTE_0;
            switch (pin)
            {
                case 0:
                    type = MediaPortal.GUI.Library.Action.ActionType.REMOTE_0;
                    break;
                case 1:
                    type = MediaPortal.GUI.Library.Action.ActionType.REMOTE_1;
                    break;
                case 2:
                    type = MediaPortal.GUI.Library.Action.ActionType.REMOTE_2;
                    break;
                case 3:
                    type = MediaPortal.GUI.Library.Action.ActionType.REMOTE_3;
                    break;
                case 4:
                    type = MediaPortal.GUI.Library.Action.ActionType.REMOTE_4;
                    break;
                case 5:
                    type = MediaPortal.GUI.Library.Action.ActionType.REMOTE_5;
                    break;
                case 6:
                    type = MediaPortal.GUI.Library.Action.ActionType.REMOTE_6;
                    break;
                case 7:
                    type = MediaPortal.GUI.Library.Action.ActionType.REMOTE_7;
                    break;
                case 8:
                    type = MediaPortal.GUI.Library.Action.ActionType.REMOTE_8;
                    break;
                case 9:
                    type = MediaPortal.GUI.Library.Action.ActionType.REMOTE_9;
                    break;
            }

            MediaPortal.GUI.Library.Action pinAction = new MediaPortal.GUI.Library.Action(type, 0, 0);
            mpDialog.OnAction(pinAction);
        }
    }
}
