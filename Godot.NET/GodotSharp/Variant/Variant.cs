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
            [FieldOffset(0)] public sbyte _sbyte;
            [FieldOffset(0)] public byte _byte;
            [FieldOffset(0)] public short _short;
            [FieldOffset(0)] public ushort _ushort;
            [FieldOffset(0)] public int _int;
            [FieldOffset(0)] public uint _uint;
            [FieldOffset(0)] public long _long;
            [FieldOffset(0)] public ulong _ulong;
            [FieldOffset(0)] public float _float;
            [FieldOffset(0)] public double _double;
            [FieldOffset(0)] public nint _nint;

            [FieldOffset(0)] public Transform2D* _Transform2D;
            [FieldOffset(0)] public Aabb* _Aabb;
            [FieldOffset(0)] public Basis* _Basis;
            [FieldOffset(0)] public Transform3D* _Transform3D;
            [FieldOffset(0)] public Projection* _Projection;

            [FieldOffset(0)] private VarMem _Mem;
            [FieldOffset(0)] public GDExtensionStringNamePtr _StringName;
            [FieldOffset(0)] public GDExtensionStringPtr _string;
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
            [FieldOffset(0)] public IntPtr _NodePath;
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
#pragma warning disable 169
                public Real _mem0;
                public Real _mem1;
                public Real _mem2;
                public Real _mem3;
