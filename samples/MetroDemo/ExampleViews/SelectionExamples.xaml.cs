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

            this.DataContextChanged += (sender, args) => {
                                           var vm = args.NewValue as MainWindowViewModel;
                                           if (vm != null)
                                           {
                                               CollectionViewSource.GetDefaultView(vm.Albums).GroupDescriptions.Clear();
                                               CollectionViewSource.GetDefaultView(vm.Albums).GroupDescriptions.Add(new PropertyGroupDescription("Artist"));
                                           }
                                       };
        }
    }
}
