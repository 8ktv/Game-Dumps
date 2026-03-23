using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SetLayoutInformer : UIBehaviour, ILayoutSelfController, ILayoutController
{
	public event Action SetLayoutHorizontalCalled;

	public event Action SetLayoutVerticalCalled;

	public void SetLayoutHorizontal()
	{
		this.SetLayoutHorizontalCalled?.Invoke();
	}

	public void SetLayoutVertical()
	{
		this.SetLayoutVerticalCalled?.Invoke();
	}
}
