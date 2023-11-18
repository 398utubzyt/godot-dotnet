using System;

namespace Godot
{
    public sealed partial class TestNode : Node2D
    {
        public float Speed = 90.0f;

        public override void _Ready()
        {
            GD.PrintWarning("haiiihaihiahihihhiahaihiahia owo", "TestNode._Ready", "TestNode.cs", 9, true);
        }

        public override void _Process(double delta)
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
            Translate(d * ((float)delta * (Speed * (Input.IsKeyPressed(Key.Shift) ? 1.5f : 1.0f))));
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

        protected override nuint _PropertyCount()
        {
            return 1;
        }
        protected override void _GetPropertyList(Span<PropertyInfo> info)
        {
            info[0] = new PropertyInfo { Name = nameof(Speed), Type = VariantType.Float };
        }
        protected override bool _PropertyCanRevert(StringName property)
        {
            return property == nameof(Speed);
        }
        protected override bool _PropertyGetRevert(StringName property, ref Variant ret)
        {
            if (property == nameof(Speed))
            {
                ret = 90.0f;
                return true;
            }

            return base._PropertyGetRevert(property, ref ret);
        }
        protected override bool _Get(StringName property, ref Variant value)
        {
            if (property == nameof(Speed))
            {
                value = Speed;
                return true;
            }

            return base._Get(property, ref value);
        }
        protected override bool _Set(StringName property, Variant value)
        {
            if (property == nameof(Speed))
            {
                Speed = (float)value;
                return true;
            }

            return base._Set(property, value);
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
                _Process((double)RefHelper.IntAsRef<double>(args));
                return;
            }

            base._Call(method, ref args, ret);
        }

        public TestNode()
        {
        }
    }
}
