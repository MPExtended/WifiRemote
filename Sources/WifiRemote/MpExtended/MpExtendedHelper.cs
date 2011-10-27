using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WifiRemote.PluginConnection;
using WifiRemote.MPPlayList;

namespace WifiRemote.MpExtended
{
    /// <summary>
    /// Helper class for MpExtended related functions
    /// </summary>
    public class MpExtendedHelper
    {
        /// <summary>
        /// Play a media item described by its' MpExtended properties (item id/item type/provider id)
        /// </summary>
        /// <param name="itemId">MpExtended item id</param>
        /// <param name="mediaType">MpExtended media type</param>
        /// <param name="providerId">MpExtended provider id</param>
        /// <param name="startPos">Start position in the video or playlist</param>
        public static void PlayMediaItem(string itemId, int mediaType, int providerId, int startPos)
        {
            try
            {
                MpExtProviders provider = (MpExtProviders)providerId;
                MpExtMediaTypes type = (MpExtMediaTypes)mediaType;
                switch (provider)
                {
                    case MpExtProviders.MovingPictures:
                        MovingPicturesHelper.PlayMovie(Int32.Parse(itemId), false, startPos);
                        break;
                    case MpExtProviders.MPTvSeries:
                        if (type == MpExtMediaTypes.TVEpisode)
                        {
                            TVSeriesHelper.PlayEpisode(Int32.Parse(itemId), false, startPos);
                        }
                        else if (type == MpExtMediaTypes.TVSeason)
                        {
                            String[] ids = itemId.Split(new string[] { "_s" }, StringSplitOptions.None);
                            TVSeriesHelper.PlaySeason(Int32.Parse(ids[0]), Int32.Parse(ids[1]), true, startPos, false, true); 
                        }
                        else if (type == MpExtMediaTypes.TVShow)
                        {
                            TVSeriesHelper.PlaySeries(Int32.Parse(itemId), true, startPos, false, true);
                        }
                        break;
                    case MpExtProviders.MPMusic:
                        if (type == MpExtMediaTypes.MusicTrack)
                        {
                            MpMusicHelper.PlayMusicTrack(Int32.Parse(itemId), startPos);
                        }
                        else if (type == MpExtMediaTypes.MusicAlbum)
                        {
                            String[] ids = itemId.Split(new string[] { "_MPExtended_" }, StringSplitOptions.None);
                            MpMusicHelper.PlayAlbum(ids[0], ids[1], startPos);
                        }
                        break;
                    default:
                        WifiRemote.LogMessage("Provider not implemented yet", WifiRemote.LogType.Warn);
                        break;
                }
            }
            catch (Exception ex)
            {
                WifiRemote.LogMessage("Error during play of MediaItem: " + ex.ToString(), WifiRemote.LogType.Error);
            }
        }
    }
}
