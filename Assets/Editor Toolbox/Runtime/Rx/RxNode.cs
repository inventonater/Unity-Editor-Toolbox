using System;
using System.Collections.Generic;
using R3;
using UnityEngine;

namespace Toolbox.RxTree
{
    public class RxTreeNode : MonoBehaviour, ILazy<RxTreeNode>
    {
        private RxRef<RxTreeNode> _parent;
        public RxRef<RxTreeNode> Parent => Ready().Ancestor(ref _parent);

        private IReadOnlyCollection<RxTreeNode> _children;
        public IReadOnlyCollection<RxTreeNode> Children => Ready().Children(ref _children);

        private bool _ready;

        public RxTreeNode Ready()
        {
            if (_ready) return this;
            _ready = true;
            // DoStuff();
            return this;
        }
    }

    public class RxConverter<TSource, TOutput> : ISubject<TOutput>, IDisposable
    {
        private readonly Observable<TSource> _source;
        private readonly IDisposable _sourceSubscription;
        private readonly ReactiveProperty<TOutput> _property = new();

        public RxConverter(Observable<TSource> source, Func<TSource, TOutput> converter)
        {
            _source = source;
            _sourceSubscription = source.Subscribe(n => OnNext(converter(n)), OnErrorResume, OnCompleted);
        }

        public void Dispose()
        {
            _property.Dispose();
            _sourceSubscription?.Dispose();
        }

        public IDisposable Subscribe(Observer<TOutput> observer) => _property.Subscribe(observer);
        public void OnNext(TOutput value) => _property.OnNext(value);
        public void OnErrorResume(Exception exception) => _property.OnErrorResume(exception);
        public void OnCompleted(Result result) => _property.OnCompleted(result);

        public TOutput Value => _property.Value;
        public bool HasObservers => _property.HasObservers;
        public bool IsCompleted => _property.IsCompleted;
        public bool IsDisposed => _property.IsDisposed;
        public bool IsCompletedOrDisposed => _property.IsCompletedOrDisposed;
    }

    public class RxConverterProperty<TSource, TOutput> : ReactiveProperty<TOutput>
    {
        private readonly IDisposable _sourceSubscription;

        public RxConverterProperty(Observable<TSource> source, Func<TSource, TOutput> converter)
        {
            _sourceSubscription = source.Subscribe(s => Value = converter.Invoke(s), OnErrorResume, OnCompleted);
        }

        protected override void DisposeCore() => _sourceSubscription?.Dispose();
    }

    public class SerializableRxConverterProperty<TSource, TOutput> : SerializableReactiveProperty<TOutput>
    {
        private readonly IDisposable _sourceSubscription;

        public SerializableRxConverterProperty(Observable<TSource> source, Func<TSource, TOutput> converter) =>
            _sourceSubscription = source.Subscribe(s => Value = converter.Invoke(s), OnErrorResume, OnCompleted);

        protected override void DisposeCore() => _sourceSubscription?.Dispose();
    }
}
