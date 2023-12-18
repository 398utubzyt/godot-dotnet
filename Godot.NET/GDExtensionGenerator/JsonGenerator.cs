using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Xml.Linq;

namespace GDExtensionGenerator
{
    public static class JsonGenerator
    {
        private static bool WriteTab(StreamWriter w, int tab = 1)
        {
            for (int i = 0; i < tab; i++)
                w.Write("    ");
            return true;
        }

        private static void EnumFixCasing(Span<char> csname)
        {
            FixCasing(csname, "left");
            FixCasing(csname, "right");
            FixCasing(csname, "up");
            FixCasing(csname, "down");
            FixCasing(csname, "play");
            FixCasing(csname, "stop");
            FixCasing(csname, "mute");
            FixCasing(csname, "previous");
            FixCasing(csname, "next");
            FixCasing(csname, "lock");
            FixCasing(csname, "tilde");
        }

        private static bool WriteEnum(StreamWriter w, string name, Span<char> valBuffer, JToken[] values, bool flags, int tab = 1)
        {
            if (flags)
            {
                WriteTab(w, tab);
                w.Write("[Flags]\n");
            }

            WriteTab(w, tab);
            w.Write("public enum ");
            w.Write(name);
            w.Write(" : long\n");
            WriteTab(w, tab++);
            w.Write("{\n");

            for (int j = 0; j < values.Length; j++)
            {
                WriteTab(w, tab);
                Span<char> csname = valBuffer[..EnumCSharpify((string)values[j]["name"], ref valBuffer, name)];

                FixCasing(csname, "left");
                FixCasing(csname, "right");
                FixCasing(csname, "up");
                FixCasing(csname, "down");
                FixCasing(csname, "play");
                FixCasing(csname, "stop");
                FixCasing(csname, "mute");
                FixCasing(csname, "previous");
                FixCasing(csname, "next");
                FixCasing(csname, "lock");
                FixCasing(csname, "tilde");

                w.Write(csname);
                w.Write(" = ");
                w.Write((int)values[j]["value"]);
                w.Write(",\n");
            }

            WriteTab(w, --tab);
            w.Write("}\n");
            return true;
        }

        private static bool GenerateEnums(JObject api, string directory)
        {
            Span<char> cname = stackalloc char[256];
            JToken[] enums = api["global_enums"].ToObject<JToken[]>();
            for (long i = 0; i < enums.LongLength; i++)
            {

                string ename = (string)enums[i]["name"];
                if (ename.Contains('.'))
                    continue;

                bool bitzo = (bool)enums[i]["is_bitfield"];
                JToken[] values = enums[i]["values"].ToObject<JToken[]>();

                string path = Path.Join(directory.AsSpan(), $"{ename}.cs");
                using FileStream fs = File.OpenWrite(path);
                using StreamWriter w = new StreamWriter(fs);

                w.Write(GenPrelude);

                if (!WriteEnum(w, ename, cname, values, bitzo, 1))
                    return false;
                
                w.Write(GenPostlude);

                w.Flush();
            }

            return true;
        }

        private struct ParameterInfo
        {
            public string Name;
            public string Type;
            public string Meta;
        }

        private struct SignalInfo
        {
            public string Name;
        }

        private struct PropertyInfo
        {
            public string Name;
            public string Type;
            public string Getter;
            public string Setter;
        }

        private struct EnumInfo
        {
            public string Name;
            public bool Flags;
            public List<ParameterInfo> Values;
        }

        private struct MethodInfo
        {
            public string Name;
            public bool IsConst;
            public bool IsVarArg;
            public bool IsStatic;
            public bool IsVirtual;
            public ulong Hash;
            public string Return;
            public string ReturnMeta;
            public List<ParameterInfo> Params;

            public int Index;
        }

        private struct ClassInfo
        {
            public string Name;
            public bool RefCounted;
            public bool Instantiatable;
            public bool Singleton;
            public string Inherits;
            public bool Editor;
            public List<EnumInfo> Enums;
            public List<SignalInfo> Signals;
            public List<MethodInfo> Methods;
            public List<PropertyInfo> Properties;
        }

