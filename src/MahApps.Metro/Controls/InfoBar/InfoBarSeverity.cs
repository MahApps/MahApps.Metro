// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// How serious the message of an <see cref="InfoBar"/> is, which sets its colour and its icon.
    /// </summary>
    public enum InfoBarSeverity
    {
        /// <summary>Something the user might want to know.</summary>
        Informational = 0,

        /// <summary>Something went well.</summary>
        Success = 1,

        /// <summary>Something may need the user's attention.</summary>
        Warning = 2,

        /// <summary>Something went wrong.</summary>
        Error = 3
    }
}
