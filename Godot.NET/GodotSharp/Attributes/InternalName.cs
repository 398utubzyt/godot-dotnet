using System;

namespace Godot
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    internal class InternalNameAttribute : Attribute
    {
        internal string Name;

        internal InternalNameAttribute(string name)
            => Name = name;
    }
}
