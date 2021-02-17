﻿//
// AaltoTLS.HandshakeLayer.Protocol.HandshakeHelloVerifyRequest
//
// Authors:
//      Juho Vähä-Herttua  <juhovh@iki.fi>
//
// Copyright (C) 2010-2011  Aalto University
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//

using IVU.Common.Tls.Plugin;

namespace IVU.Common.Tls.HandshakeLayer.Protocol
{
    using IVU.Common.Tls.Plugin;

    public class HandshakeHelloVerifyRequest : HandshakeMessage
	{
		public ProtocolVersion  ServerVersion;
		public byte[]           Cookie;
		
		internal HandshakeHelloVerifyRequest() : this(ProtocolVersion.NULL) {}
		
		public HandshakeHelloVerifyRequest(ProtocolVersion version) : base(HandshakeMessageType.HelloVerifyRequest, version)
		{
			ServerVersion  = version;
			Cookie         = new byte[0];
		}
	}
}

