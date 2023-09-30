using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Godot
{
    internal static class ScriptDB
    {
        private static readonly BiHashMap<CSharpScript, Type> _map = new BiHashMap<CSharpScript, Type>();

        [RequiresUnreferencedCode("Calls System.Reflection.Assembly.GetType(String)")]
        public static Type SearchOrCreate(CSharpScript scr)
        {
            if (scr?.IsNull ?? true)
                throw new ArgumentNullException(nameof(scr));

            if (!_map.TryGet(scr.Handle, out Type type))
            {
                type = typeof(ScriptDB).Assembly.GetType(scr.GetMeta("__dotnet.type", string.Empty).String);
                _map[scr.Handle, scr] = type ?? throw new InvalidOperationException($"Cannot register bridge for script '{scr.GetPath()}'");
            }
            
            return type;
        }

        public static Script Register(Type type)
        {
            if (_map.FindLeft(type, (a, b) => a == b, out CSharpScript scr))
                return scr;

            return scr;
        }

        public static void Unregister(Type scr)
        {
            if (_map.FindLeft(scr, (a, b) => a == b, out CSharpScript script))
                _map.Remove(script.Handle);
        }
    }
}
