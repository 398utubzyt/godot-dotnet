namespace Godot
{
    [SLayout(SLayoutOpt.Sequential)]
    public struct PhysicsServer2DExtensionShapeRestInfo
    {
        public Vector2 Point;
        public Vector2 Normal;
        public Rid Rid;
        public ObjectID ColliderId;
        public int Shape;
        public Vector2 LinearVelocity;
    }
}
