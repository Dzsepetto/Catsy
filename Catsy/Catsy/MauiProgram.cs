using Catsy.Features.Minigames;
using Catsy.Features.Minigames.Games;
using Catsy.Features.Minigames.Games.Snake;
using Catsy.Features.Shell;
using Microsoft.Extensions.Logging;

namespace Catsy
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            builder.Services.AddSingleton<MainLayoutPage>();
            builder.Services.AddSingleton<MainLayoutViewModel>();
            builder.Services.AddTransient<MinigameSelectView>();
            builder.Services.AddTransient<MinigameSelectViewModel>();


            builder.Services.AddSingleton<HomeView>();
            builder.Services.AddSingleton<RedView>();
            builder.Services.AddSingleton<YellowView>();

            builder.Services.AddSingleton<ShopView>();
            builder.Services.AddSingleton<ShopViewModel>();

            builder.Services.AddSingleton<LaneRunnerView>();
            builder.Services.AddSingleton<LaneRunnerViewModel>();

            builder.Services.AddSingleton<SnakeView>();
            builder.Services.AddSingleton<SnakeViewModel>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
