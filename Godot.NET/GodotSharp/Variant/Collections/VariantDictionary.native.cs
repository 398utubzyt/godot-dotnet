// Putting this in a separate file to avoid clutter

// This is based on the native implementation of Godot's Dictionary type
// See https://github.com/godotengine/godot/blob/master/core/variant/dictionary.cpp for more info.

using System.ComponentModel;

namespace Godot.Collections
{
    partial struct VariantDictionary
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        [SLayout(SLayoutOpt.Sequential)]
        private unsafe struct DictionaryPrivate
        {
            public uint RefCount;
            public Variant* ReadOnly;
            private fixed byte _hashmap[48]; // Maybe I should implement this at some point...
        }
    }
}
