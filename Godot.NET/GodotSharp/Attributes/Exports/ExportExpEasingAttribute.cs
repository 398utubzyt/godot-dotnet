using System;

namespace Godot
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ExportExpEasingAttribute : Attribute
    {
        public bool Attenuation;
        public bool PositiveOnly;
    }
}
