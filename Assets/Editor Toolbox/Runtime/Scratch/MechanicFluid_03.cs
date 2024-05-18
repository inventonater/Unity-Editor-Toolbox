using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using R3;

namespace Runtime.MechanicFluid3
{
    public struct InputEvent
    {
        public bool IsButtonPressed;
        public Vector3 CursorPositionWorld;
        public float Timestamp;
    }

    public struct TargetEvent
    {
        public InputEvent InputEvent;
        public bool IsHovering;
        public bool IsSelecting;
        public Vector3 CursorPositionDeltaLocalSpace;
    }

    public class StatelessPressEvent
    {
        public TargetEvent Press;
    }

    // Full sequence of Delta events?
    public class StatefulPressEvent : StatelessPressEvent
    {
        public TargetEvent Press;
        public TargetEvent Release;
        public TargetEvent Latest;
    }

    public class ClickEvent : StatelessPressEvent
    {
    }

    public class DoubleClickEvent : StatelessPressEvent
    {
        public ClickEvent First;
        public ClickEvent Second;
    }

    public class LongPressEvent : StatefulPressEvent
    {
    }

    public class DragEvent : StatefulPressEvent
    {
    }

    public class DragInstantiate : StatefulPressEvent
    {
        public Vector3 Position;
        public GameObject Prefab;
    }

    public interface IMechanic
    {
        int Priority { get; }
        Observable<Unit> ClaimInteractionObservable { get; }
        Observable<StatelessPressEvent> EventObservable { get; }
        Observable<StatelessPressEvent> Begin { get; }
        Observable<StatelessPressEvent> End { get; }
    }

    public abstract class MechanicBase<T> : IMechanic where T : StatelessPressEvent
    {
        public int Priority { get; protected set; }
        public Observable<Unit> ClaimInteractionObservable { get; protected set; }

        public Observable<T> EventObservable { get; protected set; }
        Observable<StatelessPressEvent> IMechanic.EventObservable => EventObservable.Cast<T, StatelessPressEvent>();

        public Observable<T> Begin { get; protected set; }
        Observable<StatelessPressEvent> IMechanic.Begin => EventObservable.Cast<T, StatelessPressEvent>();

        public Observable<T> End { get; protected set; }
        Observable<StatelessPressEvent> IMechanic.End => EventObservable.Cast<T, StatelessPressEvent>();
    }
}
