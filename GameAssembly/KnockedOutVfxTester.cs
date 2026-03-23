using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

public class KnockedOutVfxTester : MonoBehaviour
{
	[SerializeField]
	private GameObject knockedOutVfxPrefab;

	[SerializeField]
	private GameObject knockedOutEndVfxPrefab;

	[SerializeField]
	private GameObject knockedOutProtectionVfxPrefab;

	[SerializeField]
	private GameObject knockedOutProtectionEndVfxPrefab;

	[SerializeField]
	private Transform knockedOutVfxContainer;

	[SerializeField]
	private Transform knockedOutProtectionVfxContainer;

	private KnockedOutVfx vfx;

	private void Start()
	{
		knockedOutVfxPrefab.SetActive(value: false);
		knockedOutEndVfxPrefab.SetActive(value: false);
	}

	private void Update()
	{
		if (Keyboard.current[Key.Q].wasPressedThisFrame)
		{
			KnockOut();
		}
		if (Keyboard.current[Key.W].wasPressedThisFrame)
		{
			Clear();
		}
	}

	private void KnockOut()
	{
		if (!(vfx != null))
		{
			GameObject gameObject = Object.Instantiate(knockedOutVfxPrefab, knockedOutVfxContainer);
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.SetActive(value: true);
			bool isPressed = Keyboard.current[Key.LeftShift].isPressed;
			vfx = gameObject.GetComponent<KnockedOutVfx>();
			int starCount = (isPressed ? 5 : 3);
			vfx.Initialize(starCount, isPressed);
			vfx.AsPoolable.Play();
			PlayingVfx(isPressed);
		}
	}

	private async void PlayingVfx(bool isLongKnockout)
	{
		int totalStarCount = (isLongKnockout ? 5 : 3);
		float duration = totalStarCount;
		float timer = 0f;
		while (timer < duration && !(vfx == null))
		{
			float num = timer / duration;
			int coloredStarCount = BMath.CeilToInt((1f - num) * (float)totalStarCount);
			vfx.SetColoredStarCount(coloredStarCount);
			timer += Time.deltaTime;
			await UniTask.WaitForEndOfFrame(this);
			if (this == null)
			{
				return;
			}
		}
		Clear();
		GameObject knockedOutProtectionVfxObj = Object.Instantiate(knockedOutProtectionVfxPrefab, knockedOutProtectionVfxContainer);
		knockedOutProtectionVfxObj.transform.localPosition = Vector3.zero;
		knockedOutProtectionVfxObj.SetActive(value: true);
		knockedOutProtectionVfxObj.GetComponent<ParticleSystem>().Play(withChildren: true);
		await UniTask.WaitForSeconds(1.5f);
		Object.Destroy(knockedOutProtectionVfxObj);
		GameObject obj = Object.Instantiate(knockedOutProtectionEndVfxPrefab, knockedOutProtectionVfxContainer);
		obj.transform.localPosition = Vector3.zero;
		obj.SetActive(value: true);
		obj.GetComponent<ParticleSystem>().Play(withChildren: true);
		Object.Destroy(obj, 2f);
	}

	private void Clear()
	{
		if (!(vfx == null))
		{
			vfx.AsPoolable.Stop(ParticleSystemStopBehavior.StopEmittingAndClear);
			Object.Destroy(vfx.gameObject);
			GameObject obj = Object.Instantiate(knockedOutEndVfxPrefab, knockedOutVfxContainer);
			obj.transform.localPosition = Vector3.zero;
			obj.SetActive(value: true);
			obj.GetComponent<ParticleSystem>().Play(withChildren: true);
			Object.Destroy(obj, 2f);
		}
	}
}
