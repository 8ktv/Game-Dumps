namespace Mirror.BouncyCastle.Asn1.Cmp;

public static class CmpObjectIdentifiers
{
	public static readonly DerObjectIdentifier passwordBasedMac = new DerObjectIdentifier("1.2.840.113533.7.66.13");

	public static readonly DerObjectIdentifier dhBasedMac = new DerObjectIdentifier("1.2.840.113533.7.66.30");

	public static readonly DerObjectIdentifier id_it = new DerObjectIdentifier("1.3.6.1.5.5.7.4");

	public static readonly DerObjectIdentifier it_caProtEncCert = id_it.Branch("1");

	public static readonly DerObjectIdentifier it_signKeyPairTypes = id_it.Branch("2");

	public static readonly DerObjectIdentifier it_encKeyPairTypes = id_it.Branch("3");

	public static readonly DerObjectIdentifier it_preferredSymAlg = id_it.Branch("4");

	public static readonly DerObjectIdentifier it_caKeyUpdateInfo = id_it.Branch("5");

	public static readonly DerObjectIdentifier it_currentCRL = id_it.Branch("6");

	public static readonly DerObjectIdentifier it_unsupportedOIDs = id_it.Branch("7");

	public static readonly DerObjectIdentifier it_keyPairParamReq = id_it.Branch("10");

	public static readonly DerObjectIdentifier it_keyPairParamRep = id_it.Branch("11");

	public static readonly DerObjectIdentifier it_revPassphrase = id_it.Branch("12");

	public static readonly DerObjectIdentifier it_implicitConfirm = id_it.Branch("13");

	public static readonly DerObjectIdentifier it_confirmWaitTime = id_it.Branch("14");

	public static readonly DerObjectIdentifier it_origPKIMessage = id_it.Branch("15");

	public static readonly DerObjectIdentifier it_suppLangTags = id_it.Branch("16");

	public static readonly DerObjectIdentifier id_it_caCerts = id_it.Branch("17");

	public static readonly DerObjectIdentifier id_it_rootCaKeyUpdate = id_it.Branch("18");

	public static readonly DerObjectIdentifier id_it_certReqTemplate = id_it.Branch("19");

	public static readonly DerObjectIdentifier id_it_rootCaCert = id_it.Branch("20");

	public static readonly DerObjectIdentifier id_it_certProfile = id_it.Branch("21");

	public static readonly DerObjectIdentifier id_it_crlStatusList = id_it.Branch("22");

	public static readonly DerObjectIdentifier id_it_crls = id_it.Branch("23");

	public static readonly DerObjectIdentifier id_pkip = new DerObjectIdentifier("1.3.6.1.5.5.7.5");

	public static readonly DerObjectIdentifier id_regCtrl = new DerObjectIdentifier("1.3.6.1.5.5.7.5.1");

	public static readonly DerObjectIdentifier id_regInfo = new DerObjectIdentifier("1.3.6.1.5.5.7.5.2");

	public static readonly DerObjectIdentifier regCtrl_regToken = new DerObjectIdentifier("1.3.6.1.5.5.7.5.1.1");

	public static readonly DerObjectIdentifier regCtrl_authenticator = new DerObjectIdentifier("1.3.6.1.5.5.7.5.1.2");

	public static readonly DerObjectIdentifier regCtrl_pkiPublicationInfo = new DerObjectIdentifier("1.3.6.1.5.5.7.5.1.3");

	public static readonly DerObjectIdentifier regCtrl_pkiArchiveOptions = new DerObjectIdentifier("1.3.6.1.5.5.7.5.1.4");

	public static readonly DerObjectIdentifier regCtrl_oldCertID = new DerObjectIdentifier("1.3.6.1.5.5.7.5.1.5");

	public static readonly DerObjectIdentifier regCtrl_protocolEncrKey = new DerObjectIdentifier("1.3.6.1.5.5.7.5.1.6");

	public static readonly DerObjectIdentifier regCtrl_altCertTemplate = new DerObjectIdentifier("1.3.6.1.5.5.7.5.1.7");

	public static readonly DerObjectIdentifier regInfo_utf8Pairs = new DerObjectIdentifier("1.3.6.1.5.5.7.5.2.1");

	public static readonly DerObjectIdentifier regInfo_certReq = new DerObjectIdentifier("1.3.6.1.5.5.7.5.2.2");

	public static readonly DerObjectIdentifier ct_encKeyWithID = new DerObjectIdentifier("1.2.840.113549.1.9.16.1.21");

	public static readonly DerObjectIdentifier id_regCtrl_algId = id_pkip.Branch("1.11");

	public static readonly DerObjectIdentifier id_regCtrl_rsaKeyLen = id_pkip.Branch("1.12");
}
