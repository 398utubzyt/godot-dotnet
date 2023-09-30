using System;

namespace Godot
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ExportFlagsAvoidanceAttribute : ExportAttribute
    {
        public ExportFlagsAvoidanceAttribute() : base(PropertyHint.LayersAvoidance, null)
        {
        }
    }
}
