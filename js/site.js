
$(function(){
	// add 'top' link to h2 and h3
	var headers = $('h2[class!="toc-header"], h3');
	headers.append(' <small class="top-link"><a href="#top">TOP</small>');
	// then hide it unless hovering
	$('.top-link').hide();
	headers.hover(
		function() { $('.top-link', this).fadeIn(100); },
		function() { $('.top-link', this).fadeOut(250); }
	);
});
