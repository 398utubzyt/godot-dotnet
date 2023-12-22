// Putting this in a separate file to avoid clutter

namespace Godot.Collections
{
    partial struct TypedArray<T>
    {
        public delegate bool CustomSortFunc(T a, T b);
        public delegate bool FilterFunc(T item);
        public delegate T MapFunc(T item);
        public delegate T ReduceFunc(T accum, T item);
        public delegate bool CustomBinarySearchFunc(T item, T target);

        // bool ()
        public unsafe readonly bool IsEmpty
        {
            [MImpl(MImplOpts.AggressiveInlining)]
            get => _self.IsEmpty;
        }

        /// <summary>
        /// Assigns elements of another <paramref name="array"/> into the <see cref="VariantArray"/>.
        /// Resizes the <see cref="VariantArray"/> to match <paramref name="array"/>.
        /// Performs type conversions if the <see cref="VariantArray"/> is typed.
        /// </summary>
        /// <param name="array">The array to assign.</param>
        [MImpl(MImplOpts.AggressiveInlining)]
        public readonly void Assign(VariantArray array)
            => _self.Assign(array);
        [MImpl(MImplOpts.AggressiveInlining)]
        public readonly void Assign(TypedArray<T> array)
            => _self.Assign(array._self);

        /// <summary>
        /// Adds an element at the beginning of the array. See also <seealso cref="Add(Variant)"/>.
        /// </summary>
        /// <remarks>
        /// Note: On large arrays, this method is much slower than <see cref="Add(Variant)"/> as
        /// it will reindex all the array's elements every time it's called.
        /// The larger the array, the slower <see cref="Prepend(Variant)"/> will be.
        /// </remarks>
        /// <param name="value"></param>
        [MImpl(MImplOpts.AggressiveInlining)]
        public readonly void Prepend(T value)
            => _self.Prepend(Variant.From(value));

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
        [MImpl(MImplOpts.AggressiveInlining)]
        public readonly void AddRange(VariantArray array)
            => _self.AddRange(array);

        // int (int size)
        [MImpl(MImplOpts.AggressiveInlining)]
        public readonly int Resize(int size)
            => _self.Resize(size);

        // void (Variant value)
        [MImpl(MImplOpts.AggressiveInlining)]
        public readonly void Fill(Variant value)
            => _self.Fill(value);
        [MImpl(MImplOpts.AggressiveInlining)]
        public readonly void Fill(T value)
            => _self.Fill(Variant.From(value));

        /// <summary>
        /// Returns the first element of the array. Prints an error and returns <see langword="null"/> if the array is empty.
        /// </summary>
        public readonly T Front
        {
            [MImpl(MImplOpts.AggressiveInlining)]
            get => _self.Front.TryAs(out T value) ? value : default;
        }

        /// <summary>
        /// Returns the last element of the array. Prints an error and returns <see langword="null"/> if the array is empty.
        /// </summary>
        public readonly T Back
        {
            [MImpl(MImplOpts.AggressiveInlining)]
            get => _self.Back.TryAs(out T value) ? value : default;
        }

        // Variant ()
        [MImpl(MImplOpts.AggressiveInlining)]
        public readonly T PickRandom()
            => _self.PickRandom().TryAs(out T value) ? value : default;

        // int (Variant value)
        [MImpl(MImplOpts.AggressiveInlining)]
        public readonly int CountOf(Variant item)
            => _self.CountOf(item);
        [MImpl(MImplOpts.AggressiveInlining)]
        public readonly int CountOf(T item)
            => _self.CountOf(Variant.From(item));

        // Variant ()
        [MImpl(MImplOpts.AggressiveInlining)]
        public readonly T PopBack()
            => _self.PopBack().TryAs(out T value) ? value : default;

        // Variant ()
        [MImpl(MImplOpts.AggressiveInlining)]
        public readonly T PopFront()
            => _self.PopFront().TryAs(out T value) ? value : default;

        // Variant (int position)
        [MImpl(MImplOpts.AggressiveInlining)]
        public readonly T PopAt(int position)
            => _self.PopAt(position).TryAs(out T value) ? value : default;

        // void ()
        [MImpl(MImplOpts.AggressiveInlining)]
        public readonly void Sort()
            => _self.Sort();

        // void (Callable func)
        [MImpl(MImplOpts.AggressiveInlining)]
        public readonly void Sort(Callable func)
            => _self.Sort(func);
        [MImpl(MImplOpts.AggressiveInlining)]
        public readonly void Sort(CustomSortFunc func)
        {
            using Callable callable = (Callable)func;
            _self.Sort(callable);
        }

        // void ()
        [MImpl(MImplOpts.AggressiveInlining)]
        public unsafe readonly void Shuffle()
            => _self.Shuffle();

        // int (Variant value, bool before)
        [MImpl(MImplOpts.AggressiveInlining)]
        public readonly int BinarySearch(Variant item, bool before = true)
            => _self.BinarySearch(item, before);
        [MImpl(MImplOpts.AggressiveInlining)]
        public readonly int BinarySearch(T item, bool before = true)
            => _self.BinarySearch(Variant.From(item), before);

