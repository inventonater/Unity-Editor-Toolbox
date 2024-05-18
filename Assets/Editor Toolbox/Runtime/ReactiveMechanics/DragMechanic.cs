using R3;
using UnityEngine;

namespace Toolbox.ReactiveMechanics
{
    public class DragMechanic : ReactiveMechanic<DragMechanic.Event, MouseInputFrame>
    {
        public readonly struct Event : IMechanicEvent
        {
            public Event(DragMechanic mechanic, Vector2 dragStartPosition, Vector2 dragEndPosition)
            {
                Mechanic = mechanic;
                DragStartPosition = dragStartPosition;
                DragEndPosition = dragEndPosition;
            }

            public IMechanic Mechanic { get; }
            public Vector2 DragStartPosition { get; }
            public Vector2 DragEndPosition { get; }
        }

        public float DragThreshold = 10f;

        protected override void SetupMechanic(IInputSource<MouseInputFrame> inputSource)
        {
            var dragStart = inputSource.InputObservable
                .Where(input => input.IsPressed)
                .Select(input => input.Position);

            var dragMove = inputSource.InputObservable
                .Skip(1)
                .Where(input => !input.IsPressed)
                .Select(input => input.Position);

            dragStart
                .SelectMany(startPosition => dragMove
                    .TakeUntil(inputSource.InputObservable.Where(input => !input.IsPressed))
                    .SkipWhile(position => Vector2.Distance(startPosition, position) < DragThreshold)
                    .Select(position => new { StartPosition = startPosition, CurrentPosition = position }))
                .Subscribe(drag => FireEvent(new Event(this, drag.StartPosition, drag.CurrentPosition)));
        }
    }
}