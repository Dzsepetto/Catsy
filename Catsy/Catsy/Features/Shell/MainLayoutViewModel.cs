using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;

namespace Catsy.Features.Shell;

public class MainLayoutViewModel : BindableObject
{
    private readonly IServiceProvider _sp;
    private View _currentView;

    public View CurrentView
    {
        get => _currentView;
        set { _currentView = value; OnPropertyChanged(); }
    }

    public ICommand GoHomeCommand { get; }
    public ICommand GoRedCommand { get; }
    public ICommand GoShopCommand { get; }
    public ICommand GoYellowCommand { get; }
    public ICommand GoMinigameSelector { get; }

    public MainLayoutViewModel(
        IServiceProvider sp,
        HomeView homeView,
        RedView redView,
        ShopView shopView, YellowView yellowview )
    {
        _sp = sp ?? throw new ArgumentNullException(nameof(sp));
        _currentView = homeView ?? throw new ArgumentNullException(nameof(homeView));

        GoHomeCommand = new Command(() => CurrentView = homeView);
        GoRedCommand = new Command(() => CurrentView = redView);
        GoShopCommand = new Command(() => CurrentView = shopView);
        GoYellowCommand = new Command(() => CurrentView = yellowview);

        GoMinigameSelector = new Command(() =>
        {
            var view = _sp.GetRequiredService<Catsy.Features.Minigames.MinigameSelectView>();
            CurrentView = view;
        });
    }
}
