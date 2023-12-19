using Avalonia.Controls;
using KMeansV3.ViewModels;

namespace KMeansV3.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainWindowViewModel(/*this.Find<ItemsControl>("ic")*/);
    }
}