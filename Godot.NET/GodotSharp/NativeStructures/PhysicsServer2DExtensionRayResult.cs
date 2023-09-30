namespace Godot
{
    [SLayout(SLayoutOpt.Sequential)]
    public struct PhysicsServer2DExtensionRayResult
    {
        public Vector2 Position;
        public Vector2 Normal;
        public Rid Rid;
        public ObjectID ColliderId;
        public nint Collider;
        public int Shape;
    }
}
