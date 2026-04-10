using Cysharp.Threading.Tasks;
using FMODUnity;
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

	private bool wasThrown;

	private double appearanceTimestamp;

	public Entity AsEntity { get; private set; }

	public ThrownUsedItemType Type => type;

	public Rigidbody Rigidbody => rigidbody;

	private void OnValidate()
	{
		scaleOverTime.NormalizeTime();
	}

	private void Awake()
	{
		AsEntity = GetComponent<Entity>();
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
		if (wasThrown)
		{
			wasThrown = false;
			RuntimeManager.PlayOneShot(GameManager.AudioSettings.ThrownItemDisappearEvent, base.transform.position);
		}
	}

	public void OnBUpdate()
	{
		float timeSince = BMath.GetTimeSince(appearanceTimestamp);
		if (timeSince > lifeTime)
		{
			OnWillReturntoPool();
			ThrownUsedItemManager.ReturnThrownItem(this);
		}
		else
		{
			base.transform.localScale = scaleOverTime.Evaluate(timeSince / lifeTime) * Vector3.one;
		}
		void OnWillReturntoPool()
		{
			if (type == ThrownUsedItemType.RocketDriver && TryGetComponent<ThrownUsedRocketDriver>(out var component) && component.IsRocketActive)
			{
				VfxManager.PlayPooledVfxLocalOnly(VfxType.RocketDriverDespawn, base.transform.position, base.transform.rotation);
				RuntimeManager.PlayOneShot(GameManager.AudioSettings.RocketDriverUsedThrownExplosionEvent, base.transform.position);
			}
		}
	}

	public void Initialize(Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 angularVelocity)
	{
		base.transform.SetPositionAndRotation(position, rotation);
		rigidbody.position = position;
		rigidbody.rotation = rotation;
		rigidbody.linearVelocity = velocity;
		rigidbody.angularVelocity = angularVelocity;
		wasThrown = true;
	}
}
