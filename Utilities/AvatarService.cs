using System;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;

namespace YClimb.Utilities
{
    public class AvatarService
    {


        public BitmapImage GetDefaultAvatarImage()
        {
            try
            {
                var bitmapImage = new BitmapImage(new Uri("pack://application:,,,/Images/DefaultUser.jpg"));
                return bitmapImage;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading default avatar image: {ex.Message}");
            }
            return null;
        }

        public byte[] ProcessAndResizeImage(string filePath)
        {
            // Загружаем изображение
            var originalBitmap = new BitmapImage(new Uri(filePath));

            // Обрезаем до квадрата
            var croppedBitmap = CropToSquare(originalBitmap);

            // Изменяем размер до 512x512
            var resizedBitmap = ResizeImage(croppedBitmap, 512, 512);

            // Конвертируем в byte[]
            return ConvertBitmapToByteArray(resizedBitmap);
        }

        private BitmapImage CropToSquare(BitmapImage original)
        {
            int size = Math.Min(original.PixelWidth, original.PixelHeight);
            int x = (original.PixelWidth - size) / 2;
            int y = (original.PixelHeight - size) / 2;

            var croppedBitmap = new CroppedBitmap(original, new Int32Rect(x, y, size, size));

            // Конвертируем обратно в BitmapImage
            var bitmapImage = new BitmapImage();
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(croppedBitmap));

            using (var stream = new MemoryStream())
            {
                encoder.Save(stream);
                stream.Seek(0, SeekOrigin.Begin);

                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = stream;
                bitmapImage.EndInit();
            }

            return bitmapImage;
        }

        private BitmapImage ResizeImage(BitmapImage original, int newWidth, int newHeight)
        {
            var resizedBitmap = new TransformedBitmap(original,
                new ScaleTransform(
                    (double)newWidth / original.PixelWidth,
                    (double)newHeight / original.PixelHeight,
                    0, 0));

            var bitmapImage = new BitmapImage();
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(resizedBitmap));

            using (var stream = new MemoryStream())
            {
                encoder.Save(stream);
                stream.Seek(0, SeekOrigin.Begin);

                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = stream;
                bitmapImage.EndInit();
            }

            return bitmapImage;
        }

        private byte[] ConvertBitmapToByteArray(BitmapImage bitmapImage)
        {
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmapImage));

            using (var stream = new MemoryStream())
            {
                encoder.Save(stream);
                return stream.ToArray();
            }
        }

        public BitmapImage? ConvertByteArrayToBitmapImage(byte[]? imageData)
        {
            if (imageData == null || imageData.Length == 0)
                return null;

            var bitmapImage = new BitmapImage();
            using (var stream = new MemoryStream(imageData))
            {
                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = stream;
                bitmapImage.EndInit();
            }
            return bitmapImage;
        }
    }
}