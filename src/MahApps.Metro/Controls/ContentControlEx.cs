using System.Windows;
using System.Windows.Controls;

namespace MahApps.Metro.Controls
{
    public class ContentControlEx : ContentControl
    {
        /// <summary>
        /// The DependencyProperty for the CharacterCasing property.
        /// Controls whether or not content is converted to upper or lower case
        /// Default Value: CharacterCasing.Normal
        /// </summary>
        public static readonly DependencyProperty ContentCharacterCasingProperty =
            DependencyProperty.Register("ContentCharacterCasing",
                                        typeof(CharacterCasing),
                                        typeof(ContentControlEx),
                                        new FrameworkPropertyMetadata(CharacterCasing.Normal, FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsMeasure),
                                        value => CharacterCasing.Normal <= (CharacterCasing)value && (CharacterCasing)value <= CharacterCasing.Upper);

        /// <summary> 
        /// Character casing of the Content
        /// </summary> 
        public CharacterCasing ContentCharacterCasing
        {
            get { return (CharacterCasing)GetValue(ContentCharacterCasingProperty); }
            set { SetValue(ContentCharacterCasingProperty, value); }
        }

        /// <summary>
        /// The DependencyProperty for the RecognizesAccessKey property. 
        /// Default Value: false 
        /// </summary> 
        public static readonly DependencyProperty RecognizesAccessKeyProperty =
            DependencyProperty.Register("RecognizesAccessKey",
                                        typeof(bool),
                                        typeof(ContentControlEx),
                                        new FrameworkPropertyMetadata(false));

        /// <summary> 
        /// Determine if the inner ContentPresenter should use AccessText in its style
        /// </summary> 
        public bool RecognizesAccessKey
        {
            get { return (bool)GetValue(RecognizesAccessKeyProperty); }
            set { SetValue(RecognizesAccessKeyProperty, value); }
        }

        static ContentControlEx()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ContentControlEx), new FrameworkPropertyMetadata(typeof(ContentControlEx)));
        }
    }
}