// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Navigation;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// A reimplementation of NavigationWindow based on MetroWindow.
    /// </summary>
    /// <see cref="System.Windows.Navigation.NavigationWindow"/>
    [ContentProperty(nameof(OverlayContent))]
    public partial class MetroNavigationWindow : MetroWindow, IUriContext
    {
        /// <summary>Identifies the <see cref="OverlayContent"/> dependency property.</summary>
        public static readonly DependencyProperty OverlayContentProperty
            = DependencyProperty.Register(nameof(OverlayContent),
                                          typeof(object),
                                          typeof(MetroNavigationWindow));

        /// <summary>
        /// Gets or sets an overlay content.
        /// </summary>
        public object? OverlayContent
        {
            get => this.GetValue(OverlayContentProperty);
            set => this.SetValue(OverlayContentProperty, value);
        }

        /// <summary>Identifies the <see cref="PageContent"/> dependency property.</summary>
        public static readonly DependencyProperty PageContentProperty
            = DependencyProperty.Register(nameof(PageContent),
                                          typeof(object),
                                          typeof(MetroNavigationWindow));

        /// <summary>
        /// Gets the content of the selected frame.
        /// </summary>
        public object? PageContent
        {
            get => this.GetValue(PageContentProperty);
            private set => this.SetValue(PageContentProperty, value);
        }

        public MetroNavigationWindow()
        {
            this.InitializeComponent();

            this.Loaded += this.MetroNavigationWindow_Loaded;
            this.Closing += this.MetroNavigationWindow_Closing;
        }

        private void MetroNavigationWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.PART_Frame.Navigated += this.PART_Frame_Navigated;
            this.PART_Frame.Navigating += this.PART_Frame_Navigating;
            this.PART_Frame.NavigationFailed += this.PART_Frame_NavigationFailed;
            this.PART_Frame.NavigationProgress += this.PART_Frame_NavigationProgress;
            this.PART_Frame.NavigationStopped += this.PART_Frame_NavigationStopped;
            this.PART_Frame.LoadCompleted += this.PART_Frame_LoadCompleted;
            this.PART_Frame.FragmentNavigation += this.PART_Frame_FragmentNavigation;

            this.PART_BackButton.Click += this.PART_BackButton_Click;
            this.PART_ForwardButton.Click += this.PART_ForwardButton_Click;
        }

        [System.Diagnostics.DebuggerNonUserCode]
        private void PART_ForwardButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.CanGoForward)
            {
                this.GoForward();
            }
        }

        [System.Diagnostics.DebuggerNonUserCode]
        private void PART_Frame_FragmentNavigation(object sender, FragmentNavigationEventArgs e)
        {
            this.FragmentNavigation?.Invoke(this, e);
        }

        [System.Diagnostics.DebuggerNonUserCode]
        private void PART_Frame_LoadCompleted(object sender, NavigationEventArgs e)
        {
            this.LoadCompleted?.Invoke(this, e);
        }

        [System.Diagnostics.DebuggerNonUserCode]
        private void PART_Frame_NavigationStopped(object sender, NavigationEventArgs e)
        {
            this.NavigationStopped?.Invoke(this, e);
        }

        [System.Diagnostics.DebuggerNonUserCode]
        private void PART_Frame_NavigationProgress(object sender, NavigationProgressEventArgs e)
        {
            this.NavigationProgress?.Invoke(this, e);
        }

        [System.Diagnostics.DebuggerNonUserCode]
        private void PART_Frame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            this.NavigationFailed?.Invoke(this, e);
        }

        [System.Diagnostics.DebuggerNonUserCode]
        private void PART_Frame_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            this.Navigating?.Invoke(this, e);
        }

        [System.Diagnostics.DebuggerNonUserCode]
        private void PART_BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.CanGoBack)
            {
                this.GoBack();
            }
        }

        [System.Diagnostics.DebuggerNonUserCode]
        private void MetroNavigationWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.PART_Frame.FragmentNavigation -= this.PART_Frame_FragmentNavigation;
            this.PART_Frame.Navigating -= this.PART_Frame_Navigating;
            this.PART_Frame.NavigationFailed -= this.PART_Frame_NavigationFailed;
            this.PART_Frame.NavigationProgress -= this.PART_Frame_NavigationProgress;
            this.PART_Frame.NavigationStopped -= this.PART_Frame_NavigationStopped;
            this.PART_Frame.LoadCompleted -= this.PART_Frame_LoadCompleted;
            this.PART_Frame.Navigated -= this.PART_Frame_Navigated;

            this.PART_ForwardButton.Click -= this.PART_ForwardButton_Click;
            this.PART_BackButton.Click -= this.PART_BackButton_Click;

            this.Loaded -= this.MetroNavigationWindow_Loaded;
            this.Closing -= this.MetroNavigationWindow_Closing;
        }

        [System.Diagnostics.DebuggerNonUserCode]
        private void PART_Frame_Navigated(object sender, NavigationEventArgs e)
        {
            this.PART_Title.Content = ((Page)this.PART_Frame.Content).Title;
            (this as IUriContext).BaseUri = e.Uri;

            this.PageContent = this.PART_Frame.Content;

            this.PART_BackButton.IsEnabled = this.CanGoBack;

            this.PART_ForwardButton.IsEnabled = this.CanGoForward;

            this.Navigated?.Invoke(this, e);
        }

        /// <summary>
        /// Gets an IEnumerable that you use to enumerate the entries in back navigation history for a NavigationWindow.
        /// </summary>
        /// <see cref="System.Windows.Navigation.NavigationWindow.ForwardStack"/>
        public IEnumerable ForwardStack => this.PART_Frame.ForwardStack;

        /// <summary>
        /// Gets an IEnumerable that you use to enumerate the entries in back navigation history for a NavigationWindow.
        /// </summary>
        /// <see cref="System.Windows.Navigation.NavigationWindow.BackStack"/>
        public IEnumerable BackStack => this.PART_Frame.BackStack;

        /// <summary>
        /// Gets the NavigationService that is used by this MetroNavigationWindow to provide navigation services to its content.
        /// </summary>
        /// <see cref="System.Windows.Navigation.NavigationWindow.NavigationService"/>
        public NavigationService NavigationService => this.PART_Frame.NavigationService;

        /// <summary>
        /// Gets a value that indicates whether there is at least one entry in back navigation history.
        /// </summary>
        /// <see cref="System.Windows.Navigation.NavigationWindow.CanGoBack"/>
        public bool CanGoBack => this.PART_Frame.CanGoBack;

        /// <summary>
        /// Gets a value that indicates whether there is at least one entry in forward navigation history.
        /// </summary>
        /// <see cref="System.Windows.Navigation.NavigationWindow.CanGoForward"/>
        public bool CanGoForward => this.PART_Frame.CanGoForward;

        /// <summary>
        /// Gets or sets the base uniform resource identifier (URI) of the current context.
        /// </summary>
        /// <see cref="IUriContext.BaseUri"/>
        Uri? IUriContext.BaseUri { get; set; }

        /// <summary>
        /// Gets or sets the uniform resource identifier (URI) of the current content, or the URI of new content that is currently being navigated to.  
        /// </summary>
        /// <see cref="System.Windows.Navigation.NavigationWindow.Source"/>
        public Uri Source
        {
            get => this.PART_Frame.Source;
            set => this.PART_Frame.Source = value;
        }

        /// <summary>
        /// Adds an entry to back navigation history that contains a CustomContentState object.
        /// </summary>
        /// <param name="state">A CustomContentState object that represents application-defined state that is associated with a specific piece of content.</param>
        /// <see cref="System.Windows.Navigation.NavigationWindow.AddBackEntry"/>
        [System.Diagnostics.DebuggerNonUserCode]
        public void AddBackEntry(CustomContentState state)
        {
            this.PART_Frame.AddBackEntry(state);
        }

        /// <summary>
        /// Removes the most recent journal entry from back history.
        /// </summary>
        /// <returns>The most recent JournalEntry in back navigation history, if there is one.</returns>
        /// <see cref="System.Windows.Navigation.NavigationWindow.RemoveBackEntry"/>
        [System.Diagnostics.DebuggerNonUserCode]
        public JournalEntry RemoveBackEntry()
        {
            return this.PART_Frame.RemoveBackEntry();
        }

        /// <summary>
        /// Navigates to the most recent item in back navigation history.
        /// </summary>
        /// <see cref="System.Windows.Navigation.NavigationWindow.GoBack"/>
        [System.Diagnostics.DebuggerNonUserCode]
        public void GoBack()
        {
            this.PART_Frame.GoBack();
        }

        /// <summary>
        /// Navigates to the most recent item in forward navigation history.
        /// </summary>
        /// <see cref="System.Windows.Navigation.NavigationWindow.GoForward"/>
        [System.Diagnostics.DebuggerNonUserCode]
        public void GoForward()
        {
            this.PART_Frame.GoForward();
        }

        /// <summary>
        /// Navigates asynchronously to content that is contained by an object.
        /// </summary>
        /// <param name="content">An Object that contains the content to navigate to.</param>
        /// <returns>true if a navigation is not canceled; otherwise, false.</returns>
        /// <see cref="System.Windows.Navigation.NavigationWindow.Navigate(Object)"/>
        [System.Diagnostics.DebuggerNonUserCode]
        public bool Navigate(Object content)
        {
            return this.PART_Frame.Navigate(content);
        }

        /// <summary>
        /// Navigates asynchronously to content that is specified by a uniform resource identifier (URI).
        /// </summary>
        /// <param name="source">A Uri object initialized with the URI for the desired content.</param>
        /// <returns>true if a navigation is not canceled; otherwise, false.</returns>
        /// <see cref="System.Windows.Navigation.NavigationWindow.Navigate(Uri)"/>
        [System.Diagnostics.DebuggerNonUserCode]
        public bool Navigate(Uri source)
        {
            return this.PART_Frame.Navigate(source);
        }

        /// <summary>
        /// Navigates asynchronously to content that is contained by an object, and passes an object that contains data to be used for processing during navigation.
        /// </summary>
        /// <param name="content">An Object that contains the content to navigate to.</param>
        /// <param name="extraData">A Object that contains data to be used for processing during navigation.</param>
        /// <returns>true if a navigation is not canceled; otherwise, false.</returns>
        /// <see cref="System.Windows.Navigation.NavigationWindow.Navigate(Object, Object)"/>
        [System.Diagnostics.DebuggerNonUserCode]
        public bool Navigate(Object content, Object extraData)
        {
            return this.PART_Frame.Navigate(content, extraData);
        }

        /// <summary>
        /// Navigates asynchronously to source content located at a uniform resource identifier (URI), and pass an object that contains data to be used for processing during navigation.
        /// </summary>
        /// <param name="source">A Uri object initialized with the URI for the desired content.</param>
        /// <param name="extraData">A Object that contains data to be used for processing during navigation.</param>
        /// <returns>true if a navigation is not canceled; otherwise, false.</returns>
        /// <see cref="System.Windows.Navigation.NavigationWindow.Navigate(Uri, Object)"/>
        [System.Diagnostics.DebuggerNonUserCode]
        public bool Navigate(Uri source, Object extraData)
        {
            return this.PART_Frame.Navigate(source, extraData);
        }

        /// <summary>
        /// Stops further downloading of content for the current navigation request.
        /// </summary>
        /// <see cref="System.Windows.Navigation.NavigationWindow.StopLoading"/>
        [System.Diagnostics.DebuggerNonUserCode]
        public void StopLoading()
        {
            this.PART_Frame.StopLoading();
        }

        /// <summary>
        /// Occurs when navigation to a content fragment begins, which occurs immediately, if the desired fragment is in the current content, or after the source XAML content has been loaded, if the desired fragment is in different content.
        /// </summary>
        /// <see cref="System.Windows.Navigation.NavigationWindow.FragmentNavigation"/>
        public event FragmentNavigationEventHandler? FragmentNavigation;

        /// <summary>
        /// Occurs when a new navigation is requested.
        /// </summary>
        /// <see cref="System.Windows.Navigation.NavigationWindow.Navigating"/>
        public event NavigatingCancelEventHandler? Navigating;

        /// <summary>
        /// Occurs when an error is raised while navigating to the requested content.
        /// </summary>
        /// <see cref="System.Windows.Navigation.NavigationWindow.NavigationFailed"/>
        public event NavigationFailedEventHandler? NavigationFailed;

        /// <summary>
        /// Occurs periodically during a download to provide navigation progress information.
        /// </summary>
        /// <see cref="System.Windows.Navigation.NavigationWindow.NavigationProgress"/>
        public event NavigationProgressEventHandler? NavigationProgress;

        /// <summary>
        /// Occurs when the StopLoading method is called, or when a new navigation is requested while a current navigation is in progre
        /// </summary>
        /// <see cref="System.Windows.Navigation.NavigationWindow.NavigationStopped"/>
        public event NavigationStoppedEventHandler? NavigationStopped;

        /// <summary>
        /// Occurs when the content that is being navigated to has been found, and is available from the PageContent property, although it may not have completed loading
        /// </summary>
        /// <see cref="System.Windows.Navigation.NavigationWindow.Navigated"/>
        public event NavigatedEventHandler? Navigated;

        /// <summary>
        /// Occurs when content that was navigated to has been loaded, parsed, and has begun rendering.
        /// </summary>
        /// <see cref="System.Windows.Navigation.NavigationWindow.LoadCompleted"/>
        public event LoadCompletedEventHandler? LoadCompleted;
    }
}