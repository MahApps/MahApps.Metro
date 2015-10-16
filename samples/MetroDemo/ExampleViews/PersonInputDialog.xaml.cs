using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MetroDemo.ExampleViews
{
    public class Person
    {
        public int Age { get; set; }
        public string Name { get; set; }
    }

    /// <summary>
    /// Interaction logic for PersonInputDialog.xaml
    /// </summary>
    public partial class PersonInputDialog : CustomInputDialog<string> {
        public PersonInputDialog() {
            InitializeComponent();
        }
    }
}
