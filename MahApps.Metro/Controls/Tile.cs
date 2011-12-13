using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MahApps.Metro.Controls
{
    public class Tile : Button
    {
        Planerator _rotatorParent;
        Point _current;
        bool _isPressed;
        decimal _times;

        public Tile()
        {
            DefaultStyleKey = typeof (Tile);
            CompositionTarget.Rendering += CompositionTargetRendering;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _rotatorParent = (Planerator)GetTemplateChild("planerator");
        }

        private void CompositionTargetRendering(object sender, EventArgs e)
        {
            if (_rotatorParent == null) return;
            if (KeepDragging)
            {
                _current = Mouse.GetPosition(this);
                if (Mouse.LeftButton == MouseButtonState.Pressed)
                {
                    if (_current.X > 0 && _current.X < ActualWidth && _current.Y > 0 && _current.Y < ActualHeight)
                    {
                        _rotatorParent.RotationY = -1 * TiltFactor + _current.X * 2 * TiltFactor / ActualWidth;
                        _rotatorParent.RotationX = -1 * TiltFactor + _current.Y * 2 * TiltFactor / ActualHeight;
                    }
                }
                else
                {
                    _rotatorParent.RotationY = _rotatorParent.RotationY - 5 < 0 ? 0 : _rotatorParent.RotationY - 5;
                    _rotatorParent.RotationX = _rotatorParent.RotationX - 5 < 0 ? 0 : _rotatorParent.RotationX - 5;
                }
            }
            else
            {
                if (Mouse.LeftButton == MouseButtonState.Pressed)
                {
                    if (!_isPressed)
                    {
                        _current = Mouse.GetPosition(_rotatorParent.Child);
                        if (_current.X > 0 && _current.X < ActualWidth && _current.Y > 0 && _current.Y < ActualHeight)
                        {
                            _rotatorParent.RotationY = -1 * TiltFactor + _current.X * 2 * TiltFactor / ActualWidth;
                            _rotatorParent.RotationX = -1 * TiltFactor + _current.Y * 2 * TiltFactor / ActualHeight;
                        }
                        _isPressed = true;
                    }

                    if (_isPressed && _times == 7)
                    {
                        _rotatorParent.RotationY = _rotatorParent.RotationY - 5 < 0 ? 0 : _rotatorParent.RotationY - 5;
                        _rotatorParent.RotationX = _rotatorParent.RotationX - 5 < 0 ? 0 : _rotatorParent.RotationX - 5;
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
                    _rotatorParent.RotationY = _rotatorParent.RotationY - 5 < 0 ? 0 : _rotatorParent.RotationY - 5;
                    _rotatorParent.RotationX = _rotatorParent.RotationX - 5 < 0 ? 0 : _rotatorParent.RotationX - 5;
                }
            }
        }

        #region public string Title
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(Tile), new PropertyMetadata(default(string)));

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }
        #endregion

        #region public string Count
        public static readonly DependencyProperty CountProperty =
            DependencyProperty.Register("Count", typeof(string), typeof(Tile), new PropertyMetadata(default(string)));

        public string Count
        {
            get { return (string)GetValue(CountProperty); }
            set { SetValue(CountProperty, value); }
        }
        #endregion

        #region public bool KeepDragging
        public static readonly DependencyProperty KeepDraggingProperty =
            DependencyProperty.Register("KeepDragging", typeof(bool), typeof(Tile), new PropertyMetadata(true));

        public bool KeepDragging
        {
            get { return (bool)GetValue(KeepDraggingProperty); }
            set { SetValue(KeepDraggingProperty, value); }
        }
        #endregion

        #region public int TiltFactor
        public static readonly DependencyProperty TiltFactorProperty =
            DependencyProperty.Register("TiltFactor", typeof(int), typeof(Tile), new PropertyMetadata(5));

        public int TiltFactor
        {
            get { return (Int32)GetValue(TiltFactorProperty); }
            set { SetValue(TiltFactorProperty, value); }
        }
        #endregion
    }
}