        private static bool WriteEnum(StreamWriter w, EnumInfo info, Span<char> valBuffer, int tab = 1)
        {
            if (info.Flags)
            {
                WriteTab(w, tab);
                w.Write("[Flags]\n");
            }

            WriteTab(w, tab);
            w.Write("public enum ");
            w.Write(info.Name);
            w.Write(" : long\n");
            WriteTab(w, tab++);
            w.Write("{\n");

            for (int j = 0; j < info.Values.Count; j++)
            {
                WriteTab(w, tab);
                Span<char> csname = valBuffer[..EnumCSharpify(info.Values[j].Name, ref valBuffer, info.Name)];

                EnumFixCasing(csname);

                w.Write(csname);
                w.Write(" = ");
                w.Write(info.Values[j].Type);
                w.Write(",\n");
            }

            WriteTab(w, --tab);
            w.Write("}\n");
            return true;
        }

        private static void MethodCorrectType(ref string str, string type, string correct)
            => str = str.Replace(type, correct);
        private static void MethodFixType(ref string type)
        {
            MethodCorrectType(ref type, "enum::", string.Empty);
            MethodCorrectType(ref type, "bitfield::", string.Empty);

            if (type.StartsWith("typedarray::"))
                type = $"{type[12..]}[]";

            MethodCorrectType(ref type, "uint8_t", "byte");
            MethodCorrectType(ref type, "uint16_t", "ushort");
            MethodCorrectType(ref type, "uint32_t", "uint");
            MethodCorrectType(ref type, "uint64_t", "ulong");

            MethodCorrectType(ref type, "int8_t", "sbyte");
            MethodCorrectType(ref type, "int16_t", "short");
            MethodCorrectType(ref type, "int32_t", "int");
            MethodCorrectType(ref type, "int64_t", "long");

            MethodCorrectType(ref type, "Vector2i", "Vector2I");
            MethodCorrectType(ref type, "Vector3i", "Vector3I");
            MethodCorrectType(ref type, "Vector4i", "Vector4I");
            MethodCorrectType(ref type, "Rect2i", "Rect2I");
            MethodCorrectType(ref type, "AABB", "Aabb");

            MethodCorrectType(ref type, "RID", "Rid");

            if (type == "String")
                type = "string";
            if (type == "Object")
                type = "GodotObject";

            MethodCorrectType(ref type, "Span<void>", "Span<byte>");
        }
        private static void MethodFixTypeWithMeta(ref string type, string meta)
        {
            switch (meta)
            {
                case "double":
                    MethodCorrectType(ref type, "float", "double");
                    break;
                case "int8":
                    MethodCorrectType(ref type, "int", "sbyte");
                    break;
                case "int16":
                    MethodCorrectType(ref type, "int", "short");
                    break;
                case "int64":
                    MethodCorrectType(ref type, "int", "long");
                    break;
                case "uint8":
                    MethodCorrectType(ref type, "int", "byte");
                    break;
                case "uint16":
                    MethodCorrectType(ref type, "int", "ushort");
                    break;
                case "uint32":
                    MethodCorrectType(ref type, "int", "uint");
                    break;
                case "uint64":
                    MethodCorrectType(ref type, "int", "ulong");
                    break;
            }
        }
        private static void MethodTypeToNative(ref string type)
        {
            MethodCorrectType(ref type, "string", "void*");

            if (type.Contains("Span<") && type.EndsWith('>'))
                type = "void*";
        }
        private static bool WriteSingletonMethod(StreamWriter w, MethodInfo info, int tab = 1)
        {
            WriteTab(w, tab);
            w.Write("public static unsafe ");
            w.Write(info.Return);
            w.Write(' ');
            w.Write(info.Name);
            w.Write('(');

            for (int i = 0; i < info.Params.Count; i++)
            {
                w.Write(info.Params[i].Type);
                w.Write(' ');
                w.Write(info.Params[i].Name);
                if (i < info.Params.Count - 1 || info.IsVarArg)
                    w.Write(", ");
            }

            if (info.IsVarArg)
                w.Write("params Variant[] varargs");

            w.Write(")\n");

            WriteTab(w, ++tab);
            w.Write("=> Singleton.__instance_");
            w.Write(info.Name);
            w.Write("(");

            for (int i = 0; i < info.Params.Count; i++)
            {
                w.Write(info.Params[i].Name);
                if (i < info.Params.Count - 1 || info.IsVarArg)
                    w.Write(", ");
            }

            if (info.IsVarArg)
                w.Write("varargs");

            w.Write(");\n");
            return true;
        }
        private static bool WriteMethod(StreamWriter w, JToken[] builtins, JToken[] enums, 
            ref MethodInfo info, bool instanced, Span<char> buffer, int tab = 1)
        {
            WriteTab(w, tab);
            w.Write("[global::Godot.ExposeAs(Name = \"");
            w.Write(info.Name);
            w.Write("\")]\n");
            WriteTab(w, tab);
            if (instanced)
                w.Write("private ");
            else
                w.Write("public ");
            if (info.Return?.Contains('*') ?? false)
                w.Write("unsafe ");
            if (info.Name == "_to_string" || info.Name == "to_string")
                w.Write("override ");
            if (info.IsVirtual)
                w.Write("virtual ");
            if (info.IsStatic)
                w.Write("static ");

            string ret = info.Return?.StartsWith("const ") ?? false ? info.Return[6..] : info.Return;
            if (ret != null)
            {
                MethodFixType(ref ret);
                if (info.ReturnMeta != null)
                    MethodFixTypeWithMeta(ref ret, info.ReturnMeta);
            } else
                ret = "void";
            w.Write(ret);
            info.Return = ret;

            w.Write(' ');

            Span<char> name = buffer[(info.Name == "_to_string" ? 1 : 0)..CSharpify(info.Name, ref buffer)];
            if (instanced)
                w.Write("__instance_");
            w.Write(name);
            info.Name = name.ToString();

            w.Write('(');
            for (int i = 0; i < info.Params.Count; i++)
            {
                string type;
                if (info.Params[i].Type.EndsWith("*"))
                {
                    if (!info.Params[i].Type.EndsWith(" **"))
                    {
                        type = info.Params[i].Type.StartsWith("const ") ?
                            $"ReadOnlySpan<{info.Params[i].Type[6..^1]}>" :
                            $"Span<{info.Params[i].Type[..^1]}>";
                    } else
                        type = info.Params[i].Type.StartsWith("const ") ?
                            $"ref {info.Params[i].Type[6..^3]}[]" :
                            $"ref {info.Params[i].Type[..^3]}[]";
                } else
                    type = info.Params[i].Type;

                MethodFixType(ref type);
                if (info.Params[i].Meta != null)
                    MethodFixTypeWithMeta(ref type, info.Params[i].Meta);

                w.Write(type);
                w.Write(' ');
                name = buffer[..CSharpify(info.Params[i].Name, ref buffer)];
                name[0] = char.ToLower(name[0]);
                if (name.SequenceEqual("class") ||
                    name.SequenceEqual("ref") ||
                    name.SequenceEqual("out") ||
                    name.SequenceEqual("in") ||
                    name.SequenceEqual("event") ||
                    name.SequenceEqual("string") ||
                    name.SequenceEqual("enum") ||
                    name.SequenceEqual("private") ||
                    name.SequenceEqual("public") ||
                    name.SequenceEqual("internal") ||
                    name.SequenceEqual("protected") ||
                    name.SequenceEqual("override") ||
                    name.SequenceEqual("option") ||
                    name.SequenceEqual("sealed") ||
                    name.SequenceEqual("virtual") ||
                    name.SequenceEqual("abstract") ||
                    name.SequenceEqual("interface") ||
                    name.SequenceEqual("default") ||
                    name.SequenceEqual("readonly") ||
                    name.SequenceEqual("volatile") ||
                    name.SequenceEqual("char") ||
                    name.SequenceEqual("params") ||
                    name.SequenceEqual("lock") ||
                    name.SequenceEqual("base") ||
                    name.SequenceEqual("checked") ||
                    name.SequenceEqual("unchecked") ||
                    name.SequenceEqual("bool") ||
                    name.SequenceEqual("operator") ||
                    name.SequenceEqual("object")
                )
                {
                    name = buffer[..(name.Length + 1)];
                    for (int j = name.Length - 1; j > 0; )
                        name[j] = name[--j];
                    name[0] = '@';
                }
                w.Write(name);
                info.Params[i] = new ParameterInfo() { Name = name.ToString(), Type = type };

                if (i < info.Params.Count - 1 || info.IsVarArg)
                    w.Write(", ");
            }
            if (info.IsVarArg)
                w.Write("params Variant[] varargs");

            w.Write(')');

            w.Write('\n');
            WriteTab(w, tab++);
            w.Write("{\n");

            //w.Write("throw new System.NotImplementedException();\n");

            if (info.Index == -1)
            {
                if (info.Return != "void")
                {
                    WriteTab(w, tab);
                    w.Write("return default;\n");
                }
                WriteTab(w, --tab);
                w.Write("}\n");

                return true;
            }

            WriteTab(w, tab);

            for (int i = 0; i < info.Params.Count; i++)
            {
                if (info.Params[i].Type != "string")
                    continue;
                w.Write("byte* __");
                w.Write(info.Params[i].Name[0] == '@' ? info.Params[i].Name[1..] : info.Params[i].Name);
                w.Write(" = stackalloc byte[");
                w.Write(info.Params[i].Name);
                w.Write(".Length];\n");
                WriteTab(w, tab);
                w.Write(info.Params[i].Name);
                w.Write(".ToBytePtr(__");
                w.Write(info.Params[i].Name[0] == '@' ? info.Params[i].Name[1..] : info.Params[i].Name);
                w.Write(");\n");
                WriteTab(w, tab);
            }

            /*
            // OLD METHOD
            w.Write("((delegate* unmanaged[Cdecl]<");
            for (int i = 0; i < info.Params.Count; i++)
            {
                t = info.Params[i].Type;
                MethodTypeToNative(ref t);
                w.Write(t);
                w.Write(", ");
            }
            t = info.Return;
            MethodTypeToNative(ref t);
            w.Write(t);
            w.Write(">)__vtable[");
            w.Write(index);
            w.Write("])(");
            for (int i = 0; i < info.Params.Count; i++)
            {
                if (info.Params[i].Type == "string")
                {
                    w.Write("__");
                    w.Write(info.Params[i].Name[0] == '@' ? info.Params[i].Name[1..] : info.Params[i].Name);
                } else if (info.Params[i].Type.Contains("Span<"))
                {
                    w.Write(
                        "global::System.Runtime.CompilerServices.Unsafe.AsPointer(ref ");
                    w.Write(info.Params[i].Name);
                    w.Write(".GetRef())");
                } else if (info.Params[i].Type.StartsWith("ref "))
                {
                    w.Write("ref ");
                    w.Write(info.Params[i].Name);
                } else
                    w.Write(info.Params[i].Name);
                if (i < info.Params.Count - 1)
                    w.Write(", ");
            }
            w.Write(')');
            */

            string[] csBuiltins = ["byte", "sbyte", "short", "ushort", "uint", "long", "ulong", "double"];
            // NEW METHOD
            switch (info.Params.Count)
            {
                case 0:
                    break;

                default:
                    w.Write("nint* __args = stackalloc nint[");
                    w.Write(info.Params.Count);
                    w.Write("];\n");
                    for (int i = 0; i < info.Params.Count; i++)
                    {
                        WriteTab(w, tab);

                        if (info.Params[i].Type == "string")
                        {
                            w.Write("__args[");
                            w.Write(i);
                            w.Write("] = (nint)(&__");
                            w.Write(info.Params[i].Name[0] == '@' ? info.Params[i].Name[1..] : info.Params[i].Name);
                            w.Write(");\n");
                        } else if (info.Params[i].Type.Contains("Span<"))
                        {
                            w.Write("nint __");
                            w.Write(info.Params[i].Name[0] == '@' ? info.Params[i].Name[1..] : info.Params[i].Name);
                            w.Write(" = global::Godot.GdExtension.MemUtil.RefAsInt(ref ");
                            w.Write(info.Params[i].Name);
                            w.Write(".GetRef());\n");

                            w.Write("__args[");
                            w.Write(i);
                            w.Write("] = __");
                            w.Write(info.Params[i].Name[0] == '@' ? info.Params[i].Name[1..] : info.Params[i].Name);
                            w.Write(";\n");
                        } else if (info.Params[i].Type.StartsWith("ref "))
                        {
                            w.Write("nint __");
                            w.Write(info.Params[i].Name[0] == '@' ? info.Params[i].Name[1..] : info.Params[i].Name);
                            w.Write(" = global::Godot.GdExtension.MemUtil.RefAsInt(ref ");
                            w.Write(info.Params[i].Name);
                            w.Write(");\n");

                            w.Write("__args[");
                            w.Write(i);
                            w.Write("] = (nint)(&__");
                            w.Write(info.Params[i].Name[0] == '@' ? info.Params[i].Name[1..] : info.Params[i].Name);
                            w.Write(");\n");
                        } else
                        {
                            // Can't use 'ref' in lambda
                            List<ParameterInfo> p = info.Params;

                            if (builtins.Any((x) => p[i].Type.Equals(x["name"].ToString(), StringComparison.OrdinalIgnoreCase)) || 
                                enums.Any((x) => p[i].Type.Equals(x["name"].ToString(), StringComparison.OrdinalIgnoreCase)) ||
                                csBuiltins.Any((x) => p[i].Type.Equals(x, StringComparison.Ordinal)) ||
                                p[i].Type.Contains('.') || p[i].Type == "Variant")
                            {
                                // Is NOT a GodotObject, can safely get the address.

                                if ((p[i].Type.StartsWith("Packed") && p[i].Type.EndsWith("Array")) || p[i].Type == "Dictionary" || p[i].Type == "Array")
                                {
                                    // PackedVector3Array
                                    // Erm... nevermind

                                    break;

                                } else
                                {
                                    w.Write("__args[");
                                    w.Write(i);
                                    w.Write("] = (nint)(&");
                                    w.Write(info.Params[i].Name);
                                    w.Write(");\n");
                                }
                            } else
                            {
                                // Is a GodotObject, take the native handle instead.

                                if (p[i].Type.EndsWith("[]"))
                                    break;

                                w.Write("nint __");
                                w.Write(info.Params[i].Name[0] == '@' ? info.Params[i].Name[1..] : info.Params[i].Name);
                                w.Write(" = ");
                                w.Write(info.Params[i].Name);
                                w.Write(".Handle;\n");

                                WriteTab(w, tab);
                                w.Write("__args[");
                                w.Write(i);
                                w.Write("] = (nint)(&__");
                                w.Write(info.Params[i].Name[0] == '@' ? info.Params[i].Name[1..] : info.Params[i].Name);
                                w.Write(");\n");
                            }
                        }
                    }
                    WriteTab(w, tab);
                    break;
            }

            if (info.Return != "void")
            {
                if ((info.Return.StartsWith("Packed") && info.Return.EndsWith("Array")) || info.Return == "Dictionary" || info.Return == "Array")
                {
                    w.Write("throw new System.NotImplementedException();");
                    goto FinishMethod;
                } else
                {
                    w.Write("return ");
                    if (info.Return == "string")
                        w.Write("StringDB.SearchOrCreate(");
                    else if (info.Return.Contains('*'))
                    {
                        w.Write('(');
                        w.Write(info.Return);
                        w.Write(')');
                    }
                }
            }

            w.Write("MethodHelper.CallMethodBind");

            // Can't use 'ref' in lambda
            string retType = info.Return;
            if (info.Return != "void")
            {
                if (info.Return.Contains('*') || info.Return == "string")
                    w.Write("<nint>");
                else
                {
                    if (retType.EndsWith("[]"))
                        w.Write("Array");
                    else if (!retType.Contains('.') && !enums.Any((x) => retType.Equals(x["name"].ToString(), StringComparison.OrdinalIgnoreCase))
                        && !builtins.Any((x) => retType.Equals(x["name"].ToString(), StringComparison.OrdinalIgnoreCase))
                        && !csBuiltins.Any((x) => retType.Equals(x, StringComparison.Ordinal)) && retType != "Variant")
                        w.Write("Object");

                    w.Write('<');
                    if (retType.EndsWith("[]"))
                        w.Write(retType[..^2]);
                    else
                        w.Write(retType);
                    w.Write('>');
                }
            }

            if (info.IsStatic)
                w.Write('(');
            else
                w.Write("(this, ");

            w.Write("__vtable[");
            w.Write(info.Index);
            w.Write("]");

            if (info.Params.Count > 0)
            {
                w.Write(", __args");
            }

            w.Write(')');
            if (info.Return == "string")
            {
                w.Write(')');
            }

            FinishMethod:
            w.Write(";\n");

            WriteTab(w, --tab);
            w.Write("}\n");

            return true;
        }

