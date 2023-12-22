using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Godot.Collections
{
    public readonly partial struct VariantArray : IList<Variant>, IReadOnlyList<Variant>, ICollection<Variant>, IDisposable
    {
        private unsafe readonly static GDExtensionPtrConstructor _ctor0;
        private unsafe readonly static GDExtensionPtrDestructor _dtor;
        unsafe static VariantArray()
        {
            _ctor0 = (GDExtensionPtrConstructor)Main.i.VariantGetPtrConstructor(VariantType.Array, 0);
            _dtor = (GDExtensionPtrDestructor)Main.i.VariantGetPtrDestructor(VariantType.Array);
        }

        private readonly unsafe ArrayPrivate* _p;

        public unsafe readonly int Count => _p != null ? _p->Array.Size : 0;
        public unsafe readonly bool IsReadOnly => _p != null && _p->ReadOnly != null;

        // Could probably use RawData for this one...
        public unsafe readonly Variant this[int index]
        {
            [MImpl(MImplOpts.AggressiveInlining)]
            get
            {
                VariantArray self = this;
                return *(Variant*)Main.i.ArrayOperatorIndexConst((nint)(&self), index);
            }
            [MImpl(MImplOpts.AggressiveInlining)]
            set
            {
                VariantArray self = this;
                *(Variant*)Main.i.ArrayOperatorIndex((nint)(&self), index) = value;
            }
        }

        [MImpl(MImplOpts.AggressiveInlining)]
        public unsafe readonly void Add(Variant item)
        {
            VariantArray self = this;
            Variant* pitem = &item;
            __InternalCalls.push_back((nint)(&self), (nint*)&pitem, 0, 1);
        }

        [MImpl(MImplOpts.AggressiveInlining)]
        public unsafe readonly void RemoveAt(int index)
        {
            VariantArray self = this;
            int* pindex = &index;
            __InternalCalls.remove_at((nint)(&self), (nint*)&pindex, 0, 1);
        }

        [MImpl(MImplOpts.AggressiveInlining)]
        public readonly bool Remove(Variant item)
        {
            int index = IndexOf(item);
            if (index < 0)
                return false;
            RemoveAt(index);
            return true;
        }

        [MImpl(MImplOpts.AggressiveInlining)]
        public unsafe readonly void Clear()
        {
            VariantArray self = this;
            __InternalCalls.clear((nint)(&self), null, 0, 0);
        }

        [MImpl(MImplOpts.AggressiveInlining)]
        public unsafe readonly bool Contains(Variant item)
        {
            VariantArray self = this;
            Variant* pitem = &item;
            byte ret;
            __InternalCalls.has((nint)(&self), (nint*)&pitem, (nint)(&ret), 1);
            return ret.ToBool();
        }

        public unsafe readonly void CopyTo(Variant[] array, int arrayIndex)
        {
            Variant* data = RawData;
            if (data == null)
                return;

            for (int i = 0; i < Count && arrayIndex + i < array.Length; ++i)
                array[arrayIndex + i] = data[i];
        }

        public readonly IEnumerator<Variant> GetEnumerator()
        {
            // Can't use RawData here -- unsafe is not allowed
            for (int i = 0; i < Count; ++i)
                yield return this[i];
        }

        readonly IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

        [MImpl(MImplOpts.AggressiveInlining)]
        public unsafe readonly int IndexOf(Variant item)
        {
            VariantArray self = this;
            Variant* pitem = &item;
            int ret;
            __InternalCalls.find((nint)(&self), (nint*)&pitem, (nint)(&ret), 1);
            return ret;
        }

        [MImpl(MImplOpts.AggressiveInlining)]
        public unsafe readonly int LastIndexOf(Variant item)
        {
            VariantArray self = this;
            Variant* pitem = &item;
            int ret;
            __InternalCalls.rfind((nint)(&self), (nint*)&pitem, (nint)(&ret), 1);
            return ret;
        }

        [MImpl(MImplOpts.AggressiveInlining)]
        public unsafe readonly void Insert(int index, Variant item)
        {
            VariantArray self = this;
            nint* args = stackalloc nint[2];
            args[0] = (nint)(&item);
            args[1] = (nint)(&index);
            __InternalCalls.insert((nint)(&self), args, 0, 2);
        }

        public unsafe readonly void Dispose()
        {
            Variant var = this;
            Main.i.VariantDestroy((nint)(&var));
        }

        public override unsafe readonly int GetHashCode()
        {
            VariantArray self = this;
            int ret;
            __InternalCalls.hash((nint)(&self), null, (nint)(&ret), 0);
            return ret;
        }

        public unsafe readonly string Join(char separator)
        {
            Variant* data = RawData;
            int count = Count;

            if (data == null || count == 0)
                return string.Empty;

            StringBuilder sb = new StringBuilder(data[0].ToString());

            for (int i = 1; i < count; ++i)
            {
                sb.Append(separator);
                sb.Append(data[i].ToString());
            }

            return sb.ToString();
        }

        public unsafe readonly string Join(string separator)
        {
            Variant* data = RawData;
            int count = Count;

            if (data == null || count == 0)
                return string.Empty;

            StringBuilder sb = new StringBuilder(data[0].ToString());

            for (int i = 1; i < count; ++i)
            {
                sb.Append(separator);
                sb.Append(data[i].ToString());
            }

            return sb.ToString();
        }

        // This should match Godot's output
        public override readonly string ToString()
            => $"[{Join(", ")}]";

        public unsafe VariantArray()
        {
            VariantArray self = this;
            _ctor0((nint)(&self), null);
            this = self;
        }

        public readonly bool TryAsTyped<[MustBeVariant] T>(out TypedArray<T> typed)
        {
            Type t = typeof(T);
            VariantType type = Variant.TypeOf<T>();
            StringName className = type == VariantType.Object ? t.Name : default;
            Variant scriptType = (type == VariantType.Object && ScriptDB.Search(t, out string path)) ? ResourceLoader.Load<Script>(path) : default;

            if (IsTyped && BuiltInType == type && TypeClassName == className && TypeScript == scriptType)
            {
                typed = new TypedArray<T>(this);
                return true;
            }

            typed = default;
            return false;
        }
    }
}
