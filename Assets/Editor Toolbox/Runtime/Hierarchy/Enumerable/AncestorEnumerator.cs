using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolbox
{
    /// <summary>
    /// Represents an enumerator for iterating through ancestor components.
    /// </summary>
    /// <typeparam name="T">The type of the ancestor component.</typeparam>
    public class AncestorEnumerator<T> : IEnumerator<T> where T : Component
    {
        private readonly Component _startComponent;
        private readonly ComponentEnumerableOptions _options;
        private readonly Queue<Transform> _queue = new Queue<Transform>();
        private int _currentDepth;

        /// <summary>
        /// Initializes a new instance of the <see cref="AncestorEnumerator{T}"/> class.
        /// </summary>
        /// <param name="startComponent">Component to start the search from</param>
        /// <param name="options">The options for configuring the ancestor enumerator.</param>
        public AncestorEnumerator(Component startComponent, ComponentEnumerableOptions options)
        {
            _startComponent = startComponent;
            _options = options;
            Reset();
        }

        /// <summary>
        /// Gets the current ancestor component.
        /// </summary>
        public T Current { get; private set; }

        object IEnumerator.Current => Current;

        /// <summary>
        /// Advances the enumerator to the next ancestor component.
        /// </summary>
        /// <returns>True if there is a next ancestor component; otherwise, false.</returns>
        public bool MoveNext()
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

                EnqueueParent(currentTransform);
            }

            return false;
        }

        /// <summary>
        /// Resets the enumerator to its initial state.
        /// </summary>
        public void Reset()
        {
            _queue.Clear();
            _currentDepth = 0;

            if (_options.IncludeSelf)
            {
                _queue.Enqueue(_startComponent.transform);
            }
            else
            {
                EnqueueParent(_startComponent.transform);
            }
        }

        /// <summary>
        /// Enqueues the parent transform of the specified transform.
        /// </summary>
        /// <param name="transform">The transform to enqueue the parent of.</param>
        private void EnqueueParent(Transform transform)
        {
            if (transform != null && (_options.MaxDepth == -1 || _currentDepth < _options.MaxDepth))
            {
                _currentDepth++;
                _queue.Enqueue(transform.parent);
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