using System;

namespace Godot
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ExportEnumAttribute : ExportAttribute
    {
        public ExportEnumAttribute(params string[] names)
            : base(PropertyHint.Enum, names != null ? string.Join(',', names) : null)
        {
        }
    }
}
