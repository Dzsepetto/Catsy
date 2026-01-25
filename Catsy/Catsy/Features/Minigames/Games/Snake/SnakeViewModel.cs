using System;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Controls;
using Catsy.Features.Shell;

namespace Catsy.Features.Minigames.Games.Snake
{
    public sealed class SnakeViewModel : BindableObject
    {
        public ICommand BackToMinigameSelect { get; }

        public SnakeViewModel(IServiceProvider sp, MainLayoutViewModel main)
        {
            BackToMinigameSelect = new Command(() =>
            {
                var selectView = sp.GetRequiredService<Catsy.Features.Minigames.MinigameSelectView>();
                main.CurrentView = selectView;
            });
        }
    }
}