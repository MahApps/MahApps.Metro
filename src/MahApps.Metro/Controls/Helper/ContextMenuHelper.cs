// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using MahApps.Metro.ValueBoxes;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// Helper class for a <see cref="ContextMenu"/>.
    /// </summary>
    public static class ContextMenuHelper
    {
        // The words WPF itself writes on the menu of a text box live in the framework's own string
        // table. Its name differs between .NET Framework and .NET and is an implementation detail on
        // both, so a name that stops answering leaves an item with the text of its command, which is
        // what it carried before.
        private static readonly string[] TableNames = { "FxResources.PresentationFramework.SR", "ExceptionStringTable" };

        private static readonly Dictionary<ICommand, string> KeyByCommand
            = new Dictionary<ICommand, string>
              {
                  { ApplicationCommands.Cut, "TextBox_ContextMenu_Cut" },
                  { ApplicationCommands.Copy, "TextBox_ContextMenu_Copy" },
                  { ApplicationCommands.Paste, "TextBox_ContextMenu_Paste" },
                  { EditingCommands.IgnoreSpellingError, "TextBox_ContextMenu_IgnoreAll" }
              };

        private static readonly Lazy<ResourceManager?> ResourceManager = new Lazy<ResourceManager?>(FindResourceManager);

        /// <summary>Identifies the <see cref="GetUseSystemCommandText(DependencyObject)"/> attached dependency property.</summary>
        public static readonly DependencyProperty UseSystemCommandTextProperty
            = DependencyProperty.RegisterAttached("UseSystemCommandText",
                                                  typeof(bool),
                                                  typeof(ContextMenuHelper),
                                                  new PropertyMetadata(BooleanBoxes.FalseBox, OnUseSystemCommandTextChanged));

        /// <summary>
        /// Gets whether the items of the menu are named the way WPF names the ones on the menu it
        /// puts on a text box itself.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(ContextMenu))]
        public static bool GetUseSystemCommandText(DependencyObject element)
        {
            return (bool)element.GetValue(UseSystemCommandTextProperty);
        }

        /// <summary>
        /// Sets whether the items of the menu are named the way WPF names the ones on the menu it
        /// puts on a text box itself.
        /// </summary>
        /// <remarks>
        /// An item with a command and no header of its own is named after <see cref="RoutedUICommand.Text"/>,
        /// and WPF reads that text once per process and keeps it. A menu built that way therefore
        /// stays in the language the application started in, while the menu WPF builds for a text
        /// box is written anew on every click and follows <see cref="CultureInfo.CurrentUICulture"/>.
        /// This asks the framework for the same words at the same moment, so the two menus say the
        /// same thing (GH-4155).
        /// </remarks>
        [AttachedPropertyBrowsableForType(typeof(ContextMenu))]
        public static void SetUseSystemCommandText(DependencyObject element, bool value)
        {
            element.SetValue(UseSystemCommandTextProperty, BooleanBoxes.Box(value));
        }

        private static void OnUseSystemCommandTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not ContextMenu contextMenu)
            {
                throw new InvalidOperationException("The property 'UseSystemCommandText' may only be set on ContextMenu elements.");
            }

            contextMenu.Opened -= ContextMenuOpened;
            contextMenu.Initialized -= ContextMenuInitialized;

            if (true.Equals(e.NewValue))
            {
                contextMenu.Opened += ContextMenuOpened;

                // in markup the attribute is set before the items below it are read, so the menu
                // is named once it stands and then again on every opening
                if (contextMenu.IsInitialized)
                {
                    NameTheItems(contextMenu);
                }
                else
                {
                    contextMenu.Initialized += ContextMenuInitialized;
                }
            }
        }

        private static void ContextMenuInitialized(object? sender, EventArgs e)
        {
            NameTheItems((ContextMenu)sender!);
        }

        private static void ContextMenuOpened(object sender, RoutedEventArgs e)
        {
            NameTheItems((ContextMenu)sender);
        }

        // only the word on the item is taken over. The key beside it stays as it is, because WPF
        // writes that into the gesture when the command is made and never asks again, so its own
        // menu shows the very same one.
        private static void NameTheItems(ContextMenu contextMenu)
        {
            foreach (var item in contextMenu.Items.OfType<MenuItem>())
            {
                var text = GetSystemText(item.Command);
                if (text is not null)
                {
                    item.SetCurrentValue(HeaderedItemsControl.HeaderProperty, text);
                }
            }
        }

        private static string? GetSystemText(ICommand? command)
        {
            if (command is null || !KeyByCommand.TryGetValue(command, out var key))
            {
                return null;
            }

            try
            {
                return ResourceManager.Value?.GetString(key);
            }
            catch (MissingManifestResourceException)
            {
                return null;
            }
        }

        private static ResourceManager? FindResourceManager()
        {
            foreach (var name in TableNames)
            {
                try
                {
                    var manager = new ResourceManager(name, typeof(ContextMenu).Assembly);

                    // the table that answers is the one this runtime carries, and the neutral
                    // resources are there whatever the machine was set up with
                    if (manager.GetString("TextBox_ContextMenu_Cut", CultureInfo.InvariantCulture) is not null)
                    {
                        return manager;
                    }
                }
                catch (MissingManifestResourceException)
                {
                    // the other name is the one the other runtime uses
                }
            }

            return null;
        }
    }
}
