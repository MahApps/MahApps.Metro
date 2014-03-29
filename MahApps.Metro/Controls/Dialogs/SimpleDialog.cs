using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Markup;

namespace MahApps.Metro.Controls.Dialogs
{
    /// <summary>
    /// A simple implementation of BaseMetroDialog allowing arbitrary content.
    /// </summary>
    [ContentProperty("DialogBody")]
    public class SimpleDialog : BaseMetroDialog
    {
        public SimpleDialog()
        {
            this.Loaded += SimpleDialog_Loaded;
        }

        void SimpleDialog_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (DialogBody_ContentPresenter.Content != null) //temp fix for #1238. it forces bindings to bind since for some reason, they won't when the dialog is shown.
                ((FrameworkElement)DialogBody_ContentPresenter.Content).InvalidateProperty(FrameworkElement.DataContextProperty);
        }
    }
}
