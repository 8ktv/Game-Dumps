using Brimstone.Geometry;
using UnityEngine;

public static class CameraExtensions
{
	public static Vector3 WorldToViewportPoint(this Camera camera, Vector3 position, Quaternion rotation, Vector3 worldPoint)
	{
		Matrix4x4 projectionMatrix = camera.projectionMatrix;
		Matrix4x4 inverse = Matrix4x4.TRS(position, rotation, Vector3.one).inverse;
		Matrix4x4 matrix4x = projectionMatrix * inverse;
		Vector4 vector = new Vector4(worldPoint.x, worldPoint.y, worldPoint.z, 1f);
		Vector4 vector2 = matrix4x * vector;
		Vector3 result = vector2;
		result /= 0f - vector2.w;
		result.x = result.x / 2f + 0.5f;
		result.y = result.y / 2f + 0.5f;
		result.z = 0f - vector2.w;
		return result;
	}

	public static Vector3 ViewportToWorldPoint(this Camera camera, Vector3 position, Quaternion rotation, Vector3 viewportPoint)
	{
		Matrix4x4 projectionMatrix = camera.projectionMatrix;
		Matrix4x4 inverse = Matrix4x4.TRS(position, rotation, Vector3.one).inverse;
		Matrix4x4 matrix4x = projectionMatrix * inverse;
		Vector4 vector = projectionMatrix * new Vector4(0f, 0f, viewportPoint.z, 1f);
		Vector4 vector2 = new Vector4(1f - viewportPoint.x * 2f, 1f - viewportPoint.y * 2f, vector.z / vector.w, 1f);
		Vector4 vector3 = matrix4x.inverse * vector2;
		return vector3 / vector3.w;
	}

	public static float GetFrustumDistanceFromHeight(this Camera camera, float height)
	{
		return BGeo.GetFrustumDistanceFromHeight(camera.fieldOfView, height);
	}

	public static float GetFrustumDistanceFromWidth(this Camera camera, float width)
	{
		return BGeo.GetFrustumDistanceFromWidth(camera.fieldOfView, camera.aspect, width);
	}

	public static float GetFrustumHeightFromDistance(this Camera camera, float distance)
	{
		return BGeo.GetFrustumHeightFromDistance(camera.fieldOfView, distance);
	}

	public static float GetFrustumWidthFromDistance(this Camera camera, float distance)
	{
		return BGeo.GetFrustumWidthFromDistance(camera.fieldOfView, camera.aspect, distance);
	}

	public static float GetFrustumWidthFromHeight(this Camera camera, float height)
	{
		return BGeo.GetFrustumWidthFromHeight(camera.aspect, height);
	}

	public static float GetFrustumHeightFromWidth(this Camera camera, float width)
	{
		return BGeo.GetFrustumHeightFromWidth(camera.aspect, width);
	}
}
