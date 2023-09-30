using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Godot
{
    /// <summary>
    /// The RID type is used to access a low-level resource by its unique ID.
    /// RIDs are opaque, which means they do not grant access to the resource
    /// by themselves. They are used by the low-level server classes, such as
    /// <see cref="DisplayServer"/>, <see cref="RenderingServer"/>,
    /// <see cref="TextServer"/>, etc.
    ///
    /// A low-level resource may correspond to a high-level <see cref="Resource"/>,
    /// such as <see cref="Texture"/> or <see cref="Mesh"/>
    /// </summary>
    [SLayout(SLayoutOpt.Sequential)]
    public readonly struct Rid : IEquatable<Rid>
    {
        private readonly ulong _id;

        /// <summary>
        /// Constructs a new <see cref="Rid"/> for the given <see cref="GodotObject"/> <paramref name="obj"/>.
        /// </summary>
        public Rid(Resource obj)
            => _id = obj.GetRid()._id;

        /// <summary>
        /// Returns the ID of the referenced low-level resource.
        /// </summary>
        /// <returns>The ID of the referenced resource.</returns>
        public ulong Id => _id;
        /// <summary>
        /// Returns <see langword="true"/> if the <see cref="Rid"/> is not <c>0</c>.
        /// </summary>
        /// <returns>Whether or not the ID is valid.</returns>
        public bool IsValid => _id != 0;

        /// <summary>
        /// Converts this RID into a primitive number type <see langword="ulong"/>.
        /// </summary>
        /// <param name="rid">The RID to convert.</param>
        public static implicit operator ulong(Rid rid)
            => rid._id;
        /// <summary>
        /// Gets the the RID of the resource.
        /// </summary>
        /// <param name="resource">The resource to get the RID of.</param>
        public static implicit operator Rid(Resource resource)
            => resource?.GetRid() ?? default;

        /// <summary>
        /// Returns <see langword="true"/> if both <see cref="Rid"/>s are equal,
        /// which means they both refer to the same low-level resource.
        /// </summary>
        /// <param name="left">The left RID.</param>
        /// <param name="right">The right RID.</param>
        /// <returns>Whether or not the RIDs are equal.</returns>
        public static bool operator ==(Rid left, Rid right)
            => left._id == right._id;
        /// <summary>
        /// Returns <see langword="true"/> if the <see cref="Rid"/>s are not equal.
        /// </summary>
        /// <param name="left">The left RID.</param>
        /// <param name="right">The right RID.</param>
        /// <returns>Whether or not the RIDs are equal.</returns>
        public static bool operator !=(Rid left, Rid right)
            => left._id != right._id;
        /// <summary>
        /// Returns <see langword="true"/> if the RIDs are equal.
        /// </summary>
        /// <param name="other">The other RID.</param>
        /// <returns>Whether or not the RIDs are equal.</returns>
        public bool Equals(Rid other)
            => _id == other._id;

        /// <summary>
        /// Returns <see langword="true"/> if this RID and <paramref name="obj"/> are equal.
        /// </summary>
        /// <param name="obj">The other object to compare.</param>
        /// <returns>Whether or not the color and the other object are equal.</returns>
        public override bool Equals([NotNullWhen(true)] object obj)
            => obj is Rid rid && Equals(rid);
        /// <summary>
        /// Serves as the hash function for <see cref="Rid"/>.
        /// </summary>
        /// <returns>A hash code for this RID.</returns>
        public override readonly int GetHashCode()
            => HashCode.Combine(_id);
        /// <summary>
        /// Converts this <see cref="Rid"/> to a string.
        /// </summary>
        /// <returns>A string representation of this Rid.</returns>
        public override string ToString()
            => $"RID({_id})";
    }
}
