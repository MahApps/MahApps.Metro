// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using MetroDemo.Models;

namespace MetroDemo.ExampleViews
{
    /// <summary>
    /// Interaction logic for TransitionExamples.xaml
    /// </summary>
    public partial class TransitionExamples : UserControl
    {
        private readonly DispatcherTimer timer;
        private int contentNumber;

        /// <summary>Identifies the <see cref="TickContent"/> dependency property.</summary>
        public static readonly DependencyProperty TickContentProperty
            = DependencyProperty.Register(nameof(TickContent),
                                          typeof(object),
                                          typeof(TransitionExamples),
                                          new PropertyMetadata(null));

        /// <summary>
        /// Gets the content which is replaced every two seconds.
        /// </summary>
        public object? TickContent
        {
            get => this.GetValue(TickContentProperty);
            private set => this.SetValue(TickContentProperty, value);
        }

        /// <summary>Identifies the <see cref="PlaygroundContent"/> dependency property.</summary>
        public static readonly DependencyProperty PlaygroundContentProperty
            = DependencyProperty.Register(nameof(PlaygroundContent),
                                          typeof(object),
                                          typeof(TransitionExamples),
                                          new PropertyMetadata(null));

        /// <summary>
        /// Gets the content which is replaced by the Change content button.
        /// </summary>
        public object? PlaygroundContent
        {
            get => this.GetValue(PlaygroundContentProperty);
            private set => this.SetValue(PlaygroundContentProperty, value);
        }

        /// <summary>Identifies the <see cref="CompletedCount"/> dependency property.</summary>
        public static readonly DependencyProperty CompletedCountProperty
            = DependencyProperty.Register(nameof(CompletedCount),
                                          typeof(int),
                                          typeof(TransitionExamples),
                                          new PropertyMetadata(0));

        /// <summary>
        /// Gets how often the playground control has raised its TransitionCompleted event.
        /// </summary>
        public int CompletedCount
        {
            get => (int)this.GetValue(CompletedCountProperty);
            private set => this.SetValue(CompletedCountProperty, value);
        }

        public TransitionExamples()
        {
            this.InitializeComponent();

            this.TickContent = this.NextContent();
            this.PlaygroundContent = this.NextContent();


            this.timer = new DispatcherTimer(TimeSpan.FromSeconds(2), DispatcherPriority.Normal, this.OnTick, this.Dispatcher);
            this.timer.Stop();

            this.IsVisibleChanged += this.OnIsVisibleChanged;
        }

        private TransitionContent NextContent()
        {
            return new TransitionContent(++this.contentNumber);
        }

        private void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                this.timer.Start();
            }
            else
            {
                this.timer.Stop();
            }
        }

        private void OnTick(object? sender, EventArgs e)
        {
            this.TickContent = this.NextContent();
        }

        private void OnChangeContentClick(object sender, RoutedEventArgs e)
        {
            this.PlaygroundContent = this.NextContent();
        }

        private void OnReloadTransitionClick(object sender, RoutedEventArgs e)
        {
            this.PlaygroundTransition.ReloadTransition();
        }

        private void OnAbortTransitionClick(object sender, RoutedEventArgs e)
        {
            this.PlaygroundTransition.AbortTransition();
        }

        private void OnPlaygroundTransitionCompleted(object sender, RoutedEventArgs e)
        {
            this.CompletedCount++;
        }

    }
}
