using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media;
using MahApps.Metro.Controls;

namespace MahApps.Metro.Behaviours
{
    public class TiltBehavior : Behavior<FrameworkElement>
    {
        public static readonly DependencyProperty KeepDraggingProperty =
            DependencyProperty.Register("KeepDragging", typeof(bool), typeof(TiltBehavior), new PropertyMetadata(true));

        public static readonly DependencyProperty TiltFactorProperty =
            DependencyProperty.Register("TiltFactor", typeof(Int32), typeof(TiltBehavior), new PropertyMetadata(20));

        private bool IsPressed;

        private Thickness OriginalMargin;
        private Panel OriginalPanel;
        private Size OriginalSize;
        private FrameworkElement attachedElement;

        private Point current = new Point(-99, -99);
        private Int32 times = -1;

        public bool KeepDragging
        {
            get { return (bool)GetValue(KeepDraggingProperty); }
            set { SetValue(KeepDraggingProperty, value); }
        }

        public Int32 TiltFactor
        {
            get { return (Int32)GetValue(TiltFactorProperty); }
            set { SetValue(TiltFactorProperty, value); }
        }

        public Planerator RotatorParent { get; set; }

        protected override void OnAttached()
        {
            attachedElement = AssociatedObject;
            if (attachedElement is ListBox)
            {
                var l = (ListBox) attachedElement;
                l.Items.CurrentChanging += (s, e) =>
                {
                    Console.WriteLine("foo");
                };
                return;
            }
            if (attachedElement as Panel != null)
            {
                var y = (attachedElement as ItemsControl);
                y.Items.CurrentChanging += (s, e) => Console.WriteLine("foo");

                (attachedElement as Panel).Loaded += (sl, el) =>
                {
                    var elements = (attachedElement as Panel).Children.Cast<UIElement>().ToList();

                    elements.ForEach(element =>
                        Interaction.GetBehaviors(element).Add(
                            new TiltBehavior
                            {
                                KeepDragging = KeepDragging,
                                TiltFactor = TiltFactor
                            }));
                };

                return;
            }

            OriginalPanel = attachedElement.Parent as Panel ?? GetParentPanel(attachedElement);

            OriginalMargin = attachedElement.Margin;
            OriginalSize = new Size(attachedElement.Width, attachedElement.Height);
            double left = Canvas.GetLeft(attachedElement);
            double right = Canvas.GetRight(attachedElement);
            double top = Canvas.GetTop(attachedElement);
            double bottom = Canvas.GetBottom(attachedElement);
            int z = Panel.GetZIndex(attachedElement);
            VerticalAlignment va = attachedElement.VerticalAlignment;
            HorizontalAlignment ha = attachedElement.HorizontalAlignment;

            RotatorParent = new Planerator
            {
                Margin = OriginalMargin,
                Width = OriginalSize.Width,
                Height = OriginalSize.Height,
                VerticalAlignment = va,
                HorizontalAlignment = ha
            };

            RotatorParent.SetValue(Canvas.LeftProperty, left);
            RotatorParent.SetValue(Canvas.RightProperty, right);
            RotatorParent.SetValue(Canvas.TopProperty, top);
            RotatorParent.SetValue(Canvas.BottomProperty, bottom);
            RotatorParent.SetValue(Panel.ZIndexProperty, z);

            OriginalPanel.Children.Remove(attachedElement);
            attachedElement.Margin = new Thickness();
            attachedElement.Width = double.NaN;
            attachedElement.Height = double.NaN;

            OriginalPanel.Children.Add(RotatorParent);
            RotatorParent.Child = attachedElement;

            CompositionTarget.Rendering += CompositionTargetRendering;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            CompositionTarget.Rendering -= CompositionTargetRendering;
        }

        private void CompositionTargetRendering(object sender, EventArgs e)
        {
            if (KeepDragging)
            {
                current = Mouse.GetPosition(RotatorParent.Child);
                if (Mouse.LeftButton == MouseButtonState.Pressed)
                {
                    if (current.X > 0 && current.X < (attachedElement).ActualWidth && current.Y > 0 &&
                        current.Y < (attachedElement).ActualHeight)
                    {
                        RotatorParent.RotationY = -1 * TiltFactor + current.X * 2 * TiltFactor / (attachedElement).ActualWidth;
                        RotatorParent.RotationX = -1 * TiltFactor + current.Y * 2 * TiltFactor / (attachedElement).ActualHeight;
                    }
                }
                else
                {
                    RotatorParent.RotationY = RotatorParent.RotationY - 5 < 0 ? 0 : RotatorParent.RotationY - 5;
                    RotatorParent.RotationX = RotatorParent.RotationX - 5 < 0 ? 0 : RotatorParent.RotationX - 5;
                }
            }
            else
            {
                if (Mouse.LeftButton == MouseButtonState.Pressed)
                {
                    if (!IsPressed)
                    {
                        current = Mouse.GetPosition(RotatorParent.Child);
                        if (current.X > 0 && current.X < (attachedElement).ActualWidth && current.Y > 0 &&
                            current.Y < (attachedElement).ActualHeight)
                        {
                            RotatorParent.RotationY = -1 * TiltFactor +
                                                      current.X * 2 * TiltFactor / (attachedElement).ActualWidth;
                            RotatorParent.RotationX = -1 * TiltFactor +
                                                      current.Y * 2 * TiltFactor / (attachedElement).ActualHeight;
                        }
                        IsPressed = true;
                    }


                    if (IsPressed && times == 7)
                    {
                        RotatorParent.RotationY = RotatorParent.RotationY - 5 < 0 ? 0 : RotatorParent.RotationY - 5;
                        RotatorParent.RotationX = RotatorParent.RotationX - 5 < 0 ? 0 : RotatorParent.RotationX - 5;
                    }
                    else if (IsPressed && times < 7)
                    {
                        times++;
                    }
                }
                else
                {
                    IsPressed = false;
                    times = -1;
                    RotatorParent.RotationY = RotatorParent.RotationY - 5 < 0 ? 0 : RotatorParent.RotationY - 5;
                    RotatorParent.RotationX = RotatorParent.RotationX - 5 < 0 ? 0 : RotatorParent.RotationX - 5;
                }
            }
        }

        private static Panel GetParentPanel(DependencyObject element)
        {
            var parent = VisualTreeHelper.GetParent(element);
            var panel = parent as Panel;

            return panel ?? (parent == null ? null : GetParentPanel(parent));
        }
    }
}