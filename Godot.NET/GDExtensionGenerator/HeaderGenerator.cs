using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GDExtensionGenerator
{
    public static class HeaderGenerator
    {
        private struct NamedParam
        {
            public string Left;
            public string Right;

            public NamedParam(string name, string description)
            {
                Left = name;
                Right = description;
            }
        }

        private struct DocumentationBlock
        {
            public string Name;
            public string Since;
            public string Deprecated;
            public string Description;
            public List<NamedParam> Params;
            public string Returns;
        }

        private struct MethodDesc
        {
            public string Name;
            public string Return;
            public List<NamedParam> Params;
            public bool Unsafe;
        }

        private struct MethodCache
        {
            public string Native;
            public MethodDesc Info;

            public MethodCache(string native, MethodDesc desc)
            {
                Native = native;
                Info = desc;
                Info.Params = new List<NamedParam>(desc.Params);
            }
        }

        public const int InterfaceBegin = 612;

        private static bool ReadToDoc(StreamReader reader)
        {
            while (!reader.EndOfStream && reader.ReadLine() != "/**") ;
            return !reader.EndOfStream;
        }

        private static bool PopulateDocs(StreamReader reader, ref DocumentationBlock doc)
        {
            StringBuilder desc = null;
            string line;
            bool nl = false;

            doc.Params.Clear();
            doc.Returns = null;
            doc.Deprecated = null;

            while (!reader.EndOfStream && (line = reader.ReadLine()) != " */")
            {
                if (line.Length < 3)
                {
                    if (nl)
                        desc?.Append("\n\n");
                    else
                        nl = true;
                    continue;
                }

                if (nl)
                    nl = false;

                string[] args = (line = line[3..]).Split(' ');
                switch (args[0])
                {
                    case "@name":
                        doc.Name = args[1];
                        break;

                    case "@since":
                        doc.Since = args[1];
                        break;

                    case "@deprecated":
                        doc.Deprecated = $"D{line[2..]}";
                        break;

                    case "@param":
                        doc.Params.Add(new NamedParam(args[1], line[(8 + args[1].Length)..]));
                        break;

                    case "@return":
                        if (args.Length > 1)
                            doc.Returns = line[(args[0].Length + 1)..];
                        break;

                    default:
                        if (line.Length > 0)
                        {
                            desc ??= new StringBuilder();
                            desc.Append(line);
                        }
                        break;
                }
            }

            doc.Description = desc?.ToString();
            return true;
        }

        private static string GdSpanToString(ReadOnlySpan<char> reference, ReadOnlySpan<char> fallback)
            => reference switch
            {
                "int8_t" => "sbyte",
                "uint8_t" => "byte",
                "int16_t" => "short",
                "uint16_t" => "ushort",
                "int32_t" => "int",
                "uint32_t" => "uint",
                "int64_t" => "long",
                "uint64_t" => "ulong",
                "size_t" => "ulong",

                "char" => "byte",
                "char16_t" => "ushort",
                "char32_t" => "uint",
                "wchar_t" => "ushort",

                "int8_t *" => "sbyte *",
                "uint8_t *" => "byte *",
                "int16_t *" => "short *",
                "uint16_t *" => "ushort *",
                "int32_t *" => "int *",
                "uint32_t *" => "uint *",
                "int64_t *" => "long *",
                "uint64_t *" => "ulong *",
                "size_t *" => "ulong *",

                "char *" => "byte *",
                "char16_t *" => "ushort *",
                "char32_t *" => "uint *",
                "wchar_t *" => "ushort *",

                "GDExtensionGodotVersion" => "GodotVersion",
                "GDExtensionVariantType" => "VariantType",
                "GDExtensionVariantOperator" => "VariantOp",
                "GDExtensionCallError" => "CallError",
                "GDExtensionCallableCustomInfo" => "CallableCustomInfo",
                "GDExtensionClassMethodInfo" => "ClassMethodInfo",
                "GDExtensionClassCreationInfo" => "ClassCreationInfo",
                "GDExtensionClassCreationInfo2" => "ClassCreationInfo2",
                "GDExtensionInstanceBindingCallbacks" => "InstanceBindingCallbacks",
                "GDExtensionScriptInstanceInfo" => "ScriptInstanceInfo",
                "GDExtensionScriptInstanceInfo2" => "ScriptInstanceInfo2",
                "GDExtensionMethodInfo" => "MethodInfo",
                "GDExtensionPropertyInfo" => "PropertyInfo",

                "GDExtensionGodotVersion *" => "GodotVersion *",
                "GDExtensionVariantType *" => "VariantType *",
                "GDExtensionVariantOperator *" => "VariantOp *",
                "GDExtensionCallError *" => "CallError *",
                "GDExtensionCallableCustomInfo *" => "CallableCustomInfo *",
                "GDExtensionClassMethodInfo *" => "ClassMethodInfo *",
                "GDExtensionClassCreationInfo *" => "ClassCreationInfo *",
                "GDExtensionClassCreationInfo2 *" => "ClassCreationInfo2 *",
                "GDExtensionInstanceBindingCallbacks *" => "InstanceBindingCallbacks *",
                "GDExtensionScriptInstanceInfo *" => "ScriptInstanceInfo *",
                "GDExtensionScriptInstanceInfo2 *" => "ScriptInstanceInfo2 *",
                "GDExtensionMethodInfo *" => "MethodInfo *",
                "GDExtensionPropertyInfo *" => "PropertyInfo *",

                _ => fallback.Trim().ToString()
            };

        private static bool PopulateMethod(StreamReader reader, ref MethodDesc desc)
        {
            desc.Params.Clear();
            ReadOnlySpan<char> line = reader.ReadLine().AsSpan()[8..^1];

            desc.Unsafe = line.IndexOf('*') != line.LastIndexOf('*');

            ReadOnlySpan<char> ret = line[..line.IndexOf('(')];
            if (ret.StartsWith("const "))
                ret = ret[6..];
            desc.Return = GdSpanToString(ret.Trim(), ret);

            line = line[(line.IndexOf('(') + 22)..];
            desc.Name = line[..line.IndexOf(')')].ToString();
            
            line = line[(line.IndexOf(')') + 2)..];

            while (line.Length > 0)
            {
                ReadOnlySpan<char> param = (line.Contains(',') ? line[..line.IndexOf(',')] : line[..line.IndexOf(')')]).TrimStart();

                if (param.StartsWith("const "))
                    param = param[6..];

                int namePos = param.LastIndexOf(' ');
                if (namePos >= 0 && param[namePos + 1] == '*')
                    namePos += 2;
                string type = GdSpanToString(param[..namePos], param[..namePos]);

                desc.Params.Add(new NamedParam(type.Trim(), param[namePos..].Trim().ToString()));
                if (line.Contains(','))
                    line = line[(line.IndexOf(',') + 1)..];
                else
                    break;
            }

            return true;
        }

#if !USE_MANAGED_FUNCTION_POINTERS
        private static void WriteDelegate(StreamWriter w, MethodDesc method)
        {
            w.Write("delegate* unmanaged[Cdecl]<");
            for (int i = 0; i < method.Params.Count; i++)
            {
                w.Write(method.Params[i].Left);
                w.Write(", ");
            }
            method.Return ??= "void";
            w.Write(method.Return.EndsWith("Constructor") || method.Return.EndsWith("Destructor") ? "nint" : method.Return);
            w.Write(">");
        }
#endif

        private static bool WriteMethod(StreamWriter w, DocumentationBlock doc, MethodDesc method)
        {
            w.Write("        /// <summary>");
            w.Write(doc.Description);
            w.Write("</summary>\n");
            for (int i = 0; i < doc.Params.Count; i++)
            {
                w.Write("        /// <param name=\"");
                w.Write(doc.Params[i].Left);
                w.Write("\">");
                w.Write(doc.Params[i].Right);
                w.Write("</param>\n");
            }

            if (doc.Returns != null)
            {
                w.Write("        /// <returns>");
                w.Write(doc.Returns);
                w.Write("\n        /// Type of <see cref=\"");
                w.Write(method.Return);
                w.Write("\"/>.");
                w.Write("</returns>\n");
            }

            if (doc.Deprecated != null)
            {
                w.Write("        [Deprecated(\"");
                w.Write(doc.Deprecated);
                w.Write("\")]\n");
            }

#if USE_MANAGED_FUNCTION_POINTERS
            w.Write("        [global::System.Runtime.InteropServices.UnmanagedFunctionPointer(CallingConvention.Cdecl)]\n        public ");
            if (method.Unsafe)
                w.Write("unsafe ");
            w.Write("delegate ");
            w.Write(method.Return.EndsWith("Constructor") || method.Return.EndsWith("Destructor") ? "nint" : method.Return);
            w.Write(" __");
            w.Write(method.Name);
            w.Write('(');
            for (int i = 0; i < method.Params.Count; i++)
            {
                w.Write(method.Params[i].Left);
                w.Write(' ');
                w.Write(method.Params[i].Right);
                if (i < method.Params.Count - 1)
                    w.Write(", ");
            }
            w.Write(");\n        /// <inheritdoc cref=\"global::Godot.GdExtension.Interface.__");
            w.Write(method.Name);
            w.Write("\"/>\n");
            if (doc.Deprecated != null)
            {
                w.Write("        [Deprecated(\"");
                w.Write(doc.Deprecated);
                w.Write("\")]\n");
            }
            w.Write("        public __");
            w.Write(method.Name);
#else
            w.Write("        public unsafe ");
            WriteDelegate(w, method);
#endif
            w.Write(' ');
            w.Write(method.Name);
            w.Write(";\n\n");
            return true;
        }

        public static int GenerateApi(StreamReader reader, string directory)
        {
            for (int i = 0; i < InterfaceBegin; i++)
                reader.ReadLine();

            using FileStream fs = File.OpenWrite(Path.Join(directory, "Interface.cs"));
            using StreamWriter w = new StreamWriter(fs);

            List<MethodCache> methods = new List<MethodCache>();

            w.Write(GenPrelude2);

            w.Write("#pragma warning disable CS1572\n");
            w.Write("#pragma warning disable CS1573\n");
            w.Write("#pragma warning disable CS1574\n");
            w.Write("#pragma warning disable CS0618\n");
            w.Write("    [SLayout(SLayoutOpt.Sequential)]\n");
            w.Write("    internal struct Interface\n    {\n");

            DocumentationBlock block = new DocumentationBlock();
            MethodDesc method = new MethodDesc();
            block.Params = new List<NamedParam>();
            method.Params = new List<NamedParam>();
            while (ReadToDoc(reader))
            {
                if (!PopulateDocs(reader, ref block))
                    return Error($"Error reading documentation of method after: {method.Name}");
                if (block.Name == "worker_thread_pool_add_native_group_task" ||
                    block.Name == "worker_thread_pool_add_native_task")
                    continue;
                if (!PopulateMethod(reader, ref method))
                    return Error($"Error reading method: {block.Name}");

                if (!WriteMethod(w, block, method))
                    return Error($"Error generating method: {method.Name}");

                methods.Add(new MethodCache(block.Name, method));
            }

            w.Write("\n        public unsafe bool _Init(delegate* unmanaged[Cdecl]<byte*, nint> getProc)\n        {" +
                "\n            // Should be a reasonably sized buffer for function names...\n            byte* _u8str = stackalloc byte[128];\n"

#if USE_MANAGED_FUNCTION_POINTERS
              + "            nint _proc;");
            foreach (NamedParam m in methods)
            {
                for (int i = 0; i < m.Left.Length; i++)
                {
                    w.Write("\n            _u8str[");
                    w.Write(i);
                    w.Write("] = ");
                    w.Write((int)m.Left[i]);
                    w.Write(";");
                }

                w.Write("\n            _u8str[");
                w.Write(m.Left.Length);
                w.Write("] = 0;\n            _proc = getProc(_u8str);\n            if (_proc != nint.Zero)\n                ");
                w.Write(m.Right);
                w.Write(" = Marshal.GetDelegateForFunctionPointer<__");
                w.Write(m.Right);
                w.Write(">(getProc(_u8str));\n");
            }
#else
              );
            foreach (MethodCache m in methods)
            {
                for (int i = 0; i < m.Native.Length; i++)
                {
                    w.Write("\n            _u8str[");
                    w.Write(i);
                    w.Write("] = ");
                    w.Write((int)m.Native[i]);
                    w.Write(";");
                }

                w.Write("\n            _u8str[");
                w.Write(m.Native.Length);
                w.Write("] = 0;\n            ");
                w.Write(m.Info.Name);
                w.Write(" = (");
                WriteDelegate(w, m.Info);
                w.Write(")getProc(_u8str);\n");
            }
#endif
            w.Write("\n            return true;\n        }\n    }\n");
            w.Write(GenPostlude);

            return 0;
        }
    }
}
