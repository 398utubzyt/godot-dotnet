using System.Runtime.CompilerServices;

namespace Godot.GdExtension
{
    internal unsafe static class GlobalSingletons
    {
        public static nint GetSingleton(StringName name)
            => Main.i.GlobalGetSingleton((nint)name);
    }
}
