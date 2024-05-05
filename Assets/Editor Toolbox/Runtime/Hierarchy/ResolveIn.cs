#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Toolbox;
using UnityEngine;

namespace Toolbox
{
    /// <summary>
    /// Provides methods for resolving components in the hierarchy based on various criteria.
    /// </summary>
    public static class ResolveIn
    {
        /// <summary>
        /// Resolves a component of type T in the scene.
        /// </summary>
        /// <typeparam name="T">The type of the component to resolve.</typeparam>
        /// <param name="searcher">The component from which to start the search.</param>
        /// <param name="options">The options for resolving the component.</param>
        /// <returns>The resolved component of type T, or null if not found.</returns>
        public static T? Scene<T>(this Component searcher, ResolveOptions options) where T : Component
        {
            var components = GameObject.FindObjectsOfType<T>(includeInactive: options.includeInactive);
            return ResolveComponent(searcher, components, options);
        }

        /// <summary>
        /// Resolves a sibling component of type T.
        /// </summary>
        /// <typeparam name="T">The type of the component to resolve.</typeparam>
        /// <param name="searcher">The component from which to start the search.</param>
        /// <param name="options">The options for resolving the component.</param>
        /// <returns>The resolved component of type T, or null if not found.</returns>
        public static T? Sibling<T>(this Component searcher, ResolveOptions options) where T : Component
        {
            return ResolveComponent(searcher, searcher.GetComponents<T>(), options);
        }

        /// <summary>
        /// Resolves a descendant component of type T.
        /// </summary>
        /// <typeparam name="T">The type of the component to resolve.</typeparam>
        /// <param name="searcher">The component from which to start the search.</param>
        /// <param name="result">The resolved component of type T.</param>
        /// <param name="options">The options for resolving the component.</param>
        /// <returns>The resolved component of type T, or null if not found.</returns>
        public static T? Descendant<T>(this Component searcher, ref T? result, ResolveOptions options = null) where T : Component => result = Descendant<T>(searcher, options);

        public static T? Descendant<T>(this Component searcher, ResolveOptions options = null) where T : Component
        {
            Debug.LogError("ResolveOptions needs to align with DescendantEnumerableOptions somehow ");
            return ResolveComponent(searcher, searcher.GetDescendants<T>(), options);
        }

        /// <summary>
        /// Resolves an ancestor component of type T.
        /// </summary>
        /// <typeparam name="T">The type of the component to resolve.</typeparam>
        /// <param name="searcher">The component from which to start the search.</param>
        /// <param name="options">The options for resolving the component.</param>
        /// <returns>The resolved component of type T, or null if not found.</returns>
        public static T? Ancestor<T>(this Component searcher, ResolveOptions options) where T : Component
        {
            Debug.LogError("ResolveOptions needs to align with DescendantEnumerableOptions somehow ");
            return ResolveComponent(searcher, searcher.GetAncestors<T>(), options);
        }

        // Other methods omitted for brevity

        /// <summary>
        /// Resolves a component based on the specified options.
        /// </summary>
        /// <typeparam name="T">The type of the component to resolve.</typeparam>
        /// <param name="searcher">The component from which to start the search.</param>
        /// <param name="components">The components to filter and resolve.</param>
        /// <param name="options">The options for resolving the component.</param>
        /// <returns>The resolved component of type T, or null if not found.</returns>
        private static T? ResolveComponent<T>(Component searcher, IEnumerable<T> components, ResolveOptions options) where T : Component
        {
            var filterComponents = FilterComponents(searcher, components, options);
            var sortComponents = filterComponents.SortComponents(options.resolveSearchOrder);
            T? resolved = sortComponents.FirstOrDefault();
            if (resolved != null || options.optional) return resolved;

            Debug.LogError(
                $"<b>{searcher.GetType().Name}</b>.{options.callerMethod ?? "NoSearchMethod"} could not find <b>{typeof(T).Name}</b> [{GetPathString(searcher, options)}]",
                searcher);
            return null;
        }



        /// <summary>
        /// Sorts the components based on the specified search order.
        /// </summary>
        /// <typeparam name="T">The type of the components to sort.</typeparam>
        /// <param name="components">The components to sort.</param>
        /// <param name="resolveSearchOrder">The search order for sorting the components.</param>
        /// <returns>The sorted components.</returns>
        private static IEnumerable<T> SortComponents<T>(this IEnumerable<T> components, ResolveSearchOrder resolveSearchOrder) where T : Component
        {
            return components.OrderBy(c => GetSortKey(c, resolveSearchOrder));
        }

        /// <summary>
        /// Gets the sort key for a component based on the specified search order.
        /// </summary>
        /// <typeparam name="T">The type of the component.</typeparam>
        /// <param name="component">The component to get the sort key for.</param>
        /// <param name="resolveSearchOrder">The search order for sorting the components.</param>
        /// <returns>The sort key for the component.</returns>
        private static int GetSortKey<T>(T component, ResolveSearchOrder resolveSearchOrder) where T : Component
        {
            return resolveSearchOrder switch
            {
                ResolveSearchOrder.Closest => component.transform.GetSiblingIndex(),
                ResolveSearchOrder.Farthest => -component.transform.GetSiblingIndex(),
                _ => 0,
            };
        }

        /// <summary>
        /// Gets the path string for logging purposes.
        /// </summary>
        /// <param name="searcher">The component from which to start the search.</param>
        /// <param name="options">The options for resolving the component.</param>
        /// <returns>The path string.</returns>
        private static string GetPathString(Component searcher, ResolveOptions options)
        {
            var path = searcher?.GetPath() ?? string.Empty;

            if (!string.IsNullOrEmpty(options.nameStrict))
            {
                path += $", nameStrict: {options.nameStrict}";
            }

            if (!string.IsNullOrEmpty(options.nameContains))
            {
                path += $", nameContains: {options.nameContains}";
            }

            if (options.sidedName)
            {
                path += $", sideFromName: {searcher?.SideFromName()}";
            }

            return path;
        }
    }


}