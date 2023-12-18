namespace Godot
{
    public sealed partial class CSharpScriptLoader : ResourceFormatLoader
    {
        public override bool _HandlesType(StringName type)
            => type == "Script" || type == nameof(CSharpScript);

        public override string[] _GetRecognizedExtensions()
            => ["cs"];

        public override string _GetResourceType(string path)
            => path.ToLower().EndsWith("cs") ? nameof(CSharpScript) : string.Empty;

        public override Variant _Load(string path, string originalPath, bool useSubThreads, int cacheMode)
        {
            CSharpScript scr = new CSharpScript();

            scr.SetSourceCode(FileAccess.Open(path, FileAccess.ModeFlags.Read).GetAsText(true));

            scr.SetPath(originalPath);
            if (scr.Reload(false) != Error.Ok)
                return Variant.Null;

            return scr;
        }

        private static bool _CanCall(StringName method)
        {
            return method == "_handles_type" ||
                method == "_get_recognized_extensions" ||
                method == "_get_resource_type" ||
                method == "_load";
        }

        protected override void _Call(StringName method, ref nint args, nint ret)
        {
            if (method == "_handles_type")
            {
                MemUtil.IntAsRef<byte>(ret) = _HandlesType(MemUtil.IntAsRef<StringName>(args)).ToExtBool();
                return;
            }

            if (method == "_get_recognized_extensions")
            {
                // TODO: Return array here??
                return;
            }

            if (method == "_get_resource_type")
            {
                StringDB.Placement(
                    _GetResourceType(StringDB.SearchOrCreate(MemUtil.IntAsRef<nint>(args))), 
                    ref MemUtil.IntAsRef<nint>(ret));
                return;
            }

            if (method == "_load")
            {
                MemUtil.IntAsRef<Variant>(ret) = _Load(
                    StringDB.SearchOrCreate(MemUtil.IntAsRef<nint>(args)),
                    StringDB.SearchOrCreate(MemUtil.AddRef(ref MemUtil.IntAsRef<nint>(args), 1)),
                    MemUtil.IntAsRef<byte>(MemUtil.AddRef(ref MemUtil.IntAsRef<nint>(args), 2)).ToBool(),
                    MemUtil.IntAsRef<int>(MemUtil.AddRef(ref MemUtil.IntAsRef<nint>(args), 3)));
                return;
            }
        }
    }
}
