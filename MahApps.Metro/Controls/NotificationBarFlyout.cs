using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Animation;


namespace MahApps.Metro.Controls
{
    public enum NotificationType
    {
        NoClassification,
        Information,
        Warning,
        Error
    }

    [TemplatePart(Name = "PART_BackButton", Type = typeof(Button))]
    [TemplatePart(Name = "PART_Header", Type = typeof(ContentPresenter))]
    public class NotificationBarFlyout : Flyout
    {
        private static FrameworkElement _rootElement;
        private Window _Shell;
        private static AdornerLayer _myAdorner;

        static NotificationBarFlyout()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NotificationBarFlyout), new FrameworkPropertyMetadata(typeof(NotificationBarFlyout)));
        }

        public NotificationBarFlyout()
        {
            Loaded += NotificationBarFlyout_Loaded;
            SizeChanged += OnSizeChanged;
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.Property == IsOpenProperty)
            {
                if ((bool) e.NewValue)
                    _myAdorner.Visibility = Visibility.Visible;
                else
                    _myAdorner.Visibility = Visibility.Hidden;
            }
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs sizeChangedEventArgs)
        {
            ApplyAnimation(Position, IsOpen);
        }

        public bool IsDesignMode
        {
            get
            {
                return DesignerProperties.GetIsInDesignMode(this);
            }
        }

        private void NotificationBarFlyout_Loaded(object sender, RoutedEventArgs e)
        {
            EnsureRootElement();
            if (!IsDesignMode)
            {
                if (_Shell == null)
                {
                    _Shell = ((Window)_rootElement.GetTopLevelControl());
                    //_Shell.SizeChanged += OnSizeChanged;
                    Binding parentWidthBinding = new Binding("Width") {Source = _Shell};
                    SetBinding(WidthProperty, parentWidthBinding);
                }

                if (_myAdorner == null)
                {
                    if (_Shell is MetroWindow)
                    {
                        MetroContentControl contentControl = _Shell.FindChildren<MetroContentControl>().First();
                        _myAdorner = AdornerLayer.GetAdornerLayer(contentControl);
                        _myAdorner.Visibility = Visibility.Hidden;
                        _myAdorner.Add(new Shader(contentControl));
                    }
                }
            }
            ApplyAnimation(Position, IsOpen);
        }

        internal void ApplyAnimation(Position position, bool isOpen)
        {
            var root = (Grid)GetTemplateChild("root");
            if (root == null)
                return;

            var hideFrame = (EasingDoubleKeyFrame)GetTemplateChild("hideFrame");
            var showFrame = (EasingDoubleKeyFrame)GetTemplateChild("showFrame");

            if (hideFrame == null || showFrame == null)
                return;

            showFrame.Value = 0;
            Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));

            if (position == Position.Right)
                HorizontalAlignment = HorizontalAlignment.Right;

            if (position == Position.Right)
            {
                hideFrame.Value = DesiredSize.Width;
                if (!isOpen) root.RenderTransform = new TranslateTransform(DesiredSize.Width, 0);
            }
            else
            {
                hideFrame.Value = -DesiredSize.Width;
                if (!isOpen) root.RenderTransform = new TranslateTransform(-DesiredSize.Width, 0);
            }
        }

        private void EnsureRootElement()
        {
            if (_rootElement != null) return;

            _rootElement = this.GetParentControlOffsetFromTop(1) as FrameworkElement;
        }
    }

    /// <summary>
    /// Adorner to create background shading over an element.
    /// </summary>
    public class Shader : Adorner
    {
        /// <summary>
        /// Creates a Shader with 50% black shading.
        /// </summary>
        /// <param name="adornedElement">The element over which to apply the shading.</param>
        public Shader(UIElement adornedElement) : base(adornedElement)
        {
            Background = new SolidColorBrush(Colors.Black) {Opacity = 0.5d};
            StrokeBorder = null;
        }

        /// <summary>
        /// Creates a Shader with the specified background and border.
        /// </summary>
        /// <param name="adornedElement">The element over which to apply the shading.</param>
        /// <param name="background">The brush to use to fill the shading.  Note less than 100% opacity should have been set before the brush is passed through.</param>
        /// <param name="strokeBorder">The border to apply.</param>
        public Shader(UIElement adornedElement, SolidColorBrush background, Pen strokeBorder) : this(adornedElement)
        {
            //caller needs to have set opacity on background brush
            Background = background;
            StrokeBorder = strokeBorder;
        }

        SolidColorBrush Background
        {
            get;
            set;
        }

        Pen StrokeBorder
        {
            get;
            set;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            FrameworkElement elem = (FrameworkElement)AdornedElement;
            Rect adornedElementRect = new Rect(0, 0, elem.ActualWidth, elem.ActualHeight);
            drawingContext.DrawRectangle(Background, StrokeBorder, adornedElementRect);
        }
    }
}