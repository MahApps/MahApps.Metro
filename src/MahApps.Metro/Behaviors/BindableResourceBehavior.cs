// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Shapes;
using Microsoft.Xaml.Behaviors;

namespace MahApps.Metro.Behaviors
{
    public class BindableResourceBehavior : Behavior<Shape>
    {
        public static readonly DependencyProperty ResourceNameProperty = DependencyProperty.Register(nameof(ResourceName), typeof(string), typeof(BindableResourceBehavior), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty PropertyProperty = DependencyProperty.Register(nameof(Property), typeof(DependencyProperty), typeof(BindableResourceBehavior), new PropertyMetadata(default(DependencyProperty)));

        protected override void OnAttached()
        {
            AssociatedObject.SetResourceReference(Property, ResourceName);
            base.OnAttached();
        }

        public string ResourceName
        {
            get { return (string)GetValue(ResourceNameProperty); }
            set { SetValue(ResourceNameProperty, value); }
        }

        public DependencyProperty Property
        {
            get { return (DependencyProperty)GetValue(PropertyProperty); }
            set { SetValue(PropertyProperty, value); }
        }
    }
}