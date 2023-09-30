namespace Godot
{
    [SLayout(SLayoutOpt.Sequential)]
    public struct PhysicsServer2DExtensionShapeResult
    {
        public Rid Rid;
        public ObjectID ColliderId;
        public nint Collider;
        public int Shape;
    }
}
