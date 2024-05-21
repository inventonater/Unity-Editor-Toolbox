using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Toolbox
{
    public interface IQueryAlgorithm<T> where T : Component
    {
        IEnumerable<T> Traverse(Transform origin, IQueryFilter<T> queryFilter);
    }

    public class EmptyQueryAlgorithm<T> : IQueryAlgorithm<T> where T : Component
    {
        public static EmptyQueryAlgorithm<T> Empty { get; } = new();
        public IEnumerable<T> Traverse(Transform origin, IQueryFilter<T> queryFilter) => Enumerable.Empty<T>();
    }

    public class Query<T> : IEnumerable<T> where T : Component
    {
        private Component Origin { get; }
        private IQueryAlgorithm<T> Algorithm { get; set; }
        private QueryFilter<T> Filter { get; set; }

        public Query(Component origin, IQueryAlgorithm<T> queryAlgorithm = null, QueryFilter<T> filter = null)
        {
            Origin = origin;
            Algorithm = queryAlgorithm ?? EmptyQueryAlgorithm<T>.Empty;
            Filter = filter ?? new QueryFilter<T>();
        }

        public Query<T> With(IQueryAlgorithm<T> queryAlgorithm)
        {
            Algorithm = queryAlgorithm;
            return this;
        }

        public Query<T> With(Action<QueryFilter<T>> action)
        {
            action(Filter);
            return this;
        }

        public Query<T> With(QueryFilter<T> filer)
        {
            Filter = filer;
            return this;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public IEnumerator<T> GetEnumerator() => SelectEnumerable().GetEnumerator();
        private IEnumerable<T> SelectEnumerable() => Algorithm.Traverse(Origin.transform, Filter);
    }
}
