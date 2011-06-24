using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MediaPortal.InputDevices;
using System.Windows.Forms;
using MediaPortal.GUI.Library;
using MediaPortal.Util;
using MediaPortal.Player;
using System.Threading;
using System.IO;

namespace WifiRemote
{
    class Communication
    {
        private const int KEY_DOWN_TIMEOUT = 2000;
        private InputHandler remoteHandler;
        private Thread commandDownThread;
        private String commandDownChar;
        private int commandDownPauses;
        private bool isCommandDown;
        private StopWatch m_keyDownTimer;

        private enum RemoteButton
        {
            // Same as MCE buttons
            NumPad0 = 0,
            NumPad1 = 1,
            NumPad2 = 2,
            NumPad3 = 3,
            NumPad4 = 4,
            NumPad5 = 5,
            NumPad6 = 6,
            NumPad7 = 7,
            NumPad8 = 8,
            NumPad9 = 9,
            Clear = 10,
            Enter = 11,
            Power2 = 12,
            Start = 13,
            Mute = 14,
            Info = 15,
            VolumeUp = 16,
            VolumeDown = 17,
            ChannelUp = 18,
            ChannelDown = 19,
            Forward = 20,
            Rewind = 21,
            Play = 22,
            Record = 23,
            Pause = 24,
            Stop = 25,
            Skip = 26,
            Replay = 27,
            OemGate = 28,
            Oem8 = 29,
            Up = 30,
            Down = 31,
            Left = 32,
            Right = 33,
            Ok = 34,
            Back = 35,
            DVDMenu = 36,
            LiveTV = 37,
            Guide = 38,
            AspectRatio = 39, // FIC Spectra

            // Wifi Remote stuff
            Menu = 40,
            First = 41,
            Last = 42,
            Fullscreen = 43,
            Subtitles = 44,
            AudioTrack = 45,
            Screenshot = 46,
            // End of Wifi Remote stuff

            RecordedTV = 72,
            Print = 78, // Hewlett Packard MCE Edition
            Teletext = 90,
            Red = 91,
            Green = 92,
            Yellow = 93,
            Blue = 94,
            PowerTV = 101,
            Power1 = 165,

            Home = 800,
            BasicHome = 801,
            NowPlaying = 808,
            PlayDVD = 809,
            MyPlaylists = 810
        }


        /// <summary>
        /// Constructor.
        /// </summary>
        public Communication()
        {
            remoteHandler = new InputHandler("WifiRemote");
            m_keyDownTimer = new StopWatch();
        }

