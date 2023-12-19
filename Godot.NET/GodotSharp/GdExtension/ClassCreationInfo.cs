namespace Godot.GdExtension
{
    [SLayout(SLayoutOpt.Sequential)]
    [Deprecated("Deprecated. Use ClassCreationInfo2 instead.")]
    internal struct ClassCreationInfo
    {
        public GDExtensionBool IsVirtual;
        public GDExtensionBool IsAbstract;
        public unsafe delegate* unmanaged<nint, nint, nint, byte> SetFunc;
        public unsafe delegate* unmanaged<nint, nint, nint, byte> GetFunc;
        public unsafe delegate* unmanaged<nint, uint*, PropertyInfo*> GetPropertyListFunc;
        public unsafe delegate* unmanaged<nint, PropertyInfo*, void> FreePropertyListFunc;
        public unsafe delegate* unmanaged<nint, nint, byte> PropertyCanRevertFunc;
        public unsafe delegate* unmanaged<nint, nint, nint, byte> PropertyGetRevertFunc;
        public unsafe delegate* unmanaged<nint, int, void> NotificationFunc;
        public unsafe delegate* unmanaged<nint, byte*, nint, void> ToStringFunc;
        public unsafe delegate* unmanaged<nint, void> ReferenceFunc;
        public unsafe delegate* unmanaged<nint, void> UnreferenceFunc;
        public unsafe delegate* unmanaged<void*, nint> CreateInstanceFunc; // (Default) constructor; mandatory. If the class is not instantiable, consider making it virtual or abstract.
        public unsafe delegate* unmanaged<void*, nint, void> FreeInstanceFunc; // Destructor; mandatory.
        public unsafe delegate* unmanaged<void*, nint, delegate* unmanaged<nint, nint*, nint>> GetVirtualFunc;
        public unsafe delegate* unmanaged<nint, ulong> GetRidFunc;
        public unsafe void* ClassUserData; // Per-class user data, later accessible in instance bindings.
    }
}
