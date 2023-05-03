using System.Windows;

namespace Helper.Wpf.Main;

public partial class MainWindowView : Window
{
    public MainWindowView(MainWindowViewModel mwvm)
    {
        DataContext = mwvm;
        InitializeComponent();
    }
}