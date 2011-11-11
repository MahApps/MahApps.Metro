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

        bool _isPressed;
        Thickness _originalMargin;
        Panel _originalPanel;
        Size _originalSize;
        FrameworkElement _attachedElement;

        Point _current = new Point(-99, -99);
        Int32 _times = -1;

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
            _attachedElement = AssociatedObject;
            var panel = _attachedElement as Panel;
            if (panel != null)
            {
                panel.Loaded += (sl, el) =>
                                    {
                                        var elements = panel.Children.Cast<UIElement>().ToList();

                                        elements.ForEach(
                                            element =>
                                            Interaction.GetBehaviors(element).Add(
                                                new TiltBehavior
                                                    {
                                                        KeepDragging = KeepDragging,
                                                        TiltFactor = TiltFactor
                                                    }));
                                    };

                return;
            }

            _originalPanel = _attachedElement.Parent as Panel ?? GetParentPanel(_attachedElement);

            _originalMargin = _attachedElement.Margin;
            _originalSize = new Size(_attachedElement.Width, _attachedElement.Height);
            double left = Canvas.GetLeft(_attachedElement);
            double right = Canvas.GetRight(_attachedElement);
            double top = Canvas.GetTop(_attachedElement);
            double bottom = Canvas.GetBottom(_attachedElement);
            int z = Panel.GetZIndex(_attachedElement);
            VerticalAlignment va = _attachedElement.VerticalAlignment;
            HorizontalAlignment ha = _attachedElement.HorizontalAlignment;


            RotatorParent = new Planerator
                                {
                                    Margin = _originalMargin,
                                    Width = _originalSize.Width,
                                    Height = _originalSize.Height,
                                    VerticalAlignment = va,
                                    HorizontalAlignment = ha
                                };

            RotatorParent.SetValue(Canvas.LeftProperty, left);
            RotatorParent.SetValue(Canvas.RightProperty, right);
            RotatorParent.SetValue(Canvas.TopProperty, top);
            RotatorParent.SetValue(Canvas.BottomProperty, bottom);
            RotatorParent.SetValue(Panel.ZIndexProperty, z);

            _originalPanel.Children.Remove(_attachedElement);
            _attachedElement.Margin = new Thickness();
            _attachedElement.Width = double.NaN;
            _attachedElement.Height = double.NaN;

            _originalPanel.Children.Add(RotatorParent);
            RotatorParent.Child = _attachedElement;

            CompositionTarget.Rendering += CompositionTargetRendering;
        }

        private void CompositionTargetRendering(object sender, EventArgs e)
        {
            if (KeepDragging)
            {
                _current = Mouse.GetPosition(RotatorParent.Child);
                if (Mouse.LeftButton == MouseButtonState.Pressed)
                {
                    if (_current.X > 0 && _current.X < (_attachedElement).ActualWidth && _current.Y > 0 &&
                        _current.Y < (_attachedElement).ActualHeight)
                    {
                        RotatorParent.RotationY = -1 * TiltFactor + _current.X * 2 * TiltFactor / (_attachedElement).ActualWidth;
                        RotatorParent.RotationX = -1 * TiltFactor + _current.Y * 2 * TiltFactor / (_attachedElement).ActualHeight;
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
                    if (!_isPressed)
                    {
                        _current = Mouse.GetPosition(RotatorParent.Child);
                        if (_current.X > 0 && _current.X < (_attachedElement).ActualWidth && _current.Y > 0 &&
                            _current.Y < (_attachedElement).ActualHeight)
                        {
                            RotatorParent.RotationY = -1 * TiltFactor +
                                                      _current.X * 2 * TiltFactor / (_attachedElement).ActualWidth;
                            RotatorParent.RotationX = -1 * TiltFactor +
                                                      _current.Y * 2 * TiltFactor / (_attachedElement).ActualHeight;
                        }
                        _isPressed = true;
                    }


                    if (_isPressed && _times == 7)
                    {
                        RotatorParent.RotationY = RotatorParent.RotationY - 5 < 0 ? 0 : RotatorParent.RotationY - 5;
                        RotatorParent.RotationX = RotatorParent.RotationX - 5 < 0 ? 0 : RotatorParent.RotationX - 5;
                    }
                    else if (_isPressed && _times < 7)
                    {
                        _times++;
                    }
                }
                else
                {
                    _isPressed = false;
                    _times = -1;
                    RotatorParent.RotationY = RotatorParent.RotationY - 5 < 0 ? 0 : RotatorParent.RotationY - 5;
                    RotatorParent.RotationX = RotatorParent.RotationX - 5 < 0 ? 0 : RotatorParent.RotationX - 5;
                }
            }
        }

        private static Panel GetParentPanel(DependencyObject element)
        {
            var parent = VisualTreeHelper.GetParent(element);
            var parentPanel = parent as Panel;
            if (parentPanel != null)
                return parentPanel;
            return parent == null ? null : GetParentPanel(parent);
        }
    }


}