---
layout: no-sidebar
title: Tile
---
The `Tile` is exactly as it sounds. It is a rectangular control whose sole purpose is to mimick a tile from Window 8/WinRT's Start Screen.

![img](http://www.bleepstatic.com/tutorials/windows-8/introduction-start-screen/windows-8-start-screen.jpg)

### How to use the `Tile`

The following XAML will initialize a `Tile` control with its `Title` set to `"Hello!"` and its `Count` set to `1`.

```xml
<controls:Tile Title="Hello!" 
                    TiltFactor="2"
                    Width="100" Height="100" 
                    Count="1">
</controls:Tile>
```

### In the wild

Here are some screenshots of the `Tile` in action.

![img2](https://github-camo.global.ssl.fastly.net/4793fa88b9041fb4a1bba21aa2140ee7d0f32b3c/687474703a2f2f7777772e6e63756265642e6e65742f7a2f706963732f6d6168617070732f54696c65436f6e74656e74436f6c6f72732e706e67)
