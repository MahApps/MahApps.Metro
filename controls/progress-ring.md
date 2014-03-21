---
layout: no-sidebar
title: ProgressRing
---

The Progress Ring control is styled after a similar control in Windows 8 to indicate activity rather than a percentage of progress completed.

`<Controls:ProgressRing IsActive="True" />`

`IsActive` can easily be bound to a viewmodel property.

`<Controls:ProgressRing IsActive="{Binding IsActive}" />`

Override `Foreground` if you wish to change the colour.

```xml
<Controls:ProgressRing Foreground="{DynamicResource AccentColorBrush"/>
```

![]({{site.baseurl}}/images/progress_ring.gif)