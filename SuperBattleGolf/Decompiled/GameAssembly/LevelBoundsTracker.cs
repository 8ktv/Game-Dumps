using System;
using Cysharp.Threading.Tasks;
using Mirror;
using UnityEngine;

public class LevelBoundsTracker : MonoBehaviour
{
	[SerializeField]
	private LevelBoundsTrackerSettings settings;

	[SerializeField]
	private LevelBoundsTrackerNetworker networker;

	private Entity asEntity;

	private Vector3 initialPosition;

	private Quaternion initialRotation;

	private double serverAutomaticDisappearanceTimestamp = double.MinValue;

	public BoundsState AuthoritativeBoundsState
	{
		get
		{
			if (networker == null)
			{
				Debug.LogError("Cannot get authoritative bounds state on a non-networked tracker", base.gameObject);
				return BoundsState.InBounds;
			}
			return networker.BoundsState;
		}
	}

	public BoundsState LocalBoundsState { get; private set; }

	public float CurrentOutOfBoundsHazardWorldHeightLocalOnly { get; private set; }

	public SecondaryOutOfBoundsHazard CurrentSecondaryHazardLocalOnly { get; private set; }

	public bool AuthoritativeIsOnGreen
	{
		get
		{
			if (networker != null)
			{
				return networker.IsOnGreen;
			}
			return false;
		}
	}

	public bool LocalIsOnGreen { get; private set; }

	public double OutOfBoundsTimestamp { get; private set; } = double.MinValue;

	public LevelBoundsTrackerSettings Settings => settings;

	public event Action<BoundsState, BoundsState> AuthoritativeBoundsStateChanged;

	public event Action AuthoritativeIsOnGreenChanged;

	public event Action<BoundsState, BoundsState> LocalBoundsStateChanged;

	public event Action LocalIsOnGreenChanged;

	private void Awake()
	{
		asEntity = GetComponent<Entity>();
		base.transform.GetPositionAndRotation(out initialPosition, out initialRotation);
		if (networker != null)
		{
			networker.BoundsStateChanged += OnAuthoritativeBoundsStateChanged;
			networker.IsOnGreenChanged += OnAuthoritativeIsOnGreenChanged;
		}
	}

	private void Start()
	{
		if (!settings.ServerOnly || NetworkServer.active)
		{
			if (asEntity != null && asEntity.IsPlayer)
			{
				RegisterPlayerWhenReady();
			}
			else
			{
				BoundsManager.RegisterLevelBoundsTracker(this);
			}
		}
		bool IsPlayerReady()
		{
			if (!asEntity.PlayerInfo.AsGolfer.IsInitialized)
			{
				return false;
			}
			if (base.transform.position.Approximately(BNetworkManager.playerInitialTempPosition, 100f))
			{
				return false;
			}
			return true;
		}
		async void RegisterPlayerWhenReady()
		{
			while (!IsPlayerReady())
			{
				await UniTask.Yield();
				if (this == null)
				{
					return;
				}
			}
			BoundsManager.RegisterLevelBoundsTracker(this);
		}
	}

	private void OnDestroy()
	{
		BoundsManager.DeregisterLevelBoundsTracker(this);
		if (networker != null)
		{
			networker.BoundsStateChanged -= OnAuthoritativeBoundsStateChanged;
			networker.IsOnGreenChanged -= OnAuthoritativeIsOnGreenChanged;
		}
	}

	public void InformLevelBoundsStateChanged(BoundsState boundsState)
	{
		BoundsState localBoundsState = LocalBoundsState;
		LocalBoundsState = boundsState;
		if (NetworkServer.active && networker != null)
		{
			networker.SetBoundsState(boundsState);
		}
		if (LocalBoundsState != localBoundsState)
		{
			this.LocalBoundsStateChanged?.Invoke(localBoundsState, LocalBoundsState);
		}
	}

	public void InformOutOfBoundsHazardHeightChanged(float hazardHeight)
	{
		CurrentOutOfBoundsHazardWorldHeightLocalOnly = hazardHeight;
	}

