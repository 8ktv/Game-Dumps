using System;
using System.Collections.Generic;
using FMODUnity;
using Mirror;
using Mirror.RemoteCalls;
using UnityEngine;
using UnityEngine.Pool;

public class JumpPad : NetworkBehaviour
{
	private enum JumpType
	{
		Default,
		Ball,
		Player,
		DivingPlayer,
		KnockedOutPlayer
	}

	[Serializable]
	private struct JumpSettings
	{
		[Range(0f, 180f)]
		public float pitch;

		[Min(0f)]
		public float speed;

		public static JumpSettings Default => new JumpSettings
		{
			pitch = 45f,
			speed = 20f
		};
	}

	[SerializeField]
	private JumpPadCollider collider;

	[SerializeField]
	private JumpPadTrigger[] triggers;

	[SerializeField]
	private JumpPadVfx vfx;

	[SerializeField]
	private JumpSettings playerJump = JumpSettings.Default;

	[SerializeField]
	private JumpSettings divingPlayerJump = JumpSettings.Default;

	[SerializeField]
	private JumpSettings knockedOutPlayerJump = JumpSettings.Default;

	[SerializeField]
	private JumpSettings ballJump = JumpSettings.Default;

	[SerializeField]
	private JumpSettings defaultJump = JumpSettings.Default;

	private readonly Dictionary<Hittable, double> hitHittableTimestamps = new Dictionary<Hittable, double>();

	private AntiCheatPerPlayerRateChecker serverPlayJumpEffectsCommandRateLimiter;

	public JumpPadCollider Collider => collider;

