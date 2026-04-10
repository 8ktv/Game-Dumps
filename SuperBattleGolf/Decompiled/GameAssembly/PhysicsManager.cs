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
		Foliage,
		Hole,
		TerrainAddition,
		IceBlock
	}

	private enum RigidbodyType
	{
		Unregistered,
		Predicted
	}

	[SerializeField]
	private PhysicsSettings settings;

	private NativeHashSet<int> ballColliderIds;

	private readonly Dictionary<int, Terrain> terrainsByColliderId = new Dictionary<int, Terrain>();

	private readonly HashSet<int> foliageColliderIds = new HashSet<int>();

	private readonly HashSet<int> holeColliderIds = new HashSet<int>();

	private readonly Dictionary<int, TerrainLayer> terrainAdditionLayerPerColliderId = new Dictionary<int, TerrainLayer>();

	private readonly HashSet<int> iceBlockColliderIds = new HashSet<int>();

	private readonly Dictionary<Collider, JumpPad> jumpPadsByCollider = new Dictionary<Collider, JumpPad>();

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

	public static Dictionary<int, TerrainLayer> TerrainAdditionLayerPerColliderId
	{
		get
		{
			if (!SingletonBehaviour<PhysicsManager>.HasInstance)
			{
				return null;
			}
			return SingletonBehaviour<PhysicsManager>.Instance.terrainAdditionLayerPerColliderId;
		}
	}

	public static Dictionary<Collider, JumpPad> JumpPadsByCollider
	{
		get
		{
			if (!SingletonBehaviour<PhysicsManager>.HasInstance)
			{
				return null;
			}
			return SingletonBehaviour<PhysicsManager>.Instance.jumpPadsByCollider;
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
			TerrainCollider component = terrain.GetComponent<TerrainCollider>();
			component.hasModifiableContacts = true;
			int instanceID = component.GetInstanceID();
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

	public static void RegisterTerrainAddition(List<Collider> colliders, TerrainLayer terrainLayer)
	{
		if (SingletonBehaviour<PhysicsManager>.HasInstance)
		{
			SingletonBehaviour<PhysicsManager>.Instance.RegisterTerrainAdditionInternal(colliders, terrainLayer);
		}
	}

	public static void DeregisterTerrainAddition(List<Collider> colliders)
	{
		if (SingletonBehaviour<PhysicsManager>.HasInstance)
		{
			SingletonBehaviour<PhysicsManager>.Instance.DeregisterTerrainAdditionInternal(colliders);
		}
	}

	public static void RegisterIceBlockCollider(Collider collider)
	{
		if (SingletonBehaviour<PhysicsManager>.HasInstance)
		{
			SingletonBehaviour<PhysicsManager>.Instance.RegisterIceBlockColliderInternal(collider);
		}
	}

	public static void DeregisterIceBlockCollider(Collider collider)
	{
		if (SingletonBehaviour<PhysicsManager>.HasInstance)
		{
			SingletonBehaviour<PhysicsManager>.Instance.DeregisterIceBlockColliderInternal(collider);
		}
	}

	public static void RegisterJumpPad(JumpPad jumpPad)
	{
		if (SingletonBehaviour<PhysicsManager>.HasInstance)
		{
			SingletonBehaviour<PhysicsManager>.Instance.RegisterJumpPadInternal(jumpPad);
		}
	}

	public static void DeregisterJumpPad(JumpPad jumpPad)
	{
		if (SingletonBehaviour<PhysicsManager>.HasInstance)
		{
			SingletonBehaviour<PhysicsManager>.Instance.DeregisterJumpPadInternal(jumpPad);
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

	private void RegisterTerrainAdditionInternal(List<Collider> colliders, TerrainLayer terrainLayer)
	{
		foreach (Collider collider in colliders)
		{
			terrainAdditionLayerPerColliderId.Add(collider.GetInstanceID(), terrainLayer);
		}
	}

	private void DeregisterTerrainAdditionInternal(List<Collider> colliders)
	{
		foreach (Collider collider in colliders)
		{
			terrainAdditionLayerPerColliderId.Remove(collider.GetInstanceID());
		}
	}

	private void RegisterIceBlockColliderInternal(Collider collider)
	{
		iceBlockColliderIds.Add(collider.GetInstanceID());
	}

	private void DeregisterIceBlockColliderInternal(Collider collider)
	{
		iceBlockColliderIds.Remove(collider.GetInstanceID());
	}

	private void RegisterJumpPadInternal(JumpPad jumpPad)
	{
		jumpPadsByCollider.Add(jumpPad.Collider.Collider, jumpPad);
	}

	private void DeregisterJumpPadInternal(JumpPad jumpPad)
	{
		jumpPadsByCollider.Remove(jumpPad.Collider.Collider);
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
			RigidbodyType rigidbodyType;
			ColliderType firstColliderType = GetFirstColliderType(contactPair, out rigidbodyType);
			if (rigidbodyType == RigidbodyType.Predicted && GetSecondColliderType(contactPair, out var rigidbodyType2) == ColliderType.LocalPlayer)
			{
				ResolveLocalPlayerPredictedContactPair(contactPair, otherIsPredicted: false);
				continue;
			}
			if (firstColliderType == ColliderType.IceBlock)
			{
				ResolveContactPairWithIceBlock(contactPair);
				continue;
			}
			if (firstColliderType == ColliderType.Terrain)
			{
				ResolveContactPairWithTerrain(contactPair, GetSecondColliderType(contactPair, out rigidbodyType2));
				continue;
			}
			if (firstColliderType == ColliderType.TerrainAddition)
			{
				ResolveContactPairWithTerrainAddition(contactPair, contactPair.colliderInstanceID, GetSecondColliderType(contactPair, out rigidbodyType2));
				continue;
			}
			if (firstColliderType == ColliderType.Foliage)
			{
				rigidbodiesInFoliage.Add(contactPair.otherBodyInstanceID);
				IgnoreContactPair(contactPair);
				continue;
			}
			RigidbodyType rigidbodyType3;
			ColliderType colliderType = GetSecondColliderType(contactPair, out rigidbodyType3);
			if (rigidbodyType3 == RigidbodyType.Predicted && GetFirstColliderType(contactPair, out rigidbodyType2) == ColliderType.LocalPlayer)
			{
				ResolveLocalPlayerPredictedContactPair(contactPair, otherIsPredicted: true);
				continue;
			}
			switch (colliderType)
			{
			case ColliderType.IceBlock:
				ResolveContactPairWithIceBlock(contactPair);
				continue;
			case ColliderType.Terrain:
				ResolveContactPairWithTerrain(contactPair, firstColliderType);
				continue;
			case ColliderType.TerrainAddition:
				ResolveContactPairWithTerrainAddition(contactPair, contactPair.otherColliderInstanceID, firstColliderType);
				continue;
			case ColliderType.Foliage:
				rigidbodiesInFoliage.Add(contactPair.bodyInstanceID);
				IgnoreContactPair(contactPair);
				continue;
			}
			if (firstColliderType == ColliderType.Unregistered || colliderType == ColliderType.Unregistered)
			{
				continue;
			}
			switch (firstColliderType)
			{
			case ColliderType.Ball:
				if (colliderType == ColliderType.Hole)
				{
					ResolveBallHoleContactPair(contactPair);
				}
				break;
			case ColliderType.Hole:
				if (colliderType == ColliderType.Ball)
				{
					ResolveBallHoleContactPair(contactPair);
				}
				break;
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
		static void ApplyCollisionSettings(ModifiableContactPair modifiableContactPair, TerrainLayerSettings settings, ColliderType otherColliderType)
		{
			float staticFriction;
			float dynamicFriction;
			if (otherColliderType == ColliderType.LocalPlayer && settings.DoesOverridePlayerDiveDamping && GameManager.LocalPlayerMovement != null && GameManager.LocalPlayerMovement.DivingState != DivingState.None)
			{
				staticFriction = settings.PlayerOverrideDiveStaticFriction;
				dynamicFriction = settings.PlayerOverrideDiveDynamicFriction;
			}
			else
			{
				staticFriction = settings.StaticFriction;
				dynamicFriction = settings.DynamicFriction;
			}
			bool flag = otherColliderType == ColliderType.Ball;
			for (int j = 0; j < modifiableContactPair.contactCount; j++)
			{
				if (flag)
				{
					modifiableContactPair.SetBounciness(j, settings.Bounciness);
				}
				modifiableContactPair.SetStaticFriction(j, staticFriction);
				modifiableContactPair.SetDynamicFriction(j, dynamicFriction);
			}
		}
		ColliderType GetColliderType(int colliderId, int bodyId, out RigidbodyType reference)
		{
			reference = (predictedEntitiesPerRigidbodyId.ContainsKey(bodyId) ? RigidbodyType.Predicted : RigidbodyType.Unregistered);
			if (iceBlockColliderIds.Contains(colliderId))
			{
				return ColliderType.IceBlock;
			}
			if (bodyId == localPlayerRigidbodyId)
			{
				return ColliderType.LocalPlayer;
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
			if (terrainAdditionLayerPerColliderId.ContainsKey(colliderId))
			{
				return ColliderType.TerrainAddition;
			}
			return ColliderType.Unregistered;
		}
		ColliderType GetFirstColliderType(ModifiableContactPair modifiableContactPair, out RigidbodyType rigidbodyType4)
		{
			return GetColliderType(modifiableContactPair.colliderInstanceID, modifiableContactPair.bodyInstanceID, out rigidbodyType4);
		}
		ColliderType GetSecondColliderType(ModifiableContactPair modifiableContactPair, out RigidbodyType rigidbodyType4)
		{
			return GetColliderType(modifiableContactPair.otherColliderInstanceID, modifiableContactPair.otherBodyInstanceID, out rigidbodyType4);
		}
		static void IgnoreContactPair(ModifiableContactPair modifiableContactPair)
		{
			for (int j = 0; j < modifiableContactPair.contactCount; j++)
			{
				modifiableContactPair.IgnoreContact(j);
			}
		}
		void ResolveContactPairWithIceBlock(ModifiableContactPair modifiableContactPair)
		{
			for (int j = 0; j < modifiableContactPair.contactCount; j++)
			{
				modifiableContactPair.SetStaticFriction(j, settings.FreezeBombIceBlockStaticFriction);
				modifiableContactPair.SetDynamicFriction(j, settings.FreezeBombIceBlockDynamicFriction);
				modifiableContactPair.SetBounciness(j, settings.FreezeBombIceBlockBounciness);
			}
		}
		static void ResolveContactPairWithTerrain(ModifiableContactPair contactPair2, ColliderType otherColliderType)
		{
			int dominantLevelLayerIndexAtPoint = TerrainManager.GetDominantLevelLayerIndexAtPoint(contactPair2.GetPoint(0));
			ApplyCollisionSettings(contactPair2, TerrainManager.GetLevelLayerSettings(dominantLevelLayerIndexAtPoint), otherColliderType);
		}
		void ResolveContactPairWithTerrainAddition(ModifiableContactPair contactPair2, int terrainAdditionColliderId, ColliderType otherColliderType)
		{
			TerrainLayerSettings value2;
			if (!terrainAdditionLayerPerColliderId.TryGetValue(terrainAdditionColliderId, out var value))
			{
				Debug.LogError($"Could not find terrain layer for terrain addition collider ID {terrainAdditionColliderId}");
			}
			else if (!TerrainManager.Settings.LayerSettings.TryGetValue(value, out value2))
			{
				Debug.LogError($"Could not find terrain layer settings for terrain addition collider ID {terrainAdditionColliderId} with assigned layer {value}");
			}
			else
			{
				ApplyCollisionSettings(contactPair2, value2, otherColliderType);
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
