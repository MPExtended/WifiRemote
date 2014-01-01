using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;
using MediaPortal.GUI.Library;

namespace WifiRemote
{
    class ImageHelperError
    {
        public enum ImageHelperErrorType
        {
            WatcherCreate,
            WatcherEnable,
            DirectoryCreate,
            Timeout,
            ScreenshotRead
        };

        /// <summary>
        /// Unique code for this error
        /// </summary>
        public int ErrorCode { get; set; }

        /// <summary>
        /// Descriptive message of this error
        /// </summary>
        public String ErrorMessage { get; set; }

        public ImageHelperError(ImageHelperErrorType type)
        {
            setupExceptionWithType(type);
        }

        protected void setupExceptionWithType(ImageHelperErrorType type)
        {
            switch (type)
            {
                case ImageHelperErrorType.WatcherCreate:
                    ErrorCode = 10;
                    ErrorMessage = "Could not watch for MediaPortal screenshots.";
                    break;

                case ImageHelperErrorType.WatcherEnable:
                    ErrorCode = 11;
                    ErrorMessage = "Error starting to watch for MediaPortal screenshots.";
                    break;

                case ImageHelperErrorType.DirectoryCreate:
                    ErrorCode = 20;
                    ErrorMessage = "Could not create screenshot directory.";
                    break;

                case ImageHelperErrorType.Timeout:
                    ErrorCode = 30;
                    ErrorMessage = "Timeout while waiting for MediaPortal to take the screenshot.";
                    break;
                    
                case ImageHelperErrorType.ScreenshotRead:
                    ErrorCode = 40;
                    ErrorMessage = "Could not read MediaPortal screenshot";
                    break;

                default:
                    ErrorCode = 0;
                    ErrorMessage = "An unexpected error occured.";
                    break;
            }
        }
    }

    class ImageHelper
    {
        #region Take a MediaPortal screenshot

        /// <summary>
        /// Callback for when the screenshot was taken and is stored as a
        /// byte array in the Screenshot property.
        /// </summary>
        public delegate void ScreenshotReadyCallback();

        /// <summary>
        /// Callback for when the screenshot could not be taken or processed.
        /// </summary>
        public delegate void ScreenshotFailedCallback(ImageHelperError error);

        /// <summary>
        /// Tracks if a screenshot is being made at the moment
        /// </summary>
        bool takingScreenshot;

        /// <summary>
        /// FileSystemWatcher watching for new screenshots
        /// </summary>
        FileSystemWatcher watcher;

        /// <summary>
        /// Path of the current screenshot
        /// </summary>
        String screenshotPath;

        /// <summary>
        /// Number of times the screenshot was tried to open.
        /// We abort after maximumScreenshotOpenTries times to avoid an
        /// infinite loop.
        /// </summary>
        uint screenshotOpenTries;

        /// <summary>
        /// Abort trying to open the screenshot after this number of tries.
        /// </summary>
        uint maximumScreenshotOpenTries;

        private Image screenshot;
        /// <summary>
        /// The screenshot taken with the takeScreenshot() method
        /// </summary>
        public Image Screenshot
        {
            get { return screenshot; }
            set { screenshot = value; }
        }

        public ImageHelper()
        {
            maximumScreenshotOpenTries = 20;
            screenshotOpenTries = 0;
        }

        /// <summary>
        /// Make MediaPortal take a screenshot, take that and delete it 
        /// from disk. First we need to check if the screenshot folder already exists.
        /// See https://github.com/MediaPortal/MediaPortal-1/blob/cae80bd6dd2241bd7182c39418373bee545bf464/mediaportal/MediaPortal.Application/MediaPortal.cs#L3611
        /// </summary>
        public void TakeScreenshot()
        {
            // Only take one screenshot at a time, all requests
            // will be served from that screenshot.
            if (takingScreenshot)
            {
                return;
            }

            takingScreenshot = true;

            // MediaPortal doesn't output events for new screenshots so we 'manually' 
            // watch the screenshot folder
            setupFileSystemWatcher();

            if (watcher == null)
            {
                // Something went wrong creating the filesystem watcher
                takingScreenshot = false;
                OnScreenshotFailed(new ImageHelperError(ImageHelperError.ImageHelperErrorType.WatcherCreate));
                return;
            }

            if (!watcher.EnableRaisingEvents)
            {
                try
                {
                    watcher.EnableRaisingEvents = true;
                }
                catch (Exception e)
                {
                    WifiRemote.LogMessage(String.Format("Could not watch the screenshots folder: {0}", e.Message), WifiRemote.LogType.Error);
                    watcher = null;
                    takingScreenshot = false;
                    OnScreenshotFailed(new ImageHelperError(ImageHelperError.ImageHelperErrorType.WatcherEnable));
                    return;
                }
            }

            // Take the screenshot
            MediaPortal.GUI.Library.Action action = new MediaPortal.GUI.Library.Action(MediaPortal.GUI.Library.Action.ActionType.ACTION_TAKE_SCREENSHOT, 0, 0);
            GUIGraphicsContext.OnAction(action);
        }

        /// <summary>
        /// Returns the resized screenshot as a byte array.
        /// </summary>
        /// <param name="width">Width to resize the screenshot proportionally to, 0 to keep original</param>
        /// <returns></returns>
        public byte[] resizedScreenshot(int width)
        {
            if (Screenshot == null)
            {
                return new byte[0];
            }

            Image image = (width > 0) ? ImageHelper.ResizedImage(Screenshot, width) : Screenshot;
            return ImageHelper.imageToByteArray(image, System.Drawing.Imaging.ImageFormat.Png);
        }

