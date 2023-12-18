using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Godot
{
#pragma warning disable CA1720
    /// <summary>
    /// The most important data type in Godot.
    /// </summary>
    [SLayout(SLayoutOpt.Explicit)]
    public partial struct Variant : IEquatable<Variant>, IDisposable
    {
        public enum Type
        {
            Nil,

            Bool,
            Int,
            Float,
            String,

            Vector2,
            Vector2I,
            Rect2,
            Rect2I,
            Vector3,
            Vector3I,
            Transform2D,
            Vector4,
            Vector4I,
            Plane,
            Quaternion,
            AABB,
            Basis,
            Transform3D,
            Projection,

            Color,
            StringName,
            NodePath,
            Rid,
            Object,
            Callable,
            Signal,
            Dictionary,
            Array,

            PackedByteArray,
            PackedInt32Array,
            PackedInt64Array,
            PackedFloat32Array,
            PackedFloat64Array,
            PackedStringArray,
            PackedVector2Array,
            PackedVector3Array,
            PackedColorArray,

            Max
        }

        public enum Operator
        {
            Equal,
            NotEqual,
            Less,
            LessEqual,
            Greater,
            GreaterEqual,

            /* mathematic */
            Add,
            Subtract,
            Multiply,
            Divide,
            Negate,
            Positive,
            Module,
            Power,

            /* bitwise */
            ShiftLeft,
            ShiftRight,
            BitAnd,
            BitOr,
            BitXor,
            BitNegate,

            /* logic */
            And,
            Or,
            Xor,
            Not,

            /* containment */
            In,

            Max
        }

        [FieldOffset(0)]
        private VariantType _type;
        [FieldOffset(8)]
        private UnionData _data;

        private Variant(VariantType type, UnionData data)
        {
            _type = type;
            _data = data;
        }

        [SLayout(SLayoutOpt.Explicit)]
        private unsafe struct UnionData : IEquatable<UnionData>
        {
            [FieldOffset(0)] public bool _bool;
            [FieldOffset(0)] public long _int;
            [FieldOffset(0)] public double _float;
            [FieldOffset(0)] public nint _nint;

            [FieldOffset(0)] public Transform2D* _Transform2D;
            [FieldOffset(0)] public Aabb* _Aabb;
            [FieldOffset(0)] public Basis* _Basis;
            [FieldOffset(0)] public Transform3D* _Transform3D;
            [FieldOffset(0)] public Projection* _Projection;

            [FieldOffset(0)] private VarMem _Mem;
            [FieldOffset(0)] public StringName _StringName;
            [FieldOffset(0)] public nint _string;
            [FieldOffset(0)] public Vector4 _Vector4;
            [FieldOffset(0)] public Vector4I _Vector4I;
            [FieldOffset(0)] public Vector3 _Vector3;
            [FieldOffset(0)] public Vector3I _Vector3I;
            [FieldOffset(0)] public Vector2 _Vector2;
            [FieldOffset(0)] public Vector2I _Vector2I;
            [FieldOffset(0)] public Rect2 _Rect2;
            [FieldOffset(0)] public Rect2I _Rect2I;
            [FieldOffset(0)] public Plane _Plane;
            [FieldOffset(0)] public Quaternion _Quaternion;
            [FieldOffset(0)] public Color _Color;
            [FieldOffset(0)] public NodePath _NodePath;
            [FieldOffset(0)] public Rid _Rid;
            [FieldOffset(0)] public VarObject _Object;

            [StructLayout(LayoutKind.Sequential)]
            public struct VarObject
            {
                public ulong id;
                public IntPtr obj;
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct VarMem
            {
                public Real _mem0;
                public Real _mem1;
                public Real _mem2;
                public Real _mem3;
            }

            public override readonly int GetHashCode()
                => _Mem.GetHashCode();
            public override readonly bool Equals([NotNullWhen(true)] object obj)
                => obj is VarMem && Equals((VarMem)obj);
            public readonly bool Equals(UnionData other)
                => _Mem._mem0 == other._Mem._mem0 &&
                   _Mem._mem1 == other._Mem._mem1 &&
                   _Mem._mem2 == other._Mem._mem2 &&
                   _Mem._mem3 == other._Mem._mem3;
        }

        public static readonly Variant Null = new Variant();

        /// <summary>The type of this <see cref="Variant"/>.</summary>
        public readonly VariantType VariantType => _type;
        /// <summary>The type of this <see cref="Variant"/>.</summary>
        public readonly unsafe bool IsValid => _type switch {
            VariantType.Transform2D => _data._Transform2D != (void*)0,
            VariantType.Transform3D => _data._Transform3D != (void*)0,
            VariantType.AABB => _data._Aabb != (void*)0,
            VariantType.Basis => _data._Basis != (void*)0,
            VariantType.Projection => _data._Projection != (void*)0,
            VariantType.Object => _data._nint != nint.Zero,
            _ => true,
        };

        /// <summary>Gets the <see cref="sbyte"/> value of this <see cref="Variant"/>.</summary>
        public readonly bool Bool { [MImpl(MImplOpts.AggressiveInlining)] get => (bool)_data._bool; }

        /// <summary>Gets the <see cref="sbyte"/> value of this <see cref="Variant"/>.</summary>
        public readonly sbyte Int8 { [MImpl(MImplOpts.AggressiveInlining)] get => (sbyte)_data._int; }
        /// <summary>Gets the <see cref="short"/> value of this <see cref="Variant"/>.</summary>
        public readonly short Int16 { [MImpl(MImplOpts.AggressiveInlining)] get => (short)_data._int; }
        /// <summary>Gets the <see cref="int"/> value of this <see cref="Variant"/>.</summary>
        public readonly int Int32 { [MImpl(MImplOpts.AggressiveInlining)] get => (int)_data._int; }
        /// <summary>Gets the <see cref="long"/> value of this <see cref="Variant"/>.</summary>
        public readonly long Int64 { [MImpl(MImplOpts.AggressiveInlining)] get => (long)_data._int; }
        /// <summary>Gets the <see cref="nint"/> value of this <see cref="Variant"/>.</summary>
        public readonly nint IntPtr { [MImpl(MImplOpts.AggressiveInlining)] get => (nint)_data._int; }

        /// <summary>Gets the <see cref="byte"/> value of this <see cref="Variant"/>.</summary>
        public readonly byte UInt8 { [MImpl(MImplOpts.AggressiveInlining)] get => (byte)_data._int; }
        /// <summary>Gets the <see cref="ushort"/> value of this <see cref="Variant"/>.</summary>
        public readonly ushort UInt16 { [MImpl(MImplOpts.AggressiveInlining)] get => (ushort)_data._int; }
        /// <summary>Gets the <see cref="uint"/> value of this <see cref="Variant"/>.</summary>
        public readonly uint UInt32 { [MImpl(MImplOpts.AggressiveInlining)] get => (uint)_data._int; }
        /// <summary>Gets the <see cref="ulong"/> value of this <see cref="Variant"/>.</summary>
        public readonly ulong UInt64 { [MImpl(MImplOpts.AggressiveInlining)] get => (ulong)_data._int; }
        /// <summary>Gets the <see cref="nuint"/> value of this <see cref="Variant"/>.</summary>
        public readonly nuint UIntPtr { [MImpl(MImplOpts.AggressiveInlining)] get => (nuint)_data._int; }

        /// <summary>Gets the <see cref="float"/> value of this <see cref="Variant"/>.</summary>
        public readonly float Float32 { [MImpl(MImplOpts.AggressiveInlining)] get => (float)_data._float; }
        /// <summary>Gets the <see cref="double"/> value of this <see cref="Variant"/>.</summary>
        public readonly double Float64 { [MImpl(MImplOpts.AggressiveInlining)] get => (double)_data._float; }

        /// <summary>Gets the <see cref="string"/> value of this <see cref="Variant"/>.</summary>
        /// <remarks>The result is cached; every call will return a reference to the same string.</remarks>
        public readonly string String { [MImpl(MImplOpts.AggressiveInlining)] get { unsafe { return StringDB.SearchOrCreate(_data._string); } } }
        /// <summary>Gets the <see cref="Godot.StringName"/> value of this <see cref="Variant"/>.</summary>
        public readonly StringName StringName { [MImpl(MImplOpts.AggressiveInlining)] get => _data._StringName; }
        /// <summary>Gets the <see cref="Godot.NodePath"/> value of this <see cref="Variant"/>.</summary>
        public readonly NodePath NodePath { [MImpl(MImplOpts.AggressiveInlining)] get => _data._NodePath; }

        /// <summary>Gets the <see cref="Godot.Vector2"/> value of this <see cref="Variant"/>.</summary>
        public readonly Vector2 Vector2 { [MImpl(MImplOpts.AggressiveInlining)] get => (Vector2)_data._Vector2; }
        /// <summary>Gets the <see cref="Godot.Vector3"/> value of this <see cref="Variant"/>.</summary>
        public readonly Vector3 Vector3 { [MImpl(MImplOpts.AggressiveInlining)] get => (Vector3)_data._Vector3; }
        /// <summary>Gets the <see cref="Godot.Vector4"/> value of this <see cref="Variant"/>.</summary>
        public readonly Vector4 Vector4 { [MImpl(MImplOpts.AggressiveInlining)] get => (Vector4)_data._Vector4; }

        /// <summary>Gets the <see cref="Godot.Vector2I"/> value of this <see cref="Variant"/>.</summary>
        public readonly Vector2I Vector2I { [MImpl(MImplOpts.AggressiveInlining)] get => (Vector2I)_data._Vector2I; }
        /// <summary>Gets the <see cref="Godot.Vector3I"/> value of this <see cref="Variant"/>.</summary>
        public readonly Vector3I Vector3I { [MImpl(MImplOpts.AggressiveInlining)] get => (Vector3I)_data._Vector3I; }
        /// <summary>Gets the <see cref="Godot.Vector4I"/> value of this <see cref="Variant"/>.</summary>
        public readonly Vector4I Vector4I { [MImpl(MImplOpts.AggressiveInlining)] get => (Vector4I)_data._Vector4I; }

        /// <summary>Gets the <see cref="Godot.Color"/> value of this <see cref="Variant"/>.</summary>
        public readonly Color Color { [MImpl(MImplOpts.AggressiveInlining)] get => (Color)_data._Color; }

        /// <summary>Gets the <see cref="Godot.Rect2"/> value of this <see cref="Variant"/>.</summary>
        public readonly Rect2 Rect2 { [MImpl(MImplOpts.AggressiveInlining)] get => (Rect2)_data._Rect2; }
        /// <summary>Gets the <see cref="Godot.Rect2I"/> value of this <see cref="Variant"/>.</summary>
        public readonly Rect2I Rect2I { [MImpl(MImplOpts.AggressiveInlining)] get => (Rect2I)_data._Rect2I; }
        /// <summary>Gets the <see cref="Godot.Aabb"/> value of this <see cref="Variant"/>.</summary>
        public unsafe Aabb Aabb { [MImpl(MImplOpts.AggressiveInlining)] get => (Aabb)(*_data._Aabb); }

        /// <summary>Gets the <see cref="Godot.Quaternion"/> value of this <see cref="Variant"/>.</summary>
        public readonly Quaternion Quaternion { [MImpl(MImplOpts.AggressiveInlining)] get => (Quaternion)_data._Quaternion; }
        /// <summary>Gets the <see cref="Godot.Plane"/> value of this <see cref="Variant"/>.</summary>
        public readonly Plane Plane { [MImpl(MImplOpts.AggressiveInlining)] get => (Plane)_data._Plane; }

        /// <summary>Gets the <see cref="Godot.Basis"/> value of this <see cref="Variant"/>.</summary>
        public readonly unsafe Basis Basis { [MImpl(MImplOpts.AggressiveInlining)] get => (Basis)(*_data._Basis); }
        /// <summary>Gets the <see cref="Godot.Projection"/> value of this <see cref="Variant"/>.</summary>
        public readonly unsafe Projection Projection { [MImpl(MImplOpts.AggressiveInlining)] get => (Projection)(*_data._Projection); }
        /// <summary>Gets the <see cref="Godot.Transform2D"/> value of this <see cref="Variant"/>.</summary>
        public readonly unsafe Transform2D Transform2D { [MImpl(MImplOpts.AggressiveInlining)] get => (Transform2D)(*_data._Transform2D); }
        /// <summary>Gets the <see cref="Godot.Transform3D"/> value of this <see cref="Variant"/>.</summary>
        public readonly unsafe Transform3D Transform3D { [MImpl(MImplOpts.AggressiveInlining)] get => (Transform3D)(*_data._Transform3D); }
        /// <summary>Gets the <see cref="Godot.GodotObject"/> value of this <see cref="Variant"/>.</summary>
        public readonly unsafe GodotObject Object { [MImpl(MImplOpts.AggressiveInlining)] 
            get => ClassDB.GetOrMakeHandleFromNative(_data._Object.obj).Target as GodotObject; }

        public readonly bool TryAsObject(out GodotObject value)
        {
            if (_type != VariantType.Object)
            {
                value = null;
                return false;
            }
            value = Object;
            return true;
        }
        public readonly bool TryAsObject<T>(out T value) where T : GodotObject
        {
            if (_type != VariantType.Object || Object is not T tObj)
            {
                value = null;
                return false;
            }

            value = tObj;
            return true;
        }

        public readonly unsafe bool TryAs<[MustBeVariant] T>(out T value)
        {
            static bool SafeTryAsObject(ref readonly Variant self, ref T value)
            {
                if (!self.TryAsObject(out GodotObject obj) || obj is not T tObj)
                {
                    value = default;
                    return false;
                }

                value = tObj;
                return true;
            }

            value = default;
            return typeof(T).FullName switch
            {
                "System.Boolean" => TryAsBool(out MemUtil.As<T, bool>(ref value)),
                "System.SByte" => TryAsInt8(out MemUtil.As<T, sbyte>(ref value)),
                "System.Byte" => TryAsUInt8(out MemUtil.As<T, byte>(ref value)),
                "System.Int16" => TryAsInt16(out MemUtil.As<T, short>(ref value)),
                "System.UInt16" => TryAsUInt16(out MemUtil.As<T, ushort>(ref value)),
                "System.Int32" => TryAsInt32(out MemUtil.As<T, int>(ref value)),
                "System.UInt32" => TryAsUInt32(out MemUtil.As<T, uint>(ref value)),
                "System.Int64" => TryAsInt64(out MemUtil.As<T, long>(ref value)),
                "System.UInt64" => TryAsUInt64(out MemUtil.As<T, ulong>(ref value)),
                "System.Single" => TryAsFloat32(out MemUtil.As<T, float>(ref value)),
                "System.Double" => TryAsFloat64(out MemUtil.As<T, double>(ref value)),
                "System.String" => TryAsString(out MemUtil.As<T, string>(ref value)),
                "Godot.StringName" => TryAsStringName(out MemUtil.As<T, StringName>(ref value)),
                "Godot.NodePath" => TryAsNodePath(out MemUtil.As<T, NodePath>(ref value)),
                "Godot.Vector2" => TryAsVector2(out MemUtil.As<T, Vector2>(ref value)),
                "Godot.Vector3" => TryAsVector3(out MemUtil.As<T, Vector3>(ref value)),
                "Godot.Vector4" => TryAsVector4(out MemUtil.As<T, Vector4>(ref value)),
                "Godot.Vector2I" => TryAsVector2I(out MemUtil.As<T, Vector2I>(ref value)),
                "Godot.Vector3I" => TryAsVector3I(out MemUtil.As<T, Vector3I>(ref value)),
                "Godot.Vector4I" => TryAsVector4I(out MemUtil.As<T, Vector4I>(ref value)),
                "Godot.Color" => TryAsColor(out MemUtil.As<T, Color>(ref value)),
                _ => typeof(T).IsAssignableTo(typeof(GodotObject)) && SafeTryAsObject(in this, ref value)
            };
        }

        public static unsafe Variant From<[MustBeVariant] T>(T value)
        {
            return typeof(T).FullName switch
            {
                "System.Boolean" => value is bool t ? t : false,
                "System.SByte" => value is sbyte t ? t : (sbyte)0,
                "System.Byte" => value is byte t ? t : (byte)0,
                "System.Int16" => value is short t ? t : (short)0,
                "System.UInt16" => value is ushort t ? t : (ushort)0,
                "System.Int32" => value is int t ? t : 0,
                "System.UInt32" => value is uint t ? t : 0u,
                "System.Int64" => value is long t ? t : 0L,
                "System.UInt64" => value is ulong t ? t : 0ul,
                "System.Single" => value is float t ? t : 0.0f,
                "System.Double" => value is double t ? t : 0.0d,
                "System.String" => value is string t ? t : string.Empty,
                "Godot.StringName" => value is StringName t ? t : new(),
                "Godot.NodePath" => value is NodePath t ? t : new(),
                "Godot.Vector2" => value is Vector2 t ? t : Vector2.Zero,
                "Godot.Vector3" => value is Vector3 t ? t : Vector3.Zero,
                "Godot.Vector4" => value is Vector4 t ? t : Vector4.Zero,
                "Godot.Vector2I" => value is Vector2I t ? t : Vector2I.Zero,
                "Godot.Vector3I" => value is Vector3I t ? t : Vector3I.Zero,
                "Godot.Vector4I" => value is Vector4I t ? t : Vector4I.Zero,
                "Godot.Color" => value is Color t ? t : new(),
                _ => typeof(T).IsAssignableTo(typeof(GodotObject)) ? (value is GodotObject obj ? obj : null) : new Variant(),
            };
        }

        /// <inheritdoc/>
        public static explicit operator GodotObject(Variant v)
            => v.Object;
        /// <inheritdoc/>
        public static implicit operator Variant(GodotObject x)
            => new Variant(VariantType.Color, new UnionData() { _Object = new UnionData.VarObject() { id = 0, obj = x.Handle } });

        /// <inheritdoc/>
        public static bool operator ==(Variant a, Variant b)
            => a.Equals(b);
        /// <inheritdoc/>
        public static bool operator !=(Variant a, Variant b)
            => !a.Equals(b);

        /// <inheritdoc/>
        public override readonly int GetHashCode()
            => _data.GetHashCode();
        /// <inheritdoc/>
        public override readonly bool Equals(object obj)
            => obj is Variant && Equals((Variant)obj);
        /// <inheritdoc/>
        public readonly bool Equals(Variant other)
            => _type == other._type && _data.Equals(other._data);
        public override string ToString()
        {
            // Fixed address
            Variant self = this;
            nint str = 0;
            unsafe
            {
                Main.i.VariantStringify((nint)(&self), (nint)(&str));
            }
            return StringDB.SearchOrCreate(str);
        }

        /// <summary>Frees the resource controlled by this <see cref="Variant"/>.</summary>
        public void Dispose()
        {
            unsafe
            {
                // Fixed address
                Variant self = this;
                Main.i.VariantDestroy((nint)(&self));
            }
        }
    }
}
