using UnityEngine;

public class AirhornActivationVfx : MonoBehaviour, ILateBUpdateCallback, IAnyBUpdateCallback
{
	[SerializeField]
	private float scale;

	private void OnEnable()
	{
		BUpdate.RegisterCallback(this);
		base.transform.localScale = scale * Vector3.one;
	}

	private void OnDisable()
	{
		BUpdate.DeregisterCallback(this);
	}

	public void OnLateBUpdate()
	{
		base.transform.rotation = Quaternion.identity;
	}
}
