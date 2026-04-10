using System;

[Flags]
public enum InputMode
{
	None = 0,
	Regular = 2,
	GolfCartDriver = 4,
	GolfCartPassenger = 8,
	Spectate = 0x10,
	Paused = 0x20,
	DevConsole = 0x40,
	TextChat = 0x80,
	MatchSetup = 0x100,
	MainMenu = 0x200,
	FullScreenMessage = 0x400,
	SteamOverlay = 0x800,
	ForceDisabled = 0x1000
}
