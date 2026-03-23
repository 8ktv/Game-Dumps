using UnityEngine;

namespace Mirror;

[AddComponentMenu("Network/Network Rigidbody 2D (Unreliable)")]
public class NetworkRigidbodyUnreliable2D : NetworkTransformUnreliable
{
	private Rigidbody2D rb;

	private bool wasKinematic;

	private bool clientAuthority => syncDirection == SyncDirection.ClientToServer;

	protected override void OnValidate()
	{
		if (!Application.isPlaying)
		{
			base.OnValidate();
			if (target.GetComponent<Rigidbody2D>() == null)
			{
				Debug.LogWarning(base.name + "'s NetworkRigidbody2D.target " + target.name + " is missing a Rigidbody2D", this);
			}
		}
	}

	protected override void Awake()
	{
		rb = target.GetComponent<Rigidbody2D>();
		if (rb == null)
		{
			Debug.LogError(base.name + "'s NetworkRigidbody2D.target " + target.name + " is missing a Rigidbody2D", this);
			return;
		}
		wasKinematic = rb.bodyType.HasFlag(RigidbodyType2D.Kinematic);
		base.Awake();
	}

	public override void OnStopServer()
	{
		rb.bodyType = (wasKinematic ? RigidbodyType2D.Kinematic : RigidbodyType2D.Dynamic);
	}

	public override void OnStopClient()
	{
		rb.bodyType = (wasKinematic ? RigidbodyType2D.Kinematic : RigidbodyType2D.Dynamic);
	}

	private void FixedUpdate()
	{
		if (base.isServer && base.isClient)
		{
			if (clientAuthority && !base.IsClientWithAuthority)
			{
				rb.bodyType = RigidbodyType2D.Kinematic;
			}
		}
		else if (base.isClient)
		{
			if (!base.IsClientWithAuthority)
			{
				rb.bodyType = RigidbodyType2D.Kinematic;
			}
		}
		else if (base.isServer && clientAuthority)
		{
			rb.bodyType = RigidbodyType2D.Kinematic;
		}
	}

	protected override void OnTeleport(Vector3 destination)
	{
		BeforeTeleport();
		base.OnTeleport(destination);
		rb.position = base.transform.position;
		AfterTeleport();
	}

	protected override void OnTeleport(Vector3 destination, Quaternion rotation)
	{
		BeforeTeleport();
		base.OnTeleport(destination, rotation);
		rb.position = base.transform.position;
		rb.rotation = base.transform.rotation.eulerAngles.z;
		AfterTeleport();
	}

	public override bool Weaved()
	{
		return true;
	}
}
