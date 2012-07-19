using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MediaPortal.GUI.Library;
using MediaPortal.GUI.Home;
using WifiRemote.Messages;
using Deusty.Net;

namespace WifiRemote.MPFacade
{
    /// <summary>
    /// Handler for MpFacade messages
    /// </summary>
    class MpFacadeMessageHandler
    {
        /// <summary>
        /// Handle the facade message
        /// </summary>
        /// <param name="message">Message sent from client</param>
        /// <param name="server">Instance of the socket server</param>
        /// <param name="client">Socket that sent the message (for return messages)</param>
        internal static void HandleFacadeMessage(Newtonsoft.Json.Linq.JObject message, SocketServer server, AsyncSocket client)
        {
            String action = (string)message["FacadeAction"];
            GUIWindow currentPlugin = GUIWindowManager.GetWindow(GUIWindowManager.ActiveWindow);

            if (action.Equals("get"))
            {
                MessageFacade returnMessage = new MessageFacade();
                if (currentPlugin.GetType() == typeof(GUIHome))
                {
                    GUIMenuControl menu = (GUIMenuControl)currentPlugin.GetControl(50);
                    List<FacadeItem> items = MpFacadeHelper.GetHomeItems(menu);
                    returnMessage.FacadeItems = items;
                    returnMessage.ViewType = "Home";
                }
                else
                {
                    GUIFacadeControl facade = (GUIFacadeControl)currentPlugin.GetControl(50);
                    if (facade != null)
                    {
                        List<FacadeItem> items = MpFacadeHelper.GetFacadeItems(currentPlugin.GetID, 50);
                        returnMessage.ViewType = facade.CurrentLayout.ToString();
                        returnMessage.FacadeItems = items;
                    }
                }

                returnMessage.WindowId = currentPlugin.GetID;
                server.SendMessageToClient(returnMessage, client);
            }
            else if (action.Equals("setselected"))
            {
                if (currentPlugin.GetType() == typeof(GUIHome))
                {

                }
                else
                {
                    GUIFacadeControl facade = (GUIFacadeControl)currentPlugin.GetControl(50);
                    int selected = (int)message["SelectedIndex"];
                    facade.SelectedListItemIndex = selected;
                }
            }
            else if (action.Equals("getselected"))
            {
                if (currentPlugin.GetType() == typeof(GUIHome))
                {
                    //TODO: find a way to retrieve the currently selected home button
                }
                else
                {
                    GUIFacadeControl facade = (GUIFacadeControl)currentPlugin.GetControl(50);
                    int selected = facade.SelectedListItemIndex;
                }
            }
            else if (action.Equals("getcount"))
            {
                if (currentPlugin.GetType() == typeof(GUIHome))
                {
                    GUIMenuControl menu = (GUIMenuControl)currentPlugin.GetControl(50);
                    int count = menu.ButtonInfos.Count;
                }
                else
                {
                    GUIFacadeControl facade = (GUIFacadeControl)currentPlugin.GetControl(50);
                    int count = facade.Count;
                }
            }
            else if (action.Equals("select"))
            {
                int selected = (int)message["SelectedIndex"];
                if (currentPlugin.GetType() == typeof(GUIHome))
                {
                    GUIMenuControl menu = (GUIMenuControl)currentPlugin.GetControl(50);
                    MenuButtonInfo info = menu.ButtonInfos[selected];
                    GUIMessage msg = new GUIMessage(GUIMessage.MessageType.GUI_MSG_GOTO_WINDOW, 0, 0, 0, info.PluginID, 0, null);
                    GUIWindowManager.SendThreadMessage(msg);
                }
                else
                {
                    GUIFacadeControl facade = (GUIFacadeControl)currentPlugin.GetControl(50);
                    //TODO: is there a better way to select a list item

                    facade.SelectedListItemIndex = selected;
                    new Communication().SendCommand("ok");
                }
            }
            else if (action.Equals("context"))
            {
                int selected = (int)message["SelectedIndex"];
                if (currentPlugin.GetType() == typeof(GUIHome))
                {
                    GUIMenuControl menu = (GUIMenuControl)currentPlugin.GetControl(50);
                    MenuButtonInfo info = menu.ButtonInfos[selected];
                    GUIMessage msg = new GUIMessage(GUIMessage.MessageType.GUI_MSG_GOTO_WINDOW, 0, 0, 0, info.PluginID, 0, null);
                    GUIWindowManager.SendThreadMessage(msg);
                }
                else
                {
                    GUIFacadeControl facade = (GUIFacadeControl)currentPlugin.GetControl(50);
                    //TODO: is there a better way to select a list item

                    facade.SelectedListItemIndex = selected;
                    new Communication().SendCommand("info");
                }
            }
        }
    }
}
