using System.IO;

namespace Mirror.BouncyCastle.Crypto;

public interface IStreamCalculator<out TResult>
{
	Stream Stream { get; }

	TResult GetResult();
}
