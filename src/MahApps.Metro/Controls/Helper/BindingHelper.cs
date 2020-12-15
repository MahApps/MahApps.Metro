using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace MahApps.Metro.Controls.Helper
{
    /// <summary>
    /// A helper class to evaluate Bindings in code behind
    /// </summary>
    public static class BindingHelper
    {
        /// <summary>
        /// A dummy property to initialize the binding to evaluate
        /// </summary>
        private static readonly DependencyProperty DummyProperty = DependencyProperty.RegisterAttached(
            "Dummy",
            typeof(object),
            typeof(BindingHelper),
            new UIPropertyMetadata(null));


        /// <summary>
        /// Evaluates a defined <see cref="Binding"/>-path on the given object
        /// </summary>
        /// <param name="source">the object to evaluate</param>
        /// <param name="expression">the binding expression to evaluate</param>
        /// <returns>the result of the <see cref="Binding"/></returns>
        public static object Eval(object source, string expression)
        {
            Binding binding = new Binding(expression) { Source = source };
            return Eval(binding);
        }

        /// <summary>
        /// Evaluates a defined <see cref="Binding"/>-path on the given object
        /// </summary>
        /// <param name="source">the object to evaluate</param>
        /// <param name="expression">the binding expression to evaluate</param>
        /// <param name="format">the stringformat to use</param>
        /// <returns>the result of the <see cref="Binding"/></returns>
        public static object Eval(object source, string expression, string format)
        {
            Binding binding = new Binding(expression) { Source = source, StringFormat = format };
            return Eval(binding);
        }

        /// <summary>
        /// Evaluates a defined <see cref="Binding"/> on the given object
        /// </summary>
        /// <param name="binding">The <see cref="Binding"/> to evaluate</param>
        /// <param name="source">the object to evaluate</param>
        /// <returns></returns>
        public static object Eval(Binding binding, object source)
        {
            if (binding is null) throw new ArgumentNullException(nameof(binding));

            Binding newBinding = new Binding()
            {
                Source = source,
                AsyncState = binding.AsyncState,
                BindingGroupName = binding.BindingGroupName,
                BindsDirectlyToSource = binding.BindsDirectlyToSource,
                Path = binding.Path,
                Converter = binding.Converter,
                ConverterCulture = binding.ConverterCulture,
                ConverterParameter = binding.ConverterParameter,
                FallbackValue = binding.FallbackValue,
                IsAsync = binding.IsAsync,
                Mode = BindingMode.OneWay,
                StringFormat = binding.StringFormat,
                TargetNullValue = binding.TargetNullValue
            };
            return Eval(newBinding);
        }

        /// <summary>
        /// Evaluates a defined <see cref="Binding"/> on the given <see cref="DependencyObject"/>
        /// </summary>
        /// <param name="binding">The <see cref="Binding"/> to evaluate</param>
        /// <param name="dependencyObject">optional: The <see cref="DependencyObject"/> to evalutate</param>
        /// <returns></returns>
        public static object Eval(Binding binding, DependencyObject dependencyObject = null)
        {
            dependencyObject ??= new DependencyObject();
            BindingOperations.SetBinding(dependencyObject, DummyProperty, binding);
            return dependencyObject.GetValue(DummyProperty);
        }
    }
}