	public void InformSecondaryOutOfBoundsHazardChanged(int secondaryHazardInstanceId)
	{
		CurrentSecondaryHazardLocalOnly = (SecondaryOutOfBoundsHazard)Resources.EntityIdToObject(secondaryHazardInstanceId);
	}

	public void InformGreenBoundsStateChanged(bool isOnGreen)
	{
		bool localIsOnGreen = LocalIsOnGreen;
		LocalIsOnGreen = isOnGreen;
		if (NetworkServer.active && networker != null)
		{
			networker.SetIsOnGreen(isOnGreen);
		}
		if (LocalIsOnGreen != localIsOnGreen)
		{
			this.LocalIsOnGreenChanged?.Invoke();
		}
	}

	public EliminationReason GetPotentialOutOfBoundsHazardEliminationReason()
	{
		if (AuthoritativeBoundsState.HasState(BoundsState.InMainOutOfBoundsHazard))
		{
			return MainOutOfBoundsHazard.GetEliminationReason();
		}
		if (CurrentSecondaryHazardLocalOnly != null)
		{
			return CurrentSecondaryHazardLocalOnly.GetEliminationReason();
		}
		return EliminationReason.None;
	}

	public void CmdInformTeleportedIntoBounds(Vector3 position, Quaternion rotation)
	{
		if (!(networker == null))
		{
			networker.CmdInformTeleportedIntoBounds(position, rotation);
		}
	}

	[Server]
	public void InformTeleportedIntoBounds(Vector3 position, Quaternion rotation)
	{
		if (!NetworkServer.active)
		{
			Debug.LogWarning("[Server] function 'System.Void LevelBoundsTracker::InformTeleportedIntoBounds(UnityEngine.Vector3,UnityEngine.Quaternion)' called when server was not active");
			return;
		}
		base.transform.SetPositionAndRotation(position, rotation);
		if (asEntity.HasRigidbody)
		{
			asEntity.Rigidbody.position = position;
			asEntity.Rigidbody.rotation = rotation;
		}
		OnServerReturnedToBounds();
	}

	public bool IsInOutOfBoundsHazardAccurate()
	{
		if (AuthoritativeBoundsState.IsInOutOfBoundsHazard())
		{
			return true;
		}
		return base.transform.TransformPoint(settings.OutOfBoundsHazardSubmersionLocalPoint).y + settings.OutOfBoundsHazardSubmersionWorldVerticalOffset < CurrentOutOfBoundsHazardWorldHeightLocalOnly;
	}

	public bool IsInOrOverOutOfBoundsHazard()
	{
		if (AuthoritativeBoundsState.IsInOutOfBoundsHazard())
		{
			return true;
		}
		if (TerrainManager.GetWorldHeightAtPoint(base.transform.TransformPoint(settings.OutOfBoundsHazardSubmersionLocalPoint)) <= CurrentOutOfBoundsHazardWorldHeightLocalOnly)
		{
			return true;
		}
		return false;
	}

	[Server]
	public void ServerReturnToBounds()
	{
		if (!NetworkServer.active)
		{
			Debug.LogWarning("[Server] function 'System.Void LevelBoundsTracker::ServerReturnToBounds()' called when server was not active");
			return;
		}
		Vector3 directionIntoLevel;
		Vector3 nearestPointOnReturnSplines = BoundsManager.GetNearestPointOnReturnSplines(base.transform.position, out directionIntoLevel);
		nearestPointOnReturnSplines.y = TerrainManager.GetWorldHeightAtPoint(nearestPointOnReturnSplines) + settings.AutomaticReturnToBoundsVerticalOffset;
		Quaternion rotation = Quaternion.LookRotation(directionIntoLevel);
		if (asEntity.IsPredicted && networker != null)
		{
			foreach (NetworkConnectionToClient value in NetworkServer.connections.Values)
			{
				if (value != NetworkServer.localConnection)
				{
					networker.RpcReturnToBounds(value, nearestPointOnReturnSplines, rotation);
				}
			}
		}
		ReturnToBoundsInternal(nearestPointOnReturnSplines, rotation);
	}

