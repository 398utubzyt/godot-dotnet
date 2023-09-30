using System;

namespace Godot
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ExportFlags3DPhysicsAttribute : ExportAttribute
    {
        public ExportFlags3DPhysicsAttribute() : base(PropertyHint.Layers3dPhysics, null)
        {
        }
    }
}
