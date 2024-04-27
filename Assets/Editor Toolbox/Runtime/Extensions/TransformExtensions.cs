using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Toolbox
{
    public static class TransformExtensions
    {
        private static readonly List<Transform> DescendantCache = new();
        private static readonly List<Component> ComponentCache = new();

        /// <summary>
        /// Gets the number of descendants of the transform.
        /// </summary>
        /// <param name="transform">The transform to count descendants for.</param>
        /// <returns>The number of descendants of the transform.</returns>
        public static int GetDescendantCount(this Transform transform)
        {
            int count = 0;
            CountDescendantsRecursive(transform, ref count);
            return count;
        }

        private static void CountDescendantsRecursive(Transform transform, ref int count)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                count++;
                CountDescendantsRecursive(transform.GetChild(i), ref count);
            }
        }

        /// <summary>
        /// Gets the root transform of the hierarchy.
        /// </summary>
        /// <param name="transform">The transform to get the root for.</param>
        /// <returns>The root transform of the hierarchy.</returns>
        public static Transform GetRoot(this Transform transform)
        {
            return transform.root;
        }

        /// <summary>
        /// Determines whether the transform is an ancestor of the specified transform.
        /// </summary>
        /// <param name="transform">The transform to check.</param>
        /// <param name="descendant">The potential descendant transform.</param>
        /// <returns>True if the transform is an ancestor of the specified transform; otherwise, false.</returns>
        public static bool IsAncestorOf(this Transform transform, Transform descendant)
        {
            return descendant != null && descendant.IsChildOf(transform);
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
            if (parent == null)
            {
                return;
            }

            for (int i = 0; i < parent.childCount; i++)
            {
                Transform sibling = parent.GetChild(i);
                if (sibling != transform)
                {
                    siblings.Add(sibling);
                }
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
        /// Finds the first descendant transform with the specified name.
        /// </summary>
        /// <param name="transform">The transform to search descendants for.</param>
        /// <param name="name">The name of the transform to find.</param>
        /// <returns>The first descendant transform with the specified name, or null if not found.</returns>
        public static Transform FindDescendant(this Transform transform, string name)
        {
            if (transform.name == name) return transform;

            for (int i = 0; i < transform.childCount; i++)
            {
                Transform descendant = transform.GetChild(i).FindDescendant(name);
                if (descendant != null)
                {
                    return descendant;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets all descendant transforms with the specified name.
        /// </summary>
        /// <param name="transform">The transform to search descendants for.</param>
        /// <param name="name">The name of the transforms to find.</param>
        /// <param name="descendants">A list to store the descendant transforms.</param>
        public static void FindDescendants(this Transform transform, string name, List<Transform> descendants)
        {
            descendants.Clear();
            FindDescendantsRecursive(transform, name, descendants);
        }

        private static void FindDescendantsRecursive(Transform transform, string name, List<Transform> descendants)
        {
            if (transform.name == name)
            {
                descendants.Add(transform);
            }

            for (int i = 0; i < transform.childCount; i++)
            {
                FindDescendantsRecursive(transform.GetChild(i), name, descendants);
            }
        }

        /// <summary>
        /// Gets the first component of the specified type in the transform's hierarchy, including the transform itself.
        /// </summary>
        /// <typeparam name="T">The type of component to retrieve.</typeparam>
        /// <param name="transform">The transform to search the hierarchy for.</param>
        /// <returns>The first component of the specified type, or null if not found.</returns>
        public static T GetComponentInHierarchy<T>(this Transform transform) where T : Component
        {
            return transform.GetComponentInParent<T>() ?? transform.GetComponentInChildren<T>();
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

        /// <summary>
        /// Determines whether the transform is a descendant of the specified parent transform.
        /// </summary>
        /// <param name="transform">The transform to check.</param>
        /// <param name="parent">The potential parent transform.</param>
        /// <returns>True if the transform is a descendant of the specified parent transform; otherwise, false.</returns>
        public static bool IsDescendantOf(this Transform transform, Transform parent)
        {
            return parent != null && transform.IsChildOf(parent);
        }

        /// <summary>
        /// Finds the closest ancestor transform with the specified name.
        /// </summary>
        /// <param name="transform">The transform to search ancestors for.</param>
        /// <param name="name">The name of the ancestor transform to find.</param>
        /// <returns>The closest ancestor transform with the specified name, or null if not found.</returns>
        public static Transform FindAncestor(this Transform transform, string name)
        {
            Transform ancestor = transform.parent;
            while (ancestor != null)
            {
                if (ancestor.name == name) return ancestor;
                ancestor = ancestor.parent;
            }
            return null;
        }

        /// <summary>
        /// Gets the distance between two transforms.
        /// </summary>
        /// <param name="transform">The first transform.</param>
        /// <param name="other">The second transform.</param>
        /// <returns>The distance between the two transforms.</returns>
        public static float Distance(this Transform transform, Transform other)
        {
            return Vector3.Distance(transform.position, other.position);
        }

        /// <summary>
        /// Gets the squared distance between two transforms.
        /// </summary>
        /// <param name="transform">The first transform.</param>
        /// <param name="other">The second transform.</param>
        /// <returns>The squared distance between the two transforms.</returns>
        public static float DistanceSquared(this Transform transform, Transform other)
        {
            return (transform.position - other.position).sqrMagnitude;
        }

        /// <summary>
        /// Determines whether the transform has a parent.
        /// </summary>
        /// <param name="transform">The transform to check.</param>
        /// <returns>True if the transform has a parent; otherwise, false.</returns>
        public static bool HasParent(this Transform transform)
        {
            return transform.parent != null;
        }

        /// <summary>
        /// Determines whether the transform has any children.
        /// </summary>
        /// <param name="transform">The transform to check.</param>
        /// <returns>True if the transform has any children; otherwise, false.</returns>
        public static bool HasChildren(this Transform transform)
        {
            return transform.childCount > 0;
        }

        /// <summary>
        /// Gets the first child transform with the specified name.
        /// </summary>
        /// <param name="transform">The transform to search children for.</param>
        /// <param name="name">The name of the child transform to find.</param>
        /// <returns>The first child transform with the specified name, or null if not found.</returns>
        public static Transform FindChild(this Transform transform, string name)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);
                if (child.name == name)
                {
                    return child;
                }
            }
            return null;
        }

        /// <summary>
        /// Gets all child transforms with the specified name.
        /// </summary>
        /// <param name="transform">The transform to search children for.</param>
        /// <param name="name">The name of the child transforms to find.</param>
        /// <param name="children">A list to store the child transforms.</param>
        public static void FindChildren(this Transform transform, string name, List<Transform> children)
        {
            children.Clear();
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);
                if (child.name == name)
                {
                    children.Add(child);
                }
            }
        }
    }
}