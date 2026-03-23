using System;
using System.IO;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Asn1;

public class DerExternal : Asn1Object
{
	internal class Meta : Asn1UniversalType
	{
		internal static readonly Asn1UniversalType Instance = new Meta();

		private Meta()
			: base(typeof(DerExternal), 8)
		{
		}

		internal override Asn1Object FromImplicitConstructed(Asn1Sequence sequence)
		{
			return sequence.ToAsn1External();
		}
	}

	internal readonly DerObjectIdentifier directReference;

	internal readonly DerInteger indirectReference;

	internal readonly Asn1ObjectDescriptor dataValueDescriptor;

	internal readonly int encoding;

	internal readonly Asn1Object externalContent;

	public Asn1ObjectDescriptor DataValueDescriptor => dataValueDescriptor;

	public DerObjectIdentifier DirectReference => directReference;

	public int Encoding => encoding;

	public Asn1Object ExternalContent => externalContent;

	public DerInteger IndirectReference => indirectReference;

	public static DerExternal GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is DerExternal result)
		{
			return result;
		}
		if (obj is IAsn1Convertible asn1Convertible)
		{
			if (asn1Convertible.ToAsn1Object() is DerExternal result2)
			{
				return result2;
			}
		}
		else if (obj is byte[] bytes)
		{
			try
			{
				return (DerExternal)Meta.Instance.FromByteArray(bytes);
			}
			catch (IOException ex)
			{
				throw new ArgumentException("failed to construct external from byte[]: " + ex.Message);
			}
		}
		throw new ArgumentException("illegal object in GetInstance: " + Platform.GetTypeName(obj), "obj");
	}

	public static DerExternal GetInstance(Asn1TaggedObject taggedObject, bool declaredExplicit)
	{
		return (DerExternal)Meta.Instance.GetContextInstance(taggedObject, declaredExplicit);
	}

	public DerExternal(Asn1EncodableVector vector)
		: this(new BerSequence(vector))
	{
	}

	public DerExternal(Asn1Sequence sequence)
	{
		int num = 0;
		Asn1Object objFromSequence = GetObjFromSequence(sequence, num);
		if (objFromSequence is DerObjectIdentifier)
		{
			directReference = (DerObjectIdentifier)objFromSequence;
			objFromSequence = GetObjFromSequence(sequence, ++num);
		}
		if (objFromSequence is DerInteger)
		{
			indirectReference = (DerInteger)objFromSequence;
			objFromSequence = GetObjFromSequence(sequence, ++num);
		}
		if (!(objFromSequence is Asn1TaggedObject))
		{
			dataValueDescriptor = (Asn1ObjectDescriptor)objFromSequence;
			objFromSequence = GetObjFromSequence(sequence, ++num);
		}
		if (sequence.Count != num + 1)
		{
			throw new ArgumentException("input sequence too large", "sequence");
		}
		if (!(objFromSequence is Asn1TaggedObject))
		{
			throw new ArgumentException("No tagged object found in sequence. Structure doesn't seem to be of type External", "sequence");
		}
		Asn1TaggedObject asn1TaggedObject = (Asn1TaggedObject)objFromSequence;
		encoding = CheckEncoding(asn1TaggedObject.TagNo);
		externalContent = GetExternalContent(asn1TaggedObject);
	}

	[Obsolete("Pass 'externalData' at type Asn1TaggedObject")]
	public DerExternal(DerObjectIdentifier directReference, DerInteger indirectReference, Asn1ObjectDescriptor dataValueDescriptor, DerTaggedObject externalData)
		: this(directReference, indirectReference, dataValueDescriptor, (Asn1TaggedObject)externalData)
	{
	}

	public DerExternal(DerObjectIdentifier directReference, DerInteger indirectReference, Asn1ObjectDescriptor dataValueDescriptor, Asn1TaggedObject externalData)
	{
		this.directReference = directReference;
		this.indirectReference = indirectReference;
		this.dataValueDescriptor = dataValueDescriptor;
		encoding = CheckEncoding(externalData.TagNo);
		externalContent = GetExternalContent(externalData);
	}

	public DerExternal(DerObjectIdentifier directReference, DerInteger indirectReference, Asn1ObjectDescriptor dataValueDescriptor, int encoding, Asn1Object externalData)
	{
		this.directReference = directReference;
		this.indirectReference = indirectReference;
		this.dataValueDescriptor = dataValueDescriptor;
		this.encoding = CheckEncoding(encoding);
		externalContent = CheckExternalContent(encoding, externalData);
	}

	internal virtual Asn1Sequence BuildSequence()
	{
		Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(4);
		asn1EncodableVector.AddOptional(directReference, indirectReference, dataValueDescriptor);
		asn1EncodableVector.Add(new DerTaggedObject(encoding == 0, encoding, externalContent));
		return new DerSequence(asn1EncodableVector);
	}

	internal sealed override IAsn1Encoding GetEncoding(int encoding)
	{
		return GetEncodingImplicit(encoding, 0, 8);
	}

	internal override IAsn1Encoding GetEncodingImplicit(int encoding, int tagClass, int tagNo)
	{
		return BuildSequence().GetEncodingImplicit(3, tagClass, tagNo);
	}

	internal sealed override DerEncoding GetEncodingDer()
	{
		return GetEncodingDerImplicit(0, 8);
	}

	internal sealed override DerEncoding GetEncodingDerImplicit(int tagClass, int tagNo)
	{
		return BuildSequence().GetEncodingDerImplicit(tagClass, tagNo);
	}

	protected override int Asn1GetHashCode()
	{
		return Objects.GetHashCode(directReference) ^ Objects.GetHashCode(indirectReference) ^ Objects.GetHashCode(dataValueDescriptor) ^ encoding ^ externalContent.GetHashCode();
	}

	protected override bool Asn1Equals(Asn1Object asn1Object)
	{
		if (asn1Object is DerExternal derExternal && object.Equals(directReference, derExternal.directReference) && object.Equals(indirectReference, derExternal.indirectReference) && object.Equals(dataValueDescriptor, derExternal.dataValueDescriptor) && encoding == derExternal.encoding)
		{
			return externalContent.Equals(derExternal.externalContent);
		}
		return false;
	}

	private static Asn1ObjectDescriptor CheckDataValueDescriptor(Asn1Object dataValueDescriptor)
	{
		if (dataValueDescriptor is Asn1ObjectDescriptor)
		{
			return (Asn1ObjectDescriptor)dataValueDescriptor;
		}
		if (dataValueDescriptor is DerGraphicString)
		{
			return new Asn1ObjectDescriptor((DerGraphicString)dataValueDescriptor);
		}
		throw new ArgumentException("incompatible type for data-value-descriptor", "dataValueDescriptor");
	}

	private static int CheckEncoding(int encoding)
	{
		if (encoding < 0 || encoding > 2)
		{
			throw new InvalidOperationException("invalid encoding value: " + encoding);
		}
		return encoding;
	}

	private static Asn1Object CheckExternalContent(int tagNo, Asn1Object externalContent)
	{
		return tagNo switch
		{
			1 => Asn1OctetString.Meta.Instance.CheckedCast(externalContent), 
			2 => DerBitString.Meta.Instance.CheckedCast(externalContent), 
			_ => externalContent, 
		};
	}

	private static Asn1Object GetExternalContent(Asn1TaggedObject encoding)
	{
		Asn1Utilities.CheckContextTagClass(encoding);
		return encoding.TagNo switch
		{
			0 => encoding.GetExplicitBaseObject().ToAsn1Object(), 
			1 => Asn1OctetString.GetInstance(encoding, declaredExplicit: false), 
			2 => DerBitString.GetInstance(encoding, isExplicit: false), 
			_ => throw new ArgumentException("unknown tag: " + Asn1Utilities.GetTagText(encoding), "encoding"), 
		};
	}

	private static Asn1Object GetObjFromSequence(Asn1Sequence sequence, int index)
	{
		if (sequence.Count <= index)
		{
			throw new ArgumentException("too few objects in input sequence", "sequence");
		}
		return sequence[index].ToAsn1Object();
	}
}
