using System;
using System.Linq;

namespace Godot
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ExportPlaceholderAttribute : ExportAttribute
    {
        /// <summary>
        /// The placeholder text to show for this string property.
        /// </summary>
        public string Placeholder;
    }
}
