namespace UnityEngine.LowLevelPhysics2D;

internal interface IPhysicsJoint
{
	bool isValid { get; }

	PhysicsWorld world { get; }

	PhysicsJoint.JointType jointType { get; }

	PhysicsBody bodyA { get; }

	PhysicsBody bodyB { get; }

	PhysicsTransform localAnchorA { get; set; }

	PhysicsTransform localAnchorB { get; set; }

	float forceThreshold { get; set; }

	float torqueThreshold { get; set; }

	bool collideConnected { get; set; }

	float tuningFrequency { get; set; }

	float tuningDamping { get; set; }

	float drawScale { get; set; }

	Vector2 currentConstraintForce { get; }

	float currentConstraintTorque { get; }

	float currentLinearSeparationError { get; }

	float currentAngularSeparationError { get; }

	bool isOwned { get; }

	object callbackTarget { get; set; }

	PhysicsUserData userData { get; set; }

	bool Destroy(int ownerKey = 0);

	void WakeBodies();

	int SetOwner(Object owner);

	Object GetOwner();

	void Draw();
}