        protected void setupFileSystemWatcher()
        {
            String directory = String.Format("{0}\\MediaPortal Screenshots\\{1:0000}-{2:00}-{3:00}",
                                 Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
                                 DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

            if (!Directory.Exists(directory))
            {
                WifiRemote.LogMessage(String.Format("Creating screenshot directory: {0}", directory), WifiRemote.LogType.Info);

                try
                {
                    Directory.CreateDirectory(directory);
                }
                catch (Exception e)
                {
                    WifiRemote.LogMessage(String.Format("Could not create screenshot directory {0}: {1}", directory, e.Message), WifiRemote.LogType.Error);
                    watcher = null;
                    takingScreenshot = false;
                    OnScreenshotFailed(new ImageHelperError(ImageHelperError.ImageHelperErrorType.DirectoryCreate));
                    return;
                }
            }

            if (watcher == null)
            {
                // Add a filesystem watcher to be informed when MediaPortal creates the screenshot
                watcher = new FileSystemWatcher(directory, "*.png");
                watcher.Created += new FileSystemEventHandler(watcherCreated);
            }
            else if (!watcher.Path.Equals(directory))
            {
                // Date changed, update path
                watcher.Path = directory;
            }
        }

        /// <summary>
        /// A screenshot was created
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void watcherCreated(object sender, FileSystemEventArgs e)
        {
            screenshotPath = e.FullPath;

            // Wait until the screenshot is written to disk
            System.Timers.Timer timer = new System.Timers.Timer(100);
            timer.AutoReset = false;
            timer.Elapsed += new System.Timers.ElapsedEventHandler(screenshotReadyCheck);
            timer.Start();
        }

        protected void screenshotReadyCheck(object sender, System.Timers.ElapsedEventArgs e)
        {
            ((System.Timers.Timer)sender).Stop();
            if (IsScreenshotReady(screenshotPath))
            {
                // MediaPortal completed writing the screenshot to disk
                // We can now grab and delete it
                processScreenshot();
                screenshotOpenTries = 0;
            }
            else
            {
                if (screenshotOpenTries < maximumScreenshotOpenTries)
                {
                    // Continue checking if the file is locked
                    WifiRemote.LogMessage("Waiting for screenshot to be written ...", WifiRemote.LogType.Debug);
                    screenshotOpenTries++;
                    ((System.Timers.Timer)sender).Start();
                }
                else
                {
                    WifiRemote.LogMessage("Maximum number of screenshot open tries reached, aborting.", WifiRemote.LogType.Debug);
                    OnScreenshotFailed(new ImageHelperError(ImageHelperError.ImageHelperErrorType.Timeout));
                    screenshotOpenTries = 0;
                }
            }
        }

        protected void processScreenshot()
        {
            try
            {
                using (FileStream stream = new FileStream(screenshotPath, FileMode.Open, FileAccess.Read))
                {
                    Screenshot = Image.FromStream(stream);
                }
            }
            catch (Exception ex)
            {
                WifiRemote.LogMessage(String.Format("Could not open screenshot file {0}: {1}", screenshotPath, ex.Message), WifiRemote.LogType.Error);
                takingScreenshot = false;
                OnScreenshotFailed(new ImageHelperError(ImageHelperError.ImageHelperErrorType.ScreenshotRead));
                return;
            }

            // Delete the screenshot from disk
            try
            {
                File.Delete(screenshotPath);

                if (Directory.GetFiles(Path.GetDirectoryName(screenshotPath), "*.png").Length == 0)
                {
                    // No screenshots in the screenshot folder, delete that as well
                    Directory.Delete(Path.GetDirectoryName(screenshotPath));
                }
            }
            catch (Exception ex)
            {
                WifiRemote.LogMessage(String.Format("Could not delete screenshot or screenshot folder {0}: {1}", screenshotPath, ex.Message), WifiRemote.LogType.Info);
            }

            // Stop listening for new files
            watcher.EnableRaisingEvents = false;

            // Inform observers that the screenshot is now ready
            OnScreenshotReady();
            takingScreenshot = false;
        }

        /// <summary>
        /// Check if the screenshot is locked.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        protected static bool IsScreenshotReady(String path)
        {
            // If the file can be opened for exclusive access it means that the file
            // is no longer locked by another process.
            try
            {
                using (FileStream inputStream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    long streamLength = inputStream.Length;
                    return streamLength > 0;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Screenshot ready event
        /// </summary>
        public event ScreenshotReadyCallback ScreenshotReady;
        protected void OnScreenshotReady()
        {
            if (ScreenshotReady != null)
            {
                ScreenshotReady();
            }
        }

        /// <summary>
        /// Screenshot failed event
        /// </summary>
        public event ScreenshotFailedCallback ScreenshotFailed;
        protected void OnScreenshotFailed(ImageHelperError error)
        {
            if (ScreenshotFailed != null)
            {
                ScreenshotFailed(error);
            }
        }

        #endregion

        #region Static utility methods

        /// <summary>
        /// Returns an image as its byte array representation.
        /// Used to make images encodable in JSON.
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        public static byte[] imageToByteArray(Image img, System.Drawing.Imaging.ImageFormat format)
        {
            byte[] byteArray = new byte[0];
            using (MemoryStream stream = new MemoryStream())
            {
                img.Save(stream, format);
                stream.Close();
                byteArray = stream.ToArray();
            }

            return byteArray;
        }

        /// <summary>
        /// Resizes an image to the target with. The height is calculated
        /// proportionally to the source image height.
        /// </summary>
        /// <param name="source">The source image to resize</param>
        /// <param name="width">Target width for the resized image</param>
        /// <returns></returns>
        public static Image ResizedImage(Image source, int width)
        {
            if (source.Width <= width)
            {
                return source;
            }

            int height = (int)(source.Height / ((double)source.Width / (double)width));
            Image target = new Bitmap(source, width, height);

            return target;
        }

        #endregion
    }
}
