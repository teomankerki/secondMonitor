using System;
using System.Windows.Threading;
using MegaApp.Foundation;

namespace MegaApp.ViewModels;

public sealed class FidgetSpinnerViewModel : ObservableObject, IDisposable
{
    private readonly DispatcherTimer _timer;
    private readonly DispatcherTimer _sparkTimer;
    private double _angle;
    private double _velocity;
    private double _friction = 0.985;
    private bool _useAlternatePalette;
    private bool _showSpark;

    public FidgetSpinnerViewModel()
    {
        _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(16) };
        _timer.Tick += OnTick;
        _timer.Start();

        _sparkTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(220) };
        _sparkTimer.Tick += (_, _) =>
        {
            ShowSpark = false;
            _sparkTimer.Stop();
        };

        SpinCommand = new RelayCommand(_ => Spin());
        StopCommand = new RelayCommand(_ => Stop());
        TogglePaletteCommand = new RelayCommand(_ => UseAlternatePalette = !UseAlternatePalette);
    }

    public RelayCommand SpinCommand { get; }

    public RelayCommand StopCommand { get; }

    public RelayCommand TogglePaletteCommand { get; }

    public double Angle
    {
        get => _angle;
        private set => SetProperty(ref _angle, value);
    }

    public bool UseAlternatePalette
    {
        get => _useAlternatePalette;
        private set => SetProperty(ref _useAlternatePalette, value);
    }

    public bool ShowSpark
    {
        get => _showSpark;
        private set => SetProperty(ref _showSpark, value);
    }

    public double DisplaySpeed => Math.Round(Math.Abs(_velocity), 1);

    public double SpeedNormalized => Math.Min(1, Math.Abs(_velocity) / 40);

    public double MotionBlurOpacity => Math.Min(0.85, SpeedNormalized);

    public void Spin()
    {
        _velocity = Math.Min(_velocity + 8, 42);
        TriggerSpark();
        RaiseSpeedIndicators();
    }

    public void Stop()
    {
        _velocity = 0;
        RaiseSpeedIndicators();
    }

    private void TriggerSpark()
    {
        ShowSpark = true;
        _sparkTimer.Stop();
        _sparkTimer.Start();
    }

    private void OnTick(object? sender, EventArgs e)
    {
        if (Math.Abs(_velocity) <= 0.01)
        {
            _velocity = 0;
            RaiseSpeedIndicators();
            return;
        }

        Angle = (Angle + _velocity) % 360;
        _velocity *= _friction;
        RaiseSpeedIndicators();
    }

    private void RaiseSpeedIndicators()
    {
        OnPropertyChanged(nameof(DisplaySpeed));
        OnPropertyChanged(nameof(SpeedNormalized));
        OnPropertyChanged(nameof(MotionBlurOpacity));
    }

    public void Dispose()
    {
        _timer.Stop();
        _timer.Tick -= OnTick;
        _sparkTimer.Stop();
    }
}
