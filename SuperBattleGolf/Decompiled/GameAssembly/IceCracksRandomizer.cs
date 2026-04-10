using UnityEngine;

public class IceCracksRandomizer : MonoBehaviour
{
	[SerializeField]
	private MeshRenderer iceRenderer;

	private MaterialPropertyBlock props;

	private int triplanarOffsetId = Shader.PropertyToID("_Triplanar_Offset");

	private void OnEnable()
	{
		if (props == null)
		{
			props = new MaterialPropertyBlock();
		}
		props.SetVector(triplanarOffsetId, new Vector4(Random.Range(0f, 5f), Random.Range(0f, 5f), Random.Range(0f, 5f), 0f));
		iceRenderer.SetPropertyBlock(props);
	}
}
