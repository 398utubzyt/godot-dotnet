using System;
using System.Runtime.CompilerServices;

namespace Godot
{
    internal class BiHashMap<LeftT, RightT>
    {
        internal delegate bool CompareRight(RightT a, RightT b);

        private class Entry
        {
            public LeftT Left;
            public RightT Right;
            public ulong Hash;
            public int RefCount;
            public bool HasNext;
            public Entry Next;

            public void Ref()
                => RefCount++;
            public bool Unref()
                => --RefCount <= 0;

            public void Append(ulong hash, LeftT left, RightT right)
            {
                if (HasNext)
                    Next.Append(hash, left, right);
                else
                {
                    Next = new Entry { Left = left, Right = right, Hash = hash, HasNext = false, Next = null };
                    HasNext = true;
                }
            }

            public bool TryGet(ulong hash, out RightT value)
            {
                Entry e = this;
                while (e.HasNext && e.Hash != hash)
                    e = e.Next;

                value = e.Right;
                return e.Hash == hash;
            }

            public bool FindLeft(RightT value, CompareRight comp, out LeftT left)
            {
                Entry e = this;
                bool noeq;
                while ((noeq = !comp(value, e.Right)) && e.HasNext)
                    e = e.Next;

                if (noeq)
                {
                    left = default;
                    return false;
                }

                left = e.Left;
                return true;
            }

            public bool Remove(ulong hash)
            {
                if (!HasNext)
                    return false;
                
                if (Next.Hash != hash)
                    return Next.Remove(hash);

                Next = Next.Next;
                HasNext = Next.HasNext;
                return true;
            }

            public void Clear()
            {
                if (HasNext)
                    Next.Clear();
                HasNext = false;
                Next = null;
            }

            public void Dump()
            {
                Console.WriteLine($"{Left}, {Right} ({Hash})");
                if (HasNext)
                    Next.Dump();
            }
        }
        public const int DEFAULT_HASH_SIZE = 101;

        private readonly Entry[] _buffer;

        [MImpl(MImplOpts.AggressiveInlining)]
        public bool Contains(ulong hash)
            => TryGet(hash, out _);
        [MImpl(MImplOpts.AggressiveInlining)]
        public bool Contains(nint hash)
            => Contains((ulong)hash);
        public bool FindLeft(RightT value, CompareRight comp, out LeftT left)
        {
            if (comp == null)
            {
                left = default;
                return false;
            }

            lock (_buffer)
                for (int i = 0; i < _buffer.Length; i++)
                {
                    Entry e = _buffer[i];
                    if (e == null)
                        continue;

                    lock (e)
                        if (e.FindLeft(value, comp, out left))
                            return true;
                }
            left = default;
            return false;
        }

        [MImpl(MImplOpts.AggressiveInlining)]
        private RightT Get(ulong hash)
        {
            Entry e;
            lock (_buffer)
                e = _buffer[hash % (ulong)_buffer.Length];

            if (e != null)
                lock (e)
                    if (e.TryGet(hash, out RightT value))
                        return value;

            return default;
        }

        private bool Set(ulong hash, LeftT left, RightT value)
        {
            ref Entry e = ref MemUtil.NullRef<Entry>();
            lock (_buffer)
            {
                e = ref _buffer[hash % (ulong)_buffer.Length];
                if (e == null)
                {
                    e = new Entry() { Left = left, Right = value, Hash = hash, HasNext = false, Next = null };
                    return true;
                }
            }

            lock (e)
                e.Append(hash, left, value);
            return false;
        }

        public bool TryGet(ulong hash, out RightT value)
        {
            Entry e;
            lock (_buffer)
                e = _buffer[hash % (ulong)_buffer.Length];
            if (e == null)
            {
                value = default;
                return false;
            }

            lock (e)
                return e.TryGet(hash, out value);
        }
        [MImpl(MImplOpts.AggressiveInlining)]
        public bool TryGet(nint hash, out RightT value)
            => TryGet((ulong)hash, out value);

        public bool Remove(ulong hash)
        {
            Entry e;
            lock (_buffer)
                e = _buffer[hash % (ulong)_buffer.Length];
            if (e == null)
                return false;

            lock (e)
            {
                if (e.Hash == hash)
                {
                    lock (_buffer)
                        _buffer[hash % (ulong)_buffer.Length] = e.Next;
                    return true;
                }

                return e.Remove(hash);
            }
        }
        [MImpl(MImplOpts.AggressiveInlining)]
        public bool Remove(nint hash)
            => Remove((ulong)hash);

        public void Clear()
        {
            lock (_buffer)
            {
                Entry e;
                for (int i = 0; i < DEFAULT_HASH_SIZE; i++)
                {
                    e = _buffer[i];
                    lock (e)
                        e.Clear();
                    _buffer[i] = null;
                }
            }
        }

        public void Dump()
        {
            lock (_buffer)
            {
                Entry e;
                for (int i = 0; i < DEFAULT_HASH_SIZE; i++)
                {
                    Console.WriteLine($"-- HASH {i}:");
                    e = _buffer[i];
                    lock (e)
                        e.Dump();
                }
            }
        }

        public RightT this[ulong hash]
        {
            [MImpl(MImplOpts.AggressiveInlining)]
            get => Get(hash);
        }
        public RightT this[ulong hash, LeftT left]
        {
            [MImpl(MImplOpts.AggressiveInlining)]
            set => Set(hash, left, value);
        }
        public RightT this[IntPtr hash]
        {
            [MImpl(MImplOpts.AggressiveInlining)]
            get => Get((ulong)hash);
        }
        public RightT this[IntPtr hash, LeftT left]
        {
            [MImpl(MImplOpts.AggressiveInlining)]
            set => Set((ulong)hash, left, value);
        }

        public BiHashMap(int hashSize = DEFAULT_HASH_SIZE)
            => _buffer = new Entry[hashSize];
    }
}
