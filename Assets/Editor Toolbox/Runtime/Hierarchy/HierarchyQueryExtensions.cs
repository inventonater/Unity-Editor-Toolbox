using System.Linq;
using UnityEngine;

namespace Toolbox
{
    public static class HierarchyQueryExtensions
    {
        public static HierarchyQuery<T> GetAncestors<T>(this Component component) where T : Component
        {
            return component.Query<T>().With(HierarchyQueryTraversal<T>.Create(ETraversalAlgorithm.Ancestors));
        }

        public static HierarchyQuery<T> GetAncestorsTopDownThenDescendants<T>(this Component component) where T : Component
        {
            var ancestorsTopDown = new HierarchyQueryTraversal<T>.AncestorsTopDown();
            var descendantsBreadthFirst = new HierarchyQueryTraversal<T>.DescendantsBreadthFirst();
            var compoundTraversal = new HierarchyQueryTraversal<T>.Compound(ancestorsTopDown, descendantsBreadthFirst);
            return component.Query<T>().With(compoundTraversal);
        }

        public static HierarchyQuery<T> GetDescendants<T>(this Component component) where T : Component
        {
            return component.Query<T>().With(new HierarchyQueryTraversal<T>.DescendantsBreadthFirst());
        }

        public static HierarchyQuery<T> GetScene<T>(this Component component) where T : Component
        {
            return new HierarchyQuery<T>(component, new HierarchyQueryTraversal<T>.FindObjectsOfType());
        }

        public static HierarchyQuery<T> Query<T>(this Component component) where T : Component => new(component);

        public static T ResolveIn<T>(this Component component, ref T field, HierarchyQuery<T> query) where T : Component
        {
            query ??= component.Query<T>().With(filter => filter.AllowOriginComponent = true); // confusing
            return field = query.First();
        }
    }
}