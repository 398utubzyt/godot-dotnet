namespace Godot
{
    [SLayout(SLayoutOpt.Sequential)]
    public struct PhysicsServer3DExtensionRayResult
    {
        public Vector3 Position;
        public Vector3 Normal;
        public Rid Rid;
        public ObjectID ColliderId;
        public nint Collider;
        public int Shape;
    }
}
