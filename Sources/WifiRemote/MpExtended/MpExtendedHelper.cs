using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WifiRemote.PluginConnection;
using WifiRemote.MPPlayList;
using MediaPortal.Playlists;

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
                        else
                        {
                            WifiRemote.LogMessage("MovingPictures not installed but required!", WifiRemote.LogType.Error);
                        }
                        break;
                    case MpExtendedProviders.MPTvSeries:
                        if (WifiRemote.IsAvailableTVSeries)
                        {
                            if (type == MpExtendedMediaTypes.TVEpisode)
                            {
                                TVSeriesHelper.PlayEpisode(playInfo["Id"], false, startPos);
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
                        else
                        {
                            WifiRemote.LogMessage("MP-TvSeries not installed but required!", WifiRemote.LogType.Error);
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
                        //we have no providers (yet) for tv
                        if (type == MpExtendedMediaTypes.Recording)
                        {
                            if (!WifiRemote.IsAvailableTVPlugin)
                            {
                                WifiRemote.LogMessage("No TVPlugin installed: Aborting playrecording", WifiRemote.LogType.Error);
                                return;
                            }

                            MpTvServerHelper.PlayRecording(Int32.Parse(playInfo["Id"]), startPos, true);
                        }
                        else if (type == MpExtendedMediaTypes.Tv)
                        {
                            if (!WifiRemote.IsAvailableTVPlugin)
                            {
                                WifiRemote.LogMessage("No TVPlugin installed: Aborting playchannel", WifiRemote.LogType.Error);
                                return;
                            }

                            MpTvServerHelper.PlayTvChannel(Int32.Parse(playInfo["Id"]), true);
                        }
                        else
                        {
                            WifiRemote.LogMessage("Provider not implemented yet", WifiRemote.LogType.Warn);
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                WifiRemote.LogMessage("Error during creation of PlayListItem: " + ex.ToString(), WifiRemote.LogType.Error);
            }
        }

        /// <summary>
        /// Play a media item described by its' MpExtended properties (item id/item type/provider id)
        /// </summary>
        /// <param name="itemId">MpExtended item id</param>
        /// <param name="mediaType">MpExtended media type</param>
        /// <param name="providerId">MpExtended provider id</param>
        /// <param name="playInfo">Additional information to playback the item</param>
        public static List<PlayListItem> CreatePlayListItemItem(string itemId, int mediaType, int providerId, Dictionary<string, string> playInfo, out PlayListType playlistType)
        {
            playlistType = PlayListType.PLAYLIST_VIDEO;//default to video
            try
            {
                List<PlayListItem> items = new List<PlayListItem>();
                MpExtendedProviders provider = (MpExtendedProviders)providerId;
                MpExtendedMediaTypes type = (MpExtendedMediaTypes)mediaType;
                switch (provider)
                {
                    case MpExtendedProviders.MovingPictures:
                        if (WifiRemote.IsAvailableMovingPictures)
                        {
                            playlistType = PlayListType.PLAYLIST_VIDEO;
                            items.Add(MovingPicturesHelper.CreatePlaylistItem(Int32.Parse(playInfo["Id"])));
                        }
                        else
                        {
                            WifiRemote.LogMessage("MovingPictures not installed but required!", WifiRemote.LogType.Error);
                        }
                        break;
                    case MpExtendedProviders.MPTvSeries:
                        if (WifiRemote.IsAvailableTVSeries)
                        {
                            playlistType = PlayListType.PLAYLIST_VIDEO;
                            if (type == MpExtendedMediaTypes.TVEpisode)
                            {
                                items.Add(TVSeriesHelper.CreatePlaylistItemFromEpisode(playInfo["Id"]));
                            }
                            else if (type == MpExtendedMediaTypes.TVSeason)
                            {
                                items = TVSeriesHelper.CreatePlaylistItemsFromSeason(Int32.Parse(playInfo["ShowId"]), Int32.Parse(playInfo["SeasonIndex"]));
                            }
                            else if (type == MpExtendedMediaTypes.TVShow)
                            {
                                items = TVSeriesHelper.CreatePlaylistItemsFromShow(Int32.Parse(playInfo["Id"]));
                            }
                        }
                        else
                        {
                            WifiRemote.LogMessage("MP-TvSeries not installed but required!", WifiRemote.LogType.Error);
                        }
                        break;
                    case MpExtendedProviders.MPMusic:
                        playlistType = PlayListType.PLAYLIST_MUSIC;
                        if (type == MpExtendedMediaTypes.MusicTrack)
                        {
                            items.Add(MpMusicHelper.CreatePlaylistItemFromMusicTrack(Int32.Parse(playInfo["Id"])));
                        }
                        else if (type == MpExtendedMediaTypes.MusicAlbum)
                        {
                             items = MpMusicHelper.CreatePlaylistItemsFromMusicAlbum(playInfo["Artist"], playInfo["Album"]);
                        }
                        else if (type == MpExtendedMediaTypes.MusicArtist)
                        {
                            items = MpMusicHelper.CreatePlaylistItemsFromMusicArtist(playInfo["Artist"]);
                 
                            // MpMusicHelper.PlayArtist(playInfo["Artist"], startPos);
                        }
                        break;
                    case MpExtendedProviders.MPVideo:
                        playlistType = PlayListType.PLAYLIST_VIDEO;
                        //MpVideosHelper.PlayVideo(Int32.Parse(playInfo["Id"]), startPos);
                        break;
                    case MpExtendedProviders.MpVideosShare:
                        playlistType = PlayListType.PLAYLIST_VIDEO;
                        if (type == MpExtendedMediaTypes.File)
                        {

                            items.Add(MpVideosHelper.CreatePlaylistItemFromVideoFile(playInfo["Path"]));
                        }
                        else if (type == MpExtendedMediaTypes.Folder)
                        {
                            string[] extensions = playInfo["Extensions"].Split('|');
                            items = MpVideosHelper.CreatePlaylistItemFromVideoFolder(playInfo["Path"], extensions);
                        }
                        break;
                    case MpExtendedProviders.MpMusicShare:
                        playlistType = PlayListType.PLAYLIST_MUSIC;
                        if (type == MpExtendedMediaTypes.File)
                        {
                            items.Add(MpMusicHelper.CreatePlaylistItemFromMusicFile(playInfo["Path"]));
                        }
                        else if (type == MpExtendedMediaTypes.Folder)
                        {
                            string[] extensions = playInfo["Extensions"].Split('|');
                            items = MpMusicHelper.CreatePlaylistItemFromMusicFolder(playInfo["Path"], extensions);
                        }
                        break;
                    default:
                        playlistType = PlayListType.PLAYLIST_VIDEO;
                        //we have no providers (yet) for tv
                        if (type == MpExtendedMediaTypes.Recording)
                        {
                            if (!WifiRemote.IsAvailableTVPlugin)
                            {
                                WifiRemote.LogMessage("No TVPlugin installed: Aborting playrecording", WifiRemote.LogType.Error);
                                return null;
                            }

                            items.Add(MpTvServerHelper.CreatePlaylistItemFromRecording(Int32.Parse(playInfo["Id"])));
                        }
                        else
                        {
                            WifiRemote.LogMessage("Provider not implemented yet", WifiRemote.LogType.Warn);
                        }
                        break;
                }
                return items;
            }
            catch (Exception ex)
            {
                WifiRemote.LogMessage("Error during play of MediaItem: " + ex.ToString(), WifiRemote.LogType.Error);
            }
            return null;
        }

        /// <summary>
        /// Show the details of a mediaitem on MediaPortal (without actually starting playback)
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="mediaType"></param>
        /// <param name="providerId"></param>
        /// <param name="playInfo"></param>
        internal static void ShowMediaItem(string itemId, int mediaType, int providerId, Dictionary<string, string> playInfo)
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
                            MovingPicturesHelper.ShowMovieDetails(Int32.Parse(playInfo["Id"]));
                        }
                        else
                        {
                            WifiRemote.LogMessage("MovingPictures not installed but required!", WifiRemote.LogType.Error);
                        }
                        break;
                    case MpExtendedProviders.MPTvSeries:
                        if (WifiRemote.IsAvailableTVSeries)
                        {
                            if (type == MpExtendedMediaTypes.TVEpisode)
                            {
                                TVSeriesHelper.ShowEpisodeDetails(Int32.Parse(playInfo["ShowId"]), Int32.Parse(playInfo["SeasonIndex"]), playInfo["Id"]);
                            }
                            else if (type == MpExtendedMediaTypes.TVSeason)
                            {
                                TVSeriesHelper.ShowSeasonDetails(Int32.Parse(playInfo["ShowId"]), Int32.Parse(playInfo["SeasonIndex"]));
                            }
                            else if (type == MpExtendedMediaTypes.TVShow)
                            {
                                TVSeriesHelper.ShowSeriesDetails(Int32.Parse(playInfo["Id"]));
                            }
                        }
                        else
                        {
                            WifiRemote.LogMessage("MP-TvSeries not installed but required!", WifiRemote.LogType.Error);
                        }
                        break;
                    case MpExtendedProviders.MPMusic:
                        if (type == MpExtendedMediaTypes.MusicTrack)
                        {
                            MpMusicHelper.ShowMusicTrackDetails(Int32.Parse(playInfo["Id"]));
                        }
                        else if (type == MpExtendedMediaTypes.MusicAlbum)
                        {
                            MpMusicHelper.ShowAlbumDetails(playInfo["Artist"], playInfo["Album"]);
                        }
                        else if (type == MpExtendedMediaTypes.MusicArtist)
                        {
                            MpMusicHelper.ShowArtistDetails(playInfo["Artist"]);
                        }
                        break;
                    case MpExtendedProviders.MPVideo:
                        MpVideosHelper.ShowVideoDetails(Int32.Parse(playInfo["Id"]));
                        break;
                    case MpExtendedProviders.MpVideosShare:
                        if (type == MpExtendedMediaTypes.File)
                        {
                            //TODO: fill myvideos db information instead of just playing the file

                            MpVideosHelper.ShowFileDetails(playInfo["Path"]);
                        }
                        else if (type == MpExtendedMediaTypes.Folder)
                        {
                            string[] extensions = playInfo["Extensions"].Split('|');
                            MpVideosHelper.ShowFolderDetails(playInfo["Path"]);
                        }
                        break;
                    case MpExtendedProviders.MpMusicShare:
                        if (type == MpExtendedMediaTypes.File)
                        {
                            MpMusicHelper.ShowFileDetails(playInfo["Path"]);
                        }
                        else if (type == MpExtendedMediaTypes.Folder)
                        {
                            MpMusicHelper.ShowFolderDetails(playInfo["Path"]);
                        }
                        break;
                    default:
                        //we have no providers (yet) for tv
                        if (type == MpExtendedMediaTypes.Recording)
                        {
                            if (!WifiRemote.IsAvailableTVPlugin)
                            {
                                WifiRemote.LogMessage("No TVPlugin installed: Aborting showrecording", WifiRemote.LogType.Error);
                                return;
                            }

                            MpTvServerHelper.ShowRecordingDetails(Int32.Parse(playInfo["Id"]));
                        }
                        else if (type == MpExtendedMediaTypes.Tv)
                        {
                            if (!WifiRemote.IsAvailableTVPlugin)
                            {
                                WifiRemote.LogMessage("No TVPlugin installed: Aborting showchannel", WifiRemote.LogType.Error);
                                return;
                            }

                            MpTvServerHelper.ShowTvChannelDetails(Int32.Parse(playInfo["Id"]));
                        }
                        else
                        {
                            WifiRemote.LogMessage("Provider not implemented yet", WifiRemote.LogType.Warn);
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                WifiRemote.LogMessage("Error during show of MediaItem: " + ex.ToString(), WifiRemote.LogType.Error);
            }
        }
    }
}
