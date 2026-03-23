using System;
using UnityEngine;

public class PolygonMovement : MonoBehaviour
{
	[SerializeField]
	private Transform movementTarget;

	[SerializeField]
	private int sides = 4;

	[SerializeField]
	private float distanceMultiplier = 1f;

	[SerializeField]
	private float movementSpeedDegrees = 30f;

	private float currentAngleDegrees;

	private Vector3 direction;

	private void Update()
	{
		currentAngleDegrees += movementSpeedDegrees * Time.deltaTime;
		float num = currentAngleDegrees * (MathF.PI / 180f);
		direction.x = Mathf.Cos(num);
		direction.z = Mathf.Sin(num);
		if ((bool)movementTarget)
		{
			movementTarget.position = base.transform.TransformPoint(GetPositionInPolygon(num, direction.normalized));
		}
	}

	private Vector3 GetPositionInPolygon(float angleRad, Vector3 aimDirection)
	{
		float num = MathF.PI;
		float num2 = sides;
		float num3 = BMath.Cos(num / num2);
		float num4 = BMath.Cos(angleRad - 2f * num / num2 * BMath.Floor((num2 * angleRad + num) / (2f * num)));
		float num5 = num3 / num4;
		num5 *= distanceMultiplier;
		Vector3 vector = aimDirection;
		vector.y = 0f;
		return vector * num5;
	}
}
