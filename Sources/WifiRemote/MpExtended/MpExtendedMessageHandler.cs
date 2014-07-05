using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using MediaPortal.Playlists;
using WifiRemote.MPPlayList;

namespace WifiRemote.MpExtended
{
    /// <summary>
    /// Handler for MpExtended messages
    /// </summary>
    class MpExtendedMessageHandler
    {
        /// <summary>
        /// Handle an MpExtended message received from a client
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="socketServer">Socket server</param>
        /// <param name="sender">Sender</param>
        internal static void HandleMpExtendedMessage(Newtonsoft.Json.Linq.JObject message, SocketServer socketServer, Deusty.Net.AsyncSocket sender)
        {
            string itemId = (string)message["ItemId"];
            int itemType = (int)message["MediaType"];
            int providerId = (int)message["ProviderId"];
            String action = (String)message["Action"];
            Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(message["PlayInfo"].ToString());
            int startPos = (message["StartPosition"] != null) ? (int)message["StartPosition"] : 0;

            if (action != null)
            {
                if (action.Equals("play"))
                {
                    //play the item in MediaPortal
                    WifiRemote.LogMessage("play mediaitem: ItemId: " + itemId + ", itemType: " + itemType + ", providerId: " + providerId, WifiRemote.LogType.Debug);
                    MpExtendedHelper.PlayMediaItem(itemId, itemType, providerId, values, startPos);
                }
                else if (action.Equals("show"))
                {
                    //show the item details in mediaportal (without starting playback)
                    WifiRemote.LogMessage("show mediaitem: ItemId: " + itemId + ", itemType: " + itemType + ", providerId: " + providerId, WifiRemote.LogType.Debug);
             
                    MpExtendedHelper.ShowMediaItem(itemId, itemType, providerId, values);
                }
                else if (action.Equals("enqueue"))
                {
                    //enqueue the mpextended item to the currently active playlist
                    WifiRemote.LogMessage("enqueue mediaitem: ItemId: " + itemId + ", itemType: " + itemType + ", providerId: " + providerId, WifiRemote.LogType.Debug);
 
                    int startIndex = (message["StartIndex"] != null) ? (int)message["StartIndex"] : -1;

                    PlayListType playlistType = PlayListType.PLAYLIST_VIDEO;
                    List<PlayListItem> items = MpExtendedHelper.CreatePlayListItemItem(itemId, itemType, providerId, values, out playlistType);

                    PlaylistHelper.AddPlaylistItems(playlistType, items, startIndex);
                }
            }
            else
            {
                WifiRemote.LogMessage("No MpExtended action defined", WifiRemote.LogType.Warn);
            }
        }
    }
}
