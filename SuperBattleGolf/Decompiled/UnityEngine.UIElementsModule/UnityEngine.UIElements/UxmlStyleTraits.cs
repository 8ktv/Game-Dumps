using System;
using System.Collections.Generic;

namespace UnityEngine.UIElements;

[Obsolete("UxmlStyleTraits is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
public class UxmlStyleTraits : UxmlTraits
{
	private UxmlStringAttributeDescription m_Name = new UxmlStringAttributeDescription
	{
		name = "name"
	};

	private UxmlStringAttributeDescription m_Path = new UxmlStringAttributeDescription
	{
		name = "path"
	};

	private UxmlStringAttributeDescription m_Src = new UxmlStringAttributeDescription
	{
		name = "src"
	};

	public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
	{
		get
		{
			yield break;
		}
	}
}
