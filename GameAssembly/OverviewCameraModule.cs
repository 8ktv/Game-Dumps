using UnityEngine;

public class OverviewCameraModule : CameraModule
{
	[SerializeField]
	private float fieldOfView = 45f;

	[SerializeField]
	private float moveDuration = 20f;

	[SerializeField]
	private Transform start;

	[SerializeField]
	private Transform end;

	private float timeSinceStarted;

	private bool isSlowingDown;

	private float timeSinceStartedSlowdown;

	private float slowdownDuration;

	public override bool ControlsFieldOfView => true;

	public override float FieldOfView => fieldOfView;

	public override CameraModuleType Type => CameraModuleType.Overview;

	public override void UpdateModule()
	{
		float num = 1f;
		if (isSlowingDown)
		{
			num = BMath.EaseInOut(BMath.InverseLerpClamped(slowdownDuration, 0f, timeSinceStartedSlowdown));
			timeSinceStartedSlowdown += Time.deltaTime;
		}
		float t = timeSinceStarted / moveDuration;
		position = Vector3.Lerp(start.position, end.position, t);
		rotation = Quaternion.Slerp(start.rotation, end.rotation, t);
		timeSinceStarted += Time.deltaTime * num;
	}

	public void BeginSlowdown(float duration)
	{
		isSlowingDown = true;
		timeSinceStartedSlowdown = 0f;
		slowdownDuration = duration;
	}
}
