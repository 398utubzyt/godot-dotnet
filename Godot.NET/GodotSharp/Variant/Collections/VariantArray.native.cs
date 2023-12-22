// Putting this in a separate file to avoid clutter

// This is based on the native implementation of Godot's Array type
// See https://github.com/godotengine/godot/blob/master/core/variant/array.cpp for more info.

using System.ComponentModel;

namespace Godot.Collections
{
    partial struct VariantArray
    {
        internal unsafe readonly Variant* RawData { [MImpl(MImplOpts.AggressiveInlining)] get => _p != null ? _p->Array.CowData : null; }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [SLayout(SLayoutOpt.Sequential)]
        private unsafe struct ArrayPrivate
        {
            public uint RefCount;
            public PackedVector<Variant> Array;
            public Variant* ReadOnly;
            public ContainerTypeValidate Validate;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        private unsafe struct ContainerTypeValidate
        {
            public VariantType Type;
            public StringName ClassName;
            public nint script; // Type is Script
            public char* Where;
        }
    }
}
