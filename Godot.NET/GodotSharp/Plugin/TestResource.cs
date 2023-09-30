namespace Godot
{
    public sealed partial class TestResource : Resource
    {
        private static bool _CanCall(StringName method)
        {
            return false;
        }
    }
}
