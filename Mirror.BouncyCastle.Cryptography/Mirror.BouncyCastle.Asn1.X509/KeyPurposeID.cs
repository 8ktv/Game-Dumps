using System;

namespace Mirror.BouncyCastle.Asn1.X509;

public sealed class KeyPurposeID : DerObjectIdentifier
{
	private const string id_kp = "1.3.6.1.5.5.7.3";

	public static readonly KeyPurposeID AnyExtendedKeyUsage = new KeyPurposeID(X509Extensions.ExtendedKeyUsage.Id + ".0");

	public static readonly KeyPurposeID id_kp_serverAuth = new KeyPurposeID("1.3.6.1.5.5.7.3.1");

	public static readonly KeyPurposeID id_kp_clientAuth = new KeyPurposeID("1.3.6.1.5.5.7.3.2");

	public static readonly KeyPurposeID id_kp_codeSigning = new KeyPurposeID("1.3.6.1.5.5.7.3.3");

	public static readonly KeyPurposeID id_kp_emailProtection = new KeyPurposeID("1.3.6.1.5.5.7.3.4");

	public static readonly KeyPurposeID id_kp_ipsecEndSystem = new KeyPurposeID("1.3.6.1.5.5.7.3.5");

	public static readonly KeyPurposeID id_kp_ipsecTunnel = new KeyPurposeID("1.3.6.1.5.5.7.3.6");

	public static readonly KeyPurposeID id_kp_ipsecUser = new KeyPurposeID("1.3.6.1.5.5.7.3.7");

	public static readonly KeyPurposeID id_kp_timeStamping = new KeyPurposeID("1.3.6.1.5.5.7.3.8");

	public static readonly KeyPurposeID id_kp_OCSPSigning = new KeyPurposeID("1.3.6.1.5.5.7.3.9");

	public static readonly KeyPurposeID id_kp_dvcs = new KeyPurposeID("1.3.6.1.5.5.7.3.10");

	public static readonly KeyPurposeID id_kp_sbgpCertAAServerAuth = new KeyPurposeID("1.3.6.1.5.5.7.3.11");

	public static readonly KeyPurposeID id_kp_scvp_responder = new KeyPurposeID("1.3.6.1.5.5.7.3.12");

	public static readonly KeyPurposeID id_kp_eapOverPPP = new KeyPurposeID("1.3.6.1.5.5.7.3.13");

	public static readonly KeyPurposeID id_kp_eapOverLAN = new KeyPurposeID("1.3.6.1.5.5.7.3.14");

	public static readonly KeyPurposeID id_kp_scvpServer = new KeyPurposeID("1.3.6.1.5.5.7.3.15");

	public static readonly KeyPurposeID id_kp_scvpClient = new KeyPurposeID("1.3.6.1.5.5.7.3.16");

	public static readonly KeyPurposeID id_kp_ipsecIKE = new KeyPurposeID("1.3.6.1.5.5.7.3.17");

	public static readonly KeyPurposeID id_kp_capwapAC = new KeyPurposeID("1.3.6.1.5.5.7.3.18");

	public static readonly KeyPurposeID id_kp_capwapWTP = new KeyPurposeID("1.3.6.1.5.5.7.3.19");

	public static readonly KeyPurposeID id_kp_cmcCA = new KeyPurposeID("1.3.6.1.5.5.7.3.27");

	public static readonly KeyPurposeID id_kp_cmcRA = new KeyPurposeID("1.3.6.1.5.5.7.3.28");

	public static readonly KeyPurposeID id_kp_cmKGA = new KeyPurposeID("1.3.6.1.5.5.7.3.32");

	public static readonly KeyPurposeID id_kp_smartcardlogon = new KeyPurposeID("1.3.6.1.4.1.311.20.2.2");

	public static readonly KeyPurposeID id_kp_macAddress = new KeyPurposeID("1.3.6.1.1.1.1.22");

	public static readonly KeyPurposeID id_kp_msSGC = new KeyPurposeID("1.3.6.1.4.1.311.10.3.3");

	private const string id_pkinit = "1.3.6.1.5.2.3";

	public static readonly KeyPurposeID scSysNodeNumber = new KeyPurposeID("1.3.6.1.5.2.3.0");

	public static readonly KeyPurposeID id_pkinit_authData = new KeyPurposeID("1.3.6.1.5.2.3.1");

	public static readonly KeyPurposeID id_pkinit_DHKeyData = new KeyPurposeID("1.3.6.1.5.2.3.2");

	public static readonly KeyPurposeID id_pkinit_rkeyData = new KeyPurposeID("1.3.6.1.5.2.3.3");

	public static readonly KeyPurposeID keyPurposeClientAuth = new KeyPurposeID("1.3.6.1.5.2.3.4");

	public static readonly KeyPurposeID keyPurposeKdc = new KeyPurposeID("1.3.6.1.5.2.3.5");

	public static readonly KeyPurposeID id_kp_nsSGC = new KeyPurposeID("2.16.840.1.113730.4.1");

	[Obsolete("Use 'id_kp_serverAuth' instead")]
	public static readonly KeyPurposeID IdKPServerAuth = id_kp_serverAuth;

	[Obsolete("Use 'id_kp_clientAuth' instead")]
	public static readonly KeyPurposeID IdKPClientAuth = id_kp_clientAuth;

	[Obsolete("Use 'id_kp_codeSigning' instead")]
	public static readonly KeyPurposeID IdKPCodeSigning = id_kp_codeSigning;

	[Obsolete("Use 'id_kp_emailProtection' instead")]
	public static readonly KeyPurposeID IdKPEmailProtection = id_kp_emailProtection;

	[Obsolete("Use 'id_kp_ipsecEndSystem' instead")]
	public static readonly KeyPurposeID IdKPIpsecEndSystem = id_kp_ipsecEndSystem;

	[Obsolete("Use 'id_kp_ipsecTunnel' instead")]
	public static readonly KeyPurposeID IdKPIpsecTunnel = id_kp_ipsecTunnel;

	[Obsolete("Use 'id_kp_ipsecUser' instead")]
	public static readonly KeyPurposeID IdKPIpsecUser = id_kp_ipsecUser;

	[Obsolete("Use 'id_kp_timeStamping' instead")]
	public static readonly KeyPurposeID IdKPTimeStamping = id_kp_timeStamping;

	[Obsolete("Use 'id_kp_OCSPSigning' instead")]
	public static readonly KeyPurposeID IdKPOcspSigning = id_kp_OCSPSigning;

	[Obsolete("Use 'id_kp_smartcardlogon' instead")]
	public static readonly KeyPurposeID IdKPSmartCardLogon = id_kp_smartcardlogon;

	[Obsolete("Use 'id_kp_macAddress' instead")]
	public static readonly KeyPurposeID IdKPMacAddress = id_kp_macAddress;

	private KeyPurposeID(string id)
		: base(id)
	{
	}
}
