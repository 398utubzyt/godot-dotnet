using System;
using System.Runtime.InteropServices;

namespace Godot
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Note: the <see cref="Dispose"/> method should be used on any <see cref="Callable"/> created
    /// by the user once it is no longer needed -- <see cref="Callable"/> lifetimes are NOT tracked
    /// and cleaned up automatically.
    /// <para/>
    /// Consider using a <see langword="using"/> statement to prevent memory leaks.
    /// </remarks>
    [SLayout(SLayoutOpt.Explicit)]
    public readonly struct Callable : IDisposable
    {
        private unsafe static GDExtensionPtrDestructor _dtor;
        unsafe static Callable()
        {
            _dtor = (GDExtensionPtrDestructor)Main.i.VariantGetPtrDestructor(VariantType.Callable);
        }

        [FieldOffset(0)]
        private readonly StringName _name;
        [FieldOffset(8)]
        private readonly ulong _objId;
        [FieldOffset(8)]
        private readonly nint _custom;

        private static unsafe class ManagedHelper
        {
            public readonly struct CallableData
            {
                public readonly nint Method;
                public readonly nint Self;

                public CallableData(nint method, nint self)
                {
                    Method = method;
                    Self = self;
                }
            }

            [UnmanagedCallersOnly]
            public static void Call(void* userdata, nint* args, long argc, nint result, CallError* error)
            {
                CallableData data = *(CallableData*)userdata;
                GodotObject self = data.Self != 0 ? GCHandle.FromIntPtr(data.Self).Target as GodotObject : null;
                System.Reflection.MethodBase method;
                try
                {
                    method = data.Method.AsManagedMethodBase();
                } catch (ArgumentException)
                {
                    if (error != null)
                        *error = new CallError() { Error = CallErrorType.InvalidMethod };
                    return;
                }

                if (self != null)
                {
                    
                }
            }
            [UnmanagedCallersOnly]
            public static void Free(void* userdata)
            {
                MemUtil.Free(userdata);
            }
            [UnmanagedCallersOnly]
            public static void ToString(void* userdata, byte* valid, nint* value)
            {
                CallableData data = *(CallableData*)userdata;
                System.Reflection.MethodBase method;
                try
                {
                    method = data.Method.AsManagedMethodBase();
                } catch (ArgumentException)
                {
                    *valid = 0;
                    return;
                }

                StringDB.Placement(method.Name, ref MemUtil.PointerAsRef(value));
                *valid = (*value != 0).ToExtBool();
            }
        }

        private static unsafe Callable _Create(nint func, nint binding, ulong id)
        {
            Callable result;
            CallableCustomInfo info;
            info.Token = (void*)Main.lib;
            info.CallFunc = &ManagedHelper.Call;
            info.FreeFunc = &ManagedHelper.Free;
            info.ObjectID = id;
            info.CallableUserData = MemUtil.Alloc<ManagedHelper.CallableData>(1);
            *(ManagedHelper.CallableData*)info.CallableUserData = new ManagedHelper.CallableData(func, binding);
            Main.i.CallableCustomCreate((nint)(&result), &info);
            return result;
        }

        // Seems simple enough...
        // Probably don't need this generic argument?
        /// <summary>
        /// Creates a new <see cref="Callable"/> from the provided function delegate.
        /// </summary>
        /// <typeparam name="T">The type of the delegate.</typeparam>
        /// <param name="func">The function to be referenced.</param>
        /// <returns>The <see cref="Callable"/> of the function.</returns>
        public static unsafe Callable From<T>(T func) where T : Delegate
        {
            GodotObject obj = func.Target as GodotObject;
            return obj == null ? 
                _Create(func.Method.MethodHandle.Value, obj.Binding, obj.GetInstanceId()) : 
                _Create(func.Method.MethodHandle.Value, 0, 0);
        }

        public unsafe void Dispose()
        {
            Callable self = this;
            _dtor((nint)(&self));
        }

        public static unsafe explicit operator Callable(Delegate func)
            => From(func);

        public Callable(StringName name)
        {
            _name = name;
            _objId = 0;
        }
    }
}
