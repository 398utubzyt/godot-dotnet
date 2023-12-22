using System;
using System.Collections;
using System.Collections.Generic;

namespace Godot.Collections
{
    public readonly partial struct TypedArray<[MustBeVariant] T> : IList<T>, IReadOnlyList<T>, ICollection<T>, IDisposable
    {
        private unsafe readonly static GDExtensionPtrConstructor _ctor2;
        unsafe static TypedArray()
        {
            _ctor2 = (GDExtensionPtrConstructor)Main.i.VariantGetPtrConstructor(VariantType.Array, 2);
        }

        private readonly VariantArray _self;

        public readonly T this[int index]
        {
            get => _self[index].TryAs(out T value) ? value : default;
            set => _self[index] = Variant.From(value);
        }

        public readonly int Count => _self.Count;
        public readonly bool IsReadOnly => _self.IsReadOnly;

        public readonly void Add(T item)
            => _self.Add(Variant.From(item));

        public readonly void Clear()
            => _self.Clear();

        public readonly bool Contains(T item)
            => _self.Contains(Variant.From(item));

        public unsafe readonly void CopyTo(T[] array, int arrayIndex)
        {
            Variant* data = _self.RawData;
            if (data == null)
                return;

            for (int i = 0; i < Count && arrayIndex + i < array.Length; ++i)
                array[arrayIndex + i] = data[i].TryAs(out T value) ? value : default;
        }

        public readonly IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < Count; ++i)
                yield return this[i];
        }

        public readonly int IndexOf(T item)
            => _self.IndexOf(Variant.From(item));

        public readonly void Insert(int index, T item)
            => _self.Insert(index, Variant.From(item));

        public readonly bool Remove(T item)
            => _self.Remove(Variant.From(item));

        public readonly void RemoveAt(int index)
            => _self.RemoveAt(index);

        readonly IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

        public readonly void Dispose()
            => _self.Dispose();

        public unsafe TypedArray()
        {
            VariantArray self;

            Type t = typeof(T);
            VariantType type = Variant.TypeOf<T>();
            StringName className = type == VariantType.Object ? t.Name : default;
            Variant scriptType = (type == VariantType.Object && ScriptDB.Search(t, out string path)) ? ResourceLoader.Load<Script>(path) : default;

            nint* args = stackalloc nint[3];
            args[0] = (nint)(&type);
            args[1] = (nint)(&className);
            args[2] = (nint)(&scriptType);

            _ctor2((nint)(&self), args);
            _self = self;
        }

        internal TypedArray(VariantArray array)
        {
            _self = array;
        }
    }
}
