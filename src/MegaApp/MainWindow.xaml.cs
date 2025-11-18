using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MegaApp.ViewModels;

namespace MegaApp;

public partial class MainWindow : Window
{
    private const string CategoryFormat = "DashboardCategory";
    private const string WidgetFormat = "DashboardWidget";

    private Point? _categoryDragStart;
    private Point? _widgetDragStart;

    public MainWindow()
    {
        InitializeComponent();
    }

    private DashboardViewModel? Dashboard => DataContext as DashboardViewModel;

    private void OnClose(object sender, RoutedEventArgs e) => Close();

    private void OnMinimize(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;

    private void OnToggleMaximize(object sender, RoutedEventArgs e) => ToggleWindowState();

    private void OnDragWindow(object sender, MouseButtonEventArgs e)
    {
        if (e.Source is Button)
        {
            return;
        }

        if (e.ChangedButton != MouseButton.Left)
        {
            return;
        }

        if (e.ClickCount == 2)
        {
            ToggleWindowState();
            return;
        }

        DragMove();
    }

    private void ToggleWindowState()
    {
        WindowState = WindowState == WindowState.Maximized
            ? WindowState.Normal
            : WindowState.Maximized;
    }

    private void OnCategoryMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed)
        {
            _categoryDragStart = e.GetPosition(this);
        }
    }

    private void OnCategoryMouseMove(object sender, MouseEventArgs e)
    {
        if (_categoryDragStart is null || e.LeftButton != MouseButtonState.Pressed)
        {
            return;
        }

        if (!HasDragThresholdBeenMet(_categoryDragStart.Value, e.GetPosition(this)))
        {
            return;
        }

        _categoryDragStart = null;
        if (sender is not FrameworkElement element || element.DataContext is not CategoryViewModel category)
        {
            return;
        }

        DragDrop.DoDragDrop(element, new DataObject(CategoryFormat, category), DragDropEffects.Move);
    }

    private void OnCategoryDrop(object sender, DragEventArgs e)
    {
        if (Dashboard is null || !e.Data.GetDataPresent(CategoryFormat))
        {
            return;
        }

        if (sender is not FrameworkElement element || element.DataContext is not CategoryViewModel target)
        {
            return;
        }

        var source = (CategoryViewModel)e.Data.GetData(CategoryFormat)!;
        if (source == target)
        {
            return;
        }

        var sourceIndex = Dashboard.Categories.IndexOf(source);
        var targetIndex = Dashboard.Categories.IndexOf(target);
        if (sourceIndex == -1 || targetIndex == -1)
        {
            return;
        }

        Dashboard.Categories.Move(sourceIndex, targetIndex);
    }

    private void OnCategoryPanelDrop(object sender, DragEventArgs e)
    {
        if (Dashboard is null || !e.Data.GetDataPresent(CategoryFormat))
        {
            return;
        }

        var source = (CategoryViewModel)e.Data.GetData(CategoryFormat)!;
        var currentIndex = Dashboard.Categories.IndexOf(source);
        if (currentIndex == -1)
        {
            return;
        }

        Dashboard.Categories.Move(currentIndex, Dashboard.Categories.Count - 1);
    }

    private void OnWidgetMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed)
        {
            _widgetDragStart = e.GetPosition(this);
        }
    }

    private void OnWidgetMouseMove(object sender, MouseEventArgs e)
    {
        if (_widgetDragStart is null || e.LeftButton != MouseButtonState.Pressed)
        {
            return;
        }

        if (!HasDragThresholdBeenMet(_widgetDragStart.Value, e.GetPosition(this)))
        {
            return;
        }

        _widgetDragStart = null;
        if (sender is not FrameworkElement element || element.DataContext is not WidgetViewModel widget)
        {
            return;
        }

        if (widget.Owner is null)
        {
            return;
        }

        var payload = new WidgetDragData(widget.Owner, widget);
        DragDrop.DoDragDrop(element, new DataObject(WidgetFormat, payload), DragDropEffects.Move);
    }

    private void OnWidgetDrop(object sender, DragEventArgs e)
    {
        if (!TryGetWidgetPayload(e, out var payload))
        {
            return;
        }

        if (sender is not FrameworkElement element || element.DataContext is not WidgetViewModel target || target.Owner is null)
        {
            return;
        }

        MoveWidget(payload, target.Owner, target.Owner.Widgets.IndexOf(target));
    }

    private void OnWidgetPanelDrop(object sender, DragEventArgs e)
    {
        if (!TryGetWidgetPayload(e, out var payload))
        {
            return;
        }

        if (sender is not FrameworkElement element || element.DataContext is not CategoryViewModel category)
        {
            return;
        }

        MoveWidget(payload, category, category.Widgets.Count);
    }

    private static void MoveWidget(WidgetDragData payload, CategoryViewModel targetCategory, int insertIndex)
    {
        var sourceCategory = payload.Category;
        if (sourceCategory == targetCategory)
        {
            var currentIndex = sourceCategory.Widgets.IndexOf(payload.Widget);
            if (currentIndex == -1)
            {
                return;
            }

            if (insertIndex > currentIndex)
            {
                insertIndex--;
            }

            if (insertIndex == currentIndex)
            {
                return;
            }

            sourceCategory.Widgets.Move(currentIndex, insertIndex);
            return;
        }

        if (!sourceCategory.RemoveWidgetInternal(payload.Widget))
        {
            return;
        }

        targetCategory.InsertWidget(insertIndex, payload.Widget);
    }

    private void OnCategoryDragOver(object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent(CategoryFormat))
        {
            e.Effects = DragDropEffects.Move;
            e.Handled = true;
        }
    }

    private void OnWidgetDragOver(object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent(WidgetFormat))
        {
            e.Effects = DragDropEffects.Move;
            e.Handled = true;
        }
    }

    private static bool TryGetWidgetPayload(DragEventArgs e, out WidgetDragData payload)
    {
        if (e.Data.GetDataPresent(WidgetFormat) && e.Data.GetData(WidgetFormat) is WidgetDragData data)
        {
            payload = data;
            return true;
        }

        payload = null!;
        return false;
    }

    private static bool HasDragThresholdBeenMet(Point start, Point current)
    {
        return Math.Abs(current.X - start.X) > SystemParameters.MinimumHorizontalDragDistance ||
               Math.Abs(current.Y - start.Y) > SystemParameters.MinimumVerticalDragDistance;
    }

    private sealed class WidgetDragData
    {
        public WidgetDragData(CategoryViewModel category, WidgetViewModel widget)
        {
            Category = category;
            Widget = widget;
        }

        public CategoryViewModel Category { get; }

        public WidgetViewModel Widget { get; }
    }
}