        /// <summary>
        /// Send a command to mediaportal.
        /// Commands are defined in an input handler xml file.
        /// 
        /// A command is for example "up" or "play".
        /// </summary>
        /// <param name="command"></param>
        public void SendCommand(String command)
        {
            RemoteButton button;

            switch (command.ToLower())
            {
                case "stop":
                    button = RemoteButton.Stop;
                    break;

                case "record":
                    button = RemoteButton.Record;
                    break;

                case "pause":
                    button = RemoteButton.Pause;
                    break;

                case "play":
                    button = RemoteButton.Play;
                    break;

                case "rewind":
                    button = RemoteButton.Rewind;
                    break;

                case "forward":
                    button = RemoteButton.Forward;
                    break;

                case "replay":
                    button = RemoteButton.Replay;
                    break;

                case "skip":
                    button = RemoteButton.Skip;
                    break;

                case "back":
                    button = RemoteButton.Back;
                    break;

                case "info":
                    button = RemoteButton.Info;
                    break;

                case "menu":
                    button = RemoteButton.Menu;
                    break;

                case "up":
                    button = RemoteButton.Up;
                    break;

                case "down":
                    button = RemoteButton.Down;
                    break;

                case "left":
                    button = RemoteButton.Left;
                    break;

                case "right":
                    button = RemoteButton.Right;
                    break;

                case "ok":
                    button = RemoteButton.Ok;
                    break;

                case "volup":
                    button = RemoteButton.VolumeUp;
                    break;

                case "voldown":
                    button = RemoteButton.VolumeDown;
                    break;

                case "volmute":
                    button = RemoteButton.Mute;
                    break;

                case "chup":
                    button = RemoteButton.ChannelUp;
                    break;

                case "chdown":
                    button = RemoteButton.ChannelDown;
                    break;

                case "dvdmenu":
                    button = RemoteButton.DVDMenu;
                    break;

                case "0":
                    button = RemoteButton.NumPad0;
                    break;

                case "1":
                    button = RemoteButton.NumPad1;
                    break;

                case "2":
                    button = RemoteButton.NumPad2;
                    break;

                case "3":
                    button = RemoteButton.NumPad3;
                    break;

                case "4":
                    button = RemoteButton.NumPad4;
                    break;

                case "5":
                    button = RemoteButton.NumPad5;
                    break;

                case "6":
                    button = RemoteButton.NumPad6;
                    break;

                case "7":
                    button = RemoteButton.NumPad7;
                    break;

                case "8":
                    button = RemoteButton.NumPad8;
                    break;

                case "9":
                    button = RemoteButton.NumPad9;
                    break;

                case "clear":
                    button = RemoteButton.Clear;
                    break;

                case "enter":
                    button = RemoteButton.Enter;
                    break;

                case "teletext":
                    button = RemoteButton.Teletext;
                    break;

                case "red":
                    button = RemoteButton.Red;
                    break;

                case "blue":
                    button = RemoteButton.Blue;
                    break;

                case "yellow":
                    button = RemoteButton.Yellow;
                    break;

                case "green":
                    button = RemoteButton.Green;
                    break;

                case "home":
                    button = RemoteButton.Home;
                    break;

                case "basichome":
                    button = RemoteButton.BasicHome;
                    break;

                case "nowplaying":
                    button = RemoteButton.NowPlaying;
                    break;

                case "tvguide":
                    button = RemoteButton.Guide;
                    break;

                case "tvrecs":
                    button = RemoteButton.RecordedTV;
                    break;

                case "dvd":
                    button = RemoteButton.PlayDVD;
                    break;

                case "playlists":
                    button = RemoteButton.MyPlaylists;
                    break;

                case "first":
                    button = RemoteButton.First;
                    break;

                case "last":
                    button = RemoteButton.Last;
                    break;

                case "fullscreen":
                    button = RemoteButton.Fullscreen;
                    break;

                case "subtitles":
                    button = RemoteButton.Subtitles;
                    break;

                case "audiotrack":
                    button = RemoteButton.AudioTrack;
                    break;

                case "screenshot":
                    button = RemoteButton.Screenshot;
                    break;

                default:
                    return;
            }

            remoteHandler.MapAction((int)button);
            //System.Threading.Thread.Sleep(100);
        }


        /// <summary>
        /// Sends a command repeatedly until it is stopped by a key-up command or the timeout is reached
        /// </summary>
        /// <param name="command">the command that is being pressed</param>
        /// <param name="msBetweenPresses">how much pause between presses</param>
        public void SendCommandRepeatStart(String command, int msBetweenPresses)
        {
            commandDownChar = command;
            commandDownPauses = msBetweenPresses;
            m_keyDownTimer.StartZero();

            if (!isCommandDown)
            {
                commandDownThread = new Thread(new ThreadStart(DoKeyDown));
                commandDownThread.Start();
            }
        }

        /// <summary>
        /// Sends key-up so a running key-down is cancelled
        /// </summary>
        public void SendCommandRepeatStop()
        {
            isCommandDown = false;
        }

        /// <summary>
        /// Thread for sending key-down
        /// </summary>
        private void DoKeyDown()
        {
            isCommandDown = true;
            while (isCommandDown && m_keyDownTimer.ElapsedMilliseconds < KEY_DOWN_TIMEOUT)
            {
                SendCommand(commandDownChar);
                Thread.Sleep(commandDownPauses);
            }
            m_keyDownTimer.Stop();
            isCommandDown = false;
        }

        /// <summary>
        /// Send a keypress to mediaportal
        /// 
        /// TODO: What about umlauts?
        /// </summary>
        public void SendKey(String keyChar)
        {
            if (keyChar == "{DONE}")
             {
                //TODO: simulate pressing "done" on the virtual keyboard -> needs MediaPortal patch
             }
             else
             {
                //Sends a key to mediaportal
                 SendKeys.SendWait(keyChar);
             }
        }

        /// <summary>
        /// Open a window (for example Moving Pictures, MP-TV Series, etc.)
        /// </summary>
        /// <param name="windowId"></param>
        public void OpenWindow(int windowId)
        {
            GUIGraphicsContext.ResetLastActivity();
            GUIMessage msg = new GUIMessage(GUIMessage.MessageType.GUI_MSG_GOTO_WINDOW, 0, 0, 0, windowId, 0, null);

            GUIWindowManager.SendThreadMessage(msg);
        }