	private void Awake()
	{
		PhysicsManager.RegisterJumpPad(this);
		collider.OnCollisionStayTriggered += OnCollisionStayTriggered;
		JumpPadTrigger[] array = triggers;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].OnTriggerStayTriggered += OnTriggerStayTriggered;
		}
	}

	private void OnDestroy()
	{
		PhysicsManager.DeregisterJumpPad(this);
	}

	public override void OnStartServer()
	{
		serverPlayJumpEffectsCommandRateLimiter = new AntiCheatPerPlayerRateChecker("Jump pad effects", 0.1f, 20, 40, 1f);
	}

	private void OnCollisionStayTriggered(Collision collision)
	{
		if (collision.contactCount > 0 && collision.GetContact(0).normal.IsWithin1DegFrom(-base.transform.up))
		{
			ParseCollisionWith(collision.collider);
		}
	}

	private void OnTriggerStayTriggered(Collider collider)
	{
		if (!(collider.attachedRigidbody != null) || !(Vector3.Dot(collider.attachedRigidbody.linearVelocity, base.transform.up) <= 0f))
		{
			ParseCollisionWith(collider);
		}
	}

	private void ParseCollisionWith(Collider collider)
	{
		if (collider.gameObject.layer == GameManager.LayerSettings.StationaryBallLayer)
		{
			return;
		}
		Hittable foundComponent;
		if (collider.attachedRigidbody != null)
		{
			if (!collider.attachedRigidbody.TryGetComponentInParent<Hittable>(out foundComponent, includeInactive: true))
			{
				return;
			}
		}
		else if (!collider.TryGetComponentInParent<Hittable>(out foundComponent, includeInactive: true))
		{
			return;
		}
		ClearOldHitHittables();
		TryApplyJumpInternal(foundComponent);
		void ClearOldHitHittables()
		{
			List<Hittable> value;
			using (CollectionPool<List<Hittable>, Hittable>.Get(out value))
			{
				foreach (var (item, timestamp) in hitHittableTimestamps)
				{
					if (BMath.GetTimeSince(timestamp) > GameManager.HazardSettings.JumpPadCooldown)
					{
						value.Add(item);
					}
				}
				foreach (Hittable item2 in value)
				{
					hitHittableTimestamps.Remove(item2);
				}
			}
		}
	}

	public bool TryTriggerJumpFor(Hittable hittable)
	{
		return TryApplyJumpInternal(hittable);
	}

	private bool TryApplyJumpInternal(Hittable hittable)
	{
		if (hitHittableTimestamps.TryGetValue(hittable, out var value) && BMath.GetTimeSince(value) < GameManager.HazardSettings.JumpPadCooldown)
		{
			return false;
		}
		Vector3 jumpVelocity = GetJumpVelocity(GetJumpType(hittable));
		hittable.HitWithJumpPadLocalOnly(jumpVelocity);
		hitHittableTimestamps[hittable] = Time.timeAsDouble;
		TryPlayVfx();
		return true;
		static JumpType GetJumpType(Hittable hitHittable)
		{
			if (hitHittable == null)
			{
				return JumpType.Default;
			}
			if (hitHittable.AsEntity.IsPlayer)
			{
				if (hitHittable.AsEntity.PlayerInfo.Movement.DivingState != DivingState.None)
				{
					return JumpType.DivingPlayer;
				}
				if (hitHittable.AsEntity.PlayerInfo.Movement.IsKnockedOut)
				{
					return JumpType.KnockedOutPlayer;
				}
				return JumpType.Player;
			}
			if (hitHittable.AsEntity.IsGolfBall)
			{
				return JumpType.Ball;
			}
			return JumpType.Default;
		}
		void TryPlayVfx()
		{
			if (base.isServer)
			{
				if (hittable.connectionToClient != null && hittable.connectionToClient != NetworkServer.localConnection)
				{
					return;
				}
			}
			else if (!hittable.isOwned)
			{
				return;
			}
			PlayJumpEffectsInternal();
			CmdPlayJumpEffectsForAllClients(hittable);
		}
	}

	[Command(requiresAuthority = false)]
	private void CmdPlayJumpEffectsForAllClients(Hittable targetHittable, NetworkConnectionToClient sender = null)
	{
		if (base.isServer && base.isClient)
		{
			UserCode_CmdPlayJumpEffectsForAllClients__Hittable__NetworkConnectionToClient(targetHittable, sender);
			return;
		}
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteNetworkBehaviour(targetHittable);
		SendCommandInternal("System.Void JumpPad::CmdPlayJumpEffectsForAllClients(Hittable,Mirror.NetworkConnectionToClient)", 1477160144, writer, 0, requiresAuthority: false);
		NetworkWriterPool.Return(writer);
	}

	[TargetRpc]
	private void RpcPlayJumpEffects(NetworkConnectionToClient connection)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendTargetRPCInternal(connection, "System.Void JumpPad::RpcPlayJumpEffects(Mirror.NetworkConnectionToClient)", 887597012, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	private void PlayJumpEffectsInternal()
	{
		vfx.OnActivated();
		RuntimeManager.PlayOneShot(GameManager.AudioSettings.JumpPadTriggeredEvent, base.transform.position);
	}

	private Vector3 GetJumpVelocity(JumpType jumpType)
	{
		JumpSettings jumpSettings = jumpType switch
		{
			JumpType.Player => playerJump, 
			JumpType.DivingPlayer => divingPlayerJump, 
			JumpType.KnockedOutPlayer => knockedOutPlayerJump, 
			JumpType.Ball => ballJump, 
			_ => defaultJump, 
		};
		return Quaternion.AngleAxis(0f - jumpSettings.pitch, base.transform.right) * base.transform.forward * jumpSettings.speed;
	}

	public override bool Weaved()
	{
		return true;
	}

	protected void UserCode_CmdPlayJumpEffectsForAllClients__Hittable__NetworkConnectionToClient(Hittable targetHittable, NetworkConnectionToClient sender)
	{
		if (!serverPlayJumpEffectsCommandRateLimiter.RegisterHit(sender) || targetHittable == null)
		{
			return;
		}
		if (sender == null || sender == NetworkServer.localConnection)
		{
			if (targetHittable.connectionToClient != null && targetHittable.connectionToClient != NetworkServer.localConnection)
			{
				return;
			}
		}
		else if (targetHittable.connectionToClient != sender)
		{
			return;
		}
		foreach (NetworkConnectionToClient value in NetworkServer.connections.Values)
		{
			if (value != NetworkServer.localConnection && value != sender)
			{
				RpcPlayJumpEffects(value);
			}
		}
		if (sender != null && sender != NetworkServer.localConnection)
		{
			PlayJumpEffectsInternal();
		}
	}

	protected static void InvokeUserCode_CmdPlayJumpEffectsForAllClients__Hittable__NetworkConnectionToClient(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdPlayJumpEffectsForAllClients called on client.");
		}
		else
		{
			((JumpPad)obj).UserCode_CmdPlayJumpEffectsForAllClients__Hittable__NetworkConnectionToClient(reader.ReadNetworkBehaviour<Hittable>(), senderConnection);
		}
	}

	protected void UserCode_RpcPlayJumpEffects__NetworkConnectionToClient(NetworkConnectionToClient connection)
	{
		PlayJumpEffectsInternal();
	}

	protected static void InvokeUserCode_RpcPlayJumpEffects__NetworkConnectionToClient(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("TargetRPC RpcPlayJumpEffects called on server.");
		}
		else
		{
			((JumpPad)obj).UserCode_RpcPlayJumpEffects__NetworkConnectionToClient(null);
		}
	}

	static JumpPad()
	{
		RemoteProcedureCalls.RegisterCommand(typeof(JumpPad), "System.Void JumpPad::CmdPlayJumpEffectsForAllClients(Hittable,Mirror.NetworkConnectionToClient)", InvokeUserCode_CmdPlayJumpEffectsForAllClients__Hittable__NetworkConnectionToClient, requiresAuthority: false);
		RemoteProcedureCalls.RegisterRpc(typeof(JumpPad), "System.Void JumpPad::RpcPlayJumpEffects(Mirror.NetworkConnectionToClient)", InvokeUserCode_RpcPlayJumpEffects__NetworkConnectionToClient);
	}
}
