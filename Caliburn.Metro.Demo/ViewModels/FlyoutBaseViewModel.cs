namespace Caliburn.Metro.Demo.ViewModels
{
    using Caliburn.Micro;

    using MahApps.Metro.Controls;

    public abstract class FlyoutBaseViewModel : PropertyChangedBase
    {
        #region Fields

        private string header;

        private bool isOpen;

        private Position position;

        #endregion

        #region Public Properties

        public string Header
        {
            get
            {
                return this.header;
            }

            set
            {
                if (value == this.header)
                {
                    return;
                }

                this.header = value;
                this.NotifyOfPropertyChange();
            }
        }

        public bool IsOpen
        {
            get
            {
                return this.isOpen;
            }

            set
            {
                if (value.Equals(this.isOpen))
                {
                    return;
                }

                this.isOpen = value;
                this.NotifyOfPropertyChange();
            }
        }

        public Position Position
        {
            get
            {
                return this.position;
            }

            set
            {
                if (value == this.position)
                {
                    return;
                }

                this.position = value;
                this.NotifyOfPropertyChange();
            }
        }

        #endregion
    }
}