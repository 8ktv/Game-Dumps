public static class StatusEffectExtensions
{
	public static bool HasEffect(this StatusEffect statusEffect, StatusEffect effectToCheck)
	{
		return (statusEffect & effectToCheck) != 0;
	}
}
