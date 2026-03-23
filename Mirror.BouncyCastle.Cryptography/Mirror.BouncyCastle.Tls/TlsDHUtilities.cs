using System.IO;
using Mirror.BouncyCastle.Math;
using Mirror.BouncyCastle.Tls.Crypto;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Tls;

public abstract class TlsDHUtilities
{
	public static TlsDHConfig CreateNamedDHConfig(TlsContext context, int namedGroup)
	{
		if (namedGroup < 0 || NamedGroup.GetFiniteFieldBits(namedGroup) < 1)
		{
			return null;
		}
		bool padded = TlsUtilities.IsTlsV13(context);
		return new TlsDHConfig(namedGroup, padded);
	}

	public static DHGroup GetDHGroup(TlsDHConfig dhConfig)
	{
		int namedGroup = dhConfig.NamedGroup;
		if (namedGroup >= 0)
		{
			return GetNamedDHGroup(namedGroup);
		}
		return dhConfig.ExplicitGroup;
	}

	public static DHGroup GetNamedDHGroup(int namedGroup)
	{
		return namedGroup switch
		{
			256 => DHStandardGroups.rfc7919_ffdhe2048, 
			257 => DHStandardGroups.rfc7919_ffdhe3072, 
			258 => DHStandardGroups.rfc7919_ffdhe4096, 
			259 => DHStandardGroups.rfc7919_ffdhe6144, 
			260 => DHStandardGroups.rfc7919_ffdhe8192, 
			_ => null, 
		};
	}

	public static int GetMinimumFiniteFieldBits(int cipherSuite)
	{
		return IsDHCipherSuite(cipherSuite) ? 1 : 0;
	}

	public static bool IsDHCipherSuite(int cipherSuite)
	{
		switch (TlsUtilities.GetKeyExchangeAlgorithm(cipherSuite))
		{
		case 3:
		case 5:
		case 7:
		case 9:
		case 11:
		case 14:
			return true;
		default:
			return false;
		}
	}

	public static int GetNamedGroupForDHParameters(BigInteger p, BigInteger g)
	{
		int[] array = new int[5] { 256, 257, 258, 259, 260 };
		foreach (int num in array)
		{
			DHGroup namedDHGroup = GetNamedDHGroup(num);
			if (namedDHGroup != null && namedDHGroup.P.Equals(p) && namedDHGroup.G.Equals(g))
			{
				return num;
			}
		}
		return -1;
	}

	public static DHGroup GetStandardGroupForDHParameters(BigInteger p, BigInteger g)
	{
		DHGroup[] array = new DHGroup[13]
		{
			DHStandardGroups.rfc7919_ffdhe2048,
			DHStandardGroups.rfc7919_ffdhe3072,
			DHStandardGroups.rfc7919_ffdhe4096,
			DHStandardGroups.rfc7919_ffdhe6144,
			DHStandardGroups.rfc7919_ffdhe8192,
			DHStandardGroups.rfc3526_1536,
			DHStandardGroups.rfc3526_2048,
			DHStandardGroups.rfc3526_3072,
			DHStandardGroups.rfc3526_4096,
			DHStandardGroups.rfc3526_6144,
			DHStandardGroups.rfc3526_8192,
			DHStandardGroups.rfc5996_768,
			DHStandardGroups.rfc5996_1024
		};
		foreach (DHGroup dHGroup in array)
		{
			if (dHGroup != null && dHGroup.P.Equals(p) && dHGroup.G.Equals(g))
			{
				return dHGroup;
			}
		}
		return null;
	}

	public static TlsDHConfig ReceiveDHConfig(TlsContext context, TlsDHGroupVerifier dhGroupVerifier, Stream input)
	{
		BigInteger p = ReadDHParameter(input);
		BigInteger g = ReadDHParameter(input);
		int namedGroupForDHParameters = GetNamedGroupForDHParameters(p, g);
		if (namedGroupForDHParameters < 0)
		{
			DHGroup dHGroup = GetStandardGroupForDHParameters(p, g);
			if (dHGroup == null)
			{
				dHGroup = new DHGroup(p, null, g, 0);
			}
			if (!dhGroupVerifier.Accept(dHGroup))
			{
				throw new TlsFatalAlert(71);
			}
			return new TlsDHConfig(dHGroup);
		}
		int[] clientSupportedGroups = context.SecurityParameters.ClientSupportedGroups;
		if (clientSupportedGroups == null || Arrays.Contains(clientSupportedGroups, namedGroupForDHParameters))
		{
			return new TlsDHConfig(namedGroupForDHParameters, padded: false);
		}
		throw new TlsFatalAlert(47);
	}

	public static BigInteger ReadDHParameter(Stream input)
	{
		return new BigInteger(1, TlsUtilities.ReadOpaque16(input, 1));
	}

	public static void WriteDHConfig(TlsDHConfig dhConfig, Stream output)
	{
		DHGroup dHGroup = GetDHGroup(dhConfig);
		WriteDHParameter(dHGroup.P, output);
		WriteDHParameter(dHGroup.G, output);
	}

	public static void WriteDHParameter(BigInteger x, Stream output)
	{
		TlsUtilities.WriteOpaque16(BigIntegers.AsUnsignedByteArray(x), output);
	}
}
