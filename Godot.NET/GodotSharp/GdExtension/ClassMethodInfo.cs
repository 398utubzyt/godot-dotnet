namespace Godot.GdExtension
{
    [SLayout(SLayoutOpt.Sequential)]
    internal struct ClassMethodInfo
    {
        public GDExtensionStringNamePtr Name;
        public unsafe void* MethodUserData;
        public unsafe delegate* unmanaged<void*, GDExtensionClassInstancePtr, GDExtensionConstVariantPtr*, GDExtensionInt, GDExtensionVariantPtr, CallError*, void> CallFunc;
        public unsafe delegate* unmanaged<void*, GDExtensionClassInstancePtr, GDExtensionConstTypePtr*, GDExtensionTypePtr, void> PtrCallFunc;
        public uint MethodFlags; // Bitfield of `GDExtensionClassMethodFlags`.

        /* If `has_return_value` is false, `return_value_info` and `return_value_metadata` are ignored. */
        public GDExtensionBool HasReturnValue;
        public unsafe PropertyInfo* ReturnValueInfo;
        public ClassMethodArgumentMetadata ReturnValueMetadata;

        /* Arguments: `arguments_info` and `arguments_metadata` are array of size `argument_count`.
         * Name and hint information for the argument can be omitted in release builds. Class name should always be present if it applies.
         */
        public uint ArgumentCount;
        public unsafe PropertyInfo* ArgumentsInfo;
        public unsafe ClassMethodArgumentMetadata* ArgumentsMetadata;

        /* Default arguments: `default_arguments` is an array of size `default_argument_count`. */
        public uint DefaultArgumentCount;
        public unsafe GDExtensionVariantPtr* DefaultArguments;
    }
}
