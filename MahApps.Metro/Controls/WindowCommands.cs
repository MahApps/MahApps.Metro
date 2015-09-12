using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace MahApps.Metro.Controls
{
    public class WindowCommands : ItemsControl, INotifyPropertyChanged
    {
		#region Properties
        
        public static readonly DependencyProperty ThemeProperty =
            DependencyProperty.Register("Theme", typeof(Theme), typeof(WindowCommands),
                                        new PropertyMetadata(
                                            Theme.Light,
                                            OnThemeChanged));

        /// <summary>
        /// Gets or sets the value indicating current theme.
        /// </summary>
        public Theme Theme
        {
            get { return (Theme) GetValue(ThemeProperty); }
            set { SetValue(ThemeProperty, value); }
        }

        public static readonly DependencyProperty LightTemplateProperty =
            DependencyProperty.Register("LightTemplate", typeof(ControlTemplate), typeof(WindowCommands),
                                        new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the value indicating light theme template.
        /// </summary>
        public ControlTemplate LightTemplate
        {
            get { return (ControlTemplate) GetValue(LightTemplateProperty); }
            set { SetValue(LightTemplateProperty, value); }
        }

        public static readonly DependencyProperty DarkTemplateProperty =
            DependencyProperty.Register("DarkTemplate", typeof(ControlTemplate), typeof(WindowCommands),
                                        new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the value indicating light theme template.
        /// </summary>
        public ControlTemplate DarkTemplate
        {
            get { return (ControlTemplate)GetValue(DarkTemplateProperty); }
            set { SetValue(DarkTemplateProperty, value); }
        }

        public static readonly DependencyProperty ShowSeparatorsProperty =
            DependencyProperty.Register("ShowSeparators", typeof(bool), typeof(WindowCommands),
                                        new FrameworkPropertyMetadata(
                                            true,
                                            FrameworkPropertyMetadataOptions.AffectsArrange |
                                            FrameworkPropertyMetadataOptions.AffectsMeasure |
                                            FrameworkPropertyMetadataOptions.AffectsRender,
                                            OnShowSeparatorsChanged));

        /// <summary>
        /// Gets or sets the value indicating whether to show the separators.
        /// </summary>
        public bool ShowSeparators
        {
            get { return (bool) GetValue(ShowSeparatorsProperty); }
            set { SetValue(ShowSeparatorsProperty, value); }
        }

        public static readonly DependencyProperty ShowLastSeparatorProperty =
            DependencyProperty.Register("ShowLastSeparator", typeof(bool), typeof(WindowCommands),
                                        new FrameworkPropertyMetadata(
                                            true,
											FrameworkPropertyMetadataOptions.AffectsArrange |
											FrameworkPropertyMetadataOptions.AffectsMeasure |
											FrameworkPropertyMetadataOptions.AffectsRender,
											OnShowLastSeparatorChanged));

        /// <summary>
        /// Gets or sets the value indicating whether to show the last separator.
        /// </summary>
        public bool ShowLastSeparator
        {
            get { return (bool) GetValue(ShowLastSeparatorProperty); }
            set { SetValue(ShowLastSeparatorProperty, value); }
        }

        public static readonly DependencyProperty SeparatorHeightProperty =
            DependencyProperty.Register("SeparatorHeight", typeof(int), typeof(WindowCommands),
                                        new FrameworkPropertyMetadata(
											15,
											FrameworkPropertyMetadataOptions.AffectsArrange |
											FrameworkPropertyMetadataOptions.AffectsMeasure |
											FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Gets or sets the value indicating separator height.
        /// </summary>
        public int SeparatorHeight
        {
            get { return (int) GetValue(SeparatorHeightProperty); }
            set { SetValue(SeparatorHeightProperty, value); }
        }

        private static void OnThemeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // get object
            var obj = d as WindowCommands;
            if (obj == null) return;
            
            // apply control template
            if ((Theme) e.NewValue == Theme.Light)
            {
                if (obj.LightTemplate != null)
                    obj.SetValue(TemplateProperty, obj.LightTemplate);
            }
            else if ((Theme) e.NewValue == Theme.Dark)
            {
                if (obj.DarkTemplate != null)
                    obj.SetValue(TemplateProperty, obj.DarkTemplate);
            }
        }

        private static void OnShowSeparatorsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as WindowCommands)?.ResetSeparators();
        }

        private static void OnShowLastSeparatorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as WindowCommands)?.ResetSeparators(false);
        }
        
        #endregion

        #region Constructors

        static WindowCommands()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WindowCommands), new FrameworkPropertyMetadata(typeof(WindowCommands)));
        }

        public WindowCommands()
        {
            Loaded += WindowCommands_Loaded;
        }
		
		#endregion
		
		#region Item Handling
		
		protected override DependencyObject GetContainerForItemOverride()
        {
            return new WindowCommandsItem();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is WindowCommandsItem;
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);

            if ((Items.Count > 0) && (ReferenceEquals(item, Items[Items.Count - 1])))
                ResetSeparators(false);
        }

        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);
            ResetSeparators();
        }
        
        private void ResetSeparators(bool reset = true)
        {
            if (reset)
                for (var i = 0; i < Items.Count - 1; i++)
                {
                    var container = ItemContainerGenerator.ContainerFromIndex(i) as WindowCommandsItem;
                    if (container != null)
                        container.IsSeparatorVisible = ShowSeparators;
                }

            var lastContainer = ItemContainerGenerator.ContainerFromIndex(Items.Count - 1) as WindowCommandsItem;
            if (lastContainer != null)
                lastContainer.IsSeparatorVisible = ShowSeparators && ShowLastSeparator;
        }
		
		#endregion
		
		#region ParentWindow Handling
		
        private void WindowCommands_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= WindowCommands_Loaded;
            if (ParentWindow == null)
                ParentWindow = this.TryFindParent<Window>();
        }

        private Window _parentWindow;

        public Window ParentWindow
        {
            get { return _parentWindow; }
            set
            {
                if (Equals(_parentWindow, value)) return;

                _parentWindow = value;
                RaisePropertyChanged("ParentWindow");
            }
        }

        #endregion

        #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
		
		#endregion
    }
	
	[TemplatePart(Name = PART_ContentPresenter, Type = typeof(UIElement))]
    [TemplatePart(Name = PART_Separator, Type = typeof(UIElement))]
    public class WindowCommandsItem : ContentControl
    {
        private const string PART_ContentPresenter = "PART_ContentPresenter";
        private const string PART_Separator = "PART_Separator";
        
        private UIElement _separator;
        private bool _isSeparatorVisible = true;

        public bool IsSeparatorVisible
        {
            get { return _isSeparatorVisible;  }
            set
            {
                if (_isSeparatorVisible == value) return;

                _isSeparatorVisible = value;
                SetSeparatorVisibility();
            }
        }

        private void SetSeparatorVisibility()
        {
            if (_separator != null)
                _separator.Visibility = IsSeparatorVisible ? Visibility.Visible : Visibility.Hidden;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            
            _separator = Template.FindName(PART_Separator, this) as UIElement;
            SetSeparatorVisibility();
        }
    }
}