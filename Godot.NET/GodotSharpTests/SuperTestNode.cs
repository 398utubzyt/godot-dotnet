using Godot;

namespace GodotSharpTests
{
    public partial class SuperTestNode : Node2D
    {
        public override void _Ready()
        {
        }

        public override void _Process(float delta)
        {
            Translate(Vector2.One * delta * 60.0f);
        }
    }
}
