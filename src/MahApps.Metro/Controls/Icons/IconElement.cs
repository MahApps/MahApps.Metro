// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using MahApps.Metro.ValueBoxes;

// ReSharper disable once CheckNamespace
namespace MahApps.Metro.Controls
{
    /// <summary>
    /// Represents the base class for an icon UI element.
    /// </summary>
    public abstract class IconElement : Control
    {
        private bool isForegroundPropertyDefaultOrInherited = true;

        protected IconElement()
        {
            // nothing here
        }

        static IconElement()
        {
            ForegroundProperty.OverrideMetadata(typeof(IconElement),
                                                new FrameworkPropertyMetadata(SystemColors.ControlTextBrush,
                                                                              FrameworkPropertyMetadataOptions.Inherits,
                                                                              (sender, e) => ((IconElement)sender).OnForegroundPropertyChanged(e)));
        }

        protected void OnForegroundPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            var baseValueSource = DependencyPropertyHelper.GetValueSource(this, e.Property).BaseValueSource;
            this.isForegroundPropertyDefaultOrInherited = baseValueSource <= BaseValueSource.Inherited;
            this.UpdateInheritsForegroundFromVisualParent();
        }

        protected override void OnVisualParentChanged(DependencyObject oldParent)
        {
            base.OnVisualParentChanged(oldParent);
            this.UpdateInheritsForegroundFromVisualParent();
        }

        private void UpdateInheritsForegroundFromVisualParent()
        {
            this.InheritsForegroundFromVisualParent
                = this.isForegroundPropertyDefaultOrInherited
                  && this.Parent != null
                  && this.VisualParent != null
                  && this.Parent != this.VisualParent;
        }

        /// <summary>Identifies the <see cref="InheritsForegroundFromVisualParent"/> dependency property.</summary>
        internal static readonly DependencyPropertyKey InheritsForegroundFromVisualParentPropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(InheritsForegroundFromVisualParent),
                                                  typeof(bool),
                                                  typeof(IconElement),
                                                  new PropertyMetadata(BooleanBoxes.FalseBox, (sender, e) => ((IconElement)sender).OnInheritsForegroundFromVisualParentPropertyChanged(e)));

        /// <summary>Identifies the <see cref="InheritsForegroundFromVisualParent"/> dependency property.</summary>
        public static readonly DependencyProperty InheritsForegroundFromVisualParentProperty = InheritsForegroundFromVisualParentPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets whether that this element inherits the <see cref="Control.Foreground"/> form the <see cref="Visual.VisualParent"/>.
        /// </summary>
        public bool InheritsForegroundFromVisualParent
        {
            get => (bool)this.GetValue(InheritsForegroundFromVisualParentProperty);
            protected set => this.SetValue(InheritsForegroundFromVisualParentPropertyKey, BooleanBoxes.Box(value));
        }

        protected virtual void OnInheritsForegroundFromVisualParentPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != e.NewValue)
            {
                if (e.NewValue is true)
                {
                    this.SetBinding(VisualParentForegroundProperty,
                                    new Binding
                                    {
                                        Path = new PropertyPath(TextElement.ForegroundProperty),
                                        Source = this.VisualParent
                                    });
                }
                else
                {
                    this.ClearValue(VisualParentForegroundProperty);
                }
            }
        }

        private static readonly DependencyProperty VisualParentForegroundProperty
            = DependencyProperty.Register(nameof(VisualParentForeground),
                                          typeof(Brush),
                                          typeof(IconElement),
                                          new PropertyMetadata(default(Brush), (sender, e) => ((IconElement)sender).OnVisualParentForegroundPropertyChanged(e)));

        protected Brush? VisualParentForeground
        {
            get => (Brush?)this.GetValue(VisualParentForegroundProperty);
            set => this.SetValue(VisualParentForegroundProperty, value);
        }

        protected virtual void OnVisualParentForegroundPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
        }
    }
}