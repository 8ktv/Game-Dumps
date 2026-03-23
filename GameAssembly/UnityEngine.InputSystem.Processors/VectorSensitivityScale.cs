using System;
using System.ComponentModel;

namespace UnityEngine.InputSystem.Processors;

[Serializable]
[DisplayName("VectorSensitivityScale")]
public class VectorSensitivityScale : InputProcessor<Vector2>
{
	public static float ScaleFactor;

	public override Vector2 Process(Vector2 value, InputControl control)
	{
		return new Vector2(value.x * ScaleFactor, value.y * ScaleFactor);
	}

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
	private static void Register()
	{
		if (InputSystem.TryGetProcessor("VectorSensitivityScale") == null)
		{
			InputSystem.RegisterProcessor<VectorSensitivityScale>("VectorSensitivityScale");
		}
	}

	static VectorSensitivityScale()
	{
		ScaleFactor = 1f;
		Register();
	}
}
