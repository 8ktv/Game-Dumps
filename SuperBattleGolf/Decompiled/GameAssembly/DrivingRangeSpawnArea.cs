using UnityEngine;

public class DrivingRangeSpawnArea : MonoBehaviour
{
	[SerializeField]
	private Vector2 size;

	public Vector3 GetRandomSpawnPosition()
	{
		Vector3 position = new Vector3((0f - Random.value) * size.x / 2f, 0f, (0f - Random.value) * size.y / 2f);
		return base.transform.TransformPoint(position);
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.yellow;
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.DrawCube(Vector3.up * 0.0001f, size.AsHorizontal3());
	}
}
