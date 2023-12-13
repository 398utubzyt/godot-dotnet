using System;

namespace Godot
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ExportEnumAttribute : Attribute
    {
        /// <summary>
        /// Should be used for properties of integer or string types which should function as if it were an enum type.
        /// </summary>
        /// <remarks>
        /// Also see
        /// <seealso href="https://docs.godotengine.org/en/stable/classes/class_%40gdscript.html#class-gdscript-annotation-export-enum">@export_enum</seealso>
        /// for more info.
        /// </remarks>
        public string[] Names;
    }
}
