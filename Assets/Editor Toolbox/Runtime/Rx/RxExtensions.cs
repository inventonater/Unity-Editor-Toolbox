using System;
using System.Collections.Generic;
using System.Linq;
using Toolbox;
using UnityEngine;

namespace R3 // Keep the R3 namespace to ensure ExtensionMethods are always available
{
    public class RxRef<T> : SerializableReactiveProperty<T> where T : Component
    {
        public RxRef(T target) : base(target)
        {
        }
    }

    [Serializable]
    public class RxWrapper<T> : SerializableReactiveProperty<T>
    {
        public RxWrapper(Component source, Func<T> getter, Action<T> setter) : this(source, getter, setter, ObservableSystem.DefaultFrameProvider)
        {
        }

        public RxWrapper(Component source, Func<T> getter, Action<T> setter, FrameProvider frameProvider) : base(getter())
        {
            this.Subscribe(setter).AddTo(source);
            Observable.EveryValueChanged(source, _ => getter(), frameProvider).Subscribe(value => Value = value).AddTo(source);
        }
    }

    public interface IReady<T>
    {
        T Ready();
    }

    public static class RxExtensions
    {
        public static RxWrapper<T> RxWrapper<T>(this Component owner, ref RxWrapper<T> field, Func<T> getter, Action<T> setter)
        {
            if (field != null) return field;
            return field = new RxWrapper<T>(owner, getter, setter);
        }

        public static RxRef<T> Rx<T>(this T target) where T : Component => new(target); //

        public static RxRef<T> Descendant<T>(this Component c) where T : Component => new(c.QueryDescendants<T>().FirstOrDefault());
        public static RxRef<T> Descendant<T>(this Component c, ref RxRef<T> value) where T : Component => value = value != null ? value : c.Descendant<T>();

        public static RxRef<T> Ancestor<T>(this Component c) where T : Component => new(c.QueryAncestors<T>().FirstOrDefault());
        public static RxRef<T> Ancestor<T>(this Component c, ref RxRef<T> value) where T : Component => value = value != null ? value : c.Ancestor<T>();

        public static IReadOnlyCollection<T> Children<T>(this Component c, ref IReadOnlyCollection<T> value) where T : Component
        {
            if (c is IReady<T> rdy) rdy.Ready();
            if (value != null)
            {
                // do we need to check for changes in children here?
                return value;
            }

            return value = c.QueryChildren<T>().ToList();
        }

        public static RxRef<T> Scene<T>(this Component c, ref RxRef<T> value, Func<T, bool> filter) where T : Component
        {
            if (c is IReady<T> rdy) rdy.Ready();
            if (value != null) return value;
            return value = new(c.QueryScene<T>().FirstOrDefault());
        }
    }
}
