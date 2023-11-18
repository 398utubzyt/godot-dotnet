using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Godot
{
    public readonly struct Callable
    {
        private readonly nint _handle;

        private static unsafe class ManagedHelper
        {
            private readonly struct CallableData
            {
                public readonly void* Method;
                public readonly void* Self;
            }

            [UnmanagedCallersOnly]
            public static void Call(void* userdata, nint* args, long argc, nint result, CallError* error)
            {

            }
            [UnmanagedCallersOnly]
            public static void Free(void* userdata)
            {
                MemUtil.Free(userdata);
            }
        }

        public static unsafe implicit operator Callable(Delegate d)
        {
            Callable result;
            CallableCustomInfo info;
            info.Token = (void*)Main.lib;
            info.CallFunc = &ManagedHelper.Call;
            info.FreeFunc = &ManagedHelper.Free;
            info.ObjectID = (d.Target as GodotObject)?.GetInstanceId() ?? 0;
            unsafe { Main.i.CallableCustomCreate((nint)(&result), &info); }
            return result;
        }

        internal Callable(nint handle)
            => _handle = handle;
    }
}
