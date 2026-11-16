using System.Windows;
using Lab_6.ViewModels;
namespace Lab_6{
    public partial class MainWindow : Window{
        public MainWindow(){
            InitializeComponent();
            DataContext = new MainViewModel();
        }
    }
}
