using System;

namespace UnityEngine.UIElements;

[Obsolete("UxmlFactory<TCreatedType, TTraits> is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
public class UxmlFactory<TCreatedType, TTraits> : BaseUxmlFactory<TCreatedType, TTraits>, IUxmlFactory, IBaseUxmlFactory where TCreatedType : VisualElement, new() where TTraits : UxmlTraits, new()
{
	public virtual VisualElement Create(IUxmlAttributes bag, CreationContext cc)
	{
		TCreatedType val = new TCreatedType();
		m_Traits.Init(val, bag, cc);
		return val;
	}
}
[Obsolete("UxmlFactory<TCreatedType> is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
public class UxmlFactory<TCreatedType> : UxmlFactory<TCreatedType, VisualElement.UxmlTraits> where TCreatedType : VisualElement, new()
{
}
