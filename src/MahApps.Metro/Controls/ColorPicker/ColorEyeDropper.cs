// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace MahApps.Metro.Controls
{
    public class ColorEyeDropper : Button
    {
        private DispatcherOperation currentTask;
        internal ColorEyePreviewData previewData = new ColorEyePreviewData();
        private ToolTip previewToolTip;

        static ColorEyeDropper()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ColorEyeDropper), new FrameworkPropertyMetadata(typeof(ColorEyeDropper)));
        }

        /// <summary>Identifies the <see cref="SelectedColor"/> dependency property.</summary>
        public static readonly DependencyProperty SelectedColorProperty
            = DependencyProperty.Register(nameof(SelectedColor),
                                          typeof(Color?),
                                          typeof(ColorEyeDropper),
                                          new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedColorPropertyChanged));

        /// <summary>
        /// Gets or sets the selected <see cref="Color"/>.
        /// </summary>
        public Color? SelectedColor
        {
            get => (Color?)this.GetValue(SelectedColorProperty);
            set => this.SetValue(SelectedColorProperty, value);
        }

        /// <summary>Identifies the <see cref="PreviewImageOuterPixelCount"/> dependency property.</summary>
        public static readonly DependencyProperty PreviewImageOuterPixelCountProperty
            = DependencyProperty.Register(nameof(PreviewImageOuterPixelCount),
                                          typeof(int),
                                          typeof(ColorEyeDropper),
                                          new PropertyMetadata(2));

        private static void OnSelectedColorPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (dependencyObject is ColorEyeDropper eyeDropper)
            {
                eyeDropper.RaiseEvent(new RoutedPropertyChangedEventArgs<Color?>((Color?)e.OldValue, (Color?)e.NewValue, SelectedColorChangedEvent));
            }
        }

        /// <summary>
        /// Gets or sets the number of additional pixel in the preview image.
        /// </summary>
        public int PreviewImageOuterPixelCount
        {
            get => (int)this.GetValue(PreviewImageOuterPixelCountProperty);
            set => this.SetValue(PreviewImageOuterPixelCountProperty, value);
        }

        /// <summary>Identifies the <see cref="EyeDropperCursor"/> dependency property.</summary>
        public static readonly DependencyProperty EyeDropperCursorProperty
            = DependencyProperty.Register(nameof(EyeDropperCursor),
                                          typeof(Cursor),
                                          typeof(ColorEyeDropper),
                                          new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the Cursor for Selecting Color Mode
        /// </summary>
        public Cursor EyeDropperCursor
        {
            get => (Cursor)this.GetValue(EyeDropperCursorProperty);
            set => this.SetValue(EyeDropperCursorProperty, value);
        }

        /// <summary>Identifies the <see cref="PreviewContentTemplate"/> dependency property.</summary>
        public static readonly DependencyProperty PreviewContentTemplateProperty
            = DependencyProperty.Register(nameof(PreviewContentTemplate),
                                          typeof(DataTemplate),
                                          typeof(ColorEyeDropper),
                                          new PropertyMetadata(default(DataTemplate)));

        /// <summary>
        /// Gets or sets the ContentControl.ContentTemplate for the preview.
        /// </summary>
        public DataTemplate PreviewContentTemplate
        {
            get => (DataTemplate)this.GetValue(PreviewContentTemplateProperty);
            set => this.SetValue(PreviewContentTemplateProperty, value);
        }

        private void SetPreview(Point mousePos)
        {
            this.previewToolTip?.Move(mousePos, new Point(16, 16));

            if (this.currentTask?.Status == DispatcherOperationStatus.Executing || this.currentTask?.Status == DispatcherOperationStatus.Pending)
            {
                this.currentTask.Abort();
            }

            var action = new Action(() =>
                {
                    mousePos = this.PointToScreen(mousePos);
                    var outerPixelCount = this.PreviewImageOuterPixelCount;
                    var posX = (int)Math.Round(mousePos.X - outerPixelCount);
                    var posY = (int)Math.Round(mousePos.Y - outerPixelCount);
                    var region = new Int32Rect(posX, posY, 2 * outerPixelCount + 1, 2 * outerPixelCount + 1);
                    var previewImage = EyeDropperHelper.CaptureRegion(region);
                    var previewBrush = new SolidColorBrush(EyeDropperHelper.GetPixelColor(mousePos));
                    previewBrush.Freeze();

                    this.previewData.SetValue(ColorEyePreviewData.PreviewImagePropertyKey, previewImage);
                    this.previewData.SetValue(ColorEyePreviewData.PreviewBrushPropertyKey, previewBrush);
                });

            this.currentTask = this.Dispatcher.BeginInvoke(DispatcherPriority.Background, action);
        }

        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseLeftButtonDown(e);

            Mouse.Capture(this);

            if (this.previewToolTip is null)
            {
                this.previewToolTip = ColorEyePreview.GetPreviewToolTip(this);
            }

            this.previewToolTip.Show();

            this.Cursor = this.EyeDropperCursor;

            this.SetPreview(e.GetPosition(this));
        }

        protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseLeftButtonUp(e);

            Mouse.Capture(null);

            this.previewToolTip?.Hide();

            this.Cursor = Cursors.Arrow;

            this.SetCurrentValue(SelectedColorProperty, EyeDropperHelper.GetPixelColor(this.PointToScreen(e.GetPosition(this))));
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.SetPreview(e.GetPosition(this));
            }
        }

        /// <summary>Identifies the <see cref="SelectedColorChanged"/> routed event.</summary>
        public static readonly RoutedEvent SelectedColorChangedEvent = EventManager.RegisterRoutedEvent(
            nameof(SelectedColorChanged),
            RoutingStrategy.Bubble,
            typeof(RoutedPropertyChangedEventHandler<Color?>),
            typeof(ColorEyeDropper));

        /// <summary>
        ///     Occurs when the <see cref="SelectedColor" /> property is changed.
        /// </summary>
        public event RoutedPropertyChangedEventHandler<Color?> SelectedColorChanged
        {
            add => this.AddHandler(SelectedColorChangedEvent, value);
            remove => this.RemoveHandler(SelectedColorChangedEvent, value);
        }
    }
}