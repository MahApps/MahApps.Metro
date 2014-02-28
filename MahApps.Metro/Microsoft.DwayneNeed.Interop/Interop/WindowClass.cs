using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Diagnostics;
using Microsoft.DwayneNeed.Win32.User32;
using Microsoft.DwayneNeed.Win32.ComCtl32;
using Microsoft.DwayneNeed.Win32;

namespace Microsoft.DwayneNeed.Interop
{
    public sealed class WindowClass<TWindow> : ISupportInitialize where TWindow : WindowBase, new()
    {
        public WindowClass()
        {
            _wcex = new WNDCLASSEX();
            _wcex.cbSize = Marshal.SizeOf(_wcex);
            _wcex.hInstance = Marshal.GetHINSTANCE(typeof(WindowClass<TWindow>).Module);
            _wcex.lpszClassName = typeof(WindowClass<TWindow>).FullName;
            _wcex.lpfnWndProc = WndProc;
        }

        public string Name
        {
            get
            {
                return _wcex.lpszClassName;
            }
        }

        public bool EnableDoubleClickMessages
        {
            get
            {
                return (_wcex.style & CS.DBLCLKS) == CS.DBLCLKS;
            }

            set
            {
                _wcex.style |= CS.DBLCLKS;
            }
        }

        public bool RedrawWindowOnVerticalChange
        {
            get
            {
                return (_wcex.style & CS.VREDRAW) == CS.VREDRAW;
            }

            set
            {
                _wcex.style |= CS.VREDRAW;
            }
        }

        public bool RedrawWindowOnHorizontalChange
        {
            get
            {
                return (_wcex.style & CS.HREDRAW) == CS.HREDRAW;
            }

            set
            {
                _wcex.style |= CS.HREDRAW;
            }
        }

        public bool CacheBackgroundBitmap
        {
            get
            {
                return (_wcex.style & CS.SAVEBITS) == CS.SAVEBITS;
            }

            set
            {
                _wcex.style |= CS.SAVEBITS;
            }
        }

        public bool UseParentClippingRect
        {
            get
            {
                return (_wcex.style & CS.PARENTDC) == CS.PARENTDC;
            }

            set
            {
                _wcex.style |= CS.PARENTDC;
            }
        }

        public bool EnableDropShadow
        {
            get
            {
                return (_wcex.style & CS.DROPSHADOW) == CS.DROPSHADOW;
            }

            set
            {
                _wcex.style |= CS.DROPSHADOW;
            }
        }

        public WindowClassType Type
        {
            get
            {
                return (_wcex.style & CS.GLOBALCLASS) == CS.GLOBALCLASS ? 
                    WindowClassType.ApplicationGlobal : 
                    WindowClassType.ApplicationLocal;
            }

            set
            {
                switch (value)
                {
                    case WindowClassType.ApplicationGlobal:
                        _wcex.style |= CS.DROPSHADOW;
                        break;

                    case WindowClassType.ApplicationLocal:
                        _wcex.style &= ~CS.DROPSHADOW;
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public DeviceContextCachePolicy DeviceContextCachePolicy
        {
            get
            {
                CS mask = CS.CLASSDC | CS.OWNDC;
                switch (_wcex.style & mask)
                {
                    case CS.CLASSDC:
                        return DeviceContextCachePolicy.WindowClass;

                    case CS.OWNDC:
                        return DeviceContextCachePolicy.Window;

                    default:
                        return DeviceContextCachePolicy.Global;
                }
            }

            set
            {
                CS mask = CS.CLASSDC | CS.OWNDC;

                switch (value)
                {
                    case DeviceContextCachePolicy.WindowClass:
                        _wcex.style &= ~mask;
                        _wcex.style |= CS.CLASSDC;
                        break;

                    case DeviceContextCachePolicy.Window:
                        _wcex.style &= ~mask;
                        _wcex.style |= CS.OWNDC;
                        break;

                    case DeviceContextCachePolicy.Global:
                        _wcex.style &= ~mask;
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public int ExtraClassBytes
        {
            get
            {
                return _wcex.cbClsExtra;
            }

            set
            {
                if(value < 0)
                {
                    throw new ArgumentOutOfRangeException();
                }
                _wcex.cbClsExtra = value;
            }
        }

        public int ExtraWindowBytes
        {
            get
            {
                return _wcex.cbWndExtra;
            }

            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException();
                }
                _wcex.cbWndExtra = value;
            }
        }

        public IntPtr Background
        {
            get
            {
                return _wcex.hbrBackground;
            }

            set
            {
                _wcex.hbrBackground = value;
            }
        }

        public void BeginInit()
        {
            switch (_initializationState)
            {
                case false:
                    throw new InvalidOperationException("The window class has already begun initialization.");

                case true:
                    throw new InvalidOperationException("The window class has already completed initialization.");

                case null:
                    _initializationState = false;
                    break;
            }
        }

        public void EndInit()
        {
            switch (_initializationState)
            {
                case null:
                    throw new InvalidOperationException("The window class has not begun initialization.");

                case true:
                    throw new InvalidOperationException("The window class has already completed initialization.");

                case false:
                    _atom = NativeMethods.RegisterClassEx(ref _wcex);
                    break;
            }
        }

        public TWindow CreateWindow(WindowParameters windowParams)
        {
            GCHandle gcHandle = new GCHandle();
            IntPtr lpCreateParam = IntPtr.Zero;
            if(windowParams.Tag != null)
            {
                gcHandle = GCHandle.Alloc(windowParams.Tag);
                lpCreateParam = GCHandle.ToIntPtr(gcHandle);
            }

            StrongHWND hwnd = StrongHWND.CreateWindowEx(
                windowParams.ExtendedStyle,
                Name,
                windowParams.Name,
                windowParams.Style,
                windowParams.WindowRect.X,
                windowParams.WindowRect.Y,
                windowParams.WindowRect.Width,
                windowParams.WindowRect.Height,
                windowParams.Parent,
                IntPtr.Zero,
                Marshal.GetHINSTANCE(typeof(TWindow).Module),
                lpCreateParam);

            TWindow createdWindow = null;
            if (!hwnd.IsInvalid)
            {
                Debug.Assert(_createdWindow != null);
                _createdWindow.TransferHandleOwnership(hwnd);

                createdWindow = _createdWindow;
            }

            _createdWindow = null;
            return createdWindow;
        }

        private IntPtr WndProc(IntPtr hwnd, int message, IntPtr wParam, IntPtr lParam)
        {
            // This window proc is only ever used to receive the first message
            // intended for a window.  Here we create an instance of the real
            // TWindow type. 
            _createdWindow = new TWindow();

            // Pass the parameter for this first message to the new window.
            // It will replace the window proceedure and pass this first
            // message to it.
            return _createdWindow.InitializeFromFirstMessage(hwnd, message, wParam, lParam);
        }

        private bool? _initializationState;
        private WNDCLASSEX _wcex;
        private short _atom;
        
        [ThreadStatic]
        private TWindow _createdWindow;
    }
}
