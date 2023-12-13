using System;

namespace Godot
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ExportNodePathAttribute : ExportAttribute
    {
        /// <summary>
        /// Specifies the types that are allowed to be assigned to this property. This should specify Godot class names.
        /// </summary>
        /// <remarks>
        /// Also see
        /// <seealso href="https://docs.godotengine.org/en/stable/classes/class_%40gdscript.html#class-gdscript-annotation-export-node-path">@export_node_path</seealso>
        /// for more info.
        /// </remarks>
        public string[] Types;
    }
}
