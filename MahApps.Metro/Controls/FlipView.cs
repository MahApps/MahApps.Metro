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
    [TemplatePart(Name = "PART_BannerLabel", Type = typeof(Label))]
    public class FlipView : Selector
    {
        private const string PART_Presenter = "PART_Presenter";
        private const string PART_BackButton = "PART_BackButton";
        private const string PART_ForwardButton = "PART_ForwardButton";
        private const string PART_BannerGrid = "PART_BannerGrid";
        private const string PART_BannerLabel = "PART_BannerLabel";

        private TransitioningContentControl presenter = null;
        private Button backButton = null;
        private Button forwardButton = null;
        private Grid bannerGrid = null;
        private Label bannerLabel = null;

        private Storyboard ShowBannerStoryboard = null;
        private Storyboard HideBannerStoryboard = null;
        private Storyboard HideControlStoryboard = null;
        private Storyboard ShowControlStoryboard = null;

        private EventHandler HideControlStoryboard_CompletedHandler = null;
        /// <summary>
        /// To counteract the double Loaded event issue.
        /// </summary>
        private bool loaded = false;

        static FlipView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FlipView), new FrameworkPropertyMetadata(typeof(FlipView)));
        }
        public FlipView()
        {
            this.Unloaded += FlipView_Unloaded;
            this.Loaded += FlipView_Loaded;
        }
        ~FlipView()
        {
            Dispatcher.Invoke(new EmptyDelegate(() =>
            {
                this.Loaded -= FlipView_Loaded;
            }));
        }

        private delegate void EmptyDelegate();

        void FlipView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DetectControlButtonsStatus();
        }

        private void DetectControlButtonsStatus()
        {
            if (Items.Count > 0)
            {
                if (SelectedIndex == 0)
                {
                    backButton.Visibility = System.Windows.Visibility.Hidden;
                    forwardButton.Visibility = System.Windows.Visibility.Visible;
                }
                else
                    if (SelectedIndex == Items.Count - 1)
                    {
                        backButton.Visibility = System.Windows.Visibility.Visible;
                        forwardButton.Visibility = System.Windows.Visibility.Hidden;
                    }
                    else
                    {
                        backButton.Visibility = System.Windows.Visibility.Visible;
                        forwardButton.Visibility = System.Windows.Visibility.Visible;
                    }
            }
            else
            {
                backButton.Visibility = System.Windows.Visibility.Hidden;
                forwardButton.Visibility = System.Windows.Visibility.Hidden;
            }
        }
        void FlipView_Loaded(object sender, RoutedEventArgs e)
        {
            /* Loaded event fires twice if its a child of a TabControl.
             * Once because the TabControl seems to initiali(z|s)e everything.
             * And a second time when the Tab (housing the FlipView) is switched to. */

            if (backButton == null) //OnApplyTemplate hasn't been called yet.
                ApplyTemplate();

            if (loaded) return; //Counteracts the double 'Loaded' event issue.

            backButton.Click += backButton_Click;
            forwardButton.Click += forwardButton_Click;

            this.SelectionChanged += FlipView_SelectionChanged;
            this.KeyDown += FlipView_KeyDown;

            ShowBannerStoryboard = ((Storyboard)this.Template.Resources["ShowBannerStoryboard"]).Clone();
            HideBannerStoryboard = ((Storyboard)this.Template.Resources["HideBannerStoryboard"]).Clone();

            ShowControlStoryboard = ((Storyboard)this.Template.Resources["ShowControlStoryboard"]).Clone();
            HideControlStoryboard = ((Storyboard)this.Template.Resources["HideControlStoryboard"]).Clone();

            SelectedIndex = 0;

            DetectControlButtonsStatus();

            ShowBanner();

            loaded = true;
        }
        void FlipView_Unloaded(object sender, RoutedEventArgs e)
        {
            this.Unloaded -= FlipView_Unloaded;
            this.SelectionChanged -= FlipView_SelectionChanged;

            this.KeyDown -= FlipView_KeyDown;
            backButton.Click -= backButton_Click;
            forwardButton.Click -= forwardButton_Click;

            if (HideControlStoryboard_CompletedHandler != null)
                HideControlStoryboard.Completed -= HideControlStoryboard_CompletedHandler;

            loaded = false;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            presenter = GetTemplateChild(PART_Presenter) as TransitioningContentControl;
            backButton = GetTemplateChild(PART_BackButton) as Button;
            forwardButton = GetTemplateChild(PART_ForwardButton) as Button;
            bannerGrid = GetTemplateChild(PART_BannerGrid) as Grid;
            bannerLabel = GetTemplateChild(PART_BannerLabel) as Label;
        }

        void FlipView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left)
            {
                GoBack();
                e.Handled = true;
            }
            else if (e.Key == Key.Right)
            {
                GoForward();
                e.Handled = true;
            }

            if (e.Handled)
                this.Focus();
        }

        protected override void OnItemsSourceChanged(System.Collections.IEnumerable oldValue, System.Collections.IEnumerable newValue)
        {
            base.OnItemsSourceChanged(oldValue, newValue);

            SelectedIndex = 0;
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
                SelectedIndex--;
            }
        }

        public void GoForward()
        {
            if (SelectedIndex < Items.Count - 1)
            {
                presenter.Transition = "LeftReplaceTransition";
                SelectedIndex++;
            }
        }

        public void ShowControlButtons()
        {
            ExecuteWhenLoaded(this, () =>
                {
                    backButton.Visibility = System.Windows.Visibility.Visible;
                    forwardButton.Visibility = System.Windows.Visibility.Visible;
                });
        }
        public void HideControlButtons()
        {
            ExecuteWhenLoaded(this, () =>
                {
                    backButton.Visibility = System.Windows.Visibility.Hidden;
                    forwardButton.Visibility = System.Windows.Visibility.Hidden;
                });
        }

        private void ShowBanner()
        {
            if (IsBannerEnabled)
                bannerGrid.BeginStoryboard(ShowBannerStoryboard);
            bannerLabel.Content = BannerText;
        }

        private void HideBanner()
        {
            if (this.Height > 0.0)
                bannerGrid.BeginStoryboard(HideBannerStoryboard);
        }

        public static readonly DependencyProperty BannerTextProperty =
            DependencyProperty.Register("BannerText", typeof(string), typeof(FlipView), new PropertyMetadata("Banner", new PropertyChangedCallback((d, e) =>
            {
                if (e.OldValue != e.NewValue)
                    ExecuteWhenLoaded(((FlipView)d), () =>
                        ((FlipView)d).ChangeBannerText());
            })));

        public string BannerText
        {
            get { return (string)GetValue(BannerTextProperty); }
            set
            {
                ExecuteWhenLoaded(this, () =>
                {
                    if (IsBannerEnabled)
                        ChangeBannerText(value);
                    else
                        SetValue(BannerTextProperty, value);
                });

            }
        }

        private void ChangeBannerText(string value = null)
        {
            if (IsBannerEnabled)
            {
                var newValue = value != null ? value : BannerText;

                if (newValue == (string)GetValue(BannerTextProperty)) return;

                if (HideControlStoryboard_CompletedHandler != null)
                    HideControlStoryboard.Completed -= HideControlStoryboard_CompletedHandler;

                HideControlStoryboard_CompletedHandler = new EventHandler((sender, e) =>
                {
                    SetValue(BannerTextProperty, newValue);

                    HideControlStoryboard.Completed -= HideControlStoryboard_CompletedHandler;

                    bannerLabel.Content = value != null ? value : BannerText;

                    bannerLabel.BeginStoryboard(ShowControlStoryboard, HandoffBehavior.SnapshotAndReplace);
                });


                HideControlStoryboard.Completed += HideControlStoryboard_CompletedHandler;

                bannerLabel.BeginStoryboard(HideControlStoryboard, HandoffBehavior.SnapshotAndReplace);
            }
            else
                ExecuteWhenLoaded(this, () =>
                {
                    bannerLabel.Content = value != null ? value : BannerText;
                });
        }

        public static readonly DependencyProperty IsBannerEnabledProperty =
            DependencyProperty.Register("IsBannerEnabled", typeof(bool), typeof(FlipView), new UIPropertyMetadata(true, new PropertyChangedCallback((d, e) =>
            {
                var flipview = ((FlipView)d);

                if (!flipview.IsLoaded)
                {
                    //wait to be loaded?
                    ExecuteWhenLoaded(flipview, () =>
                    {
                        flipview.ApplyTemplate();

                        if ((bool)e.NewValue)
                        {
                            flipview.ChangeBannerText(flipview.BannerText);
                            flipview.ShowBanner();
                        }
                        else
                            flipview.HideBanner();
                    });
                }
                else
                {
                    if ((bool)e.NewValue)
                    {
                        flipview.ChangeBannerText(flipview.BannerText);
                        flipview.ShowBanner();
                    }
                    else
                        flipview.HideBanner();
                }
            })));

        public bool IsBannerEnabled
        {
            get { return (bool)GetValue(IsBannerEnabledProperty); }
            set
            {
                SetValue(IsBannerEnabledProperty, value);
            }
        }

        private static void ExecuteWhenLoaded(FlipView flipview, Action body)
        {
            if (flipview.IsLoaded)
                body();
            else
            {
                RoutedEventHandler handler = null;
                handler = new RoutedEventHandler((o, a) =>
                {
                    flipview.Loaded -= handler;
                    body();
                });

                flipview.Loaded += handler;
            }
        }
    }
}
