---
layout: no-sidebar
title: ToggleButton
---

### Introduction

There are two kinds of styles for ToggleButtons available in MahApps.Metro.

The default style, available just by placing a `ToggleButton` control in XAML looks like the default MahApps.Metro button. 

```xml
<Grid>
    <ToggleButton/>
</Grid>
```

![]({{site.baseurl}}/images/toggle-button-normal.png")

Another style, `MetroCircleToggleButtonStyle` is available by setting the ToggleButton's style to `MetroCircleToggleButtonStyle`. This style changes the button's background to `AccentColorBrush` when it is checked. To modify this behaviour, you will have to edit a copy of the control template using Blend.

```xml
<ToggleButton Width="50"
              Height="50"
              Margin="0, 10, 0, 0"
              Style="{DynamicResource MetroCircleToggleButtonStyle}">
</ToggleButton>
```

![]({{site.baseurl}}/images/toggle-button-circle.png")

## Using Glyphs Within a Circle Toggle Button

In order to use glyphs, you will have to add a reference to `Icons.xaml`. 

```xml
<UserControl.Resources>
    <ResourceDictionary>
        <ResourceDictionary.MergedDictionaries>
            <ResourceDictionary Source="pack://application:,,,/MahApps.Metro.Resources;component/Icons.xaml" />
        </ResourceDictionary.MergedDictionaries>
    </ResourceDictionary>
</UserControl.Resources>
```

You can then include the icon you want using the following steps:

1.      Nest a `Rectangle` within the `ToggleButton`
2.      Set it's fill to the color you want the icon to have
3.      Set `Rectangle.OpacityMask` to contain a `VisualBrush` with a Visual using the icon's value as a reference.

For example:

```xml
<ToggleButton Width="50"
              Height="50"
              Margin="0, 10, 0, 0"
              Style="{DynamicResource MetroCircleToggleButtonStyle}">
    <Rectangle Width="20"
               Height="20"
               Fill="{DynamicResource BlackBrush}">
        <Rectangle.OpacityMask>
            <VisualBrush Stretch="Fill"
                         Visual="{DynamicResource appbar_city}"/>
        </Rectangle.OpacityMask>
    </Rectangle>
</ToggleButton>
```

### Syncing Checked State of ToggleButton with Foreground

By default, any icon you set will retain the same color you set it to even if the ToggleButton is checked. To alter this, you can bind your content's color to the ToggleButton's Foreground property which changes to white by default when it is checked.

An example of how to do the binding can be found below:

```xml
<ToggleButton Width="50"
              Height="50"
              Margin="0, 10, 0, 0"
              Style="{DynamicResource MetroCircleToggleButtonStyle}">
    <Rectangle Width="20"
               Height="20"
               Fill="{Binding Path=Foreground, RelativeSource={RelativeSource FindAncestor, AncestorType={x:Type ToggleButton}}}">
        <Rectangle.OpacityMask>
            <VisualBrush Stretch="Fill"
                         Visual="{DynamicResource appbar_city}"/>
        </Rectangle.OpacityMask>
    </Rectangle>
</ToggleButton>
```
