using System;

namespace Godot
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ExportMultilineAttribute : ExportAttribute
    {
        public ExportMultilineAttribute() : base(PropertyHint.MultilineText)
        {
        }
    }
}
