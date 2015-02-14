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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MahApps.Metro.Controls
{
    internal partial class ResizerWindow : Window
    {
        public ResizerWindow(MetroWindow window)
        {
            InitializeComponent();
            this.Owner = window;
            this.ShowStoryboard = this.TryFindResource("ShowStoryboard") as Storyboard;
            this.HideStoryboard = this.TryFindResource("HideStoryboard") as Storyboard;
        }

        public Storyboard ShowStoryboard { get; set; }

        public Storyboard HideStoryboard { get; set; }
    }
}