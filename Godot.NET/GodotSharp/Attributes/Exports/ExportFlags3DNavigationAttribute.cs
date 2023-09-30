using System;

namespace Godot
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ExportFlags3DNavigationAttribute : ExportAttribute
    {
        public ExportFlags3DNavigationAttribute() : base(PropertyHint.Layers3dNavigation, null)
        {
        }
    }
}
