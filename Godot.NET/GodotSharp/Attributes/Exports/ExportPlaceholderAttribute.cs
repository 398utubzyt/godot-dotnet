using System;
using System.Linq;

namespace Godot
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ExportPlaceholderAttribute : ExportAttribute
    {
        public ExportPlaceholderAttribute() : base(PropertyHint.PlaceholderText, null)
        {
        }
        public ExportPlaceholderAttribute(string placeholder)
            : base(PropertyHint.PlaceholderText, placeholder)
        {
        }
    }
}
