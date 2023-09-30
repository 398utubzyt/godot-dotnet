namespace Godot
{
    [SLayout(SLayoutOpt.Sequential)]
    public struct PhysicsServer2DExtensionMotionResult
    {
        public Vector2 Travel;
        public Vector2 Remainder;
        public Vector2 CollisionPoint;
        public Vector2 CollisionNormal;
        public Vector2 ColliderVelocity;
        public Real CollisionDepth;
        public Real CollisionSafeFraction;
        public Real CollisionUnsafeFraction;
        public int CollisionLocalShape;
        public ObjectID ColliderId;
        public Rid Collider;
        public int ColliderShape;
    }
}
