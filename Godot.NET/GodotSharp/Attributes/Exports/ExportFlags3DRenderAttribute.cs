using System;

namespace Godot
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ExportFlags3DRenderAttribute : ExportAttribute
    {
        public ExportFlags3DRenderAttribute() : base(PropertyHint.Layers3dRender, null)
        {
        }
    }
}
