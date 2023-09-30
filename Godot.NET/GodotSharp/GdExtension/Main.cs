using System;

namespace Godot.GdExtension
{
    internal static class Main
    {
        internal static Interface i;
        internal static GDExtensionClassLibraryPtr lib;

        private static unsafe delegate* unmanaged[Cdecl] <byte*, nint> _getProcAddr;
        private static bool _warned;

        private unsafe delegate GDExtensionBool GodotEntryPoint(IntPtr p_get_proc_address,
            GDExtensionClassLibraryPtr p_library, Initialization* r_initialization);

        [System.Runtime.InteropServices.UnmanagedCallersOnly(EntryPoint = "godot_dotnet_initialize")]
        private static unsafe GDExtensionBool Load(IntPtr p_get_proc_address, 
            GDExtensionClassLibraryPtr p_library, Initialization* r_initialization)
        {
            Console.WriteLine("owo");
            if (p_get_proc_address == nint.Zero || p_library == nint.Zero || r_initialization == null)
                return 0;

            lib = p_library;
            _getProcAddr = (delegate* unmanaged[Cdecl]<byte*, nint>)p_get_proc_address;
            if (_getProcAddr == null || !i._Init(_getProcAddr))
                return 0;

            r_initialization->Initialize = &Initialize;
            r_initialization->Deinitialize = &Deinitialize;
            if (!_warned)
            {
                GD.PrintWarning(".NET: This API is still experimental! Please make backups of your projects before using.", "godot_dotnet_initialize", "GodotSharp/GdExtension/Main.cs", 27, true);
                _warned = true;
            }

            return 1;
        }

        [System.Runtime.InteropServices.UnmanagedCallersOnly(EntryPoint = "godot_dotnet_register")]
        public static unsafe void Initialize(void* userdata, InitializationLevel level)
        {
            if (_getProcAddr == null)
            {
                Console.WriteLine(".NET was not initialized. Continuing without initialization.");
                return;
            }

            try
            {
                switch (level)
                {
                    case InitializationLevel.Core:
                        break;

                    case InitializationLevel.Servers:
                        //ClassDB.PluginRegisterClass<CSharpLanguage>(true);
                        //ClassDB.PluginRegisterClass<CSharpScript>(true);
                        //ClassDB.PluginRegisterClass<CSharpScriptLoader>(true);
                        break;

                    case InitializationLevel.Scene:
                        ClassDB.PluginRegisterClass<TestNode>(true);
                        ClassDB.PluginRegisterClass<TestResource>(true);
                        //Engine.RegisterScriptLanguage(CSharpLanguage.Singleton);
                        break;
#if TOOLS
                    case InitializationLevel.Editor:
                        break;
#endif
                }
            } catch (Exception e)
            {
                Console.WriteLine("Something went wrong during initialization:");
                Console.WriteLine(e);
            }
        }

        [System.Runtime.InteropServices.UnmanagedCallersOnly(EntryPoint = "godot_dotnet_unregister")]
        public static unsafe void Deinitialize(void* userdata, InitializationLevel level)
        {
            if (_getProcAddr == null)
            {
                Console.WriteLine(".NET was not initialized. Quitting without denitialization.");
                return;
            }

            try
            {
                switch (level)
                {
                    case InitializationLevel.Core:
                        break;
                    case InitializationLevel.Servers:
                        //ClassDB.UnregisterClass<CSharpLanguage>();
                        //ClassDB.UnregisterClass<CSharpScript>();
                        //ClassDB.UnregisterClass<CSharpScriptLoader>();
                        break;
                    case InitializationLevel.Scene:
                        ClassDB.UnregisterClass<TestNode>();
                        ClassDB.UnregisterClass<TestResource>();
                        //Engine.UnregisterScriptLanguage(CSharpLanguage.Singleton);
                        break;
#if TOOLS
                    case InitializationLevel.Editor:
                        break;
#endif
                }
            } catch (Exception e)
            {
                Console.WriteLine("Something went wrong during denitialization:");
                Console.WriteLine(e);
            }
        }
    }
}
