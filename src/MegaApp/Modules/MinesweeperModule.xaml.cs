using System.Windows.Controls;
using MegaApp.ViewModels;

namespace MegaApp.Modules;

public partial class MinesweeperModule : UserControl
{
    public MinesweeperModule()
    {
        InitializeComponent();
        DataContext = new MinesweeperViewModel();
    }
}
