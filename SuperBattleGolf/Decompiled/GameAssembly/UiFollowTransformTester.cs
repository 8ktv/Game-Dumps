using UnityEngine;

[ExecuteAlways]
public class UiFollowTransformTester : MonoBehaviour
{
	[SerializeField]
	private RectTransform rectTransform;

	[SerializeField]
	private Transform target;

	[SerializeField]
	private Vector3 targetOffset;

	private Camera camera;

	private void Update()
	{
		if (!camera)
		{
			camera = Camera.main;
		}
		if ((bool)target && (bool)rectTransform)
		{
			Vector2 vector = camera.WorldToScreenPoint(target.position + targetOffset);
			rectTransform.position = vector;
		}
	}
}
