using System;
using System.Windows.Threading;
using MegaApp.Foundation;

namespace MegaApp.ViewModels;

public sealed class PomodoroViewModel : ObservableObject, IDisposable
{
    private readonly DispatcherTimer _timer;
    private TimeSpan _sessionLength = TimeSpan.FromMinutes(25);
    private TimeSpan _breakLength = TimeSpan.FromMinutes(5);
    private TimeSpan _remaining;
    private bool _isRunning;
    private bool _isBreak;

    public PomodoroViewModel()
    {
        _remaining = _sessionLength;
        _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
        _timer.Tick += OnTick;
    }

    public TimeSpan SessionLength
    {
        get => _sessionLength;
        set
        {
            SetProperty(ref _sessionLength, value);
            if (!_isBreak)
            {
                Remaining = value;
            }
        }
    }

    public TimeSpan BreakLength
    {
        get => _breakLength;
        set
        {
            SetProperty(ref _breakLength, value);
            if (_isBreak)
            {
                Remaining = value;
            }
        }
    }

    public TimeSpan Remaining
    {
        get => _remaining;
        private set
        {
            SetProperty(ref _remaining, value);
            OnPropertyChanged(nameof(Progress));
            OnPropertyChanged(nameof(DisplayTime));
        }
    }

    public bool IsRunning
    {
        get => _isRunning;
        private set => SetProperty(ref _isRunning, value);
    }

    public bool IsBreak
    {
        get => _isBreak;
        private set
        {
            if (SetProperty(ref _isBreak, value))
            {
                OnPropertyChanged(nameof(CurrentPhase));
            }
        }
    }

    public string CurrentPhase => IsBreak ? "Break" : "Focus";

    public string DisplayTime => Remaining.ToString(Remaining.TotalHours >= 1 ? @"hh\:mm\:ss" : @"mm\:ss");

    public double Progress
    {
        get
        {
            var total = IsBreak ? BreakLength.TotalSeconds : SessionLength.TotalSeconds;
            if (total <= 0)
            {
                return 0;
            }

            return 1 - Remaining.TotalSeconds / total;
        }
    }

    public void Toggle()
    {
        if (IsRunning)
        {
            Pause();
        }
        else
        {
            Start();
        }
    }

    public void Start()
    {
        IsRunning = true;
        _timer.Start();
    }

    public void Pause()
    {
        IsRunning = false;
        _timer.Stop();
    }

    public void Reset()
    {
        Pause();
        Remaining = IsBreak ? BreakLength : SessionLength;
    }

    private void OnTick(object? sender, EventArgs e) => Tick();

    private void Tick()
    {
        if (Remaining.TotalSeconds <= 1)
        {
            TogglePhase();
            return;
        }

        Remaining -= TimeSpan.FromSeconds(1);
    }

    private void TogglePhase()
    {
        IsBreak = !IsBreak;
        Remaining = IsBreak ? BreakLength : SessionLength;
    }

    public void Dispose()
    {
        _timer.Stop();
        _timer.Tick -= OnTick;
    }
}
