using UnityEngine;
using UnityEngine.InputSystem;

public class ShotgunVfxTester : MonoBehaviour
{
	[SerializeField]
	private Animator shotgunAnimator;

	[SerializeField]
	private GameObject muzzlePrefab;

	[SerializeField]
	private GameObject tracerPrefab;

	[SerializeField]
	private GameObject impactPrefab;

	[SerializeField]
	private Transform muzzlePivot;

	[SerializeField]
	private Transform impactPivot;

	private void Start()
	{
		muzzlePrefab.SetActive(value: false);
		tracerPrefab.SetActive(value: false);
		impactPrefab.SetActive(value: false);
	}

	private void Update()
	{
		if (Keyboard.current[Key.Q].wasPressedThisFrame)
		{
			Shoot();
		}
	}

	private void Shoot()
	{
		shotgunAnimator.SetTrigger("shoot");
		GameObject obj = Object.Instantiate(muzzlePrefab, muzzlePivot);
		obj.transform.localPosition = Vector3.zero;
		obj.transform.localRotation = Quaternion.identity;
		obj.SetActive(value: true);
		Object.Destroy(obj, 2f);
		if (obj.TryGetComponent<ParticleSystem>(out var component))
		{
			component.Play(withChildren: true);
		}
		int num = 4;
		for (int i = 0; i < num; i++)
		{
			GameObject obj2 = Object.Instantiate(tracerPrefab, muzzlePivot);
			obj2.transform.localPosition = Vector3.zero;
			obj2.transform.localRotation = Quaternion.Euler(Random.Range(-8f, 8f), Random.Range(-8f, 8f), 0f);
			obj2.SetActive(value: true);
			Object.Destroy(obj2, 2f);
			if (obj2.TryGetComponent<ParticleSystem>(out var component2))
			{
				component2.Play(withChildren: true);
			}
		}
		GameObject obj3 = Object.Instantiate(impactPrefab, impactPivot);
		obj3.transform.localPosition = Vector3.zero;
		obj3.transform.localRotation = Quaternion.identity;
		obj3.SetActive(value: true);
		Object.Destroy(obj3, 2f);
		if (obj3.TryGetComponent<ParticleSystem>(out var component3))
		{
			component3.Play(withChildren: true);
		}
	}
}
