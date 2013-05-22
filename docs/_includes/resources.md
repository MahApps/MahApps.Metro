##Resources


###Icons

MahApps.Metro does not embed any icons or "resources" other than control styles. However, there is *[MahApps.Metro.Resources](http://nuget.org/packages?q=MahApps.Metro.Resources)* package. This allows better discoverability and customisation.

You can install this package using nuget:

<pre class="nuget-button">Install-Package MahApps.Metro.Resources</pre>

Currently, this consists of [Entypo](http://www.entypo.com/) and [Temparian's Windows Phone Icon pack](http://templarian.com/project_windows_phone_icons/)

![](images/6_Resources.png)


####Usage
The resources are simply `Canvas`'s wrapping one or more `Path`s. To use these sorts of elements, you can just use WPF's `VisualBrush`.

	<Window.Resources>
        <ResourceDictionary>
            <ResourceDictionary.MergedDictionaries>
                <ResourceDictionary Source="/Resources/Icons.xaml" />
            </ResourceDictionary.MergedDictionaries>
        </ResourceDictionary>
    </Window.Resources>
    
    <Rectangle>
        <Rectangle.Fill>
            <VisualBrush Visual="{StaticResource appbar_add}" />
        </Rectangle.Fill>
    </Rectangle>
    
If you're trying to create "circle" buttons ala Windows Phone 7, the easiest way is to set the `VisualBrush` to be an *`OpacityMask`* on the `Rectangle`. This means you just need to alter the `Rectangle` colours on state change (hover, mouse down, etc)

	<Rectangle Fill="Black">
		<Rectangle.OpacityMask>
			<VisualBrush Visual="{StaticResource appbar_add}" Stretch="Fill" />
		</Rectangle.OpacityMask>
	</Rectangle>