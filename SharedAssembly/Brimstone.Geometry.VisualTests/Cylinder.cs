using UnityEngine;

namespace Brimstone.Geometry.VisualTests;

public class Cylinder : MonoBehaviour
{
	public float radius = 1f;

	private void OnValidate()
	{
		float num = radius * 2f;
		base.transform.localScale = new Vector3(num, 10000f, num);
	}
}
