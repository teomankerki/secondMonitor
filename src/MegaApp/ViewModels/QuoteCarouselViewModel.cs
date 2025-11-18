using System;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using MegaApp.Foundation;

namespace MegaApp.ViewModels;

public sealed class QuoteCarouselViewModel : ObservableObject
{
    private readonly DispatcherTimer _timer;
    private readonly Random _random = new();
    private string _currentQuote = string.Empty;

    public QuoteCarouselViewModel()
    {
        Quotes = new ObservableCollection<string>
        {
            "Breathe in calm, breathe out noise.",
            "Progress happens a single mindful click at a time.",
            "Your second screen can be a sanctuary.",
            "Curiosity is fuel — let it drift gently.",
            "Small rituals create steady focus.",
            "Whitespace is a feature, not a bug.",
        };

        _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(12) };
        _timer.Tick += (_, _) => Rotate();
        Rotate();
        _timer.Start();
    }

    public ObservableCollection<string> Quotes { get; }

    public string CurrentQuote
    {
        get => _currentQuote;
        private set => SetProperty(ref _currentQuote, value);
    }

    private void Rotate()
    {
        if (Quotes.Count == 0)
        {
            CurrentQuote = "Add your favorite quotes";
            return;
        }

        var index = _random.Next(Quotes.Count);
        CurrentQuote = Quotes[index];
    }
}
