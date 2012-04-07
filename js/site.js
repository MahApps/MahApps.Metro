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
		var headings = $("#content h1, #content h2");

		$.each(headings, function (index, heading) {
			headings[index] = $(heading);
		});

		$(window).scroll(function(){
			if (headings.length < 2) 
				return true;

			var scrolltop = $(window).scrollTop() || 0;
			if (scrolltop < (headings.get(1).offset().top + 15)) {
				$(".current-section").css({"opacity":0,"visibility":"hidden"});
				return false;
			}

			$(".current-section").css({"opacity":1,"visibility":"visible"});
			$.each(headings, function (index, value) {
				if (scrolltop >= (value.offset().top + 15)) {
					$(".current-section .name").text(value.text());					
				}
			});
		});

		$(".current-section a").click(function(){
			$(window).scrollTop(0);
			return false;
		})
	});
})(jQuery)