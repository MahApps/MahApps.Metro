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
        }

        void MetroNavigationWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.Loaded -= MetroNavigationWindow_Loaded;
            PART_Frame.Navigated -= PART_Frame_Navigated;
            this.Closing -= MetroNavigationWindow_Closing;
        }

        void PART_Frame_Navigated(object sender, NavigationEventArgs e)
        {
            PART_Title.Content = (PART_Frame.Content as Page).Title;
            (this as IUriContext).BaseUri = e.Uri;
        }

        public static readonly DependencyProperty OverlayContentProperty = DependencyProperty.Register("OverlayContent", typeof(object), typeof(MetroNavigationWindow));

        public object OverlayContent
        {
            get { return GetValue(OverlayContentProperty); }
            set { SetValue(OverlayContentProperty, value); }
        }

        /// <summary>
        /// Gets a value that indicates whether there is at least one entry in back navigation history.
        /// </summary>
        /// <see cref="http://msdn.microsoft.com/en-us/library/system.windows.navigation.navigationwindow.cangoback(v=vs.110).aspx"/>
        public bool CanGoBack { get { return PART_Frame.CanGoBack; } }
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
    }
}
