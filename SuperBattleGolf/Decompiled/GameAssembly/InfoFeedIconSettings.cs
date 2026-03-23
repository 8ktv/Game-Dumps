using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Info feed icon settings", menuName = "Settings/UI/Info feed icons")]
public class InfoFeedIconSettings : ScriptableObject
{
	public enum Type
	{
		None = 0,
		Elimination = 1,
		SwingHit = 2,
		ProjectileHit = 3,
		FallKnockout = 4,
		TimedOut = 5,
		FellIntoWater = 6,
		OutOfBounds = 7,
		ReturnedBallKnockout = 9,
		DuelingPistol = 10,
		Dive = 11,
		ElephantGun = 12,
		GolfCart = 13,
		Rocket = 14,
		RocketBackBlast = 15,
		Landmine = 19,
		ReflectedSwingProjectile = 20,
		ReflectedRocket = 21,
		DeflectedDuelingPistolShot = 22,
		DeflectedElephantGunShot = 23,
		FellIntoFog = 24,
		OrbitalLaserCenter = 25,
		OrbitalLaserPeriphery = 26,
		FellIntoHole = 27
	}

	[Serializable]
	private struct KnockoutIconPair
	{
		public KnockoutType knockoutType;

		public Type iconType;
	}

	[Serializable]
	private struct EliminationIconPair
	{
		public EliminationReason eliminationReason;

		public Type iconType;
	}

	[Serializable]
	private struct Icon
	{
		public Type type;

		public Sprite icon;
	}

	[SerializeField]
	[DynamicElementName("knockoutType")]
	private KnockoutIconPair[] knockoutIconTypes;

	[SerializeField]
	[DynamicElementName("eliminationReason")]
	private EliminationIconPair[] eliminationIconTypes;

	[SerializeField]
	[DynamicElementName("type")]
	private Icon[] icons;

	private readonly Dictionary<KnockoutType, Type> knockoutIconTypeDictionary = new Dictionary<KnockoutType, Type>();

	private readonly Dictionary<EliminationReason, Type> eliminationIconTypeDictionary = new Dictionary<EliminationReason, Type>();

	private readonly Dictionary<Type, Sprite> iconDictionary = new Dictionary<Type, Sprite>();

	private void OnValidate()
	{
		Initialize();
	}

	private void OnEnable()
	{
		Initialize();
	}

	private void Initialize()
	{
		knockoutIconTypeDictionary.Clear();
		KnockoutIconPair[] array = knockoutIconTypes;
		for (int i = 0; i < array.Length; i++)
		{
			KnockoutIconPair knockoutIconPair = array[i];
			knockoutIconTypeDictionary.Add(knockoutIconPair.knockoutType, knockoutIconPair.iconType);
		}
		eliminationIconTypeDictionary.Clear();
		EliminationIconPair[] array2 = eliminationIconTypes;
		for (int i = 0; i < array2.Length; i++)
		{
			EliminationIconPair eliminationIconPair = array2[i];
			eliminationIconTypeDictionary.Add(eliminationIconPair.eliminationReason, eliminationIconPair.iconType);
		}
		iconDictionary.Clear();
		Icon[] array3 = icons;
		for (int i = 0; i < array3.Length; i++)
		{
			Icon icon = array3[i];
			iconDictionary.Add(icon.type, icon.icon);
		}
	}

	public bool TryGetIconType(KnockoutType knockoutType, out Type iconType)
	{
		return knockoutIconTypeDictionary.TryGetValue(knockoutType, out iconType);
	}

	public bool TryGetIconType(EliminationReason eliminationReason, out Type iconType)
	{
		return eliminationIconTypeDictionary.TryGetValue(eliminationReason, out iconType);
	}

	public bool TryGetIcon(Type iconType, out Sprite icon)
	{
		return iconDictionary.TryGetValue(iconType, out icon);
	}
}
