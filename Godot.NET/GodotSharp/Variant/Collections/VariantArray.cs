using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using Array = Godot.Collections.VariantArray;

namespace Godot.Collections
{
#pragma warning disable CA1710
    /// <inheritdoc/>
    public unsafe class VariantArray : IList<Variant>, ICollection<Variant>, IDisposable
    {
        private class ArrayIsReadOnlyException : Exception
        {
            public ArrayIsReadOnlyException() : base("Cannot modify array because it is marked as readonly (see `IsReadOnly` property).") { }
        }

        /// <summary>An empty zero-length array.</summary>
        public static readonly Array Empty = new Array((Variant*)0, 0, true);

        private Variant* _ptr;
        private nuint _size;
        private nuint _count;
        private bool _readonly;

        /// <inheritdoc/>
        public static unsafe explicit operator void*(Array arr)
            => arr._ptr;
        /// <inheritdoc/>
        public static unsafe explicit operator Variant*(Array arr)
            => arr._ptr;
        /// <inheritdoc/>
        public static unsafe explicit operator Span<Variant>(Array arr)
            => arr.AsSpan();

        /// <inheritdoc/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public ref Variant GetPinnableReference()
            => ref *_ptr;
        /// <inheritdoc/>
        public Span<Variant> AsSpan()
            => MemUtil.AsSpan(_ptr, _count);

        /// <inheritdoc/>
        public unsafe Variant this[int index] { get => _ptr[index]; set => _ptr[index] = value; }
        /// <inheritdoc/>
        public unsafe Variant this[long index] { get => _ptr[index]; set => _ptr[index] = value; }
        /// <inheritdoc/>
        public unsafe Variant this[nuint index] { get => _ptr[index]; set => _ptr[index] = value; }

        /// <inheritdoc/>
        public int Count => (int)_count;
        /// <inheritdoc/>
        public long LongCount => (long)_count;

        /// <inheritdoc/>
        public bool IsReadOnly { get => _readonly; }

        private void Resize(nuint size)
        {
            if (size > 0)
                _ptr = MemUtil.Realloc(_ptr, size * (nuint)sizeof(Variant));
            _size = size;
        }
        private void Increase(nuint amount)
        {
            _count += amount;
            while (_size <= _count)
                _size *= 2;

            Resize(_size);
        }
        private void Increment()
        {
            if (_size == ++_count)
                Resize(_size * 2);
        }
        private void Decrement()
        {
            if (_size > --_count * 4)
                Resize(_size / 2);
        }

        /// <summary>Adds an item to this array.</summary>
        /// <param name="item">The item to add.</param>
        /// <exception cref="ArrayIsReadOnlyException"/>
        public void Add(Variant item)
        {
            if (_readonly)
                throw new ArrayIsReadOnlyException();
            Increment();
            _ptr[_count - 1] = item;
        }

        /// <summary>Adds a collection of items to this array.</summary>
        /// <param name="items">The items to add.</param>
        /// <exception cref="ArrayIsReadOnlyException"/>
        public void AddRange(params Variant[] items)
        {
            if (_readonly)
                throw new ArrayIsReadOnlyException();
            if (items == null || items.Length == 0)
                return;

            Variant* end = _ptr + _count;
            Increase((nuint)items.LongLength);

            fixed (Variant* arr = items)
                MemUtil.Move(arr, end, (uint)items.LongLength);
        }

        /// <summary>Removes all items from this array.</summary>
        /// <exception cref="ArrayIsReadOnlyException"/>
        public void Clear()
        {
            if (_readonly)
                throw new ArrayIsReadOnlyException();
            for (nuint i = 0; i < _count; i++)
            {
                _ptr[i].Dispose();
                _ptr[i] = default;
            }
        }

        /// <summary>Adds an item to this array.</summary>
        /// <param name="item">The item to search for.</param>
        /// <returns><see langword="true"/> if <paramref name="item"/> is found within the array, otherwise <see langword="false"/>.</returns>
        public bool Contains(Variant item)
        {
            for (nuint i = 0; i < _count; i++)
                if (_ptr[i] == item)
                    return true;
            return false;
        }

        /// <summary>Copies the contents of this array to <paramref name="array"/> at <paramref name="arrayIndex"/>.</summary>
        /// <param name="array">The array to copy to.</param>
        /// <param name="arrayIndex">The index in <paramref name="array"/> to start copying to.</param>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="ArgumentOutOfRangeException"/>
        public void CopyTo(Variant[] array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));

            ulong size = Math.Min(_count, (ulong)array.LongLength) - (ulong)arrayIndex;
            if (size == 0)
                return;
            if (arrayIndex < 0 || (ulong)arrayIndex >= size)
                throw new ArgumentOutOfRangeException(nameof(arrayIndex));

