using UnityEngine;

namespace Brimstone.Geometry.VisualTests;

public class FiniteCylinder : MonoBehaviour
{
	public float radius = 1f;

	public float length = 1f;

	private void OnValidate()
	{
		float num = radius * 2f;
		base.transform.localScale = new Vector3(num, length * 0.5f, num);
	}
}
