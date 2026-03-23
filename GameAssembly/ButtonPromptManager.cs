using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization;

public class ButtonPromptManager : SingletonBehaviour<ButtonPromptManager>
{
	public enum Type
	{
		LeftCorner,
		Center,
		WorldSpace
	}

	public ButtonPrompt leftCornerPrefab;

	public ButtonPrompt centerPrefab;

	public ButtonPrompt worldSpacePrefab;

	public Transform leftCorner;

	public Transform center;

	public Transform world;

	public Transform pooled;

	private readonly Stack<ButtonPrompt>[] availableInstances = new Stack<ButtonPrompt>[3];

	public static ButtonPrompt GetButtonPrompt(InputAction inputAction, LocalizedString localizedString, Type promptType = Type.LeftCorner)
	{
		if (!SingletonBehaviour<ButtonPromptManager>.HasInstance)
		{
			return null;
		}
		return SingletonBehaviour<ButtonPromptManager>.Instance.GetButtonPromptInternal(inputAction, localizedString, promptType);
	}

	public static void ReturnButtonPrompt(ButtonPrompt instance)
	{
		if (SingletonBehaviour<ButtonPromptManager>.HasInstance)
		{
			SingletonBehaviour<ButtonPromptManager>.Instance.ReturnButtonPromptInternal(instance);
		}
	}

	private ButtonPrompt GetButtonPromptInternal(InputAction inputAction, LocalizedString localizedString, Type promptType)
	{
		ButtonPrompt buttonPrompt = null;
		Stack<ButtonPrompt> stack = availableInstances[(int)promptType];
		buttonPrompt = ((stack != null && stack.Count != 0) ? stack.Pop() : UnityEngine.Object.Instantiate(promptType switch
		{
			Type.LeftCorner => leftCornerPrefab, 
			Type.Center => centerPrefab, 
			Type.WorldSpace => worldSpacePrefab, 
			_ => throw new NotImplementedException(), 
		}));
		Transform parent = promptType switch
		{
			Type.LeftCorner => leftCorner, 
			Type.Center => center, 
			Type.WorldSpace => world, 
			_ => base.transform, 
		};
		buttonPrompt.transform.SetParent(parent);
		buttonPrompt.transform.localPosition = Vector3.zero;
		buttonPrompt.transform.localScale = Vector3.one;
		buttonPrompt.Initialize(inputAction, localizedString);
		buttonPrompt.promptType = promptType;
		return buttonPrompt;
	}

	private void ReturnButtonPromptInternal(ButtonPrompt instance)
	{
		Stack<ButtonPrompt> stack = availableInstances[(int)instance.promptType];
		if (stack == null)
		{
			stack = (availableInstances[(int)instance.promptType] = new Stack<ButtonPrompt>());
		}
		instance.transform.SetParent(pooled);
		stack.Push(instance);
	}
}
