using System;

namespace Godot
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ExportGlobalFileAttribute : ExportAttribute
    {
        public ExportGlobalFileAttribute() : base(PropertyHint.GlobalFile, null)
        {
        }
        public ExportGlobalFileAttribute(params string[] filters)
            : base(PropertyHint.GlobalFile, filters != null ? string.Join(',', filters) : null)
        {
        }
    }
}
