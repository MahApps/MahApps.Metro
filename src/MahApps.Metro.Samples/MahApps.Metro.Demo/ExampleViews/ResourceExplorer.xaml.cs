using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;
using MahApps.Metro.IconPacks;

namespace MetroDemo.ExampleViews
{
    public partial class ResourceExplorer
    {
        public ResourceExplorer()
        {
            InitializeComponent();
        }
        private void TextBox_SelectAll(object sender, RoutedEventArgs e)
        {
            if (!(sender is TextBox textBox))
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                return;
            }

            if (textBox.SelectionStart == 0 && textBox.SelectionLength == textBox.Text.Length)
            {
                return;
            }

            textBox.SelectAll();
        }
    }
    public class Resources : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        void Set<T>(T value, ref T field, [CallerMemberName] string propertyName = "")
        {
            if (!Equals(value, field))
            {
                field = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private List<ResourceList> items;
        private List<ResourceList> filteredItems;
        private string filter;

        public List<ResourceList> Items { get => items; private set => Set(value, ref items); }
        public List<ResourceList> FilteredItems { get => filteredItems; set => Set(value, ref filteredItems); }
        public string Filter
        {
            get => filter;
            set
            {
                Set(value, ref filter);
                FilteredItems = FilterResources();
            }
        }

        public Resources() : this(App.Current.Resources) { }
        public Resources(ResourceDictionary dictionary)
        {
            FillItems(dictionary);
            Items.Sort((a, b) => string.CompareOrdinal(a.Header, b.Header));
            foreach (var item in Items)
            {
                item.Sort((a, b) => string.CompareOrdinal(a.Key.ToString(), b.Key.ToString()));
            }
            FilteredItems = Items;
        }

        void FillItems(ResourceDictionary dictionary)
        {
            if (Items == null)
            {
                Items = new List<ResourceList>();
            }

            foreach (var key in dictionary.Keys)
            {
                AddItem(dictionary[key], key);
            }
            if (dictionary.MergedDictionaries == null)
            {
                return;
            }

            foreach (var dict in dictionary.MergedDictionaries)
            {
                FillItems(dict);
            }
        }
        void AddItem(object item, object key)
        {
            var type = GetItemType(item);
            var list = Items.FirstOrDefault(a => a.Type == type);
            if (list == null)
            {
                list = new ResourceList(type.Name, type);
                Items.Add(list);
            }
            if (list.Any(a => a.Key.Equals(key)))
            {
                return;
            }

            list.Add(new ResourceItem(item, key));
        }
        Type GetItemType(object item)
        {
            if (item is IValueConverter)
            {
                return CreateList<IValueConverter>("Converters");
            }

            if (item is IMultiValueConverter)
            {
                return CreateList<IValueConverter>("Converters");
            }

            if (item is UIElement)
            {
                return CreateList<UIElement>("UI Elements");
            }

            if (item is Geometry)
            {
                return CreateList<Geometry>("Geometries");
            }

            if (item is EasingFunctionBase)
            {
                return CreateList<EasingFunctionBase>("Easing Functions");
            }

            if (item is Timeline)
            {
                return CreateList<Timeline>("Timelines");
            }

            if (item is Effect)
            {
                return CreateList<Effect>("Effects");
            }

            return CreateList(item.GetType(), item.GetType().Name);
        }
        Type CreateList<T>(string header)
        {
            return CreateList(typeof(T), header);
        }
        Type CreateList(Type type, string header)
        {
            if (!Items.Any(a => a.Type == type))
            {
                Items.Add(new ResourceList(header, type));
            }
            return type;
        }
        List<ResourceList> FilterResources()
        {
            if (string.IsNullOrWhiteSpace(filter)) return Items;
            return Items.Where(a => a.Any(r => r.Display.IndexOf(filter, StringComparison.OrdinalIgnoreCase) > -1))
                        .Select(a => new ResourceList(a.Header, a.Type, a.Items.Where(r => r.Display.IndexOf(filter, StringComparison.OrdinalIgnoreCase) > -1))).ToList();
        }
    }
    public class ResourceItem
    {
        public Type Type { get; private set; }
        public object Key { get; private set; }
        public object Value { get; private set; }
        public string Display { get; private set; }
        public string XAMLKey
        {
            get
            {
                if (Key is string s)
                {
                    return s;
                }

                if (Key is Type type)
                {
                    return $"{{x:Type {type.Name}}}";
                }

                return "Not Available";
            }
        }

        public ResourceItem(object value, object key)
        {
            Value = value ?? throw new ArgumentNullException(nameof(value));
            Key = key;
            Type = value.GetType();
            Display = $"{Key?.ToString()} = {Value.ToString()} ({Type.Name})";
        }

        public override string ToString()
        {
            return Display;
        }
    }
    public class ResourceList : List<ResourceItem>
    {
        public string Header { get; set; }
        public Type Type { get; set; }
        public ResourceList Items => this;

        public ResourceList() { }
        public ResourceList(string header) : this() => Header = header;
        public ResourceList(string header, IEnumerable<ResourceItem> items) : base(items) => Header = header;
        public ResourceList(string header, Type type) : this(header) => Type = type;
        public ResourceList(string header, Type type, IEnumerable<ResourceItem> items) : this(header, items) => Type = type;
        public ResourceList(IEnumerable<ResourceItem> items) : base(items) { }

        public override string ToString() => Header;
    }
    class ResourceToContentConverter : IValueConverter
    {
        public double MinWidth { get; set; } = 100;
        public double MinHeight { get; set; } = 100;

        FrameworkElement CreateControl(Type type)
        {
            if (type == typeof(ButtonBase))
            {
                return new Button();
            }

            var element = Activator.CreateInstance(type) as FrameworkElement;
            if (element != null)
            {
                element.SetCurrentValue(FrameworkElement.MinHeightProperty, MinHeight);
                element.SetCurrentValue(FrameworkElement.MinWidthProperty, MinWidth);
            }
            return element;
        }
        static FrameworkElement PrepareElement(FrameworkElement element)
        {
            if (element is ContextMenu contextMenu)
            {
                contextMenu.Items.Add(new MenuItem() { Header = "Test menu" });
                element = new TextBlock { Text = "Right click me!!", ContextMenu = contextMenu, Background = Brushes.Gray };
            }
            else if (element is Popup popup)
            {
                element = new TextBlock { Text = "Right click me!!" };
                popup.SetCurrentValue(Popup.PlacementTargetProperty, element);
                popup.SetCurrentValue(Popup.ChildProperty, new Button() { Content = "Test popup!!" });
                element.MouseRightButtonUp += (sender, e) => popup.SetCurrentValue(Popup.IsOpenProperty, true);
            }
            else if (element is ToolTip tooltip)
            {
                element = new TextBlock { Text = "Just move over to see tool tip.", ToolTip = tooltip };
                tooltip.SetCurrentValue(ToolTip.PlacementTargetProperty, element);
                tooltip.SetCurrentValue(ContentControl.ContentProperty, new TextBlock() { Text = "Test tool tip!!" });
            }
            else if (element is ItemsControl itemsControl)
            {
                itemsControl.SetCurrentValue(ItemsControl.ItemsSourceProperty, Enum.GetNames(typeof(StringComparison)));
            }
            else if (element is TextBlock textBlock)
            {
                textBlock.SetCurrentValue(TextBlock.TextProperty, "TextBlock control preview.");
            }
            return element;
        }
        object CreateColorPreview(Color color) => CreateBrushPreview(new SolidColorBrush(color));
        object CreateBrushPreview(Brush brush) => new Rectangle { Width = MinWidth, Height = MinHeight, Fill = brush };
        object CreateGeometryPreview(Geometry geometry) => new Path { Width = MinWidth, Height = MinHeight, Stroke = Brushes.Black, Fill = Brushes.Yellow, Data = geometry };
        object CreateFontPreview(FontFamily font) => new TextBlock { Text = "ABCDEFGHIJKLMNOPQRSTUVWXYZ", FontFamily = font };
        object CreateEffectPreview(Effect effect) => new TextBlock { Text = "Effects Preview", Effect = effect, FontWeight = FontWeights.Bold, FontSize = 24 };
        object CreateControlTemplatePreview(ControlTemplate template)
        {
            if (template.TargetType != null)
            {
                try
                {
                    if (!(CreateControl(template.TargetType) is Control control))
                    {
                        return "Target type is not a control.";
                    }

                    control.SetCurrentValue(Control.TemplateProperty, template);
                    control = PrepareElement(control) as Control;
                    return control;
                }
                catch (Exception ex)
                {
                    return $"Can not create Target type of the control template: {template.TargetType.Name}\n{ex.Message}";
                }
            }
            return "Control template does not have a target type.";
        }
        object CreateStylePreview(Style style)
        {
            if (style.TargetType != null)
            {
                try
                {
                    var control = CreateControl(style.TargetType);
                    if (control == null)
                    {
                        return "Target type is not a framework element.";
                    }

                    control.SetCurrentValue(FrameworkElement.StyleProperty, style);
                    control = PrepareElement(control);
                    return control;
                }
                catch (Exception ex)
                {
                    return $"Can not create Target type of the style: {style.TargetType.Name}\n{ex.Message}";
                }
            }
            return "Style does not have a target type.";
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Brush brush)
            {
                return CreateBrushPreview(brush);
            }

            if (value is Color color)
            {
                return CreateColorPreview(color);
            }

            if (value is Geometry geometry)
            {
                return CreateGeometryPreview(geometry);
            }

            if (value is FontFamily font)
            {
                return CreateFontPreview(font);
            }

            if (value is Effect effect)
            {
                return CreateEffectPreview(effect);
            }

            if (value is ControlTemplate controlTemplate)
            {
                return CreateControlTemplatePreview(controlTemplate);
            }

            if (value is Style style)
            {
                return CreateStylePreview(style);
            }

            return value is FrameworkElement element ? PrepareElement(element) : value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new InvalidOperationException();
        }

    }
    class ResourceToTreeIconConverter : IValueConverter
    {
        public double MinWidth { get; set; } = 50;
        public double MinHeight { get; set; } = 50;

        object CreateColorPreview(Color color) => CreateBrushPreview(new SolidColorBrush(color));
        object CreateBrushPreview(Brush brush) => new Ellipse { Width = MinWidth, Height = MinHeight, Fill = brush, Stroke = Brushes.Gray, StrokeThickness = 2 };
        object CreateGeometryPreview(Geometry geometry) => new Path { Width = MinWidth, Height = MinHeight, Stroke = Brushes.Black, Fill = Brushes.Yellow, Data = geometry };
        object CreateFontPreview(FontFamily font) => new TextBlock { Text = "ABCDEFGHIJKLMNOPQRSTUVWXYZ", FontFamily = font };
        object CreateEffectPreview(Effect effect) => new TextBlock { Text = "Effects Preview", Effect = effect, FontWeight = FontWeights.Bold, FontSize = 24 };

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Brush brush)
            {
                return CreateBrushPreview(brush);
            }

            if (value is Color color)
            {
                return CreateColorPreview(color);
            }

            if (value is Geometry geometry)
            {
                return CreateGeometryPreview(geometry);
            }

            if (value is FontFamily font)
            {
                return CreateFontPreview(font);
            }

            if (value is Effect effect)
            {
                return CreateEffectPreview(effect);
            }

            return new PackIconModern()
            { 
                Width = MinWidth, 
                Height = MinHeight, 
                Foreground = Brushes.DarkRed, 
                Kind = PackIconModernKind.SocialGithubOctocat
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new InvalidOperationException();
        }

    }
}
