using UnityEngine;
using System;
using R3;

public interface IInputFrame
{
    bool IsPressed { get; }
    Vector2 Position { get; }
}

public struct MouseInputFrame : IInputFrame
{
    public MouseInputFrame(bool isPressed, Vector2 position) : this()
    {
        IsPressed = isPressed;
        Position = position;
    }

    public bool IsPressed { get;  }
    public Vector2 Position { get; }
    public Vector3 MouseSpecificDetails { get; }
}

public interface IInputSource<TInputFrame> where TInputFrame : IInputFrame
{
    Observable<TInputFrame> InputObservable { get; }
}

public class MouseInputSource : MonoBehaviour, IInputSource<MouseInputFrame>
{
    private Subject<MouseInputFrame> _inputSubject = new();

    public Observable<MouseInputFrame> InputObservable => _inputSubject.AsObservable();

    private void Update()
    {
        _inputSubject.OnNext(new MouseInputFrame(isPressed: Input.GetMouseButton(0), position: Input.mousePosition));
    }

    private void OnDestroy() => _inputSubject.Dispose();
}

public interface IMechanicEvent
{
    IMechanic Mechanic { get; }
}

public interface IMechanic
{
    Type MechanicType { get; }
}

public abstract class Mechanic<TMechanicEvent, TInputFrame> : MonoBehaviour, IMechanic
    where TMechanicEvent : IMechanicEvent
    where TInputFrame : IInputFrame
{
    public virtual Type MechanicType => GetType();

    private IInputSource<TInputFrame> _inputSource;
    private readonly Subject<TMechanicEvent> _subject = new();
    public Observable<TMechanicEvent> Observable => _subject.AsObservable();

    protected virtual void Awake()
    {
        _inputSource = GetInputSource();
        SetupMechanic(_inputSource);
    }

    protected virtual IInputSource<TInputFrame> GetInputSource() => GetComponent<IInputSource<TInputFrame>>();

    protected abstract void SetupMechanic(IInputSource<TInputFrame> inputSource1);
    
    protected void FireEvent(in TMechanicEvent mechanicEvent) => _subject.OnNext(mechanicEvent);

    protected virtual void OnDestroy() => _subject.Dispose();
}

public class ClickMechanic : Mechanic<ClickMechanic.Event, IInputFrame>
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

public class DoubleClickMechanic : Mechanic<DoubleClickMechanic.Event, IInputFrame>
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

public class LongPressMechanic : Mechanic<LongPressMechanic.Event, IInputFrame>
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

public class DragMechanic : Mechanic<DragMechanic.Event, MouseInputFrame>
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

public class EventHandler : MonoBehaviour
{
    private ClickMechanic[] clickMechanics;
    private Mechanic<DoubleClickMechanic.Event, MouseInputFrame>[] doubleClickMechanics;
    private Mechanic<LongPressMechanic.Event, MouseInputFrame>[] longPressMechanics;
    private Mechanic<DragMechanic.Event, MouseInputFrame>[] dragMechanics;

    private void Start()
    {
        clickMechanics = GetComponents<ClickMechanic>();
        doubleClickMechanics = GetComponents<Mechanic<DoubleClickMechanic.Event, MouseInputFrame>>();
        longPressMechanics = GetComponents<Mechanic<LongPressMechanic.Event, MouseInputFrame>>();
        dragMechanics = GetComponents<Mechanic<DragMechanic.Event, MouseInputFrame>>();

        foreach (var mechanic in clickMechanics)
        {
            mechanic.Observable.Subscribe(HandleClickEvent);
        }

        foreach (var mechanic in doubleClickMechanics)
        {
            mechanic.Observable.Subscribe(HandleDoubleClickEvent);
        }

        foreach (var mechanic in longPressMechanics)
        {
            mechanic.Observable.Subscribe(HandleLongPressEvent);
        }

        foreach (var mechanic in dragMechanics)
        {
            mechanic.Observable.Subscribe(HandleDragEvent);
        }
    }

    private void HandleClickEvent(ClickMechanic.Event clickEvent)
    {
        Debug.Log("Click event triggered at position: " + clickEvent.ClickPosition);
    }

    private void HandleDoubleClickEvent(DoubleClickMechanic.Event doubleClickEvent)
    {
        Debug.Log("Double-click event triggered at position: " + doubleClickEvent.DoubleClickPosition);
    }

    private void HandleLongPressEvent(LongPressMechanic.Event longPressEvent)
    {
        Debug.Log("Long press event triggered at position: " + longPressEvent.LongPressPosition);
    }

    private void HandleDragEvent(DragMechanic.Event dragEvent)
    {
        Debug.Log($"Drag event triggered from {dragEvent.DragEndPosition} to {dragEvent.DragEndPosition}");
    }
}