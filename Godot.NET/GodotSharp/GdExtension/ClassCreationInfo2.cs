namespace Godot.GdExtension
{
    [SLayout(SLayoutOpt.Sequential)]
    internal struct ClassCreationInfo2
    {
        public GDExtensionBool IsVirtual;
        public GDExtensionBool IsAbstract;
        public GDExtensionBool IsExposed;
        public unsafe delegate* unmanaged<nint, nint, nint, byte> SetFunc;
        public unsafe delegate* unmanaged<nint, nint, nint, byte> GetFunc;
        public unsafe delegate* unmanaged<nint, uint*, PropertyInfo*> GetPropertyListFunc;
        public unsafe delegate* unmanaged<nint, PropertyInfo*, void> FreePropertyListFunc;
        public unsafe delegate* unmanaged<nint, nint, byte> PropertyCanRevertFunc;
        public unsafe delegate* unmanaged<nint, nint, nint, byte> PropertyGetRevertFunc;
        public unsafe delegate* unmanaged<nint, PropertyInfo*, byte> ValidatePropertyFunc;
        public unsafe delegate* unmanaged<nint, int, byte, void> NotificationFunc;
        public unsafe delegate* unmanaged<nint, byte*, nint, void> ToStringFunc;
        public unsafe delegate* unmanaged<nint, void> ReferenceFunc;
        public unsafe delegate* unmanaged<nint, void> UnreferenceFunc;
        public unsafe delegate* unmanaged<void*, nint> CreateInstanceFunc; // (Default) constructor; mandatory. If the class is not instantiable, consider making it virtual or abstract.
        public unsafe delegate* unmanaged<void*, nint, void> FreeInstanceFunc; // Destructor; mandatory.
        public unsafe delegate* unmanaged<void*, nint, nint> RecreateInstanceFunc;
        // Queries a virtual function by name and returns a callback to invoke the requested virtual function.
        public unsafe delegate* unmanaged<void*, nint, delegate* unmanaged<nint, nint*, nint>> GetVirtualFunc;
        // Paired with `call_virtual_with_data_func`, this is an alternative to `get_virtual_func` for extensions that
        // need or benefit from extra data when calling virtual functions.
        // Returns user data that will be passed to `call_virtual_with_data_func`.
        // Returning `NULL` from this function signals to Godot that the virtual function is not overridden.
        // Data returned from this function should be managed by the extension and must be valid until the extension is deinitialized.
        // You should supply either `get_virtual_func`, or `get_virtual_call_data_func` with `call_virtual_with_data_func`.
        public unsafe delegate* unmanaged<void*, nint, void*> GetVirtualCallDataFunc;
        // Used to call virtual functions when `get_virtual_call_data_func` is not null.
        public unsafe delegate* unmanaged<nint, nint, void*, nint*, nint, void> CallVirtualWithDataFunc;
        public unsafe delegate* unmanaged<nint, ulong> GetRidFunc;
        public unsafe void* ClassUserData; // Per-class user data, later accessible in instance bindings.
    }
}
