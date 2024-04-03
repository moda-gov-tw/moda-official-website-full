$(function () {
    "use strict";
    var $body = $("body");
    var $window = $(window);
    var resizeTimer;

    /* collapses the sidebar on window resize.
    Set min-height of #page-wrapper.*/
    var set = function () {
        var topOffset = 80,
            width = (window.innerWidth > 0) ? window.innerWidth : this.screen.width,
            height = ((window.innerHeight > 0) ? window.innerHeight : this.screen.height) - 1;
        var titleOffset = ($('.row.bg-title').length)?$('.row.bg-title').height() + 17.5 + 90 : 80;
        if (width < 768) {
            // console.log('隱藏sidebar');
            $('div.navbar-collapse').addClass('collapse');
            // topOffset = 100; 
        } else {
            // console.log('顯示sidebar');
            $('div.navbar-collapse').removeClass('collapse');
        }

        /* init show wide/slim sidebar */
        // (width < 1170)?slimSidebar():wideSidebar();

        /* get sidebar style from local storage */
        (getLocalStorage('sidebarStyle')=='slim')?slimSidebar():wideSidebar();

        /* set main contnet min-height */
        height = height - topOffset;
        if (height < 1) {
            height = 1;
        }
        if (height > topOffset) {
            $("#page-wrapper").css("min-height", (height) + "px");
            $(".row.bg-title").next('.row').css("min-height", (height - titleOffset) + "px");
        }
    },
    url = window.location.href,
    element = $('#side-menu a').filter(function () {
        var link = this.href;
        var activeItem = false;
        var exceptAry = ['UserManagement','WebsiteManagement','GroupManagement','CaseApplyClass','OpenData','CaseReconfirm','CaseApplyPage'];
        var urlPage = getPageName(url);
        var urlLast = getUrlLast(url);
        var linkLast = getUrlLast(link);
        if(urlPage == 'WebLevelManagement'){
            url = $(this).eq(0)[0].href;
        }else if(exceptAry.indexOf(urlPage) > -1){
            url = url + '/Index'
        }
        if(urlLast == 'CaseApply' && urlPage == 'ReSetDetail'){
            url = window.location.protocol + "//" + window.location.host + '/MailBox/CaseApply/ReSet'
        }
        if((urlLast == 'CaseApply' && urlPage == 'Detail') || (urlLast == 'MailBox' && urlPage == 'CaseApply')){
            url = window.location.protocol + "//" + window.location.host + '/MailBox/CaseApply/Index'
        }
        if((link === url) || (linkLast == urlLast && exceptAry.indexOf(urlLast) > -1)){activeItem = true;}
        return activeItem;
    }).addClass('active');
    
    /* set local storage for sidebar open/close */
    function getLocalStorage(key){
        if (!localStorage['sidebarStyle']) {
            localStorage.setItem('sidebarStyle', 'wide');
        }else{
            return localStorage['sidebarStyle'];
        }
    }
    function slimSidebar(){
        // console.log('窄sidebar');
        $body.addClass('content-wrapper');
        $(".open-close i").removeClass('icon-arrow-left-circle');
        $(".logo b").css('display','none');
    }
    function wideSidebar(){
        // console.log('寬sidebar');
        $body.removeClass('content-wrapper');
        $(".open-close i").addClass('icon-arrow-left-circle');
        $(".logo b").css('display','inline-block');
    }
    /* get url page name */
    function getPageName(url){
        var _url = url.substring(0, (url.indexOf("?") == -1) ? url.length : url.indexOf("?"));
        _url = _url.substring(0, (url.indexOf("#") == -1) ? url.length : url.indexOf("#"));
        return _url.split('/').pop();
    }
    /* get url last part */
    function getUrlLast(url){
        var urlAry = url.split('/');
        return urlAry[urlAry.length-2];
    }

    $window.ready(set);
    // $(window).on("resize", set);
    $window.on( 'resize', function() {
        clearTimeout(resizeTimer);
        resizeTimer = setTimeout(set, 5);
    });

    
    /* close loading */
    $(".preloader").fadeOut();

    /* Sidebar scrollbar */
    $('.slimscrollsidebar').slimScroll({
        height: '100%',
        position: 'right',
        size: "5px",
        color: '#dcdcdc',
        // alwaysVisible: true
    });
    $(".sidebar-nav, .slimScrollDiv").css("overflow-x", "visible").parent().css("overflow", "visible");

    /* set menu tree */
    // $('#side-menu').metisMenu();

    /* click on open close button Sidebar open close */
    $(".open-close").on('click', function () {
        if(!$body.hasClass("content-wrapper")){
            slimSidebar();
            localStorage.setItem('sidebarStyle','slim');
        }else{
            wideSidebar();
            localStorage.setItem('sidebarStyle','wide');
        }
    });

    /* sidebar open/close for mobile */
    $(".navbar-toggle").on("click", function () {
        $(".navbar-toggle i").toggleClass("ti-menu").addClass("ti-close");
        $(".navbar-collapse").toggleClass('show');
    });

    /* fix multiple modals overlay */
    $(document).on('show.bs.modal', '.modal', function (event) {
        var zIndex = 1040 + (10 * $('.modal:visible').length);
        $(this).css('z-index', zIndex);
        setTimeout(function() {
            $('.modal-backdrop').not('.modal-stack').css('z-index', zIndex - 1).addClass('modal-stack');
        }, 0);
    });
});