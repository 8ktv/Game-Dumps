public static class InputModeExtensions
{
	public static bool HasMode(this InputMode inputMode, InputMode modeToCheck)
	{
		return (inputMode & modeToCheck) != 0;
	}

	public static bool DisablesUiInputModule(this InputMode inputMode)
	{
		return inputMode.HasMode(InputMode.SteamOverlay | InputMode.ForceDisabled);
	}
}
