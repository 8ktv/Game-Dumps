using System;
using System.Collections.Generic;

namespace UnityEngine.Rendering;

[Serializable]
public class RenderPipelineGraphicsSettingsCollection
{
	[SerializeReference]
	private List<IRenderPipelineGraphicsSettings> m_List = new List<IRenderPipelineGraphicsSettings>();

	public List<IRenderPipelineGraphicsSettings> settingsList => m_List;
}
