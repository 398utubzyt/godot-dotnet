namespace Godot.GdExtension
{
    [SLayout(SLayoutOpt.Sequential)]
    [Deprecated("Deprecated. Use ScriptInstanceInfo2 instead.")]
    internal unsafe struct ScriptInstanceInfo
    {
        public unsafe delegate* unmanaged[Cdecl]<GDExtensionScriptInstanceSet> SetFunc;
        public unsafe delegate* unmanaged[Cdecl]<GDExtensionScriptInstanceGet> GetFunc;
        public unsafe delegate* unmanaged[Cdecl]<GDExtensionScriptInstanceGetPropertyList> GetPropertyListFunc;
        public unsafe delegate* unmanaged[Cdecl]<GDExtensionScriptInstanceFreePropertyList> FreePropertyListFunc;

        public unsafe delegate* unmanaged[Cdecl]<GDExtensionScriptInstancePropertyCanRevert> PropertyCanRevertFunc;
        public unsafe delegate* unmanaged[Cdecl]<GDExtensionScriptInstancePropertyGetRevert> PropertyGetRevertFunc;

        public unsafe delegate* unmanaged[Cdecl]<GDExtensionScriptInstanceGetOwner> GetOwnerFunc;
        public unsafe delegate* unmanaged[Cdecl]<GDExtensionScriptInstanceGetPropertyState> GetPropertyStateFunc;

        public unsafe delegate* unmanaged[Cdecl]<GDExtensionScriptInstanceGetMethodList> GetMethodListFunc;
        public unsafe delegate* unmanaged[Cdecl]<GDExtensionScriptInstanceFreeMethodList> FreeMethodListFunc;
        public unsafe delegate* unmanaged[Cdecl]<GDExtensionScriptInstanceGetPropertyType> GetPropertyTypeFunc;

        public unsafe delegate* unmanaged[Cdecl]<GDExtensionScriptInstanceHasMethod> HasMethodFunc;

        public unsafe delegate* unmanaged[Cdecl]<GDExtensionScriptInstanceCall> CallFunc;
        public unsafe delegate* unmanaged[Cdecl]<GDExtensionScriptInstanceNotification> NotificationFunc;

        public unsafe delegate* unmanaged[Cdecl]<GDExtensionScriptInstanceToString> ToStringFunc;

        public unsafe delegate* unmanaged[Cdecl]<GDExtensionScriptInstanceRefCountIncremented> RefCountIncrementedFunc;
        public unsafe delegate* unmanaged[Cdecl]<GDExtensionScriptInstanceRefCountDecremented> RefCountDecrementedFunc;

        public unsafe delegate* unmanaged[Cdecl]<GDExtensionScriptInstanceGetScript> GetScriptFunc;

        public unsafe delegate* unmanaged[Cdecl]<GDExtensionScriptInstanceIsPlaceholder> IsPlaceholderFunc;

        public unsafe delegate* unmanaged[Cdecl]<GDExtensionScriptInstanceSet> SetFallbackFunc;
        public unsafe delegate* unmanaged[Cdecl]<GDExtensionScriptInstanceGet> GetFallbackFunc;

        public unsafe delegate* unmanaged[Cdecl]<GDExtensionScriptInstanceGetLanguage> GetLanguageFunc;

        public unsafe delegate* unmanaged[Cdecl]<GDExtensionScriptInstanceFree> FreeFunc;
    }
}
