using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace MahApps.Metro.Controls
{
    [TemplatePart (Name = "PART_ColorPalettes", Type = typeof(ItemsControl))]
    public class ColorPicker : ColorPickerBase
    {
        public ColorPicker()
        {
            ColorPalettes.CollectionChanged += ColorPalettes_CollectionChanged;
        }
        
        static ColorPicker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ColorPicker), new FrameworkPropertyMetadata(typeof(ColorPicker)));
        }

        ItemsControl PART_ColorPalettes;

        // Using a DependencyProperty as the backing store for SelectedColorTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedColorTemplateProperty = DependencyProperty.Register(nameof(SelectedColorTemplate), typeof(DataTemplate), typeof(ColorPicker), new PropertyMetadata(null));

        public DataTemplate SelectedColorTemplate
        {
            get { return (DataTemplate)GetValue(SelectedColorTemplateProperty); }
            set { SetValue(SelectedColorTemplateProperty, value); }
        }


        /// <summary>Identifies the <see cref="MaxDropDownHeight"/> dependency property.</summary>
        public static readonly DependencyProperty MaxDropDownHeightProperty = DependencyProperty.Register(nameof(MaxDropDownHeight), typeof(double), typeof(ColorPicker), new FrameworkPropertyMetadata(SystemParameters.PrimaryScreenHeight / 3));

        /// <summary>Identifies the <see cref="DropDownHeight"/> dependency property.</summary>
        public static readonly DependencyProperty DropDownHeightProperty = DependencyProperty.Register(nameof(DropDownHeight), typeof(double), typeof(ColorPicker), new PropertyMetadata(300d));

        /// <summary>Identifies the <see cref="MaxDropDownWidth"/> dependency property.</summary>
        public static readonly DependencyProperty MaxDropDownWidthProperty = DependencyProperty.Register(nameof(MaxDropDownWidth), typeof(double), typeof(ColorPicker), new PropertyMetadata(MaxWidthProperty.DefaultMetadata.DefaultValue));

        /// <summary>Identifies the <see cref="DropDownWidth"/> dependency property.</summary>
        public static readonly DependencyProperty DropDownWidthProperty = DependencyProperty.Register(nameof(DropDownWidth), typeof(double), typeof(ColorPicker), new PropertyMetadata(300d));




        /// <summary>
        ///     The maximum height of the popup
        /// </summary>
        [Bindable(true), Category("Layout")]
        [TypeConverter(typeof(LengthConverter))]
        public double MaxDropDownHeight
        {
            get { return (double)GetValue(MaxDropDownHeightProperty); }
            set { SetValue(MaxDropDownHeightProperty, value); }
        }

        /// <summary>
        /// The width of the DropDown
        /// </summary>
        public double DropDownHeight
        {
            get { return (double)GetValue(DropDownHeightProperty); }
            set { SetValue(DropDownHeightProperty, value); }
        }

        /// <summary>
        /// The maximum width of the DropDown
        /// </summary>
        public double MaxDropDownWidth
        {
            get { return (double)GetValue(MaxDropDownWidthProperty); }
            set { SetValue(MaxDropDownWidthProperty, value); }
        }

        /// <summary>
        /// The width of the DropDown
        /// </summary>
        [Bindable(true), Category("Layout")]
        [TypeConverter(typeof(LengthConverter))]
        public double DropDownWidth
        {
            get { return (double)GetValue(DropDownWidthProperty); }
            set { SetValue(DropDownWidthProperty, value); }
        }





        /// <summary>
        /// DependencyProperty for IsDropDownOpen
        /// </summary>
        public static readonly DependencyProperty IsDropDownOpenProperty = DependencyProperty.Register( "IsDropDownOpen", typeof(bool), typeof(ColorPicker), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnIsDropDownOpenChanged));

        /// <summary>
        /// Whether or not the "popup" for this control is currently open
        /// </summary>
        [Bindable(true), Browsable(false), Category("Appearance")]
        public bool IsDropDownOpen
        {
            get { return (bool)GetValue(IsDropDownOpenProperty); }
            set { SetValue(IsDropDownOpenProperty, value); }
        }

        public static readonly RoutedEvent DropDownOpenedEvent = EventManager.RegisterRoutedEvent(
                                                                        nameof(DropDownOpened),
                                                                        RoutingStrategy.Bubble,
                                                                        typeof(EventHandler<EventArgs>),
                                                                        typeof(ColorPicker));

        public static readonly RoutedEvent DropDownClosedEvent = EventManager.RegisterRoutedEvent(
                                                                nameof(DropDownClosed),
                                                                RoutingStrategy.Bubble,
                                                                typeof(EventHandler<EventArgs>),
                                                                typeof(ColorPicker));

        /// <summary>
        ///     Occurs when the DropDown is opened.
        /// </summary>
        public event EventHandler<EventArgs> DropDownOpened
        {
            add { AddHandler(DropDownOpenedEvent, value); }
            remove { RemoveHandler(DropDownOpenedEvent, value); }
        }

        /// <summary>
        ///     Occurs when the DropDown is closed.
        /// </summary>
        public event EventHandler<EventArgs> DropDownClosed
        {
            add { AddHandler(DropDownClosedEvent, value); }
            remove { RemoveHandler(DropDownClosedEvent, value); }
        }

        private static void OnIsDropDownOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ColorPicker colorPicker)
            {
                if ((bool)e.NewValue)
                {
                    colorPicker.RaiseEvent(new RoutedEventArgs(DropDownOpenedEvent));
                }
                else
                {
                    colorPicker.RaiseEvent(new RoutedEventArgs(DropDownClosedEvent));
                }
            }
        }

        public override void OnApplyTemplate()
        {
            PART_ColorPalettes = (ItemsControl)this.GetTemplateChild(nameof(PART_ColorPalettes));
            base.OnApplyTemplate();
        }


        public ObservableCollection<ColorPalette> ColorPalettes { get; } = new ObservableCollection<ColorPalette>();

        private void ColorPalettes_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    foreach (ColorPalette item in e.NewItems)
                    {
                        System.Windows.Data.Binding binding = new System.Windows.Data.Binding(nameof(SelectedColor))
                        {
                            Mode = System.Windows.Data.BindingMode.TwoWay,
                            ValidatesOnExceptions = true,
                            Source = this
                        };

                        item.SetBinding(ColorPalette.SelectedValueProperty, binding);
                    }
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Move:
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                    break;
                default:
                    break;
            }
        }
    }
}
