using Eflatun.SceneReference;
using UnityEngine;

[CreateAssetMenu(fileName = "Hole camera transform data", menuName = "Settings/Courses/Hole Camera Transform")]
public class HoleCameraTransformData : ScriptableObject
{
	[SerializeField]
	public SceneReference Scene;

	[SerializeField]
	public Vector3 overviewCameraStartPosition;

	[SerializeField]
	public Vector3 overviewCameraStartRotationEuler;

	[SerializeField]
	public Vector3 overviewCameraEndPosition;

	[SerializeField]
	public Vector3 overviewCameraEndRotationEuler;

	[SerializeField]
	public Vector3 screenshotCameraPosition;

	[SerializeField]
	public Vector3 screenshotCameraRotationEuler;
}
