using System;

namespace Godot
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ExportGlobalDirAttribute : ExportAttribute
    {
        public ExportGlobalDirAttribute() : base(PropertyHint.GlobalDir)
        {
        }
    }
}
