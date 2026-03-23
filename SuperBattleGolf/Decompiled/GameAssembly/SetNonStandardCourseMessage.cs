using Mirror;

public struct SetNonStandardCourseMessage : NetworkMessage
{
	public int[] globalHoleIndices;

	public bool isRandom;
}
