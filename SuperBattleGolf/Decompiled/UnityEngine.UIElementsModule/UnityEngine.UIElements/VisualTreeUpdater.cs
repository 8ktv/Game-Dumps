using System;

namespace UnityEngine.UIElements;

internal sealed class VisualTreeUpdater : IDisposable
{
	private class UpdaterArray
	{
		private IVisualTreeUpdater[] m_VisualTreeUpdaters;

		public IVisualTreeUpdater this[VisualTreeUpdatePhase phase]
		{
			get
			{
				return m_VisualTreeUpdaters[(int)phase];
			}
			set
			{
				m_VisualTreeUpdaters[(int)phase] = value;
			}
		}

		public IVisualTreeUpdater this[int index]
		{
			get
			{
				return m_VisualTreeUpdaters[index];
			}
			set
			{
				m_VisualTreeUpdaters[index] = value;
			}
		}

		public UpdaterArray()
		{
			m_VisualTreeUpdaters = new IVisualTreeUpdater[8];
		}

		public long[] GetUpdatersFrameCount()
		{
			long[] array = new long[m_VisualTreeUpdaters.Length];
			for (int i = 0; i < m_VisualTreeUpdaters.Length; i++)
			{
				array[i] = m_VisualTreeUpdaters[i].FrameCount;
			}
			return array;
		}
	}

	private BaseVisualElementPanel m_Panel;

	private UpdaterArray m_UpdaterArray;

	public long[] GetUpdatersFrameCount()
	{
		return m_UpdaterArray.GetUpdatersFrameCount();
	}

	public VisualTreeUpdater(BaseVisualElementPanel panel)
	{
		m_Panel = panel;
		m_UpdaterArray = new UpdaterArray();
		SetDefaultUpdaters();
	}

	public void Dispose()
	{
		for (int i = 0; i < 8; i++)
		{
			IVisualTreeUpdater visualTreeUpdater = m_UpdaterArray[i];
			visualTreeUpdater.Dispose();
		}
	}

	[Obsolete("This will be removed. Please use the different update methods from Panel instead")]
	public void UpdateVisualTree()
	{
		for (int i = 0; i < 8; i++)
		{
			IVisualTreeUpdater visualTreeUpdater = m_UpdaterArray[i];
			using (visualTreeUpdater.profilerMarker.Auto())
			{
				visualTreeUpdater.Update();
				long frameCount = visualTreeUpdater.FrameCount + 1;
				visualTreeUpdater.FrameCount = frameCount;
			}
		}
	}

	public void UpdateVisualTreePhase(VisualTreeUpdatePhase phase)
	{
		IVisualTreeUpdater visualTreeUpdater = m_UpdaterArray[phase];
		using (visualTreeUpdater.profilerMarker.Auto())
		{
			visualTreeUpdater.Update();
			long frameCount = visualTreeUpdater.FrameCount + 1;
			visualTreeUpdater.FrameCount = frameCount;
		}
	}

	public void OnVersionChanged(VisualElement ve, VersionChangeType versionChangeType)
	{
		for (int i = 0; i < 8; i++)
		{
			IVisualTreeUpdater visualTreeUpdater = m_UpdaterArray[i];
			visualTreeUpdater.OnVersionChanged(ve, versionChangeType);
		}
	}

	public void SetUpdater(IVisualTreeUpdater updater, VisualTreeUpdatePhase phase)
	{
		m_UpdaterArray[phase]?.Dispose();
		updater.panel = m_Panel;
		m_UpdaterArray[phase] = updater;
	}

	public void SetUpdater<T>(VisualTreeUpdatePhase phase) where T : IVisualTreeUpdater, new()
	{
		m_UpdaterArray[phase]?.Dispose();
		T val = new T
		{
			panel = m_Panel
		};
		m_UpdaterArray[phase] = val;
	}

	public IVisualTreeUpdater GetUpdater(VisualTreeUpdatePhase phase)
	{
		return m_UpdaterArray[phase];
	}

	private void SetDefaultUpdaters()
	{
		SetUpdater<VisualTreeBindingsUpdater>(VisualTreeUpdatePhase.Bindings);
		SetUpdater<VisualTreeDataBindingsUpdater>(VisualTreeUpdatePhase.DataBinding);
		SetUpdater<VisualElementAnimationSystem>(VisualTreeUpdatePhase.Animation);
		SetUpdater<VisualTreeStyleUpdater>(VisualTreeUpdatePhase.Styles);
		SetUpdater<VisualTreeLayoutUpdater>(VisualTreeUpdatePhase.Layout);
		SetUpdater<VisualTreeHierarchyFlagsUpdater>(VisualTreeUpdatePhase.TransformClip);
		SetUpdater<UIRRepaintUpdater>(VisualTreeUpdatePhase.Repaint);
		SetUpdater<VisualTreeAuthoringUpdater>(VisualTreeUpdatePhase.Authoring);
	}
}
