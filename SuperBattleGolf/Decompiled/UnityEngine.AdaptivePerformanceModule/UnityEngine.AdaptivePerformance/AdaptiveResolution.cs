namespace UnityEngine.AdaptivePerformance;

public class AdaptiveResolution : AdaptivePerformanceScaler
{
	private static int instanceCount;

	protected override void Awake()
	{
		base.Awake();
		if (!(m_Settings == null))
		{
			ApplyDefaultSetting(m_Settings.scalerSettings.AdaptiveResolution);
		}
	}

	protected override void OnDisabled()
	{
		OnDestroy();
	}

	protected override void OnEnabled()
	{
	}

	private void OnValidate()
	{
		if (MaxLevel < 1)
		{
			MaxLevel = 1;
		}
		MaxBound = Mathf.Clamp(MaxBound, 0.25f, 1f);
		MinBound = Mathf.Clamp(MinBound, 0.25f, MaxBound);
	}

	private bool IsDynamicResolutionSupported()
	{
		return true;
	}

	private void Start()
	{
		instanceCount++;
		if (instanceCount > 1)
		{
			Debug.LogWarning("Multiple Adaptive Resolution scalers created. They will interfere with each other.");
		}
		if (!IsDynamicResolutionSupported())
		{
			Debug.Log($"Dynamic resolution is not supported. Will be using fallback to Render Scale Multiplier.");
		}
	}

	private void OnDestroy()
	{
		instanceCount--;
		if (Scale != 1f)
		{
			APLog.Debug("Restoring dynamic resolution scale factor to 1.0");
			if (IsDynamicResolutionSupported())
			{
				ScalableBufferManager.ResizeBuffers(1f, 1f);
			}
			else
			{
				AdaptivePerformanceRenderSettings.RenderScaleMultiplier = 1f;
			}
		}
	}

	protected override void OnLevel()
	{
		bool flag = ScaleChanged();
		if (IsDynamicResolutionSupported())
		{
			if (flag)
			{
				ScalableBufferManager.ResizeBuffers(Scale, Scale);
			}
			int num = (int)Mathf.Ceil(ScalableBufferManager.widthScaleFactor * (float)Screen.currentResolution.width);
			int num2 = (int)Mathf.Ceil(ScalableBufferManager.heightScaleFactor * (float)Screen.currentResolution.height);
			APLog.Debug($"Adaptive Resolution Scale: {Scale:F3} Resolution: {num}x{num2} ScaleFactor: {ScalableBufferManager.widthScaleFactor:F3}x{ScalableBufferManager.heightScaleFactor:F3} Level:{base.CurrentLevel}/{MaxLevel}");
		}
		else
		{
			AdaptivePerformanceRenderSettings.RenderScaleMultiplier = Scale;
			APLog.Debug($"Dynamic resolution is not supported. Using fallback to Render Scale Multiplier : {Scale:F3}");
		}
	}
}
