using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ControlzEx.Theming;
using MahApps.Metro.Controls;
using Microsoft.Xaml.Behaviors;

namespace MahApps.Metro.Behaviors
{
    public class TiltBehavior : Behavior<FrameworkElement>
    {
        /// <summary>Identifies the <see cref="KeepDragging"/> dependency property.</summary>
        public static readonly DependencyProperty KeepDraggingProperty
            = DependencyProperty.Register(nameof(KeepDragging),
                                          typeof(bool),
                                          typeof(TiltBehavior),
                                          new PropertyMetadata(true));

        public bool KeepDragging
        {
            get => (bool)this.GetValue(KeepDraggingProperty);
            set => this.SetValue(KeepDraggingProperty, value);
        }

        /// <summary>Identifies the <see cref="TiltFactor"/> dependency property.</summary>
        public static readonly DependencyProperty TiltFactorProperty
            = DependencyProperty.Register(nameof(TiltFactor),
                                          typeof(int),
                                          typeof(TiltBehavior),
                                          new PropertyMetadata(20));

        public int TiltFactor
        {
            get => (int)this.GetValue(TiltFactorProperty);
            set => this.SetValue(TiltFactorProperty, value);
        }

        private bool isPressed;
        private Thickness originalMargin;
        private Panel originalPanel;
        private Size originalSize;
        private FrameworkElement attachedElement;
        private Point current = new Point(-99, -99);
        private int times = -1;

        public Planerator RotatorParent { get; private set; }

        protected override void OnAttached()
        {
            this.attachedElement = this.AssociatedObject;
            if (this.attachedElement is ListBox)
            {
                return;
            }

            if (this.attachedElement is Panel panel)
            {
                panel.Loaded += (sl, el) =>
                    {
                        var elements = panel.Children.Cast<UIElement>().ToList();

                        elements.ForEach(element =>
                                             Interaction.GetBehaviors(element).Add(
                                                 new TiltBehavior
                                                 {
                                                     KeepDragging = this.KeepDragging,
                                                     TiltFactor = this.TiltFactor
                                                 }));
                    };

                return;
            }

            this.originalPanel = this.attachedElement.Parent as Panel ?? GetParentPanel(this.attachedElement);

            this.originalMargin = this.attachedElement.Margin;
            this.originalSize = new Size(this.attachedElement.Width, this.attachedElement.Height);
            double left = Canvas.GetLeft(this.attachedElement);
            double right = Canvas.GetRight(this.attachedElement);
            double top = Canvas.GetTop(this.attachedElement);
            double bottom = Canvas.GetBottom(this.attachedElement);
            int z = Panel.GetZIndex(this.attachedElement);
            VerticalAlignment va = this.attachedElement.VerticalAlignment;
            HorizontalAlignment ha = this.attachedElement.HorizontalAlignment;

            this.RotatorParent = new Planerator
                                 {
                                     Margin = this.originalMargin,
                                     Width = this.originalSize.Width,
                                     Height = this.originalSize.Height,
                                     VerticalAlignment = va,
                                     HorizontalAlignment = ha
                                 };

            this.RotatorParent.SetValue(Canvas.LeftProperty, left);
            this.RotatorParent.SetValue(Canvas.RightProperty, right);
            this.RotatorParent.SetValue(Canvas.TopProperty, top);
            this.RotatorParent.SetValue(Canvas.BottomProperty, bottom);
            this.RotatorParent.SetValue(Panel.ZIndexProperty, z);

            this.originalPanel.Children.Remove(this.attachedElement);
            this.attachedElement.Margin = new Thickness();
            this.attachedElement.Width = double.NaN;
            this.attachedElement.Height = double.NaN;

            this.originalPanel.Children.Add(this.RotatorParent);
            this.RotatorParent.Child = this.attachedElement;

            CompositionTarget.Rendering += this.CompositionTargetRendering;
            ThemeManager.Current.ThemeChanged += this.ThemeManagerIsThemeChanged;
        }

        private void ThemeManagerIsThemeChanged(object sender, ThemeChangedEventArgs e)
        {
            this.RotatorParent?.Refresh();
        }

        protected override void OnDetaching()
        {
            CompositionTarget.Rendering -= this.CompositionTargetRendering;
            ThemeManager.Current.ThemeChanged -= this.ThemeManagerIsThemeChanged;
            base.OnDetaching();
        }

        private void CompositionTargetRendering(object sender, EventArgs e)
        {
            if (this.KeepDragging)
            {
                this.current = Mouse.GetPosition(this.RotatorParent.Child);
                if (Mouse.LeftButton == MouseButtonState.Pressed)
                {
                    if (this.current.X > 0 && this.current.X < (this.attachedElement).ActualWidth && this.current.Y > 0 && this.current.Y < (this.attachedElement).ActualHeight)
                    {
                        this.RotatorParent.RotationY = -1 * this.TiltFactor + this.current.X * 2 * this.TiltFactor / (this.attachedElement).ActualWidth;
                        this.RotatorParent.RotationX = -1 * this.TiltFactor + this.current.Y * 2 * this.TiltFactor / (this.attachedElement).ActualHeight;
                    }
                }
                else
                {
                    this.RotatorParent.RotationY = this.RotatorParent.RotationY - 5 < 0 ? 0 : this.RotatorParent.RotationY - 5;
                    this.RotatorParent.RotationX = this.RotatorParent.RotationX - 5 < 0 ? 0 : this.RotatorParent.RotationX - 5;
                }
            }
            else
            {
                if (Mouse.LeftButton == MouseButtonState.Pressed)
                {
                    if (!this.isPressed)
                    {
                        this.current = Mouse.GetPosition(this.RotatorParent.Child);
                        if (this.current.X > 0 && this.current.X < (this.attachedElement).ActualWidth && this.current.Y > 0 && this.current.Y < (this.attachedElement).ActualHeight)
                        {
                            this.RotatorParent.RotationY = -1 * this.TiltFactor + this.current.X * 2 * this.TiltFactor / (this.attachedElement).ActualWidth;
                            this.RotatorParent.RotationX = -1 * this.TiltFactor + this.current.Y * 2 * this.TiltFactor / (this.attachedElement).ActualHeight;
                        }

                        this.isPressed = true;
                    }

                    if (this.times == 7)
                    {
                        this.RotatorParent.RotationY = this.RotatorParent.RotationY - 5 < 0 ? 0 : this.RotatorParent.RotationY - 5;
                        this.RotatorParent.RotationX = this.RotatorParent.RotationX - 5 < 0 ? 0 : this.RotatorParent.RotationX - 5;
                    }
                    else if (this.times < 7)
                    {
                        this.times++;
                    }
                }
                else
                {
                    this.isPressed = false;
                    this.times = -1;
                    this.RotatorParent.RotationY = this.RotatorParent.RotationY - 5 < 0 ? 0 : this.RotatorParent.RotationY - 5;
                    this.RotatorParent.RotationX = this.RotatorParent.RotationX - 5 < 0 ? 0 : this.RotatorParent.RotationX - 5;
                }
            }
        }

        private static Panel GetParentPanel(DependencyObject element)
        {
            return element.TryFindParent<Panel>();
        }
    }
}