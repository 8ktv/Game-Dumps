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
	private GameObject knockedOutProtectionBlockedVfxPrefab;

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
		if (Keyboard.current[Key.E].wasPressedThisFrame)
		{
			Shield();
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

	private void Shield()
	{
		_ = Keyboard.current[Key.Digit1].isPressed;
		bool isPressed = Keyboard.current[Key.Digit2].isPressed;
		bool isPressed2 = Keyboard.current[Key.Digit3].isPressed;
		PlayingShield(isPressed2 ? KnockOutVfxColor.Red : (isPressed ? KnockOutVfxColor.Orange : KnockOutVfxColor.Blue));
	}

	private async void PlayingShield(KnockOutVfxColor knockOutColor)
	{
		GameObject knockedOutProtectionVfxObj = Object.Instantiate(knockedOutProtectionVfxPrefab, knockedOutProtectionVfxContainer);
		knockedOutProtectionVfxObj.transform.localPosition = Vector3.zero;
		knockedOutProtectionVfxObj.SetActive(value: true);
		if (knockedOutProtectionVfxObj.TryGetComponent<KnockOutVfxVisuals>(out var component))
		{
			component.SetColor(knockOutColor);
		}
		knockedOutProtectionVfxObj.GetComponent<ParticleSystem>().Play(withChildren: true);
		await UniTask.WaitForSeconds(1.5f);
		GameObject obj = Object.Instantiate(knockedOutProtectionBlockedVfxPrefab, knockedOutProtectionVfxContainer);
		obj.transform.localPosition = Vector3.zero;
		obj.SetActive(value: true);
		if (obj.TryGetComponent<KnockOutVfxVisuals>(out var component2))
		{
			component2.SetColor(knockOutColor);
		}
		obj.GetComponent<ParticleSystem>().Play(withChildren: true);
		await UniTask.WaitForSeconds(1.5f);
		Object.Destroy(knockedOutProtectionVfxObj);
		GameObject obj2 = Object.Instantiate(knockedOutProtectionEndVfxPrefab, knockedOutProtectionVfxContainer);
		obj2.transform.localPosition = Vector3.zero;
		obj2.SetActive(value: true);
		if (obj2.TryGetComponent<KnockOutVfxVisuals>(out var component3))
		{
			component3.SetColor(knockOutColor);
		}
		obj2.GetComponent<ParticleSystem>().Play(withChildren: true);
		Object.Destroy(obj2, 2f);
	}
}
