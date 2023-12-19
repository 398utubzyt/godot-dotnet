// Putting this in a separate file to avoid clutter

// This is based on the native implementation of Godot's Array type
// See https://github.com/godotengine/godot/blob/master/core/variant/array.cpp for more info.

namespace Godot.Collections
{
    partial struct VariantArray
    {
        [SLayout(SLayoutOpt.Sequential)]
        private unsafe struct ArrayPrivate
        {
            public uint RefCount;
            public ArrayVector Array;
            public Variant* ReadOnly;
            public ContainerTypeValidate Validate;
        }

        [SLayout(SLayoutOpt.Sequential)]
        private unsafe struct ArrayVector
        {
            private readonly nint _write; // Basically padding
            public Variant* CowData; // Internally it's just a pointer

            // whar the heck
            public readonly int Size { [MImpl(MImplOpts.AggressiveInlining)] get => CowData != null ? *((int*)CowData - 1) : 0; }
        }

        private unsafe struct ContainerTypeValidate
        {
            public VariantType Type;
            public StringName ClassName;
            public nint script; // Type is Script
            public char* Where;
        }
    }
}
