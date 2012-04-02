##Controls
Most of the controls are demonstrated in the sample application which you can pull down from [source](https://github.com/mahapps/mahapps.metro/). 

There are three categories of controls in MahApps.Metro

* Implicit template/styles that replace the look of existing controls (such as `Textbox`, `Button`, or `Scrollbar`)
* Explicit template/styles that require you to set the *style* of a control (such as `ChromelessButtonStyle`)
* New controls (such as `ToggleSwitch` or `MetroWindow`)

###Buttons
####Standard Button
This just replaces the standard button when you drop in the library, nothing fancy to activate it    
![](images/08_RegularButton.png)

####MetroCircleButton  
"Standard" circle button, designed for icons.  
Add the following to a button to apply this style: `Style="{DynamicResource MetroCircleButtonStyle}"`  
![](images/07_CircleButtons.png)

####AppBarButton
Inspired by Windows Phone 7's app bar buttons which are a circle button with text underneath.  
![](http://images.theleagueofpaul.com/appbarbuttoncontrol.png)  
Use the `AppBarButton` control to use this type of button.  

    <Controls:AppBarButton
       VerticalAlignment="Top"
       MetroImageSource="{StaticResource appbar_barcode}"
       Foreground="{DynamicResource BlackBrush}"
       Content="scan" />  

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

You can bind to/set `IsChecked` to switch between the two states.

###TextBox
There is just the one style in this library for `TextBox`, however it does have a special attached property for creating 'watermarked' textboxes. 

**What is a watermark?**  
Watermarked - in the context of textboxes - refers to text that appears in the textbox *before* the user has focussed or entered text. This is often an alternative to having a set of labels, you can instead just have a textbox with a watermark like 'search terms go here'.

**Why AttachedProperty?**  
The easiest way (for me) would have been to add a custom control, something like `<WatermarkedTextBox`, but then that's another control you have to use rather than just the style definitions at the top. The attachedproperty makes it entirely opt in if you want the watermark.

####Watermark usage

``<TextBox Controls:TextboxHelper.Watermark="This is a textbox" />``

Will produce a textbox that looks like the below image. The three states are *unfocussed* with no user text entered, focussed, and unfocussed with user text.

![](images/10_textboxstates.png)