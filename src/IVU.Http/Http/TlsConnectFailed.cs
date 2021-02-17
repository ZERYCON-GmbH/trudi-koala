// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;

namespace IVU.Http
{
    using System;

    [SuppressMessage("Microsoft.Serialization", "CA2229")]
    public class TlsConnectFailed : HttpRequestException
    {
        public TlsConnectFailed()
            : this(null, null)
        { }

        public TlsConnectFailed(string message)
            : this(message, null)
        { }

        public TlsConnectFailed(string message, Exception inner)
            : base(message, inner)
        {
            if (inner != null)
            {
                HResult = inner.HResult;
            }
        }
    }
}
