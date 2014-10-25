using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace MahApps.Metro.Controls
{
   
    [ContentProperty("Content")]
    public class MetroPopover : FrameworkElement
    {
        #region Disposable event handler

        static class Disposable
        {
            public static IDisposable Subscription<TEventHandler>(Action<TEventHandler> subscribe, Action<TEventHandler> unsubscribe, TEventHandler handler)
            {
                subscribe(handler);
                return new DisposableSubscription<TEventHandler>(unsubscribe, handler);
            }
        }

        /// <summary>
        /// Unsubscribes the given event handler on disposal.
        /// </summary>
        class DisposableSubscription<TEventHandler> : IDisposable
        {
            readonly Action<TEventHandler> description;
            readonly TEventHandler handler;

            public DisposableSubscription(Action<TEventHandler> unsubscribe, TEventHandler handler)
            {
                this.description = unsubscribe;
                this.handler = handler;
            }

            public void Dispose()
            {
                description(handler);
            }
        }

        #endregion

        #region Popover adorner

        // see tech.pro/tutorial/856/wpf-tutorial-using-a-visual-collection by Michael Kuehl

        class PopoverAdorner : Adorner
        {
            readonly VisualCollection _visuals;
            readonly MetroPopover _popover;
            readonly MetroPopoverWindow _popoverWindow;

            public PopoverAdorner(UIElement adornedElement, MetroPopover popover)
                : base(adornedElement)
            {
                _popover = popover;
                _visuals = new VisualCollection(this);
                _popoverWindow = new MetroPopoverWindow(popover) {
                    Content = popover.Content
                };

                // bind key popover window properties to the popovers           
                _popoverWindow.SetBinding(MetroPopoverWindow.HorizontalAlignmentProperty, new Binding("HorizontalAlignment") { Source = popover, Mode = BindingMode.OneWay });
                //_popoverWindow.SetBinding(MetroPopoverWindow.WidthProperty, new Binding("Width") { Source = popover, Mode = BindingMode.OneWay });
                //_popoverWindow.SetBinding(MetroPopoverWindow.MinWidthProperty, new Binding("MinWidth") { Source = popover, Mode = BindingMode.OneWay });
                //_popoverWindow.SetBinding(MetroPopoverWindow.MaxWidthProperty, new Binding("MaxWidth") { Source = popover, Mode = BindingMode.OneWay });
                _visuals.Add(_popoverWindow);
            }
            
            public MetroPopover Popover
            {
                get { return _popover; }
            }

            public MetroPopoverWindow PopoverWindow
            {
                get { return _popoverWindow; }
            }

            public void UpdateContent(object content)
            {
                _popoverWindow.Content = content;
            }

            public void ShowWindow()
            {
                Attach();
                _popoverWindow.Show();
            }

            public void HideWindow()
            {
                _popoverWindow.Hide();
                Detach();
            }

            public bool IsWindowOpen()
            {
                return _popoverWindow.IsLoaded && _popoverWindow.Opacity > 0;                
            }

            public void Detach()
            {
                var adornerLayer = AdornerLayer.GetAdornerLayer(AdornedElement);
                if (adornerLayer != null) {
                    adornerLayer.Remove(this);
                }
            }
            
            public void Attach()
            {
                var adornerLayer = AdornerLayer.GetAdornerLayer(AdornedElement);
                if (adornerLayer != null && !IsAttachedTo(adornerLayer)) {
                    adornerLayer.Add(this);
                }
            }

            bool IsAttachedTo(AdornerLayer adornerLayer)
            {
                if (adornerLayer != null) {
                    var adorners = adornerLayer.GetAdorners(AdornedElement);
                    return adorners != null && adorners.Contains(this);
                } else {
                    return false;
                }
            }

            bool IsAdornedElementLoaded()
            {
                if (AdornedElement is FrameworkElement) {
                    return ((FrameworkElement)AdornedElement).IsLoaded;
                } else {
                    return true;
                }
            }

            protected override Size MeasureOverride(Size constraint)
            {                
                _popoverWindow.Measure(constraint);
                return _popoverWindow.DesiredSize;
            }

            protected override Size ArrangeOverride(Size finalSize)
            {            
                var targetSize = AdornedElement.RenderSize;

                double offsetX;
                if (Popover.HorizontalAlignment == System.Windows.HorizontalAlignment.Left) {
                    offsetX = 0;
                } else if (Popover.HorizontalAlignment == System.Windows.HorizontalAlignment.Right) {
                    offsetX = targetSize.Width - finalSize.Width;
                } else if (Popover.HorizontalAlignment == System.Windows.HorizontalAlignment.Center || Popover.HorizontalAlignment == System.Windows.HorizontalAlignment.Stretch) {
                    offsetX = (targetSize.Width - finalSize.Width) / 2.0;
                } else {
                    offsetX = 0;
                }

                var offsetY = AdornedElement.RenderSize.Height;
                if (AdornedElement is Control) {
                    offsetY -= ((Control)AdornedElement).Margin.Bottom;
                }

                _popoverWindow.Arrange(new Rect(offsetX, offsetY, finalSize.Width, finalSize.Height));
                return _popoverWindow.RenderSize;
            }

            protected override Visual GetVisualChild(int index)
            {
                return _visuals[index];
            }

            protected override int VisualChildrenCount
            {
                get { return _visuals.Count; }
            }
        
            static bool IsDescendant(DependencyObject reference, DependencyObject node)
            {
                bool result = false;
                DependencyObject dependencyObject = node;
                while (dependencyObject != null) {
                    if (dependencyObject == reference) {
                        result = true;
                        break;
                    }

                    dependencyObject = dependencyObject.GetParentObject();
                }
                return result;
            }
        }

        #endregion

        PopoverAdorner _adorner;
        IDisposable _previewOwningWindowClickSubscription;

        static MetroPopover()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MetroPopover), new FrameworkPropertyMetadata(typeof(MetroPopover)));

            // Ensure visibility is always collapsed
            UIElement.VisibilityProperty.OverrideMetadata(typeof(MetroPopover), new FrameworkPropertyMetadata(Visibility.Collapsed, null, new CoerceValueCallback(MetroPopover.CoerceCollapsedVisibility)));
        }

        private static object CoerceCollapsedVisibility(DependencyObject d, object baseValue)
        {
            return Visibility.Collapsed;
        }

        public MetroPopover()
        {
            this.Loaded += OnLoaded;
            this.Unloaded += OnUnloaded;
        }
        
        public static readonly DependencyProperty ContentProperty = DependencyProperty.Register("Content", typeof(object), typeof(MetroPopover), new PropertyMetadata(null, OnContentChanged));
        public static readonly DependencyProperty IsOpenProperty = DependencyProperty.Register("IsOpen", typeof(bool), typeof(MetroPopover), new PropertyMetadata(false, OnIsOpenChanged));
        public static readonly DependencyProperty TargetProperty = DependencyProperty.Register("Target", typeof(UIElement), typeof(MetroPopover), new PropertyMetadata(null, OnTargetChanged));
        public static readonly DependencyProperty AutoCloseProperty = DependencyProperty.Register("AutoClose", typeof(bool), typeof(MetroPopover), new PropertyMetadata(true));

        /// <summary>
        /// The element to place the popover under. Note horizontal placement is controlled via the <see cref="HorizontalAlignment"/> property.
        /// </summary>
        public UIElement Target
        {
            get { return (UIElement)GetValue(TargetProperty); }
            set { SetValue(TargetProperty, value); }
        }
        
        /// <summary>
        /// Gets or sets the content to be displayed in the popover.
        /// </summary>
        public object Content
        {
            get { return (object)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating if the popover open. 
        /// </summary>
        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }

        /// <summary>
        /// Gets or sets a <see cref="Boolean"/> value that indicates if the popover should be automatically closed when the user clicks else where in the owning window.
        /// </summary>
        public bool AutoClose
        {
            get { return (bool)GetValue(AutoCloseProperty); }
            set { SetValue(AutoCloseProperty, value); }
        }

        /// <summary>
        /// Opens the popover.
        /// </summary>
        public void Open()
        {
            IsOpen = true;
        }

        /// <summary>
        /// Closes the popover.
        /// </summary>
        public void Close()
        {
            IsOpen = false;
        }

        /// <summary>
        /// A last chance virtual method for stopping an popover from closing.
        /// </summary>
        /// <returns></returns>
        protected virtual bool OnRequestClose()
        {
            return true; //allow the dialog to close.
        }

        private static void OnTargetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var popover = (MetroPopover)d;
            popover.SetupAdorner();
        }

        private static void OnContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var popover = (MetroPopover)d;
            var oldChild = e.OldValue;
            var newChild = e.NewValue;

            // rebuild 
            if (popover._adorner != null) {
                popover._adorner.UpdateContent(newChild);
            }
        }
        
        private static void OnIsOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var popover = (MetroPopover)d;
            var isOpen = (bool)e.NewValue;

            if (popover._adorner != null) {
                if (isOpen) {
                    popover._adorner.ShowWindow();
                } else {
                    popover._adorner.HideWindow();
                }
            }
        }

        // Attach to parent window.

        void OnLoaded(object sender, RoutedEventArgs e)
        {
            Window owner = this.TryFindParent<Window>();
            if (owner != null && _previewOwningWindowClickSubscription == null) {
                _previewOwningWindowClickSubscription = Disposable.Subscription<MouseButtonEventHandler>(handler => owner.PreviewMouseDown += handler, handler => owner.PreviewMouseDown -= handler, OnPreviewOwningWindowMouseDown);
            }
        }

        void OnUnloaded(object sender, RoutedEventArgs e)
        {
            if (_previewOwningWindowClickSubscription != null) {
                _previewOwningWindowClickSubscription.Dispose();
                _previewOwningWindowClickSubscription = null;
            }
        }
        
        private void OnPreviewOwningWindowMouseDown(object sender, MouseButtonEventArgs e)
        {
            var originalSource = e.OriginalSource as Visual;
            if (AutoClose && IsPopoverOpen() &&
                originalSource != null &&    // didn't click on target or popover (adorner)
                !Target.IsAncestorOf(originalSource) &&
                !_adorner.IsAncestorOf(originalSource)) {
                Close();
            }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            // The MetroPopover control itself isn't ever rendered on screen so set the measure to 0.
            return default(Size);
        }

        /// <summary>
        /// Programically determines if the popover is actually open & visible.
        /// </summary>
        /// <returns></returns>
        bool IsPopoverOpen()
        {
            return _adorner != null && _adorner.IsWindowOpen();
        }

        private void SetupAdorner()
        {
            if (_adorner == null || _adorner.AdornedElement != Target) {
                if (_adorner != null) {
                    _adorner.Detach();
                    this.RemoveLogicalChild(_adorner.PopoverWindow);

                    _adorner = null;
                }

                if (Target != null) {
                    _adorner = new PopoverAdorner(Target, this);
                    this.AddLogicalChild(_adorner.PopoverWindow);
                }
            }
        }

    }
}
