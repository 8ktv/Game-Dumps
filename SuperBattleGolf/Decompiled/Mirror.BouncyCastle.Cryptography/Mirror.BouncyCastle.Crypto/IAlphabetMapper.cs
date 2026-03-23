namespace Mirror.BouncyCastle.Crypto;

public interface IAlphabetMapper
{
	int Radix { get; }

	byte[] ConvertToIndexes(char[] input);

	char[] ConvertToChars(byte[] input);
}
