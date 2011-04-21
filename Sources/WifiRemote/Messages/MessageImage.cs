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
    class MessageImage : IMessage
    {
        string thumbFolder;

        public string Type
        {
            get { return "image"; }
        }

        public String ImagePath { get; set; }

        byte[] image;
        public byte[] Image
        {
            get 
            { 
                try
                {
                    // ImagePath was empty or does not exists
                    if (ImagePath == null || ImagePath.Equals("") || ImagePath.Equals(" ") || !File.Exists(ImagePath))
                    {
                        return new byte[0];
                    }
                    Image thumbnail = new Bitmap(ImagePath);
                    image = ImageToByteArray(thumbnail);
                }
                catch (Exception)
                {
                    image = new byte[0];
                }
                return image; 
            }
        }

        public MessageImage(String path)
        {
            image = new byte[0];
            thumbFolder = Config.GetFolder(Config.Dir.Thumbs);
            ImagePath = path;
        }


        private byte[] ImageToByteArray(Image img)
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
