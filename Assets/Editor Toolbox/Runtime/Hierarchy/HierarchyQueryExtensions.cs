using System.Linq;
using UnityEngine;
using static Toolbox.ETraversalAlgorithm;

namespace Toolbox
{
    public static class HierarchyQueryExtensions
    {
        public static HierarchyQuery<T> QueryAncestors<T>(this Component component) where T : Component
        {
            return component.Query<T>(Ancestors);
        }

        public static HierarchyQuery<T> QueryAncestorsTopDownThenDescendants<T>(this Component component) where T : Component
        {
            var ancestorsTopDown = new HierarchyQueryTraversal<T>.AncestorsTopDown();
            var descendantsBreadthFirst = new HierarchyQueryTraversal<T>.DescendantsBreadthFirst();
            var compoundTraversal = new HierarchyQueryTraversal<T>.Compound(ancestorsTopDown, descendantsBreadthFirst);
            return component.Query<T>(compoundTraversal);
        }

        public static HierarchyQuery<T> QueryDescendants<T>(this Component component) where T : Component
        {
            return component.Query<T>(DescendantsBreadthFirst);
        }

        public static HierarchyQuery<T> QueryScene<T>(this Component component) where T : Component
        {
            return new HierarchyQuery<T>(component, new HierarchyQueryTraversal<T>.FindObjectsOfType());
        }

        public static HierarchyQuery<T> Query<T>(this Component component, ETraversalAlgorithm traversalAlgorithm) where T : Component
        {
            return component.Query(HierarchyQueryTraversal<T>.Create(traversalAlgorithm));
        }

        public static HierarchyQuery<T> Query<T>(this Component component, ITraversalAlgorithm<T> traversalAlgorithm) where T : Component
        {
            return new(component, traversalAlgorithm);
        }

        public static T Resolve<T>(this Component component, HierarchyQuery<T> query = null) where T : Component
        {
            T empty = null;
            return component.Resolve(ref empty, query);
        }

        public static T Resolve<T>(this Component component, ref T field, HierarchyQuery<T> query = null) where T : Component
        {
            if (field != null) return field;
            query ??= component.Query<T>(DescendantsBreadthFirst).With(filter => filter.AllowOriginComponent = true); // confusing
            return field = query.First();
        }
    }
}
