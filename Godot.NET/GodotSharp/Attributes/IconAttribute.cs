using System;

namespace Godot
{
    /// <summary>
    /// Associates a <see cref="Texture2D"/> resource with this class. This icon will be displayed in the editor alongside it.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class IconAttribute : Attribute
    {
        public string Path;

        /// <summary>Displays an icon in the editor with this class.</summary>
        /// <param name="path">A path to the <see cref="Texture2D"/> resource.</param>
        public IconAttribute(string path)
            => Path = path;
    }
}
