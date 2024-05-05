using System.Collections.Generic;
using UnityEngine;

namespace Toolbox
{
    /// <summary>
    /// Represents an enumerable collection of components.
    /// </summary>
    /// <typeparam name="T">The type of the component.</typeparam>
    public interface IComponentEnumerable<T> : IEnumerable<T> where T : Component
    {
        /// <summary>
        /// Returns an enumerator that iterates through the components.
        /// </summary>
        /// <returns>An enumerator for the components.</returns>
        new IEnumerator<T> GetEnumerator();


    }
}