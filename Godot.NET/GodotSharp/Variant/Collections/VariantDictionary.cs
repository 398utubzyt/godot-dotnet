using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using Dictionary = Godot.Collections.VariantDictionary;

namespace Godot.Collections
{
#pragma warning disable CA1710
    /// <inheritdoc/>
    public unsafe class VariantDictionary : IDictionary<Variant, Variant>, IDisposable
    {
        private class DictionaryIsReadOnlyException : Exception
        {
            public DictionaryIsReadOnlyException() : base("Cannot modify dictionary because it is marked as readonly (see `IsReadOnly` property).") { }
        }

        private struct Entry
        {
            public Variant Value;
            public Entry* Next;
            public int Hash;
        }

        private readonly Entry* _ptr;
        private readonly nuint _size;
        private nuint _count;
        private bool _readonly;

        public ICollection<Variant> Keys => throw new NotImplementedException();

        public ICollection<Variant> Values => throw new NotImplementedException();

        public int Count => throw new NotImplementedException();

        public bool IsReadOnly => throw new NotImplementedException();

        public Variant this[Variant key] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void Add(Variant key, Variant value)
        {
            throw new NotImplementedException();
        }

        public bool ContainsKey(Variant key)
        {
            throw new NotImplementedException();
        }

        public bool Remove(Variant key)
        {
            throw new NotImplementedException();
        }

        public bool TryGetValue(Variant key, [MaybeNullWhen(false)] out Variant value)
        {
            throw new NotImplementedException();
        }

        public void Add(KeyValuePair<Variant, Variant> item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(KeyValuePair<Variant, Variant> item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(KeyValuePair<Variant, Variant>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(KeyValuePair<Variant, Variant> item)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<KeyValuePair<Variant, Variant>> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public VariantDictionary(nuint capacity, bool readOnly)
        {
            if (capacity <= 0)
                throw new ArgumentOutOfRangeException(nameof(capacity));
            _size = capacity;
            _ptr = MemUtil.Alloc<Entry>(_size);
            _readonly = readOnly;
        }
    }
}
