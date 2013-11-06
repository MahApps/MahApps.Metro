---
layout: no-sidebar
title: Buttons
---

# Default look

This just replaces the standard button when you drop in the library, nothing fancy to activate it    
![](/images/08_RegularButton.png)

# MetroCircleButton

"Standard" circle button, designed for icons.  
Add the following to a button to apply this style: `Style="{DynamicResource MetroCircleButtonStyle}"`  
![](/images/07_CircleButtons.png)

# SquareButton 

Another WP7 styled button, this time just for text. Like all the buttons here, has normal, clicked, and hover states.  
![](http://images.theleagueofpaul.com/squarebutton04.png)  

Add the following to a button to apply this style: `Style="{DynamicResource SquareButtonStyle}"`

# FlatButton

This sort of button can be found when you're making a call on Windows Phone - all of the controls (hang up, keypad, etc) are 'flat buttons'.  

![](http://images.theleagueofpaul.com/flatbutton04.png)  

Flat button lives in   
`<ResourceDictionary Source="pack://application:,,,/MahApps.Metro;component/Styles/FlatButton.xaml" />`

You'll need to import that as well to use it.



