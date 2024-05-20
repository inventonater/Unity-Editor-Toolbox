using System;
using System.Collections.Generic;
using UnityEngine;
using static Toolbox.Hierarchy;

namespace Toolbox
{
    [Serializable]
    public class HierarchyPublisher<T> where T : MonoBehaviour
    {
        private static List<HierarchyPublisher<T>> _all = new();
        public static IReadOnlyList<HierarchyPublisher<T>> All => _all;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        private static void CheckStaticReload() => _all.Clear();

        private T _publisher;

        public HierarchyPublisher(T publisher)
        {
            _publisher = publisher;
            _all.Add(this);
            Notify();
        }

        public void Destroy()
        {
            _publisher = null;
            _all.Remove(this);
        }

        public void Notify()
        {
            foreach (var subscriber in HierarchySubscriber<T>.All) Notify(subscriber);
        }

        public void Notify(HierarchySubscriber<T> subscriber)
        {
            subscriber.Notify(_publisher, _publisher.enabled);
        }
    }

    [Serializable]
    public class HierarchySubscriber<T> where T : MonoBehaviour
    {
        private static List<HierarchySubscriber<T>> _all = new();
        public static IReadOnlyList<HierarchySubscriber<T>> All => _all;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        private static void CheckStaticReload() => _all.Clear();

        [SerializeField] public RelationFlags _relationFlags = RelationFlags.Ancestor;
        [SerializeField] private List<T> _publishers = new();
        public IReadOnlyList<T> Publishers => _publishers;

        public event Action<T, bool> WhenActiveRelativesChanged = delegate { };

        private Component _component;

        public HierarchySubscriber(Component component)
        {
            _component = component;
            _all.Add(this);
            foreach (var publisher in HierarchyPublisher<T>.All) publisher.Notify(this);
        }

        public void Destroy()
        {
            _component = null;
            _all.Remove(this);
        }

        public HierarchySubscriber(Component component, RelationFlags relationFlags) : this(component)
        {
            _relationFlags = relationFlags;
        }

        public void Notify(T publisher, bool isActive)
        {
            if (isActive)
            {
                if (_component.IsRelated(publisher, _relationFlags) && !_publishers.Contains(publisher))
                {
                    _publishers.Add(publisher);
                    WhenActiveRelativesChanged(publisher, true);
                }
            }
            else
            {
                if (_publishers.Contains(publisher))
                {
                    _publishers.Remove(publisher);
                    WhenActiveRelativesChanged(publisher, false);
                }
            }
        }
    }
}