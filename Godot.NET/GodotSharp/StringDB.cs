using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Godot
{
    internal static class StringDB
    {
        private static readonly BiHashMap<nint, string> _map = new BiHashMap<nint, string>();
        private static unsafe delegate* unmanaged[Cdecl]<nint, void> _dtor;

        public static string SearchOrCreate(nint native)
        {
            if (native == nint.Zero)
                return "null";

            if (!_map.TryGet(native, out string ret))
            {
                // Make new System.String if one does not exist for this Godot string.
                unsafe
                {
                    nuint len = (nuint)Main.i.StringToUtf16Chars((nint)(&native), null, long.MaxValue);
                    if (len < 2048)
                    {
                        char* nstr = stackalloc char[(int)(len + 1)];
                        Main.i.StringToUtf16Chars((nint)(&native), (ushort*)nstr, (long)len);
                        ret = MemUtil.ToString(nstr, len);
                    } else
                    {
                        char* nstr = MemUtil.Alloc<char>(len + 1);
                        Main.i.StringToUtf16Chars((nint)(&native), (ushort*)nstr, (long)len);
                        ret = MemUtil.ToString(nstr, len);
                        MemUtil.Free(nstr);
                    }
                }
                _map[native, native] = ret;
            }

            return ret;
        }

        public static nint Register(string managed)
        {
            if (_map.FindLeft(managed, (a, b) => a?.Equals(b, StringComparison.Ordinal) ?? (b == null), out nint native))
                return native;

            // Create a new Godot string and add it to the BiHashMap.
            unsafe
            {
                fixed (char* chars = managed)
                    Main.i.StringNewWithUtf16CharsAndLen((nint)(&native), (ushort*)chars, (long)managed.Length);
            }
            if (native == nint.Zero)
                throw new InsufficientMemoryException();

            _map[native, native] = managed;
            return native;
        }

        // Avoid memory allocations where possible
        public static void Placement(string managed, ref nint to)
        {
            if (MemUtil.IsNull(ref to))
                return;

            if (managed == null)
            {
                to = 0;
                return;
            }

            unsafe
            {
                fixed (char* chars = managed)
                    Main.i.StringNewWithUtf16CharsAndLen(MemUtil.RefAsInt(ref to), (ushort*)chars, (long)managed.Length);
            }
        }

        public unsafe static void Unregister(string managed)
        {
            if (_map.FindLeft(managed, (a, b) => a?.Equals(b, StringComparison.Ordinal) ?? b == null, out nint native))
            {
                if (_dtor == null)
                    _dtor = (delegate* unmanaged[Cdecl]<nint, void>)Main.i.VariantGetPtrDestructor(VariantType.String);
                _dtor((nint)(&native));
                _map.Remove(native);
            }
        }
    }
}
