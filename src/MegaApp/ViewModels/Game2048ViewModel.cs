using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using MegaApp.Foundation;

namespace MegaApp.ViewModels;

public sealed class Game2048ViewModel : ObservableObject
{
    private const int Size = 4;
    private readonly Random _random = new();
    private readonly RelayCommand _moveUpCommand;
    private readonly RelayCommand _moveDownCommand;
    private readonly RelayCommand _moveLeftCommand;
    private readonly RelayCommand _moveRightCommand;
    private readonly RelayCommand _resetCommand;
    private int _score;
    private bool _hasMoves;

    public Game2048ViewModel()
    {
        Tiles = new ObservableCollection<Game2048TileViewModel>(
            Enumerable.Range(0, Size * Size).Select(_ => new Game2048TileViewModel()));

        _moveUpCommand = new RelayCommand(_ => Move(Direction.Up));
        _moveDownCommand = new RelayCommand(_ => Move(Direction.Down));
        _moveLeftCommand = new RelayCommand(_ => Move(Direction.Left));
        _moveRightCommand = new RelayCommand(_ => Move(Direction.Right));
        _resetCommand = new RelayCommand(_ => Reset());

        Reset();
    }

    public ObservableCollection<Game2048TileViewModel> Tiles { get; }

    public RelayCommand MoveUpCommand => _moveUpCommand;
    public RelayCommand MoveDownCommand => _moveDownCommand;
    public RelayCommand MoveLeftCommand => _moveLeftCommand;
    public RelayCommand MoveRightCommand => _moveRightCommand;
    public RelayCommand ResetCommand => _resetCommand;

    public int Score
    {
        get => _score;
        private set => SetProperty(ref _score, value);
    }

    public bool HasMoves
    {
        get => _hasMoves;
        private set
        {
            if (SetProperty(ref _hasMoves, value))
            {
                OnPropertyChanged(nameof(StatusMessage));
            }
        }
    }

    public string StatusMessage => HasMoves ? "Swipe with the arrows." : "No moves left. Reset to try again.";

    private void Reset()
    {
        Score = 0;
        foreach (var tile in Tiles)
        {
            tile.Value = 0;
        }

        SpawnTile();
        SpawnTile();
        HasMoves = true;
    }

    private void Move(Direction direction)
    {
        if (!HasMoves)
        {
            return;
        }

        var moved = false;

        for (var line = 0; line < Size; line++)
        {
            var indices = GetLineIndices(line, direction).ToList();
            if (CompressLine(indices))
            {
                moved = true;
            }
        }

        if (moved)
        {
            SpawnTile();
        }

        HasMoves = CheckHasMoves();
    }

    private bool CompressLine(IReadOnlyList<int> indices)
    {
        var values = indices.Select(i => Tiles[i].Value).ToList();
        var filtered = values.Where(v => v > 0).ToList();
        var merged = new List<int>();

        for (var i = 0; i < filtered.Count; i++)
        {
            var current = filtered[i];
            if (i + 1 < filtered.Count && filtered[i + 1] == current)
            {
                current *= 2;
                Score += current;
                i++;
            }

            merged.Add(current);
        }

        while (merged.Count < Size)
        {
            merged.Add(0);
        }

        var changed = false;
        for (var pos = 0; pos < indices.Count; pos++)
        {
            var index = indices[pos];
            if (Tiles[index].Value != merged[pos])
            {
                Tiles[index].Value = merged[pos];
                changed = true;
            }
        }

        return changed;
    }

    private IEnumerable<int> GetLineIndices(int line, Direction direction)
    {
        switch (direction)
        {
            case Direction.Left:
                for (var col = 0; col < Size; col++)
                {
                    yield return line * Size + col;
                }
                break;
            case Direction.Right:
                for (var col = Size - 1; col >= 0; col--)
                {
                    yield return line * Size + col;
                }
                break;
            case Direction.Up:
                for (var row = 0; row < Size; row++)
                {
                    yield return row * Size + line;
                }
                break;
            case Direction.Down:
                for (var row = Size - 1; row >= 0; row--)
                {
                    yield return row * Size + line;
                }
                break;
        }
    }

    private void SpawnTile()
    {
        var empty = Tiles
            .Select((tile, index) => (tile, index))
            .Where(t => t.tile.Value == 0)
            .Select(t => t.index)
            .ToList();

        if (empty.Count == 0)
        {
            return;
        }

        var index = empty[_random.Next(empty.Count)];
        Tiles[index].Value = _random.NextDouble() < 0.9 ? 2 : 4;
    }

    private bool CheckHasMoves()
    {
        if (Tiles.Any(t => t.Value == 0))
        {
            return true;
        }

        for (var row = 0; row < Size; row++)
        {
            for (var col = 0; col < Size; col++)
            {
                var current = Tiles[row * Size + col].Value;
                if (row + 1 < Size && Tiles[(row + 1) * Size + col].Value == current)
                {
                    return true;
                }

                if (col + 1 < Size && Tiles[row * Size + (col + 1)].Value == current)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private enum Direction
    {
        Left,
        Right,
        Up,
        Down
    }
}

public sealed class Game2048TileViewModel : ObservableObject
{
    private int _value;

    public int Value
    {
        get => _value;
        set
        {
            if (SetProperty(ref _value, value))
            {
                OnPropertyChanged(nameof(DisplayValue));
                OnPropertyChanged(nameof(HasValue));
            }
        }
    }

    public bool HasValue => Value > 0;

    public string DisplayValue => HasValue ? Value.ToString() : string.Empty;
}
