// Putting this in a separate file to avoid clutter

namespace Godot.Collections
{
    partial struct VariantArray
    {
        public delegate bool CustomSortFunc(Variant a, Variant b);
        public delegate bool FilterFunc(Variant item);
        public delegate Variant MapFunc(Variant item);
        public delegate Variant ReduceFunc(Variant accum, Variant item);
        public delegate bool CustomBinarySearchFunc(Variant item, Variant target);

        // bool ()
        public unsafe readonly bool IsEmpty { 
            get
            {
                VariantArray self = this;
                byte ret;
                __InternalCalls.is_empty((nint)(&self), null, (nint)(&ret), 0);
                return ret.ToBool();
            } }

        /// <summary>
        /// Assigns elements of another <paramref name="array"/> into the <see cref="VariantArray"/>.
        /// Resizes the <see cref="VariantArray"/> to match <paramref name="array"/>.
        /// Performs type conversions if the <see cref="VariantArray"/> is typed.
        /// </summary>
        /// <param name="array">The array to assign.</param>
        public unsafe readonly void Assign(VariantArray array)
        {
            VariantArray self = this;
            VariantArray* arg = &array;
            __InternalCalls.assign((nint)(&self), (nint*)&arg, 0, 1);
        }

        /// <summary>
        /// Adds an element at the beginning of the array. See also <seealso cref="Add(Variant)"/>.
        /// </summary>
        /// <remarks>
        /// Note: On large arrays, this method is much slower than <see cref="Add(Variant)"/> as
        /// it will reindex all the array's elements every time it's called.
        /// The larger the array, the slower <see cref="Prepend(Variant)"/> will be.
        /// </remarks>
        /// <param name="value"></param>
        public readonly void Prepend(Variant value)
        {
            Insert(0, value);
        }

        /// <summary>
        /// Appends another array at the end of this array.
        /// <example>
        /// <code>
        /// var array1 = [1, 2, 3];
        /// var array2 = [4, 5, 6];
        /// array1.AddRange(array2);
        /// Console.WriteLine(string.Join(array1)) // Prints [1, 2, 3, 4, 5, 6].
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="array"></param>
        public unsafe readonly void AddRange(VariantArray array)
        {
            VariantArray self = this;
            VariantArray* arg = &array;
            __InternalCalls.append_array((nint)(&self), (nint*)&arg, 0, 1);
        }

        // int (int size)
        public unsafe readonly int Resize(int size)
        {
            VariantArray self = this;
            int* arg = &size;
            int ret;
            __InternalCalls.append_array((nint)(&self), (nint*)&arg, (nint)(&ret), 1);
            return ret;
        }

        // void (Variant value)
        public unsafe readonly void Fill(Variant item)
        {
            VariantArray self = this;
            Variant* arg = &item;
            __InternalCalls.fill((nint)(&self), (nint*)&arg, 0, 1);
        }

        /// <summary>
        /// Returns the first element of the array. Prints an error and returns <see langword="null"/> if the array is empty.
        /// </summary>
        public unsafe readonly Variant Front
        {
            [MImpl(MImplOpts.AggressiveInlining)]
            get
            {
                Variant* data = RawData;
                if (data == null)
                    return default;
                return data[0];
            }
        }

        /// <summary>
        /// Returns the last element of the array. Prints an error and returns <see langword="null"/> if the array is empty.
        /// </summary>
        public unsafe readonly Variant Back
        {
            get
            {
                Variant* data = RawData;
                int count = Count;
                if (data == null || count == 0)
                    return default;
                return data[count - 1];
            }
        }

        // Variant ()
        public unsafe readonly Variant PickRandom()
        {
            Variant* data = RawData;
            int count = Count;
            if (data == null || count == 0)
                return default;
            return data[System.Random.Shared.Next(count)];
        }

        // int (Variant value)
        public unsafe readonly int CountOf(Variant item)
        {
            VariantArray self = this;
            Variant* arg = &item;
            int ret;
            __InternalCalls.count((nint)(&self), (nint*)&arg, (nint)(&ret), 1);
            return ret;
        }

