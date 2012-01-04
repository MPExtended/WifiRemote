using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MediaPortal.Dialogs;
using MediaPortal.GUI.Library;

namespace WifiRemote.MPDialogs
{
    public class MpDialogProgress : MpDialog
    {
        private GUIDialogProgress dialogMenu;
        public MpDialogProgress(MediaPortal.Dialogs.GUIDialogProgress menu)
            : base(menu)
        {
            this.dialogMenu = menu;
            this.DialogType = menu.GetModuleName();
            this.DialogId = menu.GetID;
            this.AvailableActions.Add("cancel");
            this.AvailableActions.Add("progress");

            UpdateValues();
        }

        /// <summary>
        /// Handle actions which are available on this dialog
        /// </summary>
        /// <param name="action">Action to execute</param>
        /// <param name="index">Index (e.g. needed for lists)</param>
        public override void HandleAction(String action, int index)
        {
            base.HandleAction(action, index);
        }

        /// <summary>
        /// Update the values
        /// </summary>
        public bool UpdateValues()
        {
            bool updated = false;
            int value = dialogMenu.Percentage;
            if (this.DialogValue != value)
            {
                this.DialogValue = value;
                updated = true;
            }

            String header = GetLabel(dialogMenu, 1);
            if (!header.Equals(this.Heading))
            {
                GetHeading(dialogMenu, 1);
                updated = true;
            }

            String text = GetLabel(dialogMenu, 2, 3, 4, 5);
            if (!text.Equals(this.DialogText))
            {
                this.GetText(dialogMenu, 2, 3, 4, 5);
                updated = true;
            }

            return updated;
        }
    }
}
