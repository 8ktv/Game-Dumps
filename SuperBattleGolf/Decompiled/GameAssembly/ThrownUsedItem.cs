using Cysharp.Threading.Tasks;
using UnityEngine;

public class ThrownUsedItem : MonoBehaviour, IBUpdateCallback, IAnyBUpdateCallback
{
	[SerializeField]
	private ThrownUsedItemType type;

	[SerializeField]
	private Rigidbody rigidbody;

	[SerializeField]
	private float lifeTime;

	[SerializeField]
	private AnimationCurve scaleOverTime;

	private double appearanceTimestamp;

	public ThrownUsedItemType Type => type;

	public Rigidbody Rigidbody => rigidbody;

	private void OnValidate()
	{
		scaleOverTime.NormalizeTime();
	}

	private void Awake()
	{
		rigidbody.maxAngularVelocity = 30f;
	}

	private void OnEnable()
	{
		BUpdate.RegisterCallback(this);
		appearanceTimestamp = Time.timeAsDouble;
		if (type == ThrownUsedItemType.GolfCartBase || type == ThrownUsedItemType.GolfCartLid)
		{
			TemporarilyIgnoreCollisionsWith(GameManager.LayerSettings.GolfCartsMask);
		}
		async void TemporarilyIgnoreCollisionsWith(int layerMask)
		{
			rigidbody.excludeLayers = layerMask;
			await UniTask.WaitForSeconds(0.4f);
			if (!(this == null))
			{
				rigidbody.excludeLayers = 0;
			}
		}
	}

	private void OnDisable()
	{
		BUpdate.DeregisterCallback(this);
	}

	public void OnBUpdate()
	{
		float timeSince = BMath.GetTimeSince(appearanceTimestamp);
		if (timeSince > lifeTime)
		{
			ThrownUsedItemManager.ReturnThrownItem(this);
		}
		else
		{
			base.transform.localScale = scaleOverTime.Evaluate(timeSince / lifeTime) * Vector3.one;
		}
	}

	public void Initialize(Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 angularVelocity)
	{
		base.transform.SetPositionAndRotation(position, rotation);
		rigidbody.position = position;
		rigidbody.rotation = rotation;
		rigidbody.linearVelocity = velocity;
		rigidbody.angularVelocity = angularVelocity;
	}
}
