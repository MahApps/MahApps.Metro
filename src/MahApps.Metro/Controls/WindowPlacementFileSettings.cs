// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Xml.Serialization;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// Keeps the placement of a window in a file of its own instead of the application settings.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The default store, which builds on <see cref="System.Configuration.ApplicationSettingsBase"/>, fails
    /// to save on some runtimes and setups, for example .NET 9 before 9.0.3 when the application runs from a
    /// network location. This store has no such dependency, and it is useful wherever the placement should
    /// live somewhere the application controls.
    /// </para>
    /// <para>
    /// It is not used unless a caller asks for it:
    /// <code>
    /// this.WindowPlacementSettings = WindowPlacementFileSettings.ForWindow(this);
    /// </code>
    /// </para>
    /// </remarks>
    public class WindowPlacementFileSettings : IWindowPlacementSettings
    {
        private static readonly XmlSerializer Serializer = new(typeof(StoredPlacement));

        /// <summary>
        /// Initializes a new instance which reads and writes the given file.
        /// </summary>
        /// <param name="filePath">The file to keep the placement in. Its directory is created when saving.</param>
        public WindowPlacementFileSettings(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException("The file path must not be empty.", nameof(filePath));
            }

            this.FilePath = filePath;
        }

        /// <summary>
        /// Gets the file the placement is kept in.
        /// </summary>
        public string FilePath { get; }

        /// <inheritdoc />
        public WindowPlacementSetting? Placement { get; set; }

        /// <inheritdoc />
        public bool UpgradeSettings { get; set; }

        /// <summary>
        /// Creates a store for the given window under the local application data of the current user.
        /// </summary>
        /// <param name="window">The window whose type names the file.</param>
        public static WindowPlacementFileSettings ForWindow(Window window)
        {
            if (window is null)
            {
                throw new ArgumentNullException(nameof(window));
            }

            var root = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var application = Assembly.GetEntryAssembly()?.GetName().Name ?? "MahApps.Metro";
            var name = window.GetType().FullName ?? window.GetType().Name;

            return new WindowPlacementFileSettings(Path.Combine(root, application, "WindowPlacement", $"{name}.xml"));
        }

        /// <inheritdoc />
        public void Reload()
        {
            if (!File.Exists(this.FilePath))
            {
                // no file yet is the normal first start
                this.Placement = null;
                return;
            }

            using var stream = File.OpenRead(this.FilePath);
            var stored = (StoredPlacement?)Serializer.Deserialize(stream);

            this.Placement = stored?.ToSetting();
        }

        /// <inheritdoc />
        public void Save()
        {
            if (this.Placement is null)
            {
                return;
            }

            var directory = Path.GetDirectoryName(this.FilePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory!);
            }

            using var stream = File.Create(this.FilePath);
            Serializer.Serialize(stream, StoredPlacement.From(this.Placement));
        }

        /// <inheritdoc />
        public void Upgrade()
        {
            // there is nothing to carry over from an older version of the application
        }

        /// <inheritdoc />
        public void Reset()
        {
            this.Placement = null;

            if (File.Exists(this.FilePath))
            {
                File.Delete(this.FilePath);
            }
        }

        /// <summary>
        /// What actually goes into the file. Plain numbers rather than <see cref="WindowPlacementSetting"/>,
        /// so the file survives changes to that type and needs no serializer that knows WPF types.
        /// </summary>
        public class StoredPlacement
        {
            public uint ShowCommand { get; set; }

            public double MinimumX { get; set; }

            public double MinimumY { get; set; }

            public double MaximumX { get; set; }

            public double MaximumY { get; set; }

            public double X { get; set; }

            public double Y { get; set; }

            public double Width { get; set; }

            public double Height { get; set; }

            internal static StoredPlacement From(WindowPlacementSetting placement)
            {
                return new StoredPlacement
                       {
                           ShowCommand = placement.showCmd,
                           MinimumX = placement.minPosition.X,
                           MinimumY = placement.minPosition.Y,
                           MaximumX = placement.maxPosition.X,
                           MaximumY = placement.maxPosition.Y,
                           X = placement.normalPosition.X,
                           Y = placement.normalPosition.Y,
                           Width = placement.normalPosition.Width,
                           Height = placement.normalPosition.Height
                       };
            }

            internal WindowPlacementSetting ToSetting()
            {
                return new WindowPlacementSetting
                       {
                           showCmd = this.ShowCommand,
                           minPosition = new Point(this.MinimumX, this.MinimumY),
                           maxPosition = new Point(this.MaximumX, this.MaximumY),
                           normalPosition = new Rect(this.X, this.Y, this.Width, this.Height)
                       };
            }
        }
    }
}
