using System;
using System.Collections;
using System.Collections.Generic;

namespace Godot.Collections
{
    internal unsafe class NativeList<T> : ILongList<T>, IList<T>, IFixed<T>, IDisposable where T : unmanaged, IEquatable<T>
    {
        public const nuint DEFAULT_SIZE = 4;
        public static readonly nuint ErrorIndex = unchecked((nuint)(-1));

        private T* _buffer;
        private bool _freed;
        private nuint _size;
        private nuint _count;

        public nuint Capacity => _size;
        public nuint Count => _count;
        public bool IsReadOnly => false;

        #region Native Stuff

        public ref T GetPinnableReference()
            => ref MemUtil.PointerAsRef(_buffer);
        public Span<T> ToSpan()
            => MemUtil.AsSpan(_buffer, _count);

        public NativeList(nuint capacity)
        {
            _buffer = MemUtil.Alloc<T>(capacity);
        }

        public NativeList() : this(DEFAULT_SIZE)
        {
        }

        protected virtual void Free(bool manual)
        {
            if (_freed)
                return;

            _freed = true;
            MemUtil.Free(_buffer);
        }

        public void Free()
        {
            Free(true);
            GC.SuppressFinalize(this);
        }
        ~NativeList()
        {
            Free(false);
        }

        #endregion

        public T this[nuint index]
        {
            get => index >= 0 && index < _count ?
                _buffer[index] :
                throw new IndexOutOfRangeException("Get accessor index was out of bounds");
            set
            {
                if (index < 0 || index >= _count)
                { throw new IndexOutOfRangeException("Set accessor index was out of bounds"); }
                _buffer[index] = value;
            }
        }

#if DEBUG
        // Return false when running out of memory
        private bool Grow()
        {
            // Unrealistic, but it's possible to overflow.
            if (_size == ErrorIndex)
                return false;

            if (_count * 2 < _size)
                return true;
#else
        private void Grow()
        {
            if (_count * 2 < _size)
                return;
#endif
            _size = Mathf.NearestPo2(_count + 1);
            _buffer = MemUtil.Realloc(_buffer, _size);
#if DEBUG
            return true;
#endif
        }

        private void Shrink()
        {
            if (_size == 0)
                return;

            if (_count * 4 > _size)
                return;

            _size /= 2;
            _buffer = MemUtil.Realloc(_buffer, _size);
        }

        public nuint IndexOf(T item)
        {
            for (nuint i = 0; i < _count; ++i)
                if (_buffer[i].Equals(item))
                    return i;
            return ErrorIndex;
        }

        public nuint IndexOf(Predicate<T> predicate)
        {
            for (nuint i = 0; i < _count; ++i)
                if (predicate(_buffer[i]))
                    return i;
            return ErrorIndex;
        }

        public void Insert(nuint index, T item)
        {
            if (index > _count)
                throw new ArgumentOutOfRangeException(nameof(index));

#if DEBUG
            System.Diagnostics.Debug.Assert(Grow(), "I'm not even sure how you managed this one...");
#else
            Grow();
#endif

            if (index < _count++)
                MemUtil.Move(_buffer + index, _buffer + index + 1, _count - index);

            _buffer[index] = item;
        }

        public void RemoveAt(nuint index)
        {
            if (index >= _count)
                throw new ArgumentOutOfRangeException(nameof(index));

            if (index < --_count)
                MemUtil.Move(_buffer + index + 1, _buffer + index, _count - index);

            Shrink();
        }

        public void Add(T item)
        {
#if DEBUG
            System.Diagnostics.Debug.Assert(Grow(), "I'm not even sure how you managed this one...");
#else
            Grow();
#endif

            _buffer[_count++] = item;
        }

        public void Clear()
        {
            MemUtil.Zero(_buffer, _count);
        }

        public bool Contains(T item)
        {
            for (nuint i = 0; i < _count; ++i)
                if (_buffer[i].Equals(item))
                    return true;
            return false;
        }

        public bool Contains(Predicate<T> predicate)
        {
            for (nuint i = 0; i < _count; ++i)
                if (predicate(_buffer[i]))
                    return true;
            return false;
        }

        public void CopyTo(T[] array, nuint arrayIndex)
        {
            ArgumentNullException.ThrowIfNull(array, nameof(array));
            if (array.LongLength == 0)
                return;
            fixed (T* to = array)
                MemUtil.Move(_buffer, to + arrayIndex, Math.Min((nuint)array.LongLength - arrayIndex, _count));
        }

        public bool Remove(T item)
        {
            nuint index = IndexOf(item);
            if (index == ErrorIndex)
                return false;
            RemoveAt(index);
            return true;
        }

        public bool Search(Predicate<T> predicate, out T item)
        {
            for (nuint i = 0; i < _count; ++i)
                if (predicate(_buffer[i]))
                {
                    item = _buffer[i];
                    return true;
                }
            item = default;
            return false;
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (nuint i = 0; i < _count; ++i)
                yield return this[i];
        }

        #region Explicit Definitions

        void IDisposable.Dispose()
        {
            Free();
            GC.KeepAlive(this);
        }
        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();
        T IList<T>.this[int index] { get => this[(nuint)index]; set => this[(nuint)index] = value; }
        int IList<T>.IndexOf(T item)
            => (int)IndexOf(item);
        void IList<T>.Insert(int index, T item)
            => Insert((nuint)index, item);
        void IList<T>.RemoveAt(int index)
            => RemoveAt((nuint)index);
        int ICollection<T>.Count => (int)_count;
        void ICollection<T>.CopyTo(T[] array, int arrayIndex)
            => CopyTo(array, (nuint)arrayIndex);

        #endregion
    }
}
