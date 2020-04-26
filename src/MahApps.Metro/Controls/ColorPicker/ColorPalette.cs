using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MahApps.Metro.Controls
{
    public class ColorPalette : ListBox
    {
        static ColorPalette()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ColorPalette), new FrameworkPropertyMetadata(typeof(ColorPalette)));
        }

        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(nameof(Header), typeof(object), typeof(ColorPalette), new PropertyMetadata(null));
        public static readonly DependencyProperty ColorDictionaryProperty = DependencyProperty.Register("ColorDictionary", typeof(Dictionary<Color, string>), typeof(ColorPalette), new PropertyMetadata(null));

        /// <summary>
        /// Gets or Sets the Header of this Control
        /// </summary>
        public object Header
        {
            get { return (object)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }


        /// <summary>
        /// Gets or sets a custom dictionary for color to name. If null, the degault dictionary will be used.
        /// </summary>
        public Dictionary<Color, string> ColorDictionary
        {
            get { return (Dictionary<Color, string>)GetValue(ColorDictionaryProperty); }
            set { SetValue(ColorDictionaryProperty, value); }
        }


        #region Build in Palettes
        public static ObservableCollection<Color> PrimaryColorPalette { get; } = new ObservableCollection<Color>(
            new Color[] 
            { 
                Colors.Transparent, 
                Colors.White, 
                Colors.LightGray,
                Colors.Gray, 
                Colors.Black, 
                Colors.DarkRed,
                Colors.Red,
                Colors.Orange,
                Colors.Brown,
                Colors.Yellow, 
                Colors.LimeGreen, 
                Colors.Green, 
                Colors.DarkTurquoise, 
                Colors.Aqua, 
                Colors.Navy,
                Colors.Blue, 
                Colors.Indigo, 
                Colors.Purple,
                Colors.Fuchsia
            });

        public static ObservableCollection<Color> WpfColorPalette { get; } = new ObservableCollection<Color>(
            typeof(Colors).GetProperties().Where(x => x.PropertyType == typeof(Color))
                .Select(x => (Color)x.GetValue(null))
                .OrderBy(c => new HSVColor(c).Hue)
                .ThenBy(c => new HSVColor(c).Saturation)
                .ThenByDescending(c => new HSVColor(c).Value));

        #endregion

    }
}
