namespace Godot.GdExtension
{
    [SLayout(SLayoutOpt.Sequential)]
    internal struct PropertyInfo
    {
        public VariantType Type;
        public GDExtensionStringNamePtr Name;
        public GDExtensionStringNamePtr ClassName;
        public uint Hint; // Bitfield of `PropertyHint` (defined in `extension_api.json`).
        public GDExtensionStringPtr HintString;
        public uint Usage; // Bitfield of `PropertyUsageFlags` (defined in `extension_api.json`).
    }
}
