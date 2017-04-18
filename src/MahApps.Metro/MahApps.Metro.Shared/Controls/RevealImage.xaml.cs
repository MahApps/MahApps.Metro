using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace MahApps.Metro.Controls
{
    public partial class RevealImage
    {
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(RevealImage), new UIPropertyMetadata(""));
        public static readonly DependencyProperty ImageProperty = DependencyProperty.Register("Image", typeof(ImageSource), typeof(RevealImage), new UIPropertyMetadata(null));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public ImageSource Image
        {
            get { return (ImageSource)GetValue(ImageProperty); }
            set { SetValue(ImageProperty, value); }
        }

        

        public RevealImage()
        {
            InitializeComponent();
        }

        private static void TypewriteTextblock(string textToAnimate, TextBlock txt, TimeSpan timeSpan)
        {
            var story = new Storyboard
            {
                FillBehavior = FillBehavior.HoldEnd
            };

            DiscreteStringKeyFrame discreteStringKeyFrame;
            var stringAnimationUsingKeyFrames = new StringAnimationUsingKeyFrames
            {
                Duration = new Duration(timeSpan)
            };

            var tmp = string.Empty;
            foreach (var c in textToAnimate)
            {
                discreteStringKeyFrame = new DiscreteStringKeyFrame
                {
                    KeyTime = KeyTime.Paced
                };
                tmp += c;
                discreteStringKeyFrame.Value = tmp;
                stringAnimationUsingKeyFrames.KeyFrames.Add(discreteStringKeyFrame);
            }

            Storyboard.SetTargetName(stringAnimationUsingKeyFrames, txt.Name);
            Storyboard.SetTargetProperty(stringAnimationUsingKeyFrames, new PropertyPath(TextBlock.TextProperty));
            story.Children.Add(stringAnimationUsingKeyFrames);
            story.Begin(txt);
        }

        private void GridMouseEnter(object sender, MouseEventArgs e)
        {

            TypewriteTextblock(Text.ToUpper(), textBlock, TimeSpan.FromSeconds(.25));
        }
    }
}
