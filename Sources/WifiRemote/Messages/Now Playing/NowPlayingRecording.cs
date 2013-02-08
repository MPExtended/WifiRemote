using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MediaPortal.Video.Database;
using System.IO;
using System.Drawing;
using WifiRemote.MpExtended;

namespace WifiRemote
{
    public class NowPlayingRecording : IAdditionalNowPlayingInfo
    {
        string mediaType = "recording";
        bool recordingFound = false;
        public string MediaType
        {
            get { return mediaType; }
        }

        public string MpExtId
        {
            get { return RecordingId.ToString(); }
        }

        public int MpExtMediaType
        {
            get { return (int)MpExtendedMediaTypes.Recording; }
        }

        public int MpExtProviderId
        {
            get { return 0; }//no tv providers yet
        }

        /// <summary>
        /// ID of the channel
        /// </summary>
        public int ChannelId
        {
            get;
            set;
        }

        /// <summary>
        /// Id of recording
        /// </summary>
        public int RecordingId
        {
            get;
            set;
        }

        /// <summary>
        /// Name of channel
        /// </summary>
        public String ChannelName
        {
            get;
            set;
        }

        /// <summary>
        /// Name of program
        /// </summary>
        public string ProgramName
        {
            get;
            set;
        }

        /// <summary>
        /// Description of program
        /// </summary>
        public string ProgramDescription
        {
            get;
            set;
        }

        /// <summary>
        /// Start date of program
        /// </summary>
        public DateTime ProgramBegin
        {
            get;
            set;
        }

        /// <summary>
        /// End date of program
        /// </summary>
        public DateTime ProgramEnd
        {
            get;
            set;
        }


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="filename">The currently playing recording</param>
        public NowPlayingRecording(string filename)
        {
            TvDatabase.Recording recording = TvDatabase.Recording.Retrieve(filename);
            if (recording != null)
            {
                recordingFound = true;
                ChannelId = recording.IdChannel;
                RecordingId = recording.IdRecording;
                ProgramName = recording.Title;
                ProgramDescription = recording.Description;
                ProgramBegin = recording.StartTime;
                ProgramEnd = recording.EndTime;

                TvDatabase.Channel channel = TvDatabase.Channel.Retrieve(ChannelId);
                if (channel != null)
                {
                    ChannelName = channel.DisplayName;
                }
            }                 
        }

        public bool IsRecording()
        {
            return recordingFound;
        }
    }
}
