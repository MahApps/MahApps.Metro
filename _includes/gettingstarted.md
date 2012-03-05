<h2 class="accent">Getting Started</h2>

###Installing MahApps.Metro
You can install MahApps.Metro via Nuget using the GUI (right click on your project, Manage Nuget References, search for 'MahApps.Metro') or via the console `PM> MahApps.Metro`

If you wish to use the *alpha*/nightly releases of MahApps.Metro, you need to use the console

				TODO

This documentation assumes the latest stable release (at the time of writing, 0.6)

###Styling a Window
There are two main approaches you can take with MahApps.Metro to style a Window, using the `MetroWindow` control and rolling your own. For the getting started guide, we'll cover and assume `MetroWindow` as this approach will work for a good percentage of apps, and is the quickest and easiest way to get going. If you wish to learn more about rolling your own, it's covered in the "Advanced" section of Getting Started.

A default WPF Window with a few controls looks like so

![](images/01_UnstyledWindow.png)

After installing MahApps.Metro into your project, 

* open up MainWindow.xaml,
* add a namespace reference in the opening Window tag by adding  
`xmlns:Controls="clr-namespace:MahApps.Metro.Controls;assembly=MahApps.Metro"`
* and change `<Window ...` to `<Controls:MetroWindow ...` (and change the closing tag too!)

You'll need to change the codebehind from  
`public partial class MainWindow : Window` to  
`public partial class MainWindow : MetroWindow` 

That being said - usually you can just drop the inheritance on a partial declaration. This basic MetroWindow will result in...

![](images/02_PartiallyStyledWindow.png)

A complete and utter mess! Oh no! What has gone wrong? Nothing, we just haven't added the *resources and styles* yet. Unfortunately you need to include these resources in each Window. We have tried embedding this in `MetroWindow`, but then you lose all ability to dynamically change the theme.

Just under the opening MetroWindow tag, add the following

	   <Window.Resources>
	        <ResourceDictionary>
	            <ResourceDictionary.MergedDictionaries>
	                <ResourceDictionary Source="pack://application:,,,/MahApps.Metro;component/Styles/Colours.xaml" />
	                <ResourceDictionary Source="pack://application:,,,/MahApps.Metro;component/Styles/Fonts.xaml" />
	                <ResourceDictionary Source="pack://application:,,,/MahApps.Metro;component/Styles/Controls.xaml" />
	                <ResourceDictionary Source="pack://application:,,,/MahApps.Metro;component/Styles/Controls.AnimatedSingleRowTabControl.xaml" />
	                <ResourceDictionary Source="pack://application:,,,/MahApps.Metro;component/Icons/MergedResources.xaml" />
	                <ResourceDictionary Source="pack://application:,,,/MahApps.Metro;component/Styles/Accents/Blue.xaml" />
	                <ResourceDictionary Source="pack://application:,,,/MahApps.Metro;component/Styles/Accents/BaseLight.xaml" />
	            </ResourceDictionary.MergedDictionaries>
	        </ResourceDictionary>
	    </Window.Resources>
	
![](images/03_StyledWindow.png)

And now we have a semi-decent looking window!

###Explaining the elements
![](images/04_ExplainedStyledWindow.png)

If you don't like the elements that are labelled, fear not, they're all optional.

1. The titlebar is what sets `MetroWindow` apart from rolling your own. `ShowTitleBar="true|false"`
2. Instead of using static images, this uses a font called **Marlett**.
3. The resize grip is the *only* way to resize a `MetroWindow`, other than in code or with the min/max buttons. For dialogs where you may not want the user resizing the window,
4. `ShowIconOnTitleBar="true|false"` 

###Customisation
**WindowCommands**  
`WindowCommands` are the minimise, maximise/restore, and close buttons. As over MahApps.Metro 0.7, these can be extended to include your own buttons too.

	<Controls:MetroWindow.WindowCommands>
	    <Controls:WindowCommands>
	        <Button Content="settings" />
	            <Button >
	                <StackPanel Orientation="Horizontal">
	                    <Rectangle Width="20" Height="20">
	                        <Rectangle.Resources>
	                            <SolidColorBrush x:Key="BlackBrush" Color="White" />
	                        </Rectangle.Resources>
	                        <Rectangle.Fill>
	                            <VisualBrush Stretch="Fill" Visual="{StaticResource appbar_cupcake}" />
	                        </Rectangle.Fill>
	                    </Rectangle>
	                    <TextBlock Text="  deploy cupcakes" />
	                </StackPanel>
	            </Button>
	        </Controls:WindowCommands>
	</Controls:MetroWindow.WindowCommands>

Will produce   
![](images/05_WindowCommands.png)

`WindowCommands` foreground (link) colour will always be white *unless* you disable the titlebar, then it'll be the reverse of whatever theme you've selected. ie, for White/Light theme, the buttons will be black.

###Advanced

####Roll your own Window
The roll your own approach is still very relevant depending on what style of app you're going for. For [MarkPad](http://code52.org/DownmarkerWPF/), we needed the flexibility of rolling our own.

The key elements we used were the `WindowsCommand`, and `BorderlessWindowBehavior`.

**BorderlessWindowBehavior**  
Add a reference (in the opening Window tag) to `xmlns:i="http://schemas.microsoft.com/expression/2010/interactivity"` and to `xmlns:Behaviours="clr-namespace:MahApps.Metro.Behaviours;assembly=MahApps.Metro"`

Then somewhere within the Window tags, add

	    <i:Interaction.Behaviors>
	        <Behaviours:BorderlessWindowBehavior/>
	    </i:Interaction.Behaviors>

And you're done! `BorderlessWindowBehavior` has a few options, such as `ResizeWithGrip` and `AutoSizeToContent`.

	<Behaviours:BorderlessWindowBehavior ResizeWithGrip="False" />