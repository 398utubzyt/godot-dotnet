using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Godot
{
    public static partial class GD
    {
        internal static unsafe void PrintWarning(string description, string function, string file, int line, bool editorNotify)
        {
            byte* b_description = stackalloc byte[(int)description.BufferSize()];
            description.ToBytePtr(b_description);
            byte* b_function = stackalloc byte[(int)function.BufferSize()];
            function.ToBytePtr(b_function);
            byte* b_file = stackalloc byte[(int)file.BufferSize()];
            file.ToBytePtr(b_file);

            Main.i.PrintWarning(b_description, b_function, b_file, line, editorNotify.ToExtBool());
        }

        internal static unsafe void PrintError(string description, string function, string file, int line, bool editorNotify)
        {
            byte* b_description = stackalloc byte[(int)description.BufferSize()];
            description.ToBytePtr(b_description);
            byte* b_function = stackalloc byte[(int)function.BufferSize()];
            function.ToBytePtr(b_function);
            byte* b_file = stackalloc byte[(int)file.BufferSize()];
            file.ToBytePtr(b_file);

            Main.i.PrintError(b_description, b_function, b_file, line, editorNotify.ToExtBool());
        }

        public static unsafe void PrintException(Exception exception)
        {
            StackFrame f = new StackFrame(1, true);
            if (f.HasMethod())
#pragma warning disable IL2026
                PrintError(exception.Message, f.GetMethod()?.Name ?? "Unknown Method", f.GetFileName(), f.GetFileLineNumber(), true);
#pragma warning restore IL2026
            else
                PrintError(exception.Message, "<Unknown method>", "<Unknown file>", 0, true);
        }
    }
}
