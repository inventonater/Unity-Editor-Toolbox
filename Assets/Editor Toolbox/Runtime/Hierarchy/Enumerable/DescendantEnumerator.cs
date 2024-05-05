using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolbox
{
    /// <summary>
    /// Represents the different types of search algorithms for traversing the Unity hierarchy.
    /// </summary>
    public enum DescendantTraversalType
    {
        /// <summary>
        /// Breadth-first search traversal.
        /// </summary>
        BreadthFirst,

        /// <summary>
        /// Depth-first search traversal.
        /// </summary>
        DepthFirst,
    }

    /// <summary>
    /// Represents an enumerator for iterating through descendant components.
    /// </summary>
    /// <typeparam name="T">The type of the descendant component.</typeparam>
    public class DescendantEnumerator<T> : IEnumerator<T> where T : Component
    {
        private readonly Component _startComponent;
        private readonly DescendantEnumerableOptions<T> _options;
        private readonly Queue<Transform> _queue = new Queue<Transform>();
        private readonly Stack<Transform> _stack = new Stack<Transform>();
        private int _currentDepth;

        /// <summary>
        /// Initializes a new instance of the <see cref="DescendantEnumerator{T}"/> class.
        /// </summary>
        /// <param name="startComponent">Component to start the descent</param>
        /// <param name="traversalType">BreadthFirst or DepthFirst traversal</param>
        /// <param name="options">The options for configuring the descendant enumerator.</param>
        public DescendantEnumerator(Component startComponent, DescendantEnumerableOptions<T> options)
        {
            _startComponent = startComponent;
            _options = options;
            Reset();
        }

        /// <summary>
        /// Gets the current descendant component.
        /// </summary>
        public T Current { get; private set; }

        object IEnumerator.Current => Current;

        /// <summary>
        /// Advances the enumerator to the next descendant component.
        /// </summary>
        /// <returns>True if there is a next descendant component; otherwise, false.</returns>
        public bool MoveNext()
        {
            if (_options.IsDepthFirst())
            {
                return MoveNextDepthFirst();
            }
            else
            {
                return MoveNextBreadthFirst();
            }
        }

        private bool MoveNextDepthFirst()
        {
            while (_stack.Count > 0)
            {
                var currentTransform = _stack.Pop();
                var candidate = currentTransform.GetComponent<T>();

                if (candidate != null)
                {
                    if (_options.IncludeInactiveComponents || candidate.gameObject.activeInHierarchy)
                    {
                        if (_options.Filter == null || _options.Filter(candidate))
                        {
                            Current = candidate;
                            return true;
                        }
                    }
                }

                if (_currentDepth < _options.MaxDepth || _options.MaxDepth == -1)
                {
                    for (int i = currentTransform.childCount - 1; i >= 0; i--)
                    {
                        _stack.Push(currentTransform.GetChild(i));
                    }
                    _currentDepth++;
                }
            }

            return false;
        }

        private bool MoveNextBreadthFirst()
        {
            while (_queue.Count > 0)
            {
                var currentTransform = _queue.Dequeue();
                var candidate = currentTransform.GetComponent<T>();

                if (candidate != null)
                {
                    if (_options.IncludeInactiveComponents || candidate.gameObject.activeInHierarchy)
                    {
                        if (_options.Filter == null || _options.Filter(candidate))
                        {
                            Current = candidate;
                            return true;
                        }
                    }
                }

                if (_currentDepth < _options.MaxDepth || _options.MaxDepth == -1)
                {
                    for (int i = 0; i < currentTransform.childCount; i++)
                    {
                        _queue.Enqueue(currentTransform.GetChild(i));
                    }
                }
            }

            if (_queue.Count == 0)
            {
                _currentDepth++;
                for (int i = 0; i < _startComponent.transform.childCount; i++)
                {
                    _queue.Enqueue(_startComponent.transform.GetChild(i));
                }
            }

            return false;
        }

        /// <summary>
        /// Resets the enumerator to its initial state.
        /// </summary>
        public void Reset()
        {
            _queue.Clear();
            _stack.Clear();
            _currentDepth = 0;

            if (_options.IncludeSelf)
            {
                if (_options.IsDepthFirst())
                {
                    _stack.Push(_startComponent.transform);
                }
                else
                {
                    _queue.Enqueue(_startComponent.transform);
                }
            }
            else
            {
                var startTransform = _startComponent.transform;
                for (int i = 0; i < startTransform.childCount; i++)
                {
                    if (_options.IsDepthFirst())
                    {
                        _stack.Push(startTransform.GetChild(i));
                    }
                    else
                    {
                        _queue.Enqueue(startTransform.GetChild(i));
                    }
                }
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            // No unmanaged resources to dispose
        }
    }
}