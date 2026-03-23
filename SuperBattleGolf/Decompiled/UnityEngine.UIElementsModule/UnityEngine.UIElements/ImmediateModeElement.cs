using System;
using System.Collections.Generic;
using Unity.Profiling;
using Unity.Properties;
using UnityEngine.Internal;

namespace UnityEngine.UIElements;

public abstract class ImmediateModeElement : VisualElement
{
	[Serializable]
	[ExcludeFromDocs]
	public new class UxmlSerializedData : VisualElement.UxmlSerializedData
	{
	}

	internal static readonly BindingId cullingEnabledProperty = "cullingEnabled";

	private static readonly Dictionary<Type, ProfilerMarker> s_Markers = new Dictionary<Type, ProfilerMarker>();

	private readonly ProfilerMarker m_ImmediateRepaintMarker;

	private bool m_CullingEnabled = false;

	[CreateProperty]
	public bool cullingEnabled
	{
		get
		{
			return m_CullingEnabled;
		}
		set
		{
			if (m_CullingEnabled != value)
			{
				m_CullingEnabled = value;
				IncrementVersion(VersionChangeType.Repaint);
				NotifyPropertyChanged(in cullingEnabledProperty);
			}
		}
	}

	public ImmediateModeElement()
	{
		base.generateVisualContent = (Action<MeshGenerationContext>)Delegate.Combine(base.generateVisualContent, new Action<MeshGenerationContext>(OnGenerateVisualContent));
		Type type = GetType();
		if (!s_Markers.TryGetValue(type, out m_ImmediateRepaintMarker))
		{
			m_ImmediateRepaintMarker = new ProfilerMarker(base.typeName + ".ImmediateRepaint");
			s_Markers[type] = m_ImmediateRepaintMarker;
		}
	}

	private void OnGenerateVisualContent(MeshGenerationContext mgc)
	{
		if (base.elementPanel is BaseRuntimePanel { drawsInCameras: not false })
		{
			Debug.LogError("ImmediateModeElement cannot be used in a panel drawn by cameras.");
		}
		else
		{
			mgc.entryRecorder.DrawImmediate(mgc.parentEntry, CallImmediateRepaint, cullingEnabled);
		}
	}

	private void CallImmediateRepaint()
	{
		using (m_ImmediateRepaintMarker.Auto())
		{
			ImmediateRepaint();
		}
	}

	protected abstract void ImmediateRepaint();
}
