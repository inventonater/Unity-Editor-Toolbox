using System.Collections.Generic;
using UnityEngine;

namespace Toolbox
{
    public static class TransformExtensions
    {
        private static readonly List<Transform> DescendantCache = new();

        public static int DescendantCount(this Transform t)
        {
            t.GetComponentsInChildren(DescendantCache);
            return DescendantCache.Count - 1; // skip self
        }
    }
}
