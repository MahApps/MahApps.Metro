---
layout: no-sidebar
title: Quick Start
---

This guide will introduce you to how *MahApps.Metro* works and how to incorporate it into your app.

### Installation

You can install MahApps.Metro via the NuGet GUI (right click on your project, click **Manage NuGet Packages**, select **Online** and search for **MahApps.Metro**) or with the Package Manager Console:

<pre class="nuget-button">Install-Package MahApps.Metro</pre>

If you want to use the pre-release packages of MahApps.Metro (these have the latest code and newest features), you need to enable **Include Prerelease** in the GUI:

![]({{site.baseurl}}/images/include_prerelease.png)

or use the Package Manager Console:

<pre class="nuget-button">Install-Package MahApps.Metro -Pre</pre>

### Style the Window

There's two ways you can style your Window using MahApps.Metro:

 -  using the included `MetroWindow` control, or
 -  design your own window

For now we'll use `MetroWindow`, as this approach will work for a good percentage of apps and is the quickest and easiest way to get going. 
If you want to learn about rolling your own window, check out [the guide](advanced-guide.html).

A default WPF Window with a few controls looks like the following:

![]({{site.baseurl}}/images/01_UnstyledWindow.png)

After installing MahApps.Metro:

 - open up `MainWindow.xaml`
 - add this attribute inside the opening Window tag (it's how you can reference other namespaces in XAML):  
`xmlns:Controls="clr-namespace:MahApps.Metro.Controls;assembly=MahApps.Metro"`
 - change `<Window ...` to `<Controls:MetroWindow ...` (remember to change the closing tag too!)

You should have something like this (don't copy and paste this):

    <controls:MetroWindow x:Class="WpfApplication2.MainWindow"
                          xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                          xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                          xmlns:controls="clr-namespace:MahApps.Metro.Controls;assembly=MahApps.Metro"
                          Title="MainWindow" 
                          Height="350" 
                          Width="525">
        <!-- your layout here -->
    </controls:MetroWindow>


You'll need to modify the `MainWindow.xaml.cs` file  so that the base class for `MainWindow` matches the MetroWindow type:

    public partial class MainWindow : MetroWindow
    {
    }

But in most cases you can just drop the base class (because this is a `partial` class the XAML should take care of this):

    public partial class MainWindow
    {
    }

 Which will give us this:

![]({{site.baseurl}}/images/02_PartiallyStyledWindow.png)

Which looks different - but we're on the right track. 

Next we need to add the resources and styles.

> **Note**

In your App.xaml, add the following:
	
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
	
![]({{site.baseurl}}/images/03_StyledWindow.png)

And now we have something much better!

### What's a MetroWindow?

The default `MetroWindow` is made up of a few components:

![]({{site.baseurl}}/images/04_ExplainedStyledWindow.png)

If you don't like the elements that are labelled, fear not, they're all optional.

- The titlebar is what sets `MetroWindow` apart from rolling your own. `ShowTitleBar="true|false"`
- The resize grip is not the *only* way to resize a `MetroWindow` - all edges and corners can be gripped, but given a `MetroWindow` doesn't have a noticeable window "chrome" like an aero window, the resize grip can help reassure users.
- Instead of using static images, the icons for minimize/maximize/close are a font called **Marlett**. To explain why this is so requires a walk down memory lane, or at least a visit to [the Wikipedia article](http://en.wikipedia.org/wiki/Marlett) about it.
- You can even hide the icons on the title bar by setting the  `ShowIconOnTitleBar="true|false"` 

### Customisation

#### WindowCommands

`WindowCommands` are the minimise, maximise/restore, and close buttons. You can add your own controls to `WindowsCommands` - by default, buttons have a style automatically applied to them to make them fit in with the rest of the `WindowsCommands`. As of 0.9, you are no longer limited to just buttons, but any control. Be aware, you're responsible for styling anything other than buttons.

Including this within the `MetroWindow` tag (under the `Window.Resources` section),

	<Controls:MetroWindow.WindowCommands>
	    <Controls:WindowCommands>
	        <Button Content="settings" />
            <Button>
                <StackPanel Orientation="Horizontal">
                    <Rectangle Width="20" Height="20">
                        <Rectangle.Resources>
                            <SolidColorBrush x:Key="BlackBrush" Color="White" />
                        </Rectangle.Resources>
                        <Rectangle.Fill>
                            <VisualBrush Stretch="Fill" Visual="{StaticResource appbar_cupcake}" />
                        </Rectangle.Fill>
                    </Rectangle>
                    <TextBlock Text="deploy cupcakes" />
                </StackPanel>
            </Button>
        </Controls:WindowCommands>
	</Controls:MetroWindow.WindowCommands>

> Make sure to include the [icons](#icons) to get the cupcake

Produces this window titlebar:

![]({{site.baseurl}}/images/05_WindowCommands.png)

The foreground (link) colour of `WindowCommands` will always be white, *unless* the titlebar is disabled, in which case it will be the reverse of whatever theme you have selected. For example, using the White/Light theme, the foreground colour will be black.

### What now?

For extended documentation, take a look at the [Controls]({{site.baseurl}}/controls/) page