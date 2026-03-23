using System.IO;
using Mirror.BouncyCastle.Security;
using Mirror.BouncyCastle.Utilities.IO.Pem;

namespace Mirror.BouncyCastle.OpenSsl;

public class PemWriter : Mirror.BouncyCastle.Utilities.IO.Pem.PemWriter
{
	public PemWriter(TextWriter writer)
		: base(writer)
	{
	}

	public void WriteObject(object obj)
	{
		WriteObject(obj, null, null, null);
	}

	public void WriteObject(object obj, string algorithm, char[] password, SecureRandom random)
	{
		try
		{
			base.WriteObject((PemObjectGenerator)new MiscPemGenerator(obj, algorithm, password, random));
		}
		catch (PemGenerationException ex)
		{
			if (ex.InnerException is IOException ex2)
			{
				throw ex2;
			}
			throw;
		}
	}
}
