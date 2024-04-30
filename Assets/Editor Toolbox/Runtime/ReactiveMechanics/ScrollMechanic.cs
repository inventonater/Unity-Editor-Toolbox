using R3;
using UnityEngine;

public class ScrollMechanic : ReactiveMechanic<ScrollMechanic.Event, MouseInputFrame>
{
    public readonly struct Event : IMechanicEvent
    {
        public Event(ScrollMechanic mechanic, float scrollDelta)
        {
            Mechanic = mechanic;
            ScrollDelta = scrollDelta;
        }

        public IMechanic Mechanic { get; }
        public float ScrollDelta { get; }
    }

    protected override void SetupMechanic(IInputSource<MouseInputFrame> inputSource)
    {
        inputSource.InputObservable
            .Select(input => Input.mouseScrollDelta.y)
            .Where(delta => delta != 0)
            .Subscribe(delta => FireEvent(new Event(this, delta)));
    }
}