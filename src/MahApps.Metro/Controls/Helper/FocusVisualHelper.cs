// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// A helper class that draws the ring Windows shows around the control holding the keyboard.
    /// </summary>
    /// <remarks>
    /// WPF has <see cref="FrameworkElement.FocusVisualStyle"/> for this, and it is close but not
    /// close enough. It puts its ring up when the focus arrives from the keyboard and takes it down
    /// again at the next change of focus, so clicking the control that already holds the focus
    /// leaves the ring standing. Windows takes it down: there a press of the pointer is a focus of
    /// its own kind and no ring is drawn for it, whether or not the focus moves. WPF also hands the
    /// decision to the keyboard cues of the system, so on a machine that asks for those the ring is
    /// there after every click.
    /// <para/>
    /// So the two Windows sets draw the ring themselves. A style set here is shown as an adorner
    /// while all three of these hold: the keyboard focus is on the element or inside it, the focus
    /// arrived from the keyboard, and no pointer has been pressed on it since.
    /// <para/>
    /// The focus counting from inside as well is what puts one ring around a control built of
    /// several, a <see cref="SplitButton"/> or a <see cref="DropDownButton"/>, where the part that
    /// takes the keyboard is a button in the template and the control around it is not focusable at
    /// all. Windows draws one ring around the whole of those too.
    /// <para/>
    /// An element carrying one of these has to turn WPF's own off, or both are drawn:
    /// <code>
    /// &lt;Setter Property="FocusVisualStyle" Value="{x:Null}" /&gt;
    /// &lt;Setter Property="mah:FocusVisualHelper.FocusVisualStyle" Value="{DynamicResource MahApps.Styles.FocusVisualStyle.WinUI}" /&gt;
    /// </code>
    /// </remarks>
    [StyleTypedProperty(Property = "FocusVisualStyle", StyleTargetType = typeof(Control))]
    public static class FocusVisualHelper
    {
        public static readonly DependencyProperty FocusVisualStyleProperty
            = DependencyProperty.RegisterAttached("FocusVisualStyle",
                                                  typeof(Style),
                                                  typeof(FocusVisualHelper),
                                                  new PropertyMetadata(null, OnFocusVisualStyleChanged));

        /// <summary>Helper for getting <see cref="FocusVisualStyleProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to read <see cref="FocusVisualStyleProperty"/> from.</param>
        /// <remarks>Gets the style of the ring drawn around the element while it holds the keyboard focus.</remarks>
        /// <returns>FocusVisualStyle property value.</returns>
        [AttachedPropertyBrowsableForType(typeof(FrameworkElement))]
        public static Style? GetFocusVisualStyle(DependencyObject element)
        {
            return (Style?)element.GetValue(FocusVisualStyleProperty);
        }

        /// <summary>Helper for setting <see cref="FocusVisualStyleProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to set <see cref="FocusVisualStyleProperty"/> on.</param>
        /// <param name="value">FocusVisualStyle property value.</param>
        /// <remarks>Sets the style of the ring drawn around the element while it holds the keyboard focus.</remarks>
        public static void SetFocusVisualStyle(DependencyObject element, Style? value)
        {
            element.SetValue(FocusVisualStyleProperty, value);
        }

        /// <summary>
        /// The ring an element has up at the moment, kept on the element itself so that two of them
        /// one inside the other never take each other's away.
        /// </summary>
        private static readonly DependencyProperty AdornerProperty
            = DependencyProperty.RegisterAttached("Adorner",
                                                  typeof(Adorner),
                                                  typeof(FocusVisualHelper),
                                                  new PropertyMetadata(null));

        private static void OnFocusVisualStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not FrameworkElement element)
            {
                return;
            }

            element.IsKeyboardFocusWithinChanged -= OnIsKeyboardFocusWithinChanged;
            element.PreviewMouseDown -= OnPreviewMouseDown;

            Hide(element);

            if (e.NewValue is Style)
            {
                element.IsKeyboardFocusWithinChanged += OnIsKeyboardFocusWithinChanged;
                element.PreviewMouseDown += OnPreviewMouseDown;
            }
        }

        private static void OnIsKeyboardFocusWithinChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is not FrameworkElement element)
            {
                return;
            }

            if (e.NewValue is true)
            {
                // the focus has arrived, but only the keyboard is to be marked. The device that
                // brought it here is the one last used, which is the mouse for a click and the
                // keyboard for a tab or an arrow.
                if (InputManager.Current.MostRecentInputDevice is KeyboardDevice)
                {
                    Show(element);
                }
            }
            else
            {
                Hide(element);
            }
        }

        private static void OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            // a press of the pointer takes the ring away even where the focus stays put, which is
            // what Windows does and what WPF's own focus visual does not
            if (sender is FrameworkElement element)
            {
                Hide(element);
            }
        }

        public static readonly DependencyProperty CornerRadiusProperty
            = DependencyProperty.RegisterAttached("CornerRadius",
                                                  typeof(CornerRadius?),
                                                  typeof(FocusVisualHelper),
                                                  new PropertyMetadata(null));

        /// <summary>Helper for getting <see cref="CornerRadiusProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to read <see cref="CornerRadiusProperty"/> from.</param>
        /// <remarks>Gets the corners the ring around this element follows, where they are not the element's own.</remarks>
        /// <returns>CornerRadius property value.</returns>
        [AttachedPropertyBrowsableForType(typeof(FrameworkElement))]
        public static CornerRadius? GetCornerRadius(DependencyObject element)
        {
            return (CornerRadius?)element.GetValue(CornerRadiusProperty);
        }

        /// <summary>Helper for setting <see cref="CornerRadiusProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="DependencyObject"/> to set <see cref="CornerRadiusProperty"/> on.</param>
        /// <param name="value">CornerRadius property value.</param>
        /// <remarks>
        /// Sets the corners the ring around this element follows. An element standing inside a frame
        /// is seen as that frame, so the ring has to take the frame's corners rather than its own:
        /// the header of an expander is a button filling a border rounded on the side away from the
        /// content and square on the side the content joins, and a ring drawn to the button's own
        /// uniform radius bulges past it. Left unset, the element's own corners are used.
        /// </remarks>
        public static void SetCornerRadius(DependencyObject element, CornerRadius? value)
        {
            element.SetValue(CornerRadiusProperty, value);
        }

        /// <summary>
        /// Which of the two the ring follows. The one named here wins where a style or a template has
        /// set it, and the corners of the element itself answer where it has not.
        /// </summary>
        private static DependencyProperty RadiusSourceOf(DependencyObject element)
        {
            return GetCornerRadius(element) is null ? ControlsHelper.CornerRadiusProperty : CornerRadiusProperty;
        }

        private static void Show(FrameworkElement element)
        {
            Hide(element);

            var style = GetFocusVisualStyle(element);
            if (style is null)
            {
                return;
            }

            var layer = AdornerLayer.GetAdornerLayer(element);
            if (layer is null)
            {
                return;
            }

            var adorner = new FocusVisualAdorner(element, style);
            layer.Add(adorner);
            element.SetValue(AdornerProperty, adorner);
        }

        private static void Hide(DependencyObject element)
        {
            if (element.GetValue(AdornerProperty) is not Adorner adorner)
            {
                return;
            }

            element.ClearValue(AdornerProperty);
            (adorner.Parent as AdornerLayer)?.Remove(adorner);
        }

        /// <summary>
        /// Holds the style of the ring and nothing else. It is laid out over the element it belongs
        /// to, and where the ring lies from there, outside that edge or just inside it, is the
        /// margin the style carries.
        /// </summary>
        /// <summary>
        /// What the ring is drawn on. It is a <see cref="Control"/> so that a style with nothing but
        /// a template fits it, and it carries the corners of the element it belongs to so that a
        /// ring can follow them without a path naming an attached property, which cannot be resolved
        /// from a control built in code.
        /// </summary>
        internal sealed class FocusVisualPresenter : Control
        {
            public static readonly DependencyProperty CornerRadiusProperty
                = DependencyProperty.Register(nameof(CornerRadius),
                                              typeof(CornerRadius),
                                              typeof(FocusVisualPresenter),
                                              new FrameworkPropertyMetadata(default(CornerRadius), FrameworkPropertyMetadataOptions.AffectsRender));

            public CornerRadius CornerRadius
            {
                get => (CornerRadius)this.GetValue(CornerRadiusProperty);
                set => this.SetValue(CornerRadiusProperty, value);
            }
        }

        private sealed class FocusVisualAdorner : Adorner
        {
            private readonly Control child;

            internal FocusVisualAdorner(UIElement adornedElement, Style style)
                : base(adornedElement)
            {
                this.IsHitTestVisible = false;
                this.Focusable = false;
                // the adorner layer of a window lies over everything in it, so without this the ring
                // of a control scrolled out of sight would still be drawn where the control used to be
                this.IsClipEnabled = true;
                // this control is built here rather than parsed from XAML, so it carries none of the
                // prefixes a document would. A style meant for it has to say its corners in numbers:
                // a binding whose path names an attached property by prefix cannot be resolved from
                // here and throws every time it is tried.
                //
                // A Control takes the keyboard and stands in the tab order unless told otherwise, and
                // the adorner layer it is put into is part of the tree the tab order is walked over.
                // Nothing drawn to say where the focus is may itself be somewhere the focus can go.
                this.child = new FocusVisualPresenter
                             {
                                 Style = style,
                                 Focusable = false,
                                 IsTabStop = false
                             };

                // bound rather than read once: an expander rounds its header on the side away from
                // the content and squares the side the content joins, so the corners change under a
                // ring that is already up when the header is opened or closed
                BindingOperations.SetBinding(this.child,
                                             FocusVisualPresenter.CornerRadiusProperty,
                                             new Binding
                                             {
                                                 Source = adornedElement,
                                                 Path = new PropertyPath(RadiusSourceOf(adornedElement)),
                                                 Mode = BindingMode.OneWay
                                             });

                this.AddVisualChild(this.child);
            }

            protected override int VisualChildrenCount => 1;

            protected override Visual GetVisualChild(int index) => this.child;

            protected override Size MeasureOverride(Size constraint)
            {
                var size = this.AdornedElement.RenderSize;
                this.child.Measure(size);
                return size;
            }

            protected override Size ArrangeOverride(Size finalSize)
            {
                this.child.Arrange(new Rect(finalSize));
                return finalSize;
            }
        }
    }
}
