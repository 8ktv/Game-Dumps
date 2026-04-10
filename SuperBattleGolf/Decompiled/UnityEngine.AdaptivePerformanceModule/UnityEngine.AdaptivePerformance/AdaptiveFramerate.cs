namespace UnityEngine.AdaptivePerformance;

public class AdaptiveFramerate : AdaptivePerformanceScaler
{
	private int m_DefaultFPS;

	private int m_FirstTimeStart = 0;

	protected override void Awake()
	{
		base.Awake();
		m_FirstTimeStart = 0;
		if (!(m_Settings == null))
		{
			ApplyDefaultSetting(m_Settings.scalerSettings.AdaptiveFramerate);
		}
	}

	protected override void OnDisabled()
	{
		if (m_FirstTimeStart < 2)
		{
			m_FirstTimeStart++;
		}
		else
		{
			Application.targetFrameRate = m_DefaultFPS;
		}
	}

	protected override void OnEnabled()
	{
		if (m_FirstTimeStart >= 2)
		{
			m_DefaultFPS = Application.targetFrameRate;
			Application.targetFrameRate = (int)MaxBound;
		}
	}

	protected override void OnLevelIncrease()
	{
		base.OnLevelIncrease();
		int num = 1;
		if (Holder.Instance.Indexer.PerformanceAction == StateAction.FastDecrease)
		{
			num = 5;
		}
		int num2 = Application.targetFrameRate - num;
		if ((float)num2 >= MinBound && (float)num2 <= MaxBound)
		{
			Application.targetFrameRate = num2;
		}
	}

	protected override void OnLevelDecrease()
	{
		base.OnLevelDecrease();
		int num = Application.targetFrameRate + 5;
		if ((float)num >= MinBound && (float)num <= MaxBound)
		{
			Application.targetFrameRate = num;
		}
	}
}
