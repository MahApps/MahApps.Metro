/*
 * jQuery beforeafter plugin
 * @author admin@catchmyfame.com - http://www.catchmyfame.com
 * @version 1.4
 * @date September 19, 2011
 * @category jQuery plugin
 * @copyright (c) 2009 admin@catchmyfame.com (www.catchmyfame.com)
 * @license CC Attribution-NonCommercial-ShareAlike 3.0 Unported (CC BY-NC-SA 3.0) - http://creativecommons.org/licenses/by-nc-sa/3.0/
 */
(function($){
	$.fn.extend({ 
		beforeAfter: function(options)
		{
			var defaults = 
			{
				animateIntro : false,
				introDelay : 1000,
				introDuration : 1000,
				introPosition : .5,
				showFullLinks : true,
				beforeLinkText: 'Show only before',
				afterLinkText: 'Show only after',
				imagePath : './js/',
				cursor: 'pointer',
				clickSpeed: 600,
				linkDisplaySpeed: 200,
				dividerColor: '#888',
				enableKeyboard: false,
				keypressAmount: 20,
				onReady: function(){}
			};
		var options = $.extend(defaults, options);

		var randID =  Math.round(Math.random()*100000000);

    		return this.each(function() {
			var o=options;
			var obj = $(this);

			var imgWidth = $('img:first', obj).width();
			var imgHeight = $('img:first', obj).height();
			
			if( $('div',obj).length != 2 ) $('img',obj).wrap('<div>'); // For backwards compatability. Used to require images to be wrapped in div tags.

			$(obj)
			.width(imgWidth)
			.height(imgHeight)
			.css({'overflow':'hidden','position':'relative','padding':'0'});

			var bef = $('img:first', obj).attr('src');
			var aft = $('img:last', obj).attr('src');
			
			$('img:first', obj).attr('id','beforeimage'+randID);
			$('img:last', obj).attr('id','afterimage'+randID);
		
			// Create an inner div wrapper (dragwrapper) to hold the images.
			$(obj).prepend('<div id="dragwrapper'+randID+'"><div id="drag'+randID+'"><img width="8" height="56" alt="handle" src="'+o.imagePath+'handle.gif" id="handle'+randID+'" /></div></div>'); // Create drag handle
			$('#dragwrapper'+randID).css({'opacity':.25,'position':'absolute','padding':'0','left':(imgWidth*o.introPosition)-($('#handle'+randID).width()/2)+'px','z-index':'20'}).width($('#handle'+randID).width()).height(imgHeight);

			$('div:eq(2)', obj).height(imgHeight).width(imgWidth*o.introPosition).css({'position':'absolute','overflow':'hidden','left':'0px','z-index':'10'}); // Set CSS properties of the before image div
			$('div:eq(3)', obj).height(imgHeight).width(imgWidth).css({'position':'absolute','overflow':'hidden','right':'0px'});	// Set CSS properties of the after image div
			$('#drag'+randID).width(2).height(imgHeight).css({'background':o.dividerColor,'position':'absolute','left':'3px'});	// Set drag handle CSS properties
			$('#beforeimage'+randID).css({'position':'absolute','top':'0px','left':'0px'});
			$('#afterimage'+randID).css({'position':'absolute','top':'0px','right':'0px'});
			$('#handle'+randID).css({'z-index':'100','position':'relative','cursor':o.cursor,'top':(imgHeight/2)-($('#handle'+randID).height()/2)+'px','left':'-3px'})

			$(obj).append('<img src="'+o.imagePath+'lt-small.png" width="7" height="15" id="lt-arrow'+randID+'"><img src="'+o.imagePath+'rt-small.png" width="7" height="15" id="rt-arrow'+randID+'">');

			if(o.showFullLinks)
			{	
				$(obj).after('<div class="balinks" id="links'+randID+'" style="position:relative"><span class="balinks"><a id="showleft'+randID+'" href="javascript:void(0)">'+o.beforeLinkText+'</a></span><span class="balinks"><a id="showright'+randID+'" href="javascript:void(0)">'+o.afterLinkText+'</a></span></div>');
				$('#links'+randID).width(imgWidth);
				$('#showleft'+randID).css({'position':'relative','left':'0px'}).click(function(){
					$('div:eq(2)', obj).animate({width:imgWidth},o.linkDisplaySpeed);
					$('#dragwrapper'+randID).animate({left:imgWidth-$('#dragwrapper'+randID).width()+'px'},o.linkDisplaySpeed);
				});
				$('#showright'+randID).css({'position':'absolute','right':'0px'}).click(function(){
					$('div:eq(2)', obj).animate({width:0},o.linkDisplaySpeed);
					$('#dragwrapper'+randID).animate({left:'0px'},o.linkDisplaySpeed);
				});
			}

			if(o.enableKeyboard)
			{
				$(document).keydown(function(event){
					if(event.keyCode == 39)
					{
						if( (parseInt($('#dragwrapper'+randID).css('left'))+parseInt($('#dragwrapper'+randID).width()) + o.keypressAmount) <= imgWidth )
						{
							$('#dragwrapper'+randID).css('left', parseInt( $('#dragwrapper'+randID).css('left') ) + o.keypressAmount + 'px');
							$('div:eq(2)', obj).width( parseInt( $('div:eq(2)', obj).width() ) + o.keypressAmount + 'px' );
						}
						else
						{
							$('#dragwrapper'+randID).css('left', imgWidth - parseInt( $('#dragwrapper'+randID).width() ) + 'px');
							$('div:eq(2)', obj).width( imgWidth - parseInt( $('#dragwrapper'+randID).width() )/2 + 'px' );
						}
					}
					if(event.keyCode == 37)
					{
						if( (parseInt($('#dragwrapper'+randID).css('left')) - o.keypressAmount) >= 0 )
						{
							$('#dragwrapper'+randID).css('left', parseInt( $('#dragwrapper'+randID).css('left') ) - o.keypressAmount + 'px');
							$('div:eq(2)', obj).width( parseInt( $('div:eq(2)', obj).width() ) - o.keypressAmount + 'px' );
						}
						else
						{
							$('#dragwrapper'+randID).css('left', '0px');
							$('div:eq(2)', obj).width($('#dragwrapper'+randID).width()/2);
						}
					}
				});
			}

			$('#dragwrapper'+randID).draggable( { containment:obj,drag:drag,stop:drag });

			function drag()
			{
				$('#lt-arrow'+randID+', #rt-arrow'+randID).stop().css('opacity',0);
				$('div:eq(2)', obj).width( parseInt( $(this).css('left') ) + 4 );
			}

			if(o.animateIntro)
			{
				$('div:eq(2)', obj).width(imgWidth);
				$('#dragwrapper'+randID).css('left',imgWidth-($('#dragwrapper'+randID).width()/2)+'px');
				setTimeout(function(){
					$('#dragwrapper'+randID).css({'opacity':1}).animate({'left':(imgWidth*o.introPosition)-($('#dragwrapper'+randID).width()/2)+'px'},o.introDuration,function(){$('#dragwrapper'+randID).animate({'opacity':.25},1000)});
					$('div:eq(2)', obj).width(imgWidth).animate({'width':imgWidth*o.introPosition+'px'},o.introDuration,function(){clickit();o.onReady.call(this);});
				},o.introDelay);
			}
			else
			{
				clickit();
				o.onReady.call(this);
			}

			function clickit()
			{
				$(obj).hover(function(){
						$('#lt-arrow'+randID).stop().css({'z-index':'20','position':'absolute','top':imgHeight/2-$('#lt-arrow'+randID).height()/2+'px','left':parseInt($('#dragwrapper'+randID).css('left'))-10+'px'}).animate({opacity:1,left:parseInt($('#lt-arrow'+randID).css('left'))-6+'px'},200);
						$('#rt-arrow'+randID).stop().css({'position':'absolute','top':imgHeight/2-$('#lt-arrow'+randID).height()/2+'px','left':parseInt($('#dragwrapper'+randID).css('left'))+10+'px'}).animate({opacity:1,left:parseInt($('#rt-arrow'+randID).css('left'))+6+'px'},200);
						$('#dragwrapper'+randID).animate({'opacity':1},200);
					},function(){
						$('#lt-arrow'+randID).animate({opacity:0,left:parseInt($('#lt-arrow'+randID).css('left'))-6+'px'},350);
						$('#rt-arrow'+randID).animate({opacity:0,left:parseInt($('#rt-arrow'+randID).css('left'))+6+'px'},350);
						$('#dragwrapper'+randID).animate({'opacity':.25},350);
					}
				);

				// When clicking in the container, move the bar and imageholder divs
				$(obj).click(function(e){
					var clickX = e.pageX - $(this).offset().left;
					$('#dragwrapper'+randID).stop().animate({'left':clickX-($('#dragwrapper'+randID).width()/2)+'px'},o.clickSpeed);
					$('div:eq(2)', obj).stop().animate({'width':clickX+'px'},o.clickSpeed);
					$('#lt-arrow'+randID+',#rt-arrow'+randID).stop().animate({opacity:0},50);
				});
				$(obj).one('mousemove', function(){$('#dragwrapper'+randID).stop().animate({'opacity':1},500);}); // If the mouse is over the container and we animate the intro, we run this to change the opacity when the mouse moves since the hover event doesnt get triggered yet
			}
  		});
    	}
	});
})(jQuery);