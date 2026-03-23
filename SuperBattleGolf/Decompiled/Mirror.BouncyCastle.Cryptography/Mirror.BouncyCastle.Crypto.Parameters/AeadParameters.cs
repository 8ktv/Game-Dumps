using System;

namespace Mirror.BouncyCastle.Crypto.Parameters;

public class AeadParameters : ICipherParameters
{
	private readonly byte[] associatedText;

	private readonly byte[] nonce;

	private readonly KeyParameter key;

	private readonly int macSize;

	public virtual KeyParameter Key => key;

	public virtual int MacSize => macSize;

	public AeadParameters(KeyParameter key, int macSize, byte[] nonce)
		: this(key, macSize, nonce, null)
	{
	}

	public AeadParameters(KeyParameter key, int macSize, byte[] nonce, byte[] associatedText)
	{
		if (nonce == null)
		{
			throw new ArgumentNullException("nonce");
		}
		this.key = key;
		this.nonce = nonce;
		this.macSize = macSize;
		this.associatedText = associatedText;
	}

	public virtual byte[] GetAssociatedText()
	{
		return associatedText;
	}

	public virtual byte[] GetNonce()
	{
		return (byte[])nonce.Clone();
	}
}
