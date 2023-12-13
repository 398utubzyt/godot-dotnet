using System;

namespace Godot
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ExportAttribute : Attribute
    {
        public PropertyHint Hint;
        public string HintString;
    }
}
