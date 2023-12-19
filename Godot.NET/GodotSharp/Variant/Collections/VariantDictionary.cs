using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Godot.Collections
{
    public partial struct VariantDictionary : IDictionary<Variant, Variant>, IDisposable
    {
        private unsafe DictionaryPrivate* _p;

        public unsafe readonly Variant this[Variant key]
        {
            [MImpl(MImplOpts.AggressiveInlining)]
            get
            {
                VariantDictionary self = this;
                return *(Variant*)Main.i.DictionaryOperatorIndexConst((nint)(&self), (nint)(&key));
            }
            [MImpl(MImplOpts.AggressiveInlining)]
            set
            {
                VariantDictionary self = this;
                *(Variant*)Main.i.DictionaryOperatorIndex((nint)(&self), (nint)(&key)) = value;
            }
        }

        public readonly unsafe int Count { get
            {
                VariantDictionary self = this;
                int ret;
                __InternalCalls.size((nint)(&self), null, (nint)(&ret), 0);
                return ret;
            } }

        public unsafe readonly bool IsReadOnly => _p != null && _p->ReadOnly != null;

        private unsafe readonly VariantArray VariantKeys
        {
            get
            {
                VariantDictionary self = this;
                VariantArray keys;
                __InternalCalls.keys((nint)(&self), null, (nint)(&keys), 0);
                return keys;
            }
        }

        private unsafe readonly VariantArray VariantValues
        {
            get
            {
                VariantDictionary self = this;
                VariantArray values;
                __InternalCalls.values((nint)(&self), null, (nint)(&values), 0);
                return values;
            }
        }

        public unsafe readonly ICollection<Variant> Keys => VariantKeys;
        public unsafe readonly ICollection<Variant> Values => VariantValues;
        

        public readonly void Add(Variant key, Variant value)
            => this[key] = value;

        public readonly void Add(KeyValuePair<Variant, Variant> item)
            => Add(item.Key, item.Value);

        public unsafe readonly bool Remove(Variant key)
        {
            VariantDictionary self = this;
            Variant* pkey = &key;
            byte ret;
            __InternalCalls.erase((nint)(&self), (nint*)&pkey, (nint)(&ret), 1);
            return ret.ToBool();
        }

        public readonly bool Remove(KeyValuePair<Variant, Variant> item)
            => Contains(item) && Remove(item.Key);

        public unsafe readonly void Clear()
        {
            VariantDictionary self = this;
            __InternalCalls.clear((nint)(&self), null, 0, 0);
        }

        public readonly bool Contains(KeyValuePair<Variant, Variant> item)
            => ContainsKey(item.Key) && this[item.Key] == item.Value;

        public unsafe readonly bool ContainsKey(Variant key)
        {
            VariantDictionary self = this;
            Variant* pkey = &key;
            byte ret;
            __InternalCalls.has((nint)(&self), (nint*)&pkey, (nint)(&ret), 1);
            return ret.ToBool();
        }

        public readonly void CopyTo(KeyValuePair<Variant, Variant>[] array, int arrayIndex)
        {
            VariantArray keys = VariantKeys;
            VariantArray values = VariantValues;

            for (int i = 0; i < Count && arrayIndex + i < array.Length; ++i)
                array[arrayIndex + i] = new KeyValuePair<Variant, Variant>(keys[i], values[i]);
        }

        public readonly IEnumerator<KeyValuePair<Variant, Variant>> GetEnumerator()
        {
            VariantArray keys = VariantKeys;
            for (int i = 0; i < keys.Count; i++)
                yield return new KeyValuePair<Variant, Variant>(keys[i], this[keys[i]]);
        }

        public readonly bool TryGetValue(Variant key, [MaybeNullWhen(false)] out Variant value)
        {
            if (ContainsKey(key))
            {
                value = this[key];
                return true;
            }

            value = default;
            return false;
        }

        readonly IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

        public unsafe readonly void Dispose()
        {
            Variant var = this;
            Main.i.VariantDestroy((nint)(&var));
        }
    }
}
