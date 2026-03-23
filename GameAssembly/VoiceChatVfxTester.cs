using UnityEngine;
using UnityEngine.UI;

public class VoiceChatVfxTester : MonoBehaviour
{
	[SerializeField]
	private VoiceChatVfx vfx;

	[SerializeField]
	private Slider slider;

	[SerializeField]
	[Range(0f, 1f)]
	private float testStrengthInterpolation;

	private float previousTestValue = -1f;

	private bool previousPlaying;

	private bool playing;

	private void Start()
	{
		vfx.SetIntensity(0f);
		vfx.SetPlaying(playing);
		previousTestValue = testStrengthInterpolation;
		previousPlaying = playing;
	}

	private void Update()
	{
		if (previousTestValue != testStrengthInterpolation)
		{
			vfx.SetIntensity(testStrengthInterpolation);
			if (testStrengthInterpolation <= 0f)
			{
				playing = false;
			}
			else
			{
				playing = true;
			}
		}
		if (previousPlaying != playing)
		{
			vfx.SetPlaying(playing);
		}
		previousTestValue = testStrengthInterpolation;
		previousPlaying = playing;
		slider.value = testStrengthInterpolation;
	}

	public void SetTestInterpolation(float interpolation)
	{
		testStrengthInterpolation = interpolation;
	}
}
