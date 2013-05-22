---
layout: default
title: TransitioningContentControl - MahApps.Metro
---

# TransitioningContentControl

Taken from Silverlight (specifically [this](https://github.com/jenspettersson/WPF-Controls) port), `TransitioningContentControl` is great for switching content smoothly around. At it's core, `TransitioningContentControl` is a `ContentControl`, so only one child element can be displayed at a time. When you change the content, an animation is played switching the two.

**TODO:** code sample which has this snippet in it

`<Controls:TransitioningContentControl x:Name="transitioning" Width="150" Height="50" Transition="DownTransition" />`

Built in there are several transitions:  

* DownTransition
* UpTransition
* RightTransition
* LeftTransition
* RightReplaceTransition
* LeftReplaceTransition
                            
**TODO**: did we get this "custom transition" issue sorted?

This is still a 'work in progress' control, so there are some limitations - at the moment you can't provide a custom transition without overriding the style of the control.
