using System;
using System.Diagnostics.CodeAnalysis;

namespace Godot
{
    /// <summary>
    /// StringNames are immutable strings designed for general-purpose representation of unique names.
    /// StringName ensures that only one instance of a given name exists (so two StringNames with the
    /// same value are the same object).
    /// Comparing them is much faster than with regular strings, because only the pointers are compared,
    /// not the whole strings.
    /// </summary>
    [SLayout(SLayoutOpt.Sequential)]
    public readonly partial struct Signal : IEquatable<Signal>
    {
        private readonly nint _data;

        internal Signal(nint data)
            => _data = data;

        /// <inheritdoc/>
        public static bool operator ==(Signal a, Signal b)
            => a._data == b._data;
        /// <inheritdoc/>
        public static bool operator !=(Signal a, Signal b)
            => a._data == b._data;

        /// <inheritdoc/>
        public static implicit operator string(Signal np)
            => np.ToString();
        /// <inheritdoc/>
        public static implicit operator nint(Signal sn)
            => sn._data;

        /// <inheritdoc/>
        public bool Equals(Signal other)
            => _data == other._data;

        /// <inheritdoc/>
        public override bool Equals([NotNullWhen(true)] object obj)
            => obj is Signal && Equals((Signal)obj);
        /// <inheritdoc/>
        public override int GetHashCode()
            => _data.GetHashCode();
    }
}
