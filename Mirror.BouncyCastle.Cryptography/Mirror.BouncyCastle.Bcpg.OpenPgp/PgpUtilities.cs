using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Mirror.BouncyCastle.Asn1;
using Mirror.BouncyCastle.Asn1.EdEC;
using Mirror.BouncyCastle.Asn1.Sec;
using Mirror.BouncyCastle.Asn1.X9;
using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Crypto.Parameters;
using Mirror.BouncyCastle.Crypto.Signers;
using Mirror.BouncyCastle.Math;
using Mirror.BouncyCastle.Security;
using Mirror.BouncyCastle.Utilities;
using Mirror.BouncyCastle.Utilities.Encoders;
using Mirror.BouncyCastle.Utilities.IO;

namespace Mirror.BouncyCastle.Bcpg.OpenPgp;

public sealed class PgpUtilities
{
	private static readonly IDictionary<string, HashAlgorithmTag> NameToHashID = CreateNameToHashID();

	private static readonly IDictionary<DerObjectIdentifier, string> OidToName = CreateOidToName();

	private const int ReadAhead = 60;

	private static IDictionary<string, HashAlgorithmTag> CreateNameToHashID()
	{
		return new Dictionary<string, HashAlgorithmTag>(StringComparer.OrdinalIgnoreCase)
		{
			{
				"sha1",
				HashAlgorithmTag.Sha1
			},
			{
				"sha224",
				HashAlgorithmTag.Sha224
			},
			{
				"sha256",
				HashAlgorithmTag.Sha256
			},
			{
				"sha384",
				HashAlgorithmTag.Sha384
			},
			{
				"sha512",
				HashAlgorithmTag.Sha512
			},
			{
				"ripemd160",
				HashAlgorithmTag.RipeMD160
			},
			{
				"rmd160",
				HashAlgorithmTag.RipeMD160
			},
			{
				"md2",
				HashAlgorithmTag.MD2
			},
			{
				"tiger",
				HashAlgorithmTag.Tiger192
			},
			{
				"haval",
				HashAlgorithmTag.Haval5pass160
			},
			{
				"md5",
				HashAlgorithmTag.MD5
			}
		};
	}

	private static IDictionary<DerObjectIdentifier, string> CreateOidToName()
	{
		return new Dictionary<DerObjectIdentifier, string>
		{
			{
				EdECObjectIdentifiers.id_X25519,
				"Curve25519"
			},
			{
				EdECObjectIdentifiers.id_Ed25519,
				"Ed25519"
			},
			{
				SecObjectIdentifiers.SecP256r1,
				"NIST P-256"
			},
			{
				SecObjectIdentifiers.SecP384r1,
				"NIST P-384"
			},
			{
				SecObjectIdentifiers.SecP521r1,
				"NIST P-521"
			}
		};
	}

	public static MPInteger[] DsaSigToMpi(byte[] encoding)
	{
		DerInteger instance2;
		DerInteger instance3;
		try
		{
			Asn1Sequence instance = Asn1Sequence.GetInstance(encoding);
			instance2 = DerInteger.GetInstance(instance[0]);
			instance3 = DerInteger.GetInstance(instance[1]);
		}
		catch (Exception innerException)
		{
			throw new PgpException("exception encoding signature", innerException);
		}
		return new MPInteger[2]
		{
			new MPInteger(instance2.Value),
			new MPInteger(instance3.Value)
		};
	}

	public static MPInteger[] RsaSigToMpi(byte[] encoding)
	{
		return new MPInteger[1]
		{
			new MPInteger(new BigInteger(1, encoding))
		};
	}

	public static string GetDigestName(HashAlgorithmTag hashAlgorithm)
	{
		return hashAlgorithm switch
		{
			HashAlgorithmTag.Sha1 => "SHA1", 
			HashAlgorithmTag.MD2 => "MD2", 
			HashAlgorithmTag.MD5 => "MD5", 
			HashAlgorithmTag.RipeMD160 => "RIPEMD160", 
			HashAlgorithmTag.Sha224 => "SHA224", 
			HashAlgorithmTag.Sha256 => "SHA256", 
			HashAlgorithmTag.Sha384 => "SHA384", 
			HashAlgorithmTag.Sha512 => "SHA512", 
			_ => throw new PgpException("unknown hash algorithm tag in GetDigestName: " + hashAlgorithm), 
		};
	}

	public static int GetDigestIDForName(string name)
	{
		if (NameToHashID.TryGetValue(name, out var value))
		{
			return (int)value;
		}
		throw new ArgumentException("unable to map " + name + " to a hash id", "name");
	}

