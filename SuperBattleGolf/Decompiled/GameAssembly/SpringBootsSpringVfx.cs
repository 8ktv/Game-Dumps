using UnityEngine;

[ExecuteAlways]
public class SpringBootsSpringVfx : MonoBehaviour, IBUpdateCallback, IAnyBUpdateCallback
{
	[SerializeField]
	private MeshRenderer[] springs;

	[SerializeField]
	private Transform bottomPlatform;

	[SerializeField]
	private Vector2 bottomPlatformYRange;

	[SerializeField]
	[Range(0f, 1f)]
	private float length;

	private MaterialPropertyBlock propertyBlock;

	private float previousLength = -1f;

	private void OnEnable()
	{
		BUpdate.RegisterCallback(this);
		if (propertyBlock == null)
		{
			propertyBlock = new MaterialPropertyBlock();
		}
	}

	private void OnDisable()
	{
		BUpdate.DeregisterCallback(this);
	}

	public void OnBUpdate()
	{
		UpdateLength();
	}

	private void UpdateLength()
	{
		if (previousLength != length)
		{
			previousLength = length;
			propertyBlock.SetFloat("_VAT_Interpolation", length);
			for (int i = 0; i < springs.Length; i++)
			{
				springs[i].SetPropertyBlock(propertyBlock);
			}
			bottomPlatform.localPosition = Vector3.Lerp(Vector3.up * bottomPlatformYRange.x, Vector3.up * bottomPlatformYRange.y, length);
		}
	}
}
