using System;

[Flags]
public enum TutorialPrompt
{
	None = 0,
	LookAround = 1,
	Move = 2,
	Jump = 4,
	Dive = 8,
	Interact = 0x10,
	AimSwing = 0x20,
	AdjustAngle = 0x40,
	ChargeSwing = 0x80,
	HomingShot = 0x100,
	CancelSwing = 0x200,
	SelectItem = 0x400,
	DropItem = 0x800,
	Putt = 0x1000,
	ViewScore = 0x2000,
	OptimalAngle = 0x4000,
	HasProgress = 0x31A3
}
