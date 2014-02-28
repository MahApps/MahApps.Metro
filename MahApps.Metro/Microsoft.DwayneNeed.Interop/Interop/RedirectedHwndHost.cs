using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Interop;
using System.ComponentModel;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Input;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Threading;
using Microsoft.DwayneNeed.Win32.User32;
using Microsoft.DwayneNeed.Win32.ComCtl32;
using Microsoft.DwayneNeed.Extensions;
using Microsoft.DwayneNeed.Win32.Gdi32;
using Microsoft.DwayneNeed.Win32;

namespace Microsoft.DwayneNeed.Interop
{
    public class RedirectedHwndHost : FrameworkElement, IDisposable, IKeyboardInputSink
    {
        #region RedirectionVisibility
        public static readonly DependencyProperty RedirectionVisibilityProperty = DependencyProperty.Register(
            /* Name:                 */ "RedirectionVisibility",
            /* Value Type:           */ typeof(RedirectionVisibility),
            /* Owner Type:           */ typeof(RedirectedHwndHost),
            /* Metadata:             */ new FrameworkPropertyMetadata(
            /*     Default Value:    */ RedirectionVisibility.Hidden,
            /*     Property Changed: */ (d, e) => ((RedirectedHwndHost)d).OnRedirectionVisibilityChanged(e)));

        /// <summary>
        ///     The visibility of the redirection surface.
        /// </summary>
        public RedirectionVisibility RedirectionVisibility
        {
            get { return (RedirectionVisibility)GetValue(RedirectionVisibilityProperty); }
            set { SetValue(RedirectionVisibilityProperty, value); }
        }

        private void OnRedirectionVisibilityChanged(DependencyPropertyChangedEventArgs e)
        {
            UpdateRedirectedWindowSettings(RedirectionVisibility, false);
        }
        #endregion
        #region IsOutputRedirectionEnabled
        public static readonly DependencyProperty IsOutputRedirectionEnabledProperty = DependencyProperty.Register(
            /* Name:                 */ "IsOutputRedirectionEnabled",
            /* Value Type:           */ typeof(bool),
            /* Owner Type:           */ typeof(RedirectedHwndHost),
            /* Metadata:             */ new FrameworkPropertyMetadata(
            /*     Default Value:    */ false,
            /*     Property Changed: */ (d,e)=>((RedirectedHwndHost)d).OnIsOutputRedirectionEnabledChanged(e)));

        /// <summary>
        ///     Whether or not output redirection is enabled.
        /// </summary>
        public bool IsOutputRedirectionEnabled
        {
            get { return (bool)GetValue(IsOutputRedirectionEnabledProperty); }
            set { SetValue(IsOutputRedirectionEnabledProperty, value); }
        }

        private void OnIsOutputRedirectionEnabledChanged(DependencyPropertyChangedEventArgs e)
        {
            _outputRedirectionTimer.IsEnabled = (bool) e.NewValue;
        }
        #endregion
        #region OutputRedirectionPeriod
        public static readonly DependencyProperty OutputRedirectionPeriodProperty = DependencyProperty.Register(
            /* Name:                 */ "OutputRedirectionPeriod",
            /* Value Type:           */ typeof(TimeSpan),
            /* Owner Type:           */ typeof(RedirectedHwndHost),
            /* Metadata:             */ new FrameworkPropertyMetadata(
            /*     Default Value:    */ TimeSpan.FromMilliseconds(30),
            /*     Property Changed: */ (d,e)=>((RedirectedHwndHost)d).OnOutputRedirectionPeriodChanged(e)));

        /// <summary>
        ///     The period of time to update the output redirection.
        /// </summary>
        public TimeSpan OutputRedirectionPeriod
        {
            get { return (TimeSpan)GetValue(OutputRedirectionPeriodProperty); }
            set { SetValue(OutputRedirectionPeriodProperty, value); }
        }

