using System.Windows.Controls;
using MegaApp.ViewModels;

namespace MegaApp.Modules;

public partial class EndlessRunnerModule : UserControl
{
    public EndlessRunnerModule()
    {
        InitializeComponent();
        DataContext = new EndlessRunnerViewModel();
    }
}
