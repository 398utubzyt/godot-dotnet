using System;
using System.Reflection;

namespace Godot
{
    internal static class ScriptDB
    {
        private static readonly BiHashMap<Type, string> _map = new BiHashMap<Type, string>();

        public static bool Search(Type type, out string path)
            => _map.TryGet(type.TypeHandle.GetHashCode(), out path);
        public static bool Search(string path, out Type type)
            => _map.FindLeft(path, (a, b) => a == b, out type);

        public static void Register(Type type, string path)
        {
            if (_map.FindLeft(path, (a, b) => a == b, out Type result))
                _map.Remove(type.TypeHandle.GetHashCode());

            _map[type.TypeHandle.GetHashCode(), type] = path;
        }

        public static void Register(Assembly assembly)
        {
            AssemblyHasClassesAttribute attr = assembly.GetCustomAttribute<AssemblyHasClassesAttribute>();
            if (attr == null)
                return;
            if (attr.Types.Length != attr.Paths.Length)
                return;

            for (int i = 0; i < attr.Types.Length; ++i)
                Register(attr.Types[i], attr.Paths[i]);
        }

        public static void Unregister(Type type)
        {
            if (_map.Contains(type.TypeHandle.GetHashCode()))
                _map.Remove(type.TypeHandle.GetHashCode());
        }

        public static void Unregister(Assembly assembly)
        {
            AssemblyHasClassesAttribute attr = assembly.GetCustomAttribute<AssemblyHasClassesAttribute>();
            if (attr == null)
                return;

            for (int i = 0; i < attr.Types.Length; ++i)
                Unregister(attr.Types[i]);
        }

        public static void Clear()
        {
            _map.Clear();
        }
    }
}