        // Variant ()
        public unsafe readonly Variant PopBack()
        {
            VariantArray self = this;
            Variant ret;
            __InternalCalls.pop_back((nint)(&self), null, (nint)(&ret), 0);
            return ret;
        }

        // Variant ()
        public unsafe readonly Variant PopFront()
        {
            VariantArray self = this;
            Variant ret;
            __InternalCalls.pop_front((nint)(&self), null, (nint)(&ret), 0);
            return ret;
        }

        // Variant (int position)
        public unsafe readonly Variant PopAt(int position)
        {
            VariantArray self = this;
            int* arg = &position;
            Variant ret;
            __InternalCalls.pop_at((nint)(&self), (nint*)&arg, (nint)(&ret), 1);
            return ret;
        }

        // void ()
        public unsafe readonly void Sort()
        {
            VariantArray self = this;
            __InternalCalls.sort((nint)(&self), null, 0, 0);
        }

        // void (Callable func)
        public unsafe readonly void Sort(Callable func)
        {
            VariantArray self = this;
            Callable* arg = &func;
            __InternalCalls.sort((nint)(&self), (nint*)arg, 0, 1);
        }
        public unsafe readonly void Sort(CustomSortFunc func)
        {
            using Callable callable = (Callable)func;
            Sort(callable);
        }

        // void ()
        public unsafe readonly void Shuffle()
        {
            VariantArray self = this;
            __InternalCalls.shuffle((nint)(&self), null, 0, 0);
        }

        // int (Variant value, bool before)
        public unsafe readonly int BinarySearch(Variant item, bool before = true)
        {
            VariantArray self = this;
            nint* args = stackalloc nint[4];
            args[0] = (nint)(&item);
            byte beforeByte = before.ToExtBool();
            args[1] = (nint)(&beforeByte);
            int ret;
            __InternalCalls.bsearch_custom((nint)(&self), args, (nint)(&ret), 2);
            return ret;
        }

        // int (Variant value, Callable func, bool before)
        public unsafe readonly int CustomBinarySearch(Variant item, Callable func, bool before = true)
        {
            VariantArray self = this;
            nint* args = stackalloc nint[4];
            args[0] = (nint)(&item);
            args[1] = (nint)(&func);
            byte beforeByte = before.ToExtBool();
            args[2] = (nint)(&beforeByte);
            int ret;
            __InternalCalls.bsearch_custom((nint)(&self), args, (nint)(&ret), 3);
            return ret;
        }
        public unsafe readonly int CustomBinarySearch(Variant item, CustomBinarySearchFunc func, bool before = true)
        {
            using Callable callable = (Callable)func;
            return CustomBinarySearch(item, func, before);
        }

        // void ()
        public unsafe readonly void Reverse()
        {
            VariantArray self = this;
            __InternalCalls.sort((nint)(&self), null, 0, 0);
        }

        // Array (bool deep)
        public unsafe readonly VariantArray Duplicate(bool deep = false)
        {
            VariantArray self = this;
            byte deepByte = deep.ToExtBool();
            byte* arg = &deepByte;
            VariantArray ret;
            __InternalCalls.duplicate((nint)(&self), (nint*)&arg, (nint)(&ret), 1);
            return ret;
        }

        // Array (int begin, int end, int step, bool deep)
        public unsafe readonly VariantArray Slice(int begin, int end = int.MaxValue, int step = 1, bool deep = false)
        {
            VariantArray self = this;
            nint* args = stackalloc nint[4];
            args[0] = (nint)(&begin);
            args[1] = (nint)(&end);
            args[2] = (nint)(&step);
            byte deepByte = deep.ToExtBool();
            args[3] = (nint)(&deepByte);
            VariantArray ret;
            __InternalCalls.duplicate((nint)(&self), args, (nint)(&ret), 4);
            return ret;
        }

        // Array (Callable method)
        public unsafe readonly VariantArray Filter(Callable func)
        {
            VariantArray self = this;
            Callable* arg = &func;
            VariantArray ret;
            __InternalCalls.filter((nint)(&self), (nint*)arg, (nint)(&ret), 1);
            return ret;
        }
        public unsafe readonly VariantArray Filter(FilterFunc func)
        {
            using Callable callable = (Callable)func;
            return Filter(callable);
        }

