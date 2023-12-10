// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// ReSharper disable once CheckNamespace

namespace Windows.Win32;

using System.Runtime.CompilerServices;
using Windows.Win32.Foundation;

internal static class PInvokeExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetHeight(this RECT rect)
    {
        return rect.bottom - rect.top;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetWidth(this RECT rect)
    {
        return rect.right - rect.left;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsEmpty(this RECT rect)
    {
        return rect.left >= rect.right || rect.top >= rect.bottom;
    }

    public static void Offset(this RECT rect, int offsetX, int offsetY)
    {
        rect.left += offsetX;
        rect.top += offsetY;
    }
}