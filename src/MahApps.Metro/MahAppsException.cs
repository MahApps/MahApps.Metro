// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;

#if !NET8_0_OR_GREATER
using System.Runtime.Serialization;
using System.Security;
#endif

namespace MahApps.Metro
{
#if !NET8_0_OR_GREATER
    [Serializable]
#endif
    public class MahAppsException : Exception
    {
        public MahAppsException()
        {
        }

        public MahAppsException(string message)
            : base(message)
        {
        }

        public MahAppsException(string message, Exception? innerException)
            : base(message, innerException)
        {
        }

#if !NET8_0_OR_GREATER
        [SecuritySafeCritical]
        protected MahAppsException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
#endif
    }

#if !NET8_0_OR_GREATER
    [Serializable]
#endif
    public class MissingRequiredTemplatePartException : MahAppsException
    {
        public MissingRequiredTemplatePartException(FrameworkElement target, string templatePart)
            : base($"Template part \"{templatePart}\" in template for \"{target.GetType().FullName}\" is missing.")
        {
        }

#if !NET8_0_OR_GREATER
        [SecuritySafeCritical]
        protected MissingRequiredTemplatePartException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
#endif
    }
}