        // Array (Callable method)
        public unsafe readonly VariantArray Map(Callable func)
        {
            VariantArray self = this;
            Callable* arg = &func;
            VariantArray ret;
            __InternalCalls.map((nint)(&self), (nint*)arg, (nint)(&ret), 1);
            return ret;
        }
        public unsafe readonly VariantArray Map(MapFunc func)
        {
            using Callable callable = (Callable)func;
            return Map(callable);
        }

        // Variant (Callable method, Variant accum)
        public unsafe readonly Variant Reduce(Callable func, Variant accum = default)
        {
            VariantArray self = this;
            nint* args = stackalloc nint[2];
            args[0] = (nint)(&func);
            args[1] = (nint)(&accum);
            Variant ret;
            __InternalCalls.map((nint)(&self), args, (nint)(&ret), 1);
            return ret;
        }
        public unsafe readonly Variant Reduce(ReduceFunc func, Variant accum = default)
        {
            using Callable callable = (Callable)func;
            return Reduce(callable, accum);
        }

        // bool (Callable method)
        public unsafe readonly bool Any(Callable func)
        {
            VariantArray self = this;
            Callable* arg = &func;
            byte ret;
            __InternalCalls.any((nint)(&self), (nint*)&arg, (nint)(&ret), 1);
            return ret.ToBool();
        }
        public unsafe readonly bool Any(FilterFunc func)
        {
            using Callable callable = (Callable)func;
            return Any(callable);
        }

        // bool (Callable method)
        public unsafe readonly bool All(Callable func)
        {
            VariantArray self = this;
            Callable* arg = &func;
            byte ret;
            __InternalCalls.all((nint)(&self), (nint*)&arg, (nint)(&ret), 1);
            return ret.ToBool();
        }
        public unsafe readonly bool All(FilterFunc func)
        {
            using Callable callable = (Callable)func;
            return All(callable);
        }

        // Variant ()
        public unsafe readonly Variant Max
        {
            get
            {
                VariantArray self = this;
                Variant ret;
                __InternalCalls.max((nint)(&self), null, (nint)(&ret), 0);
                return ret;
            }
        }

        // Variant ()
        public unsafe readonly Variant Min
        {
            get
            {
                VariantArray self = this;
                Variant ret;
                __InternalCalls.min((nint)(&self), null, (nint)(&ret), 0);
                return ret;
            }
        }

        // bool ()
        public unsafe readonly bool IsTyped
        {
            [MImpl(MImplOpts.AggressiveInlining)]
            get
            {
                VariantArray self = this;
                byte ret;
                __InternalCalls.is_typed((nint)(&self), null, (nint)(&ret), 0);
                return ret.ToBool();
            }
        }

        // bool (Array array)
        public unsafe readonly bool IsSameTyped(VariantArray array)
        {
            VariantArray self = this;
            VariantArray* arg = &array;
            byte ret;
            __InternalCalls.is_same_typed((nint)(&self), (nint*)&arg, (nint)(&ret), 1);
            return ret.ToBool();
        }

        // int ()
        public unsafe readonly VariantType BuiltInType
        {
            [MImpl(MImplOpts.AggressiveInlining)]
            get
            {
                VariantArray self = this;
                int ret;
                __InternalCalls.is_typed((nint)(&self), null, (nint)(&ret), 0);
                return (VariantType)ret;
            }
        }

        // StringName ()
        public unsafe readonly StringName TypeClassName
        {
            [MImpl(MImplOpts.AggressiveInlining)]
            get
            {
                VariantArray self = this;
                StringName ret;
                __InternalCalls.get_typed_class_name((nint)(&self), null, (nint)(&ret), 0);
                return ret;
            }
        }

        // Variant ()
        public unsafe readonly Variant TypeScript
        {
            [MImpl(MImplOpts.AggressiveInlining)]
            get
            {
                VariantArray self = this;
                Variant ret;
                __InternalCalls.get_typed_script((nint)(&self), null, (nint)(&ret), 0);
                return ret;
            }
        }

        // void ()
        public unsafe readonly void MakeReadOnly()
        {
            VariantArray self = this;
            __InternalCalls.make_read_only((nint)(&self), null, 0, 0);
        }
    }
}