            fixed (Variant* arr = array)
                MemUtil.Move(_ptr, arr, (nuint)size);
        }

        private struct NativeEnumerator : IEnumerator<Variant>
        {
            private readonly VariantArray _arr;
            private nuint _index;

            public readonly Variant Current => _index > 0 && _index < _arr._count ? _arr[_index] : default;
            readonly object IEnumerator.Current => Current;

            public readonly void Dispose()
            {
            }

            public bool MoveNext()
            {
                _index++;
                return _index < _arr._count;
            }

            public void Reset()
            {
                _index = unchecked((nuint)(-1));
            }

            public NativeEnumerator(VariantArray arr)
            {
                _arr = arr;
                _index = unchecked((nuint)(-1));
            }
        }

        /// <inheritdoc/>
        public IEnumerator<Variant> GetEnumerator()
            => new NativeEnumerator(this);

        /// <summary>Gets the index of an item in this array.</summary>
        /// <param name="item">The item to search for.</param>
        /// <returns>The index of <paramref name="item"/> if it is found within the array, otherwise -1.</returns>
        public int IndexOf(Variant item)
        {
            for (nuint i = 0; i < _count; i++)
                if (_ptr[i] == item)
                    return (int)i;
            return -1;
        }

        /// <summary>Inserts an item into this array at <paramref name="index"/>.</summary>
        /// <param name="index">The index to insert <paramref name="item"/> at.</param>
        /// <param name="item">The item to add.</param>
        /// <exception cref="ArrayIsReadOnlyException"/>
        public void Insert(int index, Variant item)
        {
            if (_readonly)
                throw new ArrayIsReadOnlyException();
            if (index < 0 || (nuint)index > _count)
                return;
            if ((nuint)index != _count)
                MemUtil.Move(_ptr + index, _ptr + ++index, _count - (nuint)index);

            Increment();
            _ptr[index] = item;
        }

        private void RemoveAt(nuint index)
        {
            _ptr[index].Dispose();
            if (++index != _count)
                MemUtil.Move(_ptr + --index, _ptr + index, (uint)(_count - ++index));
            Decrement();
        }

        /// <summary>Removes <paramref name="item"/> from this array.</summary>
        /// <param name="item">The item to remove.</param>
        /// <returns><see langword="true"/> if <paramref name="item"/> is found and removed, otherwise <see langword="false"/>.</returns>
        /// <exception cref="ArrayIsReadOnlyException"/>
        public bool Remove(Variant item)
        {
            if (_readonly)
                throw new ArrayIsReadOnlyException();
            for (nuint i = 0; i < _count; i++)
                if (_ptr[i] == item)
                {
                    RemoveAt(i);
                    return true;
                }
            return false;
        }

        /// <summary>Removes the item at <paramref name="index"/> from this array.</summary>
        /// <param name="index">The index of the item to remove.</param>
        /// <returns><see langword="true"/> if <paramref name="index"/> is valid and the item is removed, otherwise <see langword="false"/>.</returns>
        /// <exception cref="ArrayIsReadOnlyException"/>
        public void RemoveAt(int index)
        {
            if (_readonly)
                throw new ArrayIsReadOnlyException();
            if (index < 0 || (nuint)index >= _count)
                return;
            RemoveAt((nuint)index);
        }

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

        private void Free()
        {
            MemUtil.Free(_ptr);
        }

#pragma warning disable CA1816
        private void ManualFree(bool elements)
        {
            GC.SuppressFinalize(this);
            Free();
        }

        /// <summary>Frees this array from memory.</summary>
        /// <remarks>Elements of this array are unaffected and need to manually be freed.</remarks>
        public void Dispose()
        {
            ManualFree(false);
        }

        /// <summary>Frees this array from memory.</summary>
        /// <remarks>
        /// Note: If <paramref name="freeElements"/> is <see langword="true"/>, 
        /// elements of this array are also freed along with the array.
        /// <para/>
        /// Otherwise, they will need to be manually freed if they are not
        /// disposed by garbage collection (e.g. a native resource).
        /// </remarks>
        /// <param name="freeElements">If <see langword="true"/>, elements of this array are freed along with the array.</param>
        public void Dispose(bool freeElements)
        {
            ManualFree(freeElements);
        }
#pragma warning restore CA1816

        /// <inheritdoc/>
        ~VariantArray()
            => Free();

        ///
        public VariantArray() : this(false)
        {
        }
        ///
        public VariantArray(bool readOnly)
        {
            _size = 4;
            _ptr = MemUtil.Alloc<Variant>(_size);
            _readonly = readOnly;
        }

        ///
        public VariantArray(int capacity) : this ((nuint)capacity, false)
        {
        }
        ///
        public VariantArray(long capacity) : this((nuint)capacity, false)
        {
        }
        ///
        public VariantArray(nint capacity) : this((nuint)capacity, false)
        {
        }
        ///
        public VariantArray(nuint capacity) : this(capacity, false)
        {
        }

        ///
        public VariantArray(int capacity, bool readOnly) : this((nuint)capacity, readOnly)
        {
        }
        ///
        public VariantArray(long capacity, bool readOnly) : this((nuint)capacity, readOnly)
        {
        }
        ///
        public VariantArray(nint capacity, bool readOnly) : this((nuint)capacity, readOnly)
        {
        }
        ///
        public VariantArray(nuint capacity, bool readOnly)
        {
            if (capacity == 0)
                throw new ArgumentOutOfRangeException(nameof(capacity));
            _size = capacity;
            _ptr = MemUtil.Alloc<Variant>(_size);
            _readonly = readOnly;
        }

        ///
        public VariantArray(params Variant[] items) : this(false, items)
        {
        }
        ///
        public VariantArray(bool readOnly, params Variant[] items)
        {
            if (items == null || items.Length == 0)
                throw new ArgumentException($"{nameof(items)} cannot be null or empty.");
            _size = _count = (nuint)items.LongLength;
            _ptr = MemUtil.Alloc<Variant>(_size);
            AddRange(items);
            _readonly = readOnly;
        }
        ///
        public VariantArray(nuint capacity, bool readOnly, params Variant[] items)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));
            else if (readOnly)
            {
            }
            _size = _count = capacity;
            _ptr = MemUtil.Alloc<Variant>(_size);
            AddRange(items);
            _readonly = readOnly;
        }

        private VariantArray(Variant* arr, nuint size, bool readOnly)
        {
            _ptr = arr;
            _size = size;
            _readonly = readOnly;
        }
    }
}
