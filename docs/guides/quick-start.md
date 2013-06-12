---
layout: no-sidebar
title: Mahapps.Metro Quick Start
---

This is basically the "Hello World" example, so it should give you enough familiarity with how MahApps.Metro works and how to plug it into your app.

TODO: Yes, these links don't work. Just scroll, it'll be fine.

- [Installing MahApps.Metro](#installing_mahappsmetro)
- [Styling the Window](#styling_a_window)
- [What's a MetroWindow?](#explaining_the_metrowindow_elements)
- [Customisation](#customisation)


## Installation

You can install MahApps.Metro via Nuget using the GUI (right click on your project, Manage Nuget References, search for 'MahApps.Metro') or via the console:

<pre class="nuget-button">Install-Package MahApps.Metro</pre>

If you wish to use the *alpha* releases of MahApps.Metro, you need to include "pre" releases in Nuget (1.7 and above) 

![]({{site.baseurl}}images/include_prerelease.png)

<!-- TODO: i suspect this library doesn't actually work work when you have multiple nuget-button links. will confirm. -->

or use the Package Manager Console (`PM> Install-Package MahApps.Metro -Pre`)

## Styling a Window

There are two main approaches you can take with MahApps.Metro to style a Window:

 -  using the `MetroWindow` control, or
 -  rolling your own 

For now we'll use `MetroWindow`, as this approach will work for a good percentage of apps and is the quickest and easiest way to get going. 

If you want to learn about rolling your own window, check out [the guide](advanced-guide.html).

A default WPF Window with a few controls looks like the following:

![]({{site.baseurl}}images/01_UnstyledWindow.png)

After installing MahApps.Metro:

 - open up `MainWindow.xaml`
 - add a namespace reference in the opening Window tag:  
`xmlns:Controls="clr-namespace:MahApps.Metro.Controls;assembly=MahApps.Metro"`
 - change `<Window ...` to `<Controls:MetroWindow ...` (remember to change the closing tag!)

You'll need to modify the `MainWindow.xaml.cs` file  so that the base class for `MainWindow` matches the MetroWindow type:

    public partial class MainWindow : MetroWindow
    {

    }

But in most cases you can just drop the base class (because this is a `partial` class the XAML should take care of this):

    public partial class MainWindow
    {
    	
    }

 Which will give us this:

![]({{site.baseurl}}images/02_PartiallyStyledWindow.png)

Which looks different - but we're on the right track. 

Next we need to add the resources and styles. Unfortunately these need to be specified with each Window.

> **A note from the team**

> We have tried embedding the resources and styles in `MetroWindow` and in `App`, but you sacrifice all ability to dynamically change the theme.

> We are looking into improving this in an upcoming release. Stay tuned.

Just after the opening MetroWindow tag, add the following:
	
	   <Window.Resources>
	        <ResourceDictionary>
	            <ResourceDictionary.MergedDictionaries>
	                <ResourceDictionary Source="pack://application:,,,/MahApps.Metro;component/Styles/Colours.xaml" />
	                <ResourceDictionary Source="pack://application:,,,/MahApps.Metro;component/Styles/Fonts.xaml" />
	                <ResourceDictionary Source="pack://application:,,,/MahApps.Metro;component/Styles/Controls.xaml" />
	                <ResourceDictionary Source="pack://application:,,,/MahApps.Metro;component/Styles/Accents/Blue.xaml" />
	                <ResourceDictionary Source="pack://application:,,,/MahApps.Metro;component/Styles/Accents/BaseLight.xaml" />
	            </ResourceDictionary.MergedDictionaries>
	        </ResourceDictionary>
	    </Window.Resources>
	
![]({{site.baseurl}}images/03_StyledWindow.png)

And now we have something much better!

## What's a MetroWindow?

The default `MetroWindow` is made up of a few components:

![]({{site.baseurl}}images/04_ExplainedStyledWindow.png)

If you don't like the elements that are labelled, fear not, they're all optional.

- The titlebar is what sets `MetroWindow` apart from rolling your own. `ShowTitleBar="true|false"`
- The resize grip is not the *only* way to resize a `MetroWindow` - all edges and corners can be gripped, but given a `MetroWindow` doesn't have a noticeable window "chrome" like an aero window, the resize grip can help reassure users.
- Instead of using static images, the icons for minimize/maximize/close are a font called **Marlett**. To explain why this is so requires a walk down memory lane, or at least a visit to [the Wikipedia article](http://en.wikipedia.org/wiki/Marlett) about it.
- You can even hide the icons on the title bar by setting the  `ShowIconOnTitleBar="true|false"` 

## Customisation

### WindowCommands

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

![]({{site.baseurl}}images/05_WindowCommands.png)

The foreground (link) colour of `WindowCommands` will always be white, *unless* the titlebar is disabled, in which case it will be the reverse of whatever theme you have selected. For example, using the White/Light theme, the foreground colour will be black.