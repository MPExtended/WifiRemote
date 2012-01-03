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

        /// <summary>
        /// Retrieve items for this list and store it in the object
        /// </summary>
        public void RetrieveListItems()
        {
            for (int i = 0; i < 10; i++)
            {
                Thread.Sleep(200);

                GUIListControl list = GetListControl(menu.controlList);

                if (list != null)
                {
                    //found the list control -> get list elements
                    GetItemsFromListControl(list);
                    break;
                }
            }
        }

        /// <summary>
        /// Try to locate the list element in a collection of gui controls
        /// </summary>
        /// <param name="collection">The collection where we browse in</param>
        /// <returns>The list if found, null otherwise</returns>
        private GUIListControl GetListControl(GUIControlCollection collection)
        {
            if (collection != null && collection.Count > 0)
            {
                //Get the control that holds all the list items
                foreach (GUIControl c in collection)
                {
                    if (c.GetType() == typeof(GUIListControl))
                    {
                        GUIListControl list = (GUIListControl)c;
                        return list;
                    }
                    else if (c.GetType() == typeof(GUIGroup))
                    {
                        GUIGroup group = (GUIGroup)c;
                        //Control type group, can have child elements
                        GUIListControl list = GetListControl(group.Children);
                        if (list != null)
                        {
                            //Found list in group type group, return it
                            return list;
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Get items from the list control
        /// </summary>
        /// <param name="list">list control</param>
        private void GetItemsFromListControl(GUIListControl list)
        {
            //make sure the list isn't in the progress of being filled (list count growing)
            int itemCount = 0;
            for (int i = 0; i < 20; i++)
            {
                Thread.Sleep(200);

                if (list.Count > 0 && list.Count == itemCount)
                {
                    break;
                }
                itemCount = list.Count;
            }

            WifiRemote.LogMessage("Retrieving " + itemCount + " dialog list items: " , WifiRemote.LogType.Debug);

            List<GUIListItem> items = list.ListItems;
            for (int i = 0; i < itemCount; i++)
            {
                FacadeItem item = new FacadeItem(list[i]);
                //WifiRemote.LogMessage("Add dialog list items: " + item.Label, WifiRemote.LogType.Debug);
                ListItems.Add(item);
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
