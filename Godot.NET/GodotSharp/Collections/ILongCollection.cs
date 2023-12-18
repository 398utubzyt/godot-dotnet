using System.Collections.Generic;

namespace Godot.Collections
{/// <summary>Like <see cref="ICollection{T}"/>, but all instances of <see cref="int"/> indexes are replaced with <see cref="nint"/>.</summary>
 /// <typeparam name="T">The type of the elements in the collection.</typeparam>
    public interface ILongCollection<T> : IEnumerable<T>, System.Collections.IEnumerable
    {
        nuint Count { get; }
        bool IsReadOnly { get; }

        void Add(T item);
        void Clear();
        bool Contains(T item);
        void CopyTo(T[] array, nuint arrayIndex);
        bool Remove(T item);
    }
}
