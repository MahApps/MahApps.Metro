## Known issues

### No automatic binding on WindowCommands with Caliburn.Micro
[Caliburn.Micro][caliburn_micro] automatically binds on the visual tree and a couple of other locations, which doesn't cover how MahApps.Metro's awesome `WindowCommands` are implemented. To work around this issue for a simple button to action binding, first add the `cal` namespace to the root element (usually `Controls:MetroWindow`):

    xmlns:cal="http://www.caliburnproject.org"
    
Then explicity attach the button to the action:

	<Controls:MetroWindow.WindowCommands>
		<Controls:WindowCommands>
			<Button x:name="SayWat" Content="wat" cal:Message.Attach="SayWat"/>
		</Controls:WindowCommands">
	</Controls:MetroWindow.WindowCommands>
        


[caliburn_micro]: http://caliburnmicro.codeplex.com