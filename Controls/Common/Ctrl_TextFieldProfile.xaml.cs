 using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace YClimb.Controls.Common
{
    public partial class Ctrl_TextFieldProfile : UserControl
    {
        private bool _isEditing = false;
        public event EventHandler<string> TextSaved;

        public Ctrl_TextFieldProfile()
        {
            InitializeComponent();
            DataContext = this;
        }

        public Ctrl_TextFieldProfile(string title = "Title", int maxLength = 32) : this()
        {
            MaxLength = maxLength;
            Title = title;
        }

        public string ButtonIconSource
        {
            get => _buttonIconSource;
            set => _buttonIconSource = value;
        }
        private string _buttonIconSource = "/Images/Icons/edit_pencil_b.png";

        public string ButtonOnEditIconSource { get; set; } = "/Images/Icons/check_b.png";

        public string Title { get; set; }

        public string Text { get; set; }

        public int MaxLength { get; set; } = 32;

        private void ButtonClick(object sender, RoutedEventArgs e)
        {
            HandleSwitch();
        }

        private void HandleSwitch()
        {
            _isEditing = !_isEditing;

            if (_isEditing)
            {
                // Устанавливаем иконку для режима редактирования
                Image.Source = new BitmapImage(new Uri(ButtonOnEditIconSource, UriKind.RelativeOrAbsolute));
                TextBox.Visibility = Visibility.Visible;

                TextBox.Text = TextBlock.Text;
                TextBox.SelectAll();

                TextBlock.Visibility = Visibility.Hidden;
                TextBox.Focus();
            }
            else
            {
                // Возвращаем обычную иконку
                Image.Source = new BitmapImage(new Uri(ButtonIconSource, UriKind.RelativeOrAbsolute));
                TextBox.Visibility = Visibility.Hidden;
                TextBlock.Visibility = Visibility.Visible;
            }

            // Сохраняем текст
            if (TextBox.Text != string.Empty)
            {
                TextBlock.Text = TextBox.Text;
                Text = TextBox.Text; // Обновляем свойство Text

                // Вызываем событие сохранения
                TextSaved?.Invoke(this, TextBox.Text);
            }
        }
            
        
    

    }
}
