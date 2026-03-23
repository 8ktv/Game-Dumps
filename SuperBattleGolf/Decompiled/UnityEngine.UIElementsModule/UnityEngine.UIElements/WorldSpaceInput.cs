using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements;

internal static class WorldSpaceInput
{
	public struct PickResult
	{
		public static readonly PickResult Empty = new PickResult
		{
			distance = float.PositiveInfinity
		};

		public Collider collider;

		public UIDocument document;

		public VisualElement pickedElement;

		public float distance;

		public Vector3 normal;

		public Vector3 point;

		public Vector3 localPoint;

		internal int CompareDrawOrder([JetBrains.Annotations.NotNull] UIDocument otherDocument, float otherDistance)
		{
			if (document == null)
			{
				return 1;
			}
			int num = document.panelSettings.sortingOrder.CompareTo(otherDocument.panelSettings.sortingOrder);
			if (num != 0)
			{
				return num;
			}
			int num2 = document.sortingOrder.CompareTo(otherDocument.sortingOrder);
			if (num2 != 0)
			{
				return num2;
			}
			return distance.CompareTo(otherDistance);
		}

		internal void ComputeCollisionData(Ray ray)
		{
			point = ray.origin + ray.direction * distance;
			if (document != null && pickedElement != null)
			{
				localPoint = pickedElement.worldTransformInverse.MultiplyPoint3x4(document.transform.InverseTransformPoint(point));
				normal = document.transform.TransformDirection(pickedElement.worldTransformRef.MultiplyVector(Vector3.forward));
			}
		}
	}

	public static VisualElement Pick3D(UIDocument document, Ray worldRay)
	{
		Ray documentRay = document.transform.worldToLocalMatrix.TransformRay(worldRay);
		return Pick_Internal(document, documentRay);
	}

	public static void PickAll3D(UIDocument document, Ray worldRay, List<VisualElement> outResults)
	{
		Ray documentRay = document.transform.worldToLocalMatrix.TransformRay(worldRay);
		Pick_Internal(document, documentRay, outResults);
	}

	public static VisualElement Pick3D(UIDocument document, Ray worldRay, out float distance)
	{
		Ray ray = document.transform.worldToLocalMatrix.TransformRay(worldRay);
		VisualElement visualElement = Pick_Internal(document, ray);
		if (visualElement != null)
		{
			visualElement.IntersectWorldRay(ray, out var distance2, out var _);
			Vector3 position = ray.origin + ray.direction * distance2;
			Vector3 b = document.transform.TransformPoint(position);
			distance = Vector3.Distance(worldRay.origin, b);
		}
		else
		{
			distance = float.PositiveInfinity;
		}
		return visualElement;
	}

	public static bool Pick3D(UIDocument document, Ray worldRay, out PickResult pickResult)
	{
		Ray ray = document.transform.worldToLocalMatrix.TransformRay(worldRay);
		VisualElement visualElement = Pick_Internal(document, ray);
		if (visualElement == null)
		{
			pickResult = PickResult.Empty;
			return false;
		}
		visualElement.IntersectWorldRay(ray, out var distance, out var _);
		Vector3 position = ray.origin + ray.direction * distance;
		Vector3 b = document.transform.TransformPoint(position);
		float distance2 = Vector3.Distance(worldRay.origin, b);
		pickResult = new PickResult
		{
			document = document,
			pickedElement = visualElement,
			distance = distance2
		};
		pickResult.ComputeCollisionData(worldRay);
		return true;
	}

	public static bool PickElement3D(VisualElement element, Ray worldRay, out PickResult pickResult, bool acceptOutside = false)
	{
		UIDocument uIDocument = UIDocument.FindRootUIDocument(element);
		if (uIDocument == null)
		{
			throw new ArgumentException("Element must be part of a UI Document.");
		}
		Ray worldRay2 = uIDocument.transform.worldToLocalMatrix.TransformRay(worldRay);
		if (!element.IntersectWorldRay(worldRay2, out var distance, out var _) && (!acceptOutside || !(distance > 0f)))
		{
			pickResult = PickResult.Empty;
			return false;
		}
		pickResult = new PickResult
		{
			document = uIDocument,
			pickedElement = element,
			distance = distance
		};
		pickResult.ComputeCollisionData(worldRay);
		return true;
	}

