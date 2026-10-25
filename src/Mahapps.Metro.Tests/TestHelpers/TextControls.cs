// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Controls;
using System.Windows.Documents;

namespace MahApps.Metro.Tests.TestHelpers
{
    public static class TextControls
    {
        /// <summary>
        /// A rich text box takes its text as a document rather than as a string.
        /// </summary>
        public static RichTextBox Document(string text)
        {
            var box = new RichTextBox();
            box.Document.Blocks.Clear();
            box.Document.Blocks.Add(new Paragraph(new Run(text)));

            return box;
        }
    }
}
