using System.IO;

namespace GDExtensionGenerator
{
    public static class VariantGenerator
    {
        private enum ConversionType
        {
            None,
            Pointer,
            String,
            StringName,
        }

        private static void GenForType(StreamWriter w, string type, string fulltype, string name, string variant, ConversionType conv = ConversionType.None)
        {
            w.Write("        /// <summary> Tries to get the <see cref=\"");
            w.Write(type);
            w.Write("\"/> value from this <see cref=\"Variant\"/>.</summary>\n");
            w.Write("        /// <returns><see langword=\"true\"/> if successful, otherwise <see langword=\"false\"/>.</returns>\n        public bool TryAs");
            w.Write(name);
            w.Write("(out ");
            w.Write(fulltype);
            w.Write(" value)\n         {\n            if (_type != VariantType.");
            w.Write(variant);
            w.Write(")\n            {\n                value = default;\n                return false;\n            }\n            value = ");
            w.Write(name);
            w.Write(";\n            return true;\n        }\n        /// <inheritdoc/>\n        public static explicit operator ");
            w.Write(fulltype);
            w.Write("(Variant v)\n        => v.");
            w.Write(name);
            w.Write(";\n        /// <inheritdoc/>\n        public static implicit operator Variant(");
            w.Write(fulltype);
            w.Write(" x)\n            => new Variant(VariantType.");
            w.Write(variant);
            w.Write(", new UnionData() { _");
            w.Write(type);
            w.Write(" = ");
            switch (conv)
            {
                case ConversionType.Pointer: 
                    w.Write("(nint)x });\n\n");
                    break;

                case ConversionType.String:
                    w.Write("(nint)StringDB.Register(x) });\n\n");
                    break;

                case ConversionType.StringName:
                    w.Write("(nint)SName.Register(x) });\n\n");
                    break;

                default:
                    w.Write("x });\n\n");
                    break;
            }
        }

        public static int GenerateApi(string directory)
        {
            using FileStream fs = File.OpenWrite(Path.Join(directory, "Variant.gen.cs"));
            using StreamWriter w = new StreamWriter(fs);

            w.Write(GenPrelude);
            w.Write("    public partial struct Variant\n    {\n");

            GenForType(w, "bool", "bool", "Bool", "Bool");

            GenForType(w, "sbyte", "sbyte", "Int8", "Int");
            GenForType(w, "short", "short", "Int16", "Int");
            GenForType(w, "int", "int", "Int32", "Int");
            GenForType(w, "long", "long", "Int64", "Int");

            GenForType(w, "byte", "byte", "UInt8", "Int");
            GenForType(w, "ushort", "ushort", "UInt16", "Int");
            GenForType(w, "uint", "uint", "UInt32", "Int");
            GenForType(w, "ulong", "ulong", "UInt64", "Int");

            GenForType(w, "float", "float", "Float32", "Float");
            GenForType(w, "double", "double", "Float64", "Float");
            
            GenForType(w, "string", "string", "String", "String", ConversionType.String);
            GenForType(w, "StringName", "Godot.StringName", "StringName", "StringName", ConversionType.StringName);
            GenForType(w, "NodePath", "Godot.NodePath", "NodePath", "NodePath", ConversionType.StringName);

            GenForType(w, "Vector2", "Godot.Vector2", "Vector2", "Vector2");
            GenForType(w, "Vector3", "Godot.Vector3", "Vector3", "Vector3");
            GenForType(w, "Vector4", "Godot.Vector4", "Vector4", "Vector4");
            GenForType(w, "Vector2I", "Godot.Vector2I", "Vector2I", "Vector2I");
            GenForType(w, "Vector3I", "Godot.Vector3I", "Vector3I", "Vector3I");
            GenForType(w, "Vector4I", "Godot.Vector4I", "Vector4I", "Vector4I");

            GenForType(w, "Color", "Godot.Color", "Color", "Color");

            w.Write("    }\n");
            w.Write(GenPostlude);

            return 0;
        }
    }
}
