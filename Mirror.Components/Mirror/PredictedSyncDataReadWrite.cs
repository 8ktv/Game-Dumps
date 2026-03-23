namespace Mirror;

public static class PredictedSyncDataReadWrite
{
	public static void WritePredictedGolfCartSyncData(this NetworkWriter writer, PredictedGolfCartSyncData data)
	{
		writer.WriteBlittable(data);
	}

	public static PredictedGolfCartSyncData ReadPredictedGolfCartSyncData(this NetworkReader reader)
	{
		return reader.ReadBlittable<PredictedGolfCartSyncData>();
	}
}
