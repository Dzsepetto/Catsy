using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using Catsy.Features.Shell;

namespace Catsy.Features.Minigames;

public class MinigameSelectViewModel : BindableObject
{
    public ICommand GoLaneRunner { get; }
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

        BackToHome = new Command(() => main.CurrentView = homeView);
    }
}
