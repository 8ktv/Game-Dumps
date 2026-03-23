using UnityEngine;

namespace Mirror;

public interface PredictedState
{
	double timestamp { get; }

	Vector3 position { get; set; }

	Vector3 positionDelta { get; set; }

	Quaternion rotation { get; set; }

	Quaternion rotationDelta { get; set; }

	Vector3 velocity { get; set; }

	Vector3 velocityDelta { get; set; }

	Vector3 angularVelocity { get; set; }

	Vector3 angularVelocityDelta { get; set; }
}
