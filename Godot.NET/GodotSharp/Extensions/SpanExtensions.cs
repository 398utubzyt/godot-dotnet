using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Godot
{
    internal static class SpanExtensions
    {
        [MImpl(MImplOpts.AggressiveInlining)]
        internal static ref T GetRef<T>(this Span<T> self)
            => ref MemoryMarshal.GetReference(self);
        [MImpl(MImplOpts.AggressiveInlining)]
        internal static ref T GetRef<T>(this ReadOnlySpan<T> self)
            => ref MemoryMarshal.GetReference(self);
    }
}
