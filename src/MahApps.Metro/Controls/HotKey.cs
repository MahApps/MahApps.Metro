// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Text;
using System.Windows.Input;
using Windows.Win32;

namespace MahApps.Metro.Controls
{
    public class HotKey : IEquatable<HotKey>
    {
        public HotKey(Key key, ModifierKeys modifierKeys = ModifierKeys.None)
        {
            this.Key = key;
            this.ModifierKeys = modifierKeys;
        }

        public Key Key { get; }

        public ModifierKeys ModifierKeys { get; }

        public override bool Equals(object? obj)
        {
            return obj is HotKey key && this.Equals(key);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((int)this.Key * 397) ^ (int)this.ModifierKeys;
            }
        }

        public bool Equals(HotKey? other)
        {
            if (other is null)
            {
                return false;
            }

            return this.Key == other.Key && this.ModifierKeys == other.ModifierKeys;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            if ((this.ModifierKeys & ModifierKeys.Alt) == ModifierKeys.Alt)
            {
                sb.Append(GetLocalizedKeyStringUnsafe(0x12 /*VK_MENU*/));
                sb.Append("+");
            }

            if ((this.ModifierKeys & ModifierKeys.Control) == ModifierKeys.Control)
            {
                sb.Append(GetLocalizedKeyStringUnsafe(0x11 /*VK_CONTROL*/));
                sb.Append("+");
            }

            if ((this.ModifierKeys & ModifierKeys.Shift) == ModifierKeys.Shift)
            {
                sb.Append(GetLocalizedKeyStringUnsafe(0x10 /*VK_SHIFT*/));
                sb.Append("+");
            }

            if ((this.ModifierKeys & ModifierKeys.Windows) == ModifierKeys.Windows)
            {
                sb.Append("Windows+");
            }

            sb.Append(GetLocalizedKeyString(this.Key));
            return sb.ToString();
        }

        private static string GetLocalizedKeyString(Key key)
        {
            if (key >= Key.BrowserBack && key <= Key.LaunchApplication2)
            {
                return key.ToString();
            }

            var vkey = KeyInterop.VirtualKeyFromKey(key);
            return GetLocalizedKeyStringUnsafe(vkey) ?? key.ToString();
        }

        private static string? GetLocalizedKeyStringUnsafe(int key)
        {
            // strip any modifier keys
            long keyCode = key & 0xffff;

            long scanCode = PInvoke.MapVirtualKey((uint)keyCode, 0x00 /*MAPVK_VK_TO_VSC*/);

            // shift the scancode to the high word
            scanCode = (scanCode << 16);
            if (keyCode == 45 ||
                keyCode == 46 ||
                keyCode == 144 ||
                (33 <= keyCode && keyCode <= 40))
            {
                // add the extended key flag
                scanCode |= 0x1000000;
            }

            unsafe
            {
                var chars = new char[256];

                fixed (char* pchars = chars)
                {
                    var resultLength = PInvoke.GetKeyNameText((int)scanCode, pchars, 256);
                    return resultLength > 0 ? new string(pchars) : null;
                }
            }
        }
    }
}