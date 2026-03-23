using System;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public static class Volume
{
	private static Bus masterBus;

	private static Bus musicBus;

	private static Bus voiceBus;

	private static Bus sfxBus;

	static Volume()
	{
		try
		{
			masterBus = RuntimeManager.GetBus("bus:/");
			musicBus = RuntimeManager.GetBus("bus:/Music");
			voiceBus = RuntimeManager.GetBus("bus:/Voice");
			sfxBus = RuntimeManager.GetBus("bus:/SFX");
		}
		catch (Exception exception)
		{
			Debug.LogError("Fmod threw an exception while getting fmod buses");
			Debug.LogException(exception);
		}
	}

	public static float GetMasterVolume()
	{
		masterBus.getVolume(out var volume);
		return volume;
	}

	public static float GetMusicVolume()
	{
		musicBus.getVolume(out var volume);
		return volume;
	}

	public static float GetVoiceVolume()
	{
		voiceBus.getVolume(out var volume);
		return volume;
	}

	public static float GetEffectsVolume()
	{
		sfxBus.getVolume(out var volume);
		return volume;
	}

	[CCommand("setMasterVolume", "Set the overall volume of the game", false, false)]
	public static void SetMasterVolume(float volume)
	{
		masterBus.setVolume(volume);
	}

	[CCommand("setMusicVolume", "Set only the volume of the music", false, false)]
	public static void SetMusicVolume(float volume)
	{
		musicBus.setVolume(volume);
	}

	[CCommand("setAmbienceVolume", "Set only the volume of the ambience", false, false)]
	public static void SetVoiceVolume(float volume)
	{
		voiceBus.setVolume(volume);
	}
}
