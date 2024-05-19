using System;
using System.Collections.Generic;
using System.Linq;
using ObservableCollections;
using Toolbox;
using UnityEngine;

namespace R3 // Keep the R3 namespace to ensure ExtensionMethods are always available
{
    // NEVER a GETTER, only a RESOLVER
    // reactiveProperty wraps a currentValue by it's nature.  Having a Getter means the currentValue can be different from teh rsult of the Getter

    // can add a subscription to the value (setter)

    // resolver is an Observable.  fires whenever and however often it says it should... Immidiate, Awake, Start, NextFramePhase, Seconds, OnValidate, CustomNotifier.  it is a subscription.
    // resolver has a Condition (ifNull, customCondition..)

    // notifier is an Observer.  fires when value changes.
    // notifier has a Condition (ifDifferent, customCondition..)

    // so we are providing Rx with a list of Observers and a list of Observables

    // this.WhenBlah.IfBlah.ResolveLikeSo
    // this.WhenChanged.IfBlah.SetSomeValue

    public class RxRef<T> : SerializableReactiveProperty<T> where T : Component
    {
        private T _target;
        public RxRef(T target) : base(target) => _target = target;
    }

    // public static Observable<Unit> PollUpdate => Observable.EveryValueChanged()

    [Serializable]
    public class RxValue<T> : SerializableReactiveProperty<T>, IDisposable
    {
        private readonly Func<T> _getter;
        private readonly Action<T> _setter;
        private readonly CompositeDisposable _disposables = new();

        public RxValue(Func<T> getter, Action<T> setter)
            : base(getter())
        {
            this._getter = getter;
            this._setter = setter;

            // Subscribe to changes in the getter and update the ReactiveProperty
            Observable.EveryUpdate()
                .Select(_ => getter())
                .DistinctUntilChanged()
                .Subscribe(value => Value = value)
                .AddTo(_disposables);

            // Subscribe to changes in the ReactiveProperty and update the setter
            this.Skip(1)
                .Subscribe(setter)
                .AddTo(_disposables);
        }

        public override void Dispose()
        {
            _disposables.Dispose();
            base.Dispose();
        }
    }

    public interface ILazy<T>
    {
        T Ready();
    }

    public static class RxExtensions
    {
        public static RxRef<T> Rx<T>(this T target) where T : Component => new(target); //

        public static RxRef<T> Descendant<T>(this Component c) where T : Component => new(c.QueryDescendants<T>().FirstOrDefault());
        public static RxRef<T> Descendant<T>(this Component c, ref RxRef<T> value) where T : Component => value = value != null ? value : c.Descendant<T>();

        public static RxRef<T> Ancestor<T>(this Component c) where T : Component => new(c.QueryAncestors<T>().FirstOrDefault());
        public static RxRef<T> Ancestor<T>(this Component c, ref RxRef<T> value) where T : Component => value = value != null ? value : c.Ancestor<T>();

        public static IReadOnlyCollection<T> Children<T>(this Component c, ref IReadOnlyCollection<T> value) where T : Component
        {
            if (c is ILazy<T> rdy) rdy.Ready();
            if (value != null)
            {
                // do we need to check for changes in children here?
                return value;
            }
            return value = c.QueryChildren<T>().ToList();
        }

        public static RxRef<T> Scene<T>(this Component c, ref RxRef<T> value, Func<T, bool> filter) where T : Component
        {
            if (c is ILazy<T> rdy) rdy.Ready();
            if (value != null) return value;
            return value = new (c.QueryScene<T>().FirstOrDefault());
        }
    }
}