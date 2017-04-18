namespace MahApps.Metro.Converters
{
    using System.Windows;
    using System.Windows.Controls;
    using MahApps.Metro.Controls.Dialogs;

    public class IconTemplateSelector : DataTemplateSelector
    {
        public IconTemplateSelector()
        {
            None = new DataTemplate();
        }

        public DataTemplate Error { get; set; }
        public DataTemplate Hand { get; set; }
        public DataTemplate Question { get; set; }
        public DataTemplate Warning { get; set; }
        public DataTemplate Information { get; set; }
        private DataTemplate None { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is MessageDialogIcon)
            {
                var dialogIcon = (MessageDialogIcon)item;
                switch (dialogIcon)
                {
                    case MessageDialogIcon.Error:
                        return Error;
                    case MessageDialogIcon.Hand:
                        return Hand;
                    case MessageDialogIcon.Information:
                        return Information;
                    case MessageDialogIcon.Question:
                        return Question;
                    case MessageDialogIcon.Warning:
                        return Warning;
                    case MessageDialogIcon.None:
                        return None;
                }
            }
            return ((ContentPresenter)container).ContentTemplate;
        }
    }
}
