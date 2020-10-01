using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace MahApps.Metro.Controls
{
    public class ColorEyeDropper : ContentControl
    {
        // Depency Properties
        /// <summary>Identifies the <see cref="SelectedColor"/> dependency property.</summary>
        public static readonly DependencyProperty SelectedColorProperty = DependencyProperty.Register(nameof(SelectedColor), typeof(Color), typeof(ColorEyeDropper), new FrameworkPropertyMetadata(Colors.Black, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        /// <summary>Identifies the <see cref="PreviewBrush"/> dependency property.</summary>
        public static readonly DependencyProperty PreviewBrushProperty = DependencyProperty.Register(nameof(PreviewBrush), typeof(Brush), typeof(ColorEyeDropper), new PropertyMetadata(Brushes.Transparent));

        /// <summary>Identifies the <see cref="PreviewImageSource"/> dependency property.</summary>
        public static readonly DependencyProperty PreviewImageSourceProperty = DependencyProperty.Register(nameof(PreviewImageSource), typeof(BitmapSource), typeof(ColorEyeDropper), new PropertyMetadata(null));
        /// <summary>Identifies the <see cref="PreviewImageOuterPixelCount"/> dependency property.</summary>
        public static readonly DependencyProperty PreviewImageOuterPixelCountProperty = DependencyProperty.Register(nameof(PreviewImageOuterPixelCount), typeof(int), typeof(ColorEyeDropper), new PropertyMetadata(2));

        /// <summary>Identifies the <see cref="EyeDropperCursor"/> dependency property.</summary>
        public static readonly DependencyProperty EyeDropperCursorProperty = DependencyProperty.Register(nameof(EyeDropperCursor), typeof(Cursor), typeof(ColorEyeDropper), new PropertyMetadata(null));

        /// <summary>
        /// Gets the preview image while the cursor is moving
        /// </summary>
        public Color SelectedColor
        {
            get { return (Color)this.GetValue(SelectedColorProperty); }
            set { this.SetValue(SelectedColorProperty, value); }
        }

        /// <summary>
        /// Gets the preview brush while the cursor is moving
        /// </summary>
        public Brush PreviewBrush
        {
            get { return (Brush)GetValue(PreviewBrushProperty); }
        }

        /// <summary>
        /// Gets the preview image while the cursor is moving
        /// </summary>
        public BitmapSource PreviewImageSource
        {
            get { return (BitmapSource)GetValue(PreviewImageSourceProperty); }
        }

        /// <summary>
        /// Gets or Sets the number of additional pixel in the preview image
        /// </summary>
        public int PreviewImageOuterPixelCount
        {
            get { return (int)GetValue(PreviewImageOuterPixelCountProperty); }
            set { SetValue(PreviewImageOuterPixelCountProperty, value); }
        }

        /// <summary>
        /// Gets or Sets the Cursor in Selecting Color Mode
        /// </summary>
        public Cursor EyeDropperCursor
        {
            get { return (Cursor)GetValue(EyeDropperCursorProperty); }
            set { SetValue(EyeDropperCursorProperty, value); }
        }


        private void ColorEyeDropper_PreviewMouseUp(object sender, MouseEventArgs e)
        {
            this.ReleaseMouseCapture();
            this.MouseMove -= this.ColorEyeDropper_PreviewMouseMove;

            this.Cursor = Cursors.Arrow;

            if (!this.IsMouseOver)
            {
                var mousePos = EyeDropperHelper.GetCursorPosition();
                this.SelectedColor = EyeDropperHelper.GetPixelColor(mousePos);
            }

            if (this.ToolTip is ToolTip toolTip)
            {
                var action = new Action (() => { toolTip.IsOpen = false; });
                Dispatcher.BeginInvoke(DispatcherPriority.Background, action);
            }
        }

        private void ColorEyeDropper_PreviewMouseDown(object sender, MouseEventArgs e)
        {
            this.PreviewMouseMove += this.ColorEyeDropper_PreviewMouseMove;
            this.Cursor = this.EyeDropperCursor;
            Mouse.Capture(this);

            if (this.ToolTip is ToolTip toolTip)
            {
                var action = new Action(() =>
                {
                    toolTip.PlacementTarget = this;
                    toolTip.Placement = System.Windows.Controls.Primitives.PlacementMode.Mouse;
                    toolTip.IsOpen = true;
                });
                Dispatcher.BeginInvoke(DispatcherPriority.Background, action);
            }

            SetPreview();
        }

        private void ColorEyeDropper_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            SetPreview();
        }

        private void SetPreview()
        {
            var action = new Action(() =>
            {
                var mousePos = EyeDropperHelper.GetCursorPosition();
                var previewImage = EyeDropperHelper.CaptureRegion(new Int32Rect(mousePos.X - PreviewImageOuterPixelCount, mousePos.Y - PreviewImageOuterPixelCount, 2 * PreviewImageOuterPixelCount + 1, 2 * PreviewImageOuterPixelCount + 1));
                var previewColor = EyeDropperHelper.GetPixelColor(mousePos);

                SetCurrentValue(PreviewImageSourceProperty, previewImage);
                SetCurrentValue(PreviewBrushProperty, new SolidColorBrush(previewColor));
            });

            Dispatcher.BeginInvoke(DispatcherPriority.Background, action);
        }

        #region Overrides
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.PreviewMouseDown += ColorEyeDropper_PreviewMouseDown;
            this.PreviewMouseUp += ColorEyeDropper_PreviewMouseUp;
        }
        #endregion

    }
}
