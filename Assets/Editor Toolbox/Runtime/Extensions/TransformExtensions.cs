using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Toolbox
{
    public static class TransformExtensions
    {
        /// <summary>
        /// Gets the number of descendants of the transform.
        /// </summary>
        /// <param name="transform">The transform to count descendants for.</param>
        /// <returns>The number of descendants of the transform.</returns>
        private static readonly Stack<Transform> DescendantCountStack = new();

        public static int GetDescendantCount(this Transform transform)
        {
            int count = 0;
            DescendantCountStack.Push(transform);
            while (DescendantCountStack.Count > 0)
            {
                Transform current = DescendantCountStack.Pop();
                int childCount = current.childCount;
                count += childCount;
                for (int i = 0; i < childCount; i++) DescendantCountStack.Push(current.GetChild(i));
            }

            return count;
        }


        /// <summary>
        /// Determines whether the transform is an ancestor of the specified transform.
        /// </summary>
        /// <param name="ancestor">The transform to check.</param>
        /// <param name="descendant">The potential descendant transform.</param>
        /// <returns>True if the transform is an ancestor of the specified transform; otherwise, false.</returns>
        public static bool IsAncestorOf(this Transform ancestor, Transform descendant)
        {
            return descendant != null && descendant.IsChildOf(ancestor);
        }

        public static bool IsDescendantOf(this Transform descendant, Transform ancestor)
        {
            return ancestor != null && descendant.IsChildOf(ancestor);
        }

        /// <summary>
        /// Gets the siblings of the transform.
        /// </summary>
        /// <param name="transform">The transform to get the siblings for.</param>
        /// <param name="siblings">The list to store the siblings.</param>
        public static void GetSiblings(this Transform transform, List<Transform> siblings)
        {
            siblings.Clear();

            Transform parent = transform.parent;
            if (parent == null) return;

            for (int i = 0; i < parent.childCount; i++)
            {
                Transform sibling = parent.GetChild(i);
                if (sibling != transform) siblings.Add(sibling);
            }
        }

        /// <summary>
        /// Resets the position, rotation, and scale of the transform to their default values.
        /// </summary>
        /// <param name="transform">The transform to reset.</param>
        public static void Reset(this Transform transform)
        {
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;
        }

        /// <summary>
        /// Sets the position, rotation, and scale of the transform.
        /// </summary>
        /// <param name="transform">The transform to modify.</param>
        /// <param name="position">The new position.</param>
        /// <param name="rotation">The new rotation.</param>
        /// <param name="scale">The new scale.</param>
        public static void SetTransform(this Transform transform, Vector3 position, Quaternion rotation, Vector3 scale)
        {
            transform.position = position;
            transform.rotation = rotation;
            transform.localScale = scale;
        }

        /// <summary>
        /// Copies the position, rotation, and scale from another transform.
        /// </summary>
        /// <param name="transform">The transform to modify.</param>
        /// <param name="other">The transform to copy from.</param>
        public static void CopyTransform(this Transform transform, Transform other)
        {
            transform.SetTransform(other.position, other.rotation, other.localScale);
        }

        /// <summary>
        /// Gets all components of the specified type in the transform's hierarchy, including the transform itself.
        /// </summary>
        /// <typeparam name="T">The type of components to retrieve.</typeparam>
        /// <param name="transform">The transform to search the hierarchy for.</param>
        /// <param name="components">A list to store the components.</param>
        public static void GetComponentsInHierarchy<T>(this Transform transform, List<T> components) where T : Component
        {
            components.Clear();
            transform.GetComponentsInParent(true, components);
            transform.GetComponentsInChildren(true, components);
        }

        /// <summary>
        /// Destroys all child objects of the transform.
        /// </summary>
        /// <param name="transform">The transform to destroy children for.</param>
        public static void DestroyChildren(this Transform transform)
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                Object.Destroy(transform.GetChild(i).gameObject);
            }
        }

        /// <summary>
        /// Sets the layer of the transform and all its descendants.
        /// </summary>
        /// <param name="transform">The transform to set the layer for.</param>
        /// <param name="layer">The layer index.</param>
        public static void SetLayerRecursively(this Transform transform, int layer)
        {
            transform.gameObject.layer = layer;
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).SetLayerRecursively(layer);
            }
        }
    }
}
