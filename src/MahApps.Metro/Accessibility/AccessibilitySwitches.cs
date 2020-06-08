#if !NET452
using System;
using System.Runtime.CompilerServices;
#endif

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
#if NET452
            get => false;
#else
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => AppContext.TryGetSwitch(UseLegacyAccessibilityFeatures3SwitchName, out bool flag) && flag == true;
#endif
        }

        #endregion
    }
}