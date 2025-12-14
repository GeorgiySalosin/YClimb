using Newtonsoft.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using YClimb.Entities;
using YClimb.Utilities;

namespace YClimb.Windows
{
    public partial class ImageEditorWindow : Window
    {
        private byte[] _originalImageBytes;
        private byte[] _resizedImageBytes;
        private BitmapImage _displayImage;
        private List<CircleLayer> _circles = new List<CircleLayer>();
        private List<LineLayer> _lines = new List<LineLayer>();
        private List<UIElement> _circleContainers = new List<UIElement>();
        private List<UIElement> _lineContainers = new List<UIElement>();

        private DoubleCircle _currentCircle;
        private DoubleLine _currentLine;
        private Point _startPoint;
        private bool _isDrawing = false;

        // Размеры изображения после ресайза
        private double _imageWidth = 0;
        private double _imageHeight = 0;

        // Текущий инструмент и цвет
        private DrawingTool _currentTool = DrawingTool.Circle;
        private DrawingColor _currentColor = DrawingColor.Yellow;

        public byte[] EditedImageBytes { get; private set; }
        public string LayerData { get; private set; }

        public ImageEditorWindow(byte[] imageBytes)
        {
            InitializeComponent();
            _originalImageBytes = imageBytes;

            Loaded += ImageEditorWindow_Loaded;
        }

        private void ImageEditorWindow_Loaded(object sender, RoutedEventArgs e)
        {
            LoadAndResizeImage();
            UpdateToolButtons();
            UpdateColorButtons();
        }

        private void LoadAndResizeImage()
        {
            try
            {
                _resizedImageBytes = ImageHelper.ResizeForEditor(_originalImageBytes);


                var dimensions = ImageHelper.GetImageDimensions(_originalImageBytes);
                var resizedDimensions = ImageHelper.GetImageDimensions(_resizedImageBytes);

                
                _displayImage = ImageHelper.CreateBitmapImage(_resizedImageBytes);
                BaseImage.Source = _displayImage;

                _imageWidth = _displayImage.PixelWidth;
                _imageHeight = _displayImage.PixelHeight;

                
                DrawingCanvas.Width = _imageWidth;
                DrawingCanvas.Height = _imageHeight;

                UpdateStatus();

                
                ImageContainer.Height = _imageHeight;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading image: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
            }
        }

        private void UpdateStatus()
        {
            StatusText.Text = $"Image: {_imageWidth}x{_imageHeight} | " +
                             $"Circles: {_circles.Count} | " +
                             $"Lines: {_lines.Count}";
        }

        private Point GetCanvasPoint(MouseEventArgs e)
        {
            // Получаем точку относительно Canvas
            var canvasPoint = e.GetPosition(DrawingCanvas);

            // Ограничиваем координаты размерами Canvas
            double x = Math.Max(0, Math.Min(canvasPoint.X, _imageWidth));
            double y = Math.Max(0, Math.Min(canvasPoint.Y, _imageHeight));

            return new Point(x, y);
        }

        private void DrawingCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var canvasPoint = GetCanvasPoint(e);
                _startPoint = canvasPoint;
                _isDrawing = true;