        private static bool PopulateClass(JToken api, JToken[] singletons, ref ClassInfo c)
        {
            c.Enums.Clear();
            c.Signals.Clear();
            c.Methods.Clear();
            c.Properties.Clear();

            c.Name = (string)api["name"];
            if (c.Name.Equals("Object"))
                c.Name = "GodotObject";

            c.Inherits = (string)api["inherits"];
            if (c.Inherits?.Equals("Object") ?? false)
                c.Inherits = "GodotObject";

            c.Singleton = singletons.Any((a) => (string)a["type"] == (string)api["name"]);

            c.Instantiatable = (bool)api["is_instantiable"];

            c.Editor = (string)api["api_type"] == "editor";

            JToken[] arr;

            arr = api["enums"]?.ToArray();
            if (arr != null)
            {
                EnumInfo ienum = new EnumInfo();
                for (int i = 0; i < arr.Length; i++)
                {
                    ienum.Values = new List<ParameterInfo>();

                    ienum.Name = (string)arr[i]["name"];
                    ienum.Flags = (bool)arr[i]["is_bitfield"];

                    JToken[] p = arr[i]["values"]?.ToArray();
                    if (p != null)
                    {
                        for (int j = 0; j < p.Length; j++)
                            ienum.Values.Add(new ParameterInfo() { Name = (string)p[j]["name"], Type = (string)p[j]["value"] });
                    }

                    c.Enums.Add(ienum);
                }
            }

            arr = api["methods"]?.ToArray();
            if (arr != null)
            {
                MethodInfo method = new MethodInfo();
                for (int i = 0; i < arr.Length; i++)
                {
                    method.Params = new List<ParameterInfo>();

                    method.Name = (string)arr[i]["name"];
                    method.IsConst = (bool)arr[i]["is_const"];
                    method.IsVarArg = (bool)arr[i]["is_vararg"];
                    method.IsStatic = (bool)arr[i]["is_static"];
                    method.IsVirtual = (bool)arr[i]["is_virtual"];
                    method.Hash = (ulong?)arr[i]["hash"] ?? 0;
                    method.Return = (string)arr[i]["return_value"]?["type"];
                    method.ReturnMeta = (string)arr[i]["return_value"]?["meta"];

                    JToken[] p = arr[i]["arguments"]?.ToArray();
                    if (p != null)
                    {
                        for (int j = 0; j < p.Length; j++)
                            method.Params.Add(new ParameterInfo() { Name = (string)p[j]["name"], Type = (string)p[j]["type"], Meta = (string)p[j]["meta"] });
                    }

                    c.Methods.Add(method);
                }
            }

            return true;
        }

