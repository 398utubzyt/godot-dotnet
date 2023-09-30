namespace Godot.GdExtension
{
    [SLayout(SLayoutOpt.Sequential)]
    [Deprecated("Deprecated. Use ClassCreationInfo2 instead.")]
    internal struct ClassCreationInfo
    {
        public GDExtensionBool IsVirtual;
        public GDExtensionBool IsAbstract;
        public unsafe delegate* <GDExtensionClassSet> SetFunc;
        public unsafe delegate* <GDExtensionClassGet> GetFunc;
        public unsafe delegate* <GDExtensionClassGetPropertyList> GetPropertyListFunc;
        public unsafe delegate* <GDExtensionClassFreePropertyList> FreePropertyListFunc;
        public unsafe delegate* <GDExtensionClassPropertyCanRevert> PropertyCanRevertFunc;
        public unsafe delegate* <GDExtensionClassPropertyGetRevert> PropertyGetRevertFunc;
        public unsafe delegate* <GDExtensionClassNotification> NotificationFunc;
        public unsafe delegate* <GDExtensionClassToString> ToStringFunc;
        public unsafe delegate* <GDExtensionClassReference> ReferenceFunc;
        public unsafe delegate* <GDExtensionClassUnreference> UnreferenceFunc;
        public unsafe delegate* <GDExtensionClassCreateInstance> CreateInstanceFunc; // (Default) constructor; mandatory. If the class is not instantiable, consider making it virtual or abstract.
        public unsafe delegate* <GDExtensionClassFreeInstance> FreeInstanceFunc; // Destructor; mandatory.
        public unsafe delegate* <GDExtensionClassGetVirtual> GetVirtualFunc; // Queries a virtual function by name and returns a callback to invoke the requested virtual function.
        public unsafe delegate* <GDExtensionClassGetRID> GetRidFunc;
        public unsafe void* ClassUserData; // Per-class user data, later accessible in instance bindings.
    }
}
