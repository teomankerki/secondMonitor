using System.Windows.Controls;
using MegaApp.ViewModels;

namespace MegaApp.Modules;

public partial class FidgetSpinnerModule : UserControl
{
    public FidgetSpinnerModule()
    {
        InitializeComponent();
        DataContext = new FidgetSpinnerViewModel();
    }
}
