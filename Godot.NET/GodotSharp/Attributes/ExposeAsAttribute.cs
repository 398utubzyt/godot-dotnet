using System;

namespace Godot
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    internal class ExposeAsAttribute : Attribute
    {
        public string Name;
    }
}
