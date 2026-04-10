using UnityEngine;

public class FreezeBombPlatformVfx : MonoBehaviour
{
	[SerializeField]
	private Shake shaker;

	private void OnEnable()
	{
		SetShaking(shaking: false);
	}

	public void SetShaking(bool shaking)
	{
		shaker.ShakeFactor = (shaking ? 1f : 0f);
	}
}
