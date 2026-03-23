#define DEBUG_DRAW
using UnityEngine;

public class BDebugBenchmark : MonoBehaviour
{
	public bool useGLDraw;

	public int numLines;

	private void Update()
	{
		for (int i = 0; i < numLines; i++)
		{
			Vector3 start = Random.onUnitSphere * 10f;
			Vector3 end = Random.onUnitSphere * 10f;
			Color color = new Color(Random.value, Random.value, Random.value, Random.value);
			if (useGLDraw)
			{
				BDebug.DrawLine(start, end, color);
			}
			else
			{
				Debug.DrawLine(start, end, color);
			}
		}
	}
}
