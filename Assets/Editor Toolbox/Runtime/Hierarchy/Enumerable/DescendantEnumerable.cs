using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolbox
{
    /// <summary>
    /// Represents the options for configuring a descendant enumerable.
    /// </summary>
    public class DescendantEnumerableOptions<T> : ComponentEnumerableOptions<T> where T : Component
    {
        /// <summary>
        /// Gets or sets the search type for traversing the hierarchy.
        /// </summary>
        public DescendantTraversalType TraversalType { get; set; } = DescendantTraversalType.DepthFirst;

        public bool IsDepthFirst() => TraversalType == DescendantTraversalType.DepthFirst;
        public bool IsBreadthFirst() => TraversalType == DescendantTraversalType.BreadthFirst;
    }

    /// <summary>
    /// Represents an enumerable collection of descendant components of a specific type.
    /// </summary>
    /// <typeparam name="T">The type of the descendant component.</typeparam>
    public class DescendantEnumerable<T> : IComponentEnumerable<T> where T : Component
    {
        private readonly Component _component;
        private readonly DescendantEnumerableOptions<T> _options;

        /// <summary>
        /// Initializes a new instance of the <see cref="DescendantEnumerable{T}"/> class.
        /// </summary>
        /// <param name="component">The starting component.</param>
        /// <param name="options">The options for configuring the descendant enumerable.</param>
        public DescendantEnumerable(Component component, DescendantEnumerableOptions<T> options = null)
        {
            _component = component;
            _options = options ?? new DescendantEnumerableOptions<T>();
        }

        /// <summary>
        /// Configures the options for the descendant enumerable.
        /// </summary>
        public DescendantEnumerable<T> WithOptions(Action<DescendantEnumerableOptions<T>> configureOptions)
        {
            configureOptions(_options);
            return this;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the descendant components.
        /// </summary>
        /// <returns>An enumerator for the descendant components.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return new DescendantEnumerator<T>(_component, _options);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}