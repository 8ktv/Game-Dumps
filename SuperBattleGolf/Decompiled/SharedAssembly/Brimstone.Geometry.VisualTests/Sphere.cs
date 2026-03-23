using UnityEngine;

namespace Brimstone.Geometry.VisualTests;

public class Sphere : MonoBehaviour
{
	public float radius = 1f;

	private void OnValidate()
	{
		base.transform.localScale = radius * 2f * Vector3.one;
	}
}
