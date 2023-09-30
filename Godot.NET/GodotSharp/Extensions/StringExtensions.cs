using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Godot
{
    internal static class StringExtensions
    {
        [MImpl(MImplOpts.AggressiveInlining)]
        internal static nuint BufferSize(this string self)
            => ((nuint)self.Length + 1);

        [MImpl(MImplOpts.AggressiveInlining)]
        internal static unsafe void ToBytePtr(this string self, byte* ptr)
        {
            // No size checking, make sure `ptr` size is correct!!
            for (int i = 0; i < self.Length; i++)
                ptr[i] = (byte)self[i];
            ptr[self.Length] = 0;
        }
        [MImpl(MImplOpts.AggressiveInlining)]
        internal static unsafe void ToBytePtr(this string self, byte* ptr, nint length)
        {
            for (nint i = 0; i < length; i++)
                ptr[i] = (byte)self[(int)i];
            ptr[length] = 0;
        }
    }
}
