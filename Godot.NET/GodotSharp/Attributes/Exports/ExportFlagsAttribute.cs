using System;

namespace Godot
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ExportFlagsAttribute : Attribute
    {
        public string[] Names;
    }
}
