namespace Godot.GdExtension
{
    internal enum ClassMethodFlags
    {
        Normal = 1,
        Editor = 2,
        Const = 4,
        Virtual = 8,
        VarArg = 16,
        Static = 32,
        Default = Normal,
    }
}
