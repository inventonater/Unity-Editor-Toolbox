using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Toolbox
{
    public class ComponentFilterRules<T> where T : Component
    {
        public bool AllowOriginComponent { get; set; } = false;
        public bool AllowOriginGameObject { get; set; } = false;
        public string NameIs { get; set; }
        public string NameContains { get; set; }
        public Regex NameRegex { get; set; }
        public Func<T, bool> CustomFilter { get; set; }

        public bool MatchesFilters(Component origin, T component)
        {
            if (!string.IsNullOrEmpty(NameIs) && component.name != NameIs) return false;
            if (!string.IsNullOrEmpty(NameContains) && !component.name.Contains(NameContains)) return false;

            if (origin == component && !AllowOriginComponent) return false;
            if (origin.gameObject == component.gameObject && !AllowOriginGameObject) return false;

            if (NameRegex != null && !NameRegex.IsMatch(component.name)) return false;
            if (CustomFilter != null && !CustomFilter(component)) return false;
            return true;
        }
    }

    public class HierarchyQuery<T> : IEnumerable<T> where T : Component
    {
        public class TraversalRules
        {
            [Flags]
            public enum InactivePolicyFlags
            {
                IncludeInactive = 0,
                SkipInactiveGameObjects = 1,
                SkipInactiveComponents = 2,
                SkipAllInactive = SkipInactiveComponents | SkipInactiveGameObjects,
            }

            public InactivePolicyFlags InactivePolicy { get; set; } = InactivePolicyFlags.SkipAllInactive;

            public bool ShouldSkip(GameObject gameObject) => !gameObject.activeInHierarchy && InactivePolicy.HasFlag(InactivePolicyFlags.SkipInactiveGameObjects);
            public bool ShouldSkip(T component)
            {
                if (component is Behaviour behaviour && !behaviour.enabled) return InactivePolicy.HasFlag(InactivePolicyFlags.SkipInactiveComponents);
                return false;
            }
        }

        public Component Origin { get; }
        public ITraversalAlgorithm<T> TraversalAlgorithm { get; set; }
        public TraversalRules QueryTraversal { get; set; } = new TraversalRules();
        public ComponentFilterRules<T> Filter { get; set; } = new ComponentFilterRules<T>();

        public HierarchyQuery(Component origin) => Origin = origin;
        public HierarchyQuery(Component origin, ITraversalAlgorithm<T> traversal) : this(origin) => TraversalAlgorithm = traversal;

        public HierarchyQuery<T> With(ITraversalAlgorithm<T> traversal)
        {
            TraversalAlgorithm = traversal;
            return this;
        }

        public HierarchyQuery<T> With(Action<TraversalRules> action)
        {
            action(QueryTraversal);
            return this;
        }

        public HierarchyQuery<T> With(TraversalRules rules)
        {
            QueryTraversal = rules;
            return this;
        }

        public HierarchyQuery<T> With(Action<ComponentFilterRules<T>> action)
        {
            action(Filter);
            return this;
        }

        public HierarchyQuery<T> With(ComponentFilterRules<T> rules)
        {
            Filter = rules;
            return this;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public IEnumerator<T> GetEnumerator() => SelectEnumerable().GetEnumerator();
        private IEnumerable<T> SelectEnumerable() => TraversalAlgorithm.Traverse(this);

        // Is it a problem that this cache lives here and not with ITraversalAlgorithm?
        private readonly List<T> _visitComponentsCache = new List<T>();

        public IEnumerable<T> VisitComponents(Transform transform)
        {
            if (QueryTraversal.ShouldSkip(transform.gameObject)) yield break;
            transform.GetComponents(_visitComponentsCache);
            for (var i = 0; i < _visitComponentsCache.Count; i++)
            {
                var visit = _visitComponentsCache[i];
                if (QueryTraversal.ShouldSkip(visit)) continue;
                if (Filter.MatchesFilters(Origin, visit)) yield return visit;
            }
        }
    }
}