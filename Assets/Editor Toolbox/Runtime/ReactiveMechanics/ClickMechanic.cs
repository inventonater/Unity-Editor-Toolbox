using R3;
using UnityEngine;


public class ClickMechanic : ReactiveMechanic<ClickMechanic.Event, IInputFrame>
{
    public readonly struct Event : IMechanicEvent
    {
        public Event(ClickMechanic mechanic, Vector2 clickPosition)
        {
            Mechanic = mechanic;
            ClickPosition = clickPosition;
        }

        public IMechanic Mechanic { get; }
        public Vector2 ClickPosition { get; }
    }

    protected override void SetupMechanic(IInputSource<IInputFrame> inputSource)
    {
        inputSource.InputObservable
            .Chunk(2, 1)
            .Where(buffer => buffer[0].IsPressed && !buffer[1].IsPressed)
            .Subscribe(buffer => FireEvent(new Event(this, buffer[1].Position)));
    }
}