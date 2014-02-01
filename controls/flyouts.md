---
layout: no-sidebar
title: Flyouts
---

A flyout is an overlay for a window that has custom content

<img src="{{site.baseurl}}/images/flyout-demo-dark.png" style="width: 800px;"/>

Add the following code to your `MetroWindow`:

    <Controls:MetroWindow.Flyouts>
        <Controls:FlyoutsControl>
            
        </Controls:FlyoutsControl>
    </Controls:MetroWindow.Flyouts>
    
This is the container for the flyouts.
Inside this container add the following:

    <Controls:Flyout Header="Flyout" Position="Right" Width="200">
        <!-- Your custom content here -->
    </Controls:Flyout>
    
This creates a flyout with a header, sliding out from the right side of the window and has a width of 200

The `Position` property can have the values

    Left,
    Right,
    Top,
    Bottom
    
As of version 0.12, flyouts can have various themes, assignable through the `Theme` property, those are:

    Adapt,
    Inverse,
    Dark,
    Light,
    Accent

`Adapt` adapts the flyout theme to the host window's theme.  
`Inverse` has the inverse theme of the host window's theme.  
`Dark` will always be the dark theme, this is also the default value.  
`Light` will always be the light theme.  
`Accent` adapts the flyout theme to the host window's theme, it looks like this for the blue theme:

<img src="{{site.baseurl}}/images/flyout-demo-accent.png" style="width: 800px;"/>