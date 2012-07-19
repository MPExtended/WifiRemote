using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MediaPortal.GUI.Library;
using WifiRemote.Messages;
using Deusty.Net;
using MediaPortal.GUI.Home;

namespace WifiRemote.MPFacade
{
    public class MpFacadeHelper
    {


        /// <summary>
        /// Get all facade items of the currently shown facade
        /// </summary>
        /// <param name="windowId"></param>
        /// <param name="controlId"></param>
        /// <returns></returns>
        public static List<FacadeItem> GetFacadeItems(int windowId, int controlId)
        {
            List<FacadeItem> returnItems = new List<FacadeItem>();
            int count = GUIFacadeControl.GetItemCount(GUIWindowManager.ActiveWindow, 50);
            for (int i = 0; i < count; i++)
            {
                GUIListItem item = GUIFacadeControl.GetListItem(GUIWindowManager.ActiveWindow, 50, i);
                returnItems.Add(new FacadeItem(item));
            }

            return returnItems;
        }

        /// <summary>
        /// Get all facade items of the home screen
        /// </summary>
        /// <param name="home"></param>
        /// <returns></returns>
        internal static List<FacadeItem> GetHomeItems(GUIMenuControl home)
        {
            List<FacadeItem> returnItems = new List<FacadeItem>();
            foreach(MenuButtonInfo i in home.ButtonInfos)
            {
                returnItems.Add(new FacadeItem(i));
            }
            return returnItems;
        }
    }
}
