// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Shapes;
using Microsoft.Xaml.Behaviors;

namespace MetroDemo.Behaviors
{
    public class BindableResourceBehavior : Behavior<Shape>
    {
        public static readonly DependencyProperty ResourceNameProperty
            = DependencyProperty.Register(
                nameof(ResourceName),
                typeof(string),
                typeof(BindableResourceBehavior),
                new PropertyMetadata(default(string),
                                     (d, e) =>
                                         {
                                             if (d is BindableResourceBehavior behavior && behavior.Property is not null && e.OldValue != e.NewValue && !string.IsNullOrEmpty((string?)e.NewValue))
                                             {
                                                 behavior.AssociatedObject?.SetResourceReference(behavior.Property, e.NewValue);
                                             }
                                         }));

        public string? ResourceName
        {
            get => (string?)this.GetValue(ResourceNameProperty);
            set => this.SetValue(ResourceNameProperty, value);
        }

        public static readonly DependencyProperty PropertyProperty
            = DependencyProperty.Register(
                nameof(Property),
                typeof(DependencyProperty),
                typeof(BindableResourceBehavior),
                new PropertyMetadata(default(DependencyProperty),
                                     (d, e) =>
                                         {
                                             if (d is BindableResourceBehavior behavior && e.OldValue != e.NewValue && e.NewValue is DependencyProperty property && !string.IsNullOrEmpty(behavior.ResourceName))
                                             {
                                                 behavior.AssociatedObject?.SetResourceReference(property, behavior.ResourceName);
                                             }
                                         }));

        public DependencyProperty? Property
        {
            get => (DependencyProperty?)this.GetValue(PropertyProperty);
            set => this.SetValue(PropertyProperty, value);
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            if (this.Property is not null && !string.IsNullOrEmpty(this.ResourceName))
            {
                this.AssociatedObject.SetResourceReference(this.Property, this.ResourceName);
            }
        }
    }
}