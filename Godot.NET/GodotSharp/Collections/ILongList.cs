using System.Collections.Generic;

namespace Godot.Collections
{
    /// <summary>Like <see cref="IList{T}"/>, but all instances of <see cref="int"/> indexes are replaced with <see cref="nuint"/>.</summary>
    /// <typeparam name="T">The type of the elements in the list.</typeparam>
    public interface ILongList<T> : ILongCollection<T>
    {
        T this[nuint index] { get; set; }

        nuint IndexOf(T item);
        void Insert(nuint index, T item);
        void RemoveAt(nuint index);
    }
}