	public static string GetCurveName(DerObjectIdentifier oid)
	{
		if (OidToName.TryGetValue(oid, out var value))
		{
			return value;
		}
		return ECNamedCurveTable.GetName(oid);
	}

	public static string GetSignatureName(PublicKeyAlgorithmTag keyAlgorithm, HashAlgorithmTag hashAlgorithm)
	{
		string text;
		switch (keyAlgorithm)
		{
		case PublicKeyAlgorithmTag.RsaGeneral:
		case PublicKeyAlgorithmTag.RsaSign:
			text = "RSA";
			break;
		case PublicKeyAlgorithmTag.Dsa:
			text = "DSA";
			break;
		case PublicKeyAlgorithmTag.ECDH:
			text = "ECDH";
			break;
		case PublicKeyAlgorithmTag.ECDsa:
			text = "ECDSA";
			break;
		case PublicKeyAlgorithmTag.EdDsa:
			text = "EdDSA";
			break;
		case PublicKeyAlgorithmTag.ElGamalEncrypt:
		case PublicKeyAlgorithmTag.ElGamalGeneral:
			text = "ElGamal";
			break;
		default:
			throw new PgpException("unknown algorithm tag in signature:" + keyAlgorithm);
		}
		return GetDigestName(hashAlgorithm) + "with" + text;
	}

	public static string GetSymmetricCipherName(SymmetricKeyAlgorithmTag algorithm)
	{
		return algorithm switch
		{
			SymmetricKeyAlgorithmTag.Null => null, 
			SymmetricKeyAlgorithmTag.TripleDes => "DESEDE", 
			SymmetricKeyAlgorithmTag.Idea => "IDEA", 
			SymmetricKeyAlgorithmTag.Cast5 => "CAST5", 
			SymmetricKeyAlgorithmTag.Blowfish => "Blowfish", 
			SymmetricKeyAlgorithmTag.Safer => "SAFER", 
			SymmetricKeyAlgorithmTag.Des => "DES", 
			SymmetricKeyAlgorithmTag.Aes128 => "AES", 
			SymmetricKeyAlgorithmTag.Aes192 => "AES", 
			SymmetricKeyAlgorithmTag.Aes256 => "AES", 
			SymmetricKeyAlgorithmTag.Twofish => "Twofish", 
			SymmetricKeyAlgorithmTag.Camellia128 => "Camellia", 
			SymmetricKeyAlgorithmTag.Camellia192 => "Camellia", 
			SymmetricKeyAlgorithmTag.Camellia256 => "Camellia", 
			_ => throw new PgpException("unknown symmetric algorithm: " + algorithm), 
		};
	}

	public static int GetKeySize(SymmetricKeyAlgorithmTag algorithm)
	{
		switch (algorithm)
		{
		case SymmetricKeyAlgorithmTag.Des:
			return 64;
		case SymmetricKeyAlgorithmTag.Idea:
		case SymmetricKeyAlgorithmTag.Cast5:
		case SymmetricKeyAlgorithmTag.Blowfish:
		case SymmetricKeyAlgorithmTag.Safer:
		case SymmetricKeyAlgorithmTag.Aes128:
		case SymmetricKeyAlgorithmTag.Camellia128:
			return 128;
		case SymmetricKeyAlgorithmTag.TripleDes:
		case SymmetricKeyAlgorithmTag.Aes192:
		case SymmetricKeyAlgorithmTag.Camellia192:
			return 192;
		case SymmetricKeyAlgorithmTag.Aes256:
		case SymmetricKeyAlgorithmTag.Twofish:
		case SymmetricKeyAlgorithmTag.Camellia256:
			return 256;
		default:
			throw new PgpException("unknown symmetric algorithm: " + algorithm);
		}
	}

	public static KeyParameter MakeKey(SymmetricKeyAlgorithmTag algorithm, byte[] keyBytes)
	{
		return ParameterUtilities.CreateKeyParameter(GetSymmetricCipherName(algorithm), keyBytes);
	}

	public static KeyParameter MakeRandomKey(SymmetricKeyAlgorithmTag algorithm, SecureRandom random)
	{
		byte[] array = new byte[(GetKeySize(algorithm) + 7) / 8];
		random.NextBytes(array);
		return MakeKey(algorithm, array);
	}

	internal static byte[] EncodePassPhrase(char[] passPhrase, bool utf8)
	{
		if (passPhrase != null)
		{
			if (!utf8)
			{
				return Strings.ToByteArray(passPhrase);
			}
			return Encoding.UTF8.GetBytes(passPhrase);
		}
		return null;
	}

