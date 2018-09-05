using System;
using System.Windows;
using System.Windows.Controls;

namespace MahApps.Metro.Controls
{
    [TemplatePart(Name = UnderlineBorderPartName, Type = typeof(Border))]
    public class Underline : ContentControl
    {
        public const string UnderlineBorderPartName = "PART_UnderlineBorder";
        private Border _underlineBorder;

        public static readonly DependencyProperty PlacementProperty =
            DependencyProperty.Register(nameof(Placement),
                                        typeof(Dock),
                                        typeof(Underline),
                                        new PropertyMetadata(default(Dock), (o, e) => { (o as Underline)?.ApplyBorderProperties(); }));

        public Dock Placement
        {
            get { return (Dock)this.GetValue(PlacementProperty); }
            set { this.SetValue(PlacementProperty, value); }
        }

        public static readonly DependencyProperty LineThicknessProperty =
            DependencyProperty.Register(nameof(LineThickness),
                                        typeof(double),
                                        typeof(Underline),
                                        new PropertyMetadata(1d, (o, e) => { (o as Underline)?.ApplyBorderProperties(); }));

        public double LineThickness
        {
            get { return (double)this.GetValue(LineThicknessProperty); }
            set { this.SetValue(LineThicknessProperty, value); }
        }

        public static readonly DependencyProperty LineExtentProperty =
            DependencyProperty.Register(nameof(LineExtent),
                                        typeof(double),
                                        typeof(Underline),
                                        new PropertyMetadata(Double.NaN, (o, e) => { (o as Underline)?.ApplyBorderProperties(); }));

        public double LineExtent
        {
            get { return (double)this.GetValue(LineExtentProperty); }
            set { this.SetValue(LineExtentProperty, value); }
        }

        static Underline()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Underline), new FrameworkPropertyMetadata(typeof(Underline)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this._underlineBorder = this.GetTemplateChild(UnderlineBorderPartName) as Border;

            this.ApplyBorderProperties();
        }

        private void ApplyBorderProperties()
        {
            if (this._underlineBorder == null)
            {
                return;
            }

            Action applyAction = () =>
                {
                    this._underlineBorder.Height = Double.NaN;
                    this._underlineBorder.Width = Double.NaN;
                    this._underlineBorder.BorderThickness = new Thickness();
                    switch (this.Placement)
                    {
                        case Dock.Left:
                            this._underlineBorder.Width = this.LineExtent;
                            this._underlineBorder.BorderThickness = new Thickness(this.LineThickness, 0d, 0d, 0d);
                            break;
                        case Dock.Top:
                            this._underlineBorder.Height = this.LineExtent;
                            this._underlineBorder.BorderThickness = new Thickness(0d, this.LineThickness, 0d, 0d);
                            break;
                        case Dock.Right:
                            this._underlineBorder.Width = this.LineExtent;
                            this._underlineBorder.BorderThickness = new Thickness(0d, 0d, this.LineThickness, 0d);
                            break;
                        case Dock.Bottom:
                            this._underlineBorder.Height = this.LineExtent;
                            this._underlineBorder.BorderThickness = new Thickness(0d, 0d, 0d, this.LineThickness);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    this.InvalidateVisual();
                };

            this.ExecuteWhenLoaded(applyAction);
        }
    }
}