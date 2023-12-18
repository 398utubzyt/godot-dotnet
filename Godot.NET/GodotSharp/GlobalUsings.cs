#if REAL_T_IS_DOUBLE
global using Real = System.Double;
#else
global using Real = System.Single;
#endif

global using GDExtensionVariantPtr = System.IntPtr;
global using GDExtensionConstVariantPtr = System.IntPtr;
global using GDExtensionUninitializedVariantPtr = System.IntPtr;

global using GDExtensionStringNamePtr = System.IntPtr;
global using GDExtensionConstStringNamePtr = System.IntPtr;
global using GDExtensionUninitializedStringNamePtr = System.IntPtr;

global using GDExtensionStringPtr = System.IntPtr;
global using GDExtensionConstStringPtr = System.IntPtr;
global using GDExtensionUninitializedStringPtr = System.IntPtr;

global using GDExtensionObjectPtr = System.IntPtr;
global using GDExtensionConstObjectPtr = System.IntPtr;
global using GDExtensionUninitializedObjectPtr = System.IntPtr;

global using GDExtensionTypePtr = System.IntPtr;
global using GDExtensionConstTypePtr = System.IntPtr;
global using GDExtensionUninitializedTypePtr = System.IntPtr;

global using GDExtensionMethodBindPtr = System.IntPtr; // const void*

global using GDExtensionInt = System.Int64;
global using GDExtensionBool = System.Byte;
global using GDObjectInstanceID = System.UInt64;

global using GDExtensionRefPtr = System.IntPtr;
global using GDExtensionConstRefPtr = System.IntPtr;

global using GDExtensionClassInstancePtr = System.IntPtr;

global using GDExtensionClassLibraryPtr = System.IntPtr;

global using GDExtensionScriptInstanceDataPtr = System.IntPtr;
global using GDExtensionScriptLanguagePtr = System.IntPtr;
global using GDExtensionScriptInstancePtr = System.IntPtr;

global using GDExtensionInterfaceFunctionPtr = System.IntPtr;


global using SLayout = System.Runtime.InteropServices.StructLayoutAttribute;
global using SLayoutOpt = System.Runtime.InteropServices.LayoutKind;
global using MImpl = System.Runtime.CompilerServices.MethodImplAttribute;
global using MImplOpts = System.Runtime.CompilerServices.MethodImplOptions;
global using Deprecated = System.ObsoleteAttribute;

global using VariantType = Godot.Variant.Type;
global using VariantOp = Godot.Variant.Operator;

global using Godot.Collections;
global using Godot.GdExtension;
global using Godot.Interop;

global using PackedByteArray = byte[];
global using PackedInt32Array = int[];
global using PackedInt64Array = long[];
global using PackedFloat32Array = float[];
global using PackedFloat64Array = double[];
global using PackedStringArray = string[];

global using PackedVector2Array = Godot.Vector2[];
global using PackedVector3Array = Godot.Vector3[];
global using PackedColorArray = Godot.Color[];