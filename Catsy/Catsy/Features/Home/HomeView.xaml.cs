using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using System;
using System.Diagnostics;

namespace Catsy;

public partial class HomeView : ContentView
{
    public HomeView()
    {
        InitializeComponent();

        // egyszerû VM példa; ha DI-t használsz, cseréld le itt
        BindingContext = new HomeViewModel();

        // figyeljük a Carousel pozícióváltozását, hogy frissítsük a pontok kinézetét
        Carousel.PositionChanged += Carousel_PositionChanged;

        // Ha a Scenes betöltése aszinkron történik, érdemes a kollekció változását is figyelni.
        // Itt egyszerû példa: inicializáljuk a pontok kinézetét késleltetve (UI készenlét után).
        this.Loaded += (_, __) => UpdateIndicators(Carousel.Position);
    }

    private void Carousel_PositionChanged(object? sender, PositionChangedEventArgs e)
    {
        UpdateIndicators(e.CurrentPosition);
    }

    private void UpdateIndicators(int selectedIndex)
    {
        try
        {
            for (int i = 0; i < IndicatorLayout.Children.Count; i++)
            {
                var child = IndicatorLayout.Children[i];
                if (child is Button btn)
                {
                    // egyszerû megjelenés: aktív fehér, inaktív halvány szürke
                    btn.BackgroundColor = (i == selectedIndex) ? Colors.White : Colors.LightGray;
                    btn.Scale = (i == selectedIndex) ? 1.25 : 1.0;
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[HomeView] UpdateIndicators error: {ex}");
        }
    }

    private void OnIndicatorClicked(object? sender, EventArgs e)
    {
        if (sender is not Button btn) return;

        var scene = btn.BindingContext as SceneModel;
        var vm = BindingContext as HomeViewModel;
        if (scene == null || vm == null) return;

        int idx = vm.Scenes.IndexOf(scene);
        if (idx < 0) return;

        // UI thread-en navigálunk a Carousel-hoz
        Dispatcher.Dispatch(() =>
        {
            Carousel.ScrollTo(idx, position: ScrollToPosition.Center, animate: true);
            vm.SelectedIndex = idx;
            UpdateIndicators(idx);
        });
    }

    private void OnPrevClicked(object? sender, EventArgs e)
    {
        var vm = BindingContext as HomeViewModel;
        if (vm == null) return;
        var count = vm.Scenes?.Count ?? 0;
        if (count == 0) return;
        int newIndex = Math.Max(0, Carousel.Position - 1);
        Dispatcher.Dispatch(() =>
        {
            Carousel.ScrollTo(newIndex, position: ScrollToPosition.Center, animate: true);
            vm.SelectedIndex = newIndex;
            UpdateIndicators(newIndex);
        });
    }

    private void OnNextClicked(object? sender, EventArgs e)
    {
        var vm = BindingContext as HomeViewModel;
        if (vm == null) return;
        var count = vm.Scenes?.Count ?? 0;
        if (count == 0) return;
        int newIndex = Math.Min(count - 1, Carousel.Position + 1);
        Dispatcher.Dispatch(() =>
        {
            Carousel.ScrollTo(newIndex, position: ScrollToPosition.Center, animate: true);
            vm.SelectedIndex = newIndex;
            UpdateIndicators(newIndex);
        });
    }
}