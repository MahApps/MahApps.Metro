// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;
using System.Windows.Controls;

namespace MahApps.Metro.Tests.Views
{
    public partial class ValidationPopupWindow : TestWindow
    {
        public ValidationPopupWindow()
        {
            this.InitializeComponent();
        }
    }

    /// <summary>
    /// A rule that turns every value down, so that a box has an error to show without a model
    /// behind it.
    /// </summary>
    public class AlwaysWrong : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            return new ValidationResult(false, "Not what it should be.");
        }
    }
}
