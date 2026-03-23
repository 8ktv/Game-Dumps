using UnityEngine;

public class SsprPlaneObject : MonoBehaviour
{
	private SsprPlane plane;

	private MaterialPropertyBlock props;

	private Renderer planeRenderer;

	private void Awake()
	{
		props = new MaterialPropertyBlock();
		planeRenderer = GetComponent<Renderer>();
		plane = ScreenSpacePlanarReflections.AddPlane(base.transform.position.y, planeRenderer.bounds);
	}

	private void Update()
	{
		props.SetInt("_SsprIndex", plane.index);
		planeRenderer.SetPropertyBlock(props);
	}
}
