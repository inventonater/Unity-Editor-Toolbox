using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Toolbox
{
    public interface IQueryFilter<T> where T : Component
    {
        IEnumerable<T> VisitComponentsOnGameObject(Component origin, GameObject visitedGameObject);
        IEnumerable<T> VisitComponent(Component origin, T visited);
    }

    public class QueryFilter<T> : IQueryFilter<T> where T : Component
    {
        public static QueryFilter<T> Default => new ();
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

        private readonly List<T> _visitComponentsCache = new();

        public IEnumerable<T> VisitComponentsOnGameObject(Component origin, GameObject visitedGameObject)
        {
            if (ShouldSkip(visitedGameObject)) yield break;

            visitedGameObject.GetComponents(_visitComponentsCache);
            for (var i = 0; i < _visitComponentsCache.Count; i++)
            {
                var visit = _visitComponentsCache[i];
                if (ShouldSkip(visit)) continue;
                if (MatchesFilters(origin, visit)) yield return visit;
            }
        }

        public IEnumerable<T> VisitComponent(Component origin, T visited)
        {
            if (ShouldSkip(visited.gameObject)) yield break;
            if (ShouldSkip(visited)) yield break;
            if (MatchesFilters(origin, visited)) yield return visited;
        }
    }
}