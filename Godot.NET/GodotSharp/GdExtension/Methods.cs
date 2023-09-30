using System;

namespace Godot.GdExtension
{
    delegate void GDExtensionVariantFromTypeConstructorFunc(GDExtensionUninitializedVariantPtr p_variant, GDExtensionTypePtr p_type);
    delegate void GDExtensionTypeFromVariantConstructorFunc(GDExtensionUninitializedTypePtr p_type, GDExtensionVariantPtr p_variant);
    delegate void GDExtensionPtrOperatorEvaluator(GDExtensionConstTypePtr p_left, GDExtensionConstTypePtr p_right, GDExtensionTypePtr r_result);
    unsafe delegate void GDExtensionPtrBuiltInMethod(GDExtensionTypePtr p_base, GDExtensionConstTypePtr* p_args, GDExtensionTypePtr r_return, int p_argument_count);
    unsafe delegate void GDExtensionPtrConstructor(GDExtensionUninitializedTypePtr p_base, GDExtensionConstTypePtr* p_args);
    delegate void GDExtensionPtrDestructor(GDExtensionTypePtr p_base);
    delegate void GDExtensionPtrSetter(GDExtensionTypePtr p_base, GDExtensionConstTypePtr p_value);
    delegate void GDExtensionPtrGetter(GDExtensionConstTypePtr p_base, GDExtensionTypePtr r_value);
    delegate void GDExtensionPtrIndexedSetter(GDExtensionTypePtr p_base, GDExtensionInt p_index, GDExtensionConstTypePtr p_value);
    delegate void GDExtensionPtrIndexedGetter(GDExtensionConstTypePtr p_base, GDExtensionInt p_index, GDExtensionTypePtr r_value);
    delegate void GDExtensionPtrKeyedSetter(GDExtensionTypePtr p_base, GDExtensionConstTypePtr p_key, GDExtensionConstTypePtr p_value);
    delegate void GDExtensionPtrKeyedGetter(GDExtensionConstTypePtr p_base, GDExtensionConstTypePtr p_key, GDExtensionTypePtr r_value);
    delegate uint GDExtensionPtrKeyedChecker(GDExtensionConstVariantPtr p_base, GDExtensionConstVariantPtr p_key);
    unsafe delegate void GDExtensionPtrUtilityFunction(GDExtensionTypePtr r_return, GDExtensionConstTypePtr* p_args, int p_argument_count);

    delegate GDExtensionObjectPtr ClassConstructor();

    unsafe delegate void* GDExtensionInstanceBindingCreateCallback(void* p_token, void* p_instance);
    unsafe delegate void GDExtensionInstanceBindingFreeCallback(void* p_token, void* p_instance, void* p_binding);
    unsafe delegate GDExtensionBool GDExtensionInstanceBindingReferenceCallback(void* p_token, void* p_binding, GDExtensionBool p_reference);



    delegate GDExtensionBool GDExtensionClassSet(GDExtensionClassInstancePtr p_instance, GDExtensionConstStringNamePtr p_name, GDExtensionConstVariantPtr p_value);
    delegate GDExtensionBool GDExtensionClassGet(GDExtensionClassInstancePtr p_instance, GDExtensionConstStringNamePtr p_name, GDExtensionVariantPtr r_ret);
    delegate ulong GDExtensionClassGetRID(GDExtensionClassInstancePtr p_instance);



