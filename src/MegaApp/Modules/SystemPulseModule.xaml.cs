using System.Windows.Controls;
using MegaApp.ViewModels;

namespace MegaApp.Modules;

public partial class SystemPulseModule : UserControl
{
    public SystemPulseModule()
    {
        InitializeComponent();
        DataContext = new SystemPulseViewModel();
    }
}
