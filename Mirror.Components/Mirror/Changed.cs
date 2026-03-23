using System;

namespace Mirror;

[Flags]
public enum Changed : byte
{
	None = 0,
	PosX = 1,
	PosY = 2,
	PosZ = 4,
	CompressRot = 8,
	RotX = 0x10,
	RotY = 0x20,
	RotZ = 0x40,
	Scale = 0x80,
	Pos = 7,
	Rot = 0x70
}
