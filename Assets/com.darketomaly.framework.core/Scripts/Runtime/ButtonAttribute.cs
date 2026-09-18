using System;

namespace Framework
{
    [AttributeUsage(AttributeTargets.Method, Inherited = true)]
    public sealed class ButtonAttribute : Attribute
    {
        public string Label { get; }

        public ButtonAttribute(string label = null)
        {
            Label = label;
        }
    }
}
