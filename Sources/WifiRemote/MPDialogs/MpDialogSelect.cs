using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WifiRemote.MPDialogs
{
    public class MpDialogSelect: MpDialog
    {
        public MpDialogSelect(MediaPortal.Dialogs.GUIDialogSelect menu)
            : base(menu)
        {
            this.DialogType = menu.GetModuleName();
            this.DialogId = menu.GetID;
            this.AvailableActions.Add("ok");
            this.AvailableActions.Add("cancel");
        }
    }
}
