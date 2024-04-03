/* ---------------------------------------
File:		website all effect
Date:       2022-05
--------------------------------------- */

//=====元素focus===================
jQuery.fn.setfocus = function () {
	return this.each(function () {
		var a = this;
		setTimeout(function () {
			try {
				a.focus()
			} catch (b) { }
		}, 0)
	})
};

//=====取值encode===================
function MODAhtmlEncode2(e, key) {
	var ele = document.createElement('span');
	var content = (key) ? e.attr(key) : e.textContent;
	ele.appendChild(document.createTextNode(content));
	return ele.innerHTML;
}

/* ================================================
Start of var set */
var $window = $(window),
	$document = $(document),
	$html = $('html, body'),
	$body = $('body'),
	$toTopBtn = $('#topBtn'),
	$loading = $('#loading'),
	$ftNav = $('.ftNav');

var resizeTimer,
	windowWidthUnder768 = false,
	windowWidthUnder992 = false,
	windowWidthUpper1200 = false,
	windowWidthUpper1940 = false,
	defaultFontSize = 2,
	ftH,
	isZh = true;

var FECommon = FECommon || {};

//=====整站功能====================
(function (FECommon) {
    "use strict";
	var basic = {
		init: function(){
		
			this.fn_toTopBtnShow();
			this.fn_hashRun();
			this.fn_wUnder768();
			this.fn_wUnder992();
			this.fn_wUpper1200();
			this.fn_wUpper1940();
			this.fn_setFontSize();
		}
	
		
		,fn_toTopBtnShow: function () {
			ftH = $('.ftBtm').outerHeight();
			//top button顯示隱藏
			($window.scrollTop() < 200)?$toTopBtn.removeClass('on'):$toTopBtn.addClass('on');
			if(!windowWidthUnder768){
				if($window.scrollTop() + $window.height() > $document.height() - ftH){
					// $toTopBtn.addClass('atBtm');
					$toTopBtn.css('bottom',ftH - 27);
				}else{
					// $toTopBtn.removeClass('atBtm');
					$toTopBtn.css('bottom',100);
				}
			}else{
				$toTopBtn.css('bottom','');
			}
		}
		,fn_hashRun: function(){
			//錨點轉跳
			// if (window.location.hash != '' && $(window.location.hash).length)toTopRun(window.location.hash);
			$body.on('click','a[href^="#"]', function(e) {
				toTopRun(e,$(this).attr('href'));
			});
			function toTopRun(e,_href) {
				var navH;
				if(_href == '#'){
					e.preventDefault();
					return false;
				}
				var $target = $(_href == '#toTop' ? 'html' : _href);
				if(windowWidthUnder992){
					if(windowWidthUnder768){
						//mobile header height
						navH = 80;
					}else{
						//pad header height
						navH = 116;
					}
				}else{
					//pc header height
					navH = 118;
				}
				var position = $target.stop().offset().top - navH;
				$html.stop().animate({ scrollTop: position },400,'linear', function() {
					//a11y_按top鈕回到頁首導盲磚
					if(_href == '#toTop'){
						$('#AU').focus();
					}
					//a11y_按跳到主要內容鈕focus到主內容
					if(_href == '#main'){
						$('#AC').focus();
					}
				});
				return false;
			}

		}
		,fn_wUnder768: function(){
			windowWidthUnder768 = ($window.outerWidth() <= 768) ? true : false;
		}
		,fn_wUnder992: function(){
			windowWidthUnder992 = ($window.outerWidth() <= 992) ? true : false;
		}
		,fn_wUpper1200: function(){
			windowWidthUpper1200 = ($window.outerWidth() >= 1200) ? true : false;
		}
		,fn_wUpper1940: function(){
			windowWidthUpper1940 = ($window.outerWidth() >= 1940) ? true : false;
		}
		,fn_setLocalStorage:function(key,data){
			//save font size
			localStorage.setItem(key, data);
		}
		,fn_getLocalStorage:function(key){
			//get storage font size
			if (!localStorage['fontSize']) {
				localStorage.setItem('fontSize', '2');
			}else{
				return localStorage['fontSize'];
			}
		}
		,fn_setFontSize:function(){
			defaultFontSize = this.fn_getLocalStorage('fontSize');
		}
		,fn_loadingOff: function(){
			$loading.fadeOut('slow');
		}
		,fn_loadingOn: function(){
			$loading.fadeIn('slow');
		}
	}
	FECommon.basicInit = function(){
		basic.init();
	}
	FECommon.basicToTopBtnShow = function(){
		basic.fn_toTopBtnShow();
	}
	FECommon.basicHashRun = function(){
		basic.fn_hashRun();
	}
	FECommon.basicWUnder768 = function(){
		basic.fn_wUnder768();
	}
	FECommon.basicWUnder992 = function(){
		basic.fn_wUnder992();
	}
	FECommon.basicWUpper1200 = function(){
		basic.fn_wUpper1200();
	}
	FECommon.basicWUpper1940 = function(){
		basic.fn_wUpper1940();
	}
	FECommon.basicSetFontSize = function(){
		basic.fn_setFontSize();
	}
	FECommon.basicSetLocalStorage = function(day,key,data){
		basic.fn_setLocalStorage(day,key,data);
	}
	FECommon.basicGetLocalStorage = function(){
		basic.fn_getLocalStorage();
	}
	FECommon.basicLoadingOff = function(){
		basic.fn_loadingOff();
	}
	FECommon.basicLoadingOn = function(){
		basic.fn_loadingOn();
	}
	/* END of basic 
	================================================*/

	var header = {
		init: function () {
			this.fn_fontSize();
		}
		,fn_fontSize:function(){
			var $fsNav = $('.fontSizeDdJs');
			var $fsNavOn = $('.fontSizeDdNow');
			var $fsSub = $fsNav.find('.dropdown-menu');
			var onTxt,onTitle,onNum;
			var ckTxt,ckTitle,ckNum;
			function getOn(){
				onTxt = MODAhtmlEncode2($fsNavOn.find('span')[0]).replace(/(\-|\+)/,'<sup>$1</sup>');
				onTitle = $fsNavOn.attr('title');
				onNum = $fsNavOn.attr('data-order');
			}
			getOn();
			$fsSub.find('button').on({
				click:function(){
					var $this = $(this);
					ckTxt = MODAhtmlEncode2($this[0]).replace(/(\-|\+)/,'<sup>$1</sup>');
					ckTitle = $this.attr('title');
					ckNum = $this.attr('data-order');
					$fsNavOn.attr({
						title:ckTitle,
						'data-order':ckNum
					}).find('span').html(ckTxt);
					$this.attr({
						title:onTitle,
						'data-order':onNum
					}).html(onTxt);
					getOn();
					$fsSub.find('li').sort(
						function(a,b){
							return $(a).find('button').attr("data-order") - $(b).find('button').attr("data-order");
						}
					).appendTo($fsSub);
					if(ckNum == 1){
						$('body').removeClass('fontSizeL fontSizeM fontSizeS').addClass('fontSizeL');
					}
					if(ckNum == 2){
						$('body').removeClass('fontSizeL fontSizeM fontSizeS').addClass('fontSizeM');
					}
					if(ckNum == 3){
						$('body').removeClass('fontSizeL fontSizeM fontSizeS').addClass('fontSizeS');
					}
					FECommon.basicSetLocalStorage('fontSize', ckNum);
					// FECommon.widgetMarqueeUpdate();
				}
			});
			if(defaultFontSize!=2){
				$fsNav.find('button[data-order='+defaultFontSize+']').click();
			}
		}

	}
	FECommon.headerInit = function(){
		header.init();
	}

	
	FECommon.headerFontSize = function(){
		header.fn_fontSize();
	}
	
	
	/* END of header 
	================================================*/


	var documentOnReady = {
		init: function () {
			FECommon.basicInit();
			FECommon.headerInit();
		
		}
	}
	FECommon.documentOnReadyInit = function(){
		documentOnReady.init();
	}
	/* END of documentOnReady 
	================================================*/

	
	// FECommon.documentOnLoadInit = function(){
	// 	documentOnLoad.init();
	// }
	/* END of documentOnLoad 
	================================================*/

	var documentOnResize = {
		init: function () {
			FECommon.basicWUnder768();
			FECommon.basicWUnder992();
			FECommon.basicWUpper1200();
			FECommon.basicWUpper1940();
		}
	}
	FECommon.documentOnResizeInit = function(){
		documentOnResize.init();
	}
	/* END of documentOnResize 
	================================================*/

	var documentOnScroll = {
		init: function () {
			FECommon.basicToTopBtnShow();
		}
	}
	FECommon.documentOnScrollInit = function(){
		documentOnScroll.init();
	}
	/* END of documentOnScroll 
	================================================*/

})(FECommon);



/* ================================================
    Start of run function */
$(document).ready(function(){
	FECommon.documentOnReadyInit();
	
	FECommon.basicLoadingOn();//開啟loading
	FECommon.basicLoadingOff();//關閉loading
	$(".navbar-toggle").click(function () {
		$("body").toggleClass('side-open');
		$(".menu").toggleClass('open');
    });
	
});

$window.on( 'load', FECommon.documentOnLoadInit);

$window.on( 'scroll', FECommon.documentOnScrollInit);

$window.on( 'resize', function() {
	clearTimeout(resizeTimer);
	resizeTimer = setTimeout(function() {
		FECommon.documentOnResizeInit();
	}, 250);
	
});