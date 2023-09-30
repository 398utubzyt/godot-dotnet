using System;

namespace Godot
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ExportGroupAttribute : Attribute
    {
        public string Name;
        public string Prefix;

        public ExportGroupAttribute(string name)
            => Name = name;
        public ExportGroupAttribute(string name, string prefix)
        {
            Name = name;
            Prefix = prefix;
        }
    }
}
