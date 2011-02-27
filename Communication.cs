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
        private InputHandler remoteHandler;
        private Thread m_keyDownThread;
        private String m_keyDownChar;
        private int m_keyDownPauses;
        private bool m_KeyDown;

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
        /// Sends a key repeatedly until it is stopped by a key-up command or the timeout is reached
        /// </summary>
        /// <param name="keyChar">the key that is being pressed</param>
        /// <param name="msBetweenPresses">how much pause between presses</param>
        public void SendKeyDown(String keyChar, int msBetweenPresses)
        {
            m_keyDownChar = keyChar;
            m_keyDownPauses = msBetweenPresses;

            if (!m_KeyDown)
            {
                m_keyDownThread = new Thread(new ThreadStart(DoKeyDown));
                m_keyDownThread.Start();
            }
        }

        /// <summary>
        /// Sends key-up so a running key-down is cancelled
        /// </summary>
        public void SendKeyUp()
        {
            m_KeyDown = false;
        }

        /// <summary>
        /// Thread for sending key-down
        /// </summary>
        private void DoKeyDown()
        {
            m_KeyDown = true;
            while (m_KeyDown)
            {
                SendCommand(m_keyDownChar);
                Thread.Sleep(m_keyDownPauses);
            }

            m_KeyDown = false;
        }

        /// <summary>
        /// Send a keypress to mediaportal
        /// 
        /// TODO: What about umlauts?
        /// </summary>
        public void SendKey(String keyChar)
        {
            int modifiers = 1;
            int keyCode = 0;

            if (keyChar != keyChar.ToLower())
            {
                modifiers = 0;
            }

            switch (keyChar.ToLower())
            {
                case "0":
                    keyCode = (int)Keys.D0;
                    break;

                case "1":
                    keyCode = (int)Keys.D1;
                    break;

                case "2":
                    keyCode = (int)Keys.D2;
                    break;

                case "3":
                    keyCode = (int)Keys.D3;
                    break;

                case "4":
                    keyCode = (int)Keys.D4;
                    break;

                case "5":
                    keyCode = (int)Keys.D5;
                    break;

                case "6":
                    keyCode = (int)Keys.D6;
                    break;

                case "7":
                    keyCode = (int)Keys.D7;
                    break;

                case "8":
                    keyCode = (int)Keys.D8;
                    break;

                case "9":
                    keyCode = (int)Keys.D9;
                    break;


                case "a":
                    keyCode = (int)Keys.A;
                    break;

                case "b":
                    keyCode = (int)Keys.B;
                    break;

                case "c":
                    keyCode = (int)Keys.C;
                    break;

                case "d":
                    keyCode = (int)Keys.D;
                    break;

                case "e":
                    keyCode = (int)Keys.E;
                    break;

                case "f":
                    keyCode = (int)Keys.F;
                    break;

                case "g":
                    keyCode = (int)Keys.G;
                    break;

                case "h":
                    keyCode = (int)Keys.H;
                    break;

                case "i":
                    keyCode = (int)Keys.I;
                    break;

                case "j":
                    keyCode = (int)Keys.J;
                    break;

                case "k":
                    keyCode = (int)Keys.K;
                    break;

                case "l":
                    keyCode = (int)Keys.L;
                    break;

                case "m":
                    keyCode = (int)Keys.M;
                    break;

                case "n":
                    keyCode = (int)Keys.N;
                    break;

                case "o":
                    keyCode = (int)Keys.O;
                    break;

                case "p":
                    keyCode = (int)Keys.P;
                    break;

                case "q":
                    keyCode = (int)Keys.Q;
                    break;

                case "r":
                    keyCode = (int)Keys.R;
                    break;

                case "s":
                    keyCode = (int)Keys.S;
                    break;

                case "t":
                    keyCode = (int)Keys.T;
                    break;

                case "u":
                    keyCode = (int)Keys.U;
                    break;

                case "v":
                    keyCode = (int)Keys.V;
                    break;

                case "w":
                    keyCode = (int)Keys.W;
                    break;

                case "x":
                    keyCode = (int)Keys.X;
                    break;

                case "y":
                    keyCode = (int)Keys.Y;
                    break;

                case "z":
                    keyCode = (int)Keys.Z;
                    break;


                case "f1":
                    keyCode = (int)Keys.F1;
                    break;

                case "f2":
                    keyCode = (int)Keys.F2;
                    break;

                case "f3":
                    keyCode = (int)Keys.F3;
                    break;

                case "f4":
                    keyCode = (int)Keys.F4;
                    break;

                case "f5":
                    keyCode = (int)Keys.F5;
                    break;

                case "f6":
                    keyCode = (int)Keys.F6;
                    break;

                case "f7":
                    keyCode = (int)Keys.F7;
                    break;

                case "f8":
                    keyCode = (int)Keys.F8;
                    break;

                case "f9":
                    keyCode = (int)Keys.F9;
                    break;

                case "f10":
                    keyCode = (int)Keys.F10;
                    break;

                case "f11":
                    keyCode = (int)Keys.F11;
                    break;

                case "f12":
                    keyCode = (int)Keys.F12;
                    break;
            }

            Key key = new Key(keyCode + (modifiers * 32), 0);
            MediaPortal.GUI.Library.Action action = new MediaPortal.GUI.Library.Action(key, MediaPortal.GUI.Library.Action.ActionType.ACTION_KEY_PRESSED, 0, 0);

            GUIWindowManager.OnAction(action);
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
            if (volume >= 0 && volume <= 100)
            {
                VolumeHandler.Instance.Volume = (int)Math.Floor(volume * VolumeHandler.Instance.Maximum / 100.0);
            }
        }

        /// <summary>
        /// Plays the local file on the MediaPortal client
        /// </summary>
        /// <param name="video">Path to the video</param>
        internal void PlayVideoFile(string video)
        {
            if (video != null & File.Exists(video))
            {
                // Play File
                g_Player.Play(video, g_Player.MediaType.Video);
            }
        }
    }
}
