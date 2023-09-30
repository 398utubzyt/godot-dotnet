namespace Godot
{
    [SLayout(SLayoutOpt.Sequential)]
    public struct ScriptLanguageExtensionProfilingInfo
    {
        public StringName Signature;
        public ulong call_count;
        public ulong total_time;
        public ulong self_time;
    }
}
