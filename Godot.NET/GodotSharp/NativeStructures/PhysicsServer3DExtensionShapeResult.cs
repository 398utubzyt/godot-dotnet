namespace Godot
{
    [SLayout(SLayoutOpt.Sequential)]
    public struct PhysicsServer3DExtensionShapeResult
    {
        public Rid Rid;
        public ObjectID ColliderId;
        public nint Collider;
        public int Shape;
    }
}
