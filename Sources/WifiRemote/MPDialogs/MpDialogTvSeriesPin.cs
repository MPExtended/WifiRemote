using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WifiRemote.MPDialogs
{
    public class MpDialogTvSeriesPin : MpDialog
    {
        WindowPlugins.GUITVSeries.GUIPinCode mpDialog;
        public MpDialogTvSeriesPin(WindowPlugins.GUITVSeries.GUIPinCode dialog)
            : base(dialog)
        {
            this.mpDialog = dialog;
            this.DialogType = dialog.GetModuleName();
            this.DialogId = dialog.GetID;
            GetHeading(dialog, 1);
            GetText(dialog, 2, 3, 4, 5);

            this.AvailableActions.Add("cancel");
            this.AvailableActions.Add("settext");
            this.AvailableActions.Add("confirmtext");
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
        }
    }
}
