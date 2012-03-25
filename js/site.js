
// set up toc-pivot
$(function(){
	$('.metro-pivot').metroPivot({
		animationDuration: 350,
		headerOpacity: 0.25,
		fixedHeaders: true,
		headerSelector: function (item) { return item.children("h2").first(); },
		itemSelector: function (item) { return item.children("section"); },
		headerItemTemplate: function () { return $("<span class='header' />"); },
		pivotItemTemplate: function () { return $("<div class='pivotItem' />"); },
		itemsTemplate: function () { return $("<div class='items' />"); },
		headersTemplate: function () { return $("<div class='headers' />"); },
		controlInitialized: undefined,
		selectedItemChanged: undefined
	});
});
