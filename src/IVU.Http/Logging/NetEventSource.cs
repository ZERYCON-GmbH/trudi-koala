// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace IVU.Http.Logging
{
    using System;
    using System.Collections;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public class NetEventSource
    {
        private static ILog logger = LogProvider.For<NetEventSource>();

        private const string MissingMember = "(?)";
        private const string NullInstance = "(null)";
        private const string StaticMethodObject = "(static)";
        private const string NoParameters = "";
        
        public static readonly NetEventSource Log = new NetEventSource();

        public static void Enter(object thisOrContextObject, object arg1 = null, object arg2 = null, object arg3 = null) { }


        public static void Info(object thisOrContextObject, object arg)
        {
            
        }

        public static void Info(object thisOrContextObject)
        {
            
        }

        public static bool IsEnabled { get; set; }

        public static void Error(
            object thisOrContextObject,
            Exception exception,
            [CallerMemberName] string memberName = null)
        {
            if (IsEnabled)
            {
                logger.Error(exception, $"{IdOf(thisOrContextObject)}, {memberName}");
            }
        }

        public static void Error(
            object thisOrContextObject,
            FormattableString formattableString,
            [CallerMemberName] string memberName = null)
        {
            if (IsEnabled)
            {
                logger.Error($"{IdOf(thisOrContextObject)}, {memberName}", formattableString.Format, formattableString.GetArguments());
            }
        }

        public static void Error(
            object thisOrContextObject,
            string formattableString,
            [CallerMemberName] string memberName = null)
        {
            if (IsEnabled)
            {
                logger.Error($"{IdOf(thisOrContextObject)}, {memberName}", formattableString);
            }
        }

        public static void Enter(object thisOrContextObject, object arg0, object arg1, object arg2, [CallerMemberName] string memberName = null) { }

        public static void Exit(object thisOrContextObject, FormattableString formattableString = null, [CallerMemberName] string memberName = null) { }

        public static void Exit(object thisOrContextObject, object arg0, [CallerMemberName] string memberName = null) { }
        public static void Exit(object thisOrContextObject, object arg0, object arg1, [CallerMemberName] string memberName = null) { }

        public static void ContentNull(object obj) { }
        public static void Associate(object first, object second, [CallerMemberName] string memberName = null) { }
        public static void UriBaseAddress(object obj, Uri baseAddress) { }

        public void HeadersInvalidValue(string name, string rawValue) { }

        public static void ClientSendCompleted(
            HttpClient httpClient,
            IVU.Http.HttpResponseMessage response,
            IVU.Http.HttpRequestMessage request)
        {
            
        }

        private static string IdOf(object value) => value != null ? value.GetType().Name + "#" + GetHashCode(value) : NullInstance;

        private static int GetHashCode(object value) => value?.GetHashCode() ?? 0;

        private static object Format(object value)
        {
            // If it's null, return a known string for null values
            if (value == null)
            {
                return NullInstance;
            }

            // Give another partial implementation a chance to provide its own string representation
            //string result = null;
            //AdditionalCustomizedToString(value, ref result);
            //if (result != null)
            //{
            //    return result;
            //}

            // Format arrays with their element type name and length
            Array arr = value as Array;
            if (arr != null)
            {
                return $"{arr.GetType().GetElementType()}[{((Array)value).Length}]";
            }

            // Format ICollections as the name and count
            ICollection c = value as ICollection;
            if (c != null)
            {
                return $"{c.GetType().Name}({c.Count})";
            }

            // Format SafeHandles as their type, hash code, and pointer value
            SafeHandle handle = value as SafeHandle;
            if (handle != null)
            {
                return $"{handle.GetType().Name}:{handle.GetHashCode()}(0x{handle.DangerousGetHandle():X})";
            }

            // Format IntPtrs as hex
            if (value is IntPtr)
            {
                return $"0x{value:X}";
            }

            // If the string representation of the instance would just be its type name,
            // use its id instead.
            string toString = value.ToString();
            if (toString == null || toString == value.GetType().FullName)
            {
                return IdOf(value);
            }

            // Otherwise, return the original object so that the caller does default formatting.
            return value;
        }
    }
}
