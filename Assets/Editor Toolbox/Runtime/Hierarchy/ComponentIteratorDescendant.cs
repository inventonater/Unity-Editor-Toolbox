using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using SearchType = Toolbox.ComponentIteratorDescendantSearchType;

namespace Toolbox
{
    public enum ComponentIteratorDescendantSearchType
    {
        BreadthFirst,
        DepthFirst
    }

    public class DescendantEnumerable<T> : IEnumerable<T> where T : Component
    {
        private readonly Component _component;
        private readonly ComponentIteratorDescendantSearchType _searchType;
        private readonly Func<T, bool> _filter;

        public DescendantEnumerable(Component component, ComponentIteratorDescendantSearchType searchType = ComponentIteratorDescendantSearchType.BreadthFirst, Func<T, bool> filter = null)
        {
            _component = component ?? throw new ArgumentNullException(nameof(component));
            _searchType = searchType;
            _filter = filter;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new ComponentIteratorDescendant<T>(_component, _searchType, _filter);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }


    public class ComponentIteratorDescendant<T> : IEnumerator<T> where T : Component
    {
        private readonly Queue<Transform> _bfsQueue = new Queue<Transform>();
        private readonly Stack<Transform> _dfsStack = new Stack<Transform>();
        private readonly Transform _startTransform;
        private readonly SearchType _searchType;
        [CanBeNull] private readonly Func<T, bool> _filter;
        public T Current { get; private set; }

        object IEnumerator.Current => Current ?? throw new InvalidOperationException("Enumeration has not started. Call MoveNext.");

        public ComponentIteratorDescendant(Component startComponent, SearchType searchType = SearchType.BreadthFirst, Func<T, bool> filter = null)
        {
            if (startComponent == null) throw new ArgumentNullException(nameof(startComponent));

            _startTransform = startComponent.transform;
            _searchType = searchType;
            _filter = filter;
            Current = null;
            EnqueueOrPush(_startTransform);
        }

        private void EnqueueOrPush(Transform transform)
        {
            if (_searchType == SearchType.BreadthFirst)
                _bfsQueue.Enqueue(transform);
            else
                _dfsStack.Push(transform);
        }

        public bool MoveNext()
        {
            while (_bfsQueue.Count > 0 || _dfsStack.Count > 0)
            {
                Transform currentTransform = _searchType == SearchType.BreadthFirst ? _bfsQueue.Dequeue() : _dfsStack.Pop();

                foreach (Transform child in currentTransform)
                {
                    EnqueueOrPush(child);
                    T candidate = child.GetComponent<T>();
                    if (candidate != null && (_filter == null || _filter(candidate)))
                    {
                        Current = candidate;
                        return true;
                    }
                }
            }
            return false;
        }

        public void Reset()
        {
            _bfsQueue.Clear();
            _dfsStack.Clear();
            EnqueueOrPush(_startTransform);
            Current = null;
        }

        public void Dispose()
        {
            _bfsQueue.Clear();
            _dfsStack.Clear();
        }
    }
}