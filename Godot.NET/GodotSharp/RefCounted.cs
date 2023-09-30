namespace Godot
{
    public partial class RefCounted
    {
        [MImpl(MImplOpts.AggressiveInlining)]
        internal void __gdext_Reference()
            => _Reference();
        [MImpl(MImplOpts.AggressiveInlining)]
        internal void __gdext_Unreference()
            => _Unreference();

        protected virtual void _Reference()
        {
        }
        protected virtual void _Unreference()
        {
        }
    }
}
