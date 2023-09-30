using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDExtensionGenerator
{
    public static class Util
    {
        public const uint VersionMajor = 4;
        public const uint VersionMinor = 2;

        public const string GenPrelude = "// This code was automatically generated!! Changes will be overriden.\n" +
            "using System;\nusing System.Runtime.InteropServices;\nusing Array = Godot.Collections.VariantArray;\n" +
            "using Dictionary = Godot.Collections.VariantDictionary;\n\nnamespace Godot\n{\n";
        public const string GenPrelude2 = "// This code was automatically generated!! Changes will be overriden.\n" +
            "using System;\nusing System.Runtime.InteropServices;\nusing Array = Godot.Collections.VariantArray;\n" +
            "using Dictionary = Godot.Collections.VariantDictionary;\n\nnamespace Godot.GdExtension\n{\n";
        public const string GenPostlude = "}\n";

        public static int CSharpify(string s, ref Span<char> chars)
        {
            bool caps = true;
            int offset = 0;
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] == '_')
                {
                    if (i == 0)
                        chars[i] = '_';
                    else
                        offset++;
                    caps = true;
                    continue;
                }

                if (caps)
                {
                    chars[i - offset] = char.ToUpper(s[i]);
                    caps = false;
                    continue;
                }

                chars[i - offset] = char.ToLower(s[i]);
            }
            chars[s.Length - offset] = '\0';

            return s.Length - offset;
        }

        public static int EnumCSharpify(string s, ref Span<char> chars, string prefix)
        {
            if (prefix.EndsWith("Flags"))
                prefix = prefix[..^5];

            prefix = prefix switch
            {
                "KeyModifierMask" => "KeyMask",
                "Error" => "Err",
                _ => prefix,
            };

            bool caps = true, intro = true;
            int underscores = 0, nameOffset = 0;
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] == '_')
                {
                    underscores++;
                    caps = true;
                    continue;
                }

                if (intro)
                {
                    if (i - underscores < prefix.Length && s[i] == char.ToUpper(prefix[i - underscores]))
                    {
                        nameOffset++;
                        continue;
                    } else
                        intro = false;
                }

                if (caps)
                {
                    chars[i - underscores - nameOffset] = char.ToUpper(s[i]);
                    caps = false;
                    continue;
                }

                chars[i - underscores - nameOffset] = char.ToLower(s[i]);
            }

            chars[s.Length - underscores - nameOffset] = '\0';
            if (char.IsNumber(chars[0]) || chars[0] == '\0')
            {
                return EnumCSharpify(s, ref chars, string.Empty);
            }

            return s.Length - underscores - nameOffset;
        }

        public static void FixCasing(Span<char> chars, string query)
        {
            int index = chars.LastIndexOf(query);
            if (index >= chars.Length - index - 1)
                chars[index] = char.ToUpper(chars[index]);
        }

        public static int Error(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(msg);
            Console.ResetColor();
            return 1;
        }
    }
}
