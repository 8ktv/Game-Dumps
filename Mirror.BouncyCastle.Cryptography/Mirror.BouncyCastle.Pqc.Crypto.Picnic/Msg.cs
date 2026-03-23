namespace Mirror.BouncyCastle.Pqc.Crypto.Picnic;

internal class Msg
{
	internal byte[][] msgs;

	internal int pos;

	internal int unopened;

	internal Msg(PicnicEngine engine)
	{
		msgs = new byte[engine.numMPCParties][];
		for (int i = 0; i < engine.numMPCParties; i++)
		{
			msgs[i] = new byte[engine.andSizeBytes];
		}
		pos = 0;
		unopened = -1;
	}
}
