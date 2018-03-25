using System;
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

        private bool isPressed;
        private Thickness originalMargin;
        private Panel originalPanel;
        private Size originalSize;
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
                return;
            }

            var attachedElementPanel = attachedElement as Panel;
            if (attachedElementPanel != null)
            {
                attachedElementPanel.Loaded += (sl, el) =>
                {
                    var elements = attachedElementPanel.Children.Cast<UIElement>().ToList();

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

            originalPanel = attachedElement.Parent as Panel ?? GetParentPanel(attachedElement);

            originalMargin = attachedElement.Margin;
            originalSize = new Size(attachedElement.Width, attachedElement.Height);
            double left = Canvas.GetLeft(attachedElement);
            double right = Canvas.GetRight(attachedElement);
            double top = Canvas.GetTop(attachedElement);
            double bottom = Canvas.GetBottom(attachedElement);
            int z = Panel.GetZIndex(attachedElement);
            VerticalAlignment va = attachedElement.VerticalAlignment;
            HorizontalAlignment ha = attachedElement.HorizontalAlignment;

            RotatorParent = new Planerator
            {
                Margin = originalMargin,
                Width = originalSize.Width,
                Height = originalSize.Height,
                VerticalAlignment = va,
                HorizontalAlignment = ha
            };

            RotatorParent.SetValue(Canvas.LeftProperty, left);
            RotatorParent.SetValue(Canvas.RightProperty, right);
            RotatorParent.SetValue(Canvas.TopProperty, top);
            RotatorParent.SetValue(Canvas.BottomProperty, bottom);
            RotatorParent.SetValue(Panel.ZIndexProperty, z);

            originalPanel.Children.Remove(attachedElement);
            attachedElement.Margin = new Thickness();
            attachedElement.Width = double.NaN;
            attachedElement.Height = double.NaN;

            originalPanel.Children.Add(RotatorParent);
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
                    if (!isPressed)
                    {
                        current = Mouse.GetPosition(RotatorParent.Child);
                        if (current.X > 0 && current.X < (attachedElement).ActualWidth && current.Y > 0 &&
                            current.Y < (attachedElement).ActualHeight)
                        {
                            RotatorParent.RotationY = -1 * TiltFactor + current.X * 2 * TiltFactor / (attachedElement).ActualWidth;
                            RotatorParent.RotationX = -1 * TiltFactor + current.Y * 2 * TiltFactor / (attachedElement).ActualHeight;
                        }
                        isPressed = true;
                    }


                    if (isPressed && times == 7)
                    {
                        RotatorParent.RotationY = RotatorParent.RotationY - 5 < 0 ? 0 : RotatorParent.RotationY - 5;
                        RotatorParent.RotationX = RotatorParent.RotationX - 5 < 0 ? 0 : RotatorParent.RotationX - 5;
                    }
                    else if (isPressed && times < 7)
                    {
                        times++;
                    }
                }
                else
                {
                    isPressed = false;
                    times = -1;
                    RotatorParent.RotationY = RotatorParent.RotationY - 5 < 0 ? 0 : RotatorParent.RotationY - 5;
                    RotatorParent.RotationX = RotatorParent.RotationX - 5 < 0 ? 0 : RotatorParent.RotationX - 5;
                }
            }
        }

        private static Panel GetParentPanel(DependencyObject element)
        {
            return element.TryFindParent<Panel>();
        }
    }
}