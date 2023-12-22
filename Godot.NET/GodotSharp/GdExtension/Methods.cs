global using unsafe GDExtensionVariantFromTypeConstructorFunc = delegate* unmanaged <nint, nint, void>;
global using unsafe GDExtensionTypeFromVariantConstructorFunc = delegate* unmanaged <nint, nint, void>;
global using unsafe GDExtensionPtrOperatorEvaluator = delegate* unmanaged <nint, nint, nint, void>;
global using unsafe GDExtensionPtrBuiltInMethod = delegate* unmanaged <nint, nint*, nint, int, void>;
global using unsafe GDExtensionPtrConstructor = delegate* unmanaged <nint, nint*, void>;
global using unsafe GDExtensionPtrDestructor = delegate* unmanaged <nint, void>;
global using unsafe GDExtensionPtrSetter = delegate* unmanaged <nint, nint, void>;
global using unsafe GDExtensionPtrGetter = delegate* unmanaged <nint, nint, void>;
global using unsafe GDExtensionPtrIndexedSetter = delegate* unmanaged <nint, long, nint, void>;
global using unsafe GDExtensionPtrIndexedGetter = delegate* unmanaged <nint, long, nint, void>;
global using unsafe GDExtensionPtrKeyedSetter = delegate* unmanaged <nint, nint, nint, void>;
global using unsafe GDExtensionPtrKeyedGetter = delegate* unmanaged <nint, nint, nint, void>;
global using unsafe GDExtensionPtrKeyedChecker = delegate* unmanaged <nint, nint, uint>;
global using unsafe GDExtensionPtrUtilityFunction = delegate* unmanaged <nint, nint*, int, void>;

global using unsafe ClassConstructor = delegate* unmanaged <nint>;

global using unsafe GDExtensionClassSet = delegate* unmanaged <nint, nint, nint, byte>;
global using unsafe GDExtensionClassGet = delegate* unmanaged <nint, nint, nint, byte>;
global using unsafe GDExtensionClassGetRID = delegate* unmanaged <nint, ulong>;

// TODO: Convert these to global usings

namespace Godot.GdExtension;

unsafe delegate void GDExtensionClassMethodCall(void* method_userdata, GDExtensionClassInstancePtr p_instance, GDExtensionConstVariantPtr* p_args, GDExtensionInt p_argument_count, GDExtensionVariantPtr r_return, CallError* r_error);
unsafe delegate void GDExtensionClassMethodValidatedCall(void* method_userdata, GDExtensionClassInstancePtr p_instance, GDExtensionConstVariantPtr* p_args, GDExtensionVariantPtr r_return);
unsafe delegate void GDExtensionClassMethodPtrCall(void* method_userdata, GDExtensionClassInstancePtr p_instance, GDExtensionConstTypePtr* p_args, GDExtensionTypePtr r_ret);



unsafe delegate void GDExtensionCallableCustomCall(void* callable_userdata, GDExtensionConstVariantPtr* p_args, GDExtensionInt p_argument_count, GDExtensionVariantPtr r_return, CallError* r_error);
unsafe delegate GDExtensionBool GDExtensionCallableCustomIsValid(void* callable_userdata);
unsafe delegate void GDExtensionCallableCustomFree(void* callable_userdata);

unsafe delegate uint GDExtensionCallableCustomHash(void* callable_userdata);
unsafe delegate GDExtensionBool GDExtensionCallableCustomEqual(void* callable_userdata_a, void* callable_userdata_b);
unsafe delegate GDExtensionBool GDExtensionCallableCustomLessThan(void* callable_userdata_a, void* callable_userdata_b);

unsafe delegate void GDExtensionCallableCustomToString(void* callable_userdata, GDExtensionBool* r_is_valid, GDExtensionStringPtr r_out);



