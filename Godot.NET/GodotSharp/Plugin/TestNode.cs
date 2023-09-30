namespace Godot
{
    public sealed partial class TestNode : Node
    {
        public override void _Ready()
        {
            GD.PrintWarning("haiiihaihiahihihhiahaihiahia owo", "TestNode._Ready", "TestNode.cs", 9, true);
        }

        private static bool _CanCall(StringName method)
        {
            return method == "_ready";
        }

        protected override void _Call(StringName method, ref Variant arg0, ref Variant ret)
        {
            if (method == "_ready")
            {
                _Ready();
                return;
            }

            base._Call(method, ref arg0, ref ret);
        }

        public TestNode()
        {
        }
    }
}
