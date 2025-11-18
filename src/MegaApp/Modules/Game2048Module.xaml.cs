using System.Windows.Controls;
using MegaApp.ViewModels;

namespace MegaApp.Modules;

public partial class Game2048Module : UserControl
{
    public Game2048Module()
    {
        InitializeComponent();
        DataContext = new Game2048ViewModel();
    }
}
