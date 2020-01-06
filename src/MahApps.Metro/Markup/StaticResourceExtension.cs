using System.Windows;
using System.Windows.Markup;

namespace MahApps.Metro.Markup
{
    /// <summary>
    /// Implements a markup extension that supports static (XAML load time) resource references made from XAML.
    /// </summary>
    [MarkupExtensionReturnType(typeof(object))]
    [Localizability(LocalizationCategory.NeverLocalize)]
    public class StaticResourceExtension : System.Windows.StaticResourceExtension
    {
        public StaticResourceExtension()
        {
        }

        public StaticResourceExtension(object resourceKey)
            : base(resourceKey)
        {
        }
    }
}