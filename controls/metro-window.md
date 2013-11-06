---
layout: no-sidebar
title: MetroWindow
---

`MetroWindow` is detailed in the [Quick Start]({{site.baseurl}}/guides/quick-start.html) section. One property not detailed is the `SaveWindowPosition` (true/false, default false) option. Setting this property to `true` will mean on next launch, it will automatically be positioned and sized to what it was on exit. This is designed to improve UX and speed development as its one of those "plumbing" UI things that is done regularly.  

Be careful though - if a monitor is detached during application exit and restart, or if certain circumstances arise, your application may launch off screen. Be sure to provide a 'reset' option or handle that in code.
  