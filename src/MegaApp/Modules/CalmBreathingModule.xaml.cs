using System;
using System.Windows.Controls;
using System.Windows.Threading;

namespace MegaApp.Modules;

public partial class CalmBreathingModule : UserControl
{
    private readonly DispatcherTimer _timer;
    private bool _inhale = true;

    public CalmBreathingModule()
    {
        InitializeComponent();
        _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(4) };
        _timer.Tick += (_, _) => TogglePhase();
        _timer.Start();
    }

    private void TogglePhase()
    {
        _inhale = !_inhale;
        PhaseText.Text = _inhale ? "Inhale" : "Exhale";
    }
}
