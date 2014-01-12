---
layout: no-sidebar
title: NumericUpDown - MahApps.Metro
---

The NumericUpDown control is used to increase or decrease a numeric value.


![]({{site.baseurl}}/images/numeric_up_down.png)

The `+` increases the value of the NumericUpDown by `SmallChange`. Pressing `-` decreases the value.
If you press and hold `+` or `-` then value keeps increasing, decreasing respectively.

If you press and hold `+` or `-` for some longe time then the `Value` increases or decreases much faster. If this behaviour is not desired you can turn this off by setting `Speedup=false`

By specifying a value for `Minimum` or `Maximum` you can set the range for legal values.

You can also set the `StringFormat` to any valid 

##Example

Following line will provide a NumericUpDown that allows numers from 0 to 1000. Furthermore by pressing `+` the value gets increased by 5. The value will be shown as currency with two decimal places:
`<Controls:NumericUpDown Minimum = 0, Maximum = 10000 SmallChange = 5, StringFormat="C2"/>`
