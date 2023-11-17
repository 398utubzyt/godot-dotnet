using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Godot
{
    public partial class ClassDB
    {
        private enum PropertyHint : uint
        {
            None = 0,
            Range = 1,
            Enum = 2,
            EnumSuggestion = 3,
            ExpEasing = 4,
            Link = 5,
            Flags = 6,
            Layers2dRender = 7,
            Layers2dPhysics = 8,
            Layers2dNavigation = 9,
            Layers3dRender = 10,
            Layers3dPhysics = 11,
            Layers3dNavigation = 12,
            LayersAvoidance = 37,
            File = 13,
            Dir = 14,
            GlobalFile = 15,
            GlobalDir = 16,
            ResourceType = 17,
            MultilineText = 18,
            Expression = 19,
            PlaceholderText = 20,
            ColorNoAlpha = 21,
            ObjectId = 22,
            TypeString = 23,
            NodePathToEditedNode = 24,
            ObjectTooBig = 25,
            NodePathValidTypes = 26,
            SaveFile = 27,
            GlobalSaveFile = 28,
            IntIsObjectid = 29,
            IntIsPointer = 30,
            ArrayType = 31,
            LocaleId = 32,
            LocalizableString = 33,
            NodeType = 34,
            HideQuaternionEdit = 35,
            Password = 36,
            Max = 38,
        }

        [Flags]
        private enum UsageFlags : uint
        {
            None = 0,
            Storage = 2,
            Editor = 4,
            Internal = 8,
            Checkable = 16,
            Checked = 32,
            Group = 64,
            Category = 128,
            Subgroup = 256,
            ClassIsBitfield = 512,
            NoInstanceState = 1024,
            RestartIfChanged = 2048,
            ScriptVariable = 4096,
            StoreIfNull = 8192,
            UpdateAllIfModified = 16384,
            ScriptDefaultValue = 32768,
            ClassIsEnum = 65536,
            NilIsVariant = 131072,
            Array = 262144,
            AlwaysDuplicate = 524288,
            NeverDuplicate = 1048576,
            HighEndGfx = 2097152,
            NodePathFromSceneRoot = 4194304,
            ResourceNotPersistent = 8388608,
            KeyingIncrements = 16777216,
            DeferredSetResource = 33554432,
            EditorInstantiateObject = 67108864,
            EditorBasicSetting = 134217728,
            ReadOnly = 268435456,
            Secret = 536870912,
            Default = 6,
            NoEditor = 2,
        }

        private static void SafeCheckNoNull(object obj, string name)
        {
            if (obj is null)
                throw new ArgumentNullException(name);
            if (obj is not GodotObject)
                throw new ArgumentException($"Cannot register info from '{obj.GetType()}' because it does not derive from '{nameof(GodotObject)}'.");
        }
        private static void NoNull(object obj, string name)
        {
            if (obj is null)
                throw new ArgumentNullException(name);
        }
        private static void SafeCheck(object obj)
        {
            if (obj is not GodotObject && obj is not null)
                throw new ArgumentException($"Cannot register info from '{obj.GetType()}' because it does not derive from '{nameof(GodotObject)}'.");
        }

#if REAL_T_IS_DOUBLE
        private const ClassMethodArgumentMetadata REAL_TYPE = ClassMethodArgumentMetadata.RealIsDouble;
#else
        private const ClassMethodArgumentMetadata REAL_TYPE = ClassMethodArgumentMetadata.RealIsFloat;
#endif
        private const ClassMethodArgumentMetadata INT_REAL_TYPE = ClassMethodArgumentMetadata.IntIsInt32;

        private static unsafe void Error(CallError* error, CallErrorType type, int arg, int expected)
        {
            if (error == null)
                return;

            error->Error = type;
            error->Argument = arg;
            error->Expected = expected;
        }

        private static class ManagedHelper
        {
            private static Type GetType(nint handle)
                => Type.GetTypeFromHandle(RuntimeTypeHandle.FromIntPtr(handle));
            private static T Instance<T>(nint bind) where T : GodotObject
                => GCHandle.FromIntPtr(bind).Target as T;
            private static GodotObject CreateUninitializedType(
                [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors
                | DynamicallyAccessedMemberTypes.NonPublicConstructors)] Type type)
            {
                return RuntimeHelpers.GetUninitializedObject(type) as GodotObject
                    ?? throw new ArgumentException("Type does not derive from Godot.GodotObject.", nameof(type));
            }
            private static bool TypeIsBuiltin(Type type)
                => type.Assembly == typeof(GodotObject).Assembly && !type.Name.StartsWith("CSharp") && !type.Name.StartsWith("Test");
            public static unsafe nint CreateNative(Type type)
            {
                Type parent = type;
                while (!TypeIsBuiltin(parent))
                    parent = parent.BaseType;

                StringName sn = parent == typeof(GodotObject) ? "Object" : parent.Name;
                return Main.i.ClassdbConstructObject((nint)(&sn));
            }
            public static unsafe InstanceBindingCallbacks MakeBindingCallbacks()
            {
                return new InstanceBindingCallbacks
                    { CreateCallback = &BindCreate, FreeCallback = &BindFree, ReferenceCallback = &BindReference };
            }
            public static unsafe void TieNative(Type type, GCHandle bind, nint handle)
            {
                if (!TypeIsBuiltin(type) && TypeDB.Search(type, out StringName sn))
                    Main.i.ObjectSetInstance(handle, (nint)(&sn), (nint)bind);

                InstanceBindingCallbacks ibc = MakeBindingCallbacks();
                Main.i.ObjectSetInstanceBinding(handle, (void*)Main.lib, (void*)(nint)bind, &ibc);
            }
            public static unsafe GodotObject CreateManaged(Type type)
            {
                GodotObject obj = CreateUninitializedType(type);
                if (obj == null)
                    return null;

                nint handle = CreateNative(type);
                if (handle == 0)
                    throw new ArgumentException($"Type {type} is not a Godot type registered in ClassDB.");

                GCHandle bind = GCHandle.Alloc(obj, GCHandleType.Weak);
                TieNative(type, bind, handle);
                obj.Initialize(bind, handle, obj is RefCounted);
                return obj;
            }

            #region UnmanagedCallers Only

            [UnmanagedCallersOnly]
            public static unsafe nint Create(void* data)
                => CreateManaged(GetType((nint)data))?.Handle ?? 0;
            [UnmanagedCallersOnly]
            public static unsafe void Free(void* data, nint managed)
            {
                Instance<GodotObject>(managed)?.Dispose();
            }
            [UnmanagedCallersOnly]
            public static unsafe nint Recreate(void* data, nint managed)
            {
                if (managed != 0)
                    Instance<GodotObject>(managed)?.Dispose();

                return CreateManaged(GetType((nint)data))?.Handle ?? 0;
            }
            [UnmanagedCallersOnly]
            public static unsafe void Reference(nint managed)
            {
                Instance<RefCounted>(managed)?.__gdext_Reference();
            }
            [UnmanagedCallersOnly]
            public static unsafe void Unreference(nint managed)
            {
                Instance<RefCounted>(managed)?.__gdext_Unreference();
            }
            [UnmanagedCallersOnly]
            public static unsafe ulong GetRid(nint managed)
            {
                return Instance<Resource>(managed)?.__gdext_GetRid().Id ?? 0;
            }
            [UnmanagedCallersOnly]
            public static unsafe void ToString(nint managed, byte* isValid, nint str)
            {
                string mstr = Instance<GodotObject>(managed)?.ToString();
                if (mstr == null)
                {
                    *isValid = 0;
                    return;
                }

                *isValid = 1;
                fixed(char* chars = mstr)
                    Main.i.StringNewWithUtf16Chars(str, (ushort*)chars);
            }
            [UnmanagedCallersOnly]
            public static unsafe void Notification(nint managed, int notification, byte reversed)
            {
                Instance<GodotObject>(managed)?._Notification(notification, reversed.ToBool());
            }

            private delegate bool MagicCanCallVirtualHandle(StringName name);
            [UnmanagedCallersOnly]
            public static unsafe void* VirtualGetData(void* type, nint name)
            {
                // TODO: Replace this with some sort of static dictionary. VirtualDB?
                Type t = GetType((nint)type);
                MagicCanCallVirtualHandle m = t.GetMethod("_CanCall",
                    System.Reflection.BindingFlags.Static |
                    System.Reflection.BindingFlags.NonPublic)?.CreateDelegate<MagicCanCallVirtualHandle>();
                return (void*)(m != null && m(new StringName(*(nint*)name))).ToExtBool();
            }
            [UnmanagedCallersOnly]
            public static unsafe void VirtualCall(nint managed, nint name, void* data, nint* args, nint ret)
            {
                //Instance<GodotObject>(managed)?.CallPtr(new StringName(*(nint*)name), (Variant**)args, (Variant*)ret);
                if ((nint)data == 0)
                    return;

                ref nint vargs = ref MemUtil.NullRef<nint>();
                if (args != null)
                    vargs = ref *args;

                Instance<GodotObject>(managed)?.__gdext_Call(
                    new StringName(*(nint*)name),
                    ref vargs,
                    ret);
            }
            [UnmanagedCallersOnly]
            public static unsafe byte Set(nint managed, nint name, nint value)
            {
                return Instance<GodotObject>(managed)?.__gdext_Set(new StringName(*(nint*)name), *(Variant*)value).ToExtBool() ?? 0;
            }
            [UnmanagedCallersOnly]
            public static unsafe byte Get(nint managed, nint name, nint value)
            {
                return Instance<GodotObject>(managed)?.__gdext_Get(new StringName(*(nint*)name), ref **(Variant**)value).ToExtBool() ?? 0;
            }
            [UnmanagedCallersOnly]
            public static unsafe byte ValidateProperty(nint managed, GdExtension.PropertyInfo* info)
            {
                PropertyInfo pi = new PropertyInfo() { 
                    Name = *(nint*)info->Name == 0 ? SName.SearchOrCreate(*(StringName*)info->Name) : null,
                    Type = info->Type,
                    Hint = (Godot.PropertyHint)info->Hint,
                    HintString = *(nint*)info->HintString == 0 ? StringDB.SearchOrCreate(*(nint*)info->HintString) : null };

                if ((!Instance<GodotObject>(managed)?.__gdext_ValidateProperty(ref pi)) ?? true)
                    return 0;

                *(StringName*)info->Name = pi.Name != null ? SName.Register(pi.Name) : default;
                info->Type = pi.Type;
                info->Hint = (uint)pi.Hint;
                *(nint*)info->HintString = pi.HintString != null ? StringDB.Register(pi.HintString) : 0;
                return 1;
            }
            [UnmanagedCallersOnly]
            public static unsafe byte PropertyCanRevert(nint managed, nint property)
            {
                return Instance<GodotObject>(managed)?.__gdext_PropertyCanRevert(*(StringName*)property).ToExtBool() ?? 0;
            }
            [UnmanagedCallersOnly]
            public static unsafe byte PropertyGetRevert(nint managed, nint property, nint ret)
            {
                return Instance<GodotObject>(managed)?.__gdext_PropertyGetRevert(*(StringName*)property, ref *(Variant*)ret).ToExtBool() ?? 0;
            }
            [UnmanagedCallersOnly]
            public static unsafe GdExtension.PropertyInfo* GetPropertyList(nint managed, uint* count)
            {
                GodotObject obj = Instance<GodotObject>(managed);
                if (obj == null)
                {
                    *count = 0;
                    return null;
                }

                nuint pcount = obj.__gdext_PropertyCount();
                if (pcount == 0)
                {
                    *count = 0;
                    return null;
                }

                *count = (uint)pcount;
                GdExtension.PropertyInfo* info = MemUtil.Alloc<GdExtension.PropertyInfo>(pcount);
                Span<PropertyInfo> minfo = MemUtil.AsSpan(ref MemUtil.RefAlloc<PropertyInfo>(pcount), pcount);

                obj.__gdext_GetPropertyList(minfo);
                for (nuint i = 0; i < pcount; i++)
                {
                    *(StringName*)info[i].Name = SName.Register(minfo[(int)i].Name);
                    info[i].Type = minfo[(int)i].Type;
                    info[i].Hint = (uint)minfo[(int)i].Hint;
                    *(nint*)info[i].HintString = StringDB.Register(minfo[(int)i].HintString);
                }

                MemUtil.Free(ref minfo.GetRef());
                return info;
            }
            [UnmanagedCallersOnly]
            public static unsafe void FreePropertyList(nint managed, GdExtension.PropertyInfo* info)
            {
                MemUtil.Free(info);
            }

            [UnmanagedCallersOnly]
            public unsafe static void* BindCreate(void* lib, void* handle)
            {
                StringName sn;
                if (!Main.i.ObjectGetClassName((nint)handle, Main.lib, (nint)(&sn)).ToBool() || !TypeDB.Search(sn, out Type type))
                    return null;

                GodotObject obj = CreateUninitializedType(type);
                if (obj == null)
                    return null;

                bool refcounted = obj is RefCounted;
                GCHandle bind = GCHandle.Alloc(obj, refcounted ? GCHandleType.Weak : GCHandleType.Normal);
                obj.Initialize(bind, (nint)handle, refcounted);
                return (void*)(nint)bind;
            }
            [UnmanagedCallersOnly]
            public unsafe static void BindFree(void* lib, void* native, void* bind)
            {
                (GCHandle.FromIntPtr((nint)bind).Target as GodotObject)?.Dispose();
            }
            [UnmanagedCallersOnly]
            public unsafe static byte BindReference(void* lib, void* bind, byte reference)
                => 1;

            #endregion
        }

        internal static unsafe T GetSingletonInstance<T>(StringName name) where T : GodotObject
        {
            return GetManagedForHandle(Main.i.GlobalGetSingleton((nint)(&name))).Target as T;
        }
        internal static nint MakeHandleForManaged(Type type, GCHandle bind)
        {
            nint handle = ManagedHelper.CreateNative(type);
            ManagedHelper.TieNative(type, bind, handle);
            return handle;
        }
        internal static unsafe GCHandle GetManagedForHandle(nint native)
        {
            InstanceBindingCallbacks ibc = ManagedHelper.MakeBindingCallbacks();
            return GCHandle.FromIntPtr((nint)Main.i.ObjectGetInstanceBinding(native, (void*)Main.lib, &ibc));
        }

        internal static unsafe void PluginRegisterClass<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods)] T>(bool global = false) 
            where T : GodotObject
        {
            ClassCreationInfo2 cci = new ClassCreationInfo2();
            
            Type type = typeof(T);
            cci.IsAbstract = type.IsAbstract.ToExtBool();
            cci.IsVirtual = type.IsAbstract.ToExtBool();
            cci.IsExposed = global.ToExtBool();
            cci.ClassUserData = (void*)typeof(T).TypeHandle.Value;
            cci.CreateInstanceFunc = &ManagedHelper.Create;
            cci.RecreateInstanceFunc = &ManagedHelper.Recreate;
            cci.FreeInstanceFunc = &ManagedHelper.Free;
            cci.ReferenceFunc = &ManagedHelper.Reference;
            cci.UnreferenceFunc = &ManagedHelper.Unreference;
            cci.ToStringFunc = &ManagedHelper.ToString;
            cci.NotificationFunc = &ManagedHelper.Notification;
            cci.GetRidFunc = &ManagedHelper.GetRid;
            cci.SetFunc = &ManagedHelper.Set;
            cci.GetFunc = &ManagedHelper.Get;
            cci.GetVirtualCallDataFunc = &ManagedHelper.VirtualGetData;
            cci.CallVirtualWithDataFunc = &ManagedHelper.VirtualCall;
            cci.ValidatePropertyFunc = &ManagedHelper.ValidateProperty;
            cci.PropertyCanRevertFunc = &ManagedHelper.PropertyCanRevert;
            cci.PropertyGetRevertFunc = &ManagedHelper.PropertyGetRevert;
            cci.GetPropertyListFunc = &ManagedHelper.GetPropertyList;
            cci.FreePropertyListFunc = &ManagedHelper.FreePropertyList;

            StringName cn = type.Name;
            StringName pcn = type.BaseType != typeof(GodotObject) ? type.BaseType.Name : "Object";
            Main.i.ClassdbRegisterExtensionClass2(Main.lib, (nint)(&cn), (nint)(&pcn), &cci);
            TypeDB.Register(type, cn);
        }

        public static unsafe void UnregisterClass<T>() where T : GodotObject
        {
            TypeDB.Unregister(typeof(T));

            StringName cn = typeof(T).Name;
            Main.i.ClassdbUnregisterExtensionClass(Main.lib, (nint)(&cn));
        }

        [UnmanagedCallersOnly]
        private static unsafe void CallClassDBMethod(void* data, nint instance, nint* args, long count, nint ret, CallError* error)
        {
            // NOTE: Do not throw exceptions here!!
            if (data == null)
            {
                Error(error, CallErrorType.InvalidMethod, 0, 0);
                return;
            }


        }

        public static unsafe void RegisterMethod(Delegate method)
        {
            NoNull(method, nameof(method));
            SafeCheck(method.Target);

            System.Reflection.MethodInfo info = method.Method;
            StringName declType = info.DeclaringType?.Name ?? throw new MethodAccessException("Method must be defined within a class.");

            ClassMethodInfo cmi = new ClassMethodInfo();
            cmi.Name = (nint)(StringName)info.Name;
            if (info.ReturnType != null)
            {
                cmi.HasReturnValue = 1;
                
            }

            System.Reflection.ParameterInfo[] pi = info.GetParameters();
            cmi.ArgumentCount = (uint)pi.Length;
            GdExtension.PropertyInfo* args = stackalloc GdExtension.PropertyInfo[pi.Length];
            ClassMethodArgumentMetadata* argMeta = stackalloc ClassMethodArgumentMetadata[pi.Length];
            if (pi.Length > 0)
            {
                for (int i = 0; i < pi.Length; i++)
                {
                    args[i].Name = (nint)(StringName)pi[i].Name;

                    Type ptype = pi[i].ParameterType;
                    switch (ptype.FullName)
                    {
                        case "System.SByte":
                            args[i].Type = VariantType.Int;
                            argMeta[i] = ClassMethodArgumentMetadata.IntIsInt8;
                            break;
                        case "System.Byte":
                            args[i].Type = VariantType.Int;
                            argMeta[i] = ClassMethodArgumentMetadata.IntIsUInt8;
                            break;
                        case "System.Int16":
                            args[i].Type = VariantType.Int;
                            argMeta[i] = ClassMethodArgumentMetadata.IntIsInt16;
                            break;
                        case "System.UInt16":
                            args[i].Type = VariantType.Int;
                            argMeta[i] = ClassMethodArgumentMetadata.IntIsUInt16;
                            break;
                        case "System.Int32":
                            args[i].Type = VariantType.Int;
                            argMeta[i] = ClassMethodArgumentMetadata.IntIsInt32;
                            break;
                        case "System.UInt32":
                            args[i].Type = VariantType.Int;
                            argMeta[i] = ClassMethodArgumentMetadata.IntIsUInt32;
                            break;
                        case "System.Int64":
                            args[i].Type = VariantType.Int;
                            argMeta[i] = ClassMethodArgumentMetadata.IntIsInt64;
                            break;
                        case "System.UInt64":
                            args[i].Type = VariantType.Int;
                            argMeta[i] = ClassMethodArgumentMetadata.IntIsUInt64;
                            break;
                        case "System.Single":
                            args[i].Type = VariantType.Float;
                            argMeta[i] = ClassMethodArgumentMetadata.RealIsFloat;
                            break;
                        case "System.Double":
                            args[i].Type = VariantType.Float;
                            argMeta[i] = ClassMethodArgumentMetadata.RealIsDouble;
                            break;
                        case "System.Bool":
                            args[i].Type = VariantType.Bool;
                            break;
                        case "Godot.Vector2":
                            args[i].Type = VariantType.Vector2;
                            argMeta[i] = REAL_TYPE;
                            break;
                        case "Godot.Vector2I":
                            args[i].Type = VariantType.Vector2I;
                            argMeta[i] = INT_REAL_TYPE;
                            break;
                        case "Godot.Rect2":
                            args[i].Type = VariantType.Rect2;
                            argMeta[i] = REAL_TYPE;
                            break;
                        case "Godot.Rect2I":
                            args[i].Type = VariantType.Rect2I;
                            argMeta[i] = INT_REAL_TYPE;
                            break;
                        case "Godot.Vector3":
                            args[i].Type = VariantType.Vector3;
                            argMeta[i] = REAL_TYPE;
                            break;
                        case "Godot.Vector3I":
                            args[i].Type = VariantType.Vector3I;
                            argMeta[i] = INT_REAL_TYPE;
                            break;
                        case "Godot.Transform2D":
                            args[i].Type = VariantType.Transform2D;
                            argMeta[i] = REAL_TYPE;
                            break;
                        case "Godot.Vector4":
                            args[i].Type = VariantType.Vector4;
                            argMeta[i] = REAL_TYPE;
                            break;
                        case "Godot.Vector4I":
                            args[i].Type = VariantType.Vector4I;
                            argMeta[i] = INT_REAL_TYPE;
                            break;
                        case "Godot.Color":
                            args[i].Type = VariantType.Color;
                            break;
                        case "Godot.StringName":
                            args[i].Type = VariantType.StringName;
                            break;
                        case "Godot.NodePath":
                            args[i].Type = VariantType.NodePath;
                            break;
                        case "Godot.Rid":
                            args[i].Type = VariantType.Rid;
                            break;
                        case "Godot.Collections.VariantArray":
                            args[i].Type = VariantType.Array;
                            break;
                        case "Godot.Collections.VariantDictionary":
                            args[i].Type = VariantType.Dictionary;
                            break;
                        default:
                            if (ptype.IsSZArray && ptype.HasElementType)
                            {
                                Type petype = ptype.GetElementType();
                                args[i].Type = petype.FullName switch
                                {
                                    "System.Byte" => VariantType.PackedByteArray,
                                    "System.Int32" => VariantType.PackedInt32Array,
                                    "System.Int64" => VariantType.PackedInt64Array,
                                    "System.Single" => VariantType.PackedFloat32Array,
                                    "System.Double" => VariantType.PackedFloat64Array,
                                    "System.String" => VariantType.PackedStringArray,
                                    "Godot.Vector2" => VariantType.PackedVector2Array,
                                    "Godot.Vector3" => VariantType.PackedVector3Array,
                                    "Godot.Color" => VariantType.PackedColorArray,
                                    _ => petype.IsAssignableTo(typeof(GodotObject)) ? VariantType.Array : 
                                        throw new ArgumentException($"The element type of the array '{pi[i].Name}' must be a Variant.")
                                };
                                break;
                            }

                            if (ptype.IsAssignableTo(typeof(GodotObject)))
                            {
                                args[i].Type = VariantType.Object;
                                args[i].ClassName = (nint)(StringName)ptype.Name;
                                break;
                            }

                            throw new ArgumentException($"The argument '{pi[i].Name}' ({ptype}) must be a Variant type or derive from 'Godot.GodotObject'.");
                    }
                }
                cmi.ArgumentsInfo = args;
            }
            cmi.ArgumentsMetadata = argMeta;
            cmi.MethodUserData = (void*)info.MethodHandle.GetFunctionPointer();

            cmi.CallFunc = &CallClassDBMethod;

            Main.i.ClassdbRegisterExtensionClassMethod(Main.lib, (nint)declType, &cmi);
        }

        internal static unsafe T InstantiateManaged<T>() where T : GodotObject
        {
            return ManagedHelper.CreateManaged(typeof(T)) as T;
        }
    }
}
