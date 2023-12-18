using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;

namespace Godot.GdExtension
{
    internal class GodotAssemblyLoadContext : AssemblyLoadContext
    {
        public class NoBaseDirectoryForLoadContextException : Exception
        {
            public NoBaseDirectoryForLoadContextException() : base("Could not locate managed assembly directory") { }
        }

        public readonly string AssembliesDirectory;
        public readonly List<string> LoadedAssemblies = new List<string>(1);

        private readonly AssemblyDependencyResolver _resolver;

        public GodotAssemblyLoadContext(string projectDir, bool isEditor) : base(isEditor)
        {
            AssembliesDirectory = Path.Join(projectDir, "lib");
            if (string.IsNullOrEmpty(AppContext.BaseDirectory))
            {
                // Cannot continue if this (somehow) fails
                if (string.IsNullOrEmpty(AssembliesDirectory))
                    throw new NoBaseDirectoryForLoadContextException();

                AppDomain.CurrentDomain.SetData("APP_CONTEXT_BASE_DIRECTORY", AssembliesDirectory);
            }

            if (isEditor)
                Unloading += Cleanup;
        }

        private void Cleanup(AssemblyLoadContext obj)
        {
            LoadedAssemblies.Clear();
        }

        protected override Assembly Load(AssemblyName name)
        {
            if (name.Name == null)
                return null;

            string path = _resolver.ResolveAssemblyToPath(name);
            if (path == null)
                return null;

            using FileStream dll = File.Open(path, FileMode.Open, System.IO.FileAccess.Read, FileShare.Read);

            string symbolsPath = Path.ChangeExtension(path, ".pdb");
            if (!File.Exists(symbolsPath))
                return LoadFromStream(dll);

            // Load optional symbol file
            using FileStream pdb = File.Open(symbolsPath, FileMode.Open, System.IO.FileAccess.Read, FileShare.Read);
            return LoadFromStream(dll, pdb);
        }

        protected override nint LoadUnmanagedDll(string name)
        {
            string path = _resolver.ResolveUnmanagedDllToPath(name);
            return path != null ? LoadUnmanagedDllFromPath(path) : 0;
        }
    }
}
