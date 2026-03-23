using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class PhysicsManager : SingletonBehaviour<PhysicsManager>, IFixedBUpdateCallback, IAnyBUpdateCallback
{
	private struct PredictedContact
	{
		public int bodyId;

		public Vector3 localPoint;

		public Vector3 impulse;

		public PredictedContact(int bodyId, Vector3 localPoint, Vector3 impulse)
		{
			this.bodyId = bodyId;
			this.localPoint = localPoint;
			this.impulse = impulse;
		}
	}

	private enum ColliderType
	{
		Unregistered,
		LocalPlayer,
		Ball,
		Terrain,
		Predicted,
		Foliage,
		Hole,
		TerrainEdgeDetail
	}

	[SerializeField]
	private PhysicsSettings settings;

	private NativeHashSet<int> ballColliderIds;

	private readonly Dictionary<int, Terrain> terrainsByColliderId = new Dictionary<int, Terrain>();

	private readonly HashSet<int> foliageColliderIds = new HashSet<int>();

	private readonly HashSet<int> holeColliderIds = new HashSet<int>();

	private readonly Dictionary<int, TerrainLayer> terrainEdgeDetailsLayerPerColliderId = new Dictionary<int, TerrainLayer>();

	private int localPlayerRigidbodyId;

	private readonly Dictionary<int, Entity> predictedEntitiesPerRigidbodyId = new Dictionary<int, Entity>();

	private readonly HashSet<int> rigidbodiesInFoliage = new HashSet<int>();

	public static PhysicsSettings Settings
	{
		get
		{
			if (!SingletonBehaviour<PhysicsManager>.HasInstance)
			{
				return null;
			}
			return SingletonBehaviour<PhysicsManager>.Instance.settings;
		}
	}

	protected override void Awake()
	{
		base.Awake();
		BUpdate.RegisterCallback(this);
		ballColliderIds = new NativeHashSet<int>(16, Allocator.Persistent);
		Physics.ContactModifyEvent += ModifyRegularContacts;
		Physics.ContactModifyEventCCD += ModifyCcdContacts;
	}

	private void Start()
	{
		Terrain[] terrains = TerrainManager.Terrains;
		foreach (Terrain terrain in terrains)
		{
			int instanceID = terrain.GetComponent<TerrainCollider>().GetInstanceID();
			terrainsByColliderId.Add(instanceID, terrain);
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		BUpdate.DeregisterCallback(this);
		if (ballColliderIds.IsCreated)
		{
			ballColliderIds.Dispose();
		}
		Physics.ContactModifyEvent -= ModifyRegularContacts;
		Physics.ContactModifyEventCCD -= ModifyCcdContacts;
	}

	public void OnFixedBUpdate()
	{
		foreach (int item in rigidbodiesInFoliage)
		{
			Object obj = Resources.InstanceIDToObject(item);
			if (!(obj == null) && obj is Rigidbody rigidbody && rigidbody.TryGetComponent<Entity>(out var component))
			{
				component.InformInFoliage();
			}
		}
		rigidbodiesInFoliage.Clear();
	}

	public static void RegisterBallColliderId(int colliderId)
	{
		if (SingletonBehaviour<PhysicsManager>.HasInstance)
		{
			SingletonBehaviour<PhysicsManager>.Instance.RegisterBallColliderIdInternal(colliderId);
		}
	}

	public static void DeregisterBallColliderId(int colliderId)
	{
		if (SingletonBehaviour<PhysicsManager>.HasInstance)
		{
			SingletonBehaviour<PhysicsManager>.Instance.DeregisterBallColliderIdInternal(colliderId);
		}
	}

	public static void RegisterLocalPlayer(PlayerInfo player)
	{
		if (SingletonBehaviour<PhysicsManager>.HasInstance)
		{
			SingletonBehaviour<PhysicsManager>.Instance.RegisterLocalPlayerInternal(player);
		}
	}

	public static void DeregisterLocalPlayer()
	{
		if (SingletonBehaviour<PhysicsManager>.HasInstance)
		{
			SingletonBehaviour<PhysicsManager>.Instance.DeregisterLocalPlayerInternal();
		}
	}

	public static void RegisterPredictedEntity(Entity entity)
	{
		if (SingletonBehaviour<PhysicsManager>.HasInstance)
		{
			SingletonBehaviour<PhysicsManager>.Instance.RegisterPredictedEntityInternal(entity);
		}
	}

	public static void DeregisterPredictedEntity(Entity entity)
	{
		if (SingletonBehaviour<PhysicsManager>.HasInstance)
		{
			SingletonBehaviour<PhysicsManager>.Instance.DeregisterPredictedEntityInternal(entity);
		}
	}

	public static void RegisterFoliageColliders(Collider[] colliders)
	{
		if (SingletonBehaviour<PhysicsManager>.HasInstance)
		{
			SingletonBehaviour<PhysicsManager>.Instance.RegisterFoliageCollidersInternal(colliders);
		}
	}

	public static void DeregisterFoliageColliders(Collider[] colliders)
	{
		if (SingletonBehaviour<PhysicsManager>.HasInstance)
		{
			SingletonBehaviour<PhysicsManager>.Instance.DeregisterFoliageCollidersInternal(colliders);
		}
	}

	public static void RegisterHoleCollider(Collider collider)
	{
		if (SingletonBehaviour<PhysicsManager>.HasInstance)
		{
			SingletonBehaviour<PhysicsManager>.Instance.RegisterHoleColliderInternal(collider);
		}
	}

	public static void DeregisterHoleCollider(Collider collider)
	{
		if (SingletonBehaviour<PhysicsManager>.HasInstance)
		{
			SingletonBehaviour<PhysicsManager>.Instance.DeregisterHoleColliderInternal(collider);
		}
	}

	public static void RegisterTerrainEdgeDetail(List<Collider> colliders, TerrainLayer terrainLayer)
	{
		if (SingletonBehaviour<PhysicsManager>.HasInstance)
		{
			SingletonBehaviour<PhysicsManager>.Instance.RegisterTerrainEdgeDetailInternal(colliders, terrainLayer);
		}
	}

	public static void DeregisterTerrainEdgeDetail(List<Collider> colliders)
	{
		if (SingletonBehaviour<PhysicsManager>.HasInstance)
		{
			SingletonBehaviour<PhysicsManager>.Instance.DeregisterTerrainEdgeDetailInternal(colliders);
		}
	}

	private void RegisterBallColliderIdInternal(int colliderId)
	{
		ballColliderIds.Add(colliderId);
	}

	private void DeregisterBallColliderIdInternal(int colliderId)
	{
		if (!BNetworkManager.IsChangingSceneOrShuttingDown)
		{
			ballColliderIds.Remove(colliderId);
		}
	}

	private void RegisterLocalPlayerInternal(PlayerInfo player)
	{
		localPlayerRigidbodyId = player.Rigidbody.GetInstanceID();
	}

	private void DeregisterLocalPlayerInternal()
	{
		if (!BNetworkManager.IsChangingSceneOrShuttingDown)
		{
			localPlayerRigidbodyId = 0;
		}
	}

	private void RegisterPredictedEntityInternal(Entity entity)
	{
		if (!BNetworkManager.IsChangingSceneOrShuttingDown)
		{
			predictedEntitiesPerRigidbodyId.Add(entity.Rigidbody.GetInstanceID(), entity);
		}
	}

	private void DeregisterPredictedEntityInternal(Entity entity)
	{
		predictedEntitiesPerRigidbodyId.Remove(entity.Rigidbody.GetInstanceID());
	}

	private void RegisterFoliageCollidersInternal(Collider[] colliders)
	{
		foreach (Collider collider in colliders)
		{
			foliageColliderIds.Add(collider.GetInstanceID());
		}
	}

	private void DeregisterFoliageCollidersInternal(Collider[] colliders)
	{
		foreach (Collider collider in colliders)
		{
			foliageColliderIds.Remove(collider.GetInstanceID());
		}
	}

	private void RegisterHoleColliderInternal(Collider collider)
	{
		holeColliderIds.Add(collider.GetInstanceID());
	}

	private void DeregisterHoleColliderInternal(Collider collider)
	{
		holeColliderIds.Remove(collider.GetInstanceID());
	}

	private void RegisterTerrainEdgeDetailInternal(List<Collider> colliders, TerrainLayer terrainLayer)
	{
		foreach (Collider collider in colliders)
		{
			terrainEdgeDetailsLayerPerColliderId.Add(collider.GetInstanceID(), terrainLayer);
		}
	}

	private void DeregisterTerrainEdgeDetailInternal(List<Collider> colliders)
	{
		foreach (Collider collider in colliders)
		{
			terrainEdgeDetailsLayerPerColliderId.Remove(collider.GetInstanceID());
		}
	}

	private void ModifyRegularContacts(PhysicsScene scene, NativeArray<ModifiableContactPair> contactPairs)
	{
		ModifyContactsInternal(contactPairs);
	}

	private void ModifyCcdContacts(PhysicsScene scene, NativeArray<ModifiableContactPair> contactPairs)
	{
		ModifyContactsInternal(contactPairs);
	}

	private void ModifyContactsInternal(NativeArray<ModifiableContactPair> contactPairs)
	{
		for (int i = 0; i < contactPairs.Length; i++)
		{
			ModifiableContactPair contactPair = contactPairs[i];
			if (contactPair.contactCount <= 0)
			{
				continue;
			}
			ColliderType firstColliderType = GetColliderType(contactPair.colliderInstanceID, contactPair.bodyInstanceID);
			if (firstColliderType == ColliderType.Foliage)
			{
				rigidbodiesInFoliage.Add(contactPair.otherBodyInstanceID);
				IgnoreContactPair(contactPair);
				continue;
			}
			ColliderType colliderType = GetColliderType(contactPair.otherColliderInstanceID, contactPair.otherBodyInstanceID);
			if (colliderType == ColliderType.Foliage)
			{
				rigidbodiesInFoliage.Add(contactPair.bodyInstanceID);
				IgnoreContactPair(contactPair);
			}
			else
			{
				if (firstColliderType == ColliderType.Unregistered || colliderType == ColliderType.Unregistered)
				{
					continue;
				}
				switch (firstColliderType)
				{
				case ColliderType.Ball:
					switch (colliderType)
					{
					case ColliderType.Terrain:
						ResolveBallTerrainContactPair(contactPair);
						break;
					case ColliderType.Hole:
						ResolveBallHoleContactPair(contactPair);
						break;
					case ColliderType.TerrainEdgeDetail:
						ResolveBallTerrainEdgeDetailContactPair(contactPair, contactPair.otherColliderInstanceID);
						break;
					}
					break;
				case ColliderType.Terrain:
					if (colliderType == ColliderType.Ball)
					{
						ResolveBallTerrainContactPair(contactPair);
					}
					break;
				case ColliderType.TerrainEdgeDetail:
					if (colliderType == ColliderType.Ball)
					{
						ResolveBallTerrainEdgeDetailContactPair(contactPair, contactPair.colliderInstanceID);
					}
					break;
				case ColliderType.LocalPlayer:
					if (colliderType == ColliderType.Predicted)
					{
						ResolveLocalPlayerPredictedContactPair(contactPair, otherIsPredicted: true);
					}
					break;
				case ColliderType.Hole:
					if (colliderType == ColliderType.Ball)
					{
						ResolveBallHoleContactPair(contactPair);
					}
					break;
				case ColliderType.Predicted:
					if (colliderType == ColliderType.LocalPlayer)
					{
						ResolveLocalPlayerPredictedContactPair(contactPair, otherIsPredicted: false);
					}
					break;
				}
			}
			void ResolveBallHoleContactPair(ModifiableContactPair contactPair2)
			{
				Vector3 point = contactPair2.GetPoint(0);
				Vector3 normal = contactPair2.GetNormal(0);
				float num = ((firstColliderType != ColliderType.Hole) ? Vector3.Dot((point - contactPair2.otherPosition).normalized, normal) : Vector3.Dot((point - contactPair2.position).normalized, -normal));
				if (num > 0.017452406f)
				{
					IgnoreContactPair(contactPair2);
				}
			}
		}
		static void ApplyCollisionSettings(ModifiableContactPair modifiableContactPair, TerrainLayerSettings settings)
		{
			for (int j = 0; j < modifiableContactPair.contactCount; j++)
			{
				modifiableContactPair.SetBounciness(j, settings.Bounciness);
				modifiableContactPair.SetStaticFriction(j, settings.StaticFriction);
				modifiableContactPair.SetDynamicFriction(j, settings.DynamicFriction);
			}
		}
		ColliderType GetColliderType(int colliderId, int bodyId)
		{
			if (bodyId == localPlayerRigidbodyId)
			{
				return ColliderType.LocalPlayer;
			}
			if (predictedEntitiesPerRigidbodyId.ContainsKey(bodyId))
			{
				return ColliderType.Predicted;
			}
			if (ballColliderIds.Contains(colliderId))
			{
				return ColliderType.Ball;
			}
			if (terrainsByColliderId.ContainsKey(colliderId))
			{
				return ColliderType.Terrain;
			}
			if (foliageColliderIds.Contains(colliderId))
			{
				return ColliderType.Foliage;
			}
			if (holeColliderIds.Contains(colliderId))
			{
				return ColliderType.Hole;
			}
			if (terrainEdgeDetailsLayerPerColliderId.ContainsKey(colliderId))
			{
				return ColliderType.TerrainEdgeDetail;
			}
			return ColliderType.Unregistered;
		}
		static void IgnoreContactPair(ModifiableContactPair modifiableContactPair)
		{
			for (int j = 0; j < modifiableContactPair.contactCount; j++)
			{
				modifiableContactPair.IgnoreContact(j);
			}
		}
		static void ResolveBallTerrainContactPair(ModifiableContactPair contactPair2)
		{
			int dominantLevelLayerIndexAtPoint = TerrainManager.GetDominantLevelLayerIndexAtPoint(contactPair2.GetPoint(0));
			ApplyCollisionSettings(contactPair2, TerrainManager.GetLevelLayerSettings(dominantLevelLayerIndexAtPoint));
		}
		void ResolveBallTerrainEdgeDetailContactPair(ModifiableContactPair contactPair2, int terrainEdgeDetailColliderId)
		{
			TerrainLayerSettings value2;
			if (!terrainEdgeDetailsLayerPerColliderId.TryGetValue(terrainEdgeDetailColliderId, out var value))
			{
				Debug.LogError($"Could not find terrain layer for terrain edge detail collider ID {terrainEdgeDetailColliderId}");
			}
			else if (!TerrainManager.Settings.LayerSettings.TryGetValue(value, out value2))
			{
				Debug.LogError($"Could not find terrain layer settings for terrain edge detail collider ID {terrainEdgeDetailColliderId} with assigned layer {value}");
			}
			else
			{
				ApplyCollisionSettings(contactPair2, value2);
			}
		}
		static void ResolveLocalPlayerPredictedContactPair(ModifiableContactPair modifiableContactPair, bool otherIsPredicted)
		{
			ModifiableMassProperties massProperties = modifiableContactPair.massProperties;
			if (otherIsPredicted)
			{
				massProperties.otherInverseMassScale = 0f;
				massProperties.otherInverseInertiaScale = 0f;
			}
			else
			{
				massProperties.inverseMassScale = 0f;
				massProperties.inverseInertiaScale = 0f;
			}
			modifiableContactPair.massProperties = massProperties;
		}
	}
}
