namespace Godot.GdExtension
{
    [SLayout(SLayoutOpt.Sequential)]
    internal struct MethodInfo
    {
        public GDExtensionStringNamePtr Name;
        public PropertyInfo ReturnValue;
        public uint Flags; // Bitfield of `GDExtensionClassMethodFlags`.
        public int Id;

        public uint ArgumentCount;
        public unsafe PropertyInfo* Arguments;

        public uint DefaultArgumentCount;
        public unsafe GDExtensionVariantPtr* DefaultArguments;
    }
}
