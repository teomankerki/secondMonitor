using System.Windows;
using System.Windows.Controls;
using MegaApp.ViewModels;

namespace MegaApp.Modules;

public partial class PomodoroModule : UserControl
{
    private readonly PomodoroViewModel _viewModel = new();

    public PomodoroModule()
    {
        InitializeComponent();
        DataContext = _viewModel;
    }

    private void OnToggle(object sender, RoutedEventArgs e) => _viewModel.Toggle();

    private void OnReset(object sender, RoutedEventArgs e) => _viewModel.Reset();
}
