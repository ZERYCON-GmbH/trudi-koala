//
// AaltoTLS.RecordLayer.Record
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

namespace IVU.Common.Tls.RecordLayer
{
    public enum RecordType : byte
    {
        /// <summary>
        /// 20
        /// </summary>
        ChangeCipherSpec  = 20,

        /// <summary>
        /// 21
        /// </summary>
        Alert             = 21,

        /// <summary>
        /// 22
        /// </summary>
        Handshake         = 22,

        /// <summary>
        /// 23
        /// </summary>
        Data              = 23
    }

    //public sealed class RecordType
    //{
    //    public const byte ChangeCipherSpec  = 20;
    //    public const byte Alert             = 21;
    //    public const byte Handshake         = 22;
    //    public const byte Data              = 23;
    //}
}

