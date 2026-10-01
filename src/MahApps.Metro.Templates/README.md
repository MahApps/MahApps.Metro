# MahApps.Metro Templates

The project templates for [MahApps.Metro](https://github.com/MahApps/MahApps.Metro).

```
dotnet new install MahApps.Metro.Templates
```

Then either of

```
dotnet new mahapps -n MyApp
dotnet new mahapps-mvvm -n MyApp
dotnet new mahapps-nav -n MyApp
```

All three give you a WPF application whose main window is a `MetroWindow` and whose `App.xaml`
already merges the dictionaries the library needs, so it runs styled from the first build. Which set
it wears is `--style`: the WinUI one unless you say otherwise.

The second adds the [MVVM Toolkit](https://learn.microsoft.com/dotnet/communitytoolkit/mvvm/) and a
view model the window is bound to.

The third is the one for an application of several pages: a `HamburgerMenu` down the left side, a
`Frame` beside it that owns its history, and a back arrow in the title bar that walks it. The pane
and the frame follow each other, so going back marks the entry of the page that comes up, and a page
reached from another page rather than from the pane leaves the pane unmarked.

A further window for a project that has one already:

```
dotnet new mahapps-window -n SettingsWindow
```

Run that one inside the project folder: it writes the two files there and takes the namespace from
the project itself, as long as the project has been restored once. Visual Studio does not show item
templates in its **Add New Item** dialog, so this one is for the command line.

Visual Studio reads the same templates. Once the package is installed they stand in the **Create a
new project** dialog, and **Install more templates from the online search** there finds the package
on nuget.org.

## Options

| Option | What it does | Default |
| --- | --- | --- |
| `-f`, `--framework` | `net10.0-windows`, `net9.0-windows`, `net8.0-windows` or `net462` | `net8.0-windows` |
| `--style` | `winui`, the look Windows 11 draws, `win10`, the look UWP gave a control, or `metro`, the one the library has always drawn | `winui` |
| `--mahapps-version` | the version of MahApps.Metro the project asks for | the version of this package |

A set goes in place of `Styles/Controls.xaml` in `App.xaml` rather than beside it, because each
merges the one below it, so switching a generated application over later is that one line.

Installing a template from a clone rather than from the package leaves `--mahapps-version` at a
placeholder, since the version is written in when the package is built. Pass the option yourself
there.

The WinUI set is newer than the release candidates on nuget.org. A project that asks for a library
older than the package it came from therefore does not find that dictionary and stops at startup, so
reach back that far with `--style metro`.
