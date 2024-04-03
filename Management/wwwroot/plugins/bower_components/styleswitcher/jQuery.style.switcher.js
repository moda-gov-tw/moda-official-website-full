// Theme color settings
$(document).ready(function () {

    /* ===========================================================
        Right Sidebar 設定
    =========================================================== */
    //var RightSidebar = new function () {
    //    // 私有
    //    var urlTarget = "/Admin/Handler/RightSidebar.ashx";

    //    // 公有
    //    return {
    //        theme: "",
    //        init: function () {
    //            $.ajax({
    //                type: 'get',
    //                url: urlTarget + "?Mod=get",
    //                datatype: "json",
    //                success: function (result) {
    //                    $('#theme').attr({ href: 'css/colors/' + result.Theme + '.css' });
    //                    $('#themecolors li a').removeClass('working');
    //                    $('#themecolors li a[theme=' + result.Theme + ']').addClass('working');
    //                },
    //                error: function () { }
    //            });
    //        },
    //        switch: function () {
    //            $.ajax({
    //                type: 'post',
    //                url: urlTarget + "?Mod=post",
    //                data: {
    //                    theme: this.theme
    //                },
    //                success: function () { },
    //                error: function () { }
    //            });
    //        }
    //    }
    //};

    // RightSidebar.init();

    function store(name, val) {
        if (typeof (Storage) !== "undefined") {
            localStorage.setItem(name, val);
        } else {
            window.alert('Please use a modern browser to properly view this template!');
        }
    }
    $("*[theme]").click(function (e) {
        e.preventDefault();
        var currentStyle = $(this).attr('theme');
        store('theme', currentStyle);
        RightSidebar.theme = currentStyle;
        RightSidebar.switch();
        $('#theme').attr({ href: 'css/colors/' + currentStyle + '.css' })
    });

    var currentTheme = get('theme');
    if (currentTheme) {
        $('#theme').attr({ href: 'css/colors/' + currentTheme + '.css' });
    }
    // color selector
    $('#themecolors').on('click', 'a', function () {
        $('#themecolors li a').removeClass('working');
        $(this).addClass('working')
    });
    $("*[theme]").click(function (e) {
        e.preventDefault();
        var currentStyle = $(this).attr('theme');
        store('theme', currentStyle);
        $('#theme').attr({ href: 'css/colors/' + currentStyle + '.css' })
    });

    var currentTheme = get('theme');
    if (currentTheme) {
        $('#theme').attr({ href: 'css/colors/' + currentTheme + '.css' });
    }
    // color selector
    $('#themecolors').on('click', 'a', function () {
        $('#themecolors li a').removeClass('working');
        $(this).addClass('working')
    });
});

function get(name) {

}
