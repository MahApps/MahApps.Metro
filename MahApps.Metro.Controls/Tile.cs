using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace MahApps.Metro.Controls
{
    public class Tile : Button
    {
        public Tile()
        {
            DefaultStyleKey = typeof(Tile);
            Loaded += TileLoaded;
        }

        void TileLoaded(object sender, RoutedEventArgs e)
        {
            var behaviours = Interaction.GetBehaviors(this);
            behaviours.Add(new TiltBehavior { TiltFactor = 5 });
        }

        #region public string Title
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(Tile), new PropertyMetadata(default(string)));

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }
        #endregion

        #region public string Count
        public static readonly DependencyProperty CountProperty =
            DependencyProperty.Register("Count", typeof(string), typeof(Tile), new PropertyMetadata(default(string)));

        public string Count
        {
            get { return (string)GetValue(CountProperty); }
            set { SetValue(CountProperty, value); }
        }
        #endregion
    }
}