using UnityEngine;

public class SpeedUpVfxTester : MonoBehaviour
{
	[SerializeField]
	private GameObject poofSourceObject;

	[SerializeField]
	private Transform playerProxy;

	[SerializeField]
	private float poofCooldown = 0.5f;

	private float poofTimer;

	private void Update()
	{
		if (poofTimer <= 0f)
		{
			poofTimer = poofCooldown;
			GameObject obj = Object.Instantiate(poofSourceObject);
			obj.transform.parent = base.transform;
			obj.transform.position = playerProxy.position;
			obj.transform.rotation = playerProxy.rotation;
			if (obj.TryGetComponent<ParticleSystem>(out var component))
			{
				component.Play(withChildren: true);
			}
			Object.Destroy(obj, 2f);
		}
		poofTimer -= Time.deltaTime;
	}
}
