using System;
using System.Collections.ObjectModel;
using System.Linq;
using MegaApp.Foundation;

namespace MegaApp.ViewModels;

public sealed class SudokuViewModel : ObservableObject
{
    private const int Size = 4;

    private static readonly (int[] puzzle, int[] solution)[] Boards =
    {
        (
            new[]
            {
                1, 0, 3, 4,
                0, 4, 0, 2,
                2, 0, 4, 0,
                4, 3, 0, 1
            },
            new[]
            {
                1, 2, 3, 4,
                3, 4, 1, 2,
                2, 1, 4, 3,
                4, 3, 2, 1
            }
        ),
        (
            new[]
            {
                2, 0, 4, 0,
                0, 3, 0, 1,
                1, 0, 0, 4,
                0, 4, 1, 0
            },
            new[]
            {
                2, 1, 4, 3,
                4, 3, 2, 1,
                1, 2, 3, 4,
                3, 4, 1, 2
            }
        ),
        (
            new[]
            {
                3, 4, 0, 0,
                0, 0, 3, 4,
                4, 0, 2, 0,
                0, 1, 0, 3
            },
            new[]
            {
                3, 4, 1, 2,
                1, 2, 3, 4,
                4, 3, 2, 1,
                2, 1, 4, 3
            }
        )
    };

    private readonly Random _random = new();
    private readonly RelayCommand _cycleCellCommand;
    private readonly RelayCommand _resetCommand;
    private readonly RelayCommand _restartCommand;
    private int _currentBoardIndex = -1;
    private int[] _currentPuzzle = Array.Empty<int>();
    private int[] _currentSolution = Array.Empty<int>();
    private bool _isSolved;

    public SudokuViewModel()
    {
        Cells = new ObservableCollection<SudokuCellViewModel>();

        _cycleCellCommand = new RelayCommand(param =>
        {
            if (param is not SudokuCellViewModel cell || cell.IsFixed)
            {
                return;
            }

            cell.Value = cell.Value <= 0 ? 1 : (cell.Value % Size) + 1;
            ValidateBoard();
        });

        _resetCommand = new RelayCommand(_ => Reset());
        _restartCommand = new RelayCommand(_ => Restart());

        Restart();
    }

    public ObservableCollection<SudokuCellViewModel> Cells { get; }

    public RelayCommand CycleCellCommand => _cycleCellCommand;

    public RelayCommand ResetCommand => _resetCommand;

    public RelayCommand RestartCommand => _restartCommand;

    public bool IsSolved
    {
        get => _isSolved;
        private set => SetProperty(ref _isSolved, value);
    }

    public string StatusMessage => IsSolved ? "Puzzle complete!" : "Tap any open cell to cycle 1-4.";

    private void Reset()
    {
        for (var i = 0; i < Cells.Count; i++)
        {
            var cell = Cells[i];
            cell.Value = _currentPuzzle[i];
            cell.IsError = false;
        }

        ValidateBoard();
    }

    private void Restart()
    {
        if (Boards.Length == 0)
        {
            return;
        }

        var next = _random.Next(Boards.Length);
        if (Boards.Length > 1)
        {
            while (next == _currentBoardIndex)
            {
                next = _random.Next(Boards.Length);
            }
        }

        _currentBoardIndex = next;
        LoadBoard(Boards[next]);
    }

    private void LoadBoard((int[] puzzle, int[] solution) board)
    {
        _currentPuzzle = board.puzzle.ToArray();
        _currentSolution = board.solution.ToArray();

        Cells.Clear();
        for (var i = 0; i < _currentPuzzle.Length; i++)
        {
            Cells.Add(new SudokuCellViewModel(i, _currentPuzzle[i], _currentPuzzle[i] != 0));
        }

        ValidateBoard();
    }

    private void ValidateBoard()
    {
        if (_currentSolution.Length == 0)
        {
            return;
        }

        foreach (var cell in Cells)
        {
            var solvedValue = _currentSolution[cell.Index];
            cell.IsError = !cell.IsFixed && cell.Value != 0 && cell.Value != solvedValue;
        }

        IsSolved = Cells.All(c => c.Value == _currentSolution[c.Index]);
        OnPropertyChanged(nameof(StatusMessage));
    }
}

public sealed class SudokuCellViewModel : ObservableObject
{
    private int _value;
    private bool _isError;

    public SudokuCellViewModel(int index, int initialValue, bool isFixed)
    {
        Index = index;
        _value = initialValue;
        IsFixed = isFixed;
    }

    public int Index { get; }

    public bool IsFixed { get; }

    public int Value
    {
        get => _value;
        set
        {
            if (SetProperty(ref _value, value))
            {
                OnPropertyChanged(nameof(DisplayValue));
            }
        }
    }

    public bool IsError
    {
        get => _isError;
        set => SetProperty(ref _isError, value);
    }

    public string DisplayValue => Value == 0 ? string.Empty : Value.ToString();
}
