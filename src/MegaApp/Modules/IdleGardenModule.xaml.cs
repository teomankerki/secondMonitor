using System.Windows.Controls;
using MegaApp.ViewModels;

namespace MegaApp.Modules;

public partial class IdleGardenModule : UserControl
{
    public IdleGardenModule()
    {
        InitializeComponent();
        DataContext = new IdleGardenViewModel();
    }
}
