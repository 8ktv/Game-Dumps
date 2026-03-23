using UnityEngine;

namespace Mirror;

[AddComponentMenu("Network/Network Rigidbody Hybrid (CORE)")]
public class NetworkRigidbodyHybridCORE : NetworkTransformHybrid
{
	private Rigidbody rb;

	private bool wasKinematic;

	private bool clientAuthority => syncDirection == SyncDirection.ClientToServer;

	protected override void OnValidate()
	{
		if (!Application.isPlaying)
		{
			base.OnValidate();
			if (target.GetComponent<Rigidbody>() == null)
			{
				Debug.LogWarning(base.name + "'s NetworkRigidbody.target " + target.name + " is missing a Rigidbody", this);
			}
		}
	}

	protected override void Awake()
	{
		rb = target.GetComponent<Rigidbody>();
		if (rb == null)
		{
			Debug.LogError(base.name + "'s NetworkRigidbody.target " + target.name + " is missing a Rigidbody", this);
			return;
		}
		wasKinematic = rb.isKinematic;
		base.Awake();
	}

	public override void OnStopServer()
	{
		rb.isKinematic = wasKinematic;
	}

	public override void OnStopClient()
	{
		rb.isKinematic = wasKinematic;
	}

	private void FixedUpdate()
	{
		if (base.isServer && base.isClient)
		{
			if (clientAuthority && !base.IsClientWithAuthority)
			{
				rb.isKinematic = true;
			}
		}
		else if (base.isClient)
		{
			if (!base.IsClientWithAuthority)
			{
				rb.isKinematic = true;
			}
		}
		else if (base.isServer && clientAuthority)
		{
			rb.isKinematic = true;
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
		rb.rotation = base.transform.rotation;
		AfterTeleport();
	}

	public override bool Weaved()
	{
		return true;
	}
}
