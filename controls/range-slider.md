---
layout: no-sidebar
title: RangeSlider
---

![]({{site.baseurl}}/images/range_slider.PNG)  

Taken from the [Avalon Controls Library](http://avaloncontrolslib.codeplex.com/) (MS-PL), `RangeSlider` lets you select a range of values with a slider, rather than a single value.

This control was updated to have the same features as standard MS slider:

DragStared/DragDelta/DragCompleted for each of 3 thumbs
events for lower/upper value changed
OldValues in event args for rangeChanged event
Vertical orientation support
MoveToPoint feature like in Slider
SmallChange/LargeChange - when MoveToPoint = False thumbs will move on the value you set in Small/LargeChange
IsSnapToTickEnabled feature with TickFrequency. If set to true, thumbs will snap to ticks.
TickBars and tickplacement property for displaying/hiding ticks (Change its ticks width according to minimum and maximum values changed)
ExtendedMode property. If it set to false, you cannot do any manipulation inside range except moving thumbs closer/farther to each other, but if it enabled you can use MoveToPoint or Small/Large change inside range by clicking Left mouse button + left or right control button to move left thumb and right mouse button+ left or right control button to move right thumb inside range. Using Exteded mode you also can without problems move whole range by clicking leftmouse button
MoveWholeRange property will let you move whole range when using MoveToPoint or Small/Large change (working also inside range)
MinRangeWidth property sets minimum width of central Thumb. It can be in range from 0 to range slider width/2.
AutoToolTipPlacement - will display tooltip, which will move with Thumb and display current value. Implemented for left/central/right thumbs
AutotoolTip precision - set the number of digits, which will be shown after . in autotooltip.


	<Сontrols:RangeSlider Style="{StaticResource RangeSliderCameraCommonStyle}" 
            Minimum="{Binding Path=MinValue, Mode=OneWay, UpdateSourceTrigger=PropertyChanged}"
            Maximum="{Binding Path=MaxValue, Mode=OneWay, UpdateSourceTrigger=PropertyChanged}"
            LowerValue="{Binding Path=CurrentMinValue, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}"
            UpperValue="{Binding Path=CurrentMaxValue, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}"
            LowerValueChanged="OnLowerValueChanged" UpperValueChanged="OnUpperValueChanged"
            LowerThumbDragStarted="SliderCameraMinX_OnDragStarted"
            LowerThumbDragCompleted="SliderCameraMinX_OnDragCompleted"
            UpperThumbDragStarted="OnUpperDragStarted" 
            UpperThumbDragCompleted="OnUpperDragCompleted" 
            AutoToolTipPlacement="TopLeft" AutoToolTipPrecision="2"
            IsSnapToTickEnabled="True" IsMoveToPoint="True"></Сontrols:RangeSlider>

