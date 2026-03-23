using UnityEngine;
using UnityEngine.InputSystem;

public class PhotoCameraModule : CameraModule
{
	[CVar("photoCameraFov", "", "", false, true)]
	public static int PhotoFieldOfView = 60;

	[CVar("photoCameraHorizontalSpeed", "", "", false, true)]
	public static int PhotoCameraHorizontalSpeed = 5;

	[CVar("photoCameraVerticalSpeed", "", "", false, true)]
	public static int PhotoCameraVerticalSpeed = 5;

	[CVar("photoCameraHorizontalMovementLock", "", "", false, true)]
	public static bool PhotoCameraHorizontalMovementLock = false;

	private Vector3 eulerAngles;

	private static Vector3 savedPosition;

	private static Quaternion savedRotation;

	public static bool Active { get; private set; }

	public override bool ControlsFieldOfView => true;

	public override float FieldOfView => PhotoFieldOfView;

	public override CameraModuleType Type => CameraModuleType.Photo;

	private void OnEnable()
	{
		Active = true;
	}

	private void OnDisable()
	{
		Active = false;
	}

	[CCommand("savePhotoCameraCoords", "", false, false)]
	public static void SavePhotoCameraCoords()
	{
		savedPosition = GameManager.Camera.transform.position;
		savedRotation = GameManager.Camera.transform.rotation;
	}

	[CCommand("applyPhotoCameraCoords", "", false, false)]
	public static void ApplyPhotoCameraCoords()
	{
		PhotoCameraModule photoCameraModule = Object.FindFirstObjectByType<PhotoCameraModule>(FindObjectsInactive.Include);
		photoCameraModule.eulerAngles = savedRotation.eulerAngles;
		photoCameraModule.rotation = savedRotation;
		photoCameraModule.position = savedPosition;
	}

	public override void OnTransitionStart(bool transitioningToThis)
	{
		if (transitioningToThis)
		{
			SyncFromTransform();
		}
		eulerAngles = rotation.eulerAngles;
	}

	public override void UpdateModule()
	{
		if (!InputManager.CurrentModeMask.HasFlag(InputMode.Paused))
		{
			Vector2 axis = PlayerInput.Controls.Camera.Look.ReadValue<Vector2>();
			axis = GameSettings.All.Controls.Camera.ScaleInput(axis, isAiming: false);
			Vector2 vector = PlayerInput.Controls.Gameplay.Move.ReadValue<Vector2>();
			float num = 0f;
			if (Keyboard.current.spaceKey.isPressed)
			{
				num = 1f;
			}
			else if (Keyboard.current.fKey.isPressed)
			{
				num = -1f;
			}
			float num2 = 1f;
			if (Keyboard.current.leftShiftKey.isPressed)
			{
				num2 = 10f;
			}
			Vector3 zero = Vector3.zero;
			Vector3 zero2 = Vector3.zero;
			if (PhotoCameraHorizontalMovementLock)
			{
				zero = (base.transform.forward.Horizontalized().normalized * vector.y + base.transform.right.Horizontalized().normalized * vector.x) * PhotoCameraHorizontalSpeed;
				zero2 = num * (float)PhotoCameraVerticalSpeed * Vector3.up;
			}
			else
			{
				zero = (base.transform.forward * vector.y + base.transform.right * vector.x) * PhotoCameraHorizontalSpeed;
				zero2 = num * (float)PhotoCameraVerticalSpeed * base.transform.up;
			}
			position += Time.unscaledDeltaTime * num2 * (zero + zero2);
			float num3 = (InputManager.UsingGamepad ? 1 : 100);
			eulerAngles.x -= axis.y * num3;
			eulerAngles.x = BMath.Clamp(eulerAngles.x, -90f, 90f);
			eulerAngles.y += axis.x * num3;
			rotation = Quaternion.Euler(eulerAngles);
			CursorManager.SetCursorLock(locked: true);
		}
	}
}
