using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows;

namespace Microsoft.DwayneNeed.Input
{
    /// <summary>
    ///     A simple generic version of RoutedCommand that provides new
    ///     strongly typed methods to support command parameters of type T.
    /// </summary>
    public class RoutedCommand<T> : RoutedCommand
    {
        public RoutedCommand() { }
        public RoutedCommand(string name, Type ownerType) : base(name, ownerType) { }
        public RoutedCommand(string name, Type ownerType, InputGestureCollection inputGestures) : base(name, ownerType, inputGestures) { }

        /// <summary>
        ///     Determines whether this command can execute on the specified
        ///     target.
        /// </summary>
        public bool CanExecute(T parameter, IInputElement target)
        {
            return base.CanExecute(parameter, target);
        }

        /// <summary>
        ///     Executes the command on the specified target.
        /// </summary>
        public void Execute(T parameter, IInputElement target)
        {
            base.Execute(parameter, target);
        }
    }
}
