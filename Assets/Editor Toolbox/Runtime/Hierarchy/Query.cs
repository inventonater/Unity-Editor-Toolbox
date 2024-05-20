using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolbox
{
    public class Query<T> : IEnumerable<T> where T : Component
    {
        private Component Origin { get; }
        private IQueryAlgorithm<T> Algorithm { get; set; }
        private QueryFilter<T> Filter { get; set; } = new();

        public Query(Component origin, IQueryAlgorithm<T> queryAlgorithm)
        {
            Origin = origin;
            Algorithm = queryAlgorithm;
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
        private IEnumerable<T> SelectEnumerable() => Algorithm.Traverse(this.Origin.transform, Filter);
    }
}