	public static PickResult PickDocument3D(Ray worldRay, float maxDistance = float.PositiveInfinity, int layerMask = -5)
	{
		PickResult result = new PickResult
		{
			distance = float.PositiveInfinity
		};
		float num = 0f;
		Ray ray = worldRay;
		int num2 = 0;
		while (num < maxDistance)
		{
			if (++num2 > 100)
			{
				Debug.LogWarning("PickDocument3D exceeded iteration limit of " + 100 + ". Returned values may be incorrect.");
				break;
			}
			if (!Physics.Raycast(ray, out var hitInfo, maxDistance - num, layerMask, QueryTriggerInteraction.Collide))
			{
				break;
			}
			float distance = hitInfo.distance + num;
			UIDocument componentInParent = hitInfo.collider.GetComponentInParent<UIDocument>(includeInactive: true);
			if (componentInParent == null)
			{
				if (distance < result.distance)
				{
					result = new PickResult
					{
						distance = distance,
						collider = hitInfo.collider,
						normal = hitInfo.normal,
						point = hitInfo.point,
						localPoint = hitInfo.point
					};
				}
				break;
			}
			float num3 = hitInfo.distance + 0.001f;
			ray.origin += ray.direction * num3;
			num += num3;
			if (componentInParent.containerPanel == null)
			{
				continue;
			}
			Bounds picking3DLocalBounds = GetPicking3DLocalBounds(componentInParent.rootVisualElement);
			ref Matrix4x4 worldTransformRef = ref componentInParent.rootVisualElement.worldTransformRef;
			Vector3 point = worldTransformRef.MultiplyPoint3x4(picking3DLocalBounds.center);
			Vector3 vector = worldTransformRef.MultiplyVector(picking3DLocalBounds.size);
			Matrix4x4 localToWorldMatrix = componentInParent.transform.localToWorldMatrix;
			Vector3 b = worldTransformRef.MultiplyPoint3x4(point);
			Vector3 vector2 = localToWorldMatrix.MultiplyVector(vector);
			float num4 = Vector3.Distance(worldRay.origin, b) + vector2.magnitude / 2f + 0.001f;
			maxDistance = Mathf.Min(maxDistance, num + num4);
			if (result.CompareDrawOrder(componentInParent, distance) > 0)
			{
				VisualElement visualElement = Pick3D(componentInParent, worldRay, out distance);
				if (visualElement != null && distance <= maxDistance && result.CompareDrawOrder(componentInParent, distance) > 0)
				{
					result = new PickResult
					{
						collider = hitInfo.collider,
						pickedElement = visualElement,
						document = componentInParent,
						distance = distance
					};
					result.ComputeCollisionData(worldRay);
				}
			}
		}
		return result;
	}

	internal static VisualElement Pick_Internal(UIDocument document, Ray documentRay, List<VisualElement> outResults = null)
	{
		document.containerPanel.ValidateLayout();
		VisualElement rootVisualElement = document.rootVisualElement;
		Ray ray = rootVisualElement.WorldToLocal(documentRay);
		return PerformPick(rootVisualElement, ray, null);
	}

	[VisibleToOtherModules(new string[] { "Assembly-CSharp-testable" })]
	internal static VisualElement PerformPick(VisualElement root, Ray ray, List<VisualElement> outResults)
	{
		return root.needs3DBounds ? PerformPick3D(root, ray, outResults) : PerformPick2D(root, ray, outResults);
	}

	private static VisualElement PerformPick2D(VisualElement root, Ray ray, List<VisualElement> outResults)
	{
		root.IntersectLocalRay(ray, out var localPoint);
		return PerformPick2D_LocalPoint(root, localPoint, outResults);
	}

