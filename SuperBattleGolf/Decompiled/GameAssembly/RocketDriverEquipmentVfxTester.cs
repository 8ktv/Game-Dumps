using UnityEngine;

public class RocketDriverEquipmentVfxTester : MonoBehaviour
{
	[SerializeField]
	private RocketDriverEquipmentVfx vfx;

	[SerializeField]
	[Range(0f, 1f)]
	private float testInterpolation;

	[SerializeField]
	private bool testOvercharged;

	private float previousInterpolation = -1f;

	private bool previousOvercharged = true;

	private void Update()
	{
		if (previousInterpolation != testInterpolation)
		{
			previousInterpolation = testInterpolation;
			vfx.SetThrusterPower(testInterpolation);
		}
		if (previousOvercharged != testOvercharged)
		{
			previousOvercharged = testOvercharged;
			vfx.SetOvercharged(testOvercharged);
		}
	}

	public void OnSliderValueChanged(float slider)
	{
		testInterpolation = slider;
	}

	public void OnToggleValueChanged(bool toggle)
	{
		testOvercharged = toggle;
	}
}
