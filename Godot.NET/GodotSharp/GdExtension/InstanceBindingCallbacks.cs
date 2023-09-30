namespace Godot.GdExtension
{
    [SLayout(SLayoutOpt.Sequential)]
    internal struct InstanceBindingCallbacks
    {
        public unsafe delegate* unmanaged<void*, void*, void*> CreateCallback;
        public unsafe delegate* unmanaged<void*, void*, void*, void> FreeCallback;
        public unsafe delegate* unmanaged<void*, void*, byte, byte> ReferenceCallback;
    }
}
