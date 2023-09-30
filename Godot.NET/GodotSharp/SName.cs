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

                /*
                // **I DON'T LIKE THIS**
                // Potentially 3 allocations could be made here.
                // This branch should be avoided at all costs until getting the
                // string pointer directly is supported by the API.
                unsafe
                {
                    if (_strCtor == null)
                        _strCtor = (delegate* unmanaged[Cdecl]<nint, nint*, void>)Main.i.VariantGetPtrConstructor(VariantType.String, 2);
                    nint native;
                    _strCtor((nint)(&native), (nint*)&name);

                    ret = StringDB.SearchOrCreate(native);
                }
                */

                Console.WriteLine("SearchOrCreate new StringName");
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
            if (name.IsNull)
                throw new Exception("WHY");
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
