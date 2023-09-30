using System;

namespace Godot
{
    /// <summary>
    /// Tells Godot what classes are exposed in this assembly. Automatically applied by Roslyn source generators.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false, Inherited = false)]
    public class AssemblyHasClassesAttribute : Attribute
    {
        public Type[] Types;

        public AssemblyHasClassesAttribute(params Type[] types)
            => Types = types;
    }
}
