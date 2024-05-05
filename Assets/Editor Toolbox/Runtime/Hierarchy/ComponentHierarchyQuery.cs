using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Toolbox
{
    public static class ComponentHierarchyQueryExtensions
    {
        public static ComponentHierarchyQuery<T> GetAncestors<T>(this Component component) where T : Component
        {
            return component.HierarchyQuery<T>().WithTraversal(a => a.TraversalAlgorithm = TraversalAlgorithm.Ancestors);
        }

        public static ComponentHierarchyQuery<T> GetDescendants<T>(this Component component) where T : Component
        {
            return component.HierarchyQuery<T>().WithTraversal(a => a.TraversalAlgorithm = TraversalAlgorithm.DescendantsBreadthFirst);;
        }

        public static ComponentHierarchyQuery<T> HierarchyQuery<T>(this Component component) where T : Component => new(component);
    }

    public enum TraversalAlgorithm
    {
        DescendantsBreadthFirst,
        DescendantsDepthFirst,
        Ancestors,
        SameTreeLevel,
    }

    [Flags]
    public enum InactiveTraversal
    {
        IncludeInactive = 0,
        SkipInactiveGameObjects = 1,
        SkipInactiveComponents = 2,
        HaltInactiveGameObjects = 4,
        HaltInactiveComponents = 8,
        SkipAllInactive = SkipInactiveComponents | SkipInactiveGameObjects,
        HaltAllInactive = HaltInactiveComponents | HaltInactiveGameObjects
    }

    public class HierarchyTraversalRules
    {
        public TraversalAlgorithm TraversalAlgorithm { get; set; } = TraversalAlgorithm.DescendantsBreadthFirst;
        public InactiveTraversal InactiveTraversal { get; set; } = InactiveTraversal.SkipAllInactive;
    }

    public class ComponentFilterRules<T> where T : Component
    {
        public bool AllowOriginComponent { get; set; } = false;
        public bool AllowOriginGameObject { get; set; } = false;
        public string NameIs { get; set; }
        public string NameContains { get; set; }
        public Regex NameRegex { get; set; }
        public Func<T, bool> CustomFilter { get; set; }
    }

    public class ComponentHierarchyQuery<T> : IEnumerable<T> where T : Component
    {
        private readonly Component _origin;

        public ComponentHierarchyQuery(Component origin)
        {
            _origin = origin;
        }

        public HierarchyTraversalRules Traversal { get; set; } = new HierarchyTraversalRules();
        public ComponentFilterRules<T> Filter { get; set; } = new ComponentFilterRules<T>();

        public ComponentHierarchyQuery<T> WithTraversal(Action<HierarchyTraversalRules> action)
        {
            action(Traversal);
            return this;
        }

        public ComponentHierarchyQuery<T> WithTraversal(HierarchyTraversalRules rules)
        {
            Traversal = rules;
            return this;
        }

        public ComponentHierarchyQuery<T> WithFilter(Action<ComponentFilterRules<T>> action)
        {
            action(Filter);
            return this;
        }

        public ComponentHierarchyQuery<T> WithFilter(ComponentFilterRules<T> rules)
        {
            Filter = rules;
            return this;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public IEnumerator<T> GetEnumerator() => SelectEnumerable().GetEnumerator();

        private IEnumerable<T> SelectEnumerable()
        {
            return Traversal.TraversalAlgorithm switch
            {
                TraversalAlgorithm.DescendantsBreadthFirst => TraverseDescendantsBreadthFirst(),
                TraversalAlgorithm.DescendantsDepthFirst => TraverseDescendantsDepthFirst(),
                TraversalAlgorithm.Ancestors => TraverseAncestors(),
                TraversalAlgorithm.SameTreeLevel => TraverseSameTreeLevel(),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private IEnumerable<T> TraverseDescendantsBreadthFirst()
        {
            var queue = new Queue<Transform>();
            queue.Enqueue(_origin.transform); // Start from the origin object so we can potentially include it
            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                if (ShouldHaltTraversal(current)) continue;
                foreach (Transform child in current) queue.Enqueue(child);
                foreach (var component in VisitComponents(current)) yield return component;
            }
        }

        private IEnumerable<T> TraverseDescendantsDepthFirst()
        {
            var stack = new Stack<Transform>();
            stack.Push(_origin.transform); // Start from the origin object so we can potentially include it
            while (stack.Count > 0)
            {
                var current = stack.Pop();
                if (ShouldHaltTraversal(current)) continue;
                foreach (var component in VisitComponents(current)) yield return component;
                for (int i = current.childCount - 1; i >= 0; i--) stack.Push(current.GetChild(i));
            }
        }

        private IEnumerable<T> TraverseAncestors()
        {
            var current = _origin.transform; // Start from the origin object so we can potentially include it
            while (current != null)
            {
                if (ShouldHaltTraversal(current)) yield break;
                foreach (var component in VisitComponents(current)) yield return component;
                current = current.parent;
            }
        }

        private IEnumerable<T> TraverseSameTreeLevel()
        {
            var sameLevel = _origin.transform.parent != null ? _origin.transform.parent.Cast<Transform>() : Hierarchy.GetRootGameObject();
            foreach (Transform current in sameLevel)
            {
                if (ShouldHaltTraversal(current)) continue;  // Skip objects based on custom criteria
                foreach (var component in VisitComponents(current)) yield return component;
            }
        }

        private IEnumerable<T> VisitComponents(Transform transform)
        {
            if (ShouldSkip(transform.gameObject)) yield break;

            foreach (var component in transform.GetComponents<T>())
            {
                if (ShouldSkip(component)) continue;
                if (MatchesFilters(component)) yield return component;
            }
        }

        private bool ShouldSkip(GameObject gameObject)
        {
            return !gameObject.activeInHierarchy && Traversal.InactiveTraversal.HasFlag(InactiveTraversal.SkipInactiveGameObjects);
        }

        private bool ShouldSkip(T component)
        {
            if (component is Behaviour behaviour && !behaviour.enabled)
            {
                return Traversal.InactiveTraversal.HasFlag(InactiveTraversal.SkipInactiveComponents);
            }

            return false;
        }

        private bool ShouldHaltTraversal(Transform transform)
        {
            return !transform.gameObject.activeInHierarchy && Traversal.InactiveTraversal.HasFlag(InactiveTraversal.HaltInactiveGameObjects);
        }

        private bool MatchesFilters(T component)
        {
            if (_origin == component && !Filter.AllowOriginComponent) return false;
            if (_origin.gameObject == component.gameObject && !Filter.AllowOriginGameObject) return false;
            if (!string.IsNullOrEmpty(Filter.NameIs) && component.name != Filter.NameIs) return false;
            if (!string.IsNullOrEmpty(Filter.NameContains) && !component.name.Contains(Filter.NameContains)) return false;
            if (Filter.NameRegex != null && !Filter.NameRegex.IsMatch(component.name)) return false;
            if (Filter.CustomFilter != null && !Filter.CustomFilter(component)) return false;
            return true;
        }
    }
}