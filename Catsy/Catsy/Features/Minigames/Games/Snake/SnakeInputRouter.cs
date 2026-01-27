#if WINDOWS
using Windows.System;
using Catsy.Features.Minigames.Games.Snake;

namespace Catsy;

public static class SnakeInputRouter
{
    public static SnakeView? CurrentSnakeView { get; set; }

    public static void HandleKey(VirtualKey key)
    {
        if (CurrentSnakeView == null)
            return;

        switch (key)
        {
            case VirtualKey.Left:
            case VirtualKey.A:
                CurrentSnakeView.HandleInput(Direction.Left);
                break;

            case VirtualKey.Right:
            case VirtualKey.D:
                CurrentSnakeView.HandleInput(Direction.Right);
                break;

            case VirtualKey.Up:
            case VirtualKey.W:
                CurrentSnakeView.HandleInput(Direction.Up);
                break;

            case VirtualKey.Down:
            case VirtualKey.S:
                CurrentSnakeView.HandleInput(Direction.Down);
                break;
        }
    }
}
#endif
