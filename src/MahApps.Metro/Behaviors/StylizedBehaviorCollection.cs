using System.Windows;
using Microsoft.Xaml.Behaviors;

namespace MahApps.Metro.Behaviors
{
    public class StylizedBehaviorCollection : FreezableCollection<Behavior>
    {
        protected override Freezable CreateInstanceCore()
        {
            return new StylizedBehaviorCollection();
        }
    }
}