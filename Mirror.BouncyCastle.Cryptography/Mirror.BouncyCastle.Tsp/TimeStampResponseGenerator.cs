using System;
using System.Collections.Generic;
using System.IO;
using Mirror.BouncyCastle.Asn1;
using Mirror.BouncyCastle.Asn1.Cmp;
using Mirror.BouncyCastle.Asn1.Cms;
using Mirror.BouncyCastle.Asn1.Tsp;
using Mirror.BouncyCastle.Asn1.X509;
using Mirror.BouncyCastle.Math;

namespace Mirror.BouncyCastle.Tsp;

public class TimeStampResponseGenerator
{
	private class FailInfo : DerBitString
	{
		internal FailInfo(int failInfoValue)
			: base(failInfoValue)
		{
		}
	}

	private PkiStatus status;

	private Asn1EncodableVector statusStrings;

	private int failInfo;

	private TimeStampTokenGenerator tokenGenerator;

	private IList<string> acceptedAlgorithms;

	private IList<string> acceptedPolicies;

	private IList<string> acceptedExtensions;

	public TimeStampResponseGenerator(TimeStampTokenGenerator tokenGenerator, IList<string> acceptedAlgorithms)
		: this(tokenGenerator, acceptedAlgorithms, null, null)
	{
	}

	public TimeStampResponseGenerator(TimeStampTokenGenerator tokenGenerator, IList<string> acceptedAlgorithms, IList<string> acceptedPolicy)
		: this(tokenGenerator, acceptedAlgorithms, acceptedPolicy, null)
	{
	}

	public TimeStampResponseGenerator(TimeStampTokenGenerator tokenGenerator, IList<string> acceptedAlgorithms, IList<string> acceptedPolicies, IList<string> acceptedExtensions)
	{
		this.tokenGenerator = tokenGenerator;
		this.acceptedAlgorithms = acceptedAlgorithms;
		this.acceptedPolicies = acceptedPolicies;
		this.acceptedExtensions = acceptedExtensions;
		statusStrings = new Asn1EncodableVector();
	}

	private void AddStatusString(string statusString)
	{
		statusStrings.Add(new DerUtf8String(statusString));
	}

	private void SetFailInfoField(int field)
	{
		failInfo |= field;
	}

	private PkiStatusInfo GetPkiStatusInfo()
	{
		Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new DerInteger((int)status));
		if (statusStrings.Count > 0)
		{
			asn1EncodableVector.Add(new PkiFreeText(new DerSequence(statusStrings)));
		}
		if (failInfo != 0)
		{
			asn1EncodableVector.Add(new FailInfo(failInfo));
		}
		return PkiStatusInfo.GetInstance(new DerSequence(asn1EncodableVector));
	}

	public TimeStampResponse Generate(TimeStampRequest request, BigInteger serialNumber, DateTime? genTime)
	{
		TimeStampResp resp;
		try
		{
			if (!genTime.HasValue)
			{
				throw new TspValidationException("The time source is not available.", 512);
			}
			request.Validate(acceptedAlgorithms, acceptedPolicies, acceptedExtensions);
			status = PkiStatus.Granted;
			AddStatusString("Operation Okay");
			PkiStatusInfo pkiStatusInfo = GetPkiStatusInfo();
			ContentInfo instance;
			try
			{
				instance = ContentInfo.GetInstance(Asn1Object.FromByteArray(tokenGenerator.Generate(request, serialNumber, genTime.Value).ToCmsSignedData().GetEncoded()));
			}
			catch (IOException innerException)
			{
				throw new TspException("Timestamp token received cannot be converted to ContentInfo", innerException);
			}
			resp = new TimeStampResp(pkiStatusInfo, instance);
		}
		catch (TspValidationException ex)
		{
			status = PkiStatus.Rejection;
			SetFailInfoField(ex.FailureCode);
			AddStatusString(ex.Message);
			resp = new TimeStampResp(GetPkiStatusInfo(), null);
		}
		try
		{
			return new TimeStampResponse(resp);
		}
		catch (IOException innerException2)
		{
			throw new TspException("created badly formatted response!", innerException2);
		}
	}

	public TimeStampResponse GenerateGrantedResponse(TimeStampRequest request, BigInteger serialNumber, DateTime? genTime, string statusString, X509Extensions additionalExtensions)
	{
		TimeStampResp resp;
		try
		{
			if (!genTime.HasValue)
			{
				throw new TspValidationException("The time source is not available.", 512);
			}
			request.Validate(acceptedAlgorithms, acceptedPolicies, acceptedExtensions);
			status = PkiStatus.Granted;
			AddStatusString(statusString);
			PkiStatusInfo pkiStatusInfo = GetPkiStatusInfo();
			ContentInfo instance;
			try
			{
				instance = ContentInfo.GetInstance(Asn1Object.FromByteArray(tokenGenerator.Generate(request, serialNumber, genTime.Value, additionalExtensions).ToCmsSignedData().GetEncoded()));
			}
			catch (IOException innerException)
			{
				throw new TspException("Timestamp token received cannot be converted to ContentInfo", innerException);
			}
			resp = new TimeStampResp(pkiStatusInfo, instance);
		}
		catch (TspValidationException ex)
		{
			status = PkiStatus.Rejection;
			SetFailInfoField(ex.FailureCode);
			AddStatusString(ex.Message);
			resp = new TimeStampResp(GetPkiStatusInfo(), null);
		}
		try
		{
			return new TimeStampResponse(resp);
		}
		catch (IOException innerException2)
		{
			throw new TspException("created badly formatted response!", innerException2);
		}
	}

	public TimeStampResponse GenerateFailResponse(PkiStatus status, int failInfoField, string statusString)
	{
		this.status = status;
		SetFailInfoField(failInfoField);
		if (statusString != null)
		{
			AddStatusString(statusString);
		}
		TimeStampResp resp = new TimeStampResp(GetPkiStatusInfo(), null);
		try
		{
			return new TimeStampResponse(resp);
		}
		catch (IOException innerException)
		{
			throw new TspException("created badly formatted response!", innerException);
		}
	}
}
