using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace YClimb.Utilities
{
    public static class ImageHelper
    {
        /// <summary>
        /// Масштабирует изображение до указанной ширины с сохранением пропорций
        /// </summary>
        public static byte[] ResizeToWidth(byte[] imageBytes, int targetWidth)
        {
            if (imageBytes == null || imageBytes.Length == 0)
                return imageBytes;

            try
            {
                using (var inputStream = new MemoryStream(imageBytes))
                using (var originalImage = System.Drawing.Image.FromStream(inputStream) as System.Drawing.Bitmap)
                {
                    if (originalImage == null)
                        return imageBytes;

                    
                    double ratio = (double)targetWidth / originalImage.Width;
                    int targetHeight = (int)(originalImage.Height * ratio);

                    Console.WriteLine($"Resizing image from {originalImage.Width}x{originalImage.Height} to {targetWidth}x{targetHeight}");

                    
                    //if (originalImage.Width <= targetWidth)
                    //    return imageBytes;

                    
                    using (var resizedImage = new System.Drawing.Bitmap(targetWidth, targetHeight))
                    {
                        using (var graphics = System.Drawing.Graphics.FromImage(resizedImage))
                        {
                            graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                            graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                            graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

                            graphics.DrawImage(originalImage, 0, 0, targetWidth, targetHeight);
                        }

                        
                        using (var outputStream = new MemoryStream())
                        {
                            var encoder = GetEncoder(System.Drawing.Imaging.ImageFormat.Png);

                            resizedImage.Save(outputStream, encoder, null);
                            return outputStream.ToArray();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error resizing image: {ex.Message}");
                return imageBytes;
            }
        }

        
        public static byte[] ResizeForEditor(byte[] imageBytes)
        {
            return ResizeToWidth(imageBytes, 860);
        }

        
        private static System.Drawing.Imaging.ImageCodecInfo GetEncoder(System.Drawing.Imaging.ImageFormat format)
        {
            var codecs = System.Drawing.Imaging.ImageCodecInfo.GetImageDecoders();
            foreach (var codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }

        
        public static BitmapImage CreateBitmapImage(byte[] imageData)
        {
            if (imageData == null || imageData.Length == 0)
                return new BitmapImage();

            try
            {
                var bitmap = new BitmapImage();
                using (var stream = new MemoryStream(imageData))
                {
                    bitmap.BeginInit();
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                    bitmap.StreamSource = stream;
                    bitmap.EndInit();
                }
                bitmap.Freeze();
                return bitmap;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating bitmap: {ex.Message}");
                return new BitmapImage();
            }
        }

        
        public static (int Width, int Height) GetImageDimensions(byte[] imageData)
        {
            try
            {
                using (var stream = new MemoryStream(imageData))
                using (var image = System.Drawing.Image.FromStream(stream))
                {
                    return (image.Width, image.Height);
                }
            }
            catch
            {
                return (0, 0);
            }
        }
    }
}