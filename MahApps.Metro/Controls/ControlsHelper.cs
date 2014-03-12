using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using MahApps.Metro.Native;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// This custom popup is used by the validation error template.
    /// It provides some additional nice features:
    ///     - repositioning if host-window size or location changed
    ///     - repositioning if host-window gets maximized and vice versa
    ///     - it's only topmost if the host-window is activated
    /// </summary>
    public class CustomValidationPopup : Popup
    {
        private Window hostWindow;

        public CustomValidationPopup()
        {
            this.Loaded += this.CustomValidationPopup_Loaded;
            this.Opened += this.CustomValidationPopup_Opened;
        }

        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            this.IsOpen = false;
        }

        private void CustomValidationPopup_Loaded(object sender, RoutedEventArgs e)
        {
            var target = this.PlacementTarget as FrameworkElement;
            if (target == null)
            {
                return;
            }

            this.hostWindow = Window.GetWindow(target);
            if (this.hostWindow == null)
            {
                return;
            }

            this.hostWindow.LocationChanged -= this.hostWindow_SizeOrLocationChanged;
            this.hostWindow.LocationChanged += this.hostWindow_SizeOrLocationChanged;
            this.hostWindow.SizeChanged -= this.hostWindow_SizeOrLocationChanged;
            this.hostWindow.SizeChanged += this.hostWindow_SizeOrLocationChanged;
            this.hostWindow.StateChanged -= this.hostWindow_StateChanged;
            this.hostWindow.StateChanged += this.hostWindow_StateChanged;
            this.hostWindow.Activated -= this.hostWindow_Activated;
            this.hostWindow.Activated += this.hostWindow_Activated;
            this.hostWindow.Deactivated -= this.hostWindow_Deactivated;
            this.hostWindow.Deactivated += this.hostWindow_Deactivated;

            this.Unloaded -= this.CustomValidationPopup_Unloaded;
            this.Unloaded += this.CustomValidationPopup_Unloaded;
        }

        private void CustomValidationPopup_Opened(object sender, EventArgs e)
        {
            this.SetTopmostState(true);
        }

        private void hostWindow_Activated(object sender, EventArgs e)
        {
            this.SetTopmostState(true);
        }

        private void hostWindow_Deactivated(object sender, EventArgs e)
        {
            this.SetTopmostState(false);
        }

        private void CustomValidationPopup_Unloaded(object sender, RoutedEventArgs e)
        {
            if (this.hostWindow != null)
            {
                this.hostWindow.LocationChanged -= this.hostWindow_SizeOrLocationChanged;
                this.hostWindow.SizeChanged -= this.hostWindow_SizeOrLocationChanged;
                this.hostWindow.StateChanged -= this.hostWindow_StateChanged;
                this.hostWindow.Activated -= this.hostWindow_Activated;
                this.hostWindow.Deactivated -= this.hostWindow_Deactivated;
            }
            this.Unloaded -= this.CustomValidationPopup_Unloaded;
            this.Opened -= this.CustomValidationPopup_Opened;
            this.hostWindow = null;
        }

        private void hostWindow_StateChanged(object sender, EventArgs e)
        {
            if (this.hostWindow != null && this.hostWindow.WindowState != WindowState.Minimized)
            {
                var target = this.PlacementTarget as FrameworkElement;
                var holder = target != null ? target.DataContext as AdornedElementPlaceholder : null;
                if (holder != null && holder.AdornedElement != null)
                {
                    var errorTemplate = holder.AdornedElement.GetValue(Validation.ErrorTemplateProperty);
                    holder.AdornedElement.SetValue(Validation.ErrorTemplateProperty, null);
                    holder.AdornedElement.SetValue(Validation.ErrorTemplateProperty, errorTemplate);
                }
            }
        }

        private void hostWindow_SizeOrLocationChanged(object sender, EventArgs e)
        {
            var offset = this.HorizontalOffset;
            // "bump" the offset to cause the popup to reposition itself on its own
            this.HorizontalOffset = offset + 1;
            this.HorizontalOffset = offset;
        }

        private bool? appliedTopMost;
        static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);
        static readonly IntPtr HWND_TOP = new IntPtr(0);
        static readonly IntPtr HWND_BOTTOM = new IntPtr(1);

        private void SetTopmostState(bool isTop)
        {
            // Don’t apply state if it’s the same as incoming state
            if (this.appliedTopMost.HasValue && this.appliedTopMost == isTop)
            {
                return;
            }

            if (this.Child == null)
            {
                return;
            }

            var hwndSource = (PresentationSource.FromVisual(this.Child)) as HwndSource;

            if (hwndSource == null)
            {
                return;
            }
            var hwnd = hwndSource.Handle;

            RECT rect;
            if (!UnsafeNativeMethods.GetWindowRect(hwnd, out rect))
            {
                return;
            }
            //Debug.WriteLine("setting z-order " + isTop);

            if (isTop)
            {
                UnsafeNativeMethods.SetWindowPos(hwnd, HWND_TOPMOST, rect.left, rect.top, (int)this.Width, (int)this.Height, Constants.TOPMOST_FLAGS);
            }
            else
            {
                // Z-Order would only get refreshed/reflected if clicking the
                // the titlebar (as opposed to other parts of the external
                // window) unless I first set the popup to HWND_BOTTOM
                // then HWND_TOP before HWND_NOTOPMOST
                UnsafeNativeMethods.SetWindowPos(hwnd, HWND_BOTTOM, rect.left, rect.top, (int)this.Width, (int)this.Height, Constants.TOPMOST_FLAGS);
                UnsafeNativeMethods.SetWindowPos(hwnd, HWND_TOP, rect.left, rect.top, (int)this.Width, (int)this.Height, Constants.TOPMOST_FLAGS);
                UnsafeNativeMethods.SetWindowPos(hwnd, HWND_NOTOPMOST, rect.left, rect.top, (int)this.Width, (int)this.Height, Constants.TOPMOST_FLAGS);
            }

            this.appliedTopMost = isTop;
        }
    }

    /// <summary>
    /// A helper class that provides various attached properties for the GroupBox, TabItem and MetroTabItem controls.
    /// </summary>
    public static class ControlsHelper
    {
        public static readonly DependencyProperty GroupBoxHeaderForegroundProperty =
            DependencyProperty.RegisterAttached("GroupBoxHeaderForeground", typeof(Brush), typeof(ControlsHelper), new UIPropertyMetadata(Brushes.White));

        [AttachedPropertyBrowsableForType(typeof(GroupBox))]
        public static Brush GetGroupBoxHeaderForeground(UIElement element)
        {
            return (Brush)element.GetValue(GroupBoxHeaderForegroundProperty);
        }

        public static void SetGroupBoxHeaderForeground(UIElement element, Brush value)
        {
            element.SetValue(GroupBoxHeaderForegroundProperty, value);
        }

        public static readonly DependencyProperty HeaderFontSizeProperty =
            DependencyProperty.RegisterAttached("HeaderFontSize", typeof(double), typeof(ControlsHelper), new UIPropertyMetadata(26.67, HeaderFontSizePropertyChangedCallback));

        private static void HeaderFontSizePropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is double)
            {
                // close button only for MetroTabItem
                var metroTabItem = dependencyObject as MetroTabItem;
                if (metroTabItem == null)
                {
                    return;
                }

                if (metroTabItem.closeButton == null)
                {
                    metroTabItem.ApplyTemplate();
                }

                if (metroTabItem.closeButton != null && metroTabItem.contentSite != null)
                {
                    // punker76: i don't like this! i think this must be done with xaml.
                    var fontDpiSize = (double)e.NewValue;
                    var fontHeight = Math.Ceiling(fontDpiSize * metroTabItem.FontFamily.LineSpacing);
                    var newMargin = (Math.Round(fontHeight) / 2.8)
                                    - metroTabItem.Padding.Top - metroTabItem.Padding.Bottom
                                    - metroTabItem.contentSite.Margin.Top - metroTabItem.contentSite.Margin.Bottom;

                    var previousMargin = metroTabItem.closeButton.Margin;
                    metroTabItem.newButtonMargin = new Thickness(previousMargin.Left, newMargin, previousMargin.Right, previousMargin.Bottom);
                    metroTabItem.closeButton.Margin = metroTabItem.newButtonMargin;

                    metroTabItem.closeButton.UpdateLayout();
                }
            }
        }

        [AttachedPropertyBrowsableForType(typeof(MetroTabItem))]
        [AttachedPropertyBrowsableForType(typeof(TabItem))]
        [AttachedPropertyBrowsableForType(typeof(GroupBox))]
        public static double GetHeaderFontSize(UIElement element)
        {
            return (double)element.GetValue(HeaderFontSizeProperty);
        }

        public static void SetHeaderFontSize(UIElement element, double value)
        {
            element.SetValue(HeaderFontSizeProperty, value);
        }

        public static readonly DependencyProperty HeaderFontStretchProperty =
            DependencyProperty.RegisterAttached("HeaderFontStretch", typeof(FontStretch), typeof(ControlsHelper), new UIPropertyMetadata(FontStretches.Normal));

        [AttachedPropertyBrowsableForType(typeof(MetroTabItem))]
        [AttachedPropertyBrowsableForType(typeof(TabItem))]
        [AttachedPropertyBrowsableForType(typeof(GroupBox))]
        public static FontStretch GetHeaderFontStretch(UIElement element)
        {
            return (FontStretch)element.GetValue(HeaderFontStretchProperty);
        }

        public static void SetHeaderFontStretch(UIElement element, FontStretch value)
        {
            element.SetValue(HeaderFontStretchProperty, value);
        }

        public static readonly DependencyProperty HeaderFontWeightProperty =
            DependencyProperty.RegisterAttached("HeaderFontWeight", typeof(FontWeight), typeof(ControlsHelper), new UIPropertyMetadata(FontWeights.Normal));

        [AttachedPropertyBrowsableForType(typeof(MetroTabItem))]
        [AttachedPropertyBrowsableForType(typeof(TabItem))]
        [AttachedPropertyBrowsableForType(typeof(GroupBox))]
        public static FontWeight GetHeaderFontWeight(UIElement element)
        {
            return (FontWeight)element.GetValue(HeaderFontWeightProperty);
        }

        public static void SetHeaderFontWeight(UIElement element, FontWeight value)
        {
            element.SetValue(HeaderFontWeightProperty, value);
        }

        /// <summary>
        /// This property can be used to set the button width (PART_ClearText) of TextBox, PasswordBox, ComboBox
        /// For multiline TextBox, PasswordBox is this the fallback for the clear text button! so it must set manually!
        /// For normal TextBox, PasswordBox the width is the height. 
        /// </summary>
        public static readonly DependencyProperty ButtonWidthProperty =
            DependencyProperty.RegisterAttached("ButtonWidth", typeof(double), typeof(ControlsHelper),
                                                new FrameworkPropertyMetadata(22d, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.Inherits));

        public static double GetButtonWidth(DependencyObject obj)
        {
            return (double)obj.GetValue(ButtonWidthProperty);
        }

        public static void SetButtonWidth(DependencyObject obj, double value)
        {
            obj.SetValue(ButtonWidthProperty, value);
        }

        /// <summary>
        /// This property can be used to set vertical scrollbar left side from the tabpanel (look at MetroAnimatedSingleRowTabControl)
        /// </summary>
        public static readonly DependencyProperty VerticalScrollBarOnLeftSideProperty =
            DependencyProperty.RegisterAttached("VerticalScrollBarOnLeftSide", typeof(bool), typeof(ControlsHelper),
                                                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.Inherits));

        public static bool GetVerticalScrollBarOnLeftSide(DependencyObject obj)
        {
            return (bool)obj.GetValue(VerticalScrollBarOnLeftSideProperty);
        }

        public static void SetVerticalScrollBarOnLeftSide(DependencyObject obj, bool value)
        {
            obj.SetValue(VerticalScrollBarOnLeftSideProperty, value);
        }

        /// <summary>
        /// This property can be used to set the Transition for animated TabControls
        /// </summary>
        public static readonly DependencyProperty TransitionProperty =
            DependencyProperty.RegisterAttached("Transition", typeof(TransitionType), typeof(ControlsHelper),
                                                new FrameworkPropertyMetadata(TransitionType.Default, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.Inherits));

        public static TransitionType GetTransition(DependencyObject obj)
        {
            return (TransitionType)obj.GetValue(TransitionProperty);
        }

        public static void SetTransition(DependencyObject obj, TransitionType value)
        {
            obj.SetValue(TransitionProperty, value);
        }

        /// <summary>
        /// This property can be used to handle the style for CheckBox and RadioButton
        /// LeftToRight means content left and button right and RightToLeft vise versa
        /// </summary>
        public static readonly DependencyProperty ContentDirectionProperty =
            DependencyProperty.RegisterAttached("ContentDirection", typeof(FlowDirection), typeof(ControlsHelper),
                                                new FrameworkPropertyMetadata(FlowDirection.LeftToRight,
                                                                              //FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.Inherits,
                                                                              ContentDirectionPropertyChanged));

        /// <summary>
        /// This property can be used to handle the style for CheckBox and RadioButton
        /// LeftToRight means content left and button right and RightToLeft vise versa
        /// </summary>
        [AttachedPropertyBrowsableForType(typeof(ToggleButton))]
        public static FlowDirection GetContentDirection(UIElement element)
        {
            return (FlowDirection)element.GetValue(ContentDirectionProperty);
        }

        [AttachedPropertyBrowsableForType(typeof(ToggleButton))]
        public static void SetContentDirection(UIElement element, FlowDirection value)
        {
            element.SetValue(ContentDirectionProperty, value);
        }

        private static void ContentDirectionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var tb = d as ToggleButton;
            if (null == tb) {
                throw new InvalidOperationException("The property 'ContentDirection' may only be set on ToggleButton elements.");
            }
        }
    }
}
