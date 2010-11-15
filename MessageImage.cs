using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;
using MediaPortal.GUI.Library;
using MediaPortal.Configuration;

namespace WifiRemote
{
    class MessageImage
    {
        string thumbFolder;
        string type = "image";

        public string Type
        {
            get { return type; }
        }

        byte[] image;
        public byte[] Image
        {
            get 
            { 
                try
                {
                    String thumbFile = GUIPropertyManager.GetProperty("#Play.Current.Title");

                    // thumb was reset
                    if (thumbFile.Equals(" "))
                    {
                        return new byte[0];
                    }

                    String thumbFullPath = Path.Combine(thumbFolder, thumbFile + ".jpg");
                    WifiRemote.LogMessage(thumbFullPath, WifiRemote.LogType.Info);

                    Image thumbnail = new Bitmap(thumbFullPath);
                    image = imageToByteArray(thumbnail);
                }
                catch (Exception)
                {
                    image = new byte[0];
                }
                return image; 
            }
        }

        public MessageImage()
        {
            image = new byte[0];
            thumbFolder = Config.GetFolder(Config.Dir.Thumbs);
        }


        private byte[] imageToByteArray(Image img)
        {
            byte[] byteArray = new byte[0];
            using (MemoryStream stream = new MemoryStream())
            {
                img.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                stream.Close();
                byteArray = stream.ToArray();
            }

            return byteArray;
        }
    }
}
