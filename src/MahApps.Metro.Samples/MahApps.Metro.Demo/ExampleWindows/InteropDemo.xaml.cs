// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows;
using MahApps.Metro;
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
            this.ContentRendered += async (_, _) => await this.InitializeAsync();
        }

        private async Task InitializeAsync()
        {
            try
            {
                var webView = new WebView2();
                webView.DefaultBackgroundColor = Color.Transparent;
                webView.HorizontalAlignment = HorizontalAlignment.Stretch;
                webView.VerticalAlignment = VerticalAlignment.Stretch;
                webView.CreationProperties = new CoreWebView2CreationProperties();

                webView.NavigationStarting += EnsureHttps;

                this.webViewContainer.Children.Add(webView);

                await webView.EnsureCoreWebView2Async(null);

                webView.Source = new Uri("https://mahapps.github.io/mahapps.com", UriKind.RelativeOrAbsolute);
            }
            catch (Exception e)
            {
                throw new MahAppsException($"Error while initializing WebView2: {e.Message}", e);
            }
        }

        private static void EnsureHttps(object? sender, CoreWebView2NavigationStartingEventArgs args)
        {
            if (sender is WebView2 webView)
            {
                if (!args.Uri.StartsWith("https://"))
                {
                    webView.CoreWebView2.ExecuteScriptAsync($"alert('{args.Uri} is not safe, try an https link')");
                    args.Cancel = true;
                }
            }
        }
    }
}