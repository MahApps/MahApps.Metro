using System;
using System.Collections;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace MahApps.Metro.Controls
{
    public class MetroProgressBar : ProgressBar
    {
        public static readonly DependencyProperty EllipseDiameterProperty =
            DependencyProperty.Register("EllipseDiameter", typeof (double), typeof (MetroProgressBar),
                                        new PropertyMetadata(default(double)));

        public static readonly DependencyProperty EllipseOffsetProperty =
            DependencyProperty.Register("EllipseOffset", typeof (double), typeof (MetroProgressBar),
                                        new PropertyMetadata(default(double)));

        static MetroProgressBar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof (MetroProgressBar),
                                                     new FrameworkPropertyMetadata(typeof (MetroProgressBar)));

        }

        public MetroProgressBar()
        {
            SizeChanged += SizeChangedHandler;
        }

        public double EllipseDiameter
        {
            get { return (double) GetValue(EllipseDiameterProperty); }
            set { SetValue(EllipseDiameterProperty, value); }
        }

        public double EllipseOffset
        {
            get { return (double) GetValue(EllipseOffsetProperty); }
            set { SetValue(EllipseOffsetProperty, value); }
        }

        private void SizeChangedHandler(object sender, SizeChangedEventArgs e)
        {
            double actualWidth = ActualWidth;
            MetroProgressBar bar = this;

            bar.ResetStoryboard(actualWidth);
        }

        private void ResetStoryboard(double width)
        {
            
            lock(this)
            {
                //perform calculations
                double containerAnimStart = CalcContainerAnimStart(width);
                double containerAnimEnd = CalcContainerAnimEnd(width);
                double ellipseAnimWell = CalcEllipseAnimWell(width);
                double ellipseAnimEnd = CalcEllipseAnimEnd(width);
                //reset the main double animation
                try
                {
                    VisualState indeterminate = GetIndeterminate();

                    if (indeterminate != null)
                    {
                        Storyboard newStoryboard = indeterminate.Storyboard.Clone();
                        Timeline doubleAnim = newStoryboard.Children.First(t => t.Name == "MainDoubleAnim");
                        doubleAnim.SetValue(DoubleAnimation.FromProperty, containerAnimStart);
                        //doubleAnim.InvalidateProperty(DoubleAnimation.FromProperty);
                        doubleAnim.SetValue(DoubleAnimation.ToProperty, containerAnimEnd);
                        //doubleAnim.InvalidateProperty(DoubleAnimation.ToProperty);
                        //indeterminate.Storyboard.Begin();

                        var namesOfElements = new[] {"E1", "E2", "E3", "E4", "E5"};
                        foreach (string elemName in namesOfElements)
                        {

                            var doubleAnimParent =
                                (DoubleAnimationUsingKeyFrames)
                                newStoryboard.Children.First(t => t.Name == elemName + "Anim");
                            DoubleKeyFrame first, second, third;
                            if (elemName == "E1")
                            {
                                first = doubleAnimParent.KeyFrames[1];
                                second = doubleAnimParent.KeyFrames[2];
                                third = doubleAnimParent.KeyFrames[3];
                            }
                            else
                            {
                                first = doubleAnimParent.KeyFrames[2];
                                second = doubleAnimParent.KeyFrames[3];
                                third = doubleAnimParent.KeyFrames[4];
                            }

                            first.Value = ellipseAnimWell;
                            second.Value = ellipseAnimWell;
                            third.Value = ellipseAnimEnd;
                            first.InvalidateProperty(DoubleKeyFrame.ValueProperty);
                            second.InvalidateProperty(DoubleKeyFrame.ValueProperty);
                            third.InvalidateProperty(DoubleKeyFrame.ValueProperty);



                            doubleAnimParent.InvalidateProperty(Storyboard.TargetPropertyProperty);
                            doubleAnimParent.InvalidateProperty(Storyboard.TargetNameProperty);


                        }
                        indeterminate.Storyboard.Remove();
                        indeterminate.Storyboard = newStoryboard;
                        if (IsIndeterminate)
                        {
                            indeterminate.Storyboard.Begin((FrameworkElement) GetTemplateChild("ContainingGrid"));
                        }



                    }
                }
                catch (Exception)
                {
                    //we just ignore 
                }
            }
        }

        private VisualState GetIndeterminate()
        {
            DependencyObject templateGrid = GetTemplateChild("ContainingGrid");
            IList groups = VisualStateManager.GetVisualStateGroups((FrameworkElement) templateGrid);
            return groups != null
                       ? groups.Cast<VisualStateGroup>().SelectMany(@group => @group.States.Cast<VisualState>()).
                             FirstOrDefault(state =>
                                            state.Name == "Indeterminate")
                       : null;
        }


        private void SetEllipseDiameter(double width)
        {
            if (width <= 180)
            {
                EllipseDiameter = 4;
                return;
            }
            if (width <= 280)
            {
                EllipseDiameter = 5;
                return;
            }

            EllipseDiameter = 6;
        }

        private void SetEllipseOffset(double width)
        {
            if (width <= 180)
            {
                EllipseOffset = 4;
                return;
            }
            if (width <= 280)
            {
                EllipseOffset = 7;
                return;
            }

            EllipseOffset = 9;
        }

        private double CalcContainerAnimStart(double width)
        {
            if (width <= 180)
                return -34;
            if (width <= 280)
                return -50.5;

            return -63;
        }

        private double CalcContainerAnimEnd(double width)
        {
            double firstPart = 0.4352*width;
            if (width <= 180)
                return firstPart - 25.731;
            if (width <= 280)
                return firstPart + 27.84;

            return firstPart + 58.862;
        }

        private double CalcEllipseAnimWell(double width)
        {
            return width*1.0/3.0;
        }

        private double CalcEllipseAnimEnd(double width)
        {
            return width*2.0/3.0;
        }


        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            SizeChangedHandler(null, null);
        }
        
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            // Update the Ellipse properties to their default values
            // only if they haven't been user-set.
            if (EllipseDiameter.Equals(0))
                SetEllipseDiameter(ActualWidth);
            if (EllipseOffset.Equals(0))
                SetEllipseOffset(ActualWidth);
        }
    }
}