        private void OnOutputRedirectionPeriodChanged(DependencyPropertyChangedEventArgs e)
        {
            _outputRedirectionTimer.Interval = (TimeSpan)e.NewValue;
        }
        #endregion
        #region IsInputRedirectionEnabled
        public static readonly DependencyProperty IsInputRedirectionEnabledProperty = DependencyProperty.Register(
            /* Name:                 */ "IsInputRedirectionEnabled",
            /* Value Type:           */ typeof(bool),
            /* Owner Type:           */ typeof(RedirectedHwndHost),
            /* Metadata:             */ new FrameworkPropertyMetadata(
            /*     Default Value:    */ false,
            /*     Property Changed: */ (d, e) => ((RedirectedHwndHost)d).OnIsInputRedirectionEnabledChanged(e)));

        /// <summary>
        ///     Whether or not input redirection is enabled.
        /// </summary>
        public bool IsInputRedirectionEnabled
        {
            get { return (bool)GetValue(IsInputRedirectionEnabledProperty); }
            set { SetValue(IsInputRedirectionEnabledProperty, value); }
        }

        private void OnIsInputRedirectionEnabledChanged(DependencyPropertyChangedEventArgs e)
        {
            _inputRedirectionTimer.IsEnabled = (bool)e.NewValue;
        }
        #endregion
        #region InputRedirectionPeriod
        public static readonly DependencyProperty InputRedirectionPeriodProperty = DependencyProperty.Register(
            /* Name:                 */ "InputRedirectionPeriod",
            /* Value Type:           */ typeof(TimeSpan),
            /* Owner Type:           */ typeof(RedirectedHwndHost),
            /* Metadata:             */ new FrameworkPropertyMetadata(
            /*     Default Value:    */ TimeSpan.FromMilliseconds(30),
            /*     Property Changed: */ (d, e) => ((RedirectedHwndHost)d).OnInputRedirectionPeriodChanged(e)));

        /// <summary>
        ///     The period of time to update the input redirection.
        /// </summary>
        public TimeSpan InputRedirectionPeriod
        {
            get { return (TimeSpan)GetValue(InputRedirectionPeriodProperty); }
            set { SetValue(InputRedirectionPeriodProperty, value); }
        }

        private void OnInputRedirectionPeriodChanged(DependencyPropertyChangedEventArgs e)
        {
            _inputRedirectionTimer.Interval = (TimeSpan)e.NewValue;
        }
        #endregion

        #region CurrentHwndSource
        public static readonly DependencyPropertyKey CurrentHwndSourcePropertyKey = DependencyProperty.RegisterReadOnly(
            "CurrentHwndSource",
            typeof(HwndSource),
            typeof(RedirectedHwndHost),
            new PropertyMetadata(null, (d, e) => ((RedirectedHwndHost)d).OnCurrentHwndSourceChanged(e)));

        public static readonly DependencyProperty CurrentHwndSourceProperty = CurrentHwndSourcePropertyKey.DependencyProperty;

        public HwndSource CurrentHwndSource
        {
            get { return (HwndSource)GetValue(CurrentHwndSourceProperty); }
            private set { SetValue(CurrentHwndSourcePropertyKey, value); }
        }
        #endregion

        static RedirectedHwndHost()
        {
            _redirectionWindowFactory = new WindowClass<RedirectedWindow>();
            _redirectionWindowFactory.BeginInit();
            _redirectionWindowFactory.Type = WindowClassType.ApplicationLocal;
            //_redirectionWindowFactory.Background = NativeMethods.GetStockObject(5);
            _redirectionWindowFactory.EndInit();
        }

        public RedirectedHwndHost()
        {
            PresentationSource.AddSourceChangedHandler(this, (s, e) => CurrentHwndSource = (HwndSource)e.NewSource);
            Loaded += new RoutedEventHandler(OnLoaded);

            _inputRedirectionTimer = new DispatcherTimer(DispatcherPriority.Input);
            _inputRedirectionTimer.Tick += (e, a) => UpdateInputRedirection();
            _inputRedirectionTimer.Interval = InputRedirectionPeriod;
            _inputRedirectionTimer.IsEnabled = IsInputRedirectionEnabled;

            _outputRedirectionTimer = new DispatcherTimer(DispatcherPriority.Render);
            _outputRedirectionTimer.Tick += (e, a) => UpdateOutputRedirection();
            _outputRedirectionTimer.Interval = OutputRedirectionPeriod;
            _outputRedirectionTimer.IsEnabled = IsOutputRedirectionEnabled;
        }

