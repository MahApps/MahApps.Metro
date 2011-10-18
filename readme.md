The goal of MahApps.Metro is to allow devs to quickly and easily cobble togther a "Metro" UI for their WPF4+ apps, with minimal effort.

[I welcome anybody who would like to contribute.][4]

Contents
--------

Currently MahApps.Metro has implicit styles for the following controls

 - Button
 - TextBox
 - PasswordBox
 - CheckBox
 - RadioButton
 - ComboBox
 - ComboBoxItem
 - ScrollBar
 - ToolTip
 - ContextMenu *
 - MenuItem
 - Progress bar
 - Tab Item

And a few extra controls

 - ToggleSwitch ([which I've discussed in the past][1])
 - MetroContentControl (from [Joe McBride][2])
 - Progress indicator (ala the 'IsIndeterminate' look from WP7)

**There is an issue with ContextMenu's with implicit styles in WPF for any control that uses the standard "Cut/Copy/Paste" menu, [go vote for the issue on Connect][3]*

Getting started with MahApps.Metro
-------------

**Step 1**: Grab MahApps.Metro from Nuget  
  
**Step 2**: In your `App.xaml` (there are a few caveats to this - see the *Themes and Accents* section below), add the following:  

    <Application.Resources>
        <ResourceDictionary>
            <ResourceDictionary.MergedDictionaries>
                <ResourceDictionary Source="pack://application:,,,/MahApps.Metro;component/Styles/Colours.xaml"/>
                <ResourceDictionary Source="pack://application:,,,/MahApps.Metro;component/Styles/Fonts.xaml"/>
                <ResourceDictionary Source="pack://application:,,,/MahApps.Metro;component/Styles/Controls.xaml"/>
            </ResourceDictionary.MergedDictionaries>
        </ResourceDictionary>
    </Application.Resources>

The order is important - Controls must come after Colours, else things go a little haywire.

![Default Colours][5]
  

**Step 4**: (Optional) This example also makes use of the `MahApps.Metro.Controls.ToggleSwitch` control, based on the ToggleSwitch in Microsoft's WP7 Toolkit. Add a reference to `MahApps.Metro.Controls.dll`, then add the reference in XAML, and it is used like any other control. ie

    <Window 
    ...
    xmlns:Controls="clr-namespace:MahApps.Metro.Controls;assembly=MahApps.Metro.Controls" >
    ...
    <Controls:ToggleSwitch HorizontalAlignment="Right" VerticalAlignment="Top" Margin="0,44,8,0" />
    ...
  

Themes & Accents
-------------------

Like WP7, I wanted to provide an easy way to switch the colours of these controls to something to suit your app - be it your logo/branding, user selection, or something else entirely different. However if you have used WP7, you'll know there are two parts to customisation - "theme" and "accent". Theme refers to light or dark, and accent to the highlight colours such as blue, red, green, etc.

To make it easier, I've created a BaseLight.xaml and BaseDark.xaml that can be used in conjunction with an accent. **While the capability is there to apply themes and accents, this part has recieved little attention yet, and as such I'd recommend rolling your own BaseLight/BaseDark and Accent.** You can apply it like so:

    private void ApplyLightGreen()
    {
        var greenRd = new ResourceDictionary();
        var lightRd = new ResourceDictionary();
    
        greenRd.Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Accents/Green.xaml");
        lightRd.Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Accents/BaseLight.xaml");
    
    
        ApplyResourceDictionary(greenRd);
        ApplyResourceDictionary(lightRd);
    }
    
    private void ApplyResourceDictionary(ResourceDictionary rd)
    {
        foreach (DictionaryEntry r in rd)
        {
            if (Resources.Contains(r.Key))
                Resources.Remove(r.Key);
    
            Resources.Add(r.Key, r.Value);
        }
    }


This will give you  
![light green accent][6]

If you were to apply Dark+Red, you'd end up with (remember this is a work in progress), 

![dark red accent][7]

**WARNING: WPF has some very interesting bugs.**
If you want to apply accents/themes dynamically at runtime, you *cannot* apply the MahApps.Metro ResourceDictionaries in App.xaml, they have to be applied in *each individual Window*. This is crazy, I know, but [unfortunately this is just how it is at the moment.][8]

**Keep in mind, [there are some bugs when you move it to Window Resource's over App Resources][9], argh!**


Icons and other resources
-------------------------
Currently MahApps.Metro includes no icons, however I *highly recommend* [Templarian][23]'s "[Project: Windows Phone Icons][22]". There is even [great tutorial on using Expression Design][24] to create vector (XAML!) metro icons designed to fit into this pack. (Which is fantastic, [somebody needs to show Microsoft this so they can learn!][25])  

![img](http://images.theleagueofpaul.com/postimages/wp_icons_features.png)

At the time of writing, this pack incldues a whopping 264 icons. All for free. And he's taking suggestions on more icons! It's hard to ask for much more than that.

There are other also fantastic CC-licensed resources out there, that may not be "designed for" Metro, but suit it well.

* ["SomeRandomDude"'s Iconic project][12] has a certain Metro-y feel to it, and is also CC licensed, and is one of my favourites. 
* Bil Simser packaged up a bunch of The Noun Project icons into a [big pack with PNG, SVG and *XAML* versions of the icons.][10] ([Noun Project][11] is the base, CC licensed). 
* ["108 mono Icons"](http://www.tutorial9.net/downloads/108-mono-icons-huge-set-of-minimal-icons/) has no specific license attached but says it is free for use without attribution for personal and commercial projects.
* ["Token"](http://brsev.deviantart.com/art/Token-128429570) can be used for *non-commercial* uses, or there are commercial (paid) licensing options  

There are of course, other packs out there such such as [MetroStation][13] and [Window's Wiki WP7 icons][14], but I was unable to determine the license of those packs so proceed with care.


License
-------

MahApps.Metro is released under a [MS-PL license][15], so that its friendly to devs of open and closed source software.


MahApps.Metro in the Wild
-------

* [Windows Phone Power Tools](http://wptools.codeplex.com/)
* Silverlight Spy (["vNext"](http://yfrog.com/kka0fp))
* [Kompressah](https://bitbucket.org/aeoth/kompressah)
* MahTweets (v4/dev branch)
* [MahChats](http://www.mahchats.com)

(Contact me if you want to be added to this list!)

References
----------

  - [Creating a Metro UI Style Control][16], Joe McBride, November 4, 2010
  - [New Silverlight 4 Themes Available][17], Tim Heuer, May 3, 2010
  - [WPF: Textbox contextmenus don't inherit the implicit contextmenu style for your application][18]
  - [MSDN Forum: DynamicResource in nested resource dictionary not working][19]
  - [Using Silverlight 4 features to create a Zune-like context menu][20], Jeff Wilcox, May 15, 2010
  - [Zune Borderless Window][21], Jeremiah Morrill, October 18, 2010


  [1]: http://www.theleagueofpaul.com/wp7-toggle-switch-in-wpf
  [2]: http://xamlcoder.com/blog/2010/11/04/creating-a-metro-ui-style-control/
  [3]: http://connect.microsoft.com/VisualStudio/feedback/details/520516/wpf-textbox-contextmenus-dont-inherit-the-implicit-contextmenu-style-for-your-application
  [4]: https://github.com/mahapps/mahapps.metro
  [5]: http://images.theleagueofpaul.com/postimages/accent_default.png
  [6]: http://images.theleagueofpaul.com/postimages/green_2.png
  [7]: http://images.theleagueofpaul.com/postimages/dark_red_2.png
  [8]: https://connect.microsoft.com/VisualStudio/feedback/details/633535/msdn-forum-dynamicresource-in-nested-resource-dictionary-not-working
  [9]: /wpf-resourcedictionary-bugs
  [10]: http://weblogs.asp.net/bsimser/archive/2011/01/09/500-metro-style-wp7-icons.aspx
  [11]: http://www.thenounproject.com/
  [12]: http://somerandomdude.com/projects/iconic/
  [13]: http://yankoa.deviantart.com/#/d312tty
  [14]: http://www.windowswiki.info/2010/03/29/metro-icon-pack-windows-phone-7-icons/
  [15]: http://www.opensource.org/licenses/ms-pl.html959595
  [16]: http://xamlcoder.com/blog/2010/11/04/creating-a-metro-ui-style-control/
  [17]: http://timheuer.com/blog/archive/2010/05/03/new-silverlight-4-themes-available-for-download.aspx
  [18]: http://connect.microsoft.com/VisualStudio/feedback/details/520516/wpf-textbox-contextmenus-dont-inherit-the-implicit-contextmenu-style-for-your-application
  [19]: https://connect.microsoft.com/VisualStudio/feedback/details/633535/msdn-forum-dynamicresource-in-nested-resource-dictionary-not-working
  [20]: http://www.jeff.wilcox.name/2010/05/zunelike-contextmenu-style/125
  [21]: http://gallery.expression.microsoft.com/ZuneWindowBehavior/view/Discussions/125
  [22]: http://templarian.com/project_windows_phone_icons/
  [23]: http://templarian.com
  [24]: http://templarian.com/2011/08/06/tutorial_creating_an_icon/
  [25]: http://www.theleagueofpaul.com/how-not-to-do-metro