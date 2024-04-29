using R3;
using UnityEngine;

public class PinchMechanic : ReactiveMechanic<PinchMechanic.Event, PinchMechanic.TouchInputFrame>
{
    public struct TouchInputFrame : IInputFrame
    {
        public int TouchCount { get; }
        public Vector2 Position1 { get; }
        public Vector2 Position2 { get; }
        public Vector2 DeltaPosition1 { get; }
        public Vector2 DeltaPosition2 { get; }

        public TouchInputFrame(int touchCount, Vector2 position1, Vector2 position2, Vector2 deltaPosition1, Vector2 deltaPosition2)
        {
            TouchCount = touchCount;
            Position1 = position1;
            Position2 = position2;
            DeltaPosition1 = deltaPosition1;
            DeltaPosition2 = deltaPosition2;
            IsPressed = false;
            Position = default;
        }

        public bool IsPressed { get; }
        public Vector2 Position { get; }
    }

    
    public readonly struct Event : IMechanicEvent
    {
        public Event(PinchMechanic mechanic, float distanceDelta)
        {
            Mechanic = mechanic;
            DistanceDelta = distanceDelta;
        }

        public IMechanic Mechanic { get; }
        public float DistanceDelta { get; }
    }

    protected override void SetupMechanic(IInputSource<TouchInputFrame> inputSource)
    {
        var pinchStart = inputSource.InputObservable
            .Where(input => input.TouchCount == 2 && input.DeltaPosition1.sqrMagnitude > 0 && input.DeltaPosition2.sqrMagnitude > 0)
            .Select(input => new { Input = input, Distance = Vector2.Distance(input.Position1, input.Position2) });

        var pinchEnd = inputSource.InputObservable
            .Where(input => input.TouchCount < 2)
            .Select(input => new { Input = input, Distance = 0f });

        pinchStart.SelectMany(start => pinchEnd
                .Take(1)
                .Select(end => new { Start = start, End = end }))
            .Where(pinch => pinch.Start.Input.TouchCount == 2 && pinch.End.Input.TouchCount < 2)
            .Subscribe(pinch =>
            {
                float distanceDelta = pinch.End.Distance - pinch.Start.Distance;
                FireEvent(new Event(this, distanceDelta));
            });
    }
}