        private static void WriteVTable(StreamWriter w, ClassInfo info)
        {
            if (info.Methods.Count <= 0)
                return;

            int vcount = 0;
            for (int i = 0; i < info.Methods.Count; i++)
            {
                MethodInfo mi = info.Methods[i];
                if (info.Methods[i].Hash != 0)
                {
                    mi.Index = vcount++;
                } else
                    mi.Index = -1;
                info.Methods[i] = mi;
            }

            if (vcount > 0)
            {
                w.Write("        private static readonly nint[] __vtable = new nint[");
                w.Write(vcount);
                w.Write("];\n\n        static ");
                w.Write(info.Name);
                w.Write("()\n        {\n            StringName __className = \"");
                w.Write(info.Name == "GodotObject" ? "Object" : info.Name);
                w.Write("\";\n");

                int vi = 0;
                for (int i = 0; i < info.Methods.Count; i++)
                {
                    if (info.Methods[i].Index == -1)
                        continue;

                    w.Write("            __vtable[");
                    w.Write(vi++);
                    w.Write("] = global::Godot.GdExtension.MethodHelper.GetMethodBind(__className, \"");
                    w.Write(info.Methods[i].Name);
                    w.Write("\", ");
                    w.Write(info.Methods[i].Hash);
                    w.Write(");\n");
                }
                w.Write("            global::Godot.TypeDB.Register(typeof(");
                w.Write(info.Name);
                w.Write("), __className);\n        }\n\n");
            }
        }

