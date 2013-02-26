using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MahApps.Metro.Models.Win32;

namespace MahApps.Metro.Controls
{
	partial class GlowWindow : Window
	{
		private Window owner;
		private double glowSize = 9.0;
		private double edgeSize = 20.0;
		private IntPtr handle;
		private IntPtr ownerHandle;
		private Func<double> getLeft;
		private Func<double> getTop;
		private Func<double> getWidth;
		private Func<double> getHeight;
		private Func<Point, HitTestValues> getHitTestValue;
		private Func<Point, Cursor> getCursor;

		public GlowWindow(Window owner, GlowDirection direction)
		{
			InitializeComponent();

			this.owner = owner;
			this.glow.Visibility = Visibility.Collapsed;
            
            Binding b = new Binding("GlowBrush.Color");
		    b.Source = owner;
		    glow.SetBinding(Glow.GlowColorProperty, b);

			switch (direction)
			{
				case GlowDirection.Left:
					this.glow.Orientation = Orientation.Vertical;
					this.glow.HorizontalAlignment = HorizontalAlignment.Right;
					this.getLeft = () => owner.Left - this.glowSize;
					this.getTop = () => owner.Top - this.glowSize;
					this.getWidth = () => this.glowSize;
					this.getHeight = () => owner.ActualHeight + this.glowSize * 2;
					this.getHitTestValue = p => new Rect(0, 0, this.ActualWidth, edgeSize).Contains(p)
						? HitTestValues.HTTOPLEFT
						: new Rect(0, this.ActualHeight - edgeSize, this.ActualWidth, edgeSize).Contains(p)
							? HitTestValues.HTBOTTOMLEFT
							: HitTestValues.HTLEFT;
					this.getCursor = p => new Rect(0, 0, this.ActualWidth, edgeSize).Contains(p)
						? Cursors.SizeNWSE
						: new Rect(0, this.ActualHeight - edgeSize, this.ActualWidth, edgeSize).Contains(p)
							? Cursors.SizeNESW
							: Cursors.SizeWE;
					break;
				case GlowDirection.Right:
					this.glow.Orientation = Orientation.Vertical;
					this.glow.HorizontalAlignment = HorizontalAlignment.Left;
					this.getLeft = () => owner.Left + owner.ActualWidth;
					this.getTop = () => owner.Top - this.glowSize;
					this.getWidth = () => this.glowSize;
					this.getHeight = () => owner.ActualHeight + this.glowSize * 2;
					this.getHitTestValue = p => new Rect(0, 0, this.ActualWidth, edgeSize).Contains(p)
						? HitTestValues.HTTOPRIGHT
						: new Rect(0, this.ActualHeight - edgeSize, this.ActualWidth, edgeSize).Contains(p)
							? HitTestValues.HTBOTTOMRIGHT
							: HitTestValues.HTRIGHT;
					this.getCursor = p => new Rect(0, 0, this.ActualWidth, edgeSize).Contains(p)
						? Cursors.SizeNESW
						: new Rect(0, this.ActualHeight - edgeSize, this.ActualWidth, edgeSize).Contains(p)
							? Cursors.SizeNWSE
							: Cursors.SizeWE;
					break;
				case GlowDirection.Top:
					this.glow.Orientation = Orientation.Horizontal;
					this.glow.VerticalAlignment = VerticalAlignment.Bottom;
					this.getLeft = () => owner.Left;
					this.getTop = () => owner.Top - this.glowSize;
					this.getWidth = () => owner.ActualWidth;
					this.getHeight = () => this.glowSize;
					this.getHitTestValue = p => new Rect(0, 0, edgeSize - glowSize, this.ActualHeight).Contains(p)
						? HitTestValues.HTTOPLEFT
						: new Rect(this.Width - edgeSize + glowSize, 0, edgeSize - glowSize, this.ActualHeight).Contains(p)
							? HitTestValues.HTTOPRIGHT
							: HitTestValues.HTTOP;
					this.getCursor = p => new Rect(0, 0, edgeSize - glowSize, this.ActualHeight).Contains(p)
						? Cursors.SizeNWSE
						: new Rect(this.Width - edgeSize + glowSize, 0, edgeSize - glowSize, this.ActualHeight).Contains(p)
							? Cursors.SizeNESW
							: Cursors.SizeNS;
					break;
				case GlowDirection.Bottom:
					this.glow.Orientation = Orientation.Horizontal;
					this.glow.VerticalAlignment = VerticalAlignment.Top;
					this.getLeft = () => owner.Left;
					this.getTop = () => owner.Top + owner.ActualHeight;
					this.getWidth = () => owner.ActualWidth;
					this.getHeight = () => this.glowSize;
					this.getHitTestValue = p => new Rect(0, 0, edgeSize - glowSize, this.ActualHeight).Contains(p)
						? HitTestValues.HTBOTTOMLEFT
						: new Rect(this.Width - edgeSize + glowSize, 0, edgeSize - glowSize, this.ActualHeight).Contains(p)
							? HitTestValues.HTBOTTOMRIGHT
							: HitTestValues.HTBOTTOM;
					this.getCursor = p => new Rect(0, 0, edgeSize - glowSize, this.ActualHeight).Contains(p)
						? Cursors.SizeNESW
						: new Rect(this.Width - edgeSize + glowSize, 0, edgeSize - glowSize, this.ActualHeight).Contains(p)
							? Cursors.SizeNWSE
							: Cursors.SizeNS;
					break;
			}

			owner.ContentRendered += (sender, e) => this.glow.Visibility = Visibility.Visible;
			owner.Activated += (sender, e) => this.Update();
			owner.Activated += (sender, e) => this.glow.IsGlow = true;
			owner.Deactivated += (sender, e) => this.glow.IsGlow = false;
			owner.LocationChanged += (sender, e) => this.Update();
			owner.SizeChanged += (sender, e) => this.Update();
			owner.StateChanged += (sender, e) => this.Update();
			owner.Closed += (sender, e) => this.Close();
		}

