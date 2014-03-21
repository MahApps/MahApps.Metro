---
layout: no-sidebar
title: RangeSlider
---

![]({{site.baseurl}}/images/range_slider.PNG)  

Taken from the [Avalon Controls Library](http://avaloncontrolslib.codeplex.com/) (MS-PL), `RangeSlider` lets you select a range of values with a slider, rather than a single value.

This control was updated to have more features

###Events

More events was added:
DragStared/DragDelta/DragCompleted for Lower/Central/Upper thumbs
Events for lower/upper value changed
OldValues parameters in event args for rangeChanged event

###Orientation

Now range slider support vertical orientation

###MoveToPoint

`IsMoveToPointEnabled` feature work like in Slider

###SmallChange/LargeChange

when `IsMoveToPointEnabled = False` thumbs will move on the value you set in Small/LargeChange

###Interval 

This property will set interval between changing values when using Small/Larnge change. 

###IsSnapToTickEnabled

If set to true, thumbs will snap to ticks like in standard Slider.

###TickBars and Tickplacement

Range Slider receive support for displaying/hiding ticks and change its ticks width according to minimum and maximum values changed

###ExtendedMode

If it set `ExtendedMode = False` you **cannot** do any manipulations **inside** range except moving thumbs closer/farther to each other with mouse, but if it enabled you **can** use MoveToPoint or Small/Large change **inside** range by clicking **Left mouse button + left or right control button** to move left thumb and **Right mouse button + left or right control button to move right thumb inside range**. If Extended mode = true you also can without problems move whole range by clicking leftmouse button

###MoveWholeRange

This property will let you move whole range when using MoveToPoint or Small/Large change (working also inside range)
 
###MinRangeWidth
Sets minimum width of **central** Thumb. It can be in range **from 0 to range_slider_width/2**.

###AutoToolTipPlacement and AutotoolTipPrecision

`AutoToolTipPlacement` will display tooltip, which will move with Thumb and display current value. Implemented for left/central/right thumbs.
 
`AutotoolTipPrecision` set the number of digits, which will be shown after dot in autotooltip.

###Small Example

```xml
	<Сontrols:RangeSlider Style="{StaticResource RangeSliderCameraCommonStyle}" 
            Minimum="{Binding Path=MinValue, Mode=OneWay, UpdateSourceTrigger=PropertyChanged}"
            Maximum="{Binding Path=MaxValue, Mode=OneWay, UpdateSourceTrigger=PropertyChanged}"
            LowerValue="{Binding Path=CurrentMinValue, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}"
            UpperValue="{Binding Path=CurrentMaxValue, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}"
            LowerValueChanged="OnLowerValueChanged" UpperValueChanged="OnUpperValueChanged"
            LowerThumbDragStarted="OnLowerDragStarted"
            LowerThumbDragCompleted="OnLowerDragCompleted"
            UpperThumbDragStarted="OnUpperDragStarted" 
            UpperThumbDragCompleted="OnUpperDragCompleted" 
            AutoToolTipPlacement="TopLeft" AutoToolTipPrecision="2" MoveWholeRange="True"
            IsSnapToTickEnabled="True" IsMoveToPoint="True" ExtendedMode="True"></Сontrols:RangeSlider>
```

