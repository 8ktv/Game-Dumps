using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class SplineWallGenerator : MonoBehaviour
{
	[SerializeField]
	private float splineInterval = 0.5f;

	[SerializeField]
	private float wallHeight = 10f;

	private MeshRenderer generatedWall;

	private List<Vector3> wallPoints;

	private float previousSplineInterval = -1f;

	private float previousWallHeight = -1f;
}