	[Server]
	public void ServerReturnToBounds(Vector3 explicitPosition, Quaternion explicitRotation)
	{
		if (!NetworkServer.active)
		{
			Debug.LogWarning("[Server] function 'System.Void LevelBoundsTracker::ServerReturnToBounds(UnityEngine.Vector3,UnityEngine.Quaternion)' called when server was not active");
			return;
		}
		if (asEntity.IsPredicted && networker != null)
		{
			foreach (NetworkConnectionToClient value in NetworkServer.connections.Values)
			{
				if (value != NetworkServer.localConnection)
				{
					networker.RpcReturnToBounds(value, explicitPosition, explicitRotation);
				}
			}
		}
		ReturnToBoundsInternal(explicitPosition, explicitRotation);
	}

	public bool IsInWaterLocalOnly(float verticalOffsetFactor = 1f)
	{
		if (CurrentSecondaryHazardLocalOnly == null)
		{
			if (MainOutOfBoundsHazard.Type != OutOfBoundsHazard.Water)
			{
				return false;
			}
		}
		else if (CurrentSecondaryHazardLocalOnly.Type != OutOfBoundsHazard.Water)
		{
			return false;
		}
		return base.transform.TransformPoint(settings.OutOfBoundsHazardSubmersionLocalPoint).y + settings.OutOfBoundsHazardSubmersionWorldVerticalOffset * verticalOffsetFactor < CurrentOutOfBoundsHazardWorldHeightLocalOnly;
	}

	public void ReturnToBoundsInternal(Vector3 position, Quaternion rotation)
	{
		if (asEntity.IsPlayer)
		{
			Debug.LogError("Default return to bounds should not be initiated for players, as they require special client-side logic");
			return;
		}
		base.transform.SetPositionAndRotation(position, rotation);
		if (asEntity.Rigidbody != null)
		{
			asEntity.Rigidbody.linearVelocity = Vector3.zero;
			asEntity.Rigidbody.angularVelocity = Vector3.zero;
			asEntity.Rigidbody.position = position;
			asEntity.Rigidbody.rotation = rotation;
		}
		if (NetworkServer.active)
		{
			OnServerReturnedToBounds();
		}
	}

	private void OnServerReturnedToBounds()
	{
		InformLevelBoundsStateChanged(BoundsState.InBounds);
		BoundsManager.SetLevelBoundsState(this, LocalBoundsState);
		if (asEntity.IsHittable)
		{
			asEntity.AsHittable.ServerStopBeingSwingProjectile();
		}
	}