    unsafe delegate PropertyInfo* GDExtensionClassGetPropertyList(GDExtensionClassInstancePtr p_instance, uint* r_count);
    unsafe delegate void GDExtensionClassFreePropertyList(GDExtensionClassInstancePtr p_instance, PropertyInfo* p_list);
    delegate GDExtensionBool GDExtensionClassPropertyCanRevert(GDExtensionClassInstancePtr p_instance, GDExtensionConstStringNamePtr p_name);
    delegate GDExtensionBool GDExtensionClassPropertyGetRevert(GDExtensionClassInstancePtr p_instance, GDExtensionConstStringNamePtr p_name, 
        GDExtensionVariantPtr r_ret);
    unsafe delegate GDExtensionBool GDExtensionClassValidateProperty(GDExtensionClassInstancePtr p_instance, PropertyInfo* p_property);
    [Deprecated("Deprecated. Use GDExtensionClassNotification2 instead.")]
    delegate void GDExtensionClassNotification(GDExtensionClassInstancePtr p_instance, int p_what); // Deprecated. Use GDExtensionClassNotification2 instead.
    delegate void GDExtensionClassNotification2(GDExtensionClassInstancePtr p_instance, int p_what, GDExtensionBool p_reversed);
    unsafe delegate void GDExtensionClassToString(GDExtensionClassInstancePtr p_instance, GDExtensionBool* r_is_valid, GDExtensionStringPtr p_out);
    delegate void GDExtensionClassReference(GDExtensionClassInstancePtr p_instance);
    delegate void GDExtensionClassUnreference(GDExtensionClassInstancePtr p_instance);
    unsafe delegate void GDExtensionClassCallVirtual(GDExtensionClassInstancePtr p_instance, GDExtensionConstTypePtr* p_args, GDExtensionTypePtr r_ret);
    unsafe delegate GDExtensionObjectPtr GDExtensionClassCreateInstance(void* p_class_userdata);
    unsafe delegate void GDExtensionClassFreeInstance(void* p_class_userdata, GDExtensionClassInstancePtr p_instance);
    unsafe delegate GDExtensionClassInstancePtr GDExtensionClassRecreateInstance(void* p_class_userdata, GDExtensionObjectPtr p_object);
    unsafe delegate GDExtensionClassCallVirtual GDExtensionClassGetVirtual(void* p_class_userdata, GDExtensionConstStringNamePtr p_name);
    unsafe delegate void* GDExtensionClassGetVirtualCallData(void* p_class_userdata, GDExtensionConstStringNamePtr p_name);
    unsafe delegate void GDExtensionClassCallVirtualWithData(GDExtensionClassInstancePtr p_instance, GDExtensionConstStringNamePtr p_name, 
        void* p_virtual_call_userdata, GDExtensionConstTypePtr* p_args, GDExtensionTypePtr r_ret);



    unsafe delegate void GDExtensionClassMethodCall(void *method_userdata, GDExtensionClassInstancePtr p_instance, GDExtensionConstVariantPtr *p_args, GDExtensionInt p_argument_count, GDExtensionVariantPtr r_return, CallError *r_error);
    unsafe delegate void GDExtensionClassMethodValidatedCall(void *method_userdata, GDExtensionClassInstancePtr p_instance, GDExtensionConstVariantPtr *p_args, GDExtensionVariantPtr r_return);
    unsafe delegate void GDExtensionClassMethodPtrCall(void *method_userdata, GDExtensionClassInstancePtr p_instance, GDExtensionConstTypePtr *p_args, GDExtensionTypePtr r_ret);



    unsafe delegate void GDExtensionCallableCustomCall(void* callable_userdata, GDExtensionConstVariantPtr* p_args, GDExtensionInt p_argument_count, GDExtensionVariantPtr r_return, CallError* r_error);
    unsafe delegate GDExtensionBool GDExtensionCallableCustomIsValid(void* callable_userdata);
    unsafe delegate void GDExtensionCallableCustomFree(void* callable_userdata);

    unsafe delegate uint GDExtensionCallableCustomHash(void* callable_userdata);
    unsafe delegate GDExtensionBool GDExtensionCallableCustomEqual(void* callable_userdata_a, void* callable_userdata_b);
    unsafe delegate GDExtensionBool GDExtensionCallableCustomLessThan(void* callable_userdata_a, void* callable_userdata_b);

    unsafe delegate void GDExtensionCallableCustomToString(void* callable_userdata, GDExtensionBool* r_is_valid, GDExtensionStringPtr r_out);



