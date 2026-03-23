using UnityEngine;

[ExecuteAlways]
public class Shake : MonoBehaviour
{
	[SerializeField]
	private CameraShakeType shakeType;

	[SerializeField]
	private Transform shakeTarget;

	[SerializeField]
	private float shakeIntensity = 0.125f;

	[SerializeField]
	private float shakeRotationIntensity = 1f;

	[SerializeField]
	private float shakeTimer;

	[SerializeField]
	private float shakeFactor;

	private float frequencyTimer;

	public float ShakeFactor
	{
		get
		{
			return shakeFactor;
		}
		set
		{
			shakeFactor = value;
		}
	}

	private void Start()
	{
		shakeTarget.localPosition = Vector3.zero;
		shakeTarget.localRotation = Quaternion.identity;
	}

	private void Update()
	{
		if (shakeTimer > 0f && frequencyTimer > 0f)
		{
			frequencyTimer -= Time.deltaTime;
			return;
		}
		frequencyTimer = shakeTimer;
		if (shakeType.HasFlag(CameraShakeType.POSITION))
		{
			Vector3 localPosition = Random.insideUnitCircle * shakeIntensity * shakeFactor;
			shakeTarget.localPosition = localPosition;
		}
		if (shakeType.HasFlag(CameraShakeType.ROTATION))
		{
			Vector3 euler = new Vector3(Random.Range(0f - shakeRotationIntensity, shakeRotationIntensity) * shakeFactor, Random.Range(0f - shakeRotationIntensity, shakeRotationIntensity) * shakeFactor, 0f);
			shakeTarget.localRotation = Quaternion.Euler(euler);
		}
	}
}
