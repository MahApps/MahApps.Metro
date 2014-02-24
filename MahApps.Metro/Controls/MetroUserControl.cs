using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;

namespace MahApps.Metro.Controls
{
    [TemplatePart(Name = PART_FlyoutModal, Type = typeof(Rectangle))]

    public class MetroUserControl : UserControl
    {
        private const string PART_FlyoutModal = "PART_FlyoutModal";

        public static readonly DependencyProperty FlyoutsProperty = DependencyProperty.Register("Flyouts", typeof(UserControlFlyoutsControl), typeof(MetroUserControl), new PropertyMetadata(null));

        Rectangle flyoutModal;

        public static readonly RoutedEvent FlyoutsStatusChangedEvent = EventManager.RegisterRoutedEvent(
            "FlyoutsStatusChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(MetroUserControl));

        // Provide CLR accessors for the event 
        public event RoutedEventHandler FlyoutsStatusChanged
        {
            add { AddHandler(FlyoutsStatusChangedEvent, value); }
            remove { RemoveHandler(FlyoutsStatusChangedEvent, value); }
        }

        /// <summary>
        /// Gets/sets the FlyoutsUserControl that hosts the control's flyouts.
        /// </summary>
        public UserControlFlyoutsControl Flyouts
        {
            get { return (UserControlFlyoutsControl)GetValue(FlyoutsProperty); }
            set { SetValue(FlyoutsProperty, value); }
        }

        public MetroUserControl()
        {
            Loaded += this.MetroUserControl_Loaded;
        }

        private void MetroUserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.Flyouts == null)
            {
                this.Flyouts = new UserControlFlyoutsControl();
            }
            
            ThemeManager.IsThemeChanged += ThemeManagerOnIsThemeChanged;
            this.Unloaded += (o, args) => ThemeManager.IsThemeChanged -= ThemeManagerOnIsThemeChanged;
        }

        private void ThemeManagerOnIsThemeChanged(object sender, OnThemeChangedEventArgs e)
        {
            if (e.Accent != null)
            {
                var flyouts = this.Flyouts.GetFlyouts().ToList();

                if (!flyouts.Any())
                    return;

                foreach (UserControlFlyout flyout in flyouts)
                {
                    flyout.ChangeFlyoutTheme(e.Accent, e.Theme);
                }
            }
        }

        private void FlyoutsPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement element = (e.OriginalSource as FrameworkElement);
            if (element != null && element.TryFindParent<UserControlFlyout>() != null)
            {
                return;
            }

            if (Flyouts.OverrideExternalCloseButton == null)
            {
                foreach (UserControlFlyout flyout in Flyouts.GetFlyouts())
                {
                    if (flyout.ExternalCloseButton == e.ChangedButton && (flyout.IsPinned == false || Flyouts.OverrideIsPinned == true))
                    {
                        flyout.IsOpen = false;
                    }
                }
            }
            else if (Flyouts.OverrideExternalCloseButton == e.ChangedButton)
            {
                foreach (UserControlFlyout flyout in Flyouts.GetFlyouts())
                {
                    if (flyout.IsPinned == false || Flyouts.OverrideIsPinned == true)
                    {
                        flyout.IsOpen = false;
                    }
                }
            }
        }

        static MetroUserControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MetroUserControl), new FrameworkPropertyMetadata(typeof(MetroUserControl)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            flyoutModal = GetTemplateChild(PART_FlyoutModal) as Rectangle;
            flyoutModal.PreviewMouseDown += FlyoutsPreviewMouseDown;
            this.PreviewMouseDown += FlyoutsPreviewMouseDown;
        }

        internal void HandleFlyoutStatusChange(UserControlFlyout flyout, IEnumerable<UserControlFlyout> visibleFlyouts)
        {
            flyoutModal.Visibility = visibleFlyouts.Any(x => x.IsModal) ? Visibility.Visible : Visibility.Hidden;

            RaiseEvent(new FlyoutStatusChangedRoutedEventArgs(FlyoutsStatusChangedEvent, this)
            {
                ChangedFlyout = flyout
            });
        }

        public class FlyoutStatusChangedRoutedEventArgs : RoutedEventArgs
        {
            internal FlyoutStatusChangedRoutedEventArgs(RoutedEvent rEvent, object source)
                : base(rEvent, source)
            { }

            public UserControlFlyout ChangedFlyout { get; internal set; }
        }
    }
}
