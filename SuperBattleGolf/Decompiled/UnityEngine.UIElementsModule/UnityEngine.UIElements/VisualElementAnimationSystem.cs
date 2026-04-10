using System.Collections.Generic;
using System.Linq;
using Unity.Profiling;
using UnityEngine.UIElements.Experimental;

namespace UnityEngine.UIElements;

internal class VisualElementAnimationSystem : BaseVisualTreeUpdater
{
	private HashSet<IValueAnimationUpdate> m_Animations = new HashSet<IValueAnimationUpdate>();

	private List<IValueAnimationUpdate> m_IterationList = new List<IValueAnimationUpdate>();

	private bool m_HasNewAnimations = false;

	private bool m_IterationListDirty = false;

	private static readonly string s_Description = "UIElements.UpdateAnimation";

	private static readonly ProfilerMarker s_ProfilerMarker = new ProfilerMarker(s_Description);

	private static readonly string s_StylePropertyAnimationDescription = "UIElements.UpdateAnimationProperties";

	private static readonly ProfilerMarker s_StylePropertyAnimationProfilerMarker = new ProfilerMarker(s_StylePropertyAnimationDescription);

	private double lastUpdate;

	public override ProfilerMarker profilerMarker => s_ProfilerMarker;

	private static ProfilerMarker stylePropertyAnimationProfilerMarker => s_StylePropertyAnimationProfilerMarker;

	public void UnregisterAnimation(IValueAnimationUpdate anim)
	{
		m_Animations.Remove(anim);
		m_IterationListDirty = true;
	}

	public void UnregisterAnimations(List<IValueAnimationUpdate> anims)
	{
		foreach (IValueAnimationUpdate anim in anims)
		{
			m_Animations.Remove(anim);
		}
		m_IterationListDirty = true;
	}

	public void RegisterAnimation(IValueAnimationUpdate anim)
	{
		m_Animations.Add(anim);
		m_HasNewAnimations = true;
		m_IterationListDirty = true;
	}

	public void RegisterAnimations(List<IValueAnimationUpdate> anims)
	{
		foreach (IValueAnimationUpdate anim in anims)
		{
			m_Animations.Add(anim);
		}
		m_HasNewAnimations = true;
		m_IterationListDirty = true;
	}

	public override void Update()
	{
		double num = base.panel.TimeSinceStartupSeconds();
		long currentTimeMs = (long)(num * 1000.0);
		if (m_IterationListDirty)
		{
			m_IterationList = m_Animations.ToList();
			m_IterationListDirty = false;
		}
		if (m_HasNewAnimations || lastUpdate != num)
		{
			foreach (IValueAnimationUpdate iteration in m_IterationList)
			{
				iteration.Tick(currentTimeMs);
			}
			m_HasNewAnimations = false;
			lastUpdate = num;
		}
		IStylePropertyAnimationSystem styleAnimationSystem = base.panel.styleAnimationSystem;
		using (stylePropertyAnimationProfilerMarker.Auto())
		{
			styleAnimationSystem.Update(num);
		}
	}

	public override void OnVersionChanged(VisualElement ve, VersionChangeType versionChangeType)
	{
	}
}
