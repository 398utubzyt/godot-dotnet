using System;

namespace Godot
{
    [AttributeUsage(AttributeTargets.GenericParameter, AllowMultiple = false, Inherited = true)]
    public class MustBeVariantAttribute : Attribute
    {
    }
}
