using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace MahApps.Metro.Controls
{
    [TemplatePart(Name = "PART_Scroll", Type = typeof(ScrollViewer))]
    [TemplatePart(Name = "PART_Mediator", Type = typeof(ScrollViewerOffsetMediator))]
    public class Pivot : ItemsControl
    {
        private ScrollViewer scroller;
        private ScrollViewerOffsetMediator mediator;

        public static readonly RoutedEvent SelectionChangedEvent = EventManager.RegisterRoutedEvent("SelectionChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Pivot));
        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register("Header", typeof(string), typeof(Pivot), new PropertyMetadata(default(string)));

        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        public event RoutedEventHandler SelectionChanged
        {
            add { AddHandler(SelectionChangedEvent, value); }
            remove { RemoveHandler(SelectionChangedEvent, value); }
        }

        public void GoToItem(PivotItem item)
        {
            var widthToScroll = 0.0;
            for (int i = 0; i < Items.Count; i++)
            {
                if (Items[i] == item)
                    break;

                widthToScroll += ((PivotItem)Items[i]).ActualWidth;
            }

            var sb = new Storyboard();
            var doubleAnimation = new DoubleAnimationUsingKeyFrames();
            doubleAnimation.KeyFrames = new DoubleKeyFrameCollection { new EasingDoubleKeyFrame(widthToScroll, KeyTime.FromTimeSpan(new TimeSpan(0, 0, 0, 1)), new CubicEase { EasingMode = EasingMode.EaseOut }) };
            Storyboard.SetTargetName(doubleAnimation, "PART_Mediator");
            Storyboard.SetTargetProperty(doubleAnimation, new PropertyPath("HorizontalOffset"));
            sb.Children.Add(doubleAnimation);
            sb.Begin(mediator);

            RaiseEvent(new RoutedEventArgs(SelectionChangedEvent));
        }

        static Pivot()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Pivot), new FrameworkPropertyMetadata(typeof(Pivot)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            scroller = (ScrollViewer)GetTemplateChild("PART_Scroll");
            mediator = (ScrollViewerOffsetMediator)GetTemplateChild("PART_Mediator");
        }
    }
}
