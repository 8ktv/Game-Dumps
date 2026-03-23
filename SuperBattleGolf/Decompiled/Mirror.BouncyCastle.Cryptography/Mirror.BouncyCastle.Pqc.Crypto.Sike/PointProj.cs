namespace Mirror.BouncyCastle.Pqc.Crypto.Sike;

internal sealed class PointProj
{
	internal ulong[][] X;

	internal ulong[][] Z;

	internal PointProj(uint nwords_field)
	{
		X = SikeUtilities.InitArray(2u, nwords_field);
		Z = SikeUtilities.InitArray(2u, nwords_field);
	}
}
