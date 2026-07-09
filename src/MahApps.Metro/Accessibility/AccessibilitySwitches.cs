// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.CompilerServices;

namespace MahApps.Metro.Accessibility
{
    public static class AccessibilitySwitches
    {
        #region UseNetFx472CompatibleAccessibilityFeatures

        internal const string UseLegacyAccessibilityFeatures3SwitchName = "Switch.UseLegacyAccessibilityFeatures.3";

        /// <summary>
        /// Switch to force accessibility to only use features compatible with .NET 472
        /// When true, all accessibility features are compatible with .NET 472
        /// When false, accessibility features added in .NET versions greater than 472 can be enabled.
        /// </summary>
        public static bool UseNetFx472CompatibleAccessibilityFeatures
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => AppContext.TryGetSwitch(UseLegacyAccessibilityFeatures3SwitchName, out bool flag) && flag == true;
        }

        #endregion
    }
}