//------------------------------------------------------------------------------
//  File    : WindowStateToPathDataConverter.cs
//  Author  : Mohammad Rahhal
//  Created : 14/8/2014 7:25:55 PM
//------------------------------------------------------------------------------

namespace MahApps.Metro.Converters
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;
	using System.Windows;
	using System.Windows.Data;

	[ValueConversion(typeof(WindowState), typeof(string))]
	public class WindowStateToPathDataConverter : IValueConverter
	{
		private static readonly string _MaximizePathData = "F1M0,0L0,9 9,9 9,0 0,0 0,3 8,3 8,8 1,8 1,3z";
		private static readonly string _RestorePathData = "F1M0,10L0,3 3,3 3,0 10,0 10,2 4,2 4,3 7,3 7,6 6,6 6,5 1,5 1,10z M1,10L7,10 7,7 10,7 10,2 9,2 9,6 6,6 6,9 1,9z"; 

		public WindowStateToPathDataConverter()
		{
		}

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			WindowState state = (WindowState)value;
			switch (state)
			{
				case WindowState.Maximized:
					return _RestorePathData;
				case WindowState.Normal:
				case WindowState.Minimized:
				default:
					return _MaximizePathData;
			}
		}

		// Doesn't need to be implemented.
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}