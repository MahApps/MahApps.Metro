---
layout: no-sidebar
title: Buttons
---

# Default look

This just replaces the standard button when you drop in the library, nothing fancy to activate it    
![]({{site.baseurl}}/images/08_RegularButton.png)

# MetroCircleButton

"Standard" circle button, designed for icons.  
Add the following to a button to apply this style: `Style="{DynamicResource MetroCircleButtonStyle}"`  
![]({{site.baseurl}}/images/07_CircleButtons.png)

# SquareButton 

Another WP7 styled button, this time just for text. Like all the buttons here, has normal, clicked, and hover states.  
![]({{site.baseurl}}/images/square-button.png)

Add the following to a button to apply this style: `Style="{DynamicResource SquareButtonStyle}"`

# AccentedAquareButton

A slightly modified version of `SquareButton` that has the current accent color as background color

![]({{site.baseurl}}/images/accent-square-button.png)

Add the following to a button to apply this style: `Style="{StaticResource AccentedSquareButtonStyle}"`

# FlatButton

This sort of button can be found when you're making a call on Windows Phone - all of the controls (hang up, keypad, etc) are 'flat buttons'.  

![](http://images.theleagueofpaul.com/flatbutton04.png)  

Flat button lives in   
`<ResourceDictionary Source="pack://application:,,,/MahApps.Metro;component/Styles/FlatButton.xaml" />`

You'll need to import that as well to use it.



