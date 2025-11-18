using System.Windows.Controls;
using MegaApp.ViewModels;

namespace MegaApp.Modules;

public partial class NotesModule : UserControl
{
    public NotesModule()
    {
        InitializeComponent();
        DataContext = new NotesViewModel();
    }
}
