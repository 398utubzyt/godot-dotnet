using System;

namespace Godot
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ExportCategoryAttribute : Attribute
    {
        public string Name;

        public ExportCategoryAttribute(string name)
            => Name = name;
    }
}
