namespace Godot
{
    [SLayout(SLayoutOpt.Sequential)]
    public struct PhysicsServer3DExtensionMotionResult
    {
        public Vector3 Travel;
        public Vector3 Remainder;
        public Vector3 CollisionPoint;
        public Vector3 CollisionNormal;
        public Vector3 ColliderVelocity;
        public Real CollisionDepth;
        public Real CollisionSafeFraction;
        public Real CollisionUnsafeFraction;
        public int CollisionLocalShape;
        public ObjectID ColliderId;
        public Rid Collider;
        public int ColliderShape;
    }
}
