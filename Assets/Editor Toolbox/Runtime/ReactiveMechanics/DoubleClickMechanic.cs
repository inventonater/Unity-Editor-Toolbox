using R3;
using UnityEngine;

public class DoubleClickMechanic : ReactiveMechanic<DoubleClickMechanic.Event, IInputFrame>
{
    public readonly struct Event : IMechanicEvent
    {
        public Event(DoubleClickMechanic mechanic, Vector2 doubleClickPosition)
        {
            Mechanic = mechanic;
            DoubleClickPosition = doubleClickPosition;
        }

        public IMechanic Mechanic { get; }
        public Vector2 DoubleClickPosition { get; }
    }

    public float DoubleClickThreshold = 0.3f;

    protected override void SetupMechanic(IInputSource<IInputFrame> inputSource)
    {
        inputSource.InputObservable
            .Chunk(2, 1)
            .Where(buffer => buffer[0].IsPressed && !buffer[1].IsPressed)
            .TimeInterval()
            .Chunk(2, 1)
            .Where(buffer => buffer[1].Interval.TotalSeconds < DoubleClickThreshold)
            .Subscribe(buffer => FireEvent(new Event(this, buffer[1].Value[1].Position)));
    }
}