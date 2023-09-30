using System.Runtime.InteropServices;

namespace Godot.GdExtension
{
    [SLayout(SLayoutOpt.Sequential)]
    internal struct Initialization
    {
        /* Minimum initialization level required.
	     * If Core or Servers, the extension needs editor or game restart to take effect */
        public InitializationLevel MinimumInitialization;
        /* Up to the user to supply when initializing */
        public unsafe void* UserData;
        /* This function will be called multiple times for each initialization level. */
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate void InitializeFunc(void* userdata, InitializationLevel p_level);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate void DeinitializeFunc(void* userdata, InitializationLevel p_level);

        public unsafe delegate* unmanaged <void*, InitializationLevel, void> Initialize;
        public unsafe delegate* unmanaged <void*, InitializationLevel, void> Deinitialize;
    }
}
