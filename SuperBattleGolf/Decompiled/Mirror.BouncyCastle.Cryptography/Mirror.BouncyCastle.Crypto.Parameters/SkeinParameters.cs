using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Mirror.BouncyCastle.Utilities.Collections;

namespace Mirror.BouncyCastle.Crypto.Parameters;

public class SkeinParameters : ICipherParameters
{
	public class Builder
	{
		private Dictionary<int, byte[]> m_parameters;

		public Builder()
		{
			m_parameters = new Dictionary<int, byte[]>();
		}

		public Builder(IDictionary<int, byte[]> paramsMap)
		{
			m_parameters = new Dictionary<int, byte[]>(paramsMap);
		}

		public Builder(SkeinParameters parameters)
			: this(parameters.m_parameters)
		{
		}

		public Builder Set(int type, byte[] value)
		{
			if (value == null)
			{
				throw new ArgumentException("Parameter value must not be null.");
			}
			switch (type)
			{
			default:
				throw new ArgumentException("Parameter types must be in the range 0,5..47,49..62.");
			case 0:
			case 5:
			case 6:
			case 7:
			case 8:
			case 9:
			case 10:
			case 11:
			case 12:
			case 13:
			case 14:
			case 15:
			case 16:
			case 17:
			case 18:
			case 19:
			case 20:
			case 21:
			case 22:
			case 23:
			case 24:
			case 25:
			case 26:
			case 27:
			case 28:
			case 29:
			case 30:
			case 31:
			case 32:
			case 33:
			case 34:
			case 35:
			case 36:
			case 37:
			case 38:
			case 39:
			case 40:
			case 41:
			case 42:
			case 43:
			case 44:
			case 45:
			case 46:
			case 47:
			case 49:
			case 50:
			case 51:
			case 52:
			case 53:
			case 54:
			case 55:
			case 56:
			case 57:
			case 58:
			case 59:
			case 60:
			case 61:
			case 62:
				if (type == 4)
				{
					throw new ArgumentException("Parameter type " + 4 + " is reserved for internal use.");
				}
				m_parameters.Add(type, value);
				return this;
			}
		}

		public Builder SetKey(byte[] key)
		{
			return Set(0, key);
		}

		public Builder SetPersonalisation(byte[] personalisation)
		{
			return Set(8, personalisation);
		}

		public Builder SetPersonalisation(DateTime date, string emailAddress, string distinguisher)
		{
			try
			{
				MemoryStream memoryStream = new MemoryStream();
				using (StreamWriter streamWriter = new StreamWriter(memoryStream, Encoding.UTF8))
				{
					streamWriter.Write(date.ToString("YYYYMMDD", CultureInfo.InvariantCulture));
					streamWriter.Write(" ");
					streamWriter.Write(emailAddress);
					streamWriter.Write(" ");
					streamWriter.Write(distinguisher);
				}
				return Set(8, memoryStream.ToArray());
			}
			catch (IOException innerException)
			{
				throw new InvalidOperationException("Byte I/O failed.", innerException);
			}
		}

		public Builder SetPublicKey(byte[] publicKey)
		{
			return Set(12, publicKey);
		}

		public Builder SetKeyIdentifier(byte[] keyIdentifier)
		{
			return Set(16, keyIdentifier);
		}

		public Builder SetNonce(byte[] nonce)
		{
			return Set(20, nonce);
		}

		public SkeinParameters Build()
		{
			return new SkeinParameters(m_parameters);
		}
	}

	public const int PARAM_TYPE_KEY = 0;

	public const int PARAM_TYPE_CONFIG = 4;

	public const int PARAM_TYPE_PERSONALISATION = 8;

	public const int PARAM_TYPE_PUBLIC_KEY = 12;

	public const int PARAM_TYPE_KEY_IDENTIFIER = 16;

	public const int PARAM_TYPE_NONCE = 20;

	public const int PARAM_TYPE_MESSAGE = 48;

	public const int PARAM_TYPE_OUTPUT = 63;

	private IDictionary<int, byte[]> m_parameters;

	public SkeinParameters()
		: this(new Dictionary<int, byte[]>())
	{
	}

	private SkeinParameters(IDictionary<int, byte[]> parameters)
	{
		m_parameters = parameters;
	}

	public IDictionary<int, byte[]> GetParameters()
	{
		return m_parameters;
	}

	public byte[] GetKey()
	{
		return CollectionUtilities.GetValueOrNull(m_parameters, 0);
	}

	public byte[] GetPersonalisation()
	{
		return CollectionUtilities.GetValueOrNull(m_parameters, 8);
	}

	public byte[] GetPublicKey()
	{
		return CollectionUtilities.GetValueOrNull(m_parameters, 12);
	}

	public byte[] GetKeyIdentifier()
	{
		return CollectionUtilities.GetValueOrNull(m_parameters, 16);
	}

	public byte[] GetNonce()
	{
		return CollectionUtilities.GetValueOrNull(m_parameters, 20);
	}
}
