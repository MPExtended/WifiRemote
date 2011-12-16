using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MediaPortal.GUI.Library;
using MediaPortal.Dialogs;

namespace WifiRemote.MPDialogs
{
    public class MpDialogNotify: MpDialog
    {
        private GUIDialogNotify dialogMenu;
        public MpDialogNotify(MediaPortal.Dialogs.GUIDialogNotify menu)
            : base(menu)
        {
            this.dialogMenu = menu;
            this.DialogType = menu.GetModuleName();
            this.DialogId = menu.GetID;
            this.AvailableActions.Add("ok");
            this.AvailableActions.Add("cancel");

            GetHeading(menu, 4);
        }

        /// <summary>
        /// Handle actions which are available on this dialog
        /// </summary>
        /// <param name="action">Action to execute</param>
        /// <param name="index">Index (e.g. needed for lists)</param>
        public override void HandleAction(String action, int index)
        {
            base.HandleAction(action, index);
            if (action.Equals("ok"))
            {
                GUIMessage msg = new GUIMessage(GUIMessage.MessageType.GUI_MSG_CLICKED, 0, 10, 10, 0, 0, null);
                dialogMenu.OnMessage(msg);
            }
        }
    }
}
