using System;
using MegaApp.Foundation;

namespace MegaApp.ViewModels;

public sealed class IdleGardenViewModel : ObservableObject
{
    private readonly RelayCommand _tapCommand;
    private double _progress;
    private int _level = 1;
    private int _energy;

    public IdleGardenViewModel()
    {
        _tapCommand = new RelayCommand(() => AddEnergy(3));
    }

    public RelayCommand TapCommand => _tapCommand;

    public double Progress
    {
        get => _progress;
        private set => SetProperty(ref _progress, value);
    }

    public int Level
    {
        get => _level;
        private set => SetProperty(ref _level, value);
    }

    public int Energy
    {
        get => _energy;
        private set => SetProperty(ref _energy, value);
    }

    private void AddEnergy(int amount)
    {
        Energy += amount;
        var required = RequiredEnergy(Level);
        Progress = Math.Min(1, (double)Energy / required);

        if (Energy >= required)
        {
            Level++;
            Energy = 0;
            Progress = 0;
        }
    }

    private static int RequiredEnergy(int level) => 25 + level * 10;
}
