using System;

namespace Godot.GdExtension
{
    [SLayout(SLayoutOpt.Sequential)]
    internal struct GodotVersion
    {
        public uint Major;
        public uint Minor;
        public uint Patch;
        public unsafe byte* String;
    }
}
