using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using JetBrains.Annotations;
using MahApps.Metro.Controls.Dialogs;

namespace MahApps.Metro.Controls.AutomationPeers
{
    public class MetroWindowAutomationPeer : WindowAutomationPeer, IWindowProvider
    {
        [NotNull]
        private readonly MetroWindow _owner;

        public MetroWindowAutomationPeer([NotNull] MetroWindow owner)
            : base(owner)
        {
            this._owner = owner;
        }

        public override object GetPattern(PatternInterface patternInterface)
        {
            if (patternInterface == PatternInterface.Window)
            {
                return this;
            }

            return base.GetPattern(patternInterface);
        }

        /// <summary>
        /// Changes the visual state of the window. For example, minimizes or maximizes it.
        /// </summary>
        /// <param name="state">The requested visual state of the window.</param>
        /// <exception cref="T:System.InvalidOperationException">When the control does not support the requested behavior.</exception>
        public void SetVisualState(WindowVisualState state)
        {
            WindowState windowState;
            switch (state)
            {
                case WindowVisualState.Maximized:
                    if (!this.Maximizable)
                    {
                        throw new InvalidOperationException("Maximizing the window is currently not allowed");
                    }

                    windowState = WindowState.Maximized;
                    break;
                case WindowVisualState.Minimized:
                    if (!this.Maximizable)
                    {
                        throw new InvalidOperationException("Minimizing the window is currently not allowed");
                    }

                    windowState = WindowState.Minimized;
                    break;
                case WindowVisualState.Normal:
                    windowState = WindowState.Normal;
                    break;
                default:
                    throw new ArgumentException($"Value '{state}' is not supported", nameof(state));
            }

            this._owner.WindowState = windowState;
        }

        public void Close()
        {
            this._owner.Close();
        }

        public bool WaitForInputIdle(int milliseconds)
        {
            return false;
        }

        public bool Maximizable => this._owner.ResizeMode == ResizeMode.CanResize || this._owner.ResizeMode == ResizeMode.CanResizeWithGrip;

        public bool Minimizable => this._owner.ResizeMode == ResizeMode.CanMinimize;

        public bool IsModal => this._owner.Topmost;

        public WindowVisualState VisualState
        {
            get
            {
                var windowState = this._owner.WindowState;
                switch (windowState)
                {
                        case WindowState.Maximized:
                            return WindowVisualState.Maximized;
                        case WindowState.Minimized:
                        return WindowVisualState.Minimized;
                    case WindowState.Normal:
                        return WindowVisualState.Normal;
                    default:
                        throw new InvalidOperationException($"Can not map WindowsState '{windowState}' to the corresponding WindowVisualState");
                }
            }
        }

        public WindowInteractionState InteractionState
        {
            get
            {
                if (this._owner.HasDialog().Result)
                {
                    return  WindowInteractionState.BlockedByModalWindow;
                }

                return WindowInteractionState.ReadyForUserInteraction;
            }
        }

        public bool IsTopmost => this._owner.Topmost;
    }
}