delegate GDExtensionBool GDExtensionScriptInstanceSet(GDExtensionScriptInstanceDataPtr p_instance, GDExtensionConstStringNamePtr p_name, GDExtensionConstVariantPtr p_value);
delegate GDExtensionBool GDExtensionScriptInstanceGet(GDExtensionScriptInstanceDataPtr p_instance, GDExtensionConstStringNamePtr p_name, GDExtensionVariantPtr r_ret);
unsafe delegate PropertyInfo* GDExtensionScriptInstanceGetPropertyList(GDExtensionScriptInstanceDataPtr p_instance, uint* r_count);
unsafe delegate void GDExtensionScriptInstanceFreePropertyList(GDExtensionScriptInstanceDataPtr p_instance, PropertyInfo* p_list);
unsafe delegate VariantType GDExtensionScriptInstanceGetPropertyType(GDExtensionScriptInstanceDataPtr p_instance, GDExtensionConstStringNamePtr p_name, GDExtensionBool* r_is_valid);
unsafe delegate GDExtensionBool GDExtensionScriptInstanceValidateProperty(GDExtensionScriptInstanceDataPtr p_instance, PropertyInfo* p_property);

delegate GDExtensionBool GDExtensionScriptInstancePropertyCanRevert(GDExtensionScriptInstanceDataPtr p_instance, GDExtensionConstStringNamePtr p_name);
delegate GDExtensionBool GDExtensionScriptInstancePropertyGetRevert(GDExtensionScriptInstanceDataPtr p_instance, GDExtensionConstStringNamePtr p_name, GDExtensionVariantPtr r_ret);

delegate GDExtensionObjectPtr GDExtensionScriptInstanceGetOwner(GDExtensionScriptInstanceDataPtr p_instance);
unsafe delegate void GDExtensionScriptInstancePropertyStateAdd(GDExtensionConstStringNamePtr p_name, GDExtensionConstVariantPtr p_value, void* p_userdata);
unsafe delegate void GDExtensionScriptInstanceGetPropertyState(GDExtensionScriptInstanceDataPtr p_instance, GDExtensionScriptInstancePropertyStateAdd p_add_func, void* p_userdata);

unsafe delegate MethodInfo* GDExtensionScriptInstanceGetMethodList(GDExtensionScriptInstanceDataPtr p_instance, uint* r_count);
unsafe delegate void GDExtensionScriptInstanceFreeMethodList(GDExtensionScriptInstanceDataPtr p_instance, MethodInfo* p_list);

delegate GDExtensionBool GDExtensionScriptInstanceHasMethod(GDExtensionScriptInstanceDataPtr p_instance, GDExtensionConstStringNamePtr p_name);

unsafe delegate void GDExtensionScriptInstanceCall(GDExtensionScriptInstanceDataPtr p_self, GDExtensionConstStringNamePtr p_method, GDExtensionConstVariantPtr* p_args, GDExtensionInt p_argument_count, GDExtensionVariantPtr r_return, CallError* r_error);
[Deprecated("Deprecated. Use GDExtensionScriptInstanceNotification2 instead.")]
delegate void GDExtensionScriptInstanceNotification(GDExtensionScriptInstanceDataPtr p_instance, int p_what);
delegate void GDExtensionScriptInstanceNotification2(GDExtensionScriptInstanceDataPtr p_instance, int p_what, GDExtensionBool p_reversed);
unsafe delegate void GDExtensionScriptInstanceToString(GDExtensionScriptInstanceDataPtr p_instance, GDExtensionBool* r_is_valid, GDExtensionStringPtr r_out);

delegate void GDExtensionScriptInstanceRefCountIncremented(GDExtensionScriptInstanceDataPtr p_instance);
delegate GDExtensionBool GDExtensionScriptInstanceRefCountDecremented(GDExtensionScriptInstanceDataPtr p_instance);

delegate GDExtensionObjectPtr GDExtensionScriptInstanceGetScript(GDExtensionScriptInstanceDataPtr p_instance);
delegate GDExtensionBool GDExtensionScriptInstanceIsPlaceholder(GDExtensionScriptInstanceDataPtr p_instance);

delegate GDExtensionScriptLanguagePtr GDExtensionScriptInstanceGetLanguage(GDExtensionScriptInstanceDataPtr p_instance);

delegate void GDExtensionScriptInstanceFree(GDExtensionScriptInstanceDataPtr p_instance);