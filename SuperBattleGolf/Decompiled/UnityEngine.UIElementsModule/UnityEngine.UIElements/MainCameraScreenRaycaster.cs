using System;

namespace UnityEngine.UIElements;

internal class MainCameraScreenRaycaster : CameraScreenRaycaster
{
	private Camera[] singleCameraArray = new Camera[1];

	public MainCameraScreenRaycaster()
	{
		ResolveCamera();
	}

	public override void Update()
	{
		ResolveCamera();
	}

	private void ResolveCamera()
	{
		Camera main = Camera.main;
		if (main != null)
		{
			cameras = singleCameraArray;
			cameras[0] = main;
		}
		else
		{
			cameras = Array.Empty<Camera>();
		}
	}
}
