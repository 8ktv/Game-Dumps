using UnityEngine;

public class SwingTrajectoryPreview : SingletonBehaviour<SwingTrajectoryPreview>, ILateBUpdateCallback, IAnyBUpdateCallback
{
	[SerializeField]
	private LineRenderer lineRenderer;

	[SerializeField]
	[Min(0.001f)]
	private float distance;

	private Vector3[] points;

	private bool hasData;

	private Vector3 worldOrigin;

	private Vector3 direction;

	private bool isEnabled;

	private bool isLockedOn;

	private bool isVisible;

	protected override void Awake()
	{
		base.Awake();
		points = new Vector3[3];
		lineRenderer.positionCount = 3;
		lineRenderer.useWorldSpace = true;
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		if (isVisible)
		{
			BUpdate.DeregisterCallback(this);
		}
	}

	public static void SetIsEnabled(bool isEnabled)
	{
		if (SingletonBehaviour<SwingTrajectoryPreview>.HasInstance)
		{
			SingletonBehaviour<SwingTrajectoryPreview>.Instance.SetIsEnabledInternal(isEnabled);
		}
	}

	public static void SetData(Vector3 worldOrigin, Vector3 direction)
	{
		if (SingletonBehaviour<SwingTrajectoryPreview>.HasInstance)
		{
			SingletonBehaviour<SwingTrajectoryPreview>.Instance.SetDataInternal(worldOrigin, direction);
		}
	}

	public static void SetIsLockedOn(bool isLockedOn)
	{
		if (SingletonBehaviour<SwingTrajectoryPreview>.HasInstance)
		{
			SingletonBehaviour<SwingTrajectoryPreview>.Instance.SetIsLockedOnInternal(isLockedOn);
		}
	}

	public static void ClearData()
	{
		if (SingletonBehaviour<SwingTrajectoryPreview>.HasInstance)
		{
			SingletonBehaviour<SwingTrajectoryPreview>.Instance.ClearDataInternal();
		}
	}

	private void SetIsEnabledInternal(bool isEnabled)
	{
		this.isEnabled = isEnabled;
		UpdateIsVisible();
	}

	private void SetDataInternal(Vector3 worldOrigin, Vector3 direction)
	{
		hasData = true;
		this.worldOrigin = worldOrigin;
		this.direction = direction;
		UpdateIsVisible();
	}

	private void SetIsLockedOnInternal(bool isLockedOn)
	{
		this.isLockedOn = isLockedOn;
		UpdateIsVisible();
	}

	private void ClearDataInternal()
	{
		hasData = false;
		UpdateIsVisible();
	}

	public void OnLateBUpdate()
	{
		base.transform.rotation = Quaternion.LookRotation(Vector3.down, Vector3.forward);
		Vector3 vector = worldOrigin + 0.01f * Vector3.up;
		Vector3 vector2 = vector + direction * distance;
		points[0] = vector;
		points[1] = Vector3.Lerp(vector, vector2, 0.5f);
		points[2] = vector2;
		lineRenderer.SetPositions(points);
	}

	private void UpdateIsVisible()
	{
		bool num = isVisible;
		isVisible = isEnabled && hasData && !isLockedOn;
		if (num != isVisible)
		{
			if (isVisible)
			{
				lineRenderer.enabled = true;
				BUpdate.RegisterCallback(this);
			}
			else
			{
				lineRenderer.enabled = false;
				BUpdate.DeregisterCallback(this);
			}
		}
	}
}
