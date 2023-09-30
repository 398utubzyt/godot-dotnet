using System;

namespace Godot
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ExportAttribute : Attribute
    {
        public PropertyHint Hint;
        public string HintString;

        public ExportAttribute()
        {
        }
        public ExportAttribute(PropertyHint hint)
        {
            Hint = hint;
        }
        public ExportAttribute(PropertyHint hint, string hintString)
        {
            Hint = hint;
            HintString = hintString;
        }
    }
}
