using System;
using System.ComponentModel;

namespace Godot.Collections
{
    internal interface IPackedVector<T> where T : unmanaged
    {
        public int Length { get; }
        ref T GetPinnableReference();

        T this[int index]
        {
            [MImpl(MImplOpts.AggressiveInlining)]
            get
            {
                if (index >= 0 && index < Length)
                    MemUtil.AddRef(ref GetPinnableReference(), (nuint)index);
                throw new IndexOutOfRangeException("Get index is out of bounds.");
            }
            [MImpl(MImplOpts.AggressiveInlining)]
            set
            {
                if (index >= 0 && index < Length)
                    MemUtil.AddRef(ref GetPinnableReference(), (nuint)index) = value;
                throw new IndexOutOfRangeException("Set index is out of bounds.");
            }
        }

        [MImpl(MImplOpts.AggressiveInlining)]
        Span<T> ToSpan()
            => MemUtil.AsSpan(ref GetPinnableReference(), (nuint)Length);
        [MImpl(MImplOpts.AggressiveInlining)]
        ReadOnlySpan<T> ToReadOnlySpan()
            => MemUtil.AsConstSpan(ref GetPinnableReference(), (nuint)Length);
        [MImpl(MImplOpts.AggressiveInlining)]
        T[] ToArray()
            => MemUtil.AsSpan(ref GetPinnableReference(), (nuint)Length).ToArray();
        [MImpl(MImplOpts.AggressiveInlining)]
        void CopyTo(Span<T> span)
        {
            if (span.Length != 0 && Length != 0)
                MemUtil.Move(ref GetPinnableReference(), ref span.GetRef(), (nuint)Mathf.Min(span.Length, Length));
        }
        unsafe void CopyTo(T[] array, int offset = 0)
        {
            if (array == null || offset < 0 || offset >= array.Length || Length == 0)
                return;
            fixed (T* dest = array)
                MemUtil.Move(
                    ref GetPinnableReference(), 
                    ref MemUtil.PointerAsRef(dest + offset), 
                    (nuint)Mathf.Min(array.Length - offset, Length));
        }
    }

    internal unsafe struct PackedVector<T> where T : unmanaged
    {
        private readonly nint _write; // Basically padding
        public T* CowData; // Internally it's just a pointer

        [EditorBrowsable(EditorBrowsableState.Never)]
        public ref T GetPinnableReference()
        {
            ref T ret = ref MemUtil.NullRef<T>();
            if (Size != 0) ret = ref *CowData;
            return ref ret;
        }
        [MImpl(MImplOpts.AggressiveInlining)]
        public readonly unsafe Span<T> ToSpan()
            => MemUtil.AsSpan(CowData, (nuint)Size);
        [MImpl(MImplOpts.AggressiveInlining)]
        public readonly unsafe ReadOnlySpan<T> ToReadOnlySpan()
            => MemUtil.AsConstSpan(CowData, (nuint)Size);
        [MImpl(MImplOpts.AggressiveInlining)]
        public readonly unsafe T[] ToArray()
            => MemUtil.AsSpan(CowData, (nuint)Size).ToArray();
        [MImpl(MImplOpts.AggressiveInlining)]
        public readonly unsafe void CopyTo(Span<T> span)
        {
            if (span.Length != 0 && Size != 0)
                MemUtil.Move(CowData, MemUtil.RefAsPointer(ref span.GetRef()), (nuint)Mathf.Min(span.Length, Size));
        }
        public readonly unsafe void CopyTo(T[] array, int offset = 0)
        {
            if (array == null || offset < 0 || offset >= array.Length || Size == 0)
                return;
            fixed (T* dest = array)
                MemUtil.Move(CowData, dest + offset, (nuint)Mathf.Min(array.Length - offset, Size));
        }

        // whar the heck
        public readonly int Size { [MImpl(MImplOpts.AggressiveInlining)] get => CowData != null ? *((int*)CowData - 1) : 0; }
    }

