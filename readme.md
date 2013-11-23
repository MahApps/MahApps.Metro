## MahApps.Metro

A toolkit for creating metro-style WPF applications. Lots of goodness out-of-the box.

CheckBox and RadioButton styles adapted from styles created by [Brian Lagunas of Infragistics](http://brianlagunas.com/free-metro-light-and-dark-themes-for-wpf-and-silverlight-microsoft-controls/).

### Documentation

Read it here: [http://mahapps.com/MahApps.Metro/](http://mahapps.com/MahApps.Metro/)

You can help keep the documentation up to date by submitting a pull request on the `gh-pages` branch of this repository. If you're unfamiliar with GitHub Pages, the help guides [here](https://help.github.com/pages/) are a good place to start.

### Breaking Changes

[Breaking Changes or WTF is happening with the ALPHA version](https://github.com/MahApps/MahApps.Metro/wiki/Breaking-Changes-or-WTF-is-happening-with-the-ALPHA-version)

### Quick How To
or, how to create a simple MahApps.Metro App and a Window... it's so easy ;-)
```XML
<Application x:Class="WpfApplication.App"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             StartupUri="MainWindow.xaml">
  <Application.Resources>
    <ResourceDictionary>
      <ResourceDictionary.MergedDictionaries>
        <ResourceDictionary Source="pack://application:,,,/MahApps.Metro;component/Styles/Controls.xaml" />
        <ResourceDictionary Source="pack://application:,,,/MahApps.Metro;component/Styles/Fonts.xaml" />
        <ResourceDictionary Source="pack://application:,,,/MahApps.Metro;component/Styles/Colors.xaml" />
        <ResourceDictionary Source="pack://application:,,,/MahApps.Metro;component/Styles/Accents/Blue.xaml" />
        <ResourceDictionary Source="pack://application:,,,/MahApps.Metro;component/Styles/Accents/BaseLight.xaml" />
      </ResourceDictionary.MergedDictionaries>
    </ResourceDictionary>
  </Application.Resources>
</Application>
```
```XML
<controls:MetroWindow x:Class="WpfApplication.MainWindow"
                      xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                      xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                      xmlns:controls="http://metro.mahapps.com/winfx/xaml/controls"
                      Title="MainWindow"
                      Height="600"
                      Width="800">
  <Grid>
    <!-- now your content -->
  
  </Grid>
</controls:MetroWindow>
```
```csharp
namespace WpfApplication
{
  public partial class MainWindow : MetroWindow
  {
    public MainWindow()
    {
      InitializeComponent();
    }
  }
}
```

### Contributions

If you've improved MahApps.Metro and think that other people would enjoy it, submit a pull request.

### IRC chatroom

Drop in on the IRC room - `#mametro` on [Freenode](http://freenode.net/) - if you want to ask a question. If no one responds and you have to leave, please post your issue on the [issue tracker](https://github.com/MahApps/MahApps.Metro/issues).

### Bug Reports

Please read [this page](https://github.com/MahApps/MahApps.Metro/wiki/About-Bug-Reports) before submitting an issue.
