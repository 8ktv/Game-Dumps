using System;

[Flags]
public enum TutorialPromptCategory
{
	None = 0,
	Basics = 1,
	Ball = 2,
	Item = 4,
	Green = 8,
	Scoreboard = 0x10
}
