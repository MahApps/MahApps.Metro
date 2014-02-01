---
layout: no-sidebar
title: RangeSlider
---

![]({{site.baseurl}}/images/range_slider.PNG)  

Taken from the [Avalon Controls Library](http://avaloncontrolslib.codeplex.com/) (MS-PL), `RangeSlider` lets you select a range of values with a slider, rather than a single value.

	<Controls:RangeSlider 
		Height="25" 
		RangeStartSelected="{Binding DarkestValue, Mode=TwoWay}" 
		RangeStopSelected="{Binding LightestValue, Mode=TwoWay}" />

