namespace Catsy.Features.Minigames.Games.Snake;

public sealed class InputState
{
    public Direction? RequestedDirection { get; private set; }

    public void Press(Direction direction)
    {
        RequestedDirection = direction;
    }

    public void Clear()
    {
        RequestedDirection = null;
    }
}
