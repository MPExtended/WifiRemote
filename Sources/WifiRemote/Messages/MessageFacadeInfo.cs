using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MediaPortal.GUI.Library;

namespace WifiRemote.Messages
{
    class MessageFacadeInfo : IMessage
    {
        /// <summary>
        /// Creates a new facade info and fills it with the value of the currently active facade
        /// </summary>
        public MessageFacadeInfo()
        {
            UpdateFacadeInfo();
        }

        /// <summary>
        /// Update the facade infor with the current values
        /// </summary>
        private void UpdateFacadeInfo()
        {
            try
            {
                GUIWindow currentPlugin = GUIWindowManager.GetWindow(GUIWindowManager.ActiveWindow);
                Object o = currentPlugin.GetControl(50);
                if (o != null)
                {
                    if (o.GetType() == typeof(GUIFacadeControl))
                    {
                        GUIFacadeControl facade = (GUIFacadeControl)o;
                        SelectedIndex = facade.SelectedListItemIndex;
                        Visible = facade.VisibleFromSkinCondition;
                        Count = facade.Count;
                    }
                    else if (o.GetType() == typeof(GUIMenuControl))
                    {
                        //TODO: also support home menu
                        GUIMenuControl menu = (GUIMenuControl)o;
                        SelectedIndex = menu.FocusedButton;
                        Visible = false;
                        Count = menu.ButtonInfos.Count;
                    }
                    else
                    {
                        SelectedIndex = 0;
                        Visible = false;
                        Count = 0;
                    }
                }
                else
                {
                    SelectedIndex = 0;
                    Visible = false;
                    Count = 0;
                }
            }
            catch (Exception ex)
            {
                WifiRemote.LogMessage("Error on updating facade info: " + ex.ToString(), WifiRemote.LogType.Error);
            }
        }

        /// <summary>
        /// message type
        /// </summary>
        public string Type
        {
            get { return "facadeinfo"; }
        }

        /// <summary>
        /// Selected index of facacde
        /// </summary>
        public int SelectedIndex { get; set; }

        /// <summary>
        /// Item count of facade
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// Is the facade visible
        /// </summary>
        public bool Visible { get; set; }

        /// <summary>
        /// Has the facade info changed
        /// </summary>
        /// <returns>True if facade has changed, false otherwise</returns>
        internal bool HasChanged()
        {
            MessageFacadeInfo newInfo = new MessageFacadeInfo();
            if (
                newInfo.SelectedIndex != SelectedIndex ||
                newInfo.Visible != Visible ||
                newInfo.Count != Count)
            {
                UpdateFacadeInfo();
                return true;
            }
            return false;
        }
    }
}
