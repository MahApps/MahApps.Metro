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
            DependencyProperty.Register(
                "ContentCharacterCasing",
                typeof(CharacterCasing),
                typeof(ContentControlEx),
                new FrameworkPropertyMetadata(CharacterCasing.Normal, FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsMeasure),
                new ValidateValueCallback(value => CharacterCasing.Normal <= (CharacterCasing)value && (CharacterCasing)value <= CharacterCasing.Upper));

        /// <summary> 
        /// Character casing of the Content
        /// </summary> 
        public CharacterCasing ContentCharacterCasing
        {
            get { return (CharacterCasing)GetValue(ContentCharacterCasingProperty); }
            set { SetValue(ContentCharacterCasingProperty, value); }
        }
    }
}