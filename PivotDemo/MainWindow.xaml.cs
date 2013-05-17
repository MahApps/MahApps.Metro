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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MahApps.Metro.Controls;

namespace PivotDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            List<DataItem> items = new List<DataItem>();
            items.Add(new DataItem { Data = "RonJeremyisthebest" });
            items.Add(new DataItem { Data = "RonJeremyisthebest" });
            items.Add(new DataItem { Data = "RonJeremyisthebest" });
            items.Add(new DataItem { Data = "RonJeremyisthebest" });
            items.Add(new DataItem { Data = "RonJeremyisthebest" });
            items.Add(new DataItem { Data = "RonJeremyisthebest" });
            items.Add(new DataItem { Data = "RonJeremyisthebest" });
            items.Add(new DataItem { Data = "RonJeremyisthebest" });
            items.Add(new DataItem { Data = "RonJeremyisthebest" });
            items.Add(new DataItem { Data = "RonJeremyisthebest" });
            items.Add(new DataItem { Data = "RonJeremyisthebest" }); items.Add(new DataItem { Data = "RonJeremyisthebest" });
            items.Add(new DataItem { Data = "RonJeremyisthebest" });
            items.Add(new DataItem { Data = "RonJeremyisthebest" });
            items.Add(new DataItem { Data = "RonJeremyisthebest" });
            items.Add(new DataItem { Data = "RonJeremyisthebest" });
            items.Add(new DataItem { Data = "RonJeremyisthebest" });
            items.Add(new DataItem { Data = "RonJeremyisthebest" });
            items.Add(new DataItem { Data = "RonJeremyisthebest" });
            items.Add(new DataItem { Data = "RonJeremyisthebest" });
            items.Add(new DataItem { Data = "RonJeremyisthebest" });
            items.Add(new DataItem { Data = "RonJeremyisthebest" });
            items.Add(new DataItem { Data = "RonJeremyisthebest" });
            items.Add(new DataItem { Data = "RonJeremyisthebest" });
            items.Add(new DataItem { Data = "RonJeremyisthebest" }); items.Add(new DataItem { Data = "RonJeremyisthebest" }); items.Add(new DataItem { Data = "RonJeremyisthebest" }); items.Add(new DataItem { Data = "RonJeremyisthebest" });
            items.Add(new DataItem { Data = "RonJeremyisthebest" });
            items.Add(new DataItem { Data = "RonJeremyisthebest" });
            items.Add(new DataItem { Data = "RonJeremyisthebest" }); items.Add(new DataItem { Data = "RonJeremyisthebest" }); items.Add(new DataItem { Data = "RonJeremyisthebest" }); items.Add(new DataItem { Data = "RonJeremyisthebest" }); items.Add(new DataItem { Data = "RonJeremyisthebest" }); items.Add(new DataItem { Data = "RonJeremyisthebest" }); items.Add(new DataItem { Data = "RonJeremyisthebest" });
            

            Items = items;
        }

        public List<DataItem> Items
        {
            get { return (List<DataItem>)GetValue(ItemsProperty); }
            set { SetValue(ItemsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Items.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemsProperty =
            DependencyProperty.Register("Items", typeof(List<DataItem>), typeof(MainWindow), new PropertyMetadata(null));

        
    }

    public class DataItem
    {
        public string Data { get; set; }
    }
}
