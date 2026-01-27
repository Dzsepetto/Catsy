using Microsoft.Maui.Controls;

namespace Catsy.Features.Minigames.Games.Snake
{
    public static class Images
    {
        public static readonly ImageSource Empty =
            LoadImage("empty");

        public static readonly ImageSource SnakeBody =
            LoadImage("body");

        public static readonly ImageSource SnakeHead =
            LoadImage("head");
        public static readonly ImageSource Food =
            LoadImage("food");

        public static readonly ImageSource DeadBody =
            LoadImage("deadbody");

        public static readonly ImageSource DeadHead =
            LoadImage("deadhead");

        private static ImageSource LoadImage(string name)
        {
            return ImageSource.FromFile($"{name}.png");
        }
    }
}