    delegate GDExtensionBool GDExtensionScriptInstanceSet(GDExtensionScriptInstanceDataPtr p_instance, GDExtensionConstStringNamePtr p_name, GDExtensionConstVariantPtr p_value);
    delegate GDExtensionBool GDExtensionScriptInstanceGet(GDExtensionScriptInstanceDataPtr p_instance, GDExtensionConstStringNamePtr p_name, GDExtensionVariantPtr r_ret);
    unsafe delegate PropertyInfo *GDExtensionScriptInstanceGetPropertyList(GDExtensionScriptInstanceDataPtr p_instance, uint *r_count);
    unsafe delegate void GDExtensionScriptInstanceFreePropertyList(GDExtensionScriptInstanceDataPtr p_instance, PropertyInfo *p_list);
    unsafe delegate VariantType GDExtensionScriptInstanceGetPropertyType(GDExtensionScriptInstanceDataPtr p_instance, GDExtensionConstStringNamePtr p_name, GDExtensionBool *r_is_valid);
    unsafe delegate GDExtensionBool GDExtensionScriptInstanceValidateProperty(GDExtensionScriptInstanceDataPtr p_instance, PropertyInfo *p_property);

    delegate GDExtensionBool GDExtensionScriptInstancePropertyCanRevert(GDExtensionScriptInstanceDataPtr p_instance, GDExtensionConstStringNamePtr p_name);
    delegate GDExtensionBool GDExtensionScriptInstancePropertyGetRevert(GDExtensionScriptInstanceDataPtr p_instance, GDExtensionConstStringNamePtr p_name, GDExtensionVariantPtr r_ret);

    delegate GDExtensionObjectPtr GDExtensionScriptInstanceGetOwner(GDExtensionScriptInstanceDataPtr p_instance);
    unsafe delegate void GDExtensionScriptInstancePropertyStateAdd(GDExtensionConstStringNamePtr p_name, GDExtensionConstVariantPtr p_value, void *p_userdata);
    unsafe delegate void GDExtensionScriptInstanceGetPropertyState(GDExtensionScriptInstanceDataPtr p_instance, GDExtensionScriptInstancePropertyStateAdd p_add_func, void *p_userdata);

    unsafe delegate MethodInfo *GDExtensionScriptInstanceGetMethodList(GDExtensionScriptInstanceDataPtr p_instance, uint *r_count);
    unsafe delegate void GDExtensionScriptInstanceFreeMethodList(GDExtensionScriptInstanceDataPtr p_instance, MethodInfo *p_list);

    delegate GDExtensionBool GDExtensionScriptInstanceHasMethod(GDExtensionScriptInstanceDataPtr p_instance, GDExtensionConstStringNamePtr p_name);

    unsafe delegate void GDExtensionScriptInstanceCall(GDExtensionScriptInstanceDataPtr p_self, GDExtensionConstStringNamePtr p_method, GDExtensionConstVariantPtr *p_args, GDExtensionInt p_argument_count, GDExtensionVariantPtr r_return, CallError *r_error);
    [Deprecated("Deprecated. Use GDExtensionScriptInstanceNotification2 instead.")]
    delegate void GDExtensionScriptInstanceNotification(GDExtensionScriptInstanceDataPtr p_instance, int p_what);
    delegate void GDExtensionScriptInstanceNotification2(GDExtensionScriptInstanceDataPtr p_instance, int p_what, GDExtensionBool p_reversed);
    unsafe delegate void GDExtensionScriptInstanceToString(GDExtensionScriptInstanceDataPtr p_instance, GDExtensionBool *r_is_valid, GDExtensionStringPtr r_out);

    delegate void GDExtensionScriptInstanceRefCountIncremented(GDExtensionScriptInstanceDataPtr p_instance);
    delegate GDExtensionBool GDExtensionScriptInstanceRefCountDecremented(GDExtensionScriptInstanceDataPtr p_instance);

    delegate GDExtensionObjectPtr GDExtensionScriptInstanceGetScript(GDExtensionScriptInstanceDataPtr p_instance);
    delegate GDExtensionBool GDExtensionScriptInstanceIsPlaceholder(GDExtensionScriptInstanceDataPtr p_instance);

    delegate GDExtensionScriptLanguagePtr GDExtensionScriptInstanceGetLanguage(GDExtensionScriptInstanceDataPtr p_instance);

    delegate void GDExtensionScriptInstanceFree(GDExtensionScriptInstanceDataPtr p_instance);
}
