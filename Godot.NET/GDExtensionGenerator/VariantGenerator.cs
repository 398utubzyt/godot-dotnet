using System.IO;

namespace GDExtensionGenerator
{
    public static class VariantGenerator
    {
        private enum ConversionType
        {
            None,
            LargeMem,
            String,
            Packed,
            Bool,
        }

        private static void GenForType(StreamWriter w, string fulltype, string data, string real, 
            string name, string variant, ConversionType conv = ConversionType.None)
        {
            w.Write("        /// <summary> Tries to get the <see cref=\"");
            w.Write(fulltype);
            w.Write("\"/> value from this <see cref=\"Variant\"/>.</summary>\n");
            w.Write("        /// <returns><see langword=\"true\"/> if successful, otherwise ");
            w.Write("<see langword=\"false\"/>.</returns>\n        public readonly bool TryAs");
            w.Write(name);
            w.Write("(out ");
            w.Write(fulltype);
            w.Write(" value)\n         {\n            if (_type != VariantType.");
            w.Write(variant);
            w.Write(")\n            {\n                value = default;\n                return false;\n            }\n            value = ");
            w.Write(name);
            w.Write(";\n            return IsValid;\n        }\n        /// <inheritdoc/>\n        public static explicit operator ");
            w.Write(fulltype);
            w.Write("(Variant v)\n        => v.");
            w.Write(name);
            w.Write(";\n        /// <inheritdoc/>\n        public static ");
            w.Write(conv switch
            {
                ConversionType.Packed => "unsafe explicit",
                _ => "implicit",
            });
            w.Write(" operator Variant(");
            w.Write(fulltype);
            w.Write(" x)\n            => ");

            if (conv == ConversionType.LargeMem)
            {
                w.Write("UnionData.LargeMemHelper.Create(x);\n\n");
                return;
            }

            w.Write("new Variant(VariantType.");
            w.Write(variant);
            w.Write(", new UnionData() { _");
            w.Write(data);
            w.Write(" = ");
            switch (conv)
            {
                case ConversionType.String:
                    w.Write("(nint)StringDB.Register(x) });\n\n");
                    break;

                case ConversionType.Packed:
                    w.Write("UnionData.PackedArrayRef.Create(x) });\n\n");
                    break;

                case ConversionType.Bool:
                    w.Write("x.ToExtBool() });\n\n");
                    break;

                default:
                    w.Write('(');
                    w.Write(real);
                    w.Write(")x });\n\n");
                    break;
            }
        }