		protected override void OnSourceInitialized(EventArgs e)
		{
			base.OnSourceInitialized(e);

			var source = (HwndSource)HwndSource.FromVisual(this);
			var ws = source.Handle.GetWindowLong();
			var wsex = source.Handle.GetWindowLongEx();

			//ws |= WS.POPUP;
			wsex ^= WSEX.APPWINDOW;
			wsex |= WSEX.NOACTIVATE;

			source.Handle.SetWindowLong(ws);
			source.Handle.SetWindowLongEx(wsex);
			source.AddHook(this.WndProc);

			this.handle = source.Handle;
		}

		public void Update()
		{
			if (owner.WindowState == WindowState.Normal)
			{
				this.Visibility = Visibility.Visible;

				this.UpdateCore();
			}
            else if (owner.Visibility == Visibility.Hidden)
            {
                this.Visibility = Visibility.Hidden;

                this.UpdateCore();
            }
			else
			{
				this.Visibility = Visibility.Collapsed;
			}
		}

		private void UpdateCore()
		{
			if (this.ownerHandle == IntPtr.Zero)
			{
				this.ownerHandle = new WindowInteropHelper(this.owner).Handle;
			}

			NativeMethods.SetWindowPos(
				this.handle,
				this.ownerHandle,
				(int)this.getLeft(),
				(int)this.getTop(),
				(int)this.getWidth(),
				(int)this.getHeight(),
				SWP.NOACTIVATE);
		}

		private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
		{
			if (msg == (int)WM.MOUSEACTIVATE)
			{
				handled = true;
				return new IntPtr(3);
			}

			if (msg == (int)WM.LBUTTONDOWN)
			{
				var pt = new Point((int)lParam & 0xFFFF, ((int)lParam >> 16) & 0xFFFF);

				NativeMethods.PostMessage(this.ownerHandle, (uint)WM.NCLBUTTONDOWN, (IntPtr)this.getHitTestValue(pt), IntPtr.Zero);
			}
			if (msg == (int)WM.NCHITTEST)
			{
				var ptScreen = new Point((int)lParam & 0xFFFF, ((int)lParam >> 16) & 0xFFFF);
				var ptClient = this.PointFromScreen(ptScreen);
				var cursor = this.getCursor(ptClient);
				if (cursor != this.Cursor) this.Cursor = cursor;
			}

			return IntPtr.Zero;
		}
	}
}
