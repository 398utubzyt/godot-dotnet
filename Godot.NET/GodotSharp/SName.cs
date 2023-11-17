using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Godot
{
    internal static class SName
    {
        private static readonly BiHashMap<StringName, string> _map = new BiHashMap<StringName, string>();
        private static unsafe delegate* unmanaged[Cdecl]<nint, nint*, void> _strCtor;

        public static string SearchOrCreate(StringName name)
        {
            if (name.IsNull)
                return "null";

            if (!_map.TryGet((nint)name, out string ret))
            {
                // Make new System.String if one does not exist for this StringName.
                unsafe { ret = ((StringName._Data*)(nint)name)->ToString(); }
                _map[(nint)name, name] = ret;
            }

            return ret;
        }

        public static StringName Register(string str)
        {
            if (_map.FindLeft(str, (a, b) => a?.Equals(b, StringComparison.Ordinal) ?? (b == null), out StringName name))
                return name;

            // Create a new StringName and add it to the BiHashMap.
            unsafe { Main.i.StringNameNewWithLatin1Chars((nint)(&name), (byte*)CStringDB.Register(str), 1); }
            _map[(nint)name, name] = str;
            return name;
        }

        /* Never delete StringName
        public static void Unregister(string str)
        {
            if (_map.FindLeft(str, (a, b) => a?.Equals(b, StringComparison.Ordinal) ?? b == null, out StringName name))
                MemUtil.Free(name);
        }
        */
    }
}
