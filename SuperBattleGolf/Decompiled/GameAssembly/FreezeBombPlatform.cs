using System;
using System.Runtime.InteropServices;
using Cysharp.Threading.Tasks;
using FMOD.Studio;
using FMODUnity;
using Mirror;
using UnityEngine;

public class FreezeBombPlatform : NetworkBehaviour
{
	[SerializeField]
	private Entity asEntity;

	[SerializeField]
	private GameObject[] meshes;

	[SerializeField]
	private GameObject[] colliders;

	[SerializeField]
	private FreezeBombPlatformVfx vfx;

	[SyncVar(hook = "OnMeshIndexChanged")]
	private int meshIndex = -1;

	[SyncVar(hook = "OnIsShakingChanged")]
	private bool isShaking;

	private EventInstance shakingSoundInstance;

	public Action<int, int> _Mirror_SyncVarHookDelegate_meshIndex;

	public Action<bool, bool> _Mirror_SyncVarHookDelegate_isShaking;

	public int NetworkmeshIndex
	{
		get
		{
			return meshIndex;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref meshIndex, 1uL, _Mirror_SyncVarHookDelegate_meshIndex);
		}
	}

	public bool NetworkisShaking
	{
		get
		{
			return isShaking;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref isShaking, 2uL, _Mirror_SyncVarHookDelegate_isShaking);
		}
	}

	public override void OnStartServer()
	{
		NetworkmeshIndex = UnityEngine.Random.Range(0, meshes.Length);
		DisappearDelayed();
		async void DisappearDelayed()
		{
			await UniTask.WaitForSeconds(GameManager.ItemSettings.FreezeBombPlatformShakeStartTime);
			NetworkisShaking = true;
			await UniTask.WaitForSeconds(GameManager.ItemSettings.FreezeBombPlatformDuration - GameManager.ItemSettings.FreezeBombPlatformShakeStartTime);
			if (!(this == null))
			{
				asEntity.DestroyEntity();
			}
		}
	}

	private void OnDestroy()
	{
		if (shakingSoundInstance.isValid())
		{
			shakingSoundInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
		}
		if (!BNetworkManager.IsChangingSceneOrShuttingDown)
		{
			VfxManager.PlayPooledVfxLocalOnly(VfxType.FreezeBombPlatformBreak, base.transform.position, base.transform.rotation);
			RuntimeManager.PlayOneShot(GameManager.AudioSettings.FreezeBombPlatformBreakEvent, base.transform.position);
		}
	}

	private void OnMeshIndexChanged(int previousIndex, int currentIndex)
	{
		for (int i = 0; i < meshes.Length; i++)
		{
			bool active = i == meshIndex;
			meshes[i].SetActive(active);
			colliders[i].SetActive(active);
		}
	}

	private void OnIsShakingChanged(bool wasShaking, bool isShaking)
	{
		vfx.SetShaking(this.isShaking);
		if (shakingSoundInstance.isValid())
		{
			shakingSoundInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
		}
		if (isShaking)
		{
			shakingSoundInstance = RuntimeManager.CreateInstance(GameManager.AudioSettings.FreezeBombPlatformShakeLoopEvent);
			RuntimeManager.AttachInstanceToGameObject(shakingSoundInstance, base.gameObject);
			shakingSoundInstance.start();
			shakingSoundInstance.release();
		}
	}

	public FreezeBombPlatform()
	{
		_Mirror_SyncVarHookDelegate_meshIndex = OnMeshIndexChanged;
		_Mirror_SyncVarHookDelegate_isShaking = OnIsShakingChanged;
	}

	public override bool Weaved()
	{
		return true;
	}

	public override void SerializeSyncVars(NetworkWriter writer, bool forceAll)
	{
		base.SerializeSyncVars(writer, forceAll);
		if (forceAll)
		{
			writer.WriteVarInt(meshIndex);
			writer.WriteBool(isShaking);
			return;
		}
		writer.WriteVarULong(syncVarDirtyBits);
		if ((syncVarDirtyBits & 1L) != 0L)
		{
			writer.WriteVarInt(meshIndex);
		}
		if ((syncVarDirtyBits & 2L) != 0L)
		{
			writer.WriteBool(isShaking);
		}
	}

	public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
	{
		base.DeserializeSyncVars(reader, initialState);
		if (initialState)
		{
			GeneratedSyncVarDeserialize(ref meshIndex, _Mirror_SyncVarHookDelegate_meshIndex, reader.ReadVarInt());
			GeneratedSyncVarDeserialize(ref isShaking, _Mirror_SyncVarHookDelegate_isShaking, reader.ReadBool());
			return;
		}
		long num = (long)reader.ReadVarULong();
		if ((num & 1L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref meshIndex, _Mirror_SyncVarHookDelegate_meshIndex, reader.ReadVarInt());
		}
		if ((num & 2L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref isShaking, _Mirror_SyncVarHookDelegate_isShaking, reader.ReadBool());
		}
	}
}