        private static void WriteInstanceBindings(StreamWriter w, string type, bool refcounted, bool construct)
        {
            if (type == "GodotObject")
                return;
            w.Write("        internal ");
            w.Write(type);
            w.Write("(bool refcounted) : base(refcounted) { }\n\n");
            if (!construct)
                return;
            w.Write("        public ");
            w.Write(type);
            w.Write("() : this(");
            w.Write(refcounted ? "true" : "false");
            w.Write(") { }\n\n");
        }

        private static bool GenerateClasses(JObject api, string directory)
        {
            Span<char> cname = stackalloc char[256];
            JToken[] builtins = api["builtin_classes"].ToArray();
            JToken[] enums = api["global_enums"].ToArray();
            JToken[] classes = api["classes"].ToArray();
            JToken[] singletons = api["singletons"].ToArray();
            
            ClassInfo info = new ClassInfo();
            info.Enums = new List<EnumInfo>();
            info.Signals = new List<SignalInfo>();
            info.Methods = new List<MethodInfo>();
            info.Properties = new List<PropertyInfo>();

            foreach (JToken c in classes)
            {
                if (!PopulateClass(c, singletons, ref info))
                    return false;

                string path = Path.Join(directory.AsSpan(), $"{info.Name}.cs");
                using FileStream fs = File.OpenWrite(path);
                using StreamWriter w = new StreamWriter(fs);

                w.Write(GenPrelude);

                if (info.Editor)
                    w.Write("#if TOOLS\n");
                w.Write("#pragma warning disable CS1591\n#pragma warning disable CA1822\n#pragma warning disable CA1720\n    public ");
                w.Write("unsafe ");
                if (!info.Instantiatable && !info.Singleton)
                    w.Write("abstract ");
                w.Write("partial class ");
                w.Write(info.Name);
                if (info.Inherits != null)
                {
                    w.Write(" : ");
                    w.Write(info.Inherits);
                }
                w.Write("\n    {\n");
                
                WriteVTable(w, info);
                WriteInstanceBindings(w, info.Name, info.RefCounted, info.Instantiatable);

                for (int i = 0; i < info.Enums.Count; i++)
                    WriteEnum(w, info.Enums[i], cname, 2);
                for (int i = 0; i < info.Methods.Count; i++)
                {
                    MethodInfo m = info.Methods[i];
                    WriteMethod(w, builtins, enums, ref m, info.Singleton, cname, 2);
                    info.Methods[i] = m;
                }
                if (info.Singleton)
                {
                    w.Write("        private static ");
                    w.Write(info.Name);
                    w.Write(" __self_Singleton;\n        public static ");
                    w.Write(info.Name);
                    w.Write(" Singleton { get {\n            if (__self_Singleton != null)\n                return __self_Singleton;\n");
                    w.Write("            return __self_Singleton = ClassDB.GetSingletonInstance<");
                    w.Write(info.Name);
                    w.Write(">(\"");
                    w.Write(info.Name);
                    w.Write("\"); } }\n\n");
                    for (int i = 0; i < info.Methods.Count; i++)
                        WriteSingletonMethod(w, info.Methods[i], 2);
                }

                w.Write("    }\n");
                if (info.Editor)
                    w.Write("#endif // TOOLS\n");

                w.Write(GenPostlude);
            }

            return true;
        }

        private static void WriteBuiltinClass(JToken api, string directory, string name)
        {
            string path = Path.Join(directory.AsSpan(), $"{name}.cs");
            using FileStream fs = File.OpenWrite(path);
            using StreamWriter w = new StreamWriter(fs);

            JToken bc = api[name];
        }

        private static bool GenerateBuiltinClasses(JObject api, string directory)
        {
            directory = Path.Join(directory, "../Variant/Generated/");
            JToken classes = api["builtin_classes"];

            return true;
        }

        public static int GenerateApi(StreamReader reader, string directory)
        {
            using JsonTextReader jr = new JsonTextReader(reader);
            JObject api = JObject.Load(jr);

            try
            {
                if (!GenerateEnums(api, directory))
                    return Error("Error generating global enums.");
                if (!GenerateClasses(api, directory))
                    return Error("Error generating Godot classes.");

            } catch (Exception e)
            {
                return Error($"`extension_api.json` is not formatted correctly:\n\n{e}");
            }

            return 0;
        }
    }
}
