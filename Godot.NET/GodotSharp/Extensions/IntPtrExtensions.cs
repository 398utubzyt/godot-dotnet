using System;

namespace Godot
{
    internal static class IntPtrExtensions
    {
        [MImpl(MImplOpts.AggressiveInlining)]
        internal static unsafe int Utf8Size(this IntPtr self)
        {
            sbyte* chars = (sbyte*)self;
            int i = 0;
            while (*chars++ != 0)
                i++;
            return i;
        }
        [MImpl(MImplOpts.AggressiveInlining)]
        internal static unsafe void EncToString(this IntPtr self, System.Text.Encoding encoding, out string str)
        {
            // Seems simple enough...
            str = new string((sbyte*)self, 0, Utf8Size(self), encoding);
        }

        [MImpl(MImplOpts.AggressiveInlining)]
        internal static unsafe void Utf8ToString(this IntPtr self, out string str)
            => EncToString(self, System.Text.Encoding.UTF8, out str);
        [MImpl(MImplOpts.AggressiveInlining)]
        internal static unsafe void Utf16ToString(this IntPtr self, out string str)
            => EncToString(self, System.Text.Encoding.Unicode, out str);
        [MImpl(MImplOpts.AggressiveInlining)]
        internal static unsafe void Utf32ToString(this IntPtr self, out string str)
            => EncToString(self, System.Text.Encoding.UTF32, out str);

        [MImpl(MImplOpts.AggressiveInlining)]
        internal static unsafe RuntimeTypeHandle AsManagedTypeHandle(this IntPtr self)
            => RuntimeTypeHandle.FromIntPtr(self);
        [MImpl(MImplOpts.AggressiveInlining)]
        internal static unsafe Type AsManagedType(this IntPtr self)
            => Type.GetTypeFromHandle(RuntimeTypeHandle.FromIntPtr(self));

        [MImpl(MImplOpts.AggressiveInlining)]
        internal static unsafe RuntimeMethodHandle AsManagedMethodHandle(this IntPtr self)
            => RuntimeMethodHandle.FromIntPtr(self);
        [MImpl(MImplOpts.AggressiveInlining)]
        internal static unsafe void* AsManagedFunctionPointer(this IntPtr self)
            => (void*)RuntimeMethodHandle.FromIntPtr(self).GetFunctionPointer();
    }
}
