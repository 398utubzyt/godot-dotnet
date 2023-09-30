using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Godot
{
    internal static class CStringDB
    {
        private static readonly BiHashMap<nint, string> _map = new BiHashMap<nint, string>();

        public static string SearchOrCreate(nint native)
        {
            if (native == nint.Zero)
                return "null";

            if (!_map.TryGet(native, out string ret))
            {
                // Make new System.String if one does not exist for this C string.
                native.Utf8ToString(out ret);
                _map[native, native] = ret;
            }

            return ret;
        }

        public static nint Register(string managed)
        {
            if (_map.FindLeft(managed, (a, b) => a?.Equals(b, StringComparison.Ordinal) ?? (b == null), out nint native))
                return native;

            // Create a new C string and add it to the BiHashMap.
            // TODO: Support UTF8 encoding
            native = MemUtil.IntAlloc(managed.BufferSize());
            if (native == nint.Zero)
                throw new InsufficientMemoryException();
            unsafe { managed.ToBytePtr((byte*)native); }

            _map[native, native] = managed;
            GC.KeepAlive(managed);
            return native;
        }

        public static void Unregister(string str)
        {
            // We can't really track the lifetime of every string.
            // Plus, StringName (as of writing) requires this pointer to exist
            // for the entire lifetime of the program.
            // if (_map.FindLeft(str, (a, b) => a?.Equals(b, StringComparison.Ordinal) ?? b == null, out nint name))
            //    MemUtil.Free(name);
        }
    }
}
