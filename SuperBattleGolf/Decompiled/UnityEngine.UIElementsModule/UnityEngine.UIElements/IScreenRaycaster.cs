using System.Collections.Generic;

namespace UnityEngine.UIElements;

internal interface IScreenRaycaster
{
	void Update();

	IEnumerable<(Ray ray, Camera camera, bool isInsideCameraRect)> MakeRay(Vector2 mousePosition, int pointerId, int? targetDisplay);
}
