// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Drawing;
using System.Windows;
using MahApps.Metro.Controls;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;

namespace MetroDemo.ExampleWindows
{
    public partial class InteropDemo : MetroWindow
    {
        public InteropDemo()
        {
            this.InitializeComponent();
            this.ContentRendered += (_, _) => this.InitializeAsync();
        }

        private async void InitializeAsync()
        {
            var webView = new WebView2();
            webView.DefaultBackgroundColor = Color.Transparent;
            webView.HorizontalAlignment = HorizontalAlignment.Stretch;
            webView.VerticalAlignment = VerticalAlignment.Stretch;
            webView.CreationProperties = new CoreWebView2CreationProperties();

            webView.NavigationStarting += this.EnsureHttps;

            this.webViewContainer.Children.Add(webView);

            await webView.EnsureCoreWebView2Async(null);

            webView.Source = new Uri("https://mahapps.com", UriKind.RelativeOrAbsolute);
        }

        private void EnsureHttps(object? sender, CoreWebView2NavigationStartingEventArgs args)
        {
            if (sender is WebView2 webView)
            {
                var uri = args.Uri;
                if (!uri.StartsWith("https://"))
                {
                    webView.CoreWebView2.ExecuteScriptAsync($"alert('{uri} is not safe, try an https link')");
                    args.Cancel = true;
                }
            }
        }
    }
}