using System;
using UnityEngine;

namespace Framework
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class ButtonAttribute : PropertyAttribute
    {
        public string Label { get; }
        public ButtonAttribute(string label = null) => Label = label;
    }
}