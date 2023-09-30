using System;

namespace Godot
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ExportFlagsAttribute : ExportAttribute
    {
        public ExportFlagsAttribute() : base(PropertyHint.Flags, null)
        {
        }
        public ExportFlagsAttribute(params string[] names)
            : base(PropertyHint.Flags, names != null ? string.Join(',', names) : null)
        {
        }
    }
}
