using System;

namespace Godot
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ExportColorNoAlphaAttribute : ExportAttribute
    {
        public ExportColorNoAlphaAttribute() : base(PropertyHint.ColorNoAlpha)
        {
        }
    }
}
