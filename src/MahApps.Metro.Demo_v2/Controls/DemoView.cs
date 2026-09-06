// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using ICSharpCode.AvalonEdit;
using MahApps.Demo.Core;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace MahApps.Demo.Controls
{
    [TemplatePart(Name = nameof(PART_AvalonEdit), Type = typeof(TextEditor))]
    public class DemoView : HeaderedContentControl
    {
        private TextEditor PART_AvalonEdit;

        /// <summary>Identifies the <see cref="ExampleXaml"/> dependency property.</summary>
        public static readonly DependencyProperty ExampleXamlProperty = DependencyProperty.Register(nameof(ExampleXaml), typeof(string), typeof(DemoView), new PropertyMetadata(null, OnExampleXamlChanged));

        /// <summary>Identifies the <see cref="HyperlinkOnlineDocs"/> dependency property.</summary>
        public static readonly DependencyProperty HyperlinkOnlineDocsProperty = DependencyProperty.Register(nameof(HyperlinkOnlineDocs), typeof(string), typeof(DemoView), new PropertyMetadata(null));

        public DemoView()
        {
            this.DemoProperties.CollectionChanged += this.DemoProperties_CollectionChanged;
        }

        public ObservableCollection<DemoViewProperty> DemoProperties { get; } = new ObservableCollection<DemoViewProperty>();

        private void DemoProperties_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (DemoViewProperty item in e.NewItems)
                {
                    item.PropertyChanged += this.DemoProperties_ItemPropertyChanged;
                }
            }

            if (e.OldItems != null)
            {
                foreach (DemoViewProperty item in e.OldItems)
                {
                    item.PropertyChanged -= this.DemoProperties_ItemPropertyChanged;
                }
            }
        }

        private void DemoProperties_ItemPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            this.SetExampleXaml();
        }

        public string ExampleXaml
        {
            get => (string)this.GetValue(ExampleXamlProperty);
            set => this.SetValue(ExampleXamlProperty, value);
        }

        private void SetExampleXaml()
        {
            if (this.PART_AvalonEdit != null)
            {
                var exampleText = this.ExampleXaml;

                foreach (var item in this.DemoProperties)
                {
                    exampleText = exampleText.Replace($"[{item.PropertyName}]", item.GetExampleXamlContent());
                }

                this.PART_AvalonEdit.Text = exampleText;
            }
        }

        private static void OnExampleXamlChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DemoView demoView)
            {
                demoView.SetExampleXaml();
            }
        }

        public string HyperlinkOnlineDocs
        {
            get => (string)this.GetValue(HyperlinkOnlineDocsProperty);
            set => this.SetValue(HyperlinkOnlineDocsProperty, value);
        }

        public SimpleCommand NavigateToOnlineDocs { get; } = new SimpleCommand(
            (param) => !string.IsNullOrEmpty(param?.ToString()),
            (param) => Process.Start(new ProcessStartInfo(param?.ToString())));

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.PART_AvalonEdit = this.GetTemplateChild(nameof(this.PART_AvalonEdit)) as TextEditor;
            this.SetExampleXaml();
        }
    }
}