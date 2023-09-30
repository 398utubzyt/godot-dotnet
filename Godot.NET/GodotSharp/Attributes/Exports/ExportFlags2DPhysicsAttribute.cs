using System;

namespace Godot
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ExportFlags2DPhysicsAttribute : ExportAttribute
    {
        public ExportFlags2DPhysicsAttribute() : base(PropertyHint.Layers2dPhysics, null)
        {
        }
    }
}