	public static KeyParameter MakeKeyFromPassPhrase(SymmetricKeyAlgorithmTag algorithm, S2k s2k, char[] passPhrase)
	{
		return DoMakeKeyFromPassPhrase(algorithm, s2k, EncodePassPhrase(passPhrase, utf8: false), clearPassPhrase: true);
	}

	public static KeyParameter MakeKeyFromPassPhraseUtf8(SymmetricKeyAlgorithmTag algorithm, S2k s2k, char[] passPhrase)
	{
		return DoMakeKeyFromPassPhrase(algorithm, s2k, EncodePassPhrase(passPhrase, utf8: true), clearPassPhrase: true);
	}

	public static KeyParameter MakeKeyFromPassPhraseRaw(SymmetricKeyAlgorithmTag algorithm, S2k s2k, byte[] rawPassPhrase)
	{
		return DoMakeKeyFromPassPhrase(algorithm, s2k, rawPassPhrase, clearPassPhrase: false);
	}

	internal static KeyParameter DoMakeKeyFromPassPhrase(SymmetricKeyAlgorithmTag algorithm, S2k s2k, byte[] rawPassPhrase, bool clearPassPhrase)
	{
		byte[] array = new byte[(GetKeySize(algorithm) + 7) / 8];
		int num = 0;
		int num2 = 0;
		while (num < array.Length)
		{
			IDigest digest;
			if (s2k != null)
			{
				try
				{
					digest = CreateDigest(s2k.HashAlgorithm);
				}
				catch (Exception innerException)
				{
					throw new PgpException("can't find S2k digest", innerException);
				}
				for (int i = 0; i != num2; i++)
				{
					digest.Update(0);
				}
				byte[] iV = s2k.GetIV();
				switch (s2k.Type)
				{
				case 0:
					digest.BlockUpdate(rawPassPhrase, 0, rawPassPhrase.Length);
					break;
				case 1:
					digest.BlockUpdate(iV, 0, iV.Length);
					digest.BlockUpdate(rawPassPhrase, 0, rawPassPhrase.Length);
					break;
				case 3:
				{
					long iterationCount = s2k.IterationCount;
					digest.BlockUpdate(iV, 0, iV.Length);
					digest.BlockUpdate(rawPassPhrase, 0, rawPassPhrase.Length);
					iterationCount -= iV.Length + rawPassPhrase.Length;
					while (iterationCount > 0)
					{
						if (iterationCount < iV.Length)
						{
							digest.BlockUpdate(iV, 0, (int)iterationCount);
							break;
						}
						digest.BlockUpdate(iV, 0, iV.Length);
						iterationCount -= iV.Length;
						if (iterationCount < rawPassPhrase.Length)
						{
							digest.BlockUpdate(rawPassPhrase, 0, (int)iterationCount);
							iterationCount = 0L;
						}
						else
						{
							digest.BlockUpdate(rawPassPhrase, 0, rawPassPhrase.Length);
							iterationCount -= rawPassPhrase.Length;
						}
					}
					break;
				}
				default:
					throw new PgpException("unknown S2k type: " + s2k.Type);
				}
			}
			else
			{
				try
				{
					digest = CreateDigest(HashAlgorithmTag.MD5);
					for (int j = 0; j != num2; j++)
					{
						digest.Update(0);
					}
					digest.BlockUpdate(rawPassPhrase, 0, rawPassPhrase.Length);
				}
				catch (Exception innerException2)
				{
					throw new PgpException("can't find MD5 digest", innerException2);
				}
			}
			byte[] array2 = DigestUtilities.DoFinal(digest);
			if (array2.Length > array.Length - num)
			{
				Array.Copy(array2, 0, array, num, array.Length - num);
			}
			else
			{
				Array.Copy(array2, 0, array, num, array2.Length);
			}
			num += array2.Length;
			num2++;
		}
		if (clearPassPhrase && rawPassPhrase != null)
		{
			Array.Clear(rawPassPhrase, 0, rawPassPhrase.Length);
		}
		return MakeKey(algorithm, array);
	}

	public static void WriteFileToLiteralData(Stream output, char fileType, FileInfo file)
	{
		using Stream pOut = new PgpLiteralDataGenerator().Open(output, fileType, file.Name, file.Length, file.LastWriteTime);
		PipeFileContents(file, pOut);
	}

	public static void WriteFileToLiteralData(Stream output, char fileType, FileInfo file, byte[] buffer)
	{
		using Stream pOut = new PgpLiteralDataGenerator().Open(output, fileType, file.Name, file.LastWriteTime, buffer);
		PipeFileContents(file, pOut, buffer.Length);
	}

