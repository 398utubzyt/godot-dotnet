using System;
using System.Collections;
using System.Collections.Generic;

namespace Godot.Collections
{
    public partial struct VariantArray : IList<Variant>, IReadOnlyList<Variant>, ICollection<Variant>, IDisposable
    {
        private unsafe ArrayPrivate* _p;

        public unsafe readonly int Count => _p != null ? _p->Array.Size : 0;
        public unsafe readonly bool IsReadOnly => _p != null && _p->ReadOnly != null;

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

        public readonly void CopyTo(Variant[] array, int arrayIndex)
        {
            for (int i = 0; i < Count && arrayIndex + i < array.Length; ++i)
                array[arrayIndex + i] = this[i];
        }

        public readonly IEnumerator<Variant> GetEnumerator()
        {
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

        public unsafe readonly void Insert(int index, Variant item)
        {
            VariantArray self = this;
            nint* args = stackalloc nint[2];
            args[0] = (nint)(&item);
            args[1] = (nint)(&index);
            __InternalCalls.find((nint)(&self), args, 0, 2);
        }

        public unsafe readonly void Dispose()
        {
            Variant var = this;
            Main.i.VariantDestroy((nint)(&var));
        }
    }
}
