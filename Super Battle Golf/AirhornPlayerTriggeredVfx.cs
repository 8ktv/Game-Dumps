using UnityEngine;

public class AirhornPlayerTriggeredVfx : MonoBehaviour
{
	[SerializeField]
	private PoolableParticleSystem asPoolableParticleSystem;

	[SerializeField]
	private GameObject itemTriggeredParticle;

	public PoolableParticleSystem AsPoolableParticleSystem => asPoolableParticleSystem;

	public void SetItemTriggered(bool triggered)
	{
		itemTriggeredParticle.SetActive(triggered);
	}
}
