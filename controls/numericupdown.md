----
layout: no-sidebar
title: NumericUpDown - MahApps.Metro
----

> **Note:** This is the documentation for the new `NumericUpDown` control that isn't implemented yet. It will be available in v0.13

The NumericUpDown control is used to increase or decrease a numeric value.

![]({{site.baseurl}}/images/numeric_up_down.png)

If you press the  `+` button the value of the NumericUpDown control increases by the value set in `Interval`. Pressing `-` decreases the value.
If you press and hold `+` or `-` then value keeps increasing, decreasing respectively.

### SpeedUp

If you press and hold `+` or `-` for some longe time then the `Value` increases or decreases much faster. If this behaviour is not desired you can turn this off by setting `Speedup=false`. The default value is `true`.

### InterceptArrowKeys

You can also increase or decrease the `Value` using `Arrow Up` or `Arrow Down`. You can enable this behaviour if you set `InterceptArrowKeys=true`. By default this behaviour is enabled. The default value is `true`.

### InterceptMouseWheel

Like with InterceptArrowKeys you can change the `Value` by using the mouse wheel. Scrolling up will increase `Value` by `Interval`, scrolling down decrease, respectively. The default value is `true`.

###Minimum / Maximum

By specifying a value for `Minimum` or `Maximum` you can set the range for legal values.

### StringFormat

You can also set the `StringFormat` to format the number of the Value that is displayed in the control.

e.g.

* C2
* N4
* E1
* "{}{0:N2} psc"

## Example

Following line will provide a NumericUpDown that allows numers from 0 to 1000. Furthermore by pressing `+` the value gets increased by 5. The value will be shown as currency with two decimal places:
`<Controls:NumericUpDown Minimum = 0, Maximum = 10000, Interval = 5, StringFormat="C2"/>`



