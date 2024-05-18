using System;
using R3;
using UnityEngine;

namespace Toolbox.ReactiveMechanics
{
    public interface IInputFrame
    {
        bool IsPressed { get; }
        Vector2 Position { get; }
    }

    public interface IInputSource<TInputFrame> where TInputFrame : IInputFrame
    {
        Observable<TInputFrame> InputObservable { get; }
    }

    public interface IMechanicEvent
    {
        IMechanic Mechanic { get; }
    }

    public interface IMechanic
    {
        Type MechanicType { get; }
    }

    public abstract class ReactiveMechanic<TMechanicEvent, TInputFrame> : MonoBehaviour, IMechanic
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
}