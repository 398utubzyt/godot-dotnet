namespace Godot
{
    public sealed partial class TestNode : Node2D
    {
        public override void _Ready()
        {
            GD.PrintWarning("haiiihaihiahihihhiahaihiahia owo", "TestNode._Ready", "TestNode.cs", 9, true);
        }

        public override void _Process(float delta)
        {
            Vector2 d = Vector2.Zero;
            if (Input.IsKeyPressed(Key.A))
                d.X -= 1.0f;
            if (Input.IsKeyPressed(Key.D))
                d.X += 1.0f;
            if (Input.IsKeyPressed(Key.W))
                d.Y -= 1.0f;
            if (Input.IsKeyPressed(Key.S))
                d.Y += 1.0f;
            Translate(d * delta * 60.0f);
        }

        private static bool _CanCall(StringName method)
        {
            return method == "_ready" || method == "_process";
        }

        protected override void _Call(StringName method, ref Variant arg0, ref Variant ret)
        {
            if (method == "_ready")
            {
                _Ready();
                return;
            }

            if (method == "_process")
            {
                _Process((float)arg0);
                return;
            }

            base._Call(method, ref arg0, ref ret);
        }

        public TestNode()
        {
        }
    }
}
