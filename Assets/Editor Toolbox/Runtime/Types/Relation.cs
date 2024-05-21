using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Toolbox
{
    /// <summary>
    /// Represents a single, specific relationship between two components.
    /// Useful when you need to specify or check for a specific relationship type.
    /// </summary>
    public enum Relation
    {
        None,
        Sibling,
        Parent,
        Ancestor,
        Child,
        Descendant,
        Scene,
    }

    /// <summary>
    /// Allows you to combine multiple Relation types using bitwise operations.
    /// Useful when you need to specify or check for multiple relationship types simultaneously, such as in search algorithms.
    /// </summary>
    [Flags]
    public enum RelationFlags
    {
        None = 1 << Relation.None,
        Sibling = 1 << Relation.Sibling,
        Parent = 1 << Relation.Parent,
        Ancestor = 1 << Relation.Ancestor,
        Child = 1 << Relation.Child,
        Descendant = 1 << Relation.Descendant,
        Scene = 1 << Relation.Scene,

        SiblingParent = Sibling | Parent,
        SiblingChild = Sibling | Child,
        SiblingDescendant = Sibling | Descendant,
        SiblingAncestor = Sibling | Ancestor,
    }

    public static class RelationExtensions
    {
        public static IEnumerable<Relation> ToRelations(this RelationFlags flags) => flags.FlagsToEnumValues<RelationFlags, Relation>();

        // public static RelationFlags ToRelationFlags(this Relation relation) => (RelationFlags)(1 << (int)relation);
        public static RelationFlags ToRelationFlags(this Relation relation) => relation.EnumValueToFlag<Relation, RelationFlags>();
        public static RelationFlags ToRelationFlags(this IEnumerable<Relation> relations) => relations.EnumValuesToFlags<Relation, RelationFlags>();

        public static bool IsNone(this Relation f) => f == Relation.None;
        public static bool IsSibling(this Relation f) => f == Relation.Sibling;
        public static bool IsParent(this Relation f) => f == Relation.Parent;
        public static bool IsChildren(this Relation f) => f == Relation.Child;
        public static bool IsDescendant(this Relation f) => f == Relation.Descendant;
        public static bool IsAncestor(this Relation f) => f == Relation.Ancestor;
        public static bool IsScene(this Relation f) => f == Relation.Scene;


        public static bool IsNone(this RelationFlags f) => f == RelationFlags.None;
        public static bool IncludeSibling(this RelationFlags f) => (f & RelationFlags.Sibling) != 0;
        public static bool IncludeParent(this RelationFlags f) => (f & RelationFlags.Parent) != 0;
        public static bool IncludeChildren(this RelationFlags f) => (f & RelationFlags.Child) != 0;
        public static bool IncludeDescendant(this RelationFlags f) => (f & RelationFlags.Descendant) != 0;
        public static bool IncludeAncestor(this RelationFlags f) => (f & RelationFlags.Ancestor) != 0;
        public static bool IncludeScene(this RelationFlags f) => (f & RelationFlags.Scene) != 0;

        public static bool IsRelated(this Component a, Component b, Relation relation) => IsRelated(a, b, relation.ToRelationFlags());

        public static bool IsRelated(this Component a, Component b, RelationFlags f)
        {
            if (f.IncludeSibling() && a.gameObject == b.gameObject) return true;
            if (f.IncludeParent() && a.transform.parent == b.gameObject.transform) return true;
            if (f.IncludeChildren() && b.transform.parent == a.gameObject.transform) return true;
            if (f.IncludeDescendant() && b.transform.IsChildOf(a.gameObject.transform)) return true;
            if (f.IncludeAncestor() && a.transform.IsChildOf(b.gameObject.transform)) return true;
            if (f.IncludeScene()) return true;
            return false;
        }

        public static IEnumerable<IQueryAlgorithm<T>> ToQueryAlgorithm<T>(this RelationFlags flags) where T : Component
        {
            return flags.ToRelations().Select(relation => relation.ToQueryAlgorithm<T>());
        }

        public static IQueryAlgorithm<T> ToQueryAlgorithm<T>(this Relation relation) where T : Component
        {
            return RelationToQueryAlgorithmMapping<T>.Get(relation);
        }

        private static class RelationToQueryAlgorithmMapping<T> where T : Component
        {
            public static IQueryAlgorithm<T> Get(Relation relation)
            {
                if (RelationToSearchFunc.TryGetValue(relation, out var result)) return result;
                Debug.LogError($"No Search method found for {relation}");
                return EmptyQueryAlgorithm<T>.Empty;
            }

            private static readonly Dictionary<Relation, IQueryAlgorithm<T>> RelationToSearchFunc =
                new()
                {
                    { Relation.None, EmptyQueryAlgorithm<T>.Empty },
                    { Relation.Sibling, new HierarchyQueryAlgorithm<T>.SameGameObject() },
                    { Relation.Parent, new HierarchyQueryAlgorithm<T>.ImmediateAncestor() },
                    { Relation.Ancestor, new HierarchyQueryAlgorithm<T>.Ancestors() },
                    { Relation.Child, new HierarchyQueryAlgorithm<T>.ImmediateDescendants() },
                    { Relation.Descendant, new HierarchyQueryAlgorithm<T>.DescendantsBreadthFirst() },
                    { Relation.Scene, new HierarchyQueryAlgorithm<T>.EntireScene() }
                };
        }
    }
}