    public readonly struct PackedByteArray : IPackedVector<byte>
    {
        private readonly PackedVector<byte> _self;

        public readonly int Length { [MImpl(MImplOpts.AggressiveInlining)] get => _self.Size; }
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MImpl(MImplOpts.AggressiveInlining)]
        public readonly ref byte GetPinnableReference()
            => ref _self.GetPinnableReference();
    }

    public readonly struct PackedColorArray : IPackedVector<Color>
    {
        private readonly PackedVector<Color> _self;

        public readonly int Length { [MImpl(MImplOpts.AggressiveInlining)] get => _self.Size; }
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MImpl(MImplOpts.AggressiveInlining)]
        public readonly ref Color GetPinnableReference()
            => ref _self.GetPinnableReference();
    }

    public readonly struct PackedFloat32Array : IPackedVector<float>
    {
        private readonly PackedVector<float> _self;

        public readonly int Length { [MImpl(MImplOpts.AggressiveInlining)] get => _self.Size; }
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MImpl(MImplOpts.AggressiveInlining)]
        public readonly ref float GetPinnableReference()
            => ref _self.GetPinnableReference();
    }

    public readonly struct PackedFloat64Array : IPackedVector<double>
    {
        private readonly PackedVector<double> _self;

        public readonly int Length { [MImpl(MImplOpts.AggressiveInlining)] get => _self.Size; }
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MImpl(MImplOpts.AggressiveInlining)]
        public readonly ref double GetPinnableReference()
            => ref _self.GetPinnableReference();
    }

    public readonly struct PackedInt32Array : IPackedVector<int>
    {
        private readonly PackedVector<int> _self;

        public readonly int Length { [MImpl(MImplOpts.AggressiveInlining)] get => _self.Size; }
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MImpl(MImplOpts.AggressiveInlining)]
        public readonly ref int GetPinnableReference()
            => ref _self.GetPinnableReference();
    }

    public readonly struct PackedInt64Array : IPackedVector<long>
    {
        private readonly PackedVector<long> _self;

        public readonly int Length { [MImpl(MImplOpts.AggressiveInlining)] get => _self.Size; }
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MImpl(MImplOpts.AggressiveInlining)]
        public readonly ref long GetPinnableReference()
            => ref _self.GetPinnableReference();
    }

    public readonly struct PackedStringArray : IPackedVector<nint>
    {
        private readonly PackedVector<nint> _self;

        public readonly int Length { [MImpl(MImplOpts.AggressiveInlining)] get => _self.Size; }
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MImpl(MImplOpts.AggressiveInlining)]
        readonly ref nint IPackedVector<nint>.GetPinnableReference()
            => ref _self.GetPinnableReference();
        [MImpl(MImplOpts.AggressiveInlining)]
        readonly Span<nint> IPackedVector<nint>.ToSpan()
            => _self.ToSpan();
        [MImpl(MImplOpts.AggressiveInlining)]
        readonly ReadOnlySpan<nint> IPackedVector<nint>.ToReadOnlySpan()
            => _self.ToReadOnlySpan();
        [MImpl(MImplOpts.AggressiveInlining)]
        readonly nint[] IPackedVector<nint>.ToArray()
            => _self.ToArray();
        [MImpl(MImplOpts.AggressiveInlining)]
        readonly void IPackedVector<nint>.CopyTo(nint[] array, int offset)
            => _self.CopyTo(array, offset);

        [MImpl(MImplOpts.AggressiveInlining)]
        public readonly Span<string> ToSpan()
            => ToArray().AsSpan();
        [MImpl(MImplOpts.AggressiveInlining)]
        public readonly ReadOnlySpan<string> ToReadOnlySpan()
            => MemUtil.AsConstSpan(ToArray().AsSpan());
        [MImpl(MImplOpts.AggressiveInlining)]
        public readonly string[] ToArray()
        {
            string[] ret = new string[_self.Size];
            CopyTo(ret);
            return ret;
        }
        public unsafe readonly void CopyTo(Span<string> span)
        {
            int max = Mathf.Min(span.Length, _self.Size);
            if (max != 0)
                for (int i = 0; i < max; ++i)
                    span[i] = StringDB.SearchOrCreate(_self.CowData[i]);
        }
        public unsafe readonly void CopyTo(string[] array, int offset = 0)
        {
            if (array == null || offset < 0 || offset >= array.Length || _self.CowData == null || _self.Size == 0)
                return;

            for (int i = offset; i < array.Length; ++i)
                array[i] = StringDB.SearchOrCreate(_self.CowData[i - offset]);
        }
    }

    public readonly struct PackedVector2Array : IPackedVector<Vector2>
    {
        private readonly PackedVector<Vector2> _self;

        public readonly int Length { [MImpl(MImplOpts.AggressiveInlining)] get => _self.Size; }
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MImpl(MImplOpts.AggressiveInlining)]
        public readonly ref Vector2 GetPinnableReference()
            => ref _self.GetPinnableReference();
    }

    public readonly struct PackedVector3Array : IPackedVector<Vector3>
    {
        private readonly PackedVector<Vector3> _self;

        public readonly int Length { [MImpl(MImplOpts.AggressiveInlining)] get => _self.Size; }
        [EditorBrowsable(EditorBrowsableState.Never)]
        [MImpl(MImplOpts.AggressiveInlining)]
        public readonly ref Vector3 GetPinnableReference()
            => ref _self.GetPinnableReference();
    }
}
