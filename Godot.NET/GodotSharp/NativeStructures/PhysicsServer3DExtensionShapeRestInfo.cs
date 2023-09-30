namespace Godot
{
    [SLayout(SLayoutOpt.Sequential)]
    public struct PhysicsServer3DExtensionShapeRestInfo
    {
        public Vector3 Point;
        public Vector3 Normal;
        public Rid Rid;
        public ObjectID ColliderId;
        public int Shape;
        public Vector3 LinearVelocity;
    }
}
