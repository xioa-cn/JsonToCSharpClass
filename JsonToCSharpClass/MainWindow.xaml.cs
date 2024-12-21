using JsonToCSharpClass.ViewModels;
using System.Windows;

namespace JsonToCSharpClass
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.DataContext = new MainViewModel();
            InitializeComponent();
        }
    }
}