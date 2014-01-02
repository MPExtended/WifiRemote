using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WifiRemote.Messages;
using Newtonsoft.Json.Linq;

namespace WifiRemote.MPPlayList
{
    /// <summary>
    /// Handler for playlist messages
    /// </summary>
    internal class PlaylistMessageHandler
    {
        /// <summary>
        /// Handle an Playlist message received from a client
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="socketServer">Socket server</param>
        /// <param name="sender">Sender</param>
        internal static void HandlePlaylistMessage(Newtonsoft.Json.Linq.JObject message, SocketServer socketServer, Deusty.Net.AsyncSocket sender)
        {
            String action = (string)message["PlaylistAction"];
            String playlistType = (message["PlaylistType"] != null) ? (string)message["PlaylistType"] : "music";
            bool shuffle = (message["Shuffle"] != null) ? (bool)message["Shuffle"] : false;
            bool autoPlay = (message["AutoPlay"] != null) ? (bool)message["AutoPlay"] : false;
            bool showPlaylist = (message["ShowPlaylist"] != null) ? (bool)message["ShowPlaylist"] : true;

            if (action.Equals("new") || action.Equals("append"))
            {
                //new playlist or append to playlist
                int insertIndex = 0;
                if (message["InsertIndex"] != null)
                {
                    insertIndex = (int)message["InsertIndex"];
                }

                // Add items from JSON or SQL
                JArray array = (message["PlaylistItems"] != null) ? (JArray)message["PlaylistItems"] : null;
                JObject sql = (message["PlayListSQL"] != null) ? (JObject)message["PlayListSQL"] : null;
                if (array != null || sql != null)
                {
                    if (action.Equals("new"))
                    {
                        PlaylistHelper.ClearPlaylist(playlistType, false);
                    }

                    int index = insertIndex;

                    if (array != null)
                    {
                        // Add items from JSON
                        foreach (JObject o in array)
                        {
                            PlaylistEntry entry = new PlaylistEntry();
                            entry.FileName = (o["FileName"] != null) ? (string)o["FileName"] : null;
                            entry.Name = (o["Name"] != null) ? (string)o["Name"] : null;
                            entry.Duration = (o["Duration"] != null) ? (int)o["Duration"] : 0;
                            PlaylistHelper.AddItemToPlaylist(playlistType, entry, index, false);
                            index++;
                        }
                        PlaylistHelper.RefreshPlaylistIfVisible();

                        if (shuffle)
                        {
                            PlaylistHelper.Shuffle(playlistType);
                        }
                    }
                    else
                    {
                        // Add items with SQL
                        string where = (sql["Where"] != null) ? (string)sql["Where"] : String.Empty;
                        int limit = (sql["Limit"] != null) ? (int)sql["Limit"] : 0;

                        PlaylistHelper.AddSongsToPlaylistWithSQL(playlistType, where, limit, shuffle, insertIndex);
                    }

                    if (autoPlay)
                    {
                        if (message["StartPosition"] != null)
                        {
                            int startPos = (int)message["StartPosition"];
                            insertIndex += startPos;
                        }
                        PlaylistHelper.StartPlayingPlaylist(playlistType, insertIndex, showPlaylist);
                    }
                }
            }
            else if (action.Equals("load"))
            {
                //load a playlist
                string playlistName = (string)message["PlayListName"];
                string playlistPath = (string)message["PlaylistPath"];

                if (!string.IsNullOrEmpty(playlistName) || !string.IsNullOrEmpty(playlistPath))
                {
                    PlaylistHelper.LoadPlaylist(playlistType, (!string.IsNullOrEmpty(playlistName)) ? playlistName : playlistPath, shuffle);
                    if (autoPlay)
                    {
                        PlaylistHelper.StartPlayingPlaylist(playlistType, 0, showPlaylist);
                    }
                }
            }
            else if (action.Equals("get"))
            {
                //get all playlist items of the currently active playlist
                List<PlaylistEntry> items = PlaylistHelper.GetPlaylistItems(playlistType);

                MessagePlaylistDetails returnPlaylist = new MessagePlaylistDetails();
                returnPlaylist.PlaylistType = playlistType;
                returnPlaylist.PlaylistName = PlaylistHelper.GetPlaylistName(playlistType);
                returnPlaylist.PlaylistRepeat = PlaylistHelper.GetPlaylistRepeat(playlistType);
                returnPlaylist.PlaylistItems = items;

                socketServer.SendMessageToClient(returnPlaylist, sender);
            }
            else if (action.Equals("remove"))
            {
                //remove an item from the playlist
                int indexToRemove = (message["Index"] != null) ? (int)message["Index"] : 0;

                PlaylistHelper.RemoveItemFromPlaylist(playlistType, indexToRemove);
            }
            else if (action.Equals("move"))
            {
                //move a playlist item to a new index
                int oldIndex = (message["OldIndex"] != null) ? (int)message["OldIndex"] : 0;
                int newIndex = (message["NewIndex"] != null) ? (int)message["NewIndex"] : 0;
                PlaylistHelper.ChangePlaylistItemPosition(playlistType, oldIndex, newIndex);
            }
            else if (action.Equals("play"))
            {
                //start playback of a playlist item
                int index = (message["Index"] != null) ? (int)message["Index"] : 0;
                PlaylistHelper.StartPlayingPlaylist(playlistType, index, showPlaylist);
            }
            else if (action.Equals("clear"))
            {
                //clear the playlist
                PlaylistHelper.ClearPlaylist(playlistType, true);
            }
            else if (action.Equals("list"))
            {
                //get a list of all available playlists
                MessagePlaylists returnList = new MessagePlaylists();
                returnList.PlayLists = PlaylistHelper.GetPlaylists();
                socketServer.SendMessageToClient(returnList, sender);
            }
            else if (action.Equals("save"))
            {
                //save the current playlist to file
                String name = (message["Name"] != null) ? (String)message["Name"] : null;
                if (name != null)
                {
                    PlaylistHelper.SaveCurrentPlaylist(name);
                }
                else
                {
                    WifiRemote.LogMessage("Must specify a name to save a playlist", WifiRemote.LogType.Warn);
                }

            }
            else if (action.Equals("shuffle"))
            {
                PlaylistHelper.Shuffle(playlistType);
            }
            else if (action.Equals("repeat"))
            {
                WifiRemote.LogMessage("Playlist action repeat", WifiRemote.LogType.Debug);
                if (message["Repeat"] != null)
                {
                    bool repeat =  (bool)message["Repeat"];
                    PlaylistHelper.Repeat(playlistType, repeat);
                }
                else
                {
                    WifiRemote.LogMessage("Must specify repeat to change playlist repeat mode", WifiRemote.LogType.Warn);
                }
                
            }
        }
    }
}
