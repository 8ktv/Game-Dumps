using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class CrtScreenController : MonoBehaviour
{
	[SerializeField]
	private PostProcessVolume volume;

	private CrtScreen settings;

	private void Awake()
	{
		GetSettings();
	}

	public void SetCrtScreenEnabled(bool crtScreenEnabled)
	{
		GetSettings().enabled.value = crtScreenEnabled;
	}

	public void SetBulgeEnabled(bool bulgeEnabled)
	{
		GetSettings().bulgeEnabled.value = bulgeEnabled;
	}

	public void SetAberrationEnabled(bool aberrationEnabled)
	{
		GetSettings().aberrationEnabled.value = aberrationEnabled;
	}

	public void SetScanlineEnabled(bool scanlineEnabled)
	{
		CrtScreen crtScreen = GetSettings();
		crtScreen.scanlineEnabled.value = scanlineEnabled;
		crtScreen.rollingLinesEnabled.value = scanlineEnabled;
	}

	public bool TryApplyBulgeOffsetToScreenPoint(Vector3 screenPoint, out Vector3 buldgedScreenPoint)
	{
		buldgedScreenPoint = screenPoint;
		if (!IsBuldgeEnabled())
		{
			return false;
		}
		Camera camera = GameManager.Camera;
		if (screenPoint.x < 0f || screenPoint.x > (float)camera.pixelWidth || screenPoint.y < 0f || screenPoint.y > (float)camera.pixelHeight)
		{
			return false;
		}
		Vector3 viewportPoint = GameManager.Camera.ScreenToViewportPoint(screenPoint);
		if (!TryApplyBulgeOffsetToViewportPoint(viewportPoint, out var buldgedViewportPoint))
		{
			return false;
		}
		buldgedScreenPoint = GameManager.Camera.ViewportToScreenPoint(buldgedViewportPoint);
		return true;
	}

	public bool TryApplyBulgeOffsetToViewportPoint(Vector3 viewportPoint, out Vector3 buldgedViewportPoint)
	{
		buldgedViewportPoint = viewportPoint;
		if (!IsBuldgeEnabled())
		{
			return false;
		}
		if (viewportPoint.x < 0f || viewportPoint.x > 1f || viewportPoint.y < 0f || viewportPoint.y > 1f)
		{
			return false;
		}
		float num = GetSettings().bulgeIntensity;
		float num2 = Vector2.Distance((Vector2)viewportPoint, new Vector2(0.5f, 0.5f));
		float x = viewportPoint.x + (num2 - 0.5f) * num * (0.5f - viewportPoint.x);
		float y = viewportPoint.y + (num2 - 0.5f) * num * (0.5f - viewportPoint.y);
		buldgedViewportPoint.x = x;
		buldgedViewportPoint.y = y;
		return true;
	}

	public bool IsEnabled()
	{
		return GetSettings().enabled;
	}

	private bool IsBuldgeEnabled()
	{
		CrtScreen crtScreen = GetSettings();
		if ((bool)crtScreen.enabled)
		{
			return crtScreen.bulgeEnabled;
		}
		return false;
	}

	private CrtScreen GetSettings()
	{
		if (settings != null)
		{
			return settings;
		}
		if (!volume.profile.TryGetSettings<CrtScreen>(out settings))
		{
			Debug.Log("Could not find CRT screen settings on post-process volume", base.gameObject);
			return null;
		}
		return settings;
	}
}
