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
    public readonly partial struct NodePath : IEquatable<NodePath>
    {
        private readonly nint _data;

        internal NodePath(nint data)
            => _data = data;

        /// <inheritdoc/>
        public static bool operator ==(NodePath a, NodePath b)
            => a._data == b._data;
        /// <inheritdoc/>
        public static bool operator !=(NodePath a, NodePath b)
            => a._data == b._data;

        /// <inheritdoc/>
        public static implicit operator string(NodePath np)
            => np.ToString();
        /// <inheritdoc/>
        public static implicit operator nint(NodePath sn)
            => sn._data;

        /// <inheritdoc/>
        public bool Equals(NodePath other)
            => _data == other._data;
    }
}