	private static void PipeFileContents(FileInfo file, Stream pOut)
	{
		PipeFileContents(file, pOut, Streams.DefaultBufferSize);
	}

	private static void PipeFileContents(FileInfo file, Stream pOut, int bufferSize)
	{
		using FileStream source = file.OpenRead();
		Streams.CopyTo(source, pOut, bufferSize);
	}

	private static bool IsPossiblyBase64(int ch)
	{
		if ((ch < 65 || ch > 90) && (ch < 97 || ch > 122) && (ch < 48 || ch > 57) && ch != 43 && ch != 47 && ch != 13)
		{
			return ch == 10;
		}
		return true;
	}

	public static Stream GetDecoderStream(Stream inputStream)
	{
		if (!inputStream.CanSeek)
		{
			throw new ArgumentException("inputStream must be seek-able", "inputStream");
		}
		long position = inputStream.Position;
		int num = inputStream.ReadByte();
		if ((num & 0x80) != 0)
		{
			inputStream.Position = position;
			return inputStream;
		}
		if (!IsPossiblyBase64(num))
		{
			inputStream.Position = position;
			return new ArmoredInputStream(inputStream);
		}
		byte[] array = new byte[60];
		int i = 1;
		int num2 = 1;
		array[0] = (byte)num;
		for (; i != 60; i++)
		{
			if ((num = inputStream.ReadByte()) < 0)
			{
				break;
			}
			if (!IsPossiblyBase64(num))
			{
				inputStream.Position = position;
				return new ArmoredInputStream(inputStream);
			}
			if (num != 10 && num != 13)
			{
				array[num2++] = (byte)num;
			}
		}
		inputStream.Position = position;
		if (i < 4)
		{
			return new ArmoredInputStream(inputStream);
		}
		byte[] array2 = new byte[8];
		Array.Copy(array, 0, array2, 0, array2.Length);
		try
		{
			bool hasHeaders = (Base64.Decode(array2)[0] & 0x80) == 0;
			return new ArmoredInputStream(inputStream, hasHeaders);
		}
		catch (IOException)
		{
			throw;
		}
		catch (Exception ex2)
		{
			throw new IOException(ex2.Message);
		}
	}

	internal static IDigest CreateDigest(HashAlgorithmTag hashAlgorithm)
	{
		return DigestUtilities.GetDigest(GetDigestName(hashAlgorithm));
	}

	internal static ISigner CreateSigner(PublicKeyAlgorithmTag publicKeyAlgorithm, HashAlgorithmTag hashAlgorithm, AsymmetricKeyParameter key)
	{
		if (publicKeyAlgorithm == PublicKeyAlgorithmTag.EdDsa)
		{
			ISigner signer;
			if (key is Ed25519PrivateKeyParameters || key is Ed25519PublicKeyParameters)
			{
				signer = new Ed25519Signer();
			}
			else
			{
				if (!(key is Ed448PrivateKeyParameters) && !(key is Ed448PublicKeyParameters))
				{
					throw new InvalidOperationException();
				}
				signer = new Ed448Signer(Arrays.EmptyBytes);
			}
			return new EdDsaSigner(signer, CreateDigest(hashAlgorithm));
		}
		return SignerUtilities.GetSigner(GetSignatureName(publicKeyAlgorithm, hashAlgorithm));
	}

	internal static IWrapper CreateWrapper(SymmetricKeyAlgorithmTag encAlgorithm)
	{
		switch (encAlgorithm)
		{
		case SymmetricKeyAlgorithmTag.Aes128:
		case SymmetricKeyAlgorithmTag.Aes192:
		case SymmetricKeyAlgorithmTag.Aes256:
			return WrapperUtilities.GetWrapper("AESWRAP");
		case SymmetricKeyAlgorithmTag.Camellia128:
		case SymmetricKeyAlgorithmTag.Camellia192:
		case SymmetricKeyAlgorithmTag.Camellia256:
			return WrapperUtilities.GetWrapper("CAMELLIAWRAP");
		default:
			throw new PgpException("unknown wrap algorithm: " + encAlgorithm);
		}
	}

	internal static byte[] GenerateIV(int length, SecureRandom random)
	{
		byte[] array = new byte[length];
		random.NextBytes(array);
		return array;
	}

	internal static S2k GenerateS2k(HashAlgorithmTag hashAlgorithm, int s2kCount, SecureRandom random)
	{
		byte[] iv = GenerateIV(8, random);
		return new S2k(hashAlgorithm, iv, s2kCount);
	}
}
