﻿using System;
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
        public static readonly DependencyProperty ExternalCloseButtonProperty =
            DependencyProperty.Register("ExternalCloseButton", typeof(MouseButton), typeof(FlyoutsControl), new PropertyMetadata(MouseButton.Left));
        public static readonly DependencyProperty OverrideIsPinnedProperty =
            DependencyProperty.Register("OverrideIsPinned", typeof(bool), typeof(FlyoutsControl), new PropertyMetadata(false));

        public MouseButton ExternalCloseButton
        {
            get { return (MouseButton) GetValue(ExternalCloseButtonProperty); }
            set { SetValue(ExternalCloseButtonProperty, value); }
        }
        
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
                    this.AttachHandlers(this.GetFlyouts(this.Items));
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
            isOpenChanged.AddValueChanged(item, this.FlyoutIsOpenChanged);
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
            isOpenChanged.RemoveValueChanged(item, this.FlyoutIsOpenChanged);
        }

        private void FlyoutIsOpenChanged(object sender, EventArgs e)
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
