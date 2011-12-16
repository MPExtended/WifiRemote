using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MediaPortal.GUI.Library;
using System.Threading;
using MediaPortal.Dialogs;
using WifiRemote.MPFacade;

namespace WifiRemote.MPDialogs
{
    public class MpDialogMenu : MpDialog
    {
        private GUIDialogMenu menu;
        public MpDialogMenu(MediaPortal.Dialogs.GUIDialogMenu menu)
            : base(menu)
        {
            this.DialogType = menu.GetModuleName();
            this.DialogId = menu.GetID;
            this.menu = menu;
            this.ListItems = new List<FacadeItem>();

            this.AvailableActions.Add("listselect");
            this.AvailableActions.Add("cancel");

            GetHeading(menu, 4);
        }

        public void RetrieveListItems()
        {
            GUIControlCollection coll = menu.controlList;
            foreach (GUIControl c in coll)
            {
                if (c.GetType() == typeof(GUIListControl))
                {
                    GUIListControl list = (GUIListControl)c;
                    List<GUIListItem> items = list.ListItems;
                    for (int i = 0; i < list.Count; i++)
                    {
                        FacadeItem item = new FacadeItem(list[i]);
                        ListItems.Add(item);
                    }
                }
            }
        }

        /// <summary>
        /// Handle actions which are available on this dialog
        /// </summary>
        /// <param name="action">Action to execute</param>
        /// <param name="index">Index (e.g. needed for lists)</param>
        public override void HandleAction(String action, int index)
        {
            base.HandleAction(action, index);
            if (action.Equals("listselect"))
            {
                SelectItem(index);
            }
        }

        /// <summary>
        /// Select one of the list items
        /// </summary>
        /// <param name="index">Index to select</param>
        public void SelectItem(int index)
        {
            index++;
            menu.selectOption(index.ToString());
        }

        /// <summary>
        /// List items
        /// </summary>
        public List<FacadeItem> ListItems { get; set; }
    }
}
