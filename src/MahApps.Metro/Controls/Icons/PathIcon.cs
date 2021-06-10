// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

// ReSharper disable once CheckNamespace
namespace MahApps.Metro.Controls
{
    /// <summary>
    /// Represents an icon that uses a vector path as its content.
    /// </summary>
    [TemplatePart(Name = nameof(PART_Path), Type = typeof(Path))]
    public class PathIcon : IconElement
    {
        /// <summary>Identifies the Data dependency property.</summary>
        public static readonly DependencyProperty DataProperty
            = Path.DataProperty.AddOwner(typeof(PathIcon),
                                         new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Gets or sets a Geometry that specifies the shape to be drawn. In XAML this can also be set using the Path Markup Syntax.
        /// </summary>
        public Geometry? Data
        {
            get => (Geometry?)this.GetValue(DataProperty);
            set => this.SetValue(DataProperty, value);
        }

        static PathIcon()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PathIcon), new FrameworkPropertyMetadata(typeof(PathIcon)));
            FocusableProperty.OverrideMetadata(typeof(PathIcon), new FrameworkPropertyMetadata(false));
        }

        private Path? PART_Path { get; set; }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.PART_Path = this.GetTemplateChild(nameof(this.PART_Path)) as Path;

            if (this.PART_Path is not null && this.InheritsForegroundFromVisualParent)
            {
                this.PART_Path.Fill = this.VisualParentForeground;
            }
        }

        protected override void OnInheritsForegroundFromVisualParentPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnInheritsForegroundFromVisualParentPropertyChanged(e);

            if (this.PART_Path is not null)
            {
                if (this.InheritsForegroundFromVisualParent)
                {
                    this.PART_Path.Fill = this.VisualParentForeground;
                }
                else
                {
                    this.PART_Path.ClearValue(Shape.FillProperty);
                }
            }
        }

        protected override void OnVisualParentForegroundPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnVisualParentForegroundPropertyChanged(e);

            if (this.PART_Path is not null && this.InheritsForegroundFromVisualParent)
            {
                this.PART_Path.Fill = e.NewValue as Brush;
            }
        }
    }
}