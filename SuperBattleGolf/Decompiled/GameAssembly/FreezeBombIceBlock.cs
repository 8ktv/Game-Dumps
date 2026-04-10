using System.Collections.Generic;
using UnityEngine;

public class FreezeBombIceBlock : MonoBehaviour
{
	[SerializeField]
	private BoxCollider collider;

	[SerializeField]
	private FrozenEntityVfx vfx;

	private Entity parentEntity;

	private void Awake()
	{
		collider.hasModifiableContacts = true;
	}

	private void OnDestroy()
	{
		PhysicsManager.DeregisterIceBlockCollider(collider);
	}

	public void Initialize(Hittable parentHittable)
	{
		PhysicsManager.RegisterIceBlockCollider(collider);
		parentEntity = parentHittable.AsEntity;
		base.transform.SetParent(parentHittable.transform);
		base.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
		UpdateScale();
		if (!(parentEntity != null) || parentEntity.TemporarilyIgnoredEntities == null)
		{
			return;
		}
		foreach (Entity temporarilyIgnoredEntity in parentEntity.TemporarilyIgnoredEntities)
		{
			IEnumerable<Collider> enumerable = ((!temporarilyIgnoredEntity.HasRigidbody) ? ((IEnumerable<Collider>)temporarilyIgnoredEntity.GetComponentsInChildren<Collider>(includeInactive: true)) : ((IEnumerable<Collider>)temporarilyIgnoredEntity.Rigidbody.GetAttachedColliders()));
			foreach (Collider item in enumerable)
			{
				Physics.IgnoreCollision(collider, item, ignore: true);
			}
		}
		void SetScale(Vector3 scale)
		{
			collider.transform.localScale = scale;
			vfx.SetScale(scale);
		}
		void UpdateScale()
		{
			if (!parentHittable.ItemSettings.AutoCalculateFreezeBombIceBlock)
			{
				base.transform.localPosition = parentHittable.ItemSettings.FreezeBombIceBlockLocalCenter;
				SetScale(parentHittable.ItemSettings.FreezeBombIceBlockSize);
			}
			else
			{
				Bounds bounds = default(Bounds);
				bool flag = false;
				MeshRenderer[] componentsInChildren = parentHittable.GetComponentsInChildren<MeshRenderer>();
				foreach (MeshRenderer meshRenderer in componentsInChildren)
				{
					if (!meshRenderer.transform.IsChildOf(base.transform))
					{
						Bounds localBounds = meshRenderer.localBounds;
						localBounds.size = Vector3.Scale(localBounds.size, meshRenderer.transform.localScale);
						if (!flag)
						{
							bounds = localBounds;
							flag = true;
						}
						else
						{
							bounds.Encapsulate(localBounds);
						}
					}
				}
				bounds.size += parentHittable.ItemSettings.FreezeBombIceBlockMargins * 2f;
				base.transform.localPosition = bounds.center;
				SetScale(bounds.size);
			}
		}
	}

	public void PlayBreakVfxLocalOnly()
	{
		if (!BNetworkManager.IsChangingSceneOrShuttingDown)
		{
			VfxManager.PlayPooledVfxLocalOnly(VfxType.IceFrozenEntityBreak, base.transform.position, base.transform.rotation, collider.transform.localScale);
		}
	}

	public void InformReturnedToPool()
	{
		PhysicsManager.DeregisterIceBlockCollider(collider);
		if (BNetworkManager.IsChangingSceneOrShuttingDown)
		{
			return;
		}
		if (parentEntity != null && parentEntity.TemporarilyIgnoredEntities != null)
		{
			foreach (Entity temporarilyIgnoredEntity in parentEntity.TemporarilyIgnoredEntities)
			{
				IEnumerable<Collider> enumerable = ((!temporarilyIgnoredEntity.HasRigidbody) ? ((IEnumerable<Collider>)temporarilyIgnoredEntity.GetComponentsInChildren<Collider>(includeInactive: true)) : ((IEnumerable<Collider>)temporarilyIgnoredEntity.Rigidbody.GetAttachedColliders()));
				foreach (Collider item in enumerable)
				{
					Physics.IgnoreCollision(collider, item, ignore: false);
				}
			}
		}
		collider.includeLayers = 0;
		collider.excludeLayers = 0;
		parentEntity = null;
	}
}
