using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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

namespace YClimb.Controls
{
    /// <summary>
    /// Логика взаимодействия для Ctrl_TextField.xaml
    /// </summary>
    public partial class Ctrl_TextField : UserControl
    {
        public Ctrl_TextField()
        {
            InitializeComponent();
            DataContext = this;
        }
        public Ctrl_TextField(string title, int maxLength = 32)
        {
            InitializeComponent();
            DataContext = this;
            Title = title;
            MaxLength = maxLength;
        }

        /*static void Validate(TextBox TB)
        {
            
        }*/
        public string Title { get; set; } = "Title";

        public string PlaceholderText { get; set; } = "Value";

        public int MaxLength { get; set; } = 16;

    }
}
