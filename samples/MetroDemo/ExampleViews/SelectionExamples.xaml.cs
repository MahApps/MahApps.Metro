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
    /// <summary>
    /// Interaction logic for SelectionExamples.xaml
    /// </summary>
    public partial class SelectionExamples : UserControl
    {
        public SelectionExamples()
        {
            InitializeComponent();

            this.Loaded += (sender, args) => {
                               CollectionViewSource.GetDefaultView(groupingComboBox.ItemsSource).GroupDescriptions.Clear();
                               CollectionViewSource.GetDefaultView(groupingComboBox.ItemsSource).GroupDescriptions.Add(new PropertyGroupDescription("Artist"));
                           };
        }
    }
}
