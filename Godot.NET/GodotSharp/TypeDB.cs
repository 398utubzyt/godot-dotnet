using System;


namespace Godot
{
    internal static class TypeDB
    {
        private static readonly BiHashMap<StringName, Type> _map = new BiHashMap<StringName, Type>();

        public static void Clear()
        {
            _map.Clear();
        }

        public static bool Search(StringName name, out Type type)
            => _map.TryGet((nint)name, out type);

        public static bool Search(Type type, out StringName name)
            => _map.FindLeft(type, (a, b) => a == b, out name);

        public static StringName GetName(nint native)
            => GodotObject.GetClass(native);

        public static unsafe void Register(Type type, StringName name)
        {
            _map[(nint)name, name] = type;
        }

        public static unsafe void Unregister(Type type)
        {
            if (_map.FindLeft(type, (a, b) => a == b, out StringName native))
            {
                _map.Remove((nint)native);
            }
        }
    }
}
