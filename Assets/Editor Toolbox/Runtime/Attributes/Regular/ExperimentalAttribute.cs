using System;
using System.Diagnostics;

namespace UnityEngine
{
    /// <summary>
    /// Draws a information box if the associated value is experimental.
    /// 
    /// <para>Supported types: any <see cref="Object"/>.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    [Conditional("UNITY_EDITOR")]
    public class ExperimentalAttribute : PropertyAttribute
    {
        public ExperimentalAttribute() : this("This class is experimental and may not be in a mature state.")
        { }

        public ExperimentalAttribute(string note)
        {
            Note = note;
        }

        public string Note { get; private set; }
    }
}