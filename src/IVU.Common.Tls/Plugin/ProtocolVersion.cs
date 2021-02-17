using System;

namespace IVU.Common.Tls.Plugin
{
	public struct ProtocolVersion
	{
		public static readonly ProtocolVersion NULL   = new ProtocolVersion();
		public static readonly ProtocolVersion SSL3_0 = new ProtocolVersion(3, 0);
		public static readonly ProtocolVersion TLS1_0 = new ProtocolVersion(3, 1);
		public static readonly ProtocolVersion TLS1_1 = new ProtocolVersion(3, 2);
		public static readonly ProtocolVersion TLS1_2 = new ProtocolVersion(3, 3);
		
		public static readonly ProtocolVersion DTLS1_0 = new ProtocolVersion(254, 255);
		public static readonly ProtocolVersion DTLS1_2 = new ProtocolVersion(254, 253);
	
		public readonly byte Major;
		public readonly byte Minor;

		public ProtocolVersion(byte major, byte minor)
		{
			Major = major;
			Minor = minor;
		}
		
		public bool IsUsingDatagrams => (Major > 128);


	    // These are similar with GnuTLS gnutls_algorithm.c file
		public bool HasSelectablePRF => (this == TLS1_2 || this == DTLS1_2);

	    public bool HasSelectableSighash => (this == TLS1_2 || this == DTLS1_2);

	    public bool HasExtensions => ((this >= TLS1_0 && this <= TLS1_2) ||
		                              (this >= DTLS1_0 && this <= DTLS1_2));

	    public bool HasExplicitIV => ((this >= TLS1_1 && this <= TLS1_2) ||
	                                  (this >= DTLS1_0 && this <= DTLS1_2));

	    public bool HasVariablePadding => ((this >= TLS1_0 && this <= TLS1_2) ||
	                                       (this >= DTLS1_0 && this <= DTLS1_2));

	    public bool HasVerifiablePadding => ((this >= TLS1_0 && this <= TLS1_2) ||
	                                         (this >= DTLS1_0 && this <= DTLS1_2));

	    public bool HasAeadSupport => (this == TLS1_2 || this == DTLS1_2);

	    public ProtocolVersion PreviousProtocolVersion
		{
			get {
				if (this == TLS1_2)
					return TLS1_1;
				if (this == TLS1_1)
					return TLS1_0;
				if (this == TLS1_0)
					return SSL3_0;
				if (this == DTLS1_2)
					return DTLS1_0;
				throw new ArgumentOutOfRangeException("This is the first supported protocol version");
			}
		}



		// Define the overloaded methods and operators
		public override string ToString()
		{
			string ret;

			if (this == NULL) {
				ret = "NULL";
			} else if (this < TLS1_0) {
				ret = "SSLv" + Major + "." + Minor;
			} else if (Major == 3) {
				ret = "TLSv1." + (Minor - 1);
			} else if (!IsUsingDatagrams) {
				ret = "TLSv" + (Major - 2) + "." + Minor;
			} else {
				ret = "DTLSv" + (255 - Major) + "." + (255 - Minor);
			}

			return ret;
		}

		public override bool Equals(Object obj)
		{
			if (obj == null || GetType() != obj.GetType()) return false;
			
			ProtocolVersion version = (ProtocolVersion) obj;
			return ((Major == version.Major) && (Minor == version.Minor));
		}
		
		public override int GetHashCode()
		{
			return ((Major << 8) | Minor);
		}
		
		public static bool operator ==(ProtocolVersion a, ProtocolVersion b)
		{
			return ((a.Major == b.Major) && (a.Minor == b.Minor));
		}
		
		public static bool operator !=(ProtocolVersion a, ProtocolVersion b)
		{
			return ((a.Major != b.Major) || (a.Minor != b.Minor));
		}

		public static bool operator <(ProtocolVersion a, ProtocolVersion b)
		{
			if (a.IsUsingDatagrams != b.IsUsingDatagrams) {
				// Can't compare TLS with DTLS, always return false
				return false;
			}
			if (!a.IsUsingDatagrams) {
				if (a.Major < b.Major) return true;
				if (a.Major > b.Major) return false;
				if (a.Minor < b.Minor) return true;
			} else {
				if (a.Major > b.Major) return true;
				if (a.Major < b.Major) return false;
				if (a.Minor > b.Minor) return true;
			}
			return false;
		}
		
		public static bool operator >(ProtocolVersion a, ProtocolVersion b)
		{
			if (a.IsUsingDatagrams != b.IsUsingDatagrams) {
				// Can't compare TLS with DTLS, always return false
				return false;
			}
			if (!a.IsUsingDatagrams) {
				if (a.Major > b.Major) return true;
				if (a.Major < b.Major) return false;
				if (a.Minor > b.Minor) return true;
			} else {
				if (a.Major < b.Major) return true;
				if (a.Major > b.Major) return false;
				if (a.Minor < b.Minor) return true;
			}
			return false;
		}
		
		public static bool operator <=(ProtocolVersion a, ProtocolVersion b)
		{
			return ((a < b) || (a == b));
		}
		
		public static bool operator >=(ProtocolVersion a, ProtocolVersion b)
		{
			return ((a > b) || (a == b));
		}
	}
}
