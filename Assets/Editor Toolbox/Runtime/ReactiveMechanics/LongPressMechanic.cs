using System;
using R3;
using UnityEngine;

namespace Toolbox.ReactiveMechanics
{
    public class LongPressMechanic : ReactiveMechanic<LongPressMechanic.Event, IInputFrame>
    {
        public struct Event : IMechanicEvent
        {
            public Event(LongPressMechanic mechanic, Vector2 longPressPosition)
            {
                Mechanic = mechanic;
                LongPressPosition = longPressPosition;
            }

            public IMechanic Mechanic { get; }
            public Vector2 LongPressPosition { get; }
        }

        public readonly float LongPressDuration = 1f;

        protected override void SetupMechanic(IInputSource<IInputFrame> inputSource)
        {
            inputSource.InputObservable
                .Where(input => input.IsPressed)
                .SelectMany(input =>
                {
                    var initialPosition = input.Position;
                    return R3.Observable.Timer(TimeSpan.FromSeconds(LongPressDuration))
                        .TakeUntil(inputSource.InputObservable.Where(i => !i.IsPressed))
                        .Select(_ => initialPosition);
                })
                .Subscribe(position => FireEvent(new Event(this, position)));
        }
    }
}