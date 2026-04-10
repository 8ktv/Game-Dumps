using FMODUnity;
using UnityEngine;

public class MineVfx : MonoBehaviour, IBUpdateCallback, IAnyBUpdateCallback
{
	[SerializeField]
	private MeshRenderer meshRenderer;

	[SerializeField]
	private Animator animator;

	[SerializeField]
	[ColorUsage(false, true)]
	private Color unlitColor;

	[SerializeField]
	[ColorUsage(false, true)]
	private Color litColor;

	[SerializeField]
	[Range(0f, 1f)]
	private float emission;

	private MaterialPropertyBlock props;

	private bool isArmed;

	private double armedBlinkTimestamp = double.MinValue;

	private float previousEmission = -1f;

	private static readonly int armHash = Animator.StringToHash("arm");

	private static readonly int blinkHash = Animator.StringToHash("blink");

	private void OnEnable()
	{
		BUpdate.RegisterCallback(this);
		if (props == null)
		{
			props = new MaterialPropertyBlock();
		}
		UpdateEmissionColor();
	}

	private void OnDisable()
	{
		BUpdate.DeregisterCallback(this);
	}

	public void OnBUpdate()
	{
		if (isArmed && BMath.GetTimeSince(armedBlinkTimestamp) >= GameManager.ItemSettings.LandmineArmedBlinkPeriod)
		{
			PlayArmedBlink(soundOnly: false);
		}
		if (previousEmission != emission)
		{
			UpdateEmissionColor();
		}
		previousEmission = emission;
	}

	private void UpdateEmissionColor()
	{
		props.SetColor("_Emissive_Color", Color.Lerp(unlitColor, litColor, emission));
		meshRenderer.SetPropertyBlock(props);
	}

	public void Arm(bool skipArmingEffects)
	{
		isArmed = true;
		animator.SetTrigger(armHash);
		if (!skipArmingEffects)
		{
			PlayArmedBlink(soundOnly: true);
			if (VfxPersistentData.TryGetPooledVfx(VfxType.MineArmedStart, out var particleSystem))
			{
				particleSystem.transform.SetParent(base.transform);
				particleSystem.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
				particleSystem.Play();
			}
			RuntimeManager.PlayOneShotAttached(GameManager.AudioSettings.LandmineArmEvent, base.gameObject);
		}
	}

	public void Unarm()
	{
		isArmed = false;
	}

	private void PlayArmedBlink(bool soundOnly)
	{
		if (!soundOnly)
		{
			animator.SetTrigger(blinkHash);
		}
		RuntimeManager.PlayOneShotAttached(GameManager.AudioSettings.LandmineArmedBeepEvent, base.gameObject);
		armedBlinkTimestamp = Time.timeAsDouble;
	}
}
