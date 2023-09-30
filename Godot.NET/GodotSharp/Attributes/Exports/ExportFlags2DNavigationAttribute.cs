using System;

namespace Godot
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ExportFlags2DNavigationAttribute : ExportAttribute
    {
        public ExportFlags2DNavigationAttribute() : base(PropertyHint.Layers2dNavigation, null)
        {
        }
    }
}
