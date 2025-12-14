using Newtonsoft.Json;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using YClimb.Entities;

namespace YClimb.Utilities
{
    public static class ImageRenderer
    {
        public static byte[] RenderImageWithLayers(byte[] baseImageBytes, List<object> layers,
                                            double imageWidth, double imageHeight)
        {
            try
            {
                // Загружаем базовое изображение
                var baseImage = LoadBitmapImage(baseImageBytes);
                if (baseImage == null) return baseImageBytes;

                var drawingVisual = new DrawingVisual();

                using (var drawingContext = drawingVisual.RenderOpen())
                {
                    // Рисуем базовое изображение
                    drawingContext.DrawImage(baseImage, new Rect(0, 0, imageWidth, imageHeight));

                    // Рисуем все слои
                    if (layers != null)
                    {
                        foreach (var layer in layers)
                        {
                            if (layer is CircleLayer circle)
                            {
                                DrawDoubleCircle(drawingContext, circle);
                            }
                            else if (layer is LineLayer line)
                            {
                                DrawDoubleLine(drawingContext, line);
                            }
                        }
                    }
                }

                // Рендерим в BitmapSource
                var renderTarget = new RenderTargetBitmap(
                    (int)Math.Round(imageWidth),
                    (int)Math.Round(imageHeight),
                    96, 96, PixelFormats.Pbgra32);

                renderTarget.Render(drawingVisual);
                renderTarget.Freeze();

                // Конвертируем в byte[]
                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(renderTarget));

                using (var stream = new MemoryStream())
                {
                    encoder.Save(stream);
                    return stream.ToArray();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in ImageRenderer: {ex.Message}\n{ex.StackTrace}");
                return baseImageBytes;
            }
        }

        private static void DrawDoubleCircle(DrawingContext drawingContext, CircleLayer circle)
        {
            try
            {
                // Цвета
                var contrastColor = (Color)ColorConverter.ConvertFromString(circle.ContrastColor);
                var mainColor = (Color)ColorConverter.ConvertFromString(circle.MainColor);

                // Сначала рисуем внешнюю (контрастную) обводку
                var contrastPen = new Pen(new SolidColorBrush(contrastColor), circle.ContrastThickness);
                contrastPen.Freeze();

                // Затем рисуем внутреннюю (основную) обводку
                var mainPen = new Pen(new SolidColorBrush(mainColor), circle.MainThickness);
                mainPen.Freeze();

                // Рассчитываем смещение для центрирования
                double totalThickness = Math.Max(circle.ContrastThickness, circle.MainThickness);

                // Рисуем внешний круг (контрастный)
                drawingContext.DrawEllipse(
                    Brushes.Transparent,
                    contrastPen,
                    new Point(circle.CenterX, circle.CenterY),
                    circle.Radius,
                    circle.Radius);

                // Рисуем внутренний круг (основной)
                drawingContext.DrawEllipse(
                    Brushes.Transparent,
                    mainPen,
                    new Point(circle.CenterX, circle.CenterY),
                    circle.Radius,
                    circle.Radius);

                Console.WriteLine($"Drawn double circle at ({circle.CenterX:F0},{circle.CenterY:F0}) R={circle.Radius:F0}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in DrawDoubleCircle: {ex.Message}");
            }
        }

        private static void DrawDoubleLine(DrawingContext drawingContext, LineLayer line)
        {
            try
            {
                // Цвета
                var contrastColor = (Color)ColorConverter.ConvertFromString(line.ContrastColor);
                var mainColor = (Color)ColorConverter.ConvertFromString(line.MainColor);

                // Сначала рисуем внешнюю (контрастную) линию
                var contrastPen = new Pen(new SolidColorBrush(contrastColor), line.ContrastThickness);
                contrastPen.Freeze();

                // Затем рисуем внутреннюю (основную) линию
                var mainPen = new Pen(new SolidColorBrush(mainColor), line.MainThickness);
                mainPen.Freeze();

                // Рисуем внешнюю линию (контрастную)
                drawingContext.DrawLine(
                    contrastPen,
                    new Point(line.X1, line.Y1),
                    new Point(line.X2, line.Y2));

                // Рисуем внутреннюю линию (основную)
                drawingContext.DrawLine(
                    mainPen,
                    new Point(line.X1, line.Y1),
                    new Point(line.X2, line.Y2));

                Console.WriteLine($"Drawn double line from ({line.X1:F0},{line.Y1:F0}) to ({line.X2:F0},{line.Y2:F0})");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in DrawDoubleLine: {ex.Message}");
            }
        }

        private static BitmapImage LoadBitmapImage(byte[] imageBytes)
        {
            try
            {
                var bitmapImage = new BitmapImage();
                using (var stream = new MemoryStream(imageBytes))
                {
                    bitmapImage.BeginInit();
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.StreamSource = stream;
                    bitmapImage.EndInit();
                    bitmapImage.Freeze();
                }
                return bitmapImage;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading image: {ex.Message}");
                return null;
            }
        }

    }
}