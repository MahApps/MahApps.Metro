using System.Collections.Generic;
using MahApps.Metro.Controls;
using MetroDemo.Models;

namespace MetroDemo
{
    /// <summary>
    /// Interaction logic for CustomFlyout.xaml
    /// </summary>
    public partial class CustomFlyout : Flyout
    {
        public List<Artist> Artists { get; set; }

        public CustomFlyout()
        {
            Artists = SampleData.Artists;
            InitializeComponent();
        }
    }
}
