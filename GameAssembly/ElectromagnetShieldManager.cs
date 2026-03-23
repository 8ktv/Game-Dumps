using System.Collections.Generic;

public class ElectromagnetShieldManager : SingletonBehaviour<ElectromagnetShieldManager>
{
	private readonly HashSet<PlayerInfo> activeShields = new HashSet<PlayerInfo>();

	public static HashSet<PlayerInfo> ActiveShields
	{
		get
		{
			if (!SingletonBehaviour<ElectromagnetShieldManager>.HasInstance)
			{
				return null;
			}
			return SingletonBehaviour<ElectromagnetShieldManager>.Instance.activeShields;
		}
	}

	public static void RegisterActiveShield(PlayerInfo shieldOwner)
	{
		if (SingletonBehaviour<ElectromagnetShieldManager>.HasInstance)
		{
			SingletonBehaviour<ElectromagnetShieldManager>.Instance.RegisterActiveShieldInternal(shieldOwner);
		}
	}

	public static void DeregisterActiveShield(PlayerInfo shieldOwner)
	{
		if (SingletonBehaviour<ElectromagnetShieldManager>.HasInstance)
		{
			SingletonBehaviour<ElectromagnetShieldManager>.Instance.DeregisterActiveShieldInternal(shieldOwner);
		}
	}

	private void RegisterActiveShieldInternal(PlayerInfo shieldOwner)
	{
		activeShields.Add(shieldOwner);
	}

	private void DeregisterActiveShieldInternal(PlayerInfo shieldOwner)
	{
		activeShields.Remove(shieldOwner);
	}
}
