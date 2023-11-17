using System;

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
            float speed = Input.IsKeyPressed(Key.Shift) ? 120.0f : 90.0f;
            Translate(d * (delta * speed));
        }

        public override bool _Notification(int what, bool reversed)
        {
#if TOOLS
            if (Engine.IsEditorHint())
                return false;
#endif
            switch (what)
            {
                case 13:
                    // Ready
                    return true;
                case 17:
                    // Process
                    return true;
            }

            return base._Notification(what, reversed);
        }

        private static bool _CanCall(StringName method)
        {
            // Emulate script behavior -- do not run virtual calls in editor.
            return
#if TOOLS
                !Engine.IsEditorHint() && 
#endif
                (method == "_ready" || method == "_process");
        }

        protected override unsafe void _Call(StringName method, ref nint args, nint ret)
        {
            // Since source generators are not running on classes in GodotSharp,
            // the source of `_Call` should match as close as possible to
            // the content of what a generator would produce.

            if (method == "_ready")
            {
                _Ready();
                return;
            }

            if (method == "_process")
            {
                _Process((float)RefHelper.IntAsRef<double>(args));
                return;
            }

            base._Call(method, ref args, ret);
        }

        public TestNode()
        {
        }
    }
}
