---
layout: default
title: TextBox - MahApps.Metro
---

# TextBox

There is just the one style in this library for `TextBox`, however it does have a special attached property for creating 'watermarked' textboxes and for added a 'clear' button.

**TODO**: example code snippet here

## Additional Features

### Watermark

A **watermark** - in the context of textboxes - refers to text that appears in the textbox *before* the user has focused or entered any text. This is often an alternative to providing a label next to your textbox - for example, a search box would have a watermark with the text 'enter search terms' to indicate it's purpose.

### An Example 

**TODO:** move this to a code snippet example

`<TextBox Controls:TextboxHelper.Watermark="This is a textbox" />`

Will produce a textbox that looks like the below image. The three states are *unfocused* (with no user text provided), focused, and unfocused (with user text provided).

![](/images/10_textboxstates.png)

### Why Attached Properties?

Rather than deriving from TextBox and adding another class to this library, this behaviour is implemented as an Attached Property.

This avoids the overhead of providing styles for another control, and makes it easy to "opt in" to this behaviour in your application.



**TODO:** isn't there another attached property?

