using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MediaPortal.GUI.Library;

namespace WifiRemote.PluginConnection
{
    /// <summary>
    /// Helper class for MP window actions
    /// </summary>
    public class WindowPluginHelper
    {
        protected delegate void ActivateWindowDelegate(int windowId, string param);
        protected delegate void OpenWindowDelegate(int windowId);

        /// <summary>
        /// Open a window (for example Moving Pictures, MP-TV Series, etc.), 
        /// invoke on GUI thread if called from a different
        /// thread than UI thread
        /// </summary>
        /// <param name="windowId"></param>
        public static void OpenWindow(int windowId)
        {
            if (GUIGraphicsContext.form.InvokeRequired)
            {
                OpenWindowDelegate d = OpenWindow;
                GUIGraphicsContext.form.Invoke(d, new object[] { windowId });
            }
            else
            {
                GUIGraphicsContext.ResetLastActivity();
                GUIMessage msg = new GUIMessage(GUIMessage.MessageType.GUI_MSG_GOTO_WINDOW, 0, 0, 0, windowId, 0, null);

                GUIWindowManager.SendThreadMessage(msg);
            }
        }


        /// <summary>
        /// Activate a window by id without resetting the last activity
        /// and supply a loadParameter, 
        /// invoke on GUI thread if called from a different
        /// thread than UI thread
        /// </summary>
        /// <param name="windowId"></param>
        /// <param name="param"></param>
        public static void ActivateWindow(int windowId, string param)
        {
            if (GUIGraphicsContext.form.InvokeRequired)
            {
                ActivateWindowDelegate d = ActivateWindow;
                GUIGraphicsContext.form.Invoke(d, new object[] { windowId, param });
            }
            else
            {
                if (param == null)
                {
                    GUIWindowManager.ActivateWindow(windowId);
                }
                else
                {
                    GUIWindowManager.ActivateWindow(windowId, param);
                }
            }
        }


        /// <summary>
        /// Activate a window by id without resetting the last activity, 
        /// invoke on GUI thread if called from a different
        /// thread than UI thread
        /// </summary>
        /// <param name="_windowId">id of window</param>
        internal static void ActivateWindow(int _windowId)
        {
            ActivateWindow(_windowId, null);
        }
    }
}
