using System.Windows.Controls;
using MegaApp.ViewModels;

namespace MegaApp.Modules;

public partial class QuoteModule : UserControl
{
    public QuoteModule()
    {
        InitializeComponent();
        DataContext = new QuoteCarouselViewModel();
    }
}
