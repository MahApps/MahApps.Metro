using MahApps.Metro.Controls.ColorPicker;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MahApps.Metro.Controls
{
    public class ColorEyeDropper : ContentControl
    {
        // Depency Properties
        public static readonly DependencyProperty SelectedColorProperty = DependencyProperty.Register("SelectedColor", typeof(Color), typeof(ColorEyeDropper), new FrameworkPropertyMetadata(Colors.Black, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty PreviewBrushProperty = DependencyProperty.Register("PreviewBrush", typeof(Brush), typeof(ColorEyeDropper), new PropertyMetadata(Brushes.Transparent));

        public static readonly DependencyProperty PreviewImageSourceProperty = DependencyProperty.Register("PreviewImageSource", typeof(BitmapSource), typeof(ColorEyeDropper), new PropertyMetadata(null));
        public static readonly DependencyProperty PreviewImageOuterPixelCountProperty = DependencyProperty.Register("PreviewImageOuterPixelCount", typeof(int), typeof(ColorEyeDropper), new PropertyMetadata(2));

        public Color SelectedColor
        {
            get { return (Color)this.GetValue(SelectedColorProperty); }
            set { this.SetValue(SelectedColorProperty, value); }
        }

        public Brush PreviewColor
        {
            get { return (Brush)GetValue(PreviewBrushProperty); }
        }

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
                toolTip.IsOpen = false;
            }
        }

        private void ColorEyeDropper_PreviewMouseDown(object sender, MouseEventArgs e)
        {
            this.PreviewMouseMove += this.ColorEyeDropper_PreviewMouseMove;
            this.Cursor = (Cursor)this.Resources["MahApps.Cursors.EyeDropper"];
            Mouse.Capture(this);

            if (this.ToolTip is ToolTip toolTip)
            {
                toolTip.IsOpen = true;
                toolTip.StaysOpen = true;
            }

            SetPreview();
            
        }

        private void ColorEyeDropper_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            SetPreviewAsync();
        }

        private async void SetPreviewAsync()
        {
            var mousePos = await Task.FromResult(EyeDropperHelper.GetCursorPosition());
            var previewImage = await Task.FromResult(EyeDropperHelper.CaptureRegion(new Int32Rect(mousePos.X - PreviewImageOuterPixelCount, mousePos.Y - PreviewImageOuterPixelCount, 2 * PreviewImageOuterPixelCount + 1, 2 * PreviewImageOuterPixelCount + 1)));
            var previewColor = await Task.FromResult(EyeDropperHelper.GetPixelColor(mousePos));

            SetCurrentValue(PreviewImageSourceProperty, previewImage);
            SetCurrentValue(PreviewBrushProperty, new SolidColorBrush(previewColor));
        }

        private void SetPreview()
        {
            var mousePos = EyeDropperHelper.GetCursorPosition();
            var previewImage = EyeDropperHelper.CaptureRegion(new Int32Rect(mousePos.X - PreviewImageOuterPixelCount, mousePos.Y - PreviewImageOuterPixelCount, 2 * PreviewImageOuterPixelCount + 1, 2 * PreviewImageOuterPixelCount + 1));
            var previewColor = EyeDropperHelper.GetPixelColor(mousePos);

            SetCurrentValue(PreviewImageSourceProperty, previewImage);
            SetCurrentValue(PreviewBrushProperty, new SolidColorBrush(previewColor));
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