        /// <summary>
        /// Shutdown/hibernate/reboot the htpc or exit mediaportal
        /// </summary>
        /// <param name="shutdownType">logoff|suspend|hibernate|reboot|shutdown|exit</param>
        public void SetPowerMode(String powerMode)
        {
            switch (powerMode.ToLower())
            {
                case "logoff":
                    WindowsController.ExitWindows(RestartOptions.LogOff, true);
                    break;

                case "suspend":
                    WindowsController.ExitWindows(RestartOptions.Suspend, true);
                    break;

                case "hibernate":
                    WindowsController.ExitWindows(RestartOptions.Hibernate, true);
                    break;

                case "reboot":
                    WindowsController.ExitWindows(RestartOptions.Reboot, true);
                    break;

                case "shutdown":
                    WindowsController.ExitWindows(RestartOptions.ShutDown, true);
                    break;

                case "exit":
                    MediaPortal.GUI.Library.Action action = new MediaPortal.GUI.Library.Action(MediaPortal.GUI.Library.Action.ActionType.ACTION_EXIT, 0, 0);
                    GUIGraphicsContext.OnAction(action);
                    break;
            }
        }

        /// <summary>
        /// Sets the volume of the client
        /// </summary>
        /// <param name="volume">The new volume, ranging from 0 to 100</param>
        internal void SetVolume(int volume)
        {
            SetVolume(volume, false);
        }

        /// <summary>
        /// Sets the volume of the client
        /// </summary>
        /// <param name="volume">The new volume, ranging from 0 to 100</param>
        /// <param name="relative">True if the volume should be changed relative to the current volume</param>
        internal void SetVolume(int volume, bool relative)
        {
            if (relative)
            {
                int currentVolume = 0;

                try
                {
                    currentVolume = VolumeHandler.Instance.Volume / (VolumeHandler.Instance.Maximum / 100);
                }
                catch (Exception) { }

                volume += currentVolume;
            }

            if (volume >= 0 && volume <= 100)
            {
                VolumeHandler.Instance.Volume = (int)Math.Floor(volume * VolumeHandler.Instance.Maximum / 100.0);
            }
        }

        /// <summary>
        /// Plays the local file on the MediaPortal client
        /// </summary>
        /// <param name="video">Path to the video</param>
        /// <param name="position">Start position</param>
        internal void PlayVideoFile(string video, int position)
        {
            if (video != null && File.Exists(video))
            {
                WifiRemote.LogMessage("Play video file: " + video + ", pos: " + position, WifiRemote.LogType.Debug);
                // from MP-TvSeries code:
                // sometimes it takes up to 30+ secs to go to fullscreen even though the video is already playing
                // lets force fullscreen here
                // note: MP might still be unresponsive during this time, but at least we are in fullscreen and can see video should this happen
                // I haven't actually found out why it happens, but I strongly believe it has something to do with the video database and the player doing something in the background
                // (why does it do anything with the video database.....i just want it to play a file and do NOTHING else!)           
                GUIGraphicsContext.IsFullScreenVideo = true;
                GUIWindowManager.ActivateWindow((int)GUIWindow.Window.WINDOW_FULLSCREEN_VIDEO);

                // Play File
                g_Player.Play(video, g_Player.MediaType.Video);
                if (position != 0)
                {
                    g_Player.SeekAbsolute(position);
                }
                g_Player.ShowFullScreenWindowVideo();
            }
        }

        /// <summary>
        /// Plays the local audio file on the MediaPortal client
        /// </summary>
        /// <param name="video">Path to the audio file</param>
        internal void PlayAudioFile(string audio, double position)
        {
            if (audio != null && File.Exists(audio))
            {
                WifiRemote.LogMessage("Play audio file: " + audio + ", pos: " + position, WifiRemote.LogType.Debug);
                g_Player.Play(audio, g_Player.MediaType.Music);
                if (position != 0)
                {
                    g_Player.SeekAbsolute(position);
                }
            }
        }

        /// <summary>
        /// Set the player position to the given absolute percentage 
        /// </summary>
        /// <param name="position">position in %</param>
        /// <param name="absolute">absolute or relative to current position</param>
        internal void SetPositionPercent(int position, bool absolute)
        {
            if (absolute)
            {
                g_Player.SeekAsolutePercentage(position);
            }
            else
            {
                g_Player.SeekRelativePercentage(position);
            }
        }

        /// <summary>
        /// Set the player position to the given absolute time (in s)
        /// </summary>
        /// <param name="position">position in s</param>
        /// <param name="absolute">absolute or relative to current position</param>
        internal void SetPosition(double position, bool absolute)
        {
            if (absolute)
            {
                g_Player.SeekAbsolute(position);
            }
            else
            {
                g_Player.SeekRelative(position);
            }
        }


    }
}
