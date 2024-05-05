using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace Toolbox
{
    public class AncestorEnumerable<T> : IEnumerable<T> where T : Component
    {
        private readonly Component _component;
        private readonly Func<T, bool> _filter;

        public AncestorEnumerable(Component component, Func<T, bool> filter = null)
        {
            _component = component ?? throw new ArgumentNullException(nameof(component));
            _filter = filter;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new ComponentIteratorAncestor<T>(_component, _filter);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class ComponentIteratorAncestor<T> : IEnumerator<T> where T : Component
    {
        [CanBeNull] private readonly Func<T, bool> _filter;
        private readonly Transform _startTransform;
        private Transform _currentTransform;
        public T Current { get; private set; }

        object IEnumerator.Current => Current;

        public ComponentIteratorAncestor(Component startComponent, Func<T, bool> filter = null)
        {
            if (startComponent == null) throw new ArgumentNullException(nameof(startComponent));

            this._filter = filter;
            this._startTransform = startComponent.transform;
            this._currentTransform = this._startTransform;
            this.Current = null;
        }

        public bool MoveNext()
        {
            while (_currentTransform != null)
            {
                _currentTransform = _currentTransform.parent;
                if (_currentTransform == null) return false;

                T candidate = _currentTransform.GetComponent<T>();
                if (_filter != null && !_filter(candidate)) continue;

                Current = candidate;
                if (Current != null)
                    return true;
            }
            return false;
        }

        public void Reset()
        {
            _currentTransform = _startTransform;
            Current = null;
        }

        public void Dispose()
        {
            // No resources to dispose in this context
        }
    }
}