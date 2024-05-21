using System.Linq;
using UnityEngine;
using static Toolbox.EHierarchyQueryAlgorithm;

namespace Toolbox
{
    public static class QueryExtensions
    {
        public static Query<T> QueryAncestors<T>(this Component component) where T : Component
        {
            return component.Query<T>(Ancestors);
        }

        public static Query<T> QueryAncestorsTopDownThenDescendants<T>(this Component component) where T : Component
        {
            var ancestorsTopDown = new HierarchyQueryAlgorithm<T>.AncestorsTopDown();
            var descendantsBreadthFirst = new HierarchyQueryAlgorithm<T>.DescendantsBreadthFirst();
            var compoundTraversal = new HierarchyQueryAlgorithm<T>.Compound(ancestorsTopDown, descendantsBreadthFirst);
            return component.Query(compoundTraversal);
        }

        public static Query<T> QueryDescendants<T>(this Component component) where T : Component
        {
            return component.Query<T>(DescendantsBreadthFirst);
        }

        public static Query<T> QueryChildren<T>(this Component component) where T : Component
        {
            return component.Query<T>(ImmediateDescendants);
        }

        public static Query<T> QueryScene<T>(this Component component) where T : Component
        {
            return new Query<T>(component, new HierarchyQueryAlgorithm<T>.EntireScene());
        }

        public static Query<T> Query<T>(this Component component, EHierarchyQueryAlgorithm hierarchyQueryAlgorithm) where T : Component
        {
            return component.Query(HierarchyQueryAlgorithm<T>.Create(hierarchyQueryAlgorithm));
        }

        public static Query<T> Query<T>(this Component component, IQueryAlgorithm<T> queryAlgorithm) where T : Component
        {
            return new(component, queryAlgorithm);
        }

        public static Query<T> Query<T>(this Component origin, RelationFlags flags) where T : Component
        {
            var algorithms = flags.ToQueryAlgorithm<T>();
            return new Query<T>(origin, new HierarchyQueryAlgorithm<T>.Compound(algorithms));
        }

        public static T Resolve<T>(this Component component, Query<T> query = null) where T : Component
        {
            T empty = null;
            return component.Resolve(ref empty, query);
        }

        public static T Resolve<T>(this Component component, ref T field, Query<T> query = null) where T : Component
        {
            if (field != null) return field;
            query ??= component.Query<T>(DescendantsBreadthFirst).With(filter => filter.AllowOriginComponent = true); // confusing
            return field = query.First();
        }
    }
}
