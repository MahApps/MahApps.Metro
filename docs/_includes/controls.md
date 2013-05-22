##Controls
Most of the controls are demonstrated in the sample application which you can pull down from [source](https://github.com/mahapps/mahapps.metro/). 

There are three categories of controls in MahApps.Metro

* Implicit template/styles that replace the look of existing controls (such as `Textbox`, `Button`, or `Scrollbar`)
* Explicit template/styles that require you to set the *style* of a control (such as `ChromelessButtonStyle`)
* New controls (such as `ToggleSwitch` or `MetroWindow`)

###MetroWindow
`MetroWindow` is detailed in the [getting started](#styling_a_window) section. One property not detailed is the `SaveWindowPosition` (true/false, default false) option. Setting this property to `true` will mean on next launch, it will automatically be positioned and sized to what it was on exit. This is designed to improve UX and speed development as its one of those "plumbing" UI things that is done regularly.  

Be careful though - if a monitor is detached during application exit and restart, or if certain circumstances arise, your application may launch off screen. Be sure to provide a 'reset' option or handle that in code.
  
###Panorama
![](images/panorama.png)

`Panorama`s are sideways scrolling (typically tile) lists, found in Windows Phone 7, Windows 8 (the "metro" start screen) and in select parts of the Zune interface.

A `Panorama` can be databound, but because of it's nature it needs a different type of collection. Specifically, `Panorama`s bind to a(n observable) collection of `PanoramaGroup`s. 

 	<Controls:Panorama ItemBox="140" ItemsSource="{Binding Groups}" />

And then on the `ViewModel`

	
	public ObservableCollection<PanoramaGroup> Groups { get; set; }
	...
	public ViewModel()
	{
		tracks = new PanoramaGroup("trending tracks");
		artists = new PanoramaGroup("trending artists");
		Groups = new ObservableCollection<PanoramaGroup> { tracks, artists };
	}
	
	...
	public void GetDataCallback()
	{
	 	tracks.SetSource(x.Tracks.track.Take(25));
	}


###Buttons
####Standard Button
This just replaces the standard button when you drop in the library, nothing fancy to activate it    
![](images/08_RegularButton.png)

####MetroCircleButton  
"Standard" circle button, designed for icons.  
Add the following to a button to apply this style: `Style="{DynamicResource MetroCircleButtonStyle}"`  
![](images/07_CircleButtons.png)

####AppBarButton &#91;Obsolete\]
Inspired by Windows Phone 7's app bar buttons which are a circle button with text underneath.  
![](http://images.theleagueofpaul.com/appbarbuttoncontrol.png)  
Use the `AppBarButton` control to use this type of button.  

    <Controls:AppBarButton
       VerticalAlignment="Top"
       MetroImageSource="{StaticResource appbar_barcode}"
       Foreground="{DynamicResource BlackBrush}"
       Content="scan" />  

> Due to issues with this control, `AppBarButton` is due to be removed for v1.0

####Square button 
Another WP7 styled button, this time just for text. Like all the buttons here, has normal, clicked, and hover states.  
![](http://images.theleagueofpaul.com/squarebutton04.png)  
Add the following to a button to apply this style: `Style="{DynamicResource SquareButtonStyle}"`

####FlatButton
This sort of button can be found when you're making a call on Windows Phone - all of the controls (hang up, keypad, etc) are 'flat buttons'.  
![](http://images.theleagueofpaul.com/flatbutton04.png)  
Flat button lives in   
`<ResourceDictionary Source="pack://application:,,,/MahApps.Metro;component/Styles/FlatButton.xaml" />`

You'll need to import that as well to use it.

###Toggle Switch
The `ToggleSwitch` control originally appeared in Windows Phone 7, but has made its way into Windows 8. The MahApps.Metro version of this control uses the Windows Phone 7 code (ported), but the Windows 8 visuals.

The function is very similar to that of a checkbox, but easier to differentiate and easier to use with touch interfaces. Basically though, it can be thought of as a pretty `CheckBox`

`<Controls:ToggleSwitch Header="WiFi rest state" />`

![](images/09_toggleswitch.png)  

You can bind to/set `IsChecked` to switch between the two states.  You can change the *on* and *off* labels by setting `<Controls:ToggleSwitch OnLabel="Yes" OffLabel="No" />`

###TextBox
There is just the one style in this library for `TextBox`, however it does have a special attached property for creating 'watermarked' textboxes and for added a 'clear' button.

**What is a watermark?**  
Watermarked - in the context of textboxes - refers to text that appears in the textbox *before* the user has focussed or entered text. This is often an alternative to having a set of labels, you can instead just have a textbox with a watermark like 'search terms go here'.

**Why AttachedProperty?**  
The easiest way (for me) would have been to add a custom control, something like `<WatermarkedTextBox`, but then that's another control you have to use rather than just the style definitions at the top. The attachedproperty makes it entirely opt in if you want the watermark.

####Watermark usage

``<TextBox Controls:TextboxHelper.Watermark="This is a textbox" />``

Will produce a textbox that looks like the below image. The three states are *unfocussed* with no user text entered, focussed, and unfocussed with user text.

![](images/10_textboxstates.png)


####Clear text button usage
Like the watermark, a simple attached property adds in the functionality

``<TextBox Controls:TextboxHelper.ClearTextButton="True" />``

Which will give you

![](images/11_textboxclearstates.png)

This can be combined with - but doesn't require - the watermark attachedproperty

###Progress Ring
The Progress Ring control is styled after a similar control in Windows 8 to indicate activity rather than a percentage of progress completed.
``<Controls:ProgressRing IsActive="True" />``

`IsActive` can easily be bound to a view model property. Override `Foreground` if you wish to change the colour.

![](images/progress_ring.gif)



###Tabs
There are three included tab styles - Animated Tab Control, Single Row Animated Tab Control and the default Tab Control. The default Tab Control style is included in `Controls.xaml`, but the other two require specific referencing (make sure to do this *after* a reference to `Controls.xaml`)

**Default look**  
![](images/default_tab_control.png)  
This shows the three states - selected/active tab, hover and inactive.

####AnimatedTabControl
``<ResourceDictionary Source="pack://application:,,,/MahApps.Metro;component/Styles/Controls.AnimatedTabControl.xaml" />``
Functioning just like the regular tab control, except it animates every tab change by wrapping everything in a `MetroContentControl`.  
![](images/animatedtabcontrol.gif) 

####AnimatedSingleRowTabControl
``<ResourceDictionary Source="pack://application:,,,/MahApps.Metro;component/Styles/Controls.AnimatedSingleRowTabControl.xaml" />``
`AnimatedSingleRowTabControl` functions exactly the same as the `AnimatedTabControl` except the tabs will only appear on a single line rather than wrapping. Instead of wrapping, arrows (left/right) are presented.   

![](images/singlerow_tab_control.png)  

###Range Slider
![](images/range_slider.PNG)  

Taken from the [Avalon Controls Library](http://avaloncontrolslib.codeplex.com/) (MS-PL), `RangeSlider` lets you select a range of values with a slider, rather than a single value.

	<Controls:RangeSlider 
		Height="25" 
		RangeStartSelected="{Binding DarkestValue, Mode=TwoWay}" 
		RangeStopSelected="{Binding LightestValue, Mode=TwoWay}" />


###TransitioningContentControl
Taken from Silverlight (specifically [this](https://github.com/jenspettersson/WPF-Controls) port), `TransitioningContentControl` is great for switching content smoothly around. At it's core, `TransitioningContentControl` is a `ContentControl`, so only one child element can be displayed at a time. When you change the content, an animation is played switching the two.

`<Controls:TransitioningContentControl x:Name="transitioning" Width="150" Height="50" Transition="DownTransition" />`

Built in there are several transitions:  

* DownTransition
* UpTransition
* RightTransition
* LeftTransition
* RightReplaceTransition
* LeftReplaceTransition
                            
This is still a 'work in progress' control, so there are some limitations - at the moment you can't provide a custom transition without overriding the style of the control.
