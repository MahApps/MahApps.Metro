using System.Windows;
using System.Windows.Controls;
using ControlzEx;

namespace MahApps.Metro.Controls
{
    [TemplatePart(Name = PART_ContentPresenter, Type = typeof(UIElement))]
    [TemplatePart(Name = PART_Separator, Type = typeof(UIElement))]
    public class WindowCommandsItem : ContentControl
    {
        private const string PART_ContentPresenter = "PART_ContentPresenter";
        private const string PART_Separator = "PART_Separator";

        internal PropertyChangeNotifier VisibilityPropertyChangeNotifier { get; set; }

        /// <summary>Identifies the <see cref="IsSeparatorVisible"/> dependency property.</summary>
        public static readonly DependencyProperty IsSeparatorVisibleProperty =
            DependencyProperty.Register(nameof(IsSeparatorVisible),
                                        typeof(bool),
                                        typeof(WindowCommandsItem),
                                        new FrameworkPropertyMetadata(true,
                                                                      FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Gets or sets the value indicating whether to show the separator.
        /// </summary>
        public bool IsSeparatorVisible
        {
            get => (bool)this.GetValue(IsSeparatorVisibleProperty);
            set => this.SetValue(IsSeparatorVisibleProperty, value);
        }

        /// <summary>Identifies the <see cref="ParentWindowCommands"/> dependency property.</summary>
        public static readonly DependencyPropertyKey ParentWindowCommandsPropertyKey =
            DependencyProperty.RegisterReadOnly(nameof(ParentWindowCommands),
                                                typeof(WindowCommands),
                                                typeof(WindowCommandsItem),
                                                new PropertyMetadata(null));

        /// <summary>Identifies the <see cref="ParentWindowCommands"/> dependency property.</summary>
        public static readonly DependencyProperty ParentWindowCommandsProperty = ParentWindowCommandsPropertyKey.DependencyProperty;

        public WindowCommands ParentWindowCommands
        {
            get => (WindowCommands)this.GetValue(ParentWindowCommandsProperty);
            protected set => this.SetValue(ParentWindowCommandsPropertyKey, value);
        }

        static WindowCommandsItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WindowCommandsItem), new FrameworkPropertyMetadata(typeof(WindowCommandsItem)));
        }

        /// <inheritdoc />
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var windowCommands = ItemsControl.ItemsControlFromItemContainer(this) as WindowCommands;
            this.SetValue(ParentWindowCommandsPropertyKey, windowCommands);
        }
    }
}