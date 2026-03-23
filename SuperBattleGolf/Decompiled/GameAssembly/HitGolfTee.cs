using UnityEngine;

public class HitGolfTee : MonoBehaviour, IBUpdateCallback, IAnyBUpdateCallback
{
	[SerializeField]
	private Rigidbody rigidbody;

	private double hitTimestamp;

	private void Awake()
	{
		rigidbody.maxAngularVelocity = BMath.Max(rigidbody.maxAngularVelocity, GameManager.GolfSettings.TeeMaxSwingAngularSpeed);
	}

	private void OnEnable()
	{
		BUpdate.RegisterCallback(this);
	}

	private void OnDisable()
	{
		BUpdate.DeregisterCallback(this);
	}

	private void OnDestroy()
	{
		BUpdate.DeregisterCallback(this);
	}

	public void Initialize(Vector3 position, Quaternion rotation, Vector3 hitDirection, float hitPower)
	{
		base.transform.SetPositionAndRotation(position, rotation);
		base.transform.localScale = Vector3.one;
		rigidbody.position = position;
		rigidbody.rotation = rotation;
		float num = BMath.Max(GameManager.GolfSettings.TeeMaxSwingPowerSpeed * hitPower, GameManager.GolfSettings.TeePostHitMinSpeed);
		rigidbody.linearVelocity = hitDirection.normalized * num;
		float num2 = BMath.Max(GameManager.GolfSettings.TeeMaxSwingAngularSpeed * hitPower, GameManager.GolfSettings.TeePostHitMinAngularSpeed);
		rigidbody.angularVelocity = Vector3.Cross(hitDirection, Vector3.up).normalized * num2;
		hitTimestamp = Time.timeAsDouble;
	}

	public void OnBUpdate()
	{
		float timeSince = BMath.GetTimeSince(hitTimestamp);
		float teePostHitAnimationDuration = GameManager.GolfSettings.TeePostHitAnimationDuration;
		if (timeSince >= teePostHitAnimationDuration)
		{
			GolfTeeManager.ReturnHitTee(this);
			return;
		}
		float num = GameManager.GolfSettings.TeePostHitSizeCurve.Evaluate(timeSince);
		base.transform.localScale = num * Vector3.one;
	}
}
