## MahApps.Metro

A toolkit for creating metro-style WPF applications. Lots of goodness out-of-the box.

`CheckBox` and `RadioButton` styles adapted from styles created by [Brian Lagunas of Infragistics](http://brianlagunas.com/free-metro-light-and-dark-themes-for-wpf-and-silverlight-microsoft-controls/).

### Documentation

Read it here: [http://mahapps.com](http://mahapps.com)

You can help keep the documentation up to date by submitting a pull request on the  [mahapps.github.com](https://github.com/MahApps/mahapps.github.com) repository. If you're unfamiliar with GitHub Pages, the help guides [here](https://help.github.com/pages/) are a good place to start.

### Icons

MahApps.Metro has also an icon package used from [WindowsIcons](https://github.com/Templarian/WindowsIcons) by [Templarian](https://github.com/Templarian)

You can download the Nuget package [here](https://www.nuget.org/packages/MahApps.Metro.Resources)

### Ran into an bug?

Did you stumble upon a bug? Before reporting it to us, please check out the [FAQ](https://github.com/MahApps/MahApps.Metro/wiki/FAQ) to see if it is actually a bug. If you can not find anything related to your issue, feel free to report it to us in the issue tracker.

#### Bug Reports

Please read [this page](https://github.com/MahApps/MahApps.Metro/wiki/About-Bug-Reports) before submitting an issue.

### Breaking Changes

* for v0.11.3.1 [WTF is happening with the ALPHA version](https://github.com/MahApps/MahApps.Metro/wiki/Breaking-Changes-or-WTF-is-happening-with-the-ALPHA-version)
* for the upcoming [v1.0.0](https://github.com/MahApps/MahApps.Metro/blob/master/docs/v1.0-Migration-Guide.md)

### Visual Studio Templates

Yes we did it! We have now 4 simple templates to start with the awesome MahApps.Metro library.

* WPF MahApps.Metro Application (.NET4)
* WPF MahApps.Metro Application (.NET45)
* Visual Basic WPF MahApps.Metro Application (.NET45)
* Window MahApps.Metro (WPF)

Download this [Templates.zip](https://github.com/MahApps/MahApps.Metro/raw/master/Visual%20Studio%20Templates/Templates.zip) (templates are created with VS 2013) and extract it to your user template folder:

```
c:\Users\<USER>\Documents\Visual Studio 2013\
```

Now you can choose the templates at the `New Project` dialog.

![](./docs/new_project_dialog.png)

### A short How To

Or, how to create a simple `MahApps.Metro` Application and a Window...

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

### Chatroom

Drop in on the Gitter room - https://gitter.im/MahApps/MahApps.Metro - if you want to ask a question or discuss something with the team.

### Missing a control?

If you're looking for a control that we don't have, we have some friends who have made MA.M compatible controls. Check them out:

- [Loading indicators](https://github.com/100GPing100/LoadingIndicators.WPF) by [@100GPing100](https://github.com/100GPing100)
