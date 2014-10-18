using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media.Animation;

namespace MahApps.Metro.Controls
{

    [ContentProperty("Content")]
    [TemplatePart(Name = PART_ContentPresenter, Type = typeof(UIElement))]
    public class MetroPopover : Control
    {
        private const string PART_ContentPresenter = "PART_ContentPresenter";

        static MetroPopover()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MetroPopover), new FrameworkPropertyMetadata(typeof(MetroPopover)));
        }

        public MetroPopover(UIElement target)
        {
            Target = target;
        }

        public MetroWindow Owner { get; internal set; }
        public UIElement Target { get; private set; }

        public static readonly DependencyProperty ContentProperty = DependencyProperty.Register("Content", typeof(object), typeof(MetroPopover), new PropertyMetadata(null));
        public static readonly DependencyProperty IsOpenProperty = DependencyProperty.Register("IsOpen", typeof(bool), typeof(MetroPopover), new PropertyMetadata(false));
        
        public object Content
        {
            get { return (object)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }

        public event EventHandler<EventArgs> Shown;
        public event EventHandler<EventArgs> Closed;      
 
        public Task OpenAsync()
        {
            return WaitForLoadAsync()
                .ContinueWith(t => {
                    OnOpened();
                }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        public Task<bool> RequestCloseAsync()
        {
            if (OnRequestClose()) {
                return WaitForCloseAsync()
                    .ContinueWith(t => {
                        OnClosed();
                        return true;
                    }, TaskScheduler.FromCurrentSynchronizationContext());
            }
            return Task.Factory.StartNew(() => false);
        }

        protected virtual void OnOpened() 
        {
            this.IsOpen = true;
            if (Shown != null) {
                Shown(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// A last chance virtual method for stopping an popover from closing.
        /// </summary>
        /// <returns></returns>
        protected virtual bool OnRequestClose()
        {
            return true; //allow the dialog to close.
        }

        protected virtual void OnClosed()
        {
            this.IsOpen = false;
            if (Closed != null) {
                Closed(this, EventArgs.Empty);
            }

        }

        /// <summary>
        /// Waits for the popover to become ready for interaction.
        /// </summary>
        /// <returns>A task that represents the operation and it's status.</returns>
        Task WaitForLoadAsync()
        {
            Dispatcher.VerifyAccess();

            if (this.IsLoaded) return new Task(() => { });

            if (!false)
                this.Opacity = 1.0; //skip the animation

            TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();

            RoutedEventHandler handler = null;
            handler = new RoutedEventHandler((sender, args) => {
                this.Loaded -= handler;

                tcs.TrySetResult(null);
            });

            this.Loaded += handler;

            return tcs.Task;
        }

        Task WaitForCloseAsync()
        {
            TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();

            if (false) {
                Storyboard closingStoryboard = this.Resources["PopoverCloseStoryboard"] as Storyboard;

                if (closingStoryboard == null)
                    throw new InvalidOperationException("Unable to find the dialog closing storyboard. Did you forget to add MetroPopoverDialog.xaml to your merged dictionaries?");

                EventHandler handler = null;
                handler = new EventHandler((sender, args) => {
                    closingStoryboard.Completed -= handler;

                    tcs.TrySetResult(null);
                });

                closingStoryboard = closingStoryboard.Clone();

                closingStoryboard.Completed += handler;

                closingStoryboard.Begin(this);
            } else {
                this.Opacity = 0.0;
                tcs.TrySetResult(null); //skip the animation
            }

            return tcs.Task;
        }


    }
}
