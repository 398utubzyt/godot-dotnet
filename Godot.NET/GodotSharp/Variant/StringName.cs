using System;
using System.Diagnostics;
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
    public readonly partial struct StringName : IEquatable<StringName>
    {
        private readonly nint _data;

        internal StringName(nint data)
            => _data = data;

        /// <summary>Is <see langword="true"/> when this <see cref="StringName"/> is a valid string. Otherwise, <see langword="false"/>.</summary>
        public bool IsValid { [MImpl(MImplOpts.AggressiveInlining)] get => _data != 0; }
        /// <summary>Is <see langword="true"/> when this <see cref="StringName"/> is <see langword="null"/>. Otherwise, <see langword="false"/>.</summary>
        public bool IsNull { [MImpl(MImplOpts.AggressiveInlining)] get => _data == 0; }

        #region System.String Compatibility

        [StackTraceHidden]
        private static T ThrowNullAccess<T>()
#pragma warning disable CA2201
            => throw new NullReferenceException("Cannot dereference a null value.");
#pragma warning restore CA2201

        /// <inheritdoc cref="string.Length"/>
        public int Length { [MImpl(MImplOpts.AggressiveInlining)] get
                => IsValid ? ((string)this).Length : ThrowNullAccess<int>(); }
        /// <inheritdoc cref="string.IndexOf(char)"/>
        public int IndexOf(char c)
            => IsValid ? ((string)this).IndexOf(c) : ThrowNullAccess<int>();
        /// <inheritdoc cref="string.IndexOf(char, int)"/>
        public int IndexOf(char c, int start)
            => IsValid ? ((string)this).IndexOf(c, start) : ThrowNullAccess<int>();
        /// <inheritdoc cref="string.IndexOf(char, int, int)"/>
        public int IndexOf(char c, int start, int length)
            => IsValid ? ((string)this).IndexOf(c, start, length) : ThrowNullAccess<int>();
        /// <inheritdoc cref="string.IndexOf(string)"/>
        public int IndexOf(string s)
            => IsValid ? ((string)this).IndexOf(s, StringComparison.Ordinal) : ThrowNullAccess<int>();
        /// <inheritdoc cref="string.IndexOf(string, int)"/>
        public int IndexOf(string s, int start)
            => IsValid ? ((string)this).IndexOf(s, start, StringComparison.Ordinal) : ThrowNullAccess<int>();
        /// <inheritdoc cref="string.IndexOf(string, int, int)"/>
        public int IndexOf(string s, int start, int length)
            => IsValid ? ((string)this).IndexOf(s, start, length, StringComparison.Ordinal) : ThrowNullAccess<int>();
        /// <inheritdoc cref="MemoryExtensions.AsSpan(string)"/>
        public ReadOnlySpan<char> AsSpan()
            => IsValid ? ((string)this).AsSpan() : throw new NullReferenceException("Cannot dereference a null value.");

        #endregion

        #region Operators

        /// <inheritdoc/>
        [MImpl(MImplOpts.AggressiveInlining)]
        public static bool operator ==(StringName a, object b)
            => b is not StringName sn ? (b == null && a.IsNull) : a == sn;
        /// <inheritdoc/>
        [MImpl(MImplOpts.AggressiveInlining)]
        public static bool operator !=(StringName a, object b)
            => b is not StringName sn ? (b != null || a.IsValid) : a == sn;
        /// <inheritdoc/>
        [MImpl(MImplOpts.AggressiveInlining)]
        public static bool operator ==(object a, StringName b)
            => b == a;
        /// <inheritdoc/>
        [MImpl(MImplOpts.AggressiveInlining)]
        public static bool operator !=(object a, StringName b)
            => b != a;

        /// <inheritdoc/>
        [MImpl(MImplOpts.AggressiveInlining)]
        public static bool operator ==(StringName a, string b)
            => a.Equals((StringName)b);
        /// <inheritdoc/>
        [MImpl(MImplOpts.AggressiveInlining)]
        public static bool operator !=(StringName a, string b)
            => !a.Equals((StringName)b);
        /// <inheritdoc/>
        [MImpl(MImplOpts.AggressiveInlining)]
        public static bool operator ==(string a, StringName b)
            => b == a;
        /// <inheritdoc/>
        [MImpl(MImplOpts.AggressiveInlining)]
        public static bool operator !=(string a, StringName b)
            => b != a;

        /// <inheritdoc/>
        [MImpl(MImplOpts.AggressiveInlining)]
        public static bool operator ==(StringName a, StringName b)
            => a._data == b._data;
        /// <inheritdoc/>
        [MImpl(MImplOpts.AggressiveInlining)]
        public static bool operator !=(StringName a, StringName b)
            => a._data == b._data;

        /// <inheritdoc/>
        [MImpl(MImplOpts.AggressiveInlining)]
        public static implicit operator string(StringName sn)
            => sn.ToString();
        /// <inheritdoc/>
        [MImpl(MImplOpts.AggressiveInlining)]
        public static implicit operator StringName(string str)
            => SName.Register(str);
        /// <inheritdoc/>
        [MImpl(MImplOpts.AggressiveInlining)]
        public static explicit operator nint(StringName sn)
            => sn._data;

        #endregion

        /// <inheritdoc/>
        public override bool Equals([NotNullWhen(true)] object obj)
            => obj is StringName && Equals((StringName)obj);
        /// <inheritdoc/>
        [MImpl(MImplOpts.AggressiveInlining)]
        public bool Equals(StringName other)
            => _data == other._data;
        /// <inheritdoc/>
        [MImpl(MImplOpts.AggressiveInlining)]
        public override int GetHashCode()
            => _data.GetHashCode();
        /// <summary>Gets the string value of this <see cref="StringName"/>.</summary>
        /// <returns>The managed string representation of this <see cref="StringName"/>.</returns>
        [MImpl(MImplOpts.AggressiveInlining)]
        public override string ToString()
            => SName.SearchOrCreate(this);

        // Technically not supported, but it's faster than creating a new String every time so... :shrug:
        [SLayout(SLayoutOpt.Sequential)]
        internal unsafe struct _Data
        {
            public uint RefCount;
            public uint StaticCount;
            public byte* CName;
            public void* Name; // GodotString
#if DEBUG_ENABLED
            // public uint DebugReferences;
#endif
            public int Index;
            public uint Hash;
            public _Data* Previous;
            public _Data* Next;
            public readonly nint GetGodotString()
            { 
                return CName != null ? 
                    StringDB.Register(CStringDB.SearchOrCreate((nint)CName)) : 
                    (nint)Name;
            }
            public readonly nint GetCString()
            {
                return CName != null ? (nint)CName : CStringDB.Register(StringDB.SearchOrCreate((nint)Name));
            }
            public override readonly string ToString()
                => CName != null ? CStringDB.SearchOrCreate((nint)CName) : StringDB.SearchOrCreate((nint)Name);
            public _Data() { }
        }
    }
}
