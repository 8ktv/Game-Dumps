using System;
using System.Collections.Generic;
using Mirror.BouncyCastle.Asn1.Nist;
using Mirror.BouncyCastle.Asn1.Oiw;

namespace Mirror.BouncyCastle.Crypto.Operators;

internal class KeyWrapperUtil
{
	private static readonly Dictionary<string, WrapperProvider> m_providerMap;

	static KeyWrapperUtil()
	{
		m_providerMap = new Dictionary<string, WrapperProvider>(StringComparer.OrdinalIgnoreCase);
		m_providerMap.Add("RSA/ECB/PKCS1PADDING", new RsaOaepWrapperProvider(OiwObjectIdentifiers.IdSha1));
		m_providerMap.Add("RSA/NONE/PKCS1PADDING", new RsaOaepWrapperProvider(OiwObjectIdentifiers.IdSha1));
		m_providerMap.Add("RSA/NONE/OAEPWITHSHA1ANDMGF1PADDING", new RsaOaepWrapperProvider(OiwObjectIdentifiers.IdSha1));
		m_providerMap.Add("RSA/NONE/OAEPWITHSHA224ANDMGF1PADDING", new RsaOaepWrapperProvider(NistObjectIdentifiers.IdSha224));
		m_providerMap.Add("RSA/NONE/OAEPWITHSHA256ANDMGF1PADDING", new RsaOaepWrapperProvider(NistObjectIdentifiers.IdSha256));
		m_providerMap.Add("RSA/NONE/OAEPWITHSHA384ANDMGF1PADDING", new RsaOaepWrapperProvider(NistObjectIdentifiers.IdSha384));
		m_providerMap.Add("RSA/NONE/OAEPWITHSHA512ANDMGF1PADDING", new RsaOaepWrapperProvider(NistObjectIdentifiers.IdSha512));
		m_providerMap.Add("RSA/NONE/OAEPWITHSHA256ANDMGF1WITHSHA1PADDING", new RsaOaepWrapperProvider(NistObjectIdentifiers.IdSha256, OiwObjectIdentifiers.IdSha1));
	}

	public static IKeyWrapper WrapperForName(string algorithm, ICipherParameters parameters)
	{
		if (!m_providerMap.TryGetValue(algorithm, out var value))
		{
			throw new ArgumentException("could not resolve " + algorithm + " to a KeyWrapper");
		}
		return (IKeyWrapper)value.CreateWrapper(forWrapping: true, parameters);
	}

	public static IKeyUnwrapper UnwrapperForName(string algorithm, ICipherParameters parameters)
	{
		if (!m_providerMap.TryGetValue(algorithm, out var value))
		{
			throw new ArgumentException("could not resolve " + algorithm + " to a KeyUnwrapper");
		}
		return (IKeyUnwrapper)value.CreateWrapper(forWrapping: false, parameters);
	}
}
