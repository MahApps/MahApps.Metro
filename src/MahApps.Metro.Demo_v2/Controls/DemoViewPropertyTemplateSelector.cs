using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MahApps.Demo.Controls
{
    public class DemoViewPropertyTemplateSelector : DataTemplateSelector
    {
        public DataTemplate FallbackTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is DemoViewProperty demoProperty)
            {
                return demoProperty.DataTemplate ?? FallbackTemplate;
            }
            else
            {
                return FallbackTemplate;
            }
        }
    }
}
