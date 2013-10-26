using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace MahApps.Metro.Controls
{
    public class ControlsHelper : DependencyObject
    {
        public static readonly DependencyProperty GroupBoxHeaderForegroundProperty = DependencyProperty.Register("GroupBoxHeaderForeground", typeof(Brush), typeof(ControlsHelper), new UIPropertyMetadata(Brushes.White));

        public Brush GroupBoxHeaderForeground
        {
            get { return (Brush)GetValue(GroupBoxHeaderForegroundProperty); }
            set { SetValue(GroupBoxHeaderForegroundProperty, value); }
        }
    }
}
