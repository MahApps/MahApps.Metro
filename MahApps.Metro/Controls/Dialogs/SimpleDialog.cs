using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;

namespace MahApps.Metro.Controls.Dialogs
{
    /// <summary>
    /// A simple implementation of BaseMetroDialog allowing arbitrary content.
    /// </summary>
    [ContentProperty("DialogBody")]
    public class SimpleDialog: BaseMetroDialog
    {
        public SimpleDialog()
        {
            this.Loaded += SimpleDialog_Loaded;
        }

        void SimpleDialog_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
           
        }
    }
}
