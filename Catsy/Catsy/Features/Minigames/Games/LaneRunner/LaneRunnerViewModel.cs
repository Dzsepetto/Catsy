using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using Catsy.Features.Shell;

namespace Catsy.Features.Minigames.Games;

public class LaneRunnerViewModel : BindableObject
{
    public ICommand BackToMinigameSelect { get; }

    public LaneRunnerViewModel(IServiceProvider sp, MainLayoutViewModel main)
    {
        BackToMinigameSelect = new Command(() =>
        {
            var selectView = sp.GetRequiredService<Catsy.Features.Minigames.MinigameSelectView>();
            main.CurrentView = selectView;
        });
    }
}
