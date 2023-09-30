namespace Godot
{
    public static class ByteExtensions
    {
        [MImpl(MImplOpts.AggressiveInlining)]
        internal static bool ToBool(this GDExtensionBool self)
            => self != 0;
        [MImpl(MImplOpts.AggressiveInlining)]
        internal static GDExtensionBool ToExtBool(this bool self)
            => self ? (GDExtensionBool)1 : (GDExtensionBool)0;

        [MImpl(MImplOpts.AggressiveInlining)]
        public static string AsHexString(this byte self)
            => $"{self:XX}";
    }
}
