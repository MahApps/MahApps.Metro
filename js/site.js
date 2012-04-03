(function () {
	var nb = document.createElement('script'); 
	nb.type = 'text/javascript'; 
	nb.async = true;
	nb.src = 'http://s.prabir.me/nuget-button/0.2.1/nuget-button.min.js';
	var s = document.getElementsByTagName('script')[0];
	s.parentNode.insertBefore(nb, s);
})();
				
(function($) {
	$(document).ready(function(){

		var headings = [];

		var collectHeaders = function(){
			headings.push({"top":$(this).offset().top + 15,"text":$(this).text()});
		}

		if ($("#content h1").length > 1)
			$("#content h1").each(collectHeaders)
		if($("#content h2").length > 1) 
			$("#content h2").each(collectHeaders)
			
		$(window).scroll(function(){
			if (headings.length==0) 
				return true;
				
			var scrolltop = $(window).scrollTop() || 0;
			if (headings[0] && scrolltop < headings[0].top) {
				$(".current-section").css({"opacity":0,"visibility":"hidden"});
				return false;
			}
			
			$(".current-section").css({"opacity":1,"visibility":"visible"});
			for(var i in headings) {
				if(scrolltop >= headings[i].top) {
					$(".current-section .name").text(headings[i].text);
				}
			}
		});

		$(".current-section a").click(function(){
			$(window).scrollTop(0);
			return false;
		})
	});
})(jQuery)