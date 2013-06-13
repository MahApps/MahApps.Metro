using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;

namespace MahApps.Metro.Controls
{
    using System.Windows;

    [StyleTypedProperty(Property = "ItemContainerStyle", StyleTargetType = typeof(Flyout))]
    public class FlyoutsControl : ItemsControl
    {
        #region Constructors and Destructors

        static FlyoutsControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(FlyoutsControl), new FrameworkPropertyMetadata(typeof(FlyoutsControl)));
        }

        #endregion

        #region Methods

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
                    this.AttachHandlers(e.NewItems.Cast<Flyout>());
                    break;
                case NotifyCollectionChangedAction.Replace:
                    this.AttachHandlers(e.NewItems.Cast<Flyout>());
                    this.DetachHandlers(e.OldItems.Cast<Flyout>());
                    break;
                case NotifyCollectionChangedAction.Remove:
                    this.DetachHandlers(e.OldItems.Cast<Flyout>());
                    break;
                case NotifyCollectionChangedAction.Reset:
                    this.AttachHandlers(this.Items.Cast<Flyout>());
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
            this.ReorderZIndices((Flyout)sender);
        }

        private void ReorderZIndices(Flyout lastChanged)
        {
            var openFlyouts = this.Items.Cast<Flyout>().Where(i => i.IsOpen && i != lastChanged).OrderBy(Panel.GetZIndex);
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

        #endregion
    }
}