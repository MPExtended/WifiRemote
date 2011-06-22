using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;
using MediaPortal.GUI.Library;
using MediaPortal.Configuration;
using System.Drawing.Drawing2D;

namespace WifiRemote
{
    class MessageImage : IMessage
    {
        int width;
        int height;
        string thumbFolder;

        public string Type
        {
            get { return "image"; }
        }

        public String ImagePath { get; set; }

        /// <summary>
        /// User definable tag for this image request
        /// </summary>
        public String UserTag { get; set; }

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

                    // Resize if necessary
                    Image thumbnail = new Bitmap(ImagePath);
                    if ((width > 0 && thumbnail.Width > width) || (height > 0 && thumbnail.Height > height))
                    {
                        thumbnail = getResizedImage(thumbnail);
                    }

                    // Convert to byte array
                    image = ImageToByteArray(thumbnail);
                }
                catch (Exception)
                {
                    image = new byte[0];
                }

                return image; 
            }
        }

        public MessageImage(String path, String tag, int imageWidth, int imageHeight)
        {
            image = new byte[0];
            thumbFolder = Config.GetFolder(Config.Dir.Thumbs);
            ImagePath = path;
            UserTag = tag;
            width = imageWidth;
            height = imageHeight;
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

        /// <summary>
        /// Returns a resized image that fits in width and height
        /// </summary>
        /// <returns></returns>
        private Image getResizedImage(Image imageToResize)
        {
            // Do not increase image size
            if (imageToResize.Width < width && imageToResize.Height < height)
            {
                return imageToResize;
            }

            // Prevent using images internal thumbnail
            imageToResize.RotateFlip(System.Drawing.RotateFlipType.Rotate180FlipNone);
            imageToResize.RotateFlip(System.Drawing.RotateFlipType.Rotate180FlipNone);

            // Find out the scale factor for the resize
            double widthScale = 0;
            double heightScale = 0;

            if (imageToResize.Width > 0)
            {
                widthScale = (double)width / (double)imageToResize.Width;
            }

            if (imageToResize.Height > 0)
            {
                heightScale = (double)height / (double)imageToResize.Height;
            }

            double scale = Math.Min(widthScale, heightScale);

            // Resize the image
            Image ResizedImage = new Bitmap((int)(imageToResize.Width * scale), (int)(imageToResize.Height * scale));
            using (Graphics graphicsHandle = Graphics.FromImage(ResizedImage))
            {
                graphicsHandle.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphicsHandle.DrawImage(imageToResize, 0, 0, ResizedImage.Width, ResizedImage.Height);
            }

            return ResizedImage;
        }
    }
}
