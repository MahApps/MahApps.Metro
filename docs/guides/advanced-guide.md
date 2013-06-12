---
layout: no-sidebar
title: Advanced MetroWindow Guide
---


### Roll your own Window

The roll your own approach is very relevant, depending on what style of app you're going for. A good example is [code52's MarkPad](http://code52.org/DownmarkerWPF/), where we needed the flexibility of rolling our own window while still using MahApps.Metro as an underlying visual framework. The key elements used were the `WindowCommands` (discussed [above](#windowcommands)), and `BorderlessWindowBehavior`.

#### BorderlessWindowBehavior

Add some namespace references to the opening `Window` tag:

	xmlns:i="http://schemas.microsoft.com/expression/2010/interactivity"
	xmlns:Behaviours="clr-namespace:MahApps.Metro.Behaviours;assembly=MahApps.Metro"

Then somewhere within the `Window`, add

    <i:Interaction.Behaviors>
        <Behaviours:BorderlessWindowBehavior/>
    </i:Interaction.Behaviors>

And you're done! `BorderlessWindowBehavior` has a few options, such as `ResizeWithGrip` and `AutoSizeToContent`:

	<Behaviours:BorderlessWindowBehavior ResizeWithGrip="False" />
