using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using Catsy.Features.Shell;

namespace Catsy.Features.Minigames;

public class MinigameSelectViewModel : BindableObject
{
    public ICommand GoLaneRunner { get; }
    public ICommand GoSnake { get; }
    public ICommand BackToHome { get; }

    public MinigameSelectViewModel(
        IServiceProvider sp,
        MainLayoutViewModel main,
        HomeView homeView)
    {
        GoLaneRunner = new Command(() =>
        {
            var laneRunnerView = sp.GetRequiredService<Catsy.Features.Minigames.Games.LaneRunnerView>();
            main.CurrentView = laneRunnerView;
        });

        GoSnake = new Command(() =>
        {
            var snakeView = sp.GetRequiredService<Catsy.Features.Minigames.Games.Snake.SnakeView>();
            main.CurrentView = snakeView;
        });

        BackToHome = new Command(() => main.CurrentView = homeView);
    }
}
