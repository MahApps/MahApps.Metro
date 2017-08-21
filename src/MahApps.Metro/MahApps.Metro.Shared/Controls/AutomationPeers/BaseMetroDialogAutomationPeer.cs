using System;
using System.Security;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using JetBrains.Annotations;
using MahApps.Metro.Controls.Dialogs;

namespace MahApps.Metro.Controls.AutomationPeers
{
    public class BaseMetroDialogAutomationPeer : FrameworkElementAutomationPeer, IWindowProvider
    {
        [NotNull]
        private readonly BaseMetroDialog _owner;
        private DialogState _state;

        private enum DialogState
        {
            IsLoaded,
            Closed,
            Closing
        }

        public BaseMetroDialogAutomationPeer([NotNull] BaseMetroDialog owner)
            : base(owner)
        {
            if (owner == null)
            {
                throw new ArgumentNullException(nameof(owner));
            }

            this._owner = owner;
            this._owner.Loaded += this.DialogOnLoaded;
            this._owner.Unloaded += this.DialogOnUnloaded;
        }


        private void DialogOnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            this._state = DialogState.IsLoaded;
        }

        private void DialogOnUnloaded(object sender, RoutedEventArgs routedEventArgs)
        {
            this._state = DialogState.Closed;
            this._owner.Unloaded -= this.DialogOnUnloaded;
            this._owner.Loaded -= this.DialogOnLoaded;
        }

        /// <summary>
        /// Gets the name of the <see cref="BaseMetroDialog" /> that is associated with this <see cref="BaseMetroDialogAutomationPeer" />.
        /// Called by <see cref="M:System.Windows.Automation.Peers.AutomationPeer.GetClassName" />.
        /// </summary>
        /// <returns>A string that contains the word "Window".</returns>
        protected override string GetClassNameCore()
        {
            return "Window";
        }

        /// <summary>
        /// Gets the control type for the <see cref="BaseMetroDialog" /> that is associated with this <see cref="BaseMetroDialogAutomationPeer" />.
        /// Called by <see cref="M:System.Windows.Automation.Peers.AutomationPeer.GetAutomationControlType" />.
        /// </summary>
        /// <returns>The <see cref="F:System.Windows.Automation.Peers.AutomationControlType.Window" /> enumeration value.</returns>
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Window;
        }

        /// <summary>
        /// Gets the title of the <see cref="BaseMetroDialog" /> that is associated with this <see cref="BaseMetroDialogAutomationPeer" />.
        /// Called by <see cref="M:System.Windows.Automation.Peers.AutomationPeer.GetName" />.
        /// </summary>
        /// <returns>A string that contains the <see cref="BaseMetroDialog.Title" /> of the <see cref="BaseMetroDialog" />, or <see cref="F:System.String.Empty" /> if the title is null.</returns>
        [SecurityCritical]
        [SecuritySafeCritical]
        protected override string GetNameCore()
        {
            return this._owner.Title ?? string.Empty;
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
            throw new InvalidOperationException("The dialog does not support changing its visual state");
        }

        /// <summary>
        /// Attempts to close the window.
        /// </summary>
        /// <exception cref="T:System.InvalidOperationException">When the control is unable to perform the requested action.</exception>
        /// <exception cref="T:System.Windows.Automation.ElementNotAvailableException">When the target element is no longer available (for example, the window has closed).</exception>
        public void Close()
        {
            if (this._state == DialogState.Closed)
            {
                throw new ElementNotAvailableException("The dialog is already closed");
            }

            if (this._state == DialogState.Closing)
            {
                throw new InvalidOperationException("The dialog is already closing");
            }

            var owner = this._owner;
            owner.Dispatcher.Invoke(new Action(() => owner.RequestCloseAsync()));
        }

        /// <summary>
        /// Causes the calling code to block for the specified time or until the associated process enters an idle state, whichever completes first.
        /// </summary>
        /// <param name="milliseconds">The amount of time, in milliseconds, to wait for the associated process to become idle. The maximum is <see cref="F:System.Int32.MaxValue" />.</param>
        /// <returns>true if the window has entered the idle state; false if the timeout occurred.</returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException">When the parameter passed in is not a valid number.</exception>
        public bool WaitForInputIdle(int milliseconds) => false;

        public bool Maximizable => false;

        public bool Minimizable => false;

        public bool IsModal => true;

        public WindowVisualState VisualState => WindowVisualState.Normal;

        public WindowInteractionState InteractionState { get; }

        public bool IsTopmost => false;
    }
}
