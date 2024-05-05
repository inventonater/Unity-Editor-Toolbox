using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Toolbox
{
    public class HierarchyTest : MonoBehaviour
    {
        public Component A;
        public Component B;
        public Component C;

        [SerializeField] private int _inspectorOrderCompare;
        [SerializeField] private int _breadthFirstCompare;

        [SerializeField] private int _aDepth;
        [SerializeField] private int _bDepth;

        [SerializeField] private Component _shallowest;
        [SerializeField] private Component _deepest;
        [SerializeField] private Component _renderFirst;
        [SerializeField] private Component _renderLast;

        [SerializeField] private Collider _collider;

        private void Update()
        {
            _collider = null;

            this.Descendant(ref _collider);

            if (!A || !B || !C) return;

            _inspectorOrderCompare = Hierarchy.CompareInspectorOrder(A.transform, B.transform);
            _breadthFirstCompare = Hierarchy.CompareBreadthFirst(A.transform, B.transform);

            _shallowest = Hierarchy.Sort(A, B, HierarchySearchOrder.FirstInHierarchy);
            _deepest = Hierarchy.Sort(A, B, HierarchySearchOrder.LastInHierarchy);

            _renderFirst = Hierarchy.Sort(A, B, HierarchySearchOrder.RenderFirst);
            _renderLast = Hierarchy.Sort(A, B, HierarchySearchOrder.RenderLast);

            _aDepth = Hierarchy.GetDepth(A);
            _bDepth = Hierarchy.GetDepth(B);
        }
    }
}