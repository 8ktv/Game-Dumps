using UnityEngine;

[ExecuteAlways]
public class LookAtCamera : MonoBehaviour
{
	private Camera targetCamera;

	private void OnEnable()
	{
		targetCamera = Camera.main;
	}

	private void Update()
	{
		UpdateRotation();
	}

	private void UpdateRotation()
	{
		if ((bool)targetCamera)
		{
			base.transform.rotation = Quaternion.LookRotation(targetCamera.transform.position - base.transform.position);
		}
	}
}
