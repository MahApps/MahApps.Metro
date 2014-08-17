//------------------------------------------------------------------------------
//  File    : UIExtensions.cs
//  Author  : Mohammad Rahhal
//  Created : 17/8/2014 8:54:19 PM
//------------------------------------------------------------------------------

//-------------------------------------------------------------------------------------------
// We can use this class as the main class for attached dps that don't have a place to go.
//-------------------------------------------------------------------------------------------

namespace MahApps.Metro.Controls
{
    using System.Windows;

    /// <summary>
    /// Class used for various UI attached dps.
    /// </summary>
    public class UIExtensions
    {
        #region Constructors

        public UIExtensions()
        {
        }

        #endregion Constructors

        //-----------------------------------------------------------------------------------------------
        // LabelText
        //-----------------------------------------------------------------------------------------------

        #region LabelText

        /// <summary>
        /// Gets the label text of a MetroCircleButtonStyle button.
        /// </summary>
        public static string GetLabelText(DependencyObject obj)
        {
            return (string)obj.GetValue(LabelTextProperty);
        }

        /// <summary>
        /// Sets the label text of a MetroCircleButtonStyle button.
        /// </summary>
        public static void SetLabelText(DependencyObject obj, string value)
        {
            obj.SetValue(LabelTextProperty, value);
        }

        public static readonly DependencyProperty LabelTextProperty =
            DependencyProperty.RegisterAttached("LabelText", typeof(string), typeof(UIExtensions), new FrameworkPropertyMetadata(""));

        #endregion LabelText

        //-----------------------------------------------------------------------------------------------
    }
}