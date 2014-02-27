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
    [StyleTypedProperty(Property = "ItemContainerStyle", StyleTargetType = typeof(UserControlFlyout))]
    public class UserControlFlyoutsControl : ItemsControl
    {
        public static readonly DependencyProperty OverrideExternalCloseButtonProperty = DependencyProperty.Register("OverrideExternalCloseButton", typeof(MouseButton?), typeof(UserControlFlyoutsControl), new PropertyMetadata(null));
        public static readonly DependencyProperty OverrideIsPinnedProperty = DependencyProperty.Register("OverrideIsPinned", typeof(bool), typeof(UserControlFlyoutsControl), new PropertyMetadata(false));
        
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
        
        static UserControlFlyoutsControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(UserControlFlyoutsControl), new FrameworkPropertyMetadata(typeof(UserControlFlyoutsControl)));
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new UserControlFlyout();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is UserControlFlyout;
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
                    List<UserControlFlyout> flyouts = this.GetFlyouts(this.Items).ToList();
                    this.DetachHandlers(flyouts);
                    this.AttachHandlers(flyouts);
                    break;
            }
        }

        private void AttachHandlers(IEnumerable<UserControlFlyout> items)
        {
            foreach (var item in items)
            {
                this.AttachHandlers(item);
            }
        }

        private void AttachHandlers(UserControlFlyout item)
        {
            var isOpenChanged = DependencyPropertyDescriptor.FromProperty(UserControlFlyout.IsOpenProperty, typeof(UserControlFlyout));
            var themeChanged = DependencyPropertyDescriptor.FromProperty(UserControlFlyout.ThemeProperty, typeof(UserControlFlyout));
            isOpenChanged.AddValueChanged(item, this.FlyoutStatusChanged);
            themeChanged.AddValueChanged(item, this.FlyoutStatusChanged);
        }

        private void DetachHandlers(IEnumerable<UserControlFlyout> items)
        {
            foreach (var item in items)
            {
                this.DetachHandlers(item);
            }
        }

        private void DetachHandlers(UserControlFlyout item)
        {
            var isOpenChanged = DependencyPropertyDescriptor.FromProperty(UserControlFlyout.IsOpenProperty, typeof(UserControlFlyout));
            var themeChanged = DependencyPropertyDescriptor.FromProperty(UserControlFlyout.ThemeProperty, typeof(UserControlFlyout));
            isOpenChanged.RemoveValueChanged(item, this.FlyoutStatusChanged);
            themeChanged.RemoveValueChanged(item, this.FlyoutStatusChanged);
        }

        private void FlyoutStatusChanged(object sender, EventArgs e)
        {
            UserControlFlyout flyout = this.GetFlyout(sender); //Get the flyout that raised the handler.

            this.ReorderZIndices(flyout);

            var parentWindow = this.TryFindParent<MetroUserControl>();
            if (parentWindow != null)
            {
                var visibleFlyouts = this.GetFlyouts(this.Items).Where(i => i.IsOpen).OrderBy(Panel.GetZIndex);
                parentWindow.HandleFlyoutStatusChange(flyout, visibleFlyouts);
            }
        }

        private UserControlFlyout GetFlyout(object item)
        {
            var flyout = item as UserControlFlyout;
            if (flyout != null)
            {
                return flyout;
            }

            return (UserControlFlyout)this.ItemContainerGenerator.ContainerFromItem(item);
        }

        internal IEnumerable<UserControlFlyout> GetFlyouts()
        {
            return GetFlyouts(this.Items);
        }

        private IEnumerable<UserControlFlyout> GetFlyouts(IEnumerable items)
        {
            return from object item in items select this.GetFlyout(item);
        }

        private void ReorderZIndices(UserControlFlyout lastChanged)
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
