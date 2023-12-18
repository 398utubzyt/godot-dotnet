using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Godot.GdExtension
{
    internal unsafe static class MethodHelper
    {
        public static nint GetMethodBind(StringName className, StringName methodName, long hash)
        {
            if ((nint)className == 0 || (nint)methodName == 0)
            {
                Console.WriteLine($"{className}::{methodName}() ({hash})");
                Console.WriteLine(new System.Diagnostics.StackTrace());
            }
            return Main.i.ClassdbGetMethodBind((nint)(&className), (nint)(&methodName), hash);
        }
        /*
        public static CallError CallMethodBind(nint bind, nint instance, out Variant ret, params Variant[] args)
        {
            CallError err;
            Variant** v = stackalloc Variant*[args.Length];
            ret = new Variant();
            fixed (Variant* vargs = args)
            {
                for (nint i = 0; i < args.LongLength; i++)
                    v[i] = &vargs[i];
                Main.i.ObjectMethodBindCall(bind, instance, (nint*)v, args.LongLength, MemUtil.RefAsInt(ref ret), &err);
            }
            return err;
        }*/
        public static T CallMethodBind<T>(nint handle, nint bind) where T : unmanaged
        {
            T ret;
            Main.i.ObjectMethodBindPtrcall(bind, handle, (nint*)0, (nint)(&ret));
            return ret;
        }

        public static void CallMethodBind(GodotObject self, nint bind)
        {
            Main.i.ObjectMethodBindPtrcall(bind, self.Handle, (nint*)0, 0);
        }
        public static T CallMethodBind<T>(GodotObject self, nint bind) where T : unmanaged
        {
            T ret;
            Main.i.ObjectMethodBindPtrcall(bind, self.Handle, (nint*)0, (nint)(&ret));
            return ret;
        }
        public static T CallMethodBindObject<T>(GodotObject self, nint bind) where T : GodotObject
        {
            nint ptr;
            Main.i.ObjectMethodBindPtrcall(bind, self.Handle, (nint*)0, (nint)(&ptr));
            GCHandle handle = ClassDB.GetOrMakeHandleFromNative(ptr);
            return handle.Target as T;
        }
        public static T[] CallMethodBindArray<T>(GodotObject self, nint bind)
        {
            // TODO: Make this work :3
            throw new NotImplementedException();
        }
        public static void CallMethodBind(GodotObject self, nint bind, nint* args)
        {
            Main.i.ObjectMethodBindPtrcall(bind, self.Handle, args, 0);
        }
        public static T CallMethodBind<T>(GodotObject self, nint bind, nint* args) where T : unmanaged
        {
            T ret;
            Main.i.ObjectMethodBindPtrcall(bind, self.Handle, args, (nint)(&ret));
            return ret;
        }
        public static T CallMethodBindObject<T>(GodotObject self, nint bind, nint* args) where T : GodotObject
        {
            nint ptr;
            Main.i.ObjectMethodBindPtrcall(bind, self.Handle, args, (nint)(&ptr));
            GCHandle handle = ClassDB.GetOrMakeHandleFromNative(ptr);
            return handle.Target as T;
        }
        public static T[] CallMethodBindArray<T>(GodotObject self, nint bind, nint* args)
        {
            // TODO: Make this work :3
            throw new NotImplementedException();
        }

        public static void CallMethodBind(nint bind)
        {
            Main.i.ObjectMethodBindPtrcall(bind, 0, (nint*)0, 0);
        }
        public static T CallMethodBind<T>(nint bind) where T : unmanaged
        {
            T ret;
            Main.i.ObjectMethodBindPtrcall(bind, 0, (nint*)0, (nint)(&ret));
            return ret;
        }
        public static T CallMethodBindObject<T>(nint bind) where T : GodotObject
        {
            nint ptr;
            Main.i.ObjectMethodBindPtrcall(bind, 0, (nint*)0, (nint)(&ptr));
            GCHandle handle = ClassDB.GetOrMakeHandleFromNative(ptr);
            return handle.Target as T;
        }
        public static T[] CallMethodBindArray<T>(nint bind)
        {
            // TODO: Make this work :3
            throw new NotImplementedException();
        }
        public static void CallMethodBind(nint bind, nint* args)
        {
            Main.i.ObjectMethodBindPtrcall(bind, 0, args, 0);
        }
        public static T CallMethodBind<T>(nint bind, nint* args) where T : unmanaged
        {
            T ret;
            Main.i.ObjectMethodBindPtrcall(bind, 0, args, (nint)(&ret));
            return ret;
        }
        public static T CallMethodBindObject<T>(nint bind, nint* args) where T : GodotObject
        {
            nint ptr;
            Main.i.ObjectMethodBindPtrcall(bind, 0, args, (nint)(&ptr));
            GCHandle handle = ClassDB.GetOrMakeHandleFromNative(ptr);
            return handle.Target as T;
        }
        public static T[] CallMethodBindArray<T>(nint bind, nint* args)
        {
            // TODO: Make this work :3
            throw new NotImplementedException();
        }
    }
}
