using System;

namespace Godot
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ExportFlags2DRenderAttribute : ExportAttribute
    {
        public ExportFlags2DRenderAttribute() : base(PropertyHint.Layers2dRender, null)
        {
        }
    }
}
