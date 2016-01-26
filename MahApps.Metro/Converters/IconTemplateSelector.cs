namespace MahApps.Metro.Converters
{
    using System.Windows;
    using System.Windows.Controls;
    using MahApps.Metro.Controls.Dialogs;

    public class IconTemplateSelector : DataTemplateSelector
    {
        public DataTemplate Error { get; set; }
        public DataTemplate Hand { get; set; }
        public DataTemplate Question { get; set; }
        public DataTemplate Warning { get; set; }
        public DataTemplate Information { get; set; }

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
                }
            }
            return ((ContentPresenter)container).ContentTemplate;
        }
    }
}
