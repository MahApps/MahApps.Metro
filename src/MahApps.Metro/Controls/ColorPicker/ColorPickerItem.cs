using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MahApps.Metro.Controls.ColorPicker
{
    public class ColorPickerItem : INotifyPropertyChanged
    {

        #region PropertyChanged EventHandler
        public event PropertyChangedEventHandler PropertyChanged;

        void RaisePropertyChanged(string PropertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }
        #endregion

        #region RGB Color
        private Color _RGB;
        public Color RGB
        {
            get { return _RGB; }
            set
            {
                _RGB = value;
                CalculateHsvValues();
                RaisePropertyChanged(null);
            }
        }

        public byte R
        {
            get { return _RGB.R; }
        }

        public byte G
        {
            get { return _RGB.G; }
        }

        public byte B
        {
            get { return _RGB.B; }
        }
        #endregion

        #region  HSV

        private double _Hue;
        public double Hue
        {
            get { return _Hue; }
            set { _Hue = value; RaisePropertyChanged("Hue"); }
        }

        private double _Saturation;
        public double Saturation
        {
            get { return _Saturation; }
            set { _Saturation = value; RaisePropertyChanged("Saturation"); }
        }

        private double _Value;
        public double Value
        {
            get { return _Value; }
            set { _Value = value; RaisePropertyChanged("Value"); }
        }

        #endregion

        #region Convert
        void CalculateHsvValues()
        {
            var max = Math.Max(R, Math.Max(G, B));
            var min = Math.Min(R, Math.Min(G, B));

            // H

            if (max == R)
            {
                _Hue = 60 * (0 + (G - B)* 1.0 /(max-min));
            }
            else if (max == G)
            {
                _Hue = 60 * (2 + (B - R) * 1.0 / (max - min));
            }
            else if (max == B)
            {
                _Hue = 60 * (4 + (R - G) * 1.0 / (max - min));
            }

            if (_Hue < 0)
            {
                _Hue = _Hue + 360; 
            }

            // S 
            if (max == 0)
            {
                _Saturation = 0;
            }
            else
            {
                _Saturation = (max - min) * 1.0 / max;
            }

            // V
            _Value = max / 255.0 ;
        }


        #endregion

        #region Constructors

        public ColorPickerItem(Color color)
        {
            _RGB = color;
            CalculateHsvValues();
            RaisePropertyChanged(null);
        }

        #endregion
    }
}
