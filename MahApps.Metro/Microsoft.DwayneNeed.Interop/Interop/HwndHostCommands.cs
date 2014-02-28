using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Microsoft.DwayneNeed.Input;

namespace Microsoft.DwayneNeed.Interop
{
    public static class HwndHostCommands
    {
        public static RoutedCommand<MouseActivateParameter> MouseActivate = new RoutedCommand<MouseActivateParameter>("MouseActivate", typeof(HwndHostCommands));
    }
}
