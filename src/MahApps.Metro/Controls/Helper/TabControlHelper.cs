using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// Specifies the underline position of a TabControl.
    /// </summary>
    public enum UnderlinedType
    {
        None,
        TabItems,
        SelectedTabItem,
        TabPanel
    }

    public static class TabControlHelper
    {
        /// Sets the Style and Template property to null.
        /// 
        /// Removing a TabItem in code behind can produce such nasty output
        /// System.Windows.Data Warning: 4 : Cannot find source for binding with reference 'RelativeSource FindAncestor, AncestorType='System.Windows.Controls.TabControl', AncestorLevel='1''. BindingExpression:Path=Background; DataItem=null; target element is 'TabItem' (Name=''); target property is 'Background' (type 'Brush')
        /// or
        /// System.Windows.Data Error: 4 : Cannot find source for binding with reference 'RelativeSource FindAncestor, AncestorType='System.Windows.Controls.TabControl', AncestorLevel='1''. BindingExpression:Path=(0); DataItem=null; target element is 'TabItem' (Name=''); target property is 'UnderlineBrush' (type 'Brush')
        ///
        /// This is a timing problem in WPF of the binding mechanism itself.
        ///
        /// To avoid this, we can set the Style and Template to null.
        public static void ClearStyle(this TabItem tabItem)
        {
            if (null == tabItem)
            {
                return;
            }

            tabItem.Template = null;
            tabItem.Style = null;
        }

        /// <summary>
        /// Identifies the CloseButtonEnabled attached property.
        /// </summary>
        public static readonly DependencyProperty CloseButtonEnabledProperty =
            DependencyProperty.RegisterAttached("CloseButtonEnabled",
                                                typeof(bool),
                                                typeof(TabControlHelper),
                                                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>
        /// Gets whether a close button should be visible or not.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(TabItem))]
        public static bool GetCloseButtonEnabled(UIElement element)
        {
            return (bool)element.GetValue(CloseButtonEnabledProperty);
        }

        /// <summary>
        /// Sets whether a close button should be visible or not.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(TabItem))]
        public static void SetCloseButtonEnabled(UIElement element, bool value)
        {
            element.SetValue(CloseButtonEnabledProperty, value);
        }

        /// <summary>
        /// Identifies the CloseTabCommand attached property.
        /// </summary>
        public static readonly DependencyProperty CloseTabCommandProperty =
            DependencyProperty.RegisterAttached("CloseTabCommand",
                                                typeof(ICommand),
                                                typeof(TabControlHelper),
                                                new PropertyMetadata(null));

        /// <summary>
        /// Gets a command for the TabItem which executes if the TabItem will be closed.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(TabItem))]
        public static ICommand GetCloseTabCommand(UIElement element)
        {
            return (ICommand)element.GetValue(CloseTabCommandProperty);
        }

        /// <summary>
        /// Sets a command for the TabItem which executes if the TabItem will be closed.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(TabItem))]
        public static void SetCloseTabCommand(UIElement element, ICommand value)
        {
            element.SetValue(CloseTabCommandProperty, value);
        }

        /// <summary>
        /// Identifies the CloseTabCommandParameter attached property.
        /// </summary>
        public static readonly DependencyProperty CloseTabCommandParameterProperty =
            DependencyProperty.RegisterAttached("CloseTabCommandParameter",
                                                typeof(object),
                                                typeof(TabControlHelper),
                                                new PropertyMetadata(null));

        /// <summary>
        /// Gets a command parameter for the TabItem that will be passed to the CloseTabCommand.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(TabItem))]
        public static object GetCloseTabCommandParameter(UIElement element)
        {
            return (object)element.GetValue(CloseTabCommandParameterProperty);
        }

        /// <summary>
        /// Sets a command parameter for the TabItem that will be passed to the CloseTabCommand.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(TabItem))]
        public static void SetCloseTabCommandParameter(UIElement element, object value)
        {
            element.SetValue(CloseTabCommandParameterProperty, value);
        }

        /// <summary>
        /// Defines whether the underline below the <see cref="TabItem"/> or <see cref="TabPanel"/> is shown or not.
        /// </summary>
        public static readonly DependencyProperty UnderlinedProperty =
            DependencyProperty.RegisterAttached("Underlined",
                                                typeof(UnderlinedType),
                                                typeof(TabControlHelper),
                                                new PropertyMetadata(UnderlinedType.None));

        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(TabControl))]
        public static UnderlinedType GetUnderlined(UIElement element)
        {
            return (UnderlinedType)element.GetValue(UnderlinedProperty);
        }

        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(TabControl))]
        public static void SetUnderlined(UIElement element, UnderlinedType value)
        {
            element.SetValue(UnderlinedProperty, value);
        }

        /// <summary>
        /// Defines the underline brush below the <see cref="TabItem"/> or <see cref="TabPanel"/>.
        /// </summary>
        public static readonly DependencyProperty UnderlineBrushProperty =
            DependencyProperty.RegisterAttached("UnderlineBrush",
                                                typeof(Brush),
                                                typeof(TabControlHelper),
                                                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(TabControl))]
        [AttachedPropertyBrowsableForType(typeof(TabItem))]
        public static Brush GetUnderlineBrush(UIElement element)
        {
            return (Brush)element.GetValue(UnderlineBrushProperty);
        }

        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(TabControl))]
        [AttachedPropertyBrowsableForType(typeof(TabItem))]
        public static void SetUnderlineBrush(UIElement element, Brush value)
        {
            element.SetValue(UnderlineBrushProperty, value);
        }

        /// <summary>
        /// Defines the underline brush below the <see cref="TabItem"/> or <see cref="TabPanel"/> of an selected item.
        /// </summary>
        public static readonly DependencyProperty UnderlineSelectedBrushProperty =
            DependencyProperty.RegisterAttached("UnderlineSelectedBrush",
                                                typeof(Brush),
                                                typeof(TabControlHelper),
                                                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(TabControl))]
        [AttachedPropertyBrowsableForType(typeof(TabItem))]
        public static Brush GetUnderlineSelectedBrush(UIElement element)
        {
            return (Brush)element.GetValue(UnderlineSelectedBrushProperty);
        }

        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(TabControl))]
        [AttachedPropertyBrowsableForType(typeof(TabItem))]
        public static void SetUnderlineSelectedBrush(UIElement element, Brush value)
        {
            element.SetValue(UnderlineSelectedBrushProperty, value);
        }

        /// <summary>
        /// Defines the underline brush below the <see cref="TabItem"/> or <see cref="TabPanel"/> if the mouse is over an item.
        /// </summary>
        public static readonly DependencyProperty UnderlineMouseOverBrushProperty =
            DependencyProperty.RegisterAttached("UnderlineMouseOverBrush",
                                                typeof(Brush),
                                                typeof(TabControlHelper),
                                                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(TabControl))]
        [AttachedPropertyBrowsableForType(typeof(TabItem))]
        public static Brush GetUnderlineMouseOverBrush(UIElement element)
        {
            return (Brush)element.GetValue(UnderlineMouseOverBrushProperty);
        }

        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(TabControl))]
        [AttachedPropertyBrowsableForType(typeof(TabItem))]
        public static void SetUnderlineMouseOverBrush(UIElement element, Brush value)
        {
            element.SetValue(UnderlineMouseOverBrushProperty, value);
        }

        /// <summary>
        /// Defines the underline brush below the <see cref="TabItem"/> or <see cref="TabPanel"/> if the mouse is over a selected item.
        /// </summary>
        public static readonly DependencyProperty UnderlineMouseOverSelectedBrushProperty =
            DependencyProperty.RegisterAttached("UnderlineMouseOverSelectedBrush",
                                                typeof(Brush),
                                                typeof(TabControlHelper),
                                                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(TabControl))]
        [AttachedPropertyBrowsableForType(typeof(TabItem))]
        public static Brush GetUnderlineMouseOverSelectedBrush(UIElement element)
        {
            return (Brush)element.GetValue(UnderlineMouseOverSelectedBrushProperty);
        }

        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(TabControl))]
        [AttachedPropertyBrowsableForType(typeof(TabItem))]
        public static void SetUnderlineMouseOverSelectedBrush(UIElement element, Brush value)
        {
            element.SetValue(UnderlineMouseOverSelectedBrushProperty, value);
        }

        /// <summary>
        /// This property can be used to set the Transition for animated TabControls
        /// </summary>
        public static readonly DependencyProperty TransitionProperty =
            DependencyProperty.RegisterAttached("Transition",
                                                typeof(TransitionType),
                                                typeof(TabControlHelper),
                                                new FrameworkPropertyMetadata(TransitionType.Default, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.Inherits));

        [Category(AppName.MahApps)]
        public static TransitionType GetTransition(DependencyObject obj)
        {
            return (TransitionType)obj.GetValue(TransitionProperty);
        }

        public static void SetTransition(DependencyObject obj, TransitionType value)
        {
            obj.SetValue(TransitionProperty, value);
        }
    }
}
