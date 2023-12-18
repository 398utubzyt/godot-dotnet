using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Godot.GdExtension
{
    internal static unsafe class MemUtil
    {
        [MImpl(MImplOpts.AggressiveInlining)]
        public static void* Alloc(nuint size)
            => NativeMemory.Alloc(size);
        [MImpl(MImplOpts.AggressiveInlining)]
        public static T* Alloc<T>(nuint count) where T : unmanaged
            => (T*)NativeMemory.Alloc(count * (nuint)sizeof(T));
        [MImpl(MImplOpts.AggressiveInlining)]
        public static ref T RefAlloc<T>(nuint count)
            => ref Unsafe.AsRef<T>(NativeMemory.Alloc(count * (nuint)Unsafe.SizeOf<T>()));
        [MImpl(MImplOpts.AggressiveInlining)]
        public static nint IntAlloc(nuint size)
            => (nint)NativeMemory.Alloc(size);

        [MImpl(MImplOpts.AggressiveInlining)]
        public static void* AllocZero(nuint size)
            => NativeMemory.AllocZeroed(size);
        [MImpl(MImplOpts.AggressiveInlining)]
        public static T* AllocZero<T>(nuint count) where T : unmanaged
            => (T*)NativeMemory.AllocZeroed(count * (nuint)sizeof(T));
        [MImpl(MImplOpts.AggressiveInlining)]
        public static ref T RefAllocZero<T>(nuint count)
            => ref Unsafe.AsRef<T>(NativeMemory.AllocZeroed(count * (nuint)Unsafe.SizeOf<T>()));
        [MImpl(MImplOpts.AggressiveInlining)]
        public static nint IntAllocZero(nuint size)
            => (nint)NativeMemory.AllocZeroed(size);

        [MImpl(MImplOpts.AggressiveInlining)]
        public static void* Realloc(void* block, nuint size)
            => NativeMemory.Realloc(block, size);
        [MImpl(MImplOpts.AggressiveInlining)]
        public static T* Realloc<T>(T* block, nuint count) where T : unmanaged
            => (T*)NativeMemory.Realloc(block, count * (nuint)sizeof(T));
        [MImpl(MImplOpts.AggressiveInlining)]
        public static ref T Realloc<T>(ref T block, nuint count)
            => ref Unsafe.AsRef<T>(NativeMemory.Realloc(Unsafe.AsPointer(ref block), count * (nuint)Unsafe.SizeOf<T>()));
        [MImpl(MImplOpts.AggressiveInlining)]
        public static nint Realloc(nint block, nuint size)
            => (nint)NativeMemory.Realloc((void*)block, size);

        [MImpl(MImplOpts.AggressiveInlining)]
        public static void Free(void* block)
            => NativeMemory.Free(block);
        [MImpl(MImplOpts.AggressiveInlining)]
        public static void Free<T>(T* block) where T : unmanaged
            => NativeMemory.Free(block);
        [MImpl(MImplOpts.AggressiveInlining)]
        public static void Free<T>(ref T block)
            => NativeMemory.Free(Unsafe.AsPointer(ref block));
        [MImpl(MImplOpts.AggressiveInlining)]
        public static void Free(nint block)
            => NativeMemory.Free((void*)block);

        [MImpl(MImplOpts.AggressiveInlining)]
        public static void Move(void* from, void* to, nuint size)
            => NativeMemory.Copy(from, to, size);
        [MImpl(MImplOpts.AggressiveInlining)]
        public static void Move<T>(T* from, T* to, nuint count) where T : unmanaged
            => NativeMemory.Copy(from, to, count * (nuint)sizeof(T));
        [MImpl(MImplOpts.AggressiveInlining)]
        public static void Move<T>(ref T from, ref T to, nuint count)
            => NativeMemory.Copy(Unsafe.AsPointer(ref from), Unsafe.AsPointer(ref to), count);
        [MImpl(MImplOpts.AggressiveInlining)]
        public static void Move(nint from, nint to, nuint size)
            => NativeMemory.Copy((void*)from, (void*)to, size);

        [MImpl(MImplOpts.AggressiveInlining)]
        public static void Zero(void* from, nuint size)
            => NativeMemory.Clear(from, size);
        [MImpl(MImplOpts.AggressiveInlining)]
        public static void Zero<T>(T* from, nuint count) where T : unmanaged
            => NativeMemory.Clear(from, count * (nuint)sizeof(T));
        [MImpl(MImplOpts.AggressiveInlining)]
        public static void Zero<T>(ref T from, nuint count)
            => NativeMemory.Clear(Unsafe.AsPointer(ref from), count * (nuint)Unsafe.SizeOf<T>());
        [MImpl(MImplOpts.AggressiveInlining)]
        public static void Zero(nint from, nuint size)
            => NativeMemory.Clear((void*)from, size);

        [MImpl(MImplOpts.AggressiveInlining)]
        public static Span<T> AsSpan<T>(void* from, nuint count) where T : unmanaged
            => MemoryMarshal.CreateSpan(ref *(T*)from, (int)count);
        [MImpl(MImplOpts.AggressiveInlining)]
        public static Span<T> AsSpan<T>(T* from, nuint count) where T : unmanaged
            => MemoryMarshal.CreateSpan(ref *from, (int)count);
        [MImpl(MImplOpts.AggressiveInlining)]
        public static Span<T> AsSpan<T>(ref T from, nuint count)
            => MemoryMarshal.CreateSpan(ref from, (int)count);

        [MImpl(MImplOpts.AggressiveInlining)]
        public static ReadOnlySpan<T> AsConstSpan<T>(T* from, nuint count) where T : unmanaged
            => MemoryMarshal.CreateReadOnlySpan(ref *from, (int)count);
        [MImpl(MImplOpts.AggressiveInlining)]
        public static ReadOnlySpan<T> AsConstSpan<T>(ref T from, nuint count)
            => MemoryMarshal.CreateReadOnlySpan(ref from, (int)count);

        [MImpl(MImplOpts.AggressiveInlining)]
        public static string ToString(byte* from, nuint count)
            => new string((sbyte*)from, 0, (int)count);
        [MImpl(MImplOpts.AggressiveInlining)]
        public static string ToString(char* from, nuint count)
            => new string(AsConstSpan(from, count));
        [MImpl(MImplOpts.AggressiveInlining)]
        public static string ToString(ref byte from, nuint count)
            => new string((sbyte*)from, 0, (int)count);
        [MImpl(MImplOpts.AggressiveInlining)]
        public static string ToString(ref char from, nuint count)
            => new string(AsConstSpan(ref from, count));

        [MImpl(MImplOpts.AggressiveInlining)]
        public static ref T NullRef<T>()
            => ref Unsafe.NullRef<T>();
        [MImpl(MImplOpts.AggressiveInlining)]
        public static bool IsNull<T>(ref T value)
            => Unsafe.AsPointer(ref value) == null;

        [MImpl(MImplOpts.AggressiveInlining)]
        public static T* RefAsPointer<T>(ref T value) where T : unmanaged
            => (T*)Unsafe.AsPointer(ref value);
        [MImpl(MImplOpts.AggressiveInlining)]
        public static nint RefAsInt<T>(ref T value) where T : unmanaged
            => (nint)Unsafe.AsPointer(ref value);
        [MImpl(MImplOpts.AggressiveInlining)]
        public static ref T PointerAsRef<T>(T* value) where T : unmanaged
            => ref *value;
        [MImpl(MImplOpts.AggressiveInlining)]
        public static ref T IntAsRef<T>(nint value) where T : unmanaged
            => ref *(T*)value;

        [MImpl(MImplOpts.AggressiveInlining)]
        public static ref T OffsetRef<T>(ref T value, nint offset) where T : unmanaged
            => ref Unsafe.Add(ref value, offset);
        [MImpl(MImplOpts.AggressiveInlining)]
        public static ref T AddRef<T>(ref T value, nuint offset) where T : unmanaged
            => ref Unsafe.Add(ref value, offset);
        [MImpl(MImplOpts.AggressiveInlining)]
        public static ref T SubtractRef<T>(ref T value, nuint offset) where T : unmanaged
            => ref Unsafe.Subtract(ref value, offset);
        [MImpl(MImplOpts.AggressiveInlining)]
        public static ref T IncrementRef<T, TSize>(ref T value) where T : unmanaged where TSize : unmanaged
            => ref Unsafe.Add(ref value, sizeof(TSize));
        [MImpl(MImplOpts.AggressiveInlining)]
        public static ref T SubtractRef<T, TSize>(ref T value) where T : unmanaged where TSize : unmanaged
            => ref Unsafe.Subtract(ref value, sizeof(TSize));

        [MImpl(MImplOpts.AggressiveInlining)]
        public static ref T ByteOffsetRef<T>(ref T value, nint offset) where T : unmanaged
            => ref Unsafe.AddByteOffset(ref value, offset);
        [MImpl(MImplOpts.AggressiveInlining)]
        public static ref T ByteAddRef<T>(ref T value, nuint offset) where T : unmanaged
            => ref Unsafe.AddByteOffset(ref value, offset);
        [MImpl(MImplOpts.AggressiveInlining)]
        public static ref T ByteSubtractRef<T>(ref T value, nuint offset) where T : unmanaged
            => ref Unsafe.SubtractByteOffset(ref value, offset);

        [MImpl(MImplOpts.AggressiveInlining)]
        public static T ManagedDelegate<T>(nint pointer) where T : Delegate
            => Marshal.GetDelegateForFunctionPointer<T>(pointer);

        [MImpl(MImplOpts.AggressiveInlining)]
        public static ref TTo As<TFrom, TTo>(ref TFrom from)
            => ref Unsafe.As<TFrom, TTo>(ref from);

        [MImpl(MImplOpts.AggressiveInlining)]
        public static nuint SizeOf<T>()
            => (nuint)Unsafe.SizeOf<T>();
    }
}