        public static int GenerateApi(string directory)
        {
            using FileStream fs = File.OpenWrite(Path.Join(directory, "Variant.conv.cs"));
            using StreamWriter w = new StreamWriter(fs);

            w.Write(GenPrelude);
            w.Write("    public partial struct Variant\n    {\n");

            GenForType(w, "bool", "bool", "bool", "Bool", "Bool");

            GenForType(w, "sbyte", "int", "long", "Int8", "Int");
            GenForType(w, "short", "int", "long", "Int16", "Int");
            GenForType(w, "int", "int", "long", "Int32", "Int");
            GenForType(w, "long", "int", "long", "Int64", "Int");

            GenForType(w, "byte", "int", "long", "UInt8", "Int");
            GenForType(w, "ushort", "int", "long", "UInt16", "Int");
            GenForType(w, "uint", "int", "long", "UInt32", "Int");
            GenForType(w, "ulong", "int", "long", "UInt64", "Int");

            GenForType(w, "float", "float", "double", "Float32", "Float");
            GenForType(w, "double", "float", "double", "Float64", "Float");
            
            GenForType(w, "string", "string", "string", "String", "String", ConversionType.String);
            GenForType(w, "Godot.StringName", "StringName", "StringName", "StringName", "StringName");
            GenForType(w, "Godot.NodePath", "NodePath", "NodePath", "NodePath", "NodePath");

            GenForType(w, "Godot.Vector2", "Vector2", "Vector2", "Vector2", "Vector2");
            GenForType(w, "Godot.Vector3", "Vector3", "Vector3", "Vector3", "Vector3");
            GenForType(w, "Godot.Vector4", "Vector4", "Vector4", "Vector4", "Vector4");
            GenForType(w, "Godot.Vector2I", "Vector2I", "Vector2I", "Vector2I", "Vector2I");
            GenForType(w, "Godot.Vector3I", "Vector3I", "Vector3I", "Vector3I", "Vector3I");
            GenForType(w, "Godot.Vector4I", "Vector4I", "Vector4I", "Vector4I", "Vector4I");
            GenForType(w, "Godot.Color", "Color", "Color", "Color", "Color");

            GenForType(w, "Godot.Rect2", "Rect2", "Rect2", "Rect2", "Rect2");
            GenForType(w, "Godot.Rect2I", "Rect2I", "Rect2I", "Rect2I", "Rect2I");
            GenForType(w, "Godot.Aabb", "Aabb", "Aabb", "Aabb", "AABB", ConversionType.LargeMem);
            GenForType(w, "Godot.Quaternion", "Quaternion", "Quaternion", "Quaternion", "Quaternion");
            GenForType(w, "Godot.Plane", "Plane", "Plane", "Plane", "Plane");
            GenForType(w, "Godot.Basis", "Basis", "Basis", "Basis", "Basis", ConversionType.LargeMem);
            GenForType(w, "Godot.Projection", "Projection", "Projection", "Projection", "Projection", ConversionType.LargeMem);
            GenForType(w, "Godot.Transform2D", "Transform2D", "Transform2D", "Transform2D", "Transform2D", ConversionType.LargeMem);
            GenForType(w, "Godot.Transform3D", "Transform3D", "Transform3D", "Transform3D", "Transform3D", ConversionType.LargeMem);

            GenForType(w, "Godot.Rid", "Rid", "Rid", "Rid", "Rid");
            GenForType(w, "Godot.Callable", "Callable", "Callable", "Callable", "Callable");
            GenForType(w, "Godot.Signal", "Signal", "Signal", "Signal", "Signal");
            GenForType(w, "Godot.Collections.VariantDictionary", "Dictionary",
                "VariantDictionary", "Dictionary", "Dictionary");
            GenForType(w, "Godot.Collections.VariantArray", "Array",
                "VariantArray", "Array", "Array");

            GenForType(w, "Godot.Collections.PackedByteArray", "Packed",
                          "PackedByteArray", "PackedByteArray", "PackedByteArray", ConversionType.Packed);
            GenForType(w, "Godot.Collections.PackedColorArray", "Packed",
                          "PackedColorArray", "PackedColorArray", "PackedColorArray", ConversionType.Packed);
            GenForType(w, "Godot.Collections.PackedFloat32Array", "Packed",
                          "PackedFloat32Array", "PackedFloat32Array", "PackedFloat32Array", ConversionType.Packed);
            GenForType(w, "Godot.Collections.PackedFloat64Array", "Packed",
                          "PackedFloat64Array", "PackedFloat64Array", "PackedFloat64Array", ConversionType.Packed);
            GenForType(w, "Godot.Collections.PackedInt32Array", "Packed",
                          "PackedInt32Array", "PackedInt32Array", "PackedInt32Array", ConversionType.Packed);
            GenForType(w, "Godot.Collections.PackedInt64Array", "Packed",
                          "PackedInt64Array", "PackedInt64Array", "PackedInt64Array", ConversionType.Packed);
            GenForType(w, "Godot.Collections.PackedStringArray", "Packed",
                          "PackedStringArray", "PackedStringArray", "PackedStringArray", ConversionType.Packed);
            GenForType(w, "Godot.Collections.PackedVector2Array", "Packed",
                          "PackedVector2Array", "PackedVector2Array", "PackedVector2Array", ConversionType.Packed);
            GenForType(w, "Godot.Collections.PackedVector3Array", "Packed",
                          "PackedVector3Array", "PackedVector3Array", "PackedVector3Array", ConversionType.Packed);

            w.Write("    }\n");
            w.Write(GenPostlude);

            return 0;
        }
    }
}
