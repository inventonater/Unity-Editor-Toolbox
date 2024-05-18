using System;
using System.Linq;
using R3;
using UnityEngine;

namespace Toolbox
{
    [AttributeUsage(AttributeTargets.Property)]
    public class FindInParentAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class FindInChildrenAttribute : Attribute
    {
    }

    public class Sample : MonoBehaviour, ILazy<Sample>
    {
        private RxValue<Vector3> _position;
        public RxValue<Vector3> Position => new(() => transform.position, v => transform.position = v);

        private RxRef<Canvas> _parentCanvas;
        public RxRef<Canvas> ParentCanvas => I.Parent(ref _parentCanvas);

        private RxRef<Transform> _reallyComplex;
        public RxRef<Transform> SuperComplex => _reallyComplex ??= this.QueryDescendants<Transform>()
                        .Where(t => t.position.y > 0.5f)
                        .OrderBy(t => t.position.z)
                        .FirstOrDefault(t => t.forward.Dot(ParentCanvas.Value.transform.forward) > 0).Rx();

        void Start()
        {
            Position.Subscribe(WhenPositionChanged);
            ParentCanvas.Select(c => c.GetComponent<Transform>()).Subscribe(UpdateParentCanvasTransform);
            if (SuperComplex.HasObservers)
            {
                float HeightDelta(Transform complex, Canvas parent) => complex.transform.position.y - parent.transform.position.y;
                SuperComplex.CombineLatest(ParentCanvas, HeightDelta).Subscribe(heightDelta => Debug.Log($"Delta Changed: {heightDelta}"));
            }

            var rxValue = Position;
            var calledHeya = CalledHeya;
        }

        public Observer<Transform> UpdateParentCanvasTransform { get; set; }

        private RxRef<Canvas> _childCanvas;
        public RxRef<Canvas> ChildCanvas => this.Child(ref _childCanvas);


        private RxRef<Transform> _calledHeya;
        public RxRef<Transform> CalledHeya => this.Scene(ref _calledHeya, filter: c => c.name == "Heya");


        public Observer<Vector3> WhenPositionChanged { get; set; }
        public Observer<Canvas> WhenParentCanvasChanged { get; set; }

        public Sample _parent1;
        public Sample _child1;
        public Sample _childRef1;
        public Transform _heya1;

        private Sample I => Ready();
        private bool _init;
        public Sample Ready()
        {
            if(_init) return this;
            _init = true;
            DoStuff();
            return this;
        }


        private void DoStuff()
        {

        }

        void ParentChanged(Sample t)
        {
        }

        // private void Start()
        // {
        //     _parent1 = ParentCanvas.Value;
        //     ParentCanvas.Subscribe(ParentChanged);
        //     _child1 = ChildPanel.Value;
        //     _childRef1 = ChildRef.Value;
        //     _heya1 = ReallyComplex.Value;
        // }
    }
}
