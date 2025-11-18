using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using MegaApp.Foundation;

namespace MegaApp.ViewModels;

public sealed class MinesweeperViewModel : ObservableObject
{
    private const int Rows = 6;
    private const int Columns = 6;
    private const int MineCount = 6;

    private readonly Random _random = new();
    private readonly RelayCommand _revealCommand;
    private readonly RelayCommand _toggleFlagCommand;
    private readonly RelayCommand _resetCommand;
    private readonly RelayCommand _restartCommand;
    private bool _isGameOver;
    private string _statusMessage = string.Empty;
    private bool[] _mineLayout = Array.Empty<bool>();

    public MinesweeperViewModel()
    {
        Cells = new ObservableCollection<MineCellViewModel>();
        _revealCommand = new RelayCommand(param =>
        {
            if (param is MineCellViewModel cell)
            {
                Reveal(cell);
            }
        });
        _toggleFlagCommand = new RelayCommand(param =>
        {
            if (param is MineCellViewModel cell)
            {
                ToggleFlag(cell);
            }
        });
        _resetCommand = new RelayCommand(_ => ResetCurrent());
        _restartCommand = new RelayCommand(_ => Restart());

        Restart();
    }

    public ObservableCollection<MineCellViewModel> Cells { get; }

    public RelayCommand RevealCommand => _revealCommand;
    public RelayCommand ToggleFlagCommand => _toggleFlagCommand;
    public RelayCommand ResetCommand => _resetCommand;

    public RelayCommand RestartCommand => _restartCommand;

    public bool IsGameOver
    {
        get => _isGameOver;
        private set => SetProperty(ref _isGameOver, value);
    }

    public string StatusMessage
    {
        get => _statusMessage;
        private set => SetProperty(ref _statusMessage, value);
    }

    public int FlagsPlaced => Cells.Count(c => c.IsFlagged);
    public int FlagsRemaining => Math.Max(0, MineCount - FlagsPlaced);

    private void Restart()
    {
        _mineLayout = GenerateMineLayout();

        if (Cells.Count == 0)
        {
            for (var index = 0; index < Rows * Columns; index++)
            {
                var row = index / Columns;
                var col = index % Columns;
                Cells.Add(new MineCellViewModel(row, col));
            }
        }

        for (var index = 0; index < Cells.Count; index++)
        {
            var cell = Cells[index];
            cell.IsMine = _mineLayout[index];
            cell.IsRevealed = false;
            cell.IsFlagged = false;
            cell.IsExploded = false;
        }

        foreach (var cell in Cells)
        {
            cell.AdjacentMines = GetNeighbors(cell).Count(n => n.IsMine);
        }

        IsGameOver = false;
        StatusMessage = "Reveal safe tiles. Right click to flag.";
        OnPropertyChanged(nameof(FlagsRemaining));
    }

    private void ResetCurrent()
    {
        foreach (var cell in Cells)
        {
            cell.IsRevealed = false;
            cell.IsFlagged = false;
            cell.IsExploded = false;
        }

        IsGameOver = false;
        StatusMessage = "Reveal safe tiles. Right click to flag.";
        OnPropertyChanged(nameof(FlagsRemaining));
    }

    private void Reveal(MineCellViewModel cell)
    {
        if (IsGameOver || cell.IsFlagged || cell.IsRevealed)
        {
            return;
        }

        cell.IsRevealed = true;

        if (cell.IsMine)
        {
            cell.IsExploded = true;
            foreach (var mine in Cells.Where(c => c.IsMine))
            {
                mine.IsRevealed = true;
            }

            IsGameOver = true;
            StatusMessage = "Boom! Use Reset to retry or Restart for a new grid.";
            return;
        }

        if (cell.AdjacentMines == 0)
        {
            foreach (var neighbor in GetNeighbors(cell))
            {
                Reveal(neighbor);
            }
        }

        if (Cells.Where(c => !c.IsMine).All(c => c.IsRevealed))
        {
            IsGameOver = true;
            StatusMessage = "You cleared the field!";
        }
        else
        {
            StatusMessage = "Keep sweeping...";
        }
    }

    private void ToggleFlag(MineCellViewModel cell)
    {
        if (IsGameOver || cell.IsRevealed)
        {
            return;
        }

        cell.IsFlagged = !cell.IsFlagged;
        OnPropertyChanged(nameof(FlagsRemaining));
    }

    private IEnumerable<MineCellViewModel> GetNeighbors(MineCellViewModel cell)
    {
        for (var row = cell.Row - 1; row <= cell.Row + 1; row++)
        {
            for (var col = cell.Column - 1; col <= cell.Column + 1; col++)
            {
                if (row == cell.Row && col == cell.Column)
                {
                    continue;
                }

                if (row >= 0 && row < Rows && col >= 0 && col < Columns)
                {
                    yield return Cells[row * Columns + col];
                }
            }
        }
    }

    private bool[] GenerateMineLayout()
    {
        var layout = new bool[Rows * Columns];
        var mineIndices = new HashSet<int>();

        while (mineIndices.Count < MineCount)
        {
            mineIndices.Add(_random.Next(Rows * Columns));
        }

        foreach (var index in mineIndices)
        {
            layout[index] = true;
        }

        return layout;
    }
}

public sealed class MineCellViewModel : ObservableObject
{
    private bool _isRevealed;
    private bool _isFlagged;
    private bool _isExploded;
    private int _adjacentMines;

    public MineCellViewModel(int row, int column)
    {
        Row = row;
        Column = column;
    }

    public int Row { get; }
    public int Column { get; }

    public bool IsMine { get; set; }

    public bool IsRevealed
    {
        get => _isRevealed;
        set
        {
            if (SetProperty(ref _isRevealed, value))
            {
                OnPropertyChanged(nameof(DisplayValue));
                OnPropertyChanged(nameof(ShowNumber));
            }
        }
    }

    public bool IsFlagged
    {
        get => _isFlagged;
        set
        {
            if (SetProperty(ref _isFlagged, value))
            {
                OnPropertyChanged(nameof(ShowFlag));
            }
        }
    }

    public bool IsExploded
    {
        get => _isExploded;
        set => SetProperty(ref _isExploded, value);
    }

    public int AdjacentMines
    {
        get => _adjacentMines;
        set
        {
            if (SetProperty(ref _adjacentMines, value))
            {
                OnPropertyChanged(nameof(DisplayValue));
            }
        }
    }

    public bool ShowFlag => IsFlagged && !IsRevealed;

    public bool ShowNumber => IsRevealed && !IsMine && AdjacentMines > 0;

    public string DisplayValue
    {
        get
        {
            if (!IsRevealed)
            {
                return string.Empty;
            }

            if (IsMine)
            {
                return "*";
            }

            return AdjacentMines > 0 ? AdjacentMines.ToString() : string.Empty;
        }
    }
}
