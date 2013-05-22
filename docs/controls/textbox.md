---
layout: default
title: TextBox - MahApps.Metro
---

There is just the one style in this library for `TextBox`, however it does have a special attached property for creating 'watermarked' textboxes and for added a 'clear' button.

**What is a watermark?**  
Watermarked - in the context of textboxes - refers to text that appears in the textbox *before* the user has focussed or entered text. This is often an alternative to having a set of labels, you can instead just have a textbox with a watermark like 'search terms go here'.

**Why AttachedProperty?**  
The easiest way (for me) would have been to add a custom control, something like `<WatermarkedTextBox`, but then that's another control you have to use rather than just the style definitions at the top. The attachedproperty makes it entirely opt in if you want the watermark.

####Watermark usage

``<TextBox Controls:TextboxHelper.Watermark="This is a textbox" />``

Will produce a textbox that looks like the below image. The three states are *unfocussed* with no user text entered, focussed, and unfocussed with user text.

![](/images/10_textboxstates.png)



