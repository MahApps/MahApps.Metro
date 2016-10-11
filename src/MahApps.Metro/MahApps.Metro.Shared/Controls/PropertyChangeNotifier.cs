using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// AddValueChanged of dependency property descriptor results in memory leak as you already know.
    /// So, as described here, you can create custom class PropertyChangeNotifier to listen
    /// to any dependency property changes.
    /// 
    /// This class takes advantage of the fact that bindings use weak references to manage associations
    /// so the class will not root the object who property changes it is watching. It also uses a WeakReference
    /// to maintain a reference to the object whose property it is watching without rooting that object.
    /// In this way, you can maintain a collection of these objects so that you can unhook the property
    /// change later without worrying about that collection rooting the object whose values you are watching.
    /// 
    /// Complete implementation can be found here: http://agsmith.wordpress.com/2008/04/07/propertydescriptor-addvaluechanged-alternative/
    /// </summary>
    internal sealed class PropertyChangeNotifier : DependencyObject, IDisposable
    {
        private WeakReference _propertySource;

        public PropertyChangeNotifier(DependencyObject propertySource, string path)
            : this(propertySource, new PropertyPath(path))
        {
        }

        public PropertyChangeNotifier(DependencyObject propertySource, DependencyProperty property)
            : this(propertySource, new PropertyPath(property))
        {
        }

        public PropertyChangeNotifier(DependencyObject propertySource, PropertyPath property)
        {
            if (null == propertySource)
            {
                throw new ArgumentNullException("propertySource");
            }
            if (null == property)
            {
                throw new ArgumentNullException("property");
            }
            this._propertySource = new WeakReference(propertySource);
            var binding = new Binding();
            binding.Path = property;
            binding.Mode = BindingMode.OneWay;
            binding.Source = propertySource;
            BindingOperations.SetBinding(this, ValueProperty, binding);
        }

        public DependencyObject PropertySource
        {
            get
            {
                try
                {
                    // note, it is possible that accessing the target property
                    // will result in an exception so i’ve wrapped this check
                    // in a try catch
                    return this._propertySource.IsAlive
                        ? this._propertySource.Target as DependencyObject
                        : null;
                }
                catch
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Identifies the <see cref="Value"/> dependency property
        /// </summary>
        public static readonly DependencyProperty ValueProperty
            = DependencyProperty.Register("Value", typeof(object), typeof(PropertyChangeNotifier),
                                          new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnPropertyChanged)));

        /// <summary>
        /// Returns/sets the value of the property
        /// </summary>
        /// <seealso cref="ValueProperty"/>
        [Description("Returns/sets the value of the property")]
        [Category("Behavior")]
        [Bindable(true)]
        public object Value
        {
            get { return (object)this.GetValue(ValueProperty); }
            set { this.SetValue(ValueProperty, value); }
        }

        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var notifier = (PropertyChangeNotifier)d;
            if (null != notifier.ValueChanged)
            {
                notifier.ValueChanged(notifier.PropertySource, EventArgs.Empty);
            }
        }

        public event EventHandler ValueChanged;

        public void Dispose()
        {
            BindingOperations.ClearBinding(this, ValueProperty);
        }
    }
}