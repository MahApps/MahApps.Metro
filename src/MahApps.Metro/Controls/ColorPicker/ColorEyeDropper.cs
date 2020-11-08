// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace MahApps.Metro.Controls
{
    [TemplatePart (Name = nameof(PART_PreviewToolTip), Type = typeof(ToolTip))]
    [TemplatePart (Name = nameof(PART_PreviewImage), Type = typeof(Image))]
    public class ColorEyeDropper : ContentControl
    {
        static ColorEyeDropper()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ColorEyeDropper), new FrameworkPropertyMetadata(typeof(ColorEyeDropper)));
        }

        private ToolTip PART_PreviewToolTip;
        private Image PART_PreviewImage;
        private DispatcherOperation currentTask;

        // Depency Properties
        /// <summary>Identifies the <see cref="SelectedColor"/> dependency property.</summary>
        public static readonly DependencyProperty SelectedColorProperty = DependencyProperty.Register(nameof(SelectedColor), typeof(Color?), typeof(ColorEyeDropper), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        /// <summary>Identifies the <see cref="PreviewImageOuterPixelCount"/> dependency property.</summary>
        public static readonly DependencyProperty PreviewImageOuterPixelCountProperty = DependencyProperty.Register(nameof(PreviewImageOuterPixelCount), typeof(int), typeof(ColorEyeDropper), new PropertyMetadata(2));

        /// <summary>Identifies the <see cref="EyeDropperCursor"/> dependency property.</summary>
        public static readonly DependencyProperty EyeDropperCursorProperty = DependencyProperty.Register(nameof(EyeDropperCursor), typeof(Cursor), typeof(ColorEyeDropper), new PropertyMetadata(null));
        
        /// <summary>Identifies the <see cref="PreviewBrush"/> dependency property.</summary>
        public static readonly DependencyProperty PreviewBrushProperty = DependencyProperty.Register(nameof(PreviewBrush), typeof(Brush), typeof(ColorEyeDropper), new PropertyMetadata(Brushes.Transparent));

        /// <summary>
        /// Gets the preview image while the cursor is moving
        /// </summary>
        public Color? SelectedColor
        {
            get { return (Color?)this.GetValue(SelectedColorProperty); }
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

        private void SetPreview()
        {
            if (currentTask?.Status == DispatcherOperationStatus.Executing || currentTask?.Status == DispatcherOperationStatus.Pending)
            {
                currentTask.Abort();
            }

            var action = new Action(() =>
            {
                var mousePos = EyeDropperHelper.GetCursorPosition();
                var previewImage = EyeDropperHelper.CaptureRegion(new Int32Rect(mousePos.X - PreviewImageOuterPixelCount, mousePos.Y - PreviewImageOuterPixelCount, 2 * PreviewImageOuterPixelCount + 1, 2 * PreviewImageOuterPixelCount + 1));
                var previewBrush = new SolidColorBrush(EyeDropperHelper.GetPixelColor(mousePos));
                previewBrush.Freeze();

                PART_PreviewImage.Source = previewImage;
                SetCurrentValue(PreviewBrushProperty, previewBrush);
            });

            currentTask = Dispatcher.BeginInvoke(DispatcherPriority.Background, action);
        }

        #region Overrides
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.PART_PreviewToolTip = GetTemplateChild(nameof(PART_PreviewToolTip)) as ToolTip;
            this.PART_PreviewImage = GetTemplateChild(nameof(PART_PreviewImage)) as Image;
        }

        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseLeftButtonDown(e);

            Mouse.Capture(this);

            if (!(PART_PreviewToolTip is null))
            {
                PART_PreviewToolTip.Visibility = Visibility.Visible;
                PART_PreviewToolTip.IsOpen = true;
            }

            this.Cursor = this.EyeDropperCursor;

            SetPreview();
        }

        protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseLeftButtonUp(e);
     
            Mouse.Capture(null);

            if (!(PART_PreviewToolTip is null))
            {
                PART_PreviewToolTip.IsOpen = false;
                PART_PreviewToolTip.Visibility = Visibility.Collapsed;
            }

            this.Cursor = Cursors.Arrow;

            if (!this.IsMouseOver)
            {
                var mousePos = EyeDropperHelper.GetCursorPosition();
                this.SelectedColor = EyeDropperHelper.GetPixelColor(mousePos);
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                PART_PreviewToolTip.Placement = PlacementMode.Relative;
                PART_PreviewToolTip.HorizontalOffset = e.GetPosition(PART_PreviewToolTip.PlacementTarget).X + 16;
                PART_PreviewToolTip.VerticalOffset = e.GetPosition(PART_PreviewToolTip.PlacementTarget).Y + 16;

                SetPreview();
            }
        }
        #endregion

    }
}
