---
layout: default
title: Panorama Control - MahApps.Metro
---

# Panorama Control
![](images/panorama.png)

`Panorama`s are sideways scrolling (typically tile) lists, found in Windows Phone 7, Windows 8 (the "metro" start screen) and in select parts of the Zune interface.

A `Panorama` can be databound, but because of it's nature it needs a different type of collection. Specifically, `Panorama`s bind to a(n observable) collection of `PanoramaGroup`s. 

    <Controls:Panorama ItemBox="140" ItemsSource="{Binding Groups}" />

And then on the `ViewModel`

	
	public ObservableCollection<PanoramaGroup> Groups { get; set; }
	...
	public ViewModel()
	{
		tracks = new PanoramaGroup("trending tracks");
		artists = new PanoramaGroup("trending artists");
		Groups = new ObservableCollection<PanoramaGroup> { tracks, artists };
	}
	
	...
	public void GetDataCallback()
	{
	 	tracks.SetSource(x.Tracks.track.Take(25));
	}

