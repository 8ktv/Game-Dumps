using UnityEngine;

public class LockOnTargetUi : MonoBehaviour
{
	[SerializeField]
	private Transform container;

	private bool isVisible;

	private LockOnTarget target;

	private Vector3 worldPosition;

	private void Awake()
	{
		container.gameObject.SetActive(value: false);
	}

	public void OnLateUpdate()
	{
		UpdatePosition();
		UpdateVisibility();
		bool IsVisible()
		{
			return container.position.z > 0f;
		}
		void UpdatePosition()
		{
			if (target != null)
			{
				worldPosition = target.GetLockOnPosition();
			}
			container.position = CameraModuleController.WorldToScreenPoint(worldPosition);
		}
		void UpdateVisibility()
		{
			bool flag = isVisible;
			isVisible = IsVisible();
			if (isVisible != flag)
			{
				container.gameObject.SetActive(isVisible);
			}
		}
	}

	public void SetTarget(LockOnTarget target)
	{
		this.target = target;
	}
}