        // int (Variant value, Callable func, bool before)
        [MImpl(MImplOpts.AggressiveInlining)]
        public readonly int BinarySearch(Variant item, Callable func, bool before = true)
            => _self.CustomBinarySearch(item, func, before);
        [MImpl(MImplOpts.AggressiveInlining)]
        public readonly int CustomBinarySearch(T item, Callable func, bool before = true)
            => _self.CustomBinarySearch(Variant.From(item), func, before);
        [MImpl(MImplOpts.AggressiveInlining)]
        public readonly int CustomBinarySearch(T item, CustomBinarySearchFunc func, bool before = true)
        {
            Callable callable = (Callable)func;
            return CustomBinarySearch(item, callable, before);
        }

        // void ()
        [MImpl(MImplOpts.AggressiveInlining)]
        public unsafe readonly void Reverse()
            => _self.Reverse();

        // Array (bool deep)
        [MImpl(MImplOpts.AggressiveInlining)]
        public readonly TypedArray<T> Duplicate(bool deep = false)
            => _self.Duplicate(deep).TryAsTyped(out TypedArray<T> result) ? result : default;

        // Array (int begin, int end, int step, bool deep)
        [MImpl(MImplOpts.AggressiveInlining)]
        public readonly TypedArray<T> Slice(int begin, int end = int.MaxValue, int step = 1, bool deep = false)
            => _self.Slice(begin, end, step, deep).TryAsTyped(out TypedArray<T> result) ? result : default;

        // Array (Callable method)
        [MImpl(MImplOpts.AggressiveInlining)]
        public readonly TypedArray<T> Filter(Callable func)
            => _self.Filter(func).TryAsTyped(out TypedArray<T> result) ? result : default;
        [MImpl(MImplOpts.AggressiveInlining)]
        public readonly TypedArray<T> Filter(FilterFunc func)
        {
            using Callable callable = (Callable)func;
            return Filter(callable);
        }

        // Array (Callable method)
        [MImpl(MImplOpts.AggressiveInlining)]
        public readonly TypedArray<T> Map(Callable func)
            => _self.Map(func).TryAsTyped(out TypedArray<T> result) ? result : default;
        [MImpl(MImplOpts.AggressiveInlining)]
        public readonly TypedArray<T> Map(FilterFunc func)
        {
            using Callable callable = (Callable)func;
            return Map(callable);
        }

        // Variant (Callable method, Variant accum)
        [MImpl(MImplOpts.AggressiveInlining)]
        public readonly Variant Reduce(Callable func, Variant accum = default)
            => _self.Reduce(func, accum);
        [MImpl(MImplOpts.AggressiveInlining)]
        public readonly T Reduce(Callable func, T accum = default)
            => Reduce(func, Variant.From(accum)).TryAs(out T result) ? result : default;
        [MImpl(MImplOpts.AggressiveInlining)]
        public readonly T Reduce(ReduceFunc func, T accum = default)
        {
            using Callable callable = (Callable)func;
            return Reduce(callable, accum);
        }

        // bool (Callable method)
        [MImpl(MImplOpts.AggressiveInlining)]
        public readonly bool Any(Callable func)
            => _self.Any(func);
        [MImpl(MImplOpts.AggressiveInlining)]
        public readonly bool Any(FilterFunc func)
        {
            using Callable callable = (Callable)func;
            return Any(callable);
        }

        // bool (Callable method)
        [MImpl(MImplOpts.AggressiveInlining)]
        public readonly bool All(Callable func)
            => _self.All(func);
        [MImpl(MImplOpts.AggressiveInlining)]
        public readonly bool All(FilterFunc func)
        {
            using Callable callable = (Callable)func;
            return All(callable);
        }

        // Variant ()
        public readonly T Max
        {
            [MImpl(MImplOpts.AggressiveInlining)]
            get => _self.Max.TryAs(out T value) ? value : default;
        }

        // Variant ()
        public readonly T Min
        {
            [MImpl(MImplOpts.AggressiveInlining)]
            get => _self.Max.TryAs(out T value) ? value : default;
        }

        // bool ()
        public unsafe readonly bool IsTyped
        {
            [MImpl(MImplOpts.AggressiveInlining)]
            get => _self.IsTyped;
        }

        // bool (Array array)
        public unsafe readonly bool IsSameTyped(VariantArray array)
            => _self.IsSameTyped(array);

        // int ()
        public unsafe readonly VariantType BuiltInType
        {
            [MImpl(MImplOpts.AggressiveInlining)]
            get => _self.BuiltInType;
        }

        // StringName ()
        public unsafe readonly StringName TypeClassName
        {
            [MImpl(MImplOpts.AggressiveInlining)]
            get => _self.TypeClassName;
        }

        // Variant ()
        public unsafe readonly Variant TypeScript
        {
            [MImpl(MImplOpts.AggressiveInlining)]
            get => _self.TypeScript;
        }

        // void ()
        public unsafe readonly void MakeReadOnly()
            => _self.MakeReadOnly();
    }
}