// Different, more relevant namespace that so Godot.GdExtension isn't exposed.
namespace Godot.Interop
{
    /// <summary>
    /// Helper methods for manipulating <see langword="ref"/> types without having to use the <see langword="unsafe"/> keyword.
    /// </summary>
    /// <remarks>This is only used internally. It is not recommended to use this unless you know what you are doing.</remarks>
    public static class RefHelper
    {
        /// <summary>
        /// Increments the <see langword="ref"/> pointer by <paramref name="offset"/> elements.
        /// </summary>
        /// <typeparam name="T">The type of the reference.</typeparam>
        /// <param name="value">The reference to increment.</param>
        /// <param name="offset">The amount of elements to increment by.</param>
        /// <returns>The offset reference.</returns>
        [MImpl(MImplOpts.AggressiveInlining)]
        public static ref T AddRef<T>(ref T value, nuint offset) where T : unmanaged
            => ref MemUtil.AddRef(ref value, offset);
        /// <summary>
        /// Reinterprets a <see langword="ref"/> pointer as an <see cref="nint"/>.
        /// </summary>
        /// <typeparam name="T">The type of the reference.</typeparam>
        /// <param name="value">The reference to reinterpret.</param>
        /// <returns>The <see cref="nint"/> address of the reference.</returns>
        [MImpl(MImplOpts.AggressiveInlining)]
        public static nint RefAsInt<T>(ref T value) where T : unmanaged
            => MemUtil.RefAsInt(ref value);
        /// <summary>
        /// Reinterprets a <see cref="nint"/> as a <see langword="ref"/> pointer.
        /// </summary>
        /// <typeparam name="T">The type of the reference.</typeparam>
        /// <param name="value">The <see cref="nint"/> to reinterpret.</param>
        /// <returns>The reference at the <see cref="nint"/> address.</returns>
        [MImpl(MImplOpts.AggressiveInlining)]
        public static ref T IntAsRef<T>(nint value) where T : unmanaged
            => ref MemUtil.IntAsRef<T>(value);
    }
}