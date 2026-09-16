// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;

namespace MahApps.Metro.Tests.Views
{
    public partial class IconTemplateWindow : TestWindow
    {
        public IconTemplateWindow()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// How often the window asked for its system menu. The menu itself is left out of it,
        /// because a real one holds the message loop until somebody picks something.
        /// </summary>
        public int SystemMenuRequests { get; set; }

        protected override void ShowSystemMenuCore(Point physicalScreenLocation)
        {
            this.SystemMenuRequests++;
        }
    }
}
