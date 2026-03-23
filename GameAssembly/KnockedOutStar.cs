using UnityEngine;

public class KnockedOutStar : MonoBehaviour
{
	[SerializeField]
	private ParticleSystemRenderer particleRenderer;

	private bool currentColored = true;

	private MaterialPropertyBlock props;

	public void Initialize(bool highEnergy)
	{
		props = new MaterialPropertyBlock();
		props.SetFloat("_Energy", highEnergy ? 5f : 2f);
		particleRenderer.SetPropertyBlock(props);
		SetColored(colored: true, force: true);
	}

	public void SetColored(bool colored, bool force = false)
	{
		if (colored != currentColored || force)
		{
			currentColored = colored;
			props.SetFloat("_Grayscale", colored ? 0f : 1f);
			particleRenderer.SetPropertyBlock(props);
		}
	}

	public void Destroy()
	{
		props = null;
		Object.Destroy(base.gameObject);
	}
}
