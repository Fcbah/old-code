//
// SignatureTest.cs - NUnit Test Cases for Signature
//
// Author:
//	Sebastien Pouliot (spouliot@motus.com)
//
// (C) 2003 Motus Technologies Inc. (http://www.motus.com)
//

using NUnit.Framework;
using Microsoft.Web.Services.Security;
using MWSS = Microsoft.Web.Services.Security;
using Microsoft.Web.Services.Security.X509;
using System;
using System.Xml;

namespace MonoTests.MS.Web.Services.Security {

	[TestFixture]
	public class SignatureTest : Assertion {

		static private byte[] nonDSigCertificate = { 0x30,0x82,0x06,0x2F,0x30,0x82,0x05,0x17,0xA0,0x03,0x02,0x01,0x02,0x02,0x01,0x6A,0x30,0x0D,0x06,0x09,0x2A,0x86,0x48,0x86,0xF7,0x0D,0x01,0x01,0x05,0x05,0x00,0x30,0x20,0x31,0x0B,0x30,0x09,0x06,0x03,0x55,0x04,0x06,0x13,0x02,0x55,0x53,0x31,0x11,0x30,0x0F,0x06,0x03,0x55,0x04,0x0A,0x13,0x08,0x53,0x45,0x54,0x20,0x52,0x6F,0x6F,0x74,0x30,0x1E,0x17,0x0D,0x39,0x37,0x30,0x37,0x31,0x35,0x30,0x30,0x30,0x30,0x30,0x30,0x5A,0x17,0x0D,0x30,0x34,0x30,0x37,0x31,0x35,0x30,0x30,0x30,0x30,0x30,0x30,0x5A,0x30,0x20,0x31,0x0B,
			0x30,0x09,0x06,0x03,0x55,0x04,0x06,0x13,0x02,0x55,0x53,0x31,0x11,0x30,0x0F,0x06,0x03,0x55,0x04,0x0A,0x13,0x08,0x53,0x45,0x54,0x20,0x52,0x6F,0x6F,0x74,0x30,0x82,0x01,0x22,0x30,0x0D,0x06,0x09,0x2A,0x86,0x48,0x86,0xF7,0x0D,0x01,0x01,0x01,0x05,0x00,0x03,0x82,0x01,0x0F,0x00,0x30,0x82,0x01,0x0A,0x02,0x82,0x01,0x01,0x00,0xD4,0xDC,0x3E,0xBA,0xE1,0x94,0xF7,0xBE,0xCD,0xED,0x21,0x77,0xCF,0xDA,0x88,0x58,0x51,0x0E,0x8F,0xF4,0xDA,0x00,0x14,0x1E,0x0D,0xA7,0xAD,0xB6,0x96,0x5A,0xC6,0xD3,0xEC,0x25,0xED,0xD8,0x43,
			0x0B,0x6E,0x7F,0x3F,0x9E,0x1E,0x74,0xA0,0x1E,0x97,0x76,0x30,0xCA,0x6F,0x0A,0x63,0xC0,0xA3,0x31,0x40,0x25,0x80,0xB8,0xBD,0x28,0xEB,0x7D,0x75,0x0B,0x4E,0x41,0x34,0xC4,0x20,0x00,0xC2,0xCB,0xF4,0x9A,0x20,0x00,0x58,0xD9,0xF4,0x40,0x13,0x18,0x77,0x0C,0xB5,0x04,0xDE,0xB7,0xB6,0x43,0x8B,0xA4,0xCC,0x36,0x76,0x79,0xC5,0x0B,0x17,0xCB,0x7E,0x88,0xA1,0x33,0xB0,0xD3,0x34,0x27,0xBF,0x3B,0x61,0xDA,0xC5,0x20,0xEB,0xF9,0x94,0x9A,0x8B,0x79,0xB2,0xA8,0x8E,0xCB,0xC1,0xD9,0x94,0x4A,0x99,0x66,0x50,0x55,0xB2,0x83,0x28,
			0x7D,0x22,0x3D,0xEC,0xDC,0xA3,0xE8,0x39,0xDB,0x83,0x54,0xC9,0x89,0xA9,0xDF,0x59,0x52,0x9F,0x7A,0xEF,0x7C,0x11,0x62,0x52,0xEC,0xE6,0x67,0xBA,0x3D,0xEA,0xAB,0x47,0xDB,0xE4,0xF4,0x1F,0x73,0xC3,0x3D,0xEC,0x7E,0x84,0x7D,0x2F,0x29,0xFE,0x6C,0x17,0x3F,0x75,0x6D,0x56,0x6E,0xC0,0x4E,0xB5,0xBF,0x2A,0x20,0x8A,0xE4,0x57,0xAE,0xC0,0x2E,0x68,0xC9,0x09,0xCF,0x85,0x77,0x0A,0xEF,0x3A,0x37,0xCB,0x60,0x4C,0x45,0x73,0x7F,0x90,0x3E,0x86,0x1D,0xFA,0xC3,0xFC,0x50,0x8A,0xB2,0xC5,0x8A,0x34,0xF0,0xF2,0x43,0xEE,0x3C,0x56,
			0xBA,0x24,0xE9,0xE0,0xA5,0x87,0x1E,0x7C,0x30,0x33,0x77,0xFD,0x5D,0xE0,0x57,0x0D,0x6C,0x19,0x39,0x02,0x03,0x01,0x00,0x01,0xA3,0x82,0x03,0x72,0x30,0x82,0x03,0x6E,0x30,0x12,0x06,0x03,0x55,0x1D,0x13,0x01,0x01,0xFF,0x04,0x08,0x30,0x06,0x01,0x01,0xFF,0x02,0x01,0x03,0x30,0x82,0x02,0xC9,0x06,0x03,0x55,0x1D,0x20,0x01,0x01,0xFF,0x04,0x82,0x02,0xBD,0x30,0x82,0x02,0xB9,0x30,0x82,0x02,0xB5,0x06,0x04,0x67,0x2A,0x05,0x00,0x30,0x82,0x02,0xAB,0x30,0x82,0x02,0xA7,0x06,0x04,0x67,0x2A,0x07,0x06,0x30,0x82,0x02,0x9D,
			0x30,0x82,0x02,0x99,0x1A,0x82,0x02,0x95,0x54,0x68,0x69,0x73,0x20,0x53,0x45,0x54,0x20,0x52,0x6F,0x6F,0x74,0x20,0x43,0x65,0x72,0x74,0x69,0x66,0x69,0x63,0x61,0x74,0x65,0x20,0x61,0x6E,0x64,0x20,0x61,0x6E,0x79,0x20,0x63,0x65,0x72,0x74,0x69,0x66,0x69,0x63,0x61,0x74,0x65,0x20,0x61,0x75,0x74,0x68,0x65,0x6E,0x74,0x69,0x63,0x61,0x74,0x65,0x64,0x20,0x64,0x69,0x72,0x65,0x63,0x74,0x6C,0x79,0x20,0x6F,0x72,0x20,0x69,0x6E,0x64,0x69,0x72,0x65,0x63,0x74,0x6C,0x79,0x20,0x62,0x79,0x20,0x74,0x68,0x69,0x73,0x20,0x63,
			0x65,0x72,0x74,0x69,0x66,0x69,0x63,0x61,0x74,0x65,0x2C,0x20,0x6D,0x61,0x79,0x20,0x6F,0x6E,0x6C,0x79,0x20,0x62,0x65,0x20,0x75,0x73,0x65,0x64,0x20,0x74,0x6F,0x20,0x65,0x6E,0x61,0x62,0x6C,0x65,0x20,0x22,0x53,0x65,0x63,0x75,0x72,0x65,0x20,0x46,0x69,0x6E,0x61,0x6E,0x63,0x69,0x61,0x6C,0x20,0x54,0x72,0x61,0x6E,0x73,0x61,0x63,0x74,0x69,0x6F,0x6E,0x73,0x22,0x20,0x61,0x73,0x20,0x64,0x65,0x66,0x69,0x6E,0x65,0x64,0x20,0x69,0x6E,0x20,0x74,0x68,0x65,0x20,0x53,0x45,0x54,0x20,0x52,0x6F,0x6F,0x74,0x20,0x43,0x65,
			0x72,0x74,0x69,0x66,0x69,0x63,0x61,0x74,0x65,0x20,0x50,0x72,0x61,0x63,0x74,0x69,0x63,0x65,0x20,0x53,0x74,0x61,0x74,0x65,0x6D,0x65,0x6E,0x74,0x20,0x61,0x6E,0x64,0x2C,0x20,0x77,0x68,0x65,0x6E,0x20,0x61,0x70,0x70,0x72,0x6F,0x70,0x72,0x69,0x61,0x74,0x65,0x2C,0x20,0x69,0x6E,0x20,0x61,0x20,0x53,0x45,0x54,0x20,0x42,0x72,0x61,0x6E,0x64,0x20,0x43,0x65,0x72,0x74,0x69,0x66,0x69,0x63,0x61,0x74,0x65,0x20,0x50,0x72,0x61,0x63,0x74,0x69,0x63,0x65,0x20,0x53,0x74,0x61,0x74,0x65,0x6D,0x65,0x6E,0x74,0x2E,0x20,0x20,
			0x4E,0x6F,0x20,0x50,0x61,0x72,0x74,0x79,0x20,0x6D,0x61,0x79,0x20,0x72,0x65,0x6C,0x79,0x20,0x75,0x70,0x6F,0x6E,0x20,0x74,0x68,0x65,0x20,0x53,0x45,0x54,0x20,0x52,0x6F,0x6F,0x74,0x20,0x43,0x65,0x72,0x74,0x69,0x66,0x69,0x63,0x61,0x74,0x65,0x20,0x66,0x6F,0x72,0x20,0x61,0x6E,0x79,0x20,0x6F,0x74,0x68,0x65,0x72,0x20,0x70,0x75,0x72,0x70,0x6F,0x73,0x65,0x2E,0x20,0x20,0x41,0x20,0x53,0x45,0x54,0x20,0x42,0x72,0x61,0x6E,0x64,0x20,0x73,0x68,0x61,0x6C,0x6C,0x20,0x62,0x65,0x20,0x61,0x6E,0x79,0x20,0x70,0x61,0x79,
			0x6D,0x65,0x6E,0x74,0x20,0x62,0x72,0x61,0x6E,0x64,0x20,0x77,0x68,0x6F,0x73,0x65,0x20,0x53,0x45,0x54,0x20,0x63,0x65,0x72,0x74,0x69,0x66,0x69,0x63,0x61,0x74,0x65,0x20,0x69,0x73,0x20,0x73,0x69,0x67,0x6E,0x65,0x64,0x20,0x62,0x79,0x20,0x74,0x68,0x65,0x20,0x70,0x72,0x69,0x76,0x61,0x74,0x65,0x20,0x6B,0x65,0x79,0x20,0x63,0x6F,0x72,0x72,0x65,0x73,0x70,0x6F,0x6E,0x64,0x69,0x6E,0x67,0x20,0x74,0x6F,0x20,0x74,0x68,0x65,0x20,0x70,0x75,0x62,0x6C,0x69,0x63,0x20,0x6B,0x65,0x79,0x20,0x63,0x6F,0x6E,0x74,0x61,0x69,
			0x6E,0x65,0x64,0x20,0x69,0x6E,0x20,0x74,0x68,0x69,0x73,0x20,0x63,0x65,0x72,0x74,0x69,0x66,0x69,0x63,0x61,0x74,0x65,0x2E,0x20,0x20,0x41,0x6C,0x6C,0x20,0x6D,0x61,0x74,0x74,0x65,0x72,0x73,0x20,0x72,0x65,0x6C,0x61,0x74,0x69,0x6E,0x67,0x20,0x74,0x6F,0x20,0x75,0x73,0x61,0x67,0x65,0x2C,0x20,0x6C,0x69,0x61,0x62,0x69,0x6C,0x69,0x74,0x79,0x20,0x61,0x6E,0x64,0x20,0x70,0x72,0x6F,0x63,0x65,0x64,0x75,0x72,0x65,0x73,0x20,0x77,0x69,0x74,0x68,0x20,0x53,0x45,0x54,0x20,0x63,0x65,0x72,0x74,0x69,0x66,0x69,0x63,0x61,
			0x74,0x65,0x73,0x20,0x69,0x73,0x73,0x75,0x65,0x64,0x20,0x62,0x65,0x6E,0x65,0x61,0x74,0x68,0x20,0x61,0x20,0x53,0x45,0x54,0x20,0x42,0x72,0x61,0x6E,0x64,0x20,0x73,0x68,0x61,0x6C,0x6C,0x20,0x62,0x65,0x20,0x64,0x65,0x74,0x65,0x72,0x6D,0x69,0x6E,0x65,0x64,0x20,0x62,0x79,0x20,0x74,0x68,0x61,0x74,0x20,0x53,0x45,0x54,0x20,0x42,0x72,0x61,0x6E,0x64,0x2E,0x30,0x0E,0x06,0x03,0x55,0x1D,0x0F,0x01,0x01,0xFF,0x04,0x04,0x03,0x02,0x01,0x06,0x30,0x2B,0x06,0x03,0x55,0x1D,0x10,0x04,0x24,0x30,0x22,0x80,0x0F,0x31,0x39,
			0x39,0x37,0x30,0x37,0x31,0x35,0x30,0x30,0x30,0x30,0x30,0x30,0x5A,0x81,0x0F,0x31,0x39,0x39,0x38,0x30,0x37,0x31,0x35,0x30,0x30,0x30,0x30,0x30,0x30,0x5A,0x30,0x10,0x06,0x04,0x67,0x2A,0x07,0x01,0x01,0x01,0xFF,0x04,0x05,0x03,0x03,0x07,0x00,0x80,0x30,0x3C,0x06,0x04,0x67,0x2A,0x07,0x00,0x01,0x01,0xFF,0x04,0x31,0x30,0x2F,0x30,0x2D,0x02,0x01,0x00,0x30,0x09,0x06,0x05,0x2B,0x0E,0x03,0x02,0x1A,0x05,0x00,0x30,0x07,0x06,0x05,0x67,0x2A,0x03,0x00,0x00,0x04,0x14,0xC8,0x57,0x44,0x4F,0xD7,0x91,0x56,0x3E,0xC6,0xF3,
			0xE0,0xE6,0x08,0x2E,0x9A,0xAF,0x61,0x11,0x43,0x5D,0x30,0x0D,0x06,0x09,0x2A,0x86,0x48,0x86,0xF7,0x0D,0x01,0x01,0x05,0x05,0x00,0x03,0x82,0x01,0x01,0x00,0x91,0x6D,0x0D,0x97,0xB7,0x8D,0x44,0x23,0xB9,0x49,0xAD,0x23,0xA9,0x8B,0xED,0x93,0x33,0x97,0x4C,0xE1,0x6E,0xB1,0x34,0x96,0x18,0xF3,0x58,0xB3,0x9C,0xBF,0x63,0x0F,0x61,0x46,0xC7,0xD1,0x01,0x41,0x0C,0xC8,0x42,0x55,0x6B,0x54,0x71,0x06,0x3B,0xF7,0xD1,0x77,0x65,0xDF,0x16,0xE7,0x63,0x03,0x7B,0x23,0x26,0x28,0xEC,0x94,0xF8,0x9F,0x94,0x04,0x0F,0xE5,0x45,0x99,
			0x4E,0xB5,0x1B,0xBC,0xB9,0xC4,0xB0,0xE2,0x8A,0x3E,0x05,0xA6,0xE3,0x56,0x7D,0x01,0x77,0xAB,0xC2,0xA6,0x72,0x90,0x23,0xD3,0x15,0x8F,0x0F,0xEA,0x7B,0x31,0xDE,0x89,0x31,0xF0,0x1B,0x81,0x6B,0x5F,0xA8,0x13,0xC6,0x62,0x7D,0xFE,0x74,0x14,0x40,0x2A,0x14,0xC2,0xA1,0x1B,0x9C,0xB2,0xD6,0xEF,0x2A,0x6D,0xA5,0xF7,0xA6,0x38,0x8F,0xD4,0x94,0x74,0x30,0x10,0x9E,0xBA,0xA9,0xAB,0x6B,0x61,0x6B,0xFC,0xB2,0x3F,0x87,0x6B,0x19,0x82,0x83,0x70,0xE7,0xD8,0xEA,0x28,0x7B,0xB4,0x29,0x47,0xF4,0x59,0xB3,0x3E,0x4B,0x6A,0x9D,0x54,
			0x0D,0x4E,0x1C,0xD0,0x29,0xB4,0xD1,0xE1,0x19,0x79,0x41,0x73,0xF6,0x57,0x72,0xBE,0x75,0x03,0x94,0xD7,0x58,0xA8,0xC4,0x08,0x71,0xA2,0xE3,0x16,0x31,0xCD,0xC0,0xEE,0x1C,0x21,0x26,0x52,0x55,0x7B,0x00,0x54,0x6D,0xA6,0x44,0xC2,0x4F,0xEA,0x8F,0x04,0x1C,0x3A,0xA2,0xE3,0x5B,0xD7,0x9D,0xE2,0x57,0x30,0x2C,0xF5,0xAE,0x62,0x3B,0xB5,0x49,0x89,0xCB,0x01,0xD1,0x5A,0x38,0xDE,0x97,0x57,0x85,0x91,0x68,0x6B,0xFD,0xEC,0xD3,0x80,0xF0,0x82,0xBF,0x9A };

