using ICSharpCode.AvalonEdit;
using MahApps.Demo.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MahApps.Demo.Controls
{
    [TemplatePart(Name = nameof(PART_AvalonEdit), Type = typeof(TextEditor))]
    public class DemoView : HeaderedContentControl
    {
        private TextEditor PART_AvalonEdit;

        /// <summary>Identifies the <see cref="ExampleXaml"/> dependency property.</summary>
        public static readonly DependencyProperty ExampleXamlProperty = DependencyProperty.Register(nameof(ExampleXaml), typeof(string), typeof(DemoView), new PropertyMetadata(null));
        
        /// <summary>Identifies the <see cref="HyperlinkOnlineDocs"/> dependency property.</summary>
        public static readonly DependencyProperty HyperlinkOnlineDocsProperty = DependencyProperty.Register(nameof(HyperlinkOnlineDocs), typeof(string), typeof(DemoView), new PropertyMetadata(null));
        


        public DemoView()
        {
            DemoProperties.CollectionChanged += DemoProperties_CollectionChanged;
        }

        public ObservableCollection<DemoViewProperty> DemoProperties { get; } = new ObservableCollection<DemoViewProperty>();

        private void DemoProperties_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (DemoViewProperty item in e.NewItems)
                {
                    item.PropertyChanged += DemoProperties_ItemPropertyChanged;
                }
            }
            if (e.OldItems != null)
            {
                foreach (DemoViewProperty item in e.OldItems)
                {
                    item.PropertyChanged -= DemoProperties_ItemPropertyChanged;
                }
            }
        }

        private void DemoProperties_ItemPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            SetExampleXaml();
        }

        

        public string ExampleXaml
        {
            get { return (string)GetValue(ExampleXamlProperty); }
            set { SetValue(ExampleXamlProperty, value); }
        }

        private void SetExampleXaml()
        {
            if (PART_AvalonEdit != null)
            {
                PART_AvalonEdit.Text = ExampleXaml;
            }
        }

        public string HyperlinkOnlineDocs
        {
            get { return (string)GetValue(HyperlinkOnlineDocsProperty); }
            set { SetValue(HyperlinkOnlineDocsProperty, value); }
        }

        public SimpleCommand NavigateToOnlineDocs { get; } = new SimpleCommand(
            (param) => !string.IsNullOrEmpty(param?.ToString()),
            (param) => Process.Start(new ProcessStartInfo(param?.ToString())));

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            PART_AvalonEdit = GetTemplateChild(nameof(PART_AvalonEdit)) as TextEditor;
        }

    }
}
