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
        /// <param name="playInfo">Additional information to playback the item</param>
        /// <param name="startPos">Start position in the video or playlist</param>
        public static void PlayMediaItem(string itemId, int mediaType, int providerId, Dictionary<string, string> playInfo, int startPos)
        {
            try
            {
                MpExtendedProviders provider = (MpExtendedProviders)providerId;
                MpExtendedMediaTypes type = (MpExtendedMediaTypes)mediaType;
                switch (provider)
                {
                    case MpExtendedProviders.MovingPictures:
                        if (WifiRemote.IsAvailableMovingPictures)
                        {
                            MovingPicturesHelper.PlayMovie(Int32.Parse(playInfo["Id"]), false, startPos);
                        }
                        break;
                    case MpExtendedProviders.MPTvSeries:
                        if (WifiRemote.IsAvailableTVSeries)
                        {
                            if (type == MpExtendedMediaTypes.TVEpisode)
                            {
                                TVSeriesHelper.PlayEpisode(Int32.Parse(playInfo["Id"]), false, startPos);
                            }
                            else if (type == MpExtendedMediaTypes.TVSeason)
                            {
                                TVSeriesHelper.PlaySeason(Int32.Parse(playInfo["ShowId"]), Int32.Parse(playInfo["SeasonIndex"]), true, startPos, false, true);
                            }
                            else if (type == MpExtendedMediaTypes.TVShow)
                            {
                                TVSeriesHelper.PlaySeries(Int32.Parse(playInfo["Id"]), true, startPos, false, true);
                            }
                        }
                        break;
                    case MpExtendedProviders.MPMusic:
                        if (type == MpExtendedMediaTypes.MusicTrack)
                        {
                            MpMusicHelper.PlayMusicTrack(Int32.Parse(playInfo["Id"]), startPos);
                        }
                        else if (type == MpExtendedMediaTypes.MusicAlbum)
                        {
                            MpMusicHelper.PlayAlbum(playInfo["Artist"], playInfo["Album"], startPos);
                        }
                        else if (type == MpExtendedMediaTypes.MusicArtist)
                        {
                            MpMusicHelper.PlayArtist(playInfo["Artist"], startPos);
                        }
                        break;
                    case MpExtendedProviders.MPVideo:
                        MpVideosHelper.PlayVideo(Int32.Parse(playInfo["Id"]), startPos);
                        break;
                    case MpExtendedProviders.MpVideosShare:
                        if (type == MpExtendedMediaTypes.File)
                        {
                            //TODO: fill myvideos db information instead of just playing the file
                            
                            MpVideosHelper.PlayVideoFromFile(playInfo["Path"], startPos);
                        }
                        else if (type == MpExtendedMediaTypes.Folder)
                        {
                            string[] extensions = playInfo["Extensions"].Split('|');
                            MpVideosHelper.PlayFolder(playInfo["Path"], extensions, startPos);
                        }
                        break;
                    case MpExtendedProviders.MpMusicShare:
                        if (type == MpExtendedMediaTypes.File)
                        {
                            MpMusicHelper.PlayFile(playInfo["Path"], startPos);
                        }
                        else if (type == MpExtendedMediaTypes.Folder)
                        {
                            string[] extensions = playInfo["Extensions"].Split('|');
                            MpMusicHelper.PlayAllFilesInFolder(playInfo["Path"], extensions, startPos);
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
