// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace MahApps.Metro.Controls
{
    [TemplatePart(Name = "PART_Text", Type = typeof(TextBlock))]
    public class RevealImage : Control
    {
        private TextBlock? textBlock;

        /// <summary>Identifies the <see cref="Text"/> dependency property.</summary>
        public static readonly DependencyProperty TextProperty
            = DependencyProperty.Register(nameof(Text),
                                          typeof(string),
                                          typeof(RevealImage),
                                          new UIPropertyMetadata(string.Empty));

        public string Text
        {
            get => (string)this.GetValue(TextProperty);
            set => this.SetValue(TextProperty, value);
        }

        /// <summary>Identifies the <see cref="Image"/> dependency property.</summary>
        public static readonly DependencyProperty ImageProperty
            = DependencyProperty.Register(nameof(Image),
                                          typeof(ImageSource),
                                          typeof(RevealImage),
                                          new UIPropertyMetadata(null));

        public ImageSource? Image
        {
            get => (ImageSource?)this.GetValue(ImageProperty);
            set => this.SetValue(ImageProperty, value);
        }

        static RevealImage()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RevealImage), new FrameworkPropertyMetadata(typeof(RevealImage)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.textBlock = this.GetTemplateChild("PART_Text") as TextBlock;
        }

        private static void AnimateText(string textToAnimate, TextBlock? txt, TimeSpan timeSpan)
        {
            if (txt is null)
            {
                return;
            }

            var story = new Storyboard { FillBehavior = FillBehavior.HoldEnd };
            var stringAnimationUsingKeyFrames = new StringAnimationUsingKeyFrames { Duration = new Duration(timeSpan) };

            var tmp = string.Empty;
            foreach (var c in textToAnimate)
            {
                var stringKeyFrame = new DiscreteStringKeyFrame
                                     {
                                         KeyTime = KeyTime.Paced
                                     };
                tmp += c;
                stringKeyFrame.Value = tmp;
                stringAnimationUsingKeyFrames.KeyFrames.Add(stringKeyFrame);
            }

            Storyboard.SetTargetName(stringAnimationUsingKeyFrames, txt.Name);
            Storyboard.SetTargetProperty(stringAnimationUsingKeyFrames, new PropertyPath(TextBlock.TextProperty));
            story.Children.Add(stringAnimationUsingKeyFrames);
            story.Begin(txt);
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);

            AnimateText(this.Text.ToUpper(), this.textBlock, TimeSpan.FromSeconds(.25));
        }
    }
}