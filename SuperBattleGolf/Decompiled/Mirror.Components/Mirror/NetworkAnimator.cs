using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Mirror.RemoteCalls;
using UnityEngine;
using UnityEngine.Serialization;

namespace Mirror;

[AddComponentMenu("Network/Network Animator")]
[HelpURL("https://mirror-networking.gitbook.io/docs/components/network-animator")]
public class NetworkAnimator : NetworkBehaviour
{
	[Obsolete("clientAuthority was replaced with syncDirection. To enable client authority, set SyncDirection to ClientToServer in the Inspector.")]
	[Header("[Obsolete - use Sync Direction instead]")]
	[Tooltip("Obsolete - use Sync Direction instead")]
	public bool clientAuthority;

	[FormerlySerializedAs("m_Animator")]
	[Header("Animator")]
	[Tooltip("Animator that will have parameters synchronized")]
	public Animator animator;

	[SyncVar(hook = "OnAnimatorSpeedChanged")]
	private float animatorSpeed = 1f;

	private float previousSpeed = 1f;

	private int[] lastIntParameters;

	private float[] lastFloatParameters;

	private bool[] lastBoolParameters;

	private AnimatorControllerParameter[] parameters;

	private int[] animationHash;

	private int[] transitionHash;

	private float[] layerWeight;

	private double nextSendTime;

	[HideInInspector]
	public int[] parametersToSkipNameHashes;

	private readonly HashSet<int> triggerBuffer = new HashSet<int>();

	public Action<float, float> _Mirror_SyncVarHookDelegate_animatorSpeed;

	private bool SendMessagesAllowed
	{
		get
		{
			if (base.isServer)
			{
				if (syncDirection == SyncDirection.ServerToClient)
				{
					return true;
				}
				if (base.netIdentity != null && base.netIdentity.connectionToClient == null)
				{
					return true;
				}
			}
			if (base.isOwned)
			{
				return syncDirection == SyncDirection.ClientToServer;
			}
			return false;
		}
	}

