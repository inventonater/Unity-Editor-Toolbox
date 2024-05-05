using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace Toolbox
{
    /// <summary>
    /// Represents an enumerable collection of ancestor components of a specific type.
    /// </summary>
    /// <typeparam name="T">The type of the ancestor component.</typeparam>
    public class AncestorEnumerable<T> : IComponentEnumerable<T> where T : Component
    {
        private readonly Component _component;
        private readonly ComponentEnumerableOptions _options;

        /// <summary>
        /// Initializes a new instance of the <see cref="AncestorEnumerable{T}"/> class.
        /// </summary>
        /// <param name="component">The starting component.</param>
        /// <param name="options">The options for configuring the ancestor enumerable.</param>
        public AncestorEnumerable(Component component, [CanBeNull] ComponentEnumerableOptions options = null)
        {
            _component = component;
            _options = options ?? new ComponentEnumerableOptions();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the ancestor components.
        /// </summary>
        /// <returns>An enumerator for the ancestor components.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return new AncestorEnumerator<T>(_component, _options);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}