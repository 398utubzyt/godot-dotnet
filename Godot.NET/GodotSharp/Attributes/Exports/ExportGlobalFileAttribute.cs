using System;

namespace Godot
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ExportGlobalFileAttribute : ExportAttribute
    {
        public string[] Filters;
    }
}
