using System;

namespace Godot
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ExportFileAttribute : ExportAttribute
    {
        public ExportFileAttribute() : base(PropertyHint.File, null)
        {
        }
        public ExportFileAttribute(params string[] filters)
            : base(PropertyHint.File, filters != null ? string.Join(',', filters) : null)
        {
        }
    }
}
