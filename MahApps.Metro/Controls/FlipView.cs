using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MahApps.Metro.Controls
{
    [TemplatePart(Name = "PART_Presenter", Type = typeof(TransitioningContentControl))]
    [TemplatePart(Name = "PART_BackButton", Type = typeof(Button))]
    [TemplatePart(Name = "PART_ForwardButton", Type = typeof(Button))]
    [TemplatePart(Name = "PART_BannerGrid", Type = typeof(Grid))]
    public class FlipView : Selector
    {
        private const string PART_Presenter = "PART_Presenter";
        private const string PART_BackButton = "PART_BackButton";
        private const string PART_ForwardButton = "PART_ForwardButton";
        private const string PART_BannerGrid = "PART_BannerGrid";

        private TransitioningContentControl presenter = null;
        private Button backButton = null;
        private Button forwardButton = null;
        private Grid bannerGrid = null;

        private Storyboard ShowBannerStoryboard = null;
        private Storyboard HideBannerStoryboard = null;

        static FlipView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FlipView), new FrameworkPropertyMetadata(typeof(FlipView)));
        }
        public FlipView()
        {
            this.Unloaded += FlipView_Unloaded;
        }

        void FlipView_Unloaded(object sender, RoutedEventArgs e)
        {
            this.Unloaded -= FlipView_Unloaded;

            backButton.Click -= backButton_Click;
            forwardButton.Click -= forwardButton_Click;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            presenter = this.Template.FindName(PART_Presenter, this) as TransitioningContentControl;
            backButton = this.Template.FindName(PART_BackButton, this) as Button;
            forwardButton = this.Template.FindName(PART_ForwardButton, this) as Button;
            bannerGrid = this.Template.FindName(PART_BannerGrid, this) as Grid;

            backButton.Click += backButton_Click;
            forwardButton.Click += forwardButton_Click;

            ShowBannerStoryboard = (Storyboard)this.Template.Resources["ShowBannerStoryboard"];
            HideBannerStoryboard = (Storyboard)this.Template.Resources["HideBannerStoryboard"];

            ShowBanner();
        }

        void forwardButton_Click(object sender, RoutedEventArgs e)
        {
            GoForward();
        }

        void backButton_Click(object sender, RoutedEventArgs e)
        {
            GoBack();
        }

        public void GoBack()
        {
            if (SelectedIndex > 0)
            {
                presenter.Transition = "RightReplaceTransition";

                HideBanner();

                SelectedIndex--;

                ShowBanner();
            }
        }

        private void ShowBanner()
        {
            if (IsBannerEnabled) bannerGrid.BeginStoryboard(ShowBannerStoryboard);
        }

        private void HideBanner()
        {
            if (IsBannerEnabled) bannerGrid.BeginStoryboard(HideBannerStoryboard);
        }
        public void GoForward()
        {
            if (SelectedIndex < Items.Count - 1)
            {
                presenter.Transition = "LeftReplaceTransition";

                HideBanner();

                SelectedIndex++;

                ShowBanner();
            }
        }

        public static readonly DependencyProperty BannerTextProperty =
            DependencyProperty.Register("BannerText", typeof(string), typeof(FlipView), new UIPropertyMetadata("Banner"));

        public string BannerText
        {
            get { return (string)GetValue(BannerTextProperty); }
            set { SetValue(BannerTextProperty, value); }
        }

        public static readonly DependencyProperty IsBannerEnabledProperty =
            DependencyProperty.Register("IsBannerEnabled", typeof(bool), typeof(FlipView), new UIPropertyMetadata(false));

        public bool IsBannerEnabled
        {
            get { return (bool)GetValue(IsBannerEnabledProperty); }
            set
            {
                SetValue(IsBannerEnabledProperty, value);

                if (value)
                    ShowBanner();
                else
                    HideBanner();
            }
        }
    }
}
