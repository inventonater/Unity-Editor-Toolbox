using R3;
using UnityEngine;

public class SwipeMechanic : ReactiveMechanic<SwipeMechanic.Event, IInputFrame>
{
    public enum SwipeDirection
    {
        Up,
        Down,
        Left,
        Right
    }

    public readonly struct Event : IMechanicEvent
    {
        public Event(SwipeMechanic mechanic, SwipeDirection direction)
        {
            Mechanic = mechanic;
            Direction = direction;
        }

        public IMechanic Mechanic { get; }
        public SwipeDirection Direction { get; }
    }

    public float SwipeThreshold = 50f;

    protected override void SetupMechanic(IInputSource<IInputFrame> inputSource)
    {
        var swipeStart = inputSource.InputObservable
            .Where(input => input.IsPressed)
            .Select(input => input.Position);

        var swipeEnd = inputSource.InputObservable
            .Where(input => !input.IsPressed)
            .Select(input => input.Position);

        swipeStart.SelectMany(startPosition => swipeEnd
                .Take(1)
                .Select(endPosition => new { StartPosition = startPosition, EndPosition = endPosition }))
            .Where(swipe => Vector2.Distance(swipe.StartPosition, swipe.EndPosition) > SwipeThreshold)
            .Subscribe(swipe =>
            {
                Vector2 direction = swipe.EndPosition - swipe.StartPosition;
                SwipeDirection swipeDirection = GetSwipeDirection(direction);
                FireEvent(new Event(this, swipeDirection));
            });
    }

    private SwipeDirection GetSwipeDirection(Vector2 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        if (angle < -135 || angle > 135)
            return SwipeDirection.Left;
        else if (angle < -45)
            return SwipeDirection.Down;
        else if (angle < 45)
            return SwipeDirection.Right;
        else
            return SwipeDirection.Up;
    }
}