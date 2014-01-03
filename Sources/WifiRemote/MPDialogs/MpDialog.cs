using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MediaPortal.Dialogs;
using MediaPortal.GUI.Library;

namespace WifiRemote.MPDialogs
{
    public class MpDialog
    {
        GUIDialogWindow dialogWindow;
        public MpDialog(GUIDialogWindow _dialog)
            : this()
        {
            dialogWindow = _dialog;
        }

        public MpDialog()
        {
            AvailableActions = new List<String>();
        }

        /// <summary>
        /// Gets the dialog heading from the given mp dialog and saves it into this instance
        /// </summary>
        /// <param name="dialog">MP Dialog</param>
        /// <param name="controlId">Control id of heading label</param>
        public void GetHeading(GUIDialogWindow dialog, params int[] controlIds)
        {
            this.Heading = GetLabel(dialog, controlIds);
        }

        /// <summary>
        /// Gets the dialog text from the given mp dialog and saves it into this instance
        /// </summary>
        /// <param name="dialog">MP Dialog</param>
        /// <param name="controlId">Control id of text label</param>
        public void GetText(GUIDialogWindow dialog, params int[] controlIds)
        {
            this.DialogText = GetLabel(dialog, controlIds);
        }

        /// <summary>
        /// Gets the text from the given control id on the dialog and returns it
        /// </summary>
        /// <param name="dialog">MP Dialog</param>
        /// <param name="controlId">ID of the label</param>
        /// <returns></returns>
        public String GetLabel(GUIDialogWindow dialog, params int[] controlIds)
        {
            try
            {
                int index = 0;
                StringBuilder text = new StringBuilder();
                foreach (int control in controlIds)
                {
                    String t = GetSingleLabel(dialog, control);
                    if (t != null && !t.Equals(""))
                    {
                        if (index > 0)
                        {
                            text.AppendLine();
                        }
                        index++;

                        text.Append(t);
                    }
                }

                return text.ToString();
            }
            catch (Exception ex)
            {
                WifiRemote.LogMessage("Error filling dialog: " + ex, WifiRemote.LogType.Error);
            }
            return null;
        }

        private string GetSingleLabel(GUIDialogWindow dialog, int control)
        {
            GUIControlCollection coll = dialog.controlList;
            foreach (GUIControl c in coll)
            {
                if (c.GetType() == typeof(GUIGroup))
                {
                    foreach (GUIControl subControl in ((GUIGroup)c).Children)
                    {
                        if (subControl.GetID == control)
                        {
                            return GetSingleLabelFromControl(subControl);
                        }
                    }
                }
                else if (c.GetID == control)
                {
                    return GetSingleLabelFromControl(c);
                }
            }

            return null;
        }

        private string GetSingleLabelFromControl(GUIControl control)
        {
            if (control.GetType() == typeof(GUILabelControl))
            {
                GUILabelControl label = (GUILabelControl)control;
                return label.Label;
            }
            else if (control.GetType() == typeof(GUIFadeLabel))
            {
                GUIFadeLabel label = (GUIFadeLabel)control;
                return label.Label;
            }

            return null;
        }

        /// <summary>
        /// Dialog Heading
        /// </summary>
        public String Heading { get; set; }

        /// <summary>
        /// Text of dialog
        /// </summary>
        public String DialogText { get; set; }

        /// <summary>
        /// Dialog Module name in MediaPortal (language dependant)
        /// </summary>
        public String DialogType { get; set; }

        /// <summary>
        /// Id of Dialog
        /// </summary>
        public int DialogId { get; set; }

        /// <summary>
        /// Value of Dialog
        /// </summary>
        public int DialogValue { get; set; }

        /// <summary>
        /// Actions available for this dialog
        /// </summary>
        public List<String> AvailableActions { get; set; }

        /// <summary>
        /// Handle actions which are available on all dialogs (e.g. cancel). 
        /// 
        /// Should be overwritten in all subclasses to handle the dialog-specific
        /// actions (use base.HandleAction(action, index) to also cover the base
        /// functions)
        /// </summary>
        /// <param name="action">Action to execute</param>
        /// <param name="index">Index (e.g. needed for lists)</param>
        public virtual void HandleAction(String action, int index)
        {
            if (action.Equals("cancel"))
            {
                MediaPortal.GUI.Library.Action cancelAction = new MediaPortal.GUI.Library.Action(MediaPortal.GUI.Library.Action.ActionType.ACTION_CLOSE_DIALOG, 0, 0);
                dialogWindow.OnAction(cancelAction);
            }
        }
    }
}