#pragma warning restore 169
            }

            public override int GetHashCode()
                => _Mem.GetHashCode();
            public override bool Equals([NotNullWhen(true)] object obj)
                => obj is VarMem && Equals((VarMem)obj);
            public readonly bool Equals(UnionData other)
                => _Mem._mem0 == other._Mem._mem0 &&
                   _Mem._mem1 == other._Mem._mem1 &&
                   _Mem._mem2 == other._Mem._mem2 &&
                   _Mem._mem3 == other._Mem._mem3;
        }

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
        public bool Bool { [MImpl(MImplOpts.AggressiveInlining)] get => (bool)_data._bool; }

        /// <summary>Gets the <see cref="sbyte"/> value of this <see cref="Variant"/>.</summary>
        public sbyte Int8 { [MImpl(MImplOpts.AggressiveInlining)] get => (sbyte)_data._sbyte; }
        /// <summary>Gets the <see cref="short"/> value of this <see cref="Variant"/>.</summary>
        public short Int16 { [MImpl(MImplOpts.AggressiveInlining)] get => (short)_data._short; }
        /// <summary>Gets the <see cref="int"/> value of this <see cref="Variant"/>.</summary>
        public int Int32 { [MImpl(MImplOpts.AggressiveInlining)] get => (int)_data._int; }
        /// <summary>Gets the <see cref="long"/> value of this <see cref="Variant"/>.</summary>
        public long Int64 { [MImpl(MImplOpts.AggressiveInlining)] get => (long)_data._long; }

        /// <summary>Gets the <see cref="byte"/> value of this <see cref="Variant"/>.</summary>
        public byte UInt8 { [MImpl(MImplOpts.AggressiveInlining)] get => (byte)_data._byte; }
        /// <summary>Gets the <see cref="ushort"/> value of this <see cref="Variant"/>.</summary>
        public ushort UInt16 { [MImpl(MImplOpts.AggressiveInlining)] get => (ushort)_data._ushort; }
        /// <summary>Gets the <see cref="uint"/> value of this <see cref="Variant"/>.</summary>
        public uint UInt32 { [MImpl(MImplOpts.AggressiveInlining)] get => (uint)_data._uint; }
        /// <summary>Gets the <see cref="ulong"/> value of this <see cref="Variant"/>.</summary>
        public ulong UInt64 { [MImpl(MImplOpts.AggressiveInlining)] get => (ulong)_data._ulong; }

        /// <summary>Gets the <see cref="float"/> value of this <see cref="Variant"/>.</summary>
        public float Float32 { [MImpl(MImplOpts.AggressiveInlining)] get => (float)_data._float; }
        /// <summary>Gets the <see cref="double"/> value of this <see cref="Variant"/>.</summary>
        public double Float64 { [MImpl(MImplOpts.AggressiveInlining)] get => (double)_data._double; }

        /// <summary>Gets the <see cref="string"/> value of this <see cref="Variant"/>.</summary>
        /// <remarks>The result is cached; every call will return a reference to the same string.</remarks>
        public string String { [MImpl(MImplOpts.AggressiveInlining)] get { return new StringName(_data._string); } }
        /// <summary>Gets the <see cref="Godot.StringName"/> value of this <see cref="Variant"/>.</summary>
        public StringName StringName { [MImpl(MImplOpts.AggressiveInlining)] get => new StringName(_data._StringName); }
        /// <summary>Gets the <see cref="Godot.NodePath"/> value of this <see cref="Variant"/>.</summary>
        public NodePath NodePath { [MImpl(MImplOpts.AggressiveInlining)] get => new NodePath(_data._NodePath); }

        /// <summary>Gets the <see cref="Godot.Vector2"/> value of this <see cref="Variant"/>.</summary>
        public Vector2 Vector2 { [MImpl(MImplOpts.AggressiveInlining)] get => (Vector2)_data._Vector2; }
        /// <summary>Gets the <see cref="Godot.Vector3"/> value of this <see cref="Variant"/>.</summary>
        public Vector3 Vector3 { [MImpl(MImplOpts.AggressiveInlining)] get => (Vector3)_data._Vector3; }
        /// <summary>Gets the <see cref="Godot.Vector4"/> value of this <see cref="Variant"/>.</summary>
        public Vector4 Vector4 { [MImpl(MImplOpts.AggressiveInlining)] get => (Vector4)_data._Vector4; }

        /// <summary>Gets the <see cref="Godot.Vector2I"/> value of this <see cref="Variant"/>.</summary>
        public Vector2I Vector2I { [MImpl(MImplOpts.AggressiveInlining)] get => (Vector2I)_data._Vector2I; }
        /// <summary>Gets the <see cref="Godot.Vector3I"/> value of this <see cref="Variant"/>.</summary>
        public Vector3I Vector3I { [MImpl(MImplOpts.AggressiveInlining)] get => (Vector3I)_data._Vector3I; }
        /// <summary>Gets the <see cref="Godot.Vector4I"/> value of this <see cref="Variant"/>.</summary>
        public Vector4I Vector4I { [MImpl(MImplOpts.AggressiveInlining)] get => (Vector4I)_data._Vector4I; }

        /// <summary>Gets the <see cref="Godot.Color"/> value of this <see cref="Variant"/>.</summary>
        public Color Color { [MImpl(MImplOpts.AggressiveInlining)] get => (Color)_data._Color; }

        /// <summary>Gets the <see cref="Godot.Rect2"/> value of this <see cref="Variant"/>.</summary>
        public Rect2 Rect2 { [MImpl(MImplOpts.AggressiveInlining)] get => (Rect2)_data._Rect2; }
        /// <summary>Gets the <see cref="Godot.Rect2I"/> value of this <see cref="Variant"/>.</summary>
        public Rect2I Rect2I { [MImpl(MImplOpts.AggressiveInlining)] get => (Rect2I)_data._Rect2I; }
        /// <summary>Gets the <see cref="Godot.Aabb"/> value of this <see cref="Variant"/>.</summary>
        public unsafe Aabb Aabb { [MImpl(MImplOpts.AggressiveInlining)] get => (Aabb)(*_data._Aabb); }

        /// <summary>Gets the <see cref="Godot.Quaternion"/> value of this <see cref="Variant"/>.</summary>
        public Quaternion Quaternion { [MImpl(MImplOpts.AggressiveInlining)] get => (Quaternion)_data._Quaternion; }
        /// <summary>Gets the <see cref="Godot.Plane"/> value of this <see cref="Variant"/>.</summary>
        public Plane Plane { [MImpl(MImplOpts.AggressiveInlining)] get => (Plane)_data._Plane; }

        /// <summary>Gets the <see cref="Godot.Basis"/> value of this <see cref="Variant"/>.</summary>
        public unsafe Basis Basis { [MImpl(MImplOpts.AggressiveInlining)] get => (Basis)(*_data._Basis); }
        /// <summary>Gets the <see cref="Godot.Projection"/> value of this <see cref="Variant"/>.</summary>
        public unsafe Projection Projection { [MImpl(MImplOpts.AggressiveInlining)] get => (Projection)(*_data._Projection); }
        /// <summary>Gets the <see cref="Godot.Transform2D"/> value of this <see cref="Variant"/>.</summary>
        public unsafe Transform2D Transform2D { [MImpl(MImplOpts.AggressiveInlining)] get => (Transform2D)(*_data._Transform2D); }
        /// <summary>Gets the <see cref="Godot.Transform3D"/> value of this <see cref="Variant"/>.</summary>
        public unsafe Transform3D Transform3D { [MImpl(MImplOpts.AggressiveInlining)] get => (Transform3D)(*_data._Transform3D); }
        /// <summary>Gets the <see cref="Godot.GodotObject"/> value of this <see cref="Variant"/>.</summary>
        public unsafe GodotObject Object { [MImpl(MImplOpts.AggressiveInlining)]
            get
            {
                StringName name = new StringName();
                if (Main.i.ObjectGetClassName(_data._Object.obj, Main.lib, (nint)(void*)&name) == 0)
                {
                    nint scr = Main.i.ObjectGetScriptInstance(_data._Object.obj, CSharpLanguage.Singleton.Handle);
                    if (scr == 0)
                        return null;
                }

                //(GodotObject)(Main.i.ObjectGetInstanceBinding(_data._Object.id, Main.lib, );
                return null;
            }
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
        public override int GetHashCode()
            => _data.GetHashCode();
        /// <inheritdoc/>
        public override readonly bool Equals(object obj)
            => obj is Variant && Equals((Variant)obj);
        /// <inheritdoc/>
        public readonly bool Equals(Variant other)
            => _type == other._type && _data.Equals(other._data);

        /// <summary>Frees the resource controlled by this <see cref="Variant"/>.</summary>
        public void Dispose()
        {
            switch (_type)
            {
                case VariantType.Transform2D:
                case VariantType.Transform3D:
                case VariantType.AABB:
                case VariantType.Basis:
                case VariantType.Projection:
                    if (_data._nint != nint.Zero)
                    {
                        MemUtil.Free(_data._nint);
                        _data._nint = nint.Zero;
                    }
                    break;
            }
        }
    }
}
