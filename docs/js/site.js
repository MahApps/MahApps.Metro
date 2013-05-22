(function () {
	var nb = document.createElement('script'); 
	nb.type = 'text/javascript'; 
	nb.async = true;
	nb.src = 'http://s.prabir.me/nuget-button/0.2.1/nuget-button.min.js';
	var s = document.getElementsByTagName('script')[0];
	s.parentNode.insertBefore(nb, s);
})();

var template = '{{#contributors}}<li><img src="http://gravatar.com/avatar/{{gravatar_id}}?s=25" alt="{{name}}" />  <a href="https://github.com/{{login}}">{{login}}</a> - ({{contributions}} commits) </li>{{/contributors}}';
function getContribs() {
	$.ajax({
		url: "http://github.com/api/v2/json/repos/show/MahApps/MahApps.Metro/contributors",
		dataType: 'jsonp',
		success: function(data) 
		{
			data.contributors = data.contributors.sort(function (a, b) 
			{ 
				if (a.contributions > b.contributions) return -1;
				if (a.contributions < b.contributions) return 1;
				return 0;
			});
			
			var output = Mustache.render(template, data);
			$("#contributors").html(output);
			//$("#contributorTemplate").tmpl(data).appendTo("#contributors");
		}
	});
  }

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
		
		getContribs();
	});
})(jQuery)