	private void OnAuthoritativeBoundsStateChanged(BoundsState previousState, BoundsState currentState)
	{
		if (NetworkServer.active)
		{
			OnServerAuthoritativeBoundsStateChanged();
		}
		if (previousState.HasState(BoundsState.OutOfBounds) != currentState.HasState(BoundsState.OutOfBounds))
		{
			OutOfBoundsTimestamp = Time.timeAsDouble;
		}
		this.AuthoritativeBoundsStateChanged?.Invoke(previousState, currentState);
		void OnServerAuthoritativeBoundsStateChanged()
		{
			if (currentState != BoundsState.InBounds && currentState != BoundsState.OverSecondaryOutOfBoundsHazard)
			{
				switch (SingletonBehaviour<DrivingRangeManager>.HasInstance ? settings.DrivingRangeAutomaticOutOfBoundsBehaviour : settings.MatchAutomaticOutOfBoundsBehaviour)
				{
				case AutomaticOutOfBoundsBehaviour.Destroy:
					PlayAutomaticDisappearanceVfxForAllClients();
					NetworkServer.Destroy(base.gameObject);
					break;
				case AutomaticOutOfBoundsBehaviour.Return:
					if (asEntity.IsPlayer)
					{
						asEntity.PlayerInfo.Movement.ReturnToBounds();
						OnServerReturnedToBounds();
					}
					else
					{
						PlayAutomaticDisappearanceVfxForAllClients();
						if (settings.AutomaticReturnToInitialPosition)
						{
							ServerReturnToBounds(initialPosition + settings.AutomaticReturnToBoundsVerticalOffset * Vector3.up, initialRotation);
						}
						else
						{
							ServerReturnToBounds();
						}
						PlayAutomaticReturnToBoundsVfxForAllClients();
					}
					break;
				}
			}
		}
		void PlayAutomaticDisappearanceVfxForAllClients()
		{
			if (!(BMath.GetTimeSince(serverAutomaticDisappearanceTimestamp) < 2f))
			{
				serverAutomaticDisappearanceTimestamp = Time.timeAsDouble;
				VfxType vfxType = (AuthoritativeBoundsState.HasState(BoundsState.InMainOutOfBoundsHazard) ? (MainOutOfBoundsHazard.Type switch
				{
					OutOfBoundsHazard.Water => asEntity.IsGolfCart ? VfxType.WaterGolfCartOutOfBounds : VfxType.WaterItemOutOfBounds, 
					OutOfBoundsHazard.Fog => asEntity.IsGolfCart ? VfxType.FogGolfCartOutOfBounds : VfxType.FogItemOutOfBounds, 
					_ => VfxType.None, 
				}) : ((!AuthoritativeBoundsState.HasState(BoundsState.InSecondaryOutOfBoundsHazard)) ? VfxType.BoundaryOutOfBoundsSparkle : (CurrentSecondaryHazardLocalOnly.Type switch
				{
					OutOfBoundsHazard.Water => asEntity.IsGolfCart ? VfxType.WaterGolfCartOutOfBounds : VfxType.WaterItemOutOfBounds, 
					OutOfBoundsHazard.Fog => asEntity.IsGolfCart ? VfxType.FogGolfCartOutOfBounds : VfxType.FogItemOutOfBounds, 
					_ => VfxType.None, 
				})));
				if (vfxType != VfxType.None)
				{
					Vector3 vector = (asEntity.HasRigidbody ? asEntity.Rigidbody.worldCenterOfMass : base.transform.position);
					vector.y = CurrentOutOfBoundsHazardWorldHeightLocalOnly;
					VfxManager.ServerPlayPooledVfxForAllClients(vfxType, vector, Quaternion.identity);
					if (asEntity.IsGolfCart)
					{
						if (vfxType == VfxType.WaterGolfCartOutOfBounds)
						{
							asEntity.AsGolfCart.ServerPlayWaterSplashForAllClients(vector);
						}
					}
					else if (asEntity.IsItem && vfxType == VfxType.WaterItemOutOfBounds)
					{
						asEntity.AsItem.ServerPlayWaterSplashForAllClients(vector);
					}
				}
			}
		}
		void PlayAutomaticReturnToBoundsVfxForAllClients()
		{
			Vector3 position = (asEntity.HasRigidbody ? asEntity.Rigidbody.worldCenterOfMass : base.transform.position);
			VfxManager.ServerPlayPooledVfxForAllClients(VfxType.BoundaryOutOfBoundsSparkle, position, Quaternion.identity);
		}
	}

	private void OnAuthoritativeIsOnGreenChanged()
	{
		this.AuthoritativeIsOnGreenChanged?.Invoke();
	}

	private void OnDrawGizmosSelected()
	{
		if (!(settings == null))
		{
			Color blue = Color.blue;
			blue.a = 0.3f;
			Gizmos.color = blue;
			Gizmos.DrawSphere(base.transform.TransformPoint(settings.OutOfBoundsHazardSubmersionLocalPoint) + settings.OutOfBoundsHazardSubmersionWorldVerticalOffset * Vector3.up, 0.05f);
		}
	}
}
