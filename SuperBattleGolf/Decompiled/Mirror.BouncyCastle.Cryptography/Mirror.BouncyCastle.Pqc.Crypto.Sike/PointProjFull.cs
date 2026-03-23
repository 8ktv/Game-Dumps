namespace Mirror.BouncyCastle.Pqc.Crypto.Sike;

internal sealed class PointProjFull
{
	internal ulong[][] X;

	internal ulong[][] Y;

	internal ulong[][] Z;

	internal PointProjFull(uint nwords_field)
	{
		X = SikeUtilities.InitArray(2u, nwords_field);
		Y = SikeUtilities.InitArray(2u, nwords_field);
		Z = SikeUtilities.InitArray(2u, nwords_field);
	}
}