	private static VisualElement PerformPick3D(VisualElement root, Ray ray, List<VisualElement> outResults)
	{
		if (root.resolvedStyle.display == DisplayStyle.None)
		{
			return null;
		}
		if (root.pickingMode == PickingMode.Ignore && root.hierarchy.childCount == 0)
		{
			return null;
		}
		if (!GetPicking3DLocalBounds(root).IntersectRay(ray))
		{
			return null;
		}
		Vector3 localPoint;
		bool flag = root.IntersectLocalRay(ray, out localPoint) && root.ContainsPoint(localPoint);
		if (!flag && root.ShouldClip())
		{
			return null;
		}
		VisualElement visualElement = null;
		int childCount = root.hierarchy.childCount;
		for (int num = childCount - 1; num >= 0; num--)
		{
			VisualElement visualElement2 = root.hierarchy[num];
			Ray ray2 = root.ChangeCoordinatesTo(visualElement2, ray);
			VisualElement visualElement3 = PerformPick(visualElement2, ray2, outResults);
			if (visualElement == null && visualElement3 != null)
			{
				if (outResults == null)
				{
					return visualElement3;
				}
				visualElement = visualElement3;
			}
		}
		if (root.visible && root.pickingMode == PickingMode.Position && flag)
		{
			outResults?.Add(root);
			if (visualElement == null)
			{
				visualElement = root;
			}
		}
		return visualElement;
	}

	private static VisualElement PerformPick2D_LocalPoint(VisualElement root, Vector3 localPoint, List<VisualElement> picked = null)
	{
		if (root.resolvedStyle.display == DisplayStyle.None)
		{
			return null;
		}
		if (root.pickingMode == PickingMode.Ignore && root.hierarchy.childCount == 0)
		{
			return null;
		}
		if (!root.boundingBox.Contains(localPoint))
		{
			return null;
		}
		bool flag = root.ContainsPoint(localPoint);
		if (!flag && root.ShouldClip())
		{
			return null;
		}
		VisualElement visualElement = null;
		int childCount = root.hierarchy.childCount;
		for (int num = childCount - 1; num >= 0; num--)
		{
			VisualElement visualElement2 = root.hierarchy[num];
			Vector2 vector = root.ChangeCoordinatesTo(visualElement2, localPoint);
			VisualElement visualElement3 = PerformPick2D_LocalPoint(visualElement2, vector, picked);
			if (visualElement == null && visualElement3 != null)
			{
				if (picked == null)
				{
					return visualElement3;
				}
				visualElement = visualElement3;
			}
		}
		if (root.visible && root.pickingMode == PickingMode.Position && flag)
		{
			picked?.Add(root);
			if (visualElement == null)
			{
				visualElement = root;
			}
		}
		return visualElement;
	}

	internal static Bounds GetPicking3DWorldBounds(VisualElement ve)
	{
		Bounds bounds = GetPicking3DLocalBounds(ve);
		VisualElement.TransformAlignedBounds(ref ve.worldTransformRef, ref bounds);
		return bounds;
	}

	internal static Bounds GetPicking3DLocalBounds(VisualElement ve)
	{
		if (ve.needs3DBounds)
		{
			return ve.localBoundsPicking3D;
		}
		Rect boundingBox = ve.boundingBox;
		return new Bounds(boundingBox.center, boundingBox.size);
	}

	public static Vector3 LocalPointToGameObjectWorldSpace(VisualElement element, Vector3 localPoint)
	{
		UIDocument uIDocument = UIDocument.FindRootUIDocument(element);
		if (uIDocument == null)
		{
			throw new ArgumentException("Element must be part of a UI Document.");
		}
		Vector3 position = element.LocalToWorld3D(localPoint);
		return uIDocument.transform.TransformPoint(position);
	}

	public static Vector3 LocalDeltaToGameObjectWorldSpace(VisualElement element, Vector3 localDelta)
	{
		return LocalPointToGameObjectWorldSpace(element, localDelta) - LocalPointToGameObjectWorldSpace(element, Vector3.zero);
	}

	public static Vector3 GameObjectWorldSpaceToLocalPoint(VisualElement element, Vector3 worldPoint)
	{
		UIDocument uIDocument = UIDocument.FindRootUIDocument(element);
		if (uIDocument == null)
		{
			throw new ArgumentException("Element must be part of a UI Document.");
		}
		Vector3 p = uIDocument.transform.InverseTransformPoint(worldPoint);
		return element.WorldToLocal3D(p);
	}

	public static Vector3 GameObjectWorldSpaceToLocalDelta(VisualElement element, Vector3 worldDelta)
	{
		return GameObjectWorldSpaceToLocalPoint(element, worldDelta) - GameObjectWorldSpaceToLocalPoint(element, Vector3.zero);
	}
}
