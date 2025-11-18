using System.Windows.Controls;
using MegaApp.ViewModels;

namespace MegaApp.Modules;

public partial class SudokuModule : UserControl
{
    public SudokuModule()
    {
        InitializeComponent();
        DataContext = new SudokuViewModel();
    }
}