        /// <summary>
        ///     The window handle of the hosted child window.
        /// </summary>
        /// <remarks>
        ///     Derived types override the BuildWindowCore virtual method to
        ///     create the child window, the handle is exposed through this
        ///     property.
        /// </remarks>
        public HWND Handle {get; private set;}

        #region IDisposable
        public void Dispose()
        {
            Dispose(true);

            // Call SuppressFinalize, even though we don't have a finalizer.
            // This is because a derived type may add a finalizer, and they
            // should not have to reimplement the Dispose pattern.
            GC.SuppressFinalize(this);
        }

        public bool IsDisposed {get; private set;}

        protected virtual void Dispose(bool disposing)
        {
            VerifyNotDisposed();

            if (disposing)
            {
                _inputRedirectionTimer.IsEnabled = false;
                _inputRedirectionTimer = null;

                _outputRedirectionTimer.IsEnabled = false;
                _outputRedirectionTimer = null;

                if (Handle != null)
                {
                    Handle.Dispose();
                    Handle = null;
                }

                if (_redirectedWindow != null)
                {
                    _redirectedWindow.Dispose();
                }
            }

            IsDisposed = true;
        }

        private void VerifyNotDisposed()
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(this.GetType().ToString());
            }
        }
        #endregion

        protected virtual void OnCurrentHwndSourceChanged(DependencyPropertyChangedEventArgs e)
        {
            Initialize();

            // Unregister the old keyboard input site.
            IKeyboardInputSite keyboardInputSite = ((IKeyboardInputSink)this).KeyboardInputSite;
            if (keyboardInputSite != null)
            {
                ((IKeyboardInputSink)this).KeyboardInputSite = null;
                keyboardInputSite.Unregister();
            }

            // Register the new keyboard input site with the containing
            // HwndSource.
            IKeyboardInputSink sink = CurrentHwndSource;
            if (sink != null)
            {
                ((IKeyboardInputSink)this).KeyboardInputSite = sink.RegisterKeyboardInputSink(this);
            }

            // Set the owner of the RedirectedWindow to our CurrentHwndSource.
            // This keeps the RedirectedWindow on top of the HwndSource.
            if (CurrentHwndSource != null)
            {
                HWND hwndSource = new HWND(CurrentHwndSource.Handle);
                HWND hwndRoot = hwndSource; // User32NativeMethods.GetAncestor(hwndSource, GA.ROOT); // need to get the top-level window?
                NativeMethods.SetWindowLongPtr(
                    _redirectedWindow.Handle,
                    GWL.HWNDPARENT,
                    hwndRoot.DangerousGetHandle());
            }
        }

        /// <summary>
        ///     Creates the hosted child window.
        /// </summary>
        /// <remarks>
        ///     Derived types override the BuildWindowCore virtual method to
        ///     create the child window.  The child window is parented to
        ///     a seperate top-level window for the purposes of redirection.
        ///     The SafeWindowHandle type controls the lifetime of the
        ///     child window.  It will be disposed when the RedirectedHwndHost
        ///     is disposed, or when the SafeWindowHandle is finalized.  Set
        ///     the SafeWindowHandle.DestroyWindowOnRelease property to true
        ///     if you want the window destroyed automatically.
        /// </remarks>
        protected virtual HWND BuildWindowCore(HWND hwndParent)
        {
            StrongHWND hwndChild = StrongHWND.CreateWindowEx(
                0,
                "STATIC",
                "Default RedirectedHwndHost Window",
                WS.CHILD | WS.CLIPSIBLINGS | WS.CLIPCHILDREN,
                0,
                0,
                0,
                0,
                hwndParent,
                IntPtr.Zero,
                IntPtr.Zero,
                IntPtr.Zero);

            return hwndChild;
        }

        protected override void OnRender(System.Windows.Media.DrawingContext drawingContext)
        {
            if (_bitmap != null)
            {
                drawingContext.PushClip(new RectangleGeometry(new Rect(RenderSize)));
                drawingContext.DrawImage(_bitmap, new Rect(new Size(_bitmap.Width, _bitmap.Height)));
                drawingContext.Pop();
            }
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            Matrix dpiScale = CurrentHwndSource.CompositionTarget.TransformToDevice;
            Vector vSize = new Vector(sizeInfo.NewSize.Width, sizeInfo.NewSize.Height);
            vSize = dpiScale.Transform(vSize);

            int width = (int) Math.Ceiling(vSize.X);
            int height = (int) Math.Ceiling(vSize.Y);

            // Size the child window to be the natural size of the element.
            NativeMethods.SetWindowPos(
                Handle,
                HWND.NULL,
                0,
                0,
                width,
                height,
                SWP.NOZORDER | SWP.NOCOPYBITS);

            // Size the redirected window to contain the child window.
            _redirectedWindow.SetClientAreaSize(width, height);

            if (IsOutputRedirectionEnabled)
            {
                UpdateOutputRedirection();
            }

            base.OnRenderSizeChanged(sizeInfo);
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            // Only need this once.
            Loaded -= OnLoaded;

            Initialize();
        }

        private void Initialize()
        {
            if (Handle != null)
            {
                // Already initialized.
                return;
            }

            // When loaded for the first time, build the top-level redirected
            // window to host the child window.
            WindowParameters windowParams = new WindowParameters();
            windowParams.Name = "RedirectedHwndHost";
            windowParams.Style = WS.OVERLAPPED | WS.CLIPCHILDREN | WS.CAPTION;
            windowParams.ExtendedStyle = WS_EX.LAYERED | WS_EX.NOACTIVATE | WS_EX.TOOLWINDOW | WS_EX.TRANSPARENT;
            windowParams.WindowRect = new Int32Rect(0, 0, 500, 500);

            _redirectedWindow = _redirectionWindowFactory.CreateWindow(windowParams);
            UpdateRedirectedWindowSettings(RedirectionVisibility, false);

            // Then create the child window to host.
            Handle = BuildWindowCore(_redirectedWindow.Handle);
            if (Handle == null || Handle.IsInvalid)
            {
                throw new InvalidOperationException("BuildWindowCore did not return a valid handle.");
            }

            _redirectedWindow.Show(WindowShowState.Normal, false);
        }

        private void UpdateInputRedirection()
        {
            uint messagePos = NativeMethods.GetMessagePos();
            int xScreen = NativeMacros.GET_X_LPARAM(messagePos);
            int yScreen = NativeMacros.GET_Y_LPARAM(messagePos);

            POINT? ptClient = GetInputSyncPoint(xScreen, yScreen);
            if (ptClient.HasValue)
            {
                _redirectedWindow.AlignClientAndScreen(ptClient.Value.x, ptClient.Value.y, xScreen, yScreen);
                UpdateRedirectedWindowSettings(RedirectionVisibility, true);
            }
            else
            {
                UpdateRedirectedWindowSettings(RedirectionVisibility, false);
                // TODO: shove the window away? (on an animation?)
            }
        }

        private void UpdateRedirectedWindowSettings(RedirectionVisibility visibility, bool isMouseOver)
        {
            if (_redirectedWindow != null)
            {
                switch (visibility)
                {
                    case RedirectionVisibility.Visible:
                        _redirectedWindow.Alpha = 100;
                        _redirectedWindow.IsHitTestable = isMouseOver;
                        break;

                    case RedirectionVisibility.Interactive:
                        _redirectedWindow.Alpha = 100;
                        _redirectedWindow.IsHitTestable = true;
                        break;

                    default:
                    case RedirectionVisibility.Hidden:
                        _redirectedWindow.Alpha = (byte)1; // Not *quite* invisible, which is important so we can still capture content.
                        _redirectedWindow.IsHitTestable = isMouseOver;
                        break;
                }
            }
        }

        /// <summary>
        ///     Return the point to sync the window to.  The point is in the coordinate
        ///     space of the image, which is the same as the client coordinate space
        ///     of the hosted window.  This function returns null if the input should
        ///     not be synchronized for this redirected window.
        /// </summary>
        /// <returns></returns>
        private POINT? GetInputSyncPoint(int xScreen, int yScreen)
        {
            POINT? ptClient = null;

            HwndSource currentHwndSource = CurrentHwndSource;
            if (currentHwndSource != null)
            {
                HWND hwndCapture = NativeMethods.GetCapture();
                if (hwndCapture != HWND.NULL)
                {
                    // The mouse is captured, so only sync input if the mouse is
                    // captured to a hosted window within us.
                    HWND root = NativeMethods.GetAncestor(hwndCapture, GA.ROOT);
                    if (_redirectedWindow.Handle == root)
                    {
                        // The HWND with capture is within us.
                        // Transform the screen coordinates into the local coordinates.
                        Point pt = new Point(xScreen, yScreen);
                        pt = currentHwndSource.TransformScreenToClient(pt);
                        pt = currentHwndSource.TransformClientToRoot(pt);
                        pt = currentHwndSource.RootVisual.TransformToDescendant(this).Transform(pt);

                        ptClient = new POINT { x = (int)Math.Round(pt.X), y = (int)Math.Round(pt.Y) };
                    }
                }
                else
                {
                    // The mouse is not captured, so only sync input if the mouse
                    // is over our element.
                    // Convert the mouse coordinates to the client coordinates of the
                    // HwndSource.
                    Point pt = new Point(xScreen, yScreen);
                    pt = currentHwndSource.TransformScreenToClient(pt);
                    pt = currentHwndSource.TransformClientToRoot(pt);
                    RedirectedHwndHost hit = ((UIElement)currentHwndSource.RootVisual).InputHitTest(pt) as RedirectedHwndHost;

                    if (hit == this)
                    {
                        // Transform into the coordinate space of the
                        // RedirectedHwndHost element.
                        var xfrm = currentHwndSource.RootVisual.TransformToDescendant(hit);
                        pt = xfrm.Transform(pt);
                        ptClient = new POINT { x = (int)Math.Round(pt.X), y = (int)Math.Round(pt.Y) };
                    }
                }
            }

            return ptClient;
        }

        private void UpdateOutputRedirection()
        {
            BitmapSource bitmap = _redirectedWindow.UpdateRedirectedBitmap();
            if (bitmap != _bitmap)
            {
                _bitmap = bitmap;
                InvalidateVisual();
            }
        }

        #region IKeyboardInputSink
        bool IKeyboardInputSink.HasFocusWithin()
        {
            return HasFocusWithinCore();
        }

        IKeyboardInputSite IKeyboardInputSink.KeyboardInputSite { get; set; }

        bool IKeyboardInputSink.OnMnemonic(ref MSG msg, ModifierKeys modifiers)
        {
            return OnMnemonicCore(ref msg, modifiers);
        }

        IKeyboardInputSite IKeyboardInputSink.RegisterKeyboardInputSink(IKeyboardInputSink sink)
        {
            return RegisterKeyboardInputSinkCore(sink);
        }

        bool IKeyboardInputSink.TabInto(TraversalRequest request)
        {
            return TabIntoCore(request);
        }

        bool IKeyboardInputSink.TranslateAccelerator(ref MSG msg, ModifierKeys modifiers)
        {
            return TranslateAcceleratorCore(ref msg, modifiers);
        }

        bool IKeyboardInputSink.TranslateChar(ref MSG msg, ModifierKeys modifiers)
        {
            return TranslateCharCore(ref msg, modifiers);
        }

        protected virtual bool HasFocusWithinCore()
        {
            HWND hwndFocus = NativeMethods.GetFocus();
            return (hwndFocus != null && !hwndFocus.IsInvalid && NativeMethods.IsChild(Handle, hwndFocus));
        }

        protected virtual bool OnMnemonicCore(ref MSG msg, ModifierKeys modifiers)
        {
            return false;
        }

        protected virtual IKeyboardInputSite RegisterKeyboardInputSinkCore(IKeyboardInputSink sink)
        {
            throw new InvalidOperationException("RedirectedHwndHost does not support child keyboard sinks be default.");
        }

        protected virtual bool TabIntoCore(TraversalRequest request)
        {
            return false;
        }

        protected virtual bool TranslateAcceleratorCore(ref MSG msg, ModifierKeys modifiers)
        {
            return false;
        }

        protected virtual bool TranslateCharCore(ref MSG msg, ModifierKeys modifiers)
        {
            return false;
        }
        #endregion

        private static WindowClass<RedirectedWindow> _redirectionWindowFactory;
        internal RedirectedWindow _redirectedWindow;
        private BitmapSource _bitmap;
        private DispatcherTimer _inputRedirectionTimer;
        private DispatcherTimer _outputRedirectionTimer;
    }
}