		[Test]
		public void ConstructorUsernameToken () 
		{
			// UsernameToken supports digital signature
			UsernameToken ut = new UsernameToken ("me", "mono");
			Signature s = new Signature (ut);
			AssertNotNull ("Signature/UsernameToken", s);
			Assert ("Signature/UsernameToken/SecurityToken", s.SecurityToken is UsernameToken);
		}

		[Test]
		[ExpectedException (typeof (ArgumentException))] 
		public void ConstructorX509 () 
		{
			// this certificate was choosen because it DOESN'T support Digital Signature
			X509Certificate x509 = new X509Certificate (nonDSigCertificate);
			Assert ("!X509Certificate.SupportsDigitalSignature", !x509.SupportsDigitalSignature);
			X509SecurityToken xst = new X509SecurityToken (x509);
			Signature s = new Signature (xst);
		}

		[Test]
		[ExpectedException (typeof (ArgumentNullException))] 
		public void ConstructorNull () 
		{
			Signature s = new Signature (null);
		}

		[Test]
		public void SignatureOptionsAssign () 
		{
			UsernameToken ut = new UsernameToken ("me", "mono");
			Signature s = new Signature (ut);
			s.SignatureOptions = SignatureOptions.IncludeNone;
			AssertEquals ("SignatureOptions.IncludeNone", SignatureOptions.IncludeNone, s.SignatureOptions);
			s.SignatureOptions = SignatureOptions.IncludePath;
			AssertEquals ("SignatureOptions.IncludePath", SignatureOptions.IncludePath, s.SignatureOptions);
#if WSE1
			s.SignatureOptions = SignatureOptions.IncludePathAction;
			AssertEquals ("SignatureOptions.IncludePathAction", SignatureOptions.IncludePathAction, s.SignatureOptions);
			s.SignatureOptions = SignatureOptions.IncludePathFrom;
			AssertEquals ("SignatureOptions.IncludePathFrom", SignatureOptions.IncludePathFrom, s.SignatureOptions);
			s.SignatureOptions = SignatureOptions.IncludePathId;
			AssertEquals ("SignatureOptions.IncludePathId", SignatureOptions.IncludePathId, s.SignatureOptions);
			s.SignatureOptions = SignatureOptions.IncludePathTo;
			AssertEquals ("SignatureOptions.IncludePathTo", SignatureOptions.IncludePathTo, s.SignatureOptions);
#endif
			s.SignatureOptions = SignatureOptions.IncludeSoapBody;
			AssertEquals ("SignatureOptions.IncludeSoapBody", SignatureOptions.IncludeSoapBody, s.SignatureOptions);
			s.SignatureOptions = SignatureOptions.IncludeTimestamp;
			AssertEquals ("SignatureOptions.IncludeTimestamp", SignatureOptions.IncludeTimestamp, s.SignatureOptions);
			s.SignatureOptions = SignatureOptions.IncludeTimestampCreated;
			AssertEquals ("SignatureOptions.IncludeTimestampCreated", SignatureOptions.IncludeTimestampCreated, s.SignatureOptions);
			s.SignatureOptions = SignatureOptions.IncludeTimestampExpires;
			AssertEquals ("SignatureOptions.IncludeTimestampExpires", SignatureOptions.IncludeTimestampExpires, s.SignatureOptions);
		}

		[Test]
		[ExpectedException (typeof (ArgumentNullException))] 
		public void LoadXmlSecurityNull () 
		{
			UsernameToken ut = new UsernameToken ("me", "mono");
			Signature s = new Signature (ut);
			XmlDocument doc = new XmlDocument ();
			s.LoadXml (null, doc.DocumentElement);
		}

		[Test]
		//[ExpectedException (typeof (SecurityFault))] SecurityFault is internal
		public void LoadXmlXmlElementNull () 
		{
			UsernameToken ut = new UsernameToken ("me", "mono");
			Signature s = new Signature (ut);
			MWSS.Security sec = new MWSS.Security ("actor");
			try {
				s.LoadXml (sec, null);
			}
			catch (Exception e) {
				if (!e.ToString ().StartsWith ("Microsoft.Web.Services.Security.SecurityFault"))
					Fail ("Expected SecurityFault bug got " + e.ToString ());
			}
		}
	}
}