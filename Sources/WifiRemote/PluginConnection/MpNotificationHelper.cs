using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WifiRemote.PluginConnection
{
    internal class MpNotificationHelper
    {
        internal static void SendNotification(String _text)
        {
            SendNotification(_text, 2);
        }

        internal static void SendNotification(String _text, int _timeout)
        {
            MPNotificationBar.Notification notification = new MPNotificationBar.Notification(111, _text, MPNotificationBar.NotificationBarManager.Types.Information);
            notification.SecondsToLive = _timeout;
            notification.ShowInFullScreen = true;
            MPNotificationBar.NotificationBarManager.AddNotification(notification);
        }
    }
}