                if (_currentTool == DrawingTool.Circle)
                {
                    // Создаем контейнер для двойной обводки
                    var container = new Grid();

                    // Внешний круг (контрастный цвет)
                    var outerCircle = new Ellipse
                    {
                        Stroke = GetContrastBrush(),
                        StrokeThickness = 5,
                        Fill = Brushes.Transparent,
                        StrokeDashArray = new DoubleCollection() { 4, 2 },
                        Opacity = 0.8
                    };

                    // Внутренний круг (основной цвет)
                    var innerCircle = new Ellipse
                    {
                        Stroke = GetMainBrush(),
                        StrokeThickness = 3,
                        Fill = Brushes.Transparent,
                        StrokeDashArray = new DoubleCollection() { 4, 2 },
                        Opacity = 0.8
                    };

                    container.Children.Add(outerCircle);
                    container.Children.Add(innerCircle);

                    // Начальная позиция
                    Canvas.SetLeft(container, canvasPoint.X);
                    Canvas.SetTop(container, canvasPoint.Y);
                    DrawingCanvas.Children.Add(container);

                    // Сохраняем ссылки
                    _currentCircle = new DoubleCircle { Container = container, Outer = outerCircle, Inner = innerCircle };
                    StatusText.Text = "Drawing circle... Release mouse to finish.";
                }
                else if (_currentTool == DrawingTool.Line)
                {
                    // Создаем контейнер для линии
                    var container = new Canvas();

                    // Внешняя линия (контрастный цвет, толще)
                    var outerLine = new Line
                    {
                        X1 = canvasPoint.X,
                        Y1 = canvasPoint.Y,
                        X2 = canvasPoint.X,
                        Y2 = canvasPoint.Y,
                        Stroke = GetContrastBrush(),
                        StrokeThickness = 5,
                        StrokeDashArray = new DoubleCollection() { 4, 2 },
                        Opacity = 0.8
                    };

                    // Внутренняя линия (основной цвет)
                    var innerLine = new Line
                    {
                        X1 = canvasPoint.X,
                        Y1 = canvasPoint.Y,
                        X2 = canvasPoint.X,
                        Y2 = canvasPoint.Y,
                        Stroke = GetMainBrush(),
                        StrokeThickness = 3,
                        StrokeDashArray = new DoubleCollection() { 4, 2 },
                        Opacity = 0.8
                    };

                    container.Children.Add(outerLine);
                    container.Children.Add(innerLine);

                    Canvas.SetLeft(container, 0);
                    Canvas.SetTop(container, 0);
                    DrawingCanvas.Children.Add(container);

                    // Сохраняем ссылки
                    _currentLine = new DoubleLine { Container = container, Outer = outerLine, Inner = innerLine };
                    StatusText.Text = "Drawing line... Release mouse to finish.";
                }
            }
        }

        private Brush GetMainBrush()
        {
            return _currentColor == DrawingColor.Yellow ? Brushes.Yellow : Brushes.Red;
        }

        private Brush GetContrastBrush()
        {
            return _currentColor == DrawingColor.Yellow ? Brushes.Black : Brushes.White;
        }

        private string GetMainColorHex()
        {
            return _currentColor == DrawingColor.Yellow ? "#FFFF00" : "#FF0000";
        }

        private string GetContrastColorHex()
        {
            return _currentColor == DrawingColor.Yellow ? "#000000" : "#FFFFFF";
        }

        private void DrawingCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isDrawing)
            {
                var currentPoint = GetCanvasPoint(e);

                if (_currentTool == DrawingTool.Circle && _currentCircle != null)
                {
                    // Вычисляем параметры круга
                    double radius = Math.Sqrt(
                        Math.Pow(currentPoint.X - _startPoint.X, 2) +
                        Math.Pow(currentPoint.Y - _startPoint.Y, 2)
                    ) / 2;

                    double centerX = (_startPoint.X + currentPoint.X) / 2;
                    double centerY = (_startPoint.Y + currentPoint.Y) / 2;

                    // Обновляем оба круга
                    Canvas.SetLeft(_currentCircle.Container, centerX - radius);
                    Canvas.SetTop(_currentCircle.Container, centerY - radius);

                    _currentCircle.Outer.Width = radius * 2;
                    _currentCircle.Outer.Height = radius * 2;
                    _currentCircle.Inner.Width = radius * 2;
                    _currentCircle.Inner.Height = radius * 2;
                }
                else if (_currentTool == DrawingTool.Line && _currentLine != null)
                {
                    // Обновляем обе линии
                    _currentLine.Outer.X2 = currentPoint.X;
                    _currentLine.Outer.Y2 = currentPoint.Y;
                    _currentLine.Inner.X2 = currentPoint.X;
                    _currentLine.Inner.Y2 = currentPoint.Y;
                }
            }
        }

        private void DrawingCanvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (_isDrawing)
            {
                _isDrawing = false;

                if (_currentTool == DrawingTool.Circle && _currentCircle != null)
                {
                    var endPoint = GetCanvasPoint(e);

                    // Вычисляем окончательные параметры
                    double radius = Math.Sqrt(
                        Math.Pow(endPoint.X - _startPoint.X, 2) +
                        Math.Pow(endPoint.Y - _startPoint.Y, 2)
                    ) / 2;

                    double centerX = (_startPoint.X + endPoint.X) / 2;
                    double centerY = (_startPoint.Y + endPoint.Y) / 2;

                    Console.WriteLine($"Circle added: Center({centerX:F0}, {centerY:F0}), Radius: {radius:F0}");

                    // Создаем слой круга
                    var circleLayer = new CircleLayer
                    {
                        CenterX = centerX,
                        CenterY = centerY,
                        Radius = radius,
                        MainColor = GetMainColorHex(),
                        ContrastColor = GetContrastColorHex(),
                        MainThickness = 3,
                        ContrastThickness = 5,
                        ToolType = "circle"
                    };

                    // Сохраняем
                    _circles.Add(circleLayer);
                    _circleContainers.Add(_currentCircle.Container);

                    // Делаем круг постоянным
                    _currentCircle.Outer.StrokeDashArray = null;
                    _currentCircle.Inner.StrokeDashArray = null;
                    _currentCircle.Outer.Opacity = 1.0;
                    _currentCircle.Inner.Opacity = 1.0;
                    _currentCircle = null;
                }
                else if (_currentTool == DrawingTool.Line && _currentLine != null)
                {
                    var endPoint = GetCanvasPoint(e);

                    // Создаем слой линии
                    var lineLayer = new LineLayer
                    {
                        X1 = _startPoint.X,
                        Y1 = _startPoint.Y,
                        X2 = endPoint.X,
                        Y2 = endPoint.Y,
                        MainColor = GetMainColorHex(),
                        ContrastColor = GetContrastColorHex(),
                        MainThickness = 3,
                        ContrastThickness = 5,
                        ToolType = "line"
                    };

                    // Сохраняем
                    _lines.Add(lineLayer);
                    _lineContainers.Add(_currentLine.Container);

                    // Делаем линию постоянной
                    _currentLine.Outer.StrokeDashArray = null;
                    _currentLine.Inner.StrokeDashArray = null;
                    _currentLine.Outer.Opacity = 1.0;
                    _currentLine.Inner.Opacity = 1.0;
                    _currentLine = null;
                }

                UpdateStatus();
            }
        }

        // Вспомогательные классы для хранения двойных элементов
        private class DoubleCircle
        {
            public Grid Container { get; set; }
            public Ellipse Outer { get; set; }
            public Ellipse Inner { get; set; }
        }

        private class DoubleLine
        {
            public Canvas Container { get; set; }
            public Line Outer { get; set; }
            public Line Inner { get; set; }
        }

        private void UndoButton_Click(object sender, RoutedEventArgs e)
        {
            // Удаляем последний элемент в зависимости от текущего инструмента
            if (_currentTool == DrawingTool.Circle && _circles.Count > 0 && !_isDrawing)
            {
                var lastContainer = _circleContainers.LastOrDefault();
                if (lastContainer != null)
                {
                    DrawingCanvas.Children.Remove(lastContainer);
                    _circleContainers.Remove(lastContainer);
                }
                _circles.RemoveAt(_circles.Count - 1);
            }
            else if (_currentTool == DrawingTool.Line && _lines.Count > 0 && !_isDrawing)
            {
                var lastContainer = _lineContainers.LastOrDefault();
                if (lastContainer != null)
                {
                    DrawingCanvas.Children.Remove(lastContainer);
                    _lineContainers.Remove(lastContainer);
                }
                _lines.RemoveAt(_lines.Count - 1);
            }

            UpdateStatus();
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            // Удаляем все элементы с Canvas
            foreach (var container in _circleContainers)
            {
                DrawingCanvas.Children.Remove(container);
            }
            foreach (var container in _lineContainers)
            {
                DrawingCanvas.Children.Remove(container);
            }

            _circles.Clear();
            _lines.Clear();
            _circleContainers.Clear();
            _lineContainers.Clear();
            UpdateStatus();
        }

        private async void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ApplyButton.IsEnabled = false;
                ApplyButton.Content = "Saving...";

                // Сохраняем данные слоев (объединяем круги и линии)
                var allLayers = new List<object>();
                allLayers.AddRange(_circles.Cast<object>());
                allLayers.AddRange(_lines.Cast<object>());
                LayerData = JsonConvert.SerializeObject(allLayers);

                // Рендерим финальное изображение (уже масштабированное)
                EditedImageBytes = await Task.Run(() => RenderFinalImage());

                if (EditedImageBytes == null || EditedImageBytes.Length == 0)
                {
                    throw new InvalidOperationException("Failed to render image");
                }

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving image: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);

                ApplyButton.IsEnabled = true;
                ApplyButton.Content = "💾 Save Image";
                UpdateStatus();
            }
        }

        private byte[] RenderFinalImage()
        {
            try
            {

                // Используем ImageRenderer с масштабированным изображением
                var result = ImageRenderer.RenderImageWithLayers(
                    _resizedImageBytes,
                    _circles.Cast<object>().Concat(_lines.Cast<object>()).ToList(),
                    _imageWidth,
                    _imageHeight
                );

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in RenderFinalImage: {ex.Message}\n{ex.StackTrace}");
                return _resizedImageBytes;
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        // Обработчики для выбора инструментов
        private void CircleToolButton_Click(object sender, RoutedEventArgs e)
        {
            _currentTool = DrawingTool.Circle;
            UpdateToolButtons();
            StatusText.Text = "Tool: Circle - Click and drag to draw a circle";
        }

        private void LineToolButton_Click(object sender, RoutedEventArgs e)
        {
            _currentTool = DrawingTool.Line;
            UpdateToolButtons();
            StatusText.Text = "Tool: Line - Click and drag to draw a line";
        }

        private void YellowColorButton_Click(object sender, RoutedEventArgs e)
        {
            _currentColor = DrawingColor.Yellow;
            UpdateColorButtons();
            StatusText.Text = "For outlining all route holds";
        }

        private void RedColorButton_Click(object sender, RoutedEventArgs e)
        {
            _currentColor = DrawingColor.Red;
            UpdateColorButtons();
            StatusText.Text = "For selecting TOP & start holds";
        }

        private void UpdateToolButtons()
        {
            CircleToolButton.IsEnabled = !(_currentTool == DrawingTool.Circle);
            LineToolButton.IsEnabled = !(_currentTool == DrawingTool.Line);
        }

        private void UpdateColorButtons()
        {
            YellowColorButton.IsEnabled = !(_currentColor == DrawingColor.Yellow);
            RedColorButton.IsEnabled = !(_currentColor == DrawingColor.Red);
        }

        
    }

    public enum DrawingTool
    {
        Circle,
        Line
    }

    public enum DrawingColor
    {
        Yellow,
        Red
    }
}