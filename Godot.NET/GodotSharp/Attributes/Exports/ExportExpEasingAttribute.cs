using System;

namespace Godot
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ExportExpEasingAttribute : ExportAttribute
    {
        public ExportExpEasingAttribute(bool attenuation = false, bool positiveOnly = false)
            : base(PropertyHint.ExpEasing, 
                  attenuation ? (positiveOnly ? "attenuation,positive_only" : "attenuation") : (positiveOnly ? "positive_only" : null))
        {
        }
    }
}
