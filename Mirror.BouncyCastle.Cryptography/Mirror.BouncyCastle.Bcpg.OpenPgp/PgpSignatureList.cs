namespace Mirror.BouncyCastle.Bcpg.OpenPgp;

public class PgpSignatureList : PgpObject
{
	private readonly PgpSignature[] sigs;

	public PgpSignature this[int index] => sigs[index];

	public int Count => sigs.Length;

	public bool IsEmpty => sigs.Length == 0;

	public PgpSignatureList(PgpSignature[] sigs)
	{
		this.sigs = (PgpSignature[])sigs.Clone();
	}

	public PgpSignatureList(PgpSignature sig)
	{
		sigs = new PgpSignature[1] { sig };
	}
}
