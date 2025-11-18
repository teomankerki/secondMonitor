using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Threading;
using MegaApp.Foundation;

namespace MegaApp.ViewModels;

public sealed class EndlessRunnerViewModel : ObservableObject, IDisposable
{
    private const int TrackLength = 14;
    private readonly DispatcherTimer _timer;
    private readonly Random _random = new();
    private readonly List<int> _obstacles = new();
    private int _jumpTicks;
    private bool _isGameOver;
    private int _score;
    private string _statusMessage = "Tap jump to begin!";

    public EndlessRunnerViewModel()
    {
        Tiles = new ObservableCollection<RunnerTile>(Enumerable.Range(0, TrackLength).Select(_ => new RunnerTile()));
        JumpCommand = new RelayCommand(_ => Jump());
        ResetCommand = new RelayCommand(_ => Reset());
        _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(200) };
        _timer.Tick += OnTick;
        _timer.Start();
        Reset();
    }

    public ObservableCollection<RunnerTile> Tiles { get; }

    public RelayCommand JumpCommand { get; }

    public RelayCommand ResetCommand { get; }

    public int Score
    {
        get => _score;
        private set => SetProperty(ref _score, value);
    }

    public string StatusMessage
    {
        get => _statusMessage;
        private set => SetProperty(ref _statusMessage, value);
    }

    private void Jump()
    {
        if (_isGameOver)
        {
            Reset();
            return;
        }

        _jumpTicks = 3;
        UpdateTiles();
    }

    private void Reset()
    {
        _obstacles.Clear();
        _jumpTicks = 0;
        _isGameOver = false;
        Score = 0;
        StatusMessage = "Dodge the obstacles!";
        UpdateTiles();
    }

    private void OnTick(object? sender, EventArgs e)
    {
        if (_isGameOver)
        {
            return;
        }

        AdvanceObstacles();
        if (_jumpTicks > 0)
        {
            _jumpTicks--;
        }

        Score++;
        UpdateTiles();
    }

    private void AdvanceObstacles()
    {
        for (var i = 0; i < _obstacles.Count; i++)
        {
            _obstacles[i]--;
        }

        _obstacles.RemoveAll(x => x < 0);

        if (_random.NextDouble() < 0.3)
        {
            _obstacles.Add(TrackLength - 1);
        }

        foreach (var obstaclePosition in _obstacles.ToList())
        {
            if (obstaclePosition == PlayerIndex && _jumpTicks == 0)
            {
                GameOver();
                break;
            }
        }
    }

    private void GameOver()
    {
        _isGameOver = true;
        StatusMessage = "Ouch! Tap jump to play again.";
    }

    private void UpdateTiles()
    {
        for (var i = 0; i < Tiles.Count; i++)
        {
            var tile = Tiles[i];
            tile.HasPlayer = i == PlayerIndex;
            tile.IsJumping = tile.HasPlayer && _jumpTicks > 0;
            tile.HasObstacle = _obstacles.Contains(i);
        }
    }

    private static int PlayerIndex => 2;

    public void Dispose()
    {
        _timer.Stop();
        _timer.Tick -= OnTick;
    }
}

public sealed class RunnerTile : ObservableObject
{
    private bool _hasPlayer;
    private bool _hasObstacle;
    private bool _isJumping;

    public bool HasPlayer
    {
        get => _hasPlayer;
        set
        {
            if (SetProperty(ref _hasPlayer, value))
            {
                OnPropertyChanged(nameof(DisplayGlyph));
            }
        }
    }

    public bool HasObstacle
    {
        get => _hasObstacle;
        set
        {
            if (SetProperty(ref _hasObstacle, value))
            {
                OnPropertyChanged(nameof(DisplayGlyph));
            }
        }
    }

    public bool IsJumping
    {
        get => _isJumping;
        set
        {
            if (SetProperty(ref _isJumping, value))
            {
                OnPropertyChanged(nameof(DisplayGlyph));
            }
        }
    }

    public string DisplayGlyph
    {
        get
        {
            if (HasPlayer)
            {
                return IsJumping ? "↥" : "▲";
            }

            if (HasObstacle)
            {
                return "■";
            }

            return string.Empty;
        }
    }
}
