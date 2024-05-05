using System;
using UnityEngine;

namespace Toolbox
{
    /// <summary>
    /// Represents the options for configuring a component enumerable.
    /// </summary>
    public class ComponentEnumerableOptions<T> where T : Component
    {
        /// <summary>
        /// Gets or sets the filter function for descendant components.
        /// </summary>
        public Func<T, bool> Filter { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to include the starting component in the iteration.
        /// </summary>
        public bool IncludeSelf { get; set; }

        /// <summary>
        /// Gets or sets the maximum depth to traverse down the hierarchy. Use -1 for infinite depth.
        /// </summary>
        public int MaxDepth { get; set; } = -1;

        /// <summary>
        /// Gets or sets a value indicating whether to include inactive components in the iteration.
        /// </summary>
        public bool IncludeInactiveComponents { get; set; }
    }
}