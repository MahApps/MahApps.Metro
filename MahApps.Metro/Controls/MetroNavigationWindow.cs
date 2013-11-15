using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Navigation;

namespace MahApps.Metro.Controls
{
    [ContentProperty("OverlayContent")]
    public partial class MetroNavigationWindow : MetroWindow, IUriContext
    {
        public MetroNavigationWindow()
        {
            InitializeComponent();

            this.Loaded += MetroNavigationWindow_Loaded;
            this.Closing += MetroNavigationWindow_Closing;
        }

        void MetroNavigationWindow_Loaded(object sender, RoutedEventArgs e)
        {
            PART_Frame.Navigated += PART_Frame_Navigated;
            PART_BackButton.Click += PART_BackButton_Click;
        }

        void PART_BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (CanGoBack)
                GoBack();
        }

        void MetroNavigationWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            PART_BackButton.Click -= PART_BackButton_Click;
            this.Loaded -= MetroNavigationWindow_Loaded;
            PART_Frame.Navigated -= PART_Frame_Navigated;
            this.Closing -= MetroNavigationWindow_Closing;
        }

        void PART_Frame_Navigated(object sender, NavigationEventArgs e)
        {
            PART_Title.Content = (PART_Frame.Content as Page).Title;
            (this as IUriContext).BaseUri = e.Uri;

            PART_BackButton.Visibility = CanGoBack ? Visibility.Visible : System.Windows.Visibility.Hidden;
        }

        public static readonly DependencyProperty OverlayContentProperty = DependencyProperty.Register("OverlayContent", typeof(object), typeof(MetroNavigationWindow));

        public object OverlayContent
        {
            get { return GetValue(OverlayContentProperty); }
            set { SetValue(OverlayContentProperty, value); }
        }


        /// <summary>
        /// Gets the NavigationService that is used by this MetroNavigationWindow to provide navigation services to its content.
        /// </summary>
        /// <see cref="http://msdn.microsoft.com/en-us/library/system.windows.navigation.navigationwindow.navigationservice(v=vs.110).aspx"/>
        public NavigationService NavigationService { get { return PART_Frame.NavigationService; } }
        /// <summary>
        /// Gets a value that indicates whether there is at least one entry in back navigation history.
        /// </summary>
        /// <see cref="http://msdn.microsoft.com/en-us/library/system.windows.navigation.navigationwindow.cangoback(v=vs.110).aspx"/>
        public bool CanGoBack { get { return PART_Frame.CanGoBack; } }
        /// <summary>
        /// Gets a value that indicates whether there is at least one entry in forward navigation history.
        /// </summary>
        /// <see cref="http://msdn.microsoft.com/en-us/library/system.windows.navigation.navigationwindow.cangoforward(v=vs.110).aspx"/>
        public bool CanGoForward { get { return PART_Frame.CanGoForward; } }
        /// <summary>
        /// Gets or sets the base uniform resource identifier (URI) of the current context.
        /// </summary>
        /// <see cref="http://msdn.microsoft.com/en-us/library/dd807467(v=vs.110).aspx"/>
        Uri IUriContext.BaseUri { get; set; }

        /// <summary>
        /// Gets or sets the uniform resource identifier (URI) of the current content, or the URI of new content that is currently being navigated to.  
        /// </summary>
        /// <see cref="http://msdn.microsoft.com/en-us/library/system.windows.navigation.navigationwindow.source(v=vs.110).aspx"/>
        public Uri Source { get { return PART_Frame.Source; } set { PART_Frame.Source = value; } }

        /// <summary>
        /// Adds an entry to back navigation history that contains a CustomContentState object.
        /// </summary>
        /// <param name="state">A CustomContentState object that represents application-defined state that is associated with a specific piece of content.</param>
        /// <see cref="http://msdn.microsoft.com/en-us/library/system.windows.navigation.navigationwindow.addbackentry(v=vs.110).aspx"/>
        [System.Diagnostics.DebuggerNonUserCode]
        public void AddBackEntry(CustomContentState state)
        {
            PART_Frame.AddBackEntry(state);
        }

        /// <summary>
        /// Navigates to the most recent item in back navigation history.
        /// </summary>
        /// <see cref="http://msdn.microsoft.com/en-us/library/system.windows.navigation.navigationwindow.goback(v=vs.110).aspx"/>
        [System.Diagnostics.DebuggerNonUserCode]
        public void GoBack()
        {
            PART_Frame.GoBack();
        }

        /// <summary>
        /// Navigates to the most recent item in forward navigation history.
        /// </summary>
        /// <see cref="http://msdn.microsoft.com/en-us/library/system.windows.navigation.navigationwindow.goforward(v=vs.110).aspx"/>
        [System.Diagnostics.DebuggerNonUserCode]
        public void GoForward()
        {
            PART_Frame.GoForward();
        }

        /// <summary>
        /// Navigates asynchronously to content that is contained by an object.
        /// </summary>
        /// <param name="content">An Object that contains the content to navigate to.</param>
        /// <returns>true if a navigation is not canceled; otherwise, false.</returns>
        /// <see cref="http://msdn.microsoft.com/en-us/library/ms591150(v=vs.110).aspx"/>
        [System.Diagnostics.DebuggerNonUserCode]
        public bool Navigate(Object content)
        {
            return PART_Frame.Navigate(content);
        }
        /// <summary>
        /// Navigates asynchronously to content that is specified by a uniform resource identifier (URI).
        /// </summary>
        /// <param name="source">A Uri object initialized with the URI for the desired content.</param>
        /// <returns>true if a navigation is not canceled; otherwise, false.</returns>
        /// <see cref="http://msdn.microsoft.com/en-us/library/ms591151(v=vs.110).aspx"/>
        [System.Diagnostics.DebuggerNonUserCode]
        public bool Navigate(Uri source)
        {
            return PART_Frame.Navigate(source);
        }
        /// <summary>
        /// Navigates asynchronously to content that is contained by an object, and passes an object that contains data to be used for processing during navigation.
        /// </summary>
        /// <param name="content">An Object that contains the content to navigate to.</param>
        /// <param name="extraData">A Object that contains data to be used for processing during navigation.</param>
        /// <returns>true if a navigation is not canceled; otherwise, false.</returns>
        /// <see cref="http://msdn.microsoft.com/en-us/library/ms591148(v=vs.110).aspx"/>
        [System.Diagnostics.DebuggerNonUserCode]
        public bool Navigate(Object content, Object extraData)
        {
            return PART_Frame.Navigate(content, extraData);
        }
        /// <summary>
        /// Navigates asynchronously to source content located at a uniform resource identifier (URI), and pass an object that contains data to be used for processing during navigation.
        /// </summary>
        /// <param name="source">A Uri object initialized with the URI for the desired content.</param>
        /// <param name="extraData">A Object that contains data to be used for processing during navigation.</param>
        /// <returns>true if a navigation is not canceled; otherwise, false.</returns>
        /// <see cref="http://msdn.microsoft.com/en-us/library/ms591149(v=vs.110).aspx"/>
        public bool Navigate(Uri source, Object extraData)
        {
            return PART_Frame.Navigate(source, extraData);
        }

    }
}
