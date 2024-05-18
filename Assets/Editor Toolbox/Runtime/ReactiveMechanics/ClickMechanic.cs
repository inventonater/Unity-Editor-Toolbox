using R3;
using UnityEngine;

namespace Toolbox.ReactiveMechanics
{
    public struct InputEvent
    {
        // position of the cursor in local button space
        public Vector3 CursorPosition;

        // is the button currently pressed this frame?
        public bool IsButtonPressed;

        // events are provided every frame, typically 60 per second
        public float Timestamp;
    }

    public struct ButtonEvent
    {
        public bool IsHovering;
        public bool IsSelecting;
        public Vector3 CursorPositionDelta;
    }

    public class UIEvent { }
    public class SingleClickEvent : UIEvent {}
    public class DoubleClickEvent : UIEvent {}
    public class LongPressEvent : UIEvent {}
    public class DragDropEvent : UIEvent {}

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
}
