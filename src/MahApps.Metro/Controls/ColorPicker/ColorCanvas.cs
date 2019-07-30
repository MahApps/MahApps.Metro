using MahApps.Metro.Controls.ColorPicker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MahApps.Metro.Controls
{
    public class ColorCanvas: Control
    {

        public static readonly DependencyProperty SelectedColorItemProperty = DependencyProperty.Register("SelectedColorItem", typeof(ColorPickerItem), typeof(ColorCanvas), new PropertyMetadata(new ColorPickerItem(Colors.GreenYellow)));
            
        public ColorPickerItem SelectedColorItem
        {
            get { return (ColorPickerItem)GetValue(SelectedColorItemProperty); }
            set { SetValue(SelectedColorItemProperty, value); }
        }

        public Color SelectedColor => SelectedColorItem.RGB;

    }
}
