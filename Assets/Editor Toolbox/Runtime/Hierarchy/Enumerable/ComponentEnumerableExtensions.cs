using System;
using UnityEngine;

namespace Toolbox
{
    /// <summary>
    /// Provides extension methods for working with component enumerables.
    /// </summary>
    public static class ComponentEnumerableExtensions
    {
        /// <summary>
        /// Returns an enumerable collection of ancestor components of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of the ancestor component.</typeparam>
        /// <param name="component">The starting component.</param>
        /// <param name="options">The options for configuring the ancestor enumerable.</param>
        /// <returns>An enumerable collection of ancestor components.</returns>
        public static AncestorEnumerable<T> GetAncestors<T>(this Component component, ComponentEnumerableOptions<T> options = null) where T : Component
        {
            return new AncestorEnumerable<T>(component, options);
        }

        /// <summary>
        /// Returns an enumerable collection of descendant components of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of the descendant component.</typeparam>
        /// <param name="component">The starting component.</param>
        /// <param name="traversalType"></param>
        /// <param name="options">The options for configuring the descendant enumerable.</param>
        /// <returns>An enumerable collection of descendant components.</returns>
        public static DescendantEnumerable<T> GetDescendants<T>(this Component component, ComponentEnumerableOptions<T> options = null) where T : Component
        {
            return new DescendantEnumerable<T>(component, options);
        }

        /// <summary>
        /// Configures the ancestor enumerable to include the starting component in the iteration.
        /// </summary>
        /// <typeparam name="T">The type of the component.</typeparam>
        /// <param name="options">The options for configuring the ancestor enumerable.</param>
        /// <returns>The updated options.</returns>
        public static ComponentEnumerableOptions IncludeSelf(this ComponentEnumerableOptions options)
        {
            options.IncludeSelf = true;
            return options;
        }
    }
}