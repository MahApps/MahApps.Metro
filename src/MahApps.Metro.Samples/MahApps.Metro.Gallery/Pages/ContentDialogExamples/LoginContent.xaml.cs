// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Controls;

namespace MahApps.Metro.Gallery.Pages.ContentDialogExamples
{
    /// <summary>
    /// The content of the login dialog: the user name and the box to remember it bound to the view model, the password in a box that gives it out only through code.
    /// </summary>
    public partial class LoginContent : UserControl
    {
        public LoginContent()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// The password typed in. A password box keeps it out of bindings, so the code asks for it.
        /// </summary>
        public string Password => this.PasswordBox.Password;
    }
}
