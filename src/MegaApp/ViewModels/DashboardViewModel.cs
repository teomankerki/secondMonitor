using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using MegaApp.Foundation;
using MegaApp.Modules;

namespace MegaApp.ViewModels;

public sealed class DashboardViewModel : ObservableObject
{
    private readonly Dictionary<string, WidgetChoice> _widgetMap;
    private readonly CategoryTemplate[] _categoryTemplates =
    {
        new("productivity", "Productivity"),
        new("games", "Games"),
        new("vibes", "Vibes")
    };
    private string? _selectedCategoryKey;

    public DashboardViewModel()
    {
        var choices = new[]
        {
            new WidgetChoice("pomodoro", "Pomodoro Timer", () => new PomodoroModule(), 320, 240),
            new WidgetChoice("systemPulse", "System Pulse", () => new SystemPulseModule(), 320, 200),
            new WidgetChoice("notes", "Quick Notes", () => new NotesModule(), 380, 320),
            new WidgetChoice("idleGarden", "Idle Garden", () => new IdleGardenModule(), 320, 220),
            new WidgetChoice("miniSudoku", "Mini Sudoku", () => new SudokuModule(), 340, 320),
            new WidgetChoice("game2048", "2048", () => new Game2048Module(), 380, 360),
            new WidgetChoice("minesweeper", "Minesweeper", () => new MinesweeperModule(), 380, 360),
            new WidgetChoice("spinner", "Fidget Spinner", () => new FidgetSpinnerModule(), 320, 320),
            new WidgetChoice("runner", "Endless Runner", () => new EndlessRunnerModule(), 360, 280),
            new WidgetChoice("quotes", "Affirmation Carousel", () => new QuoteModule(), 320, 180),
            new WidgetChoice("breathing", "Calm Breathing", () => new CalmBreathingModule(), 320, 260),
            new WidgetChoice("particles", "Ambient Drift", () => new ParticleFlowModule(), 320, 260)
        };

        _widgetMap = choices.ToDictionary(c => c.Key, c => c);
        AvailableWidgets = new ObservableCollection<WidgetChoice>(choices);
        Categories = new ObservableCollection<CategoryViewModel>();
        Categories.CollectionChanged += (_, __) =>
        {
            OnPropertyChanged(nameof(AvailableCategoryTemplates));
        };

        AddCategoryCommand = new RelayCommand(_ => AddCategory());
        RemoveCategoryCommand = new RelayCommand(category => RemoveCategory(category as CategoryViewModel));
        MoveCategoryUpCommand = new RelayCommand(category => MoveCategory(category as CategoryViewModel, -1));
        MoveCategoryDownCommand = new RelayCommand(category => MoveCategory(category as CategoryViewModel, 1));
    }

    public ObservableCollection<CategoryViewModel> Categories { get; }

    public ObservableCollection<WidgetChoice> AvailableWidgets { get; }

    public IEnumerable<CategoryTemplate> AvailableCategoryTemplates =>
        _categoryTemplates.Where(template => Categories.All(c => c.Key != template.Key)).ToArray();

    public string? SelectedCategoryKey
    {
        get => _selectedCategoryKey;
        set => SetProperty(ref _selectedCategoryKey, value);
    }

    public RelayCommand AddCategoryCommand { get; }

    public RelayCommand RemoveCategoryCommand { get; }

    public RelayCommand MoveCategoryUpCommand { get; }

    public RelayCommand MoveCategoryDownCommand { get; }

    internal WidgetViewModel? CreateWidget(string key)
    {
        if (!_widgetMap.TryGetValue(key, out var choice))
        {
            return null;
        }

        return new WidgetViewModel(choice);
    }

    private void AddCategory()
    {
        if (string.IsNullOrWhiteSpace(SelectedCategoryKey))
        {
            return;
        }

        var template = _categoryTemplates.FirstOrDefault(t => t.Key == SelectedCategoryKey);
        if (template is null || Categories.Any(c => c.Key == template.Key))
        {
            return;
        }

        Categories.Add(new CategoryViewModel(this, template));
        SelectedCategoryKey = null;
        OnPropertyChanged(nameof(AvailableCategoryTemplates));
    }

