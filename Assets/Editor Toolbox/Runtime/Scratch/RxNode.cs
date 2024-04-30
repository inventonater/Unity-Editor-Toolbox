using System;
using R3;
using UnityEngine;

public class RxNode : MonoBehaviour
{
    public Observable<EventData> EventStream => _eventSubject;

    private readonly Subject<EventData> _eventSubject = new Subject<EventData>();
    protected readonly CompositeDisposable _disposables = new CompositeDisposable();

    protected virtual void OnEvent(EventData eventData)
    {
        // Default implementation: Propagate the event up the hierarchy
        _eventSubject.OnNext(eventData);
    }

    protected virtual void OnDestroy()
    {
        _disposables.Dispose();
    }

    protected void EmitEvent(string eventName, object eventData = null)
    {
        var eventWrapper = new EventData(eventName, eventData);
        OnEvent(eventWrapper);
    }
}

public class ParentNode : RxNode
{
    protected override void OnEvent(EventData eventData)
    {
        // Customize event handling or wrapping before propagating up the hierarchy
        if (eventData.EventName == "ChildEvent")
        {
            // Wrap the child event with additional details
            var wrappedEventData = new EventData("ParentEvent", new { ChildData = eventData.Data, AdditionalData = "Parent Data" });
            base.OnEvent(wrappedEventData);
        }
        else
        {
            // Propagate other events without modification
            base.OnEvent(eventData);
        }
    }

    private void Start()
    {
        void OnNext(EventData eventData)
        {
            // Handle child events or make decisions based on the event
            Debug.Log($"Received event from child: {eventData.EventName}");
            OnEvent(eventData);
        }

        // Subscribe to child node events
        foreach (var childNode in GetComponentsInChildren<RxNode>())
        {
            if (childNode == this) continue;

            childNode.EventStream
                .Subscribe(OnNext)
                .AddTo(_disposables);
        }
    }
}

public class ChildNode : RxNode
{
    private void Start()
    {
        // Emit a child event after a delay
        Observable.Timer(TimeSpan.FromSeconds(2))
            .Subscribe(_ => EmitEvent("ChildEvent", "Child Data"))
            .AddTo(_disposables);
    }
}

public class EventData
{
    public string EventName { get; }
    public object Data { get; }

    public EventData(string eventName, object data)
    {
        EventName = eventName;
        Data = data;
    }
}