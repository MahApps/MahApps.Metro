// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;

namespace MahApps.Metro.Controls
{
    [TemplatePart(Name = UnderlineBorderPartName, Type = typeof(Border))]
    public class Underline : ContentControl
    {
        public const string UnderlineBorderPartName = "PART_UnderlineBorder";
        private Border underlineBorder;

        public static readonly DependencyProperty PlacementProperty
            = DependencyProperty.Register(nameof(Placement),
                                          typeof(Dock),
                                          typeof(Underline),
                                          new PropertyMetadata(default(Dock), (o, e) => { (o as Underline)?.ApplyBorderProperties(); }));

        public Dock Placement
        {
            get => (Dock)this.GetValue(PlacementProperty);
            set => this.SetValue(PlacementProperty, value);
        }

        public static readonly DependencyProperty LineThicknessProperty
            = DependencyProperty.Register(nameof(LineThickness),
                                          typeof(double),
                                          typeof(Underline),
                                          new PropertyMetadata(1d, (o, e) => { (o as Underline)?.ApplyBorderProperties(); }));

        public double LineThickness
        {
            get => (double)this.GetValue(LineThicknessProperty);
            set => this.SetValue(LineThicknessProperty, value);
        }

        public static readonly DependencyProperty LineExtentProperty
            = DependencyProperty.Register(nameof(LineExtent),
                                          typeof(double),
                                          typeof(Underline),
                                          new PropertyMetadata(Double.NaN, (o, e) => { (o as Underline)?.ApplyBorderProperties(); }));

        public double LineExtent
        {
            get => (double)this.GetValue(LineExtentProperty);
            set => this.SetValue(LineExtentProperty, value);
        }

        static Underline()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Underline), new FrameworkPropertyMetadata(typeof(Underline)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.underlineBorder = this.GetTemplateChild(UnderlineBorderPartName) as Border;

            this.ApplyBorderProperties();
        }

        private void ApplyBorderProperties()
        {
            if (this.underlineBorder == null)
            {
                return;
            }

            void Execute()
            {
                this.underlineBorder.Height = Double.NaN;
                this.underlineBorder.Width = Double.NaN;
                this.underlineBorder.BorderThickness = new Thickness();
                switch (this.Placement)
                {
                    case Dock.Left:
                        this.underlineBorder.Width = this.LineExtent;
                        this.underlineBorder.BorderThickness = new Thickness(this.LineThickness, 0d, 0d, 0d);
                        break;
                    case Dock.Top:
                        this.underlineBorder.Height = this.LineExtent;
                        this.underlineBorder.BorderThickness = new Thickness(0d, this.LineThickness, 0d, 0d);
                        break;
                    case Dock.Right:
                        this.underlineBorder.Width = this.LineExtent;
                        this.underlineBorder.BorderThickness = new Thickness(0d, 0d, this.LineThickness, 0d);
                        break;
                    case Dock.Bottom:
                        this.underlineBorder.Height = this.LineExtent;
                        this.underlineBorder.BorderThickness = new Thickness(0d, 0d, 0d, this.LineThickness);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                this.InvalidateVisual();
            }

            this.ExecuteWhenLoaded(Execute);
        }
    }
}