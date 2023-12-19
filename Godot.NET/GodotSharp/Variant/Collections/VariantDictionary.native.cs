// Putting this in a separate file to avoid clutter

// This is based on the native implementation of Godot's Dictionary type
// See https://github.com/godotengine/godot/blob/master/core/variant/dictionary.cpp for more info.

namespace Godot.Collections
{
    partial struct VariantDictionary
    {
        [SLayout(SLayoutOpt.Sequential)]
        private unsafe struct DictionaryPrivate
        {
            public uint RefCount;
            public Variant* ReadOnly;
            private fixed byte _hashmap[48];
        }
    }
}
