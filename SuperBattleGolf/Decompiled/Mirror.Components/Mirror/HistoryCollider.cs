using UnityEngine;

namespace Mirror;

[DisallowMultipleComponent]
[AddComponentMenu("Network/ Lag Compensation/ History Collider")]
public class HistoryCollider : MonoBehaviour
{
	[Header("Components")]
	[Tooltip("The object's actual collider. We need to know where it is, and how large it is.")]
	public Collider actualCollider;

	[Tooltip("The helper collider that the history bounds are projected onto.\nNeeds to be added to a child GameObject to counter-rotate an axis aligned Bounding Box onto it.\nThis is only used by this component.")]
	public BoxCollider boundsCollider;

	[Header("History")]
	[Tooltip("Keep this many past bounds in the buffer. The larger this is, the further we can raycast into the past.\nMaximum time := historyAmount * captureInterval")]
	public int boundsLimit = 8;

	[Tooltip("Gather N bounds at a time into a bucket for faster encapsulation. A factor of 2 will be twice as fast, etc.")]
	public int boundsPerBucket = 2;

	[Tooltip("Capture bounds every 'captureInterval' seconds. Larger values will require fewer computations, but may not capture every small move.")]
	public float captureInterval = 0.1f;

	private double lastCaptureTime;

	[Header("Debug")]
	public Color historyColor = new Color(1f, 0.5f, 0f, 1f);

	public Color currentColor = Color.red;

	protected HistoryBounds history;

	protected virtual void Awake()
	{
		history = new HistoryBounds(boundsLimit, boundsPerBucket);
		if (actualCollider == null)
		{
			Debug.LogError("HistoryCollider: actualCollider was not set.");
		}
		if (boundsCollider == null)
		{
			Debug.LogError("HistoryCollider: boundsCollider was not set.");
		}
		if (boundsCollider.transform.parent != base.transform)
		{
			Debug.LogError("HistoryCollider: boundsCollider must be a child of this GameObject.");
		}
		if (!boundsCollider.isTrigger)
		{
			Debug.LogError("HistoryCollider: boundsCollider must be a trigger.");
		}
	}

	protected virtual void FixedUpdate()
	{
		if (NetworkTime.localTime >= lastCaptureTime + (double)captureInterval)
		{
			lastCaptureTime = NetworkTime.localTime;
			CaptureBounds();
		}
		ProjectBounds();
	}

	protected virtual void CaptureBounds()
	{
		Bounds bounds = actualCollider.bounds;
		history.Insert(bounds);
	}

	protected virtual void ProjectBounds()
	{
		Bounds total = history.total;
		if (history.boundsCount != 0)
		{
			if (base.transform.lossyScale != Vector3.one)
			{
				Debug.LogWarning("HistoryCollider: " + base.name + "'s transform global scale must be (1,1,1).");
				return;
			}
			boundsCollider.transform.localRotation = Quaternion.Inverse(base.transform.rotation);
			boundsCollider.center = boundsCollider.transform.InverseTransformPoint(total.center);
			boundsCollider.size = total.size;
		}
	}

	protected virtual void OnDrawGizmos()
	{
		Gizmos.color = historyColor;
		Gizmos.DrawWireCube(history.total.center, history.total.size);
		Gizmos.color = currentColor;
		Gizmos.DrawWireCube(actualCollider.bounds.center, actualCollider.bounds.size);
	}
}