	public float NetworkanimatorSpeed
	{
		get
		{
			return animatorSpeed;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref animatorSpeed, 1uL, _Mirror_SyncVarHookDelegate_animatorSpeed);
		}
	}

	private void Initialize()
	{
		HashSet<int> parametersToSkipNameHashesHashSet = ((parametersToSkipNameHashes != null) ? new HashSet<int>(parametersToSkipNameHashes) : null);
		parameters = animator.parameters.Where(ShouldSyncParameter).ToArray();
		lastIntParameters = new int[parameters.Length];
		lastFloatParameters = new float[parameters.Length];
		lastBoolParameters = new bool[parameters.Length];
		animationHash = new int[animator.layerCount];
		transitionHash = new int[animator.layerCount];
		layerWeight = new float[animator.layerCount];
		bool ShouldSyncParameter(AnimatorControllerParameter parameter)
		{
			if (animator.IsParameterControlledByCurve(parameter.nameHash))
			{
				return false;
			}
			if (parametersToSkipNameHashesHashSet != null && parametersToSkipNameHashesHashSet.Contains(parameter.nameHash))
			{
				return false;
			}
			return true;
		}
	}

	private void Awake()
	{
		Initialize();
	}

	private void OnEnable()
	{
		Initialize();
	}

	protected override void OnValidate()
	{
		if (!Application.isPlaying)
		{
			base.OnValidate();
			if (clientAuthority)
			{
				syncDirection = SyncDirection.ClientToServer;
				Debug.LogWarning(base.name + "'s NetworkAnimator component has obsolete .clientAuthority enabled. Please disable it and set SyncDirection to ClientToServer instead.", base.gameObject);
			}
		}
	}

	public virtual void Reset()
	{
		syncDirection = SyncDirection.ClientToServer;
	}

	private void FixedUpdate()
	{
		if (!SendMessagesAllowed || !animator.enabled)
		{
			return;
		}
		CheckSendRate();
		for (int i = 0; i < animator.layerCount; i++)
		{
			if (CheckAnimStateChanged(out var stateHash, out var normalizedTime, i))
			{
				using NetworkWriterPooled networkWriterPooled = NetworkWriterPool.Get();
				WriteParameters(networkWriterPooled);
				SendAnimationMessage(stateHash, normalizedTime, i, layerWeight[i], networkWriterPooled.ToArray());
			}
		}
		CheckSpeed();
		foreach (int item in triggerBuffer)
		{
			SendTriggerMessageInternal(item);
		}
		triggerBuffer.Clear();
	}

	private void CheckSpeed()
	{
		float speed = animator.speed;
		if (Mathf.Abs(previousSpeed - speed) > 0.001f)
		{
			previousSpeed = speed;
			if (base.isServer)
			{
				NetworkanimatorSpeed = speed;
			}
			else if (base.isClient)
			{
				CmdSetAnimatorSpeed(speed);
			}
		}
	}

	private void OnAnimatorSpeedChanged(float _, float value)
	{
		if (!base.isServer && (!base.isOwned || syncDirection != SyncDirection.ClientToServer))
		{
			animator.speed = value;
		}
	}

	private bool CheckAnimStateChanged(out int stateHash, out float normalizedTime, int layerId)
	{
		bool result = false;
		stateHash = 0;
		normalizedTime = 0f;
		float num = animator.GetLayerWeight(layerId);
		if (Mathf.Abs(num - layerWeight[layerId]) > 0.001f)
		{
			layerWeight[layerId] = num;
			result = true;
		}
		if (animator.IsInTransition(layerId))
		{
			AnimatorTransitionInfo animatorTransitionInfo = animator.GetAnimatorTransitionInfo(layerId);
			if (animatorTransitionInfo.fullPathHash != transitionHash[layerId])
			{
				transitionHash[layerId] = animatorTransitionInfo.fullPathHash;
				animationHash[layerId] = 0;
				return true;
			}
			return result;
		}
		AnimatorStateInfo currentAnimatorStateInfo = animator.GetCurrentAnimatorStateInfo(layerId);
		if (currentAnimatorStateInfo.fullPathHash != animationHash[layerId])
		{
			if (animationHash[layerId] != 0)
			{
				stateHash = currentAnimatorStateInfo.fullPathHash;
				normalizedTime = currentAnimatorStateInfo.normalizedTime;
			}
			transitionHash[layerId] = 0;
			animationHash[layerId] = currentAnimatorStateInfo.fullPathHash;
			return true;
		}
		return result;
	}

	private void CheckSendRate()
	{
		double localTime = NetworkTime.localTime;
		if (!SendMessagesAllowed || !(syncInterval >= 0f) || !(localTime > nextSendTime))
		{
			return;
		}
		nextSendTime = localTime + (double)syncInterval;
		using NetworkWriterPooled networkWriterPooled = NetworkWriterPool.Get();
		if (WriteParameters(networkWriterPooled))
		{
			SendAnimationParametersMessage(networkWriterPooled.ToArray());
		}
	}

	private void SendAnimationMessage(int stateHash, float normalizedTime, int layerId, float weight, byte[] parameters)
	{
		if (base.isServer)
		{
			RpcOnAnimationClientMessage(stateHash, normalizedTime, layerId, weight, parameters);
		}
		else if (base.isClient)
		{
			CmdOnAnimationServerMessage(stateHash, normalizedTime, layerId, weight, parameters);
		}
	}

	private void SendAnimationParametersMessage(byte[] parameters)
	{
		if (base.isServer)
		{
			RpcOnAnimationParametersClientMessage(parameters);
		}
		else if (base.isClient)
		{
			CmdOnAnimationParametersServerMessage(parameters);
		}
	}

	private void HandleAnimMsg(int stateHash, float normalizedTime, int layerId, float weight, NetworkReader reader)
	{
		if (!base.isOwned || syncDirection != SyncDirection.ClientToServer)
		{
			if (stateHash != 0 && animator.enabled)
			{
				animator.Play(stateHash, layerId, normalizedTime);
			}
			animator.SetLayerWeight(layerId, weight);
			ReadParameters(reader);
		}
	}

	private void HandleAnimParamsMsg(NetworkReader reader)
	{
		if (!base.isOwned || syncDirection != SyncDirection.ClientToServer)
		{
			ReadParameters(reader);
		}
	}

	private void HandleAnimTriggerMsg(int hash)
	{
		if (animator.enabled)
		{
			animator.SetTrigger(hash);
		}
	}

	private void HandleAnimResetTriggerMsg(int hash)
	{
		if (animator.enabled)
		{
			animator.ResetTrigger(hash);
		}
	}

	private ulong NextDirtyBits()
	{
		ulong num = 0uL;
		for (int i = 0; i < parameters.Length; i++)
		{
			AnimatorControllerParameter animatorControllerParameter = parameters[i];
			bool flag = false;
			if (animatorControllerParameter.type == AnimatorControllerParameterType.Int)
			{
				int integer = animator.GetInteger(animatorControllerParameter.nameHash);
				flag = integer != lastIntParameters[i];
				if (flag)
				{
					lastIntParameters[i] = integer;
				}
			}
			else if (animatorControllerParameter.type == AnimatorControllerParameterType.Float)
			{
				float num2 = animator.GetFloat(animatorControllerParameter.nameHash);
				flag = Mathf.Abs(num2 - lastFloatParameters[i]) > 0.001f;
				if (flag)
				{
					lastFloatParameters[i] = num2;
				}
			}
			else if (animatorControllerParameter.type == AnimatorControllerParameterType.Bool)
			{
				bool flag2 = animator.GetBool(animatorControllerParameter.nameHash);
				flag = flag2 != lastBoolParameters[i];
				if (flag)
				{
					lastBoolParameters[i] = flag2;
				}
			}
			if (flag)
			{
				num |= (ulong)(1L << i);
			}
		}
		return num;
	}

	private bool WriteParameters(NetworkWriter writer, bool forceAll = false)
	{
		byte b = (byte)parameters.Length;
		writer.WriteByte(b);
		ulong num = (forceAll ? ulong.MaxValue : NextDirtyBits());
		writer.WriteULong(num);
		for (int i = 0; i < b; i++)
		{
			if ((num & (ulong)(1L << i)) != 0L)
			{
				AnimatorControllerParameter animatorControllerParameter = parameters[i];
				if (animatorControllerParameter.type == AnimatorControllerParameterType.Int)
				{
					int integer = animator.GetInteger(animatorControllerParameter.nameHash);
					writer.WriteInt(integer);
				}
				else if (animatorControllerParameter.type == AnimatorControllerParameterType.Float)
				{
					float value = animator.GetFloat(animatorControllerParameter.nameHash);
					writer.WriteFloat(value);
				}
				else if (animatorControllerParameter.type == AnimatorControllerParameterType.Bool)
				{
					bool value2 = animator.GetBool(animatorControllerParameter.nameHash);
					writer.WriteBool(value2);
				}
			}
		}
		return num != 0;
	}

	private void ReadParameters(NetworkReader reader)
	{
		byte b = reader.ReadByte();
		if (b != parameters.Length)
		{
			Debug.LogError($"NetworkAnimator: serialized parameter count={b} does not match expected parameter count={parameters.Length}. Are you changing animators at runtime?", base.gameObject);
			return;
		}
		bool flag = animator.enabled;
		ulong num = reader.ReadULong();
		for (int i = 0; i < b; i++)
		{
			if ((num & (ulong)(1L << i)) == 0L)
			{
				continue;
			}
			AnimatorControllerParameter animatorControllerParameter = parameters[i];
			if (animatorControllerParameter.type == AnimatorControllerParameterType.Int)
			{
				int value = reader.ReadInt();
				if (flag)
				{
					animator.SetInteger(animatorControllerParameter.nameHash, value);
				}
			}
			else if (animatorControllerParameter.type == AnimatorControllerParameterType.Float)
			{
				float value2 = reader.ReadFloat();
				if (flag)
				{
					animator.SetFloat(animatorControllerParameter.nameHash, value2);
				}
			}
			else if (animatorControllerParameter.type == AnimatorControllerParameterType.Bool)
			{
				bool value3 = reader.ReadBool();
				if (flag)
				{
					animator.SetBool(animatorControllerParameter.nameHash, value3);
				}
			}
		}
	}

	public override void OnSerialize(NetworkWriter writer, bool initialState)
	{
		base.OnSerialize(writer, initialState);
		if (initialState)
		{
			byte b = (byte)animator.layerCount;
			writer.WriteByte(b);
			for (int i = 0; i < b; i++)
			{
				AnimatorStateInfo animatorStateInfo = (animator.IsInTransition(i) ? animator.GetNextAnimatorStateInfo(i) : animator.GetCurrentAnimatorStateInfo(i));
				writer.WriteInt(animatorStateInfo.fullPathHash);
				writer.WriteFloat(animatorStateInfo.normalizedTime);
				writer.WriteFloat(animator.GetLayerWeight(i));
			}
			WriteParameters(writer, forceAll: true);
		}
	}

	public override void OnDeserialize(NetworkReader reader, bool initialState)
	{
		base.OnDeserialize(reader, initialState);
		if (!initialState)
		{
			return;
		}
		byte b = reader.ReadByte();
		if (b != animator.layerCount)
		{
			Debug.LogError($"NetworkAnimator: serialized layer count={b} does not match expected layer count={animator.layerCount}. Are you changing animators at runtime?", base.gameObject);
			return;
		}
		for (int i = 0; i < b; i++)
		{
			int stateNameHash = reader.ReadInt();
			float normalizedTime = reader.ReadFloat();
			float weight = reader.ReadFloat();
			animator.SetLayerWeight(i, weight);
			animator.Play(stateNameHash, i, normalizedTime);
		}
		ReadParameters(reader);
	}

	public void SetTrigger(string triggerName)
	{
		SetTrigger(Animator.StringToHash(triggerName));
	}

	public void SetTrigger(int hash)
	{
		HandleAnimTriggerMsg(hash);
		triggerBuffer.Add(hash);
	}

	public void SendTriggerMessageInternal(int hash)
	{
		if (syncDirection == SyncDirection.ClientToServer)
		{
			if (!base.isClient)
			{
				Debug.LogWarning("Tried to set animation in the server for a client-controlled animator", base.gameObject);
			}
			else if (!base.isOwned)
			{
				Debug.LogWarning("Only the client with authority can set animations", base.gameObject);
			}
			else if (base.isClient)
			{
				CmdOnAnimationTriggerServerMessage(hash);
			}
		}
		else if (!base.isServer)
		{
			Debug.LogWarning("Tried to set animation in the client for a server-controlled animator", base.gameObject);
		}
		else
		{
			HandleAnimTriggerMsg(hash);
			RpcOnAnimationTriggerClientMessage(hash);
		}
	}

	public void ResetTrigger(string triggerName)
	{
		ResetTrigger(Animator.StringToHash(triggerName));
	}

	public void ResetTrigger(int hash)
	{
		if (syncDirection == SyncDirection.ClientToServer)
		{
			if (!base.isClient)
			{
				Debug.LogWarning("Tried to reset animation in the server for a client-controlled animator", base.gameObject);
				return;
			}
			if (!base.isOwned)
			{
				Debug.LogWarning("Only the client with authority can reset animations", base.gameObject);
				return;
			}
			if (base.isClient)
			{
				CmdOnAnimationResetTriggerServerMessage(hash);
			}
			HandleAnimResetTriggerMsg(hash);
		}
		else if (!base.isServer)
		{
			Debug.LogWarning("Tried to reset animation in the client for a server-controlled animator", base.gameObject);
		}
		else
		{
			HandleAnimResetTriggerMsg(hash);
			RpcOnAnimationResetTriggerClientMessage(hash);
		}
	}

	[Command]
	private void CmdOnAnimationServerMessage(int stateHash, float normalizedTime, int layerId, float weight, byte[] parameters)
	{
		if (base.isServer && base.isClient)
		{
			UserCode_CmdOnAnimationServerMessage__Int32__Single__Int32__Single__Byte_005B_005D(stateHash, normalizedTime, layerId, weight, parameters);
			return;
		}
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteVarInt(stateHash);
		writer.WriteFloat(normalizedTime);
		writer.WriteVarInt(layerId);
		writer.WriteFloat(weight);
		writer.WriteBytesAndSize(parameters);
		SendCommandInternal("System.Void Mirror.NetworkAnimator::CmdOnAnimationServerMessage(System.Int32,System.Single,System.Int32,System.Single,System.Byte[])", -1058764313, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[Command]
	private void CmdOnAnimationParametersServerMessage(byte[] parameters)
	{
		if (base.isServer && base.isClient)
		{
			UserCode_CmdOnAnimationParametersServerMessage__Byte_005B_005D(parameters);
			return;
		}
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteBytesAndSize(parameters);
		SendCommandInternal("System.Void Mirror.NetworkAnimator::CmdOnAnimationParametersServerMessage(System.Byte[])", -581664369, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[Command]
	private void CmdOnAnimationTriggerServerMessage(int hash)
	{
		if (base.isServer && base.isClient)
		{
			UserCode_CmdOnAnimationTriggerServerMessage__Int32(hash);
			return;
		}
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteVarInt(hash);
		SendCommandInternal("System.Void Mirror.NetworkAnimator::CmdOnAnimationTriggerServerMessage(System.Int32)", -842357433, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[Command]
	private void CmdOnAnimationResetTriggerServerMessage(int hash)
	{
		if (base.isServer && base.isClient)
		{
			UserCode_CmdOnAnimationResetTriggerServerMessage__Int32(hash);
			return;
		}
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteVarInt(hash);
		SendCommandInternal("System.Void Mirror.NetworkAnimator::CmdOnAnimationResetTriggerServerMessage(System.Int32)", -196964342, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[Command]
	private void CmdSetAnimatorSpeed(float newSpeed)
	{
		if (base.isServer && base.isClient)
		{
			UserCode_CmdSetAnimatorSpeed__Single(newSpeed);
			return;
		}
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteFloat(newSpeed);
		SendCommandInternal("System.Void Mirror.NetworkAnimator::CmdSetAnimatorSpeed(System.Single)", 672320566, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[ClientRpc]
	private void RpcOnAnimationClientMessage(int stateHash, float normalizedTime, int layerId, float weight, byte[] parameters)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteVarInt(stateHash);
		writer.WriteFloat(normalizedTime);
		writer.WriteVarInt(layerId);
		writer.WriteFloat(weight);
		writer.WriteBytesAndSize(parameters);
		SendRPCInternal("System.Void Mirror.NetworkAnimator::RpcOnAnimationClientMessage(System.Int32,System.Single,System.Int32,System.Single,System.Byte[])", -392669502, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	[ClientRpc]
	private void RpcOnAnimationParametersClientMessage(byte[] parameters)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteBytesAndSize(parameters);
		SendRPCInternal("System.Void Mirror.NetworkAnimator::RpcOnAnimationParametersClientMessage(System.Byte[])", -2095336766, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	[ClientRpc]
	private void RpcOnAnimationTriggerClientMessage(int hash)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteVarInt(hash);
		SendRPCInternal("System.Void Mirror.NetworkAnimator::RpcOnAnimationTriggerClientMessage(System.Int32)", 1759094990, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	[ClientRpc]
	private void RpcOnAnimationResetTriggerClientMessage(int hash)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteVarInt(hash);
		SendRPCInternal("System.Void Mirror.NetworkAnimator::RpcOnAnimationResetTriggerClientMessage(System.Int32)", 1545278305, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	public NetworkAnimator()
	{
		_Mirror_SyncVarHookDelegate_animatorSpeed = OnAnimatorSpeedChanged;
	}

	public override bool Weaved()
	{
		return true;
	}

	protected void UserCode_CmdOnAnimationServerMessage__Int32__Single__Int32__Single__Byte_005B_005D(int stateHash, float normalizedTime, int layerId, float weight, byte[] parameters)
	{
		if (syncDirection == SyncDirection.ServerToClient)
		{
			return;
		}
		using NetworkReaderPooled reader = NetworkReaderPool.Get(parameters);
		HandleAnimMsg(stateHash, normalizedTime, layerId, weight, reader);
		RpcOnAnimationClientMessage(stateHash, normalizedTime, layerId, weight, parameters);
	}

	protected static void InvokeUserCode_CmdOnAnimationServerMessage__Int32__Single__Int32__Single__Byte_005B_005D(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdOnAnimationServerMessage called on client.");
		}
		else
		{
			((NetworkAnimator)obj).UserCode_CmdOnAnimationServerMessage__Int32__Single__Int32__Single__Byte_005B_005D(reader.ReadVarInt(), reader.ReadFloat(), reader.ReadVarInt(), reader.ReadFloat(), reader.ReadBytesAndSize());
		}
	}

	protected void UserCode_CmdOnAnimationParametersServerMessage__Byte_005B_005D(byte[] parameters)
	{
		if (syncDirection == SyncDirection.ServerToClient)
		{
			return;
		}
		using NetworkReaderPooled reader = NetworkReaderPool.Get(parameters);
		HandleAnimParamsMsg(reader);
		RpcOnAnimationParametersClientMessage(parameters);
	}

	protected static void InvokeUserCode_CmdOnAnimationParametersServerMessage__Byte_005B_005D(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdOnAnimationParametersServerMessage called on client.");
		}
		else
		{
			((NetworkAnimator)obj).UserCode_CmdOnAnimationParametersServerMessage__Byte_005B_005D(reader.ReadBytesAndSize());
		}
	}

	protected void UserCode_CmdOnAnimationTriggerServerMessage__Int32(int hash)
	{
		if (syncDirection != SyncDirection.ServerToClient)
		{
			if (!base.isClient || !base.isOwned)
			{
				HandleAnimTriggerMsg(hash);
			}
			RpcOnAnimationTriggerClientMessage(hash);
		}
	}

	protected static void InvokeUserCode_CmdOnAnimationTriggerServerMessage__Int32(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdOnAnimationTriggerServerMessage called on client.");
		}
		else
		{
			((NetworkAnimator)obj).UserCode_CmdOnAnimationTriggerServerMessage__Int32(reader.ReadVarInt());
		}
	}

	protected void UserCode_CmdOnAnimationResetTriggerServerMessage__Int32(int hash)
	{
		if (syncDirection != SyncDirection.ServerToClient)
		{
			if (!base.isClient || !base.isOwned)
			{
				HandleAnimResetTriggerMsg(hash);
			}
			RpcOnAnimationResetTriggerClientMessage(hash);
		}
	}

	protected static void InvokeUserCode_CmdOnAnimationResetTriggerServerMessage__Int32(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdOnAnimationResetTriggerServerMessage called on client.");
		}
		else
		{
			((NetworkAnimator)obj).UserCode_CmdOnAnimationResetTriggerServerMessage__Int32(reader.ReadVarInt());
		}
	}

	protected void UserCode_CmdSetAnimatorSpeed__Single(float newSpeed)
	{
		animator.speed = newSpeed;
		NetworkanimatorSpeed = newSpeed;
	}

	protected static void InvokeUserCode_CmdSetAnimatorSpeed__Single(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdSetAnimatorSpeed called on client.");
		}
		else
		{
			((NetworkAnimator)obj).UserCode_CmdSetAnimatorSpeed__Single(reader.ReadFloat());
		}
	}

	protected void UserCode_RpcOnAnimationClientMessage__Int32__Single__Int32__Single__Byte_005B_005D(int stateHash, float normalizedTime, int layerId, float weight, byte[] parameters)
	{
		using NetworkReaderPooled reader = NetworkReaderPool.Get(parameters);
		HandleAnimMsg(stateHash, normalizedTime, layerId, weight, reader);
	}

	protected static void InvokeUserCode_RpcOnAnimationClientMessage__Int32__Single__Int32__Single__Byte_005B_005D(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC RpcOnAnimationClientMessage called on server.");
		}
		else
		{
			((NetworkAnimator)obj).UserCode_RpcOnAnimationClientMessage__Int32__Single__Int32__Single__Byte_005B_005D(reader.ReadVarInt(), reader.ReadFloat(), reader.ReadVarInt(), reader.ReadFloat(), reader.ReadBytesAndSize());
		}
	}

	protected void UserCode_RpcOnAnimationParametersClientMessage__Byte_005B_005D(byte[] parameters)
	{
		using NetworkReaderPooled reader = NetworkReaderPool.Get(parameters);
		HandleAnimParamsMsg(reader);
	}

	protected static void InvokeUserCode_RpcOnAnimationParametersClientMessage__Byte_005B_005D(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC RpcOnAnimationParametersClientMessage called on server.");
		}
		else
		{
			((NetworkAnimator)obj).UserCode_RpcOnAnimationParametersClientMessage__Byte_005B_005D(reader.ReadBytesAndSize());
		}
	}

	protected void UserCode_RpcOnAnimationTriggerClientMessage__Int32(int hash)
	{
		bool flag = base.isOwned && syncDirection == SyncDirection.ClientToServer;
		if (!base.isServer && !flag)
		{
			HandleAnimTriggerMsg(hash);
		}
	}

	protected static void InvokeUserCode_RpcOnAnimationTriggerClientMessage__Int32(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC RpcOnAnimationTriggerClientMessage called on server.");
		}
		else
		{
			((NetworkAnimator)obj).UserCode_RpcOnAnimationTriggerClientMessage__Int32(reader.ReadVarInt());
		}
	}

	protected void UserCode_RpcOnAnimationResetTriggerClientMessage__Int32(int hash)
	{
		bool flag = base.isOwned && syncDirection == SyncDirection.ClientToServer;
		if (!base.isServer && !flag)
		{
			HandleAnimResetTriggerMsg(hash);
		}
	}

	protected static void InvokeUserCode_RpcOnAnimationResetTriggerClientMessage__Int32(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC RpcOnAnimationResetTriggerClientMessage called on server.");
		}
		else
		{
			((NetworkAnimator)obj).UserCode_RpcOnAnimationResetTriggerClientMessage__Int32(reader.ReadVarInt());
		}
	}

	static NetworkAnimator()
	{
		RemoteProcedureCalls.RegisterCommand(typeof(NetworkAnimator), "System.Void Mirror.NetworkAnimator::CmdOnAnimationServerMessage(System.Int32,System.Single,System.Int32,System.Single,System.Byte[])", InvokeUserCode_CmdOnAnimationServerMessage__Int32__Single__Int32__Single__Byte_005B_005D, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(NetworkAnimator), "System.Void Mirror.NetworkAnimator::CmdOnAnimationParametersServerMessage(System.Byte[])", InvokeUserCode_CmdOnAnimationParametersServerMessage__Byte_005B_005D, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(NetworkAnimator), "System.Void Mirror.NetworkAnimator::CmdOnAnimationTriggerServerMessage(System.Int32)", InvokeUserCode_CmdOnAnimationTriggerServerMessage__Int32, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(NetworkAnimator), "System.Void Mirror.NetworkAnimator::CmdOnAnimationResetTriggerServerMessage(System.Int32)", InvokeUserCode_CmdOnAnimationResetTriggerServerMessage__Int32, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(NetworkAnimator), "System.Void Mirror.NetworkAnimator::CmdSetAnimatorSpeed(System.Single)", InvokeUserCode_CmdSetAnimatorSpeed__Single, requiresAuthority: true);
		RemoteProcedureCalls.RegisterRpc(typeof(NetworkAnimator), "System.Void Mirror.NetworkAnimator::RpcOnAnimationClientMessage(System.Int32,System.Single,System.Int32,System.Single,System.Byte[])", InvokeUserCode_RpcOnAnimationClientMessage__Int32__Single__Int32__Single__Byte_005B_005D);
		RemoteProcedureCalls.RegisterRpc(typeof(NetworkAnimator), "System.Void Mirror.NetworkAnimator::RpcOnAnimationParametersClientMessage(System.Byte[])", InvokeUserCode_RpcOnAnimationParametersClientMessage__Byte_005B_005D);
		RemoteProcedureCalls.RegisterRpc(typeof(NetworkAnimator), "System.Void Mirror.NetworkAnimator::RpcOnAnimationTriggerClientMessage(System.Int32)", InvokeUserCode_RpcOnAnimationTriggerClientMessage__Int32);
		RemoteProcedureCalls.RegisterRpc(typeof(NetworkAnimator), "System.Void Mirror.NetworkAnimator::RpcOnAnimationResetTriggerClientMessage(System.Int32)", InvokeUserCode_RpcOnAnimationResetTriggerClientMessage__Int32);
	}

	public override void SerializeSyncVars(NetworkWriter writer, bool forceAll)
	{
		base.SerializeSyncVars(writer, forceAll);
		if (forceAll)
		{
			writer.WriteFloat(animatorSpeed);
			return;
		}
		writer.WriteVarULong(syncVarDirtyBits);
		if ((syncVarDirtyBits & 1L) != 0L)
		{
			writer.WriteFloat(animatorSpeed);
		}
	}

	public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
	{
		base.DeserializeSyncVars(reader, initialState);
		if (initialState)
		{
			GeneratedSyncVarDeserialize(ref animatorSpeed, _Mirror_SyncVarHookDelegate_animatorSpeed, reader.ReadFloat());
			return;
		}
		long num = (long)reader.ReadVarULong();
		if ((num & 1L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref animatorSpeed, _Mirror_SyncVarHookDelegate_animatorSpeed, reader.ReadFloat());
		}
	}
}
