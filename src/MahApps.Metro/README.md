# MahApps.Metro

A UI toolkit for WPF. It takes the controls you already use, gives them a clean and modern look, and adds the pieces WPF never shipped: a window you can style, a theming engine, dialogs that do not block, and a set of navigation controls.

MIT licensed, part of the [.NET Foundation](https://dotnetfoundation.org), and going since 2011.

**It restyles what you already have.** Reference the package, merge two resource dictionaries, and your existing `Button`, `TextBox`, `DataGrid` and `TreeView` are restyled where they stand. No rewrite, no new control names.

## Getting started

Merge the dictionaries into `App.xaml`, a theme and the controls:

```xml
<Application.Resources>
  <ResourceDictionary>
    <ResourceDictionary.MergedDictionaries>
      <ResourceDictionary Source="pack://application:,,,/MahApps.Metro;component/Styles/Controls.xaml" />
      <ResourceDictionary Source="pack://application:,,,/MahApps.Metro;component/Styles/Fonts.xaml" />
      <ResourceDictionary Source="pack://application:,,,/MahApps.Metro;component/Styles/Themes/Light.Blue.xaml" />
    </ResourceDictionary.MergedDictionaries>
  </ResourceDictionary>
</Application.Resources>
```

Then swap `Window` for `MetroWindow`:

```xml
<mah:MetroWindow x:Class="MyApp.MainWindow"
                 xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                 xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                 xmlns:mah="http://metro.mahapps.com/winfx/xaml/controls"
                 Title="My application">
  <Grid>
    <!-- your content, restyled where it stands -->
  </Grid>
</mah:MetroWindow>
```

The code behind has to inherit from `MetroWindow` as well:

```csharp
public partial class MainWindow : MetroWindow
{
    public MainWindow()
    {
        this.InitializeComponent();
    }
}
```

That is the whole setup. The [quick start](https://mahapps.github.io/mahapps.com/docs/guides/quick-start) has it with pictures.

## What you get

- **A window that looks like an app.** `MetroWindow` replaces the Windows title bar with one you control: your own commands left and right of the title, a glow border, an overlay for dialogs, and flyouts that slide in from any edge.
- **Theming at run time.** A theme is a base theme, Light or Dark, plus an accent colour. `ThemeManager` switches them while the application runs and can follow the Windows accent colour and the system light and dark setting.
- **Dialogs that do not block.** Message, input, login and progress dialogs shown inside your window rather than as a modal of their own. They are awaitable and reachable from a view model.
- **Controls WPF does not have.** `ColorPicker`, `DateTimePicker`, `NumericUpDown`, `MultiSelectionComboBox`, `HotKeyBox`, `HamburgerMenu`, `SplitView`, `FlipView`, badges, toggle switches and progress rings.
- **Helpers instead of subclasses.** Watermarks, clear buttons, corner radii and selected-item brushes arrive as attached properties such as `TextBoxHelper.Watermark`, so behaviour attaches to a stock control rather than asking you to inherit from a special one.

## Where to look next

- [Documentation](https://mahapps.github.io/mahapps.com/)
- [Source and issues](https://github.com/MahApps/MahApps.Metro)
- [Releases and release notes](https://github.com/MahApps/MahApps.Metro/releases)
- [Icons](https://github.com/MahApps/MahApps.Metro.IconPacks), a package of its own with thousands of them

## Which version to take

| | |
| --- | --- |
| 3.x | .NET Framework 4.6.2, .NET 6 and .NET 8 on Windows. Window chrome up to date with Windows 11: the caption buttons take part in snap layouts, and the newer backdrop materials are available. |
| 2.4 | .NET Framework 4.5.2 and newer, and .NET Core 3.x. |
