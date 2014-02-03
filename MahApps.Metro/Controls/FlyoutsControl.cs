using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// A FlyoutsControl is for displaying flyouts in a MetroWindow.
    /// <see cref="MetroWindow"/>
    /// </summary>
    [StyleTypedProperty(Property = "ItemContainerStyle", StyleTargetType = typeof(Flyout))]
    public class FlyoutsControl : ItemsControl
    {
        public static readonly DependencyProperty OverrideExternalCloseButtonProperty = DependencyProperty.Register("OverrideExternalCloseButton", typeof(MouseButton?), typeof(FlyoutsControl), new PropertyMetadata(null));
        public static readonly DependencyProperty OverrideIsPinnedProperty = DependencyProperty.Register("OverrideIsPinned", typeof(bool), typeof(FlyoutsControl), new PropertyMetadata(false));
        
        /// <summary>
        /// Gets/sets whether <see cref="MahApps.Metro.Controls.Flyout.ExternalCloseButton"/> is ignored and all flyouts behave as if it was set to the value of this property.
        /// </summary>
        public MouseButton? OverrideExternalCloseButton
        {
            get { return (MouseButton?) GetValue(OverrideExternalCloseButtonProperty); }
            set { SetValue(OverrideExternalCloseButtonProperty, value); }
        }
        
        /// <summary>
        /// Gets/sets whether <see cref="MahApps.Metro.Controls.Flyout.IsPinned"/> is ignored and all flyouts behave as if it was set false.
        /// </summary>
        public bool OverrideIsPinned
        {
            get { return (bool) GetValue(OverrideIsPinnedProperty); }
            set { SetValue(OverrideIsPinnedProperty, value); }
        }
        
        static FlyoutsControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(FlyoutsControl), new FrameworkPropertyMetadata(typeof(FlyoutsControl)));
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new Flyout();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is Flyout;
        }

        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    this.AttachHandlers(this.GetFlyouts(e.NewItems));
                    break;
                case NotifyCollectionChangedAction.Replace:
                    this.AttachHandlers(this.GetFlyouts(e.NewItems));
                    this.DetachHandlers(this.GetFlyouts(e.OldItems));
                    break;
                case NotifyCollectionChangedAction.Remove:
                    this.DetachHandlers(this.GetFlyouts(e.OldItems));
                    break;
                case NotifyCollectionChangedAction.Reset:
                    List<Flyout> flyouts = this.GetFlyouts(this.Items).ToList();
                    this.DetachHandlers(flyouts);
                    this.AttachHandlers(flyouts);
                    break;
            }
        }

        private void AttachHandlers(IEnumerable<Flyout> items)
        {
            foreach (var item in items)
            {
                this.AttachHandlers(item);
            }
        }

        private void AttachHandlers(Flyout item)
        {
            var isOpenChanged = DependencyPropertyDescriptor.FromProperty(Flyout.IsOpenProperty, typeof(Flyout));
            var themeChanged = DependencyPropertyDescriptor.FromProperty(Flyout.ThemeProperty, typeof(Flyout));
            isOpenChanged.AddValueChanged(item, this.FlyoutStatusChanged);
            themeChanged.AddValueChanged(item, this.FlyoutStatusChanged);
        }

        private void DetachHandlers(IEnumerable<Flyout> items)
        {
            foreach (var item in items)
            {
                this.DetachHandlers(item);
            }
        }

        private void DetachHandlers(Flyout item)
        {
            var isOpenChanged = DependencyPropertyDescriptor.FromProperty(Flyout.IsOpenProperty, typeof(Flyout));
            var themeChanged = DependencyPropertyDescriptor.FromProperty(Flyout.ThemeProperty, typeof(Flyout));
            isOpenChanged.RemoveValueChanged(item, this.FlyoutStatusChanged);
            themeChanged.RemoveValueChanged(item, this.FlyoutStatusChanged);
        }

        private void FlyoutStatusChanged(object sender, EventArgs e)
        {
            Flyout flyout = this.GetFlyout(sender); //Get the flyout that raised the handler.

            this.ReorderZIndices(flyout);

            var parentWindow = this.TryFindParent<MetroWindow>();
            if (parentWindow != null)
            {
                var visibleFlyouts = this.GetFlyouts(this.Items).Where(i => i.IsOpen).OrderBy(Panel.GetZIndex);
                parentWindow.HandleFlyoutStatusChange(flyout, visibleFlyouts);
            }
        }

        private Flyout GetFlyout(object item)
        {
            var flyout = item as Flyout;
            if (flyout != null)
            {
                return flyout;
            }

            return (Flyout)this.ItemContainerGenerator.ContainerFromItem(item);
        }

        internal IEnumerable<Flyout> GetFlyouts()
        {
            return GetFlyouts(this.Items);
        }

        private IEnumerable<Flyout> GetFlyouts(IEnumerable items)
        {
            return from object item in items select this.GetFlyout(item);
        }

        private void ReorderZIndices(Flyout lastChanged)
        {
            var openFlyouts = this.GetFlyouts(this.Items).Where(i => i.IsOpen && i != lastChanged).OrderBy(Panel.GetZIndex);
            var index = 0;
            foreach (var openFlyout in openFlyouts)
            {
                Panel.SetZIndex(openFlyout, index);
                index++;
            }

            if (lastChanged.IsOpen)
            {
                Panel.SetZIndex(lastChanged, index);
            }
        }
    }
}
