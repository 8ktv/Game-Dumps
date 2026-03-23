#define DEBUG_DRAW
using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractableTargeter : MonoBehaviour
{
	private struct InteractableHit
	{
		public IInteractable interactable;

		public bool isTrigger;

		public float distance;

		public float dotProduct;

		public bool isInSearchDirection;

		public static InteractableHit Empty = new InteractableHit(null, isTrigger: false, float.PositiveInfinity, float.NegativeInfinity, isInSearchDirection: false);

		public InteractableHit(IInteractable interactable, bool isTrigger, float distance, float dotProduct, bool isInSearchDirection)
		{
			this.interactable = interactable;
			this.isTrigger = isTrigger;
			this.distance = distance;
			this.dotProduct = dotProduct;
			this.isInSearchDirection = isInSearchDirection;
		}

		public readonly InteractableHit CompareAgainst(InteractableHit other)
		{
			if (IsBetterThan(other))
			{
				return this;
			}
			return other;
		}

		public readonly bool IsBetterThan(InteractableHit other)
		{
			if (isInSearchDirection && !other.isInSearchDirection)
			{
				return true;
			}
			if (!isInSearchDirection && other.isInSearchDirection)
			{
				return false;
			}
			if (interactable == null)
			{
				return false;
			}
			if (other.interactable == null)
			{
				return true;
			}
			if (isTrigger != other.isTrigger)
			{
				if (isTrigger)
				{
					return false;
				}
				return true;
			}
			if (dotProduct > other.dotProduct)
			{
				return true;
			}
			return false;
		}
	}

	private const float updatesPerSecond = 10f;

	private const float timeBetweenUpdates = 0.1f;

	[SerializeField]
	private PlayerTargetingSettings settings;

	private PlayerInfo playerInfo;

	private CapsuleCollider capsuleCollider;

	private Vector3 searchDirection;

	public readonly List<IInteractable> CurrentInteractables = new List<IInteractable>();

	private float lastUpdateTime;

	private static readonly Collider[] boundingBoxOverlapResults = new Collider[50];

	[CVar("drawTargetSearchDebug", "", "", false, true)]
	private static bool drawSearchDebug;

	public Entity CurrentTargetEntity { get; private set; }

	public bool HasTarget => CurrentTargetEntity != null;

	public IInteractable FirstTargetInteracable => CurrentInteractables[0];

	public Vector3 CurrentTargetReticlePosition
	{
		get
		{
			if (CurrentTargetEntity != null)
			{
				return CurrentTargetEntity.GetTargetReticleWorldPosition();
			}
			return Vector3.zero;
		}
	}

	public event Action TargetChanged;

	public event Action TargetLost;

	private void Awake()
	{
		playerInfo = GetComponent<PlayerInfo>();
		capsuleCollider = GetComponentInChildren<CapsuleCollider>(includeInactive: true);
	}

	private void Update()
	{
		if (!playerInfo.isLocalPlayer || Time.unscaledTime - lastUpdateTime < 0.1f)
		{
			return;
		}
		lastUpdateTime = Time.unscaledTime;
		searchDirection = GameManager.Camera.transform.forward;
		int count = CurrentInteractables.Count;
		Entity currentTargetEntity = CurrentTargetEntity;
		bool flag = TryFindInteractables(capsuleCollider.bounds.center, searchDirection, settings.SearchConeDistance, settings.SearchConeAperture, settings.SearchConeBaseDiameter, CurrentInteractables, initialCheck: true, drawSearchDebug);
		CurrentTargetEntity = (flag ? CurrentInteractables[0].AsEntity : null);
		if (!(CurrentTargetEntity == currentTargetEntity) || CurrentInteractables.Count != count)
		{
			if (currentTargetEntity != null)
			{
				currentTargetEntity.WillBeDestroyed -= OnTargetEntityWillBeDestroyed;
			}
			if (CurrentTargetEntity == null)
			{
				this.TargetLost?.Invoke();
				return;
			}
			CurrentTargetEntity.WillBeDestroyed += OnTargetEntityWillBeDestroyed;
			this.TargetChanged?.Invoke();
		}
	}

	private void OnTargetEntityWillBeDestroyed()
	{
		if (CurrentTargetEntity != null)
		{
			CurrentTargetEntity.WillBeDestroyed -= OnTargetEntityWillBeDestroyed;
		}
		ClearTarget();
	}

	private void ClearTarget()
	{
		CurrentTargetEntity = null;
		CurrentInteractables.Clear();
	}

	private bool TryFindInteractables(Vector3 origin, Vector3 direction, float distance, float aperture, float baseDiameter, List<IInteractable> interactables, bool initialCheck = true, bool drawDebug = false)
	{
		if (!CanSearchForInteractables())
		{
			return false;
		}
		if (BMath.Abs(direction.sqrMagnitude - 1f) >= 0.001f)
		{
			direction.Normalize();
		}
		float angle = aperture * 0.5f;
		float num = baseDiameter + BMath.TanDeg(angle) * distance * 2f;
		Vector3 center = origin + 0.5f * distance * direction;
		Vector3 vector = new Vector3(num, num, distance);
		Vector3 halfExtents = vector * 0.5f;
		Quaternion quaternion = Quaternion.LookRotation(direction);
		Quaternion orientation = quaternion;
		int num2 = Physics.OverlapBoxNonAlloc(center, halfExtents, boundingBoxOverlapResults, orientation, GameManager.LayerSettings.PotentiallyInteractableMask, QueryTriggerInteraction.Collide);
		switch (num2)
		{
		case 0:
			return SecondaryCheck(interactables);
		case 1:
			if (TryPopulateInteractablesFrom(boundingBoxOverlapResults[0], interactables))
			{
				if (drawDebug)
				{
					BDebug.DrawWireCube(center, vector, quaternion, Color.red, 0.1f);
				}
				return true;
			}
			break;
		}
		InteractableHit interactableHit = InteractableHit.Empty;
		Vector3 end = default(Vector3);
		for (int i = 0; i < num2; i++)
		{
			Collider collider = boundingBoxOverlapResults[i];
			if (!TryPopulateInteractablesFrom(collider, interactables))
			{
				continue;
			}
			Vector3 vector2 = collider.ClosestPoint(origin);
			Vector3 vector3 = vector2 - origin;
			float dotProduct = Vector3.Dot(searchDirection, vector3.normalized);
			bool isInSearchDirection = Vector3.Dot(searchDirection, vector3.Horizontalized()) > 0f;
			float magnitude = vector3.magnitude;
			bool flag = false;
			InteractableHit interactableHit2 = interactableHit;
			for (int j = 0; j < interactables.Count; j++)
			{
				InteractableHit interactableHit3 = new InteractableHit(interactables[j], isTrigger: true, magnitude, dotProduct, isInSearchDirection);
				if (interactableHit3.IsBetterThan(interactableHit2))
				{
					flag = true;
					interactableHit2 = interactableHit3;
				}
			}
			if (!flag)
			{
				continue;
			}
			Vector3 vector4 = vector2;
			RaycastHit hitInfo;
			bool flag2 = Physics.Linecast(origin, vector4, out hitInfo, GameManager.LayerSettings.PotentiallyInteractableMask, QueryTriggerInteraction.Collide) && hitInfo.collider != collider;
			if (flag2)
			{
				vector4 = collider.ClosestPoint(origin + direction * distance);
				flag2 = Physics.Linecast(origin, vector4, out hitInfo, GameManager.LayerSettings.PotentiallyInteractableMask, QueryTriggerInteraction.Collide) && hitInfo.collider != collider;
			}
			if (flag2)
			{
				vector4 = collider.ClosestPoint(origin + direction * distance + num / 2f * Vector3.up);
				flag2 = Physics.Linecast(origin, vector4, out hitInfo, GameManager.LayerSettings.PotentiallyInteractableMask, QueryTriggerInteraction.Collide) && hitInfo.collider != collider;
			}
			if (!flag2)
			{
				if (drawDebug)
				{
					BDebug.DrawLine(origin, vector4, Color.yellow, 0.1f);
					end = vector4;
				}
				interactableHit = interactableHit2;
			}
			else if (drawDebug)
			{
				BDebug.DrawLine(origin, hitInfo.point, Color.red, 0.1f);
			}
		}
		interactables.Clear();
		bool flag3 = !interactableHit.Equals(InteractableHit.Empty);
		if (flag3)
		{
			IInteractable[] components = (interactableHit.interactable as Component).GetComponents<IInteractable>();
			foreach (IInteractable interactable in components)
			{
				if (interactable.IsInteractionEnabled)
				{
					interactables.Add(interactable);
				}
			}
		}
		if (drawDebug && flag3)
		{
			BDebug.DrawLine(origin, end, Color.green, 0.1f);
		}
		if (flag3)
		{
			return flag3;
		}
		return SecondaryCheck(interactables);
		bool CanSearchForInteractables()
		{
			if (playerInfo.AsGolfer.IsMatchResolved)
			{
				return false;
			}
			if (playerInfo.ActiveGolfCartSeat.IsValid())
			{
				return false;
			}
			return true;
		}
		bool SecondaryCheck(List<IInteractable> list)
		{
			if (!initialCheck)
			{
				list.Clear();
				return false;
			}
			return TryFindInteractables(origin, base.transform.forward, distance, aperture, baseDiameter, list, initialCheck: false, drawDebug);
		}
	}

	private bool TryPopulateInteractablesFrom(Collider collider, List<IInteractable> interactables)
	{
		interactables.Clear();
		IInteractable[] componentsInParent = collider.GetComponentsInParent<IInteractable>();
		foreach (IInteractable interactable in componentsInParent)
		{
			if (interactable.IsInteractionEnabled)
			{
				if (interactable.AsEntity == playerInfo.AsEntity)
				{
					interactables.Clear();
					return false;
				}
				interactables.Add(interactable);
			}
		}
		return interactables.Count > 0;
	}
}