    private void RemoveCategory(CategoryViewModel? category)
    {
        if (category is null)
        {
            return;
        }

        if (Categories.Remove(category))
        {
            OnPropertyChanged(nameof(AvailableCategoryTemplates));
        }
    }

    private void MoveCategory(CategoryViewModel? category, int delta)
    {
        if (category is null)
        {
            return;
        }

        var index = Categories.IndexOf(category);
        var newIndex = index + delta;
        if (index < 0 || newIndex < 0 || newIndex >= Categories.Count)
        {
            return;
        }

        Categories.Move(index, newIndex);
    }
}

public sealed class CategoryViewModel : ObservableObject
{
    private readonly DashboardViewModel _owner;
    private readonly CategoryTemplate _template;
    private string? _selectedWidgetKey;

    public CategoryViewModel(DashboardViewModel owner, CategoryTemplate template)
    {
        _owner = owner;
        _template = template;
        Widgets = new ObservableCollection<WidgetViewModel>();

        AddWidgetCommand = new RelayCommand(_ => AddWidget());
        RemoveWidgetCommand = new RelayCommand(widget => RemoveWidgetInternal(widget as WidgetViewModel));
        MoveWidgetUpCommand = new RelayCommand(widget => MoveWidget(widget as WidgetViewModel, -1));
        MoveWidgetDownCommand = new RelayCommand(widget => MoveWidget(widget as WidgetViewModel, 1));
    }

    public string Key => _template.Key;

    public string Name => _template.DisplayName;

    public ObservableCollection<WidgetViewModel> Widgets { get; }

    public string? SelectedWidgetKey
    {
        get => _selectedWidgetKey;
        set => SetProperty(ref _selectedWidgetKey, value);
    }

    public RelayCommand AddWidgetCommand { get; }

    public RelayCommand RemoveWidgetCommand { get; }

    public RelayCommand MoveWidgetUpCommand { get; }

    public RelayCommand MoveWidgetDownCommand { get; }

    private void AddWidget()
    {
        if (string.IsNullOrWhiteSpace(SelectedWidgetKey))
        {
            return;
        }

        var widget = _owner.CreateWidget(SelectedWidgetKey);
        if (widget is null)
        {
            return;
        }

        InsertWidget(Widgets.Count, widget);
    }

    internal void InsertWidget(int index, WidgetViewModel widget)
    {
        widget.Owner = this;
        if (index < 0 || index > Widgets.Count)
        {
            index = Widgets.Count;
        }

        Widgets.Insert(index, widget);
    }

    internal bool RemoveWidgetInternal(WidgetViewModel? widget)
    {
        if (widget is null)
        {
            return false;
        }

        var removed = Widgets.Remove(widget);
        if (removed)
        {
            widget.Owner = null;
        }

        return removed;
    }

    private void MoveWidget(WidgetViewModel? widget, int delta)
    {
        if (widget is null)
        {
            return;
        }

        var index = Widgets.IndexOf(widget);
        var newIndex = index + delta;
        if (index < 0 || newIndex < 0 || newIndex >= Widgets.Count)
        {
            return;
        }

        Widgets.Move(index, newIndex);
    }
}

public sealed class WidgetViewModel : ObservableObject
{
    public WidgetViewModel(WidgetChoice choice)
    {
        Choice = choice;
        Control = choice.Factory();
    }

    public WidgetChoice Choice { get; }

    public string DisplayName => Choice.DisplayName;

    public UserControl Control { get; }

    public double Width => Choice.Width;

    public double Height => Choice.Height;

    public CategoryViewModel? Owner { get; internal set; }
}

public sealed class WidgetChoice
{
    public WidgetChoice(string key, string displayName, Func<UserControl> factory, double width, double height)
    {
        Key = key;
        DisplayName = displayName;
        Factory = factory;
        Width = width;
        Height = height;
    }

    public string Key { get; }

    public string DisplayName { get; }

    public Func<UserControl> Factory { get; }

    public double Width { get; }

    public double Height { get; }
}

public sealed class CategoryTemplate
{
    public CategoryTemplate(string key, string displayName)
    {
        Key = key;
        DisplayName = displayName;
    }

    public string Key { get; }

    public string DisplayName { get; }
}
