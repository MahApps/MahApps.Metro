namespace MahApps.Metro.Behaviours
{
    using System.Windows;
    using System.Windows.Interactivity;

    public class StylizedBehaviorCollection : FreezableCollection<Behavior>
    {
        #region Methods

        protected override Freezable CreateInstanceCore()
        {
            return new StylizedBehaviorCollection();
        }

        #endregion
    }
}