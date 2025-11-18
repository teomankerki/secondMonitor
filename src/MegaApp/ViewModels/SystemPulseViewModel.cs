using System;
using System.Diagnostics;
using System.Windows.Threading;
using MegaApp.Foundation;

namespace MegaApp.ViewModels;

public sealed class SystemPulseViewModel : ObservableObject
{
    private readonly DispatcherTimer _timer;
    private DateTime _now = DateTime.Now;
    private readonly Process _currentProcess = Process.GetCurrentProcess();
    private double _memoryMegabytes;

    public SystemPulseViewModel()
    {
        _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
        _timer.Tick += (_, _) => Refresh();
        Refresh();
        _timer.Start();
    }

    public DateTime Now
    {
        get => _now;
        private set => SetProperty(ref _now, value);
    }

    public double MemoryMegabytes
    {
        get => _memoryMegabytes;
        private set => SetProperty(ref _memoryMegabytes, value);
    }

    private void Refresh()
    {
        Now = DateTime.Now;
        _currentProcess.Refresh();
        MemoryMegabytes = Math.Round(_currentProcess.WorkingSet64 / 1024d / 1024d, 1);
    }
}
