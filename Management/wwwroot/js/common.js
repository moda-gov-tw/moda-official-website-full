
//判斷NULL
function strIsNull(str) {
    if (str != null
        && str.replace(/(^\s*)|(\s*$)/g, '').length > 0) {
        return false;
    }
    return true;
}
//判斷是否email
function strIsEmail(email) {
    var regex = /^([a-zA-Z0-9_\.\-\+])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$/;
    if (!regex.test(email)) {
        return false;
    } else {
        return true;
    }
}
/*
 * 判斷中文是否存在
 */
function strIsChinese(str) {
    var pattern = new RegExp("[\u4E00-\u9FA5]+");
    if (pattern.test(str)) { return true }
    return false;
}

function pop(obj) {
    $("#divShowbox").load(obj.popUrl, function () {
        var divhtml = $("#divShowbox").html();
        $("#divShowbox").html("");
        Swal.fire({
            title: '',
            width: 800,
            padding: 5,
            allowOutsideClick: false,
            html: divhtml,
            type: "success",
            showConfirmButton: false,

        });
    });
}
// val
function MODASelectVal(name) {
    var val = $("select[name=" + name + "] option:selected").val();
    var ele = document.createElement('span');
    ele.appendChild(document.createTextNode(val));
    return MODAhtmlDecode(ele.innerHTML);
}

function MODAhtmlEncode(e, type) {
    var ele = document.createElement('span');
    switch (type) {
        case "val":
            ele.appendChild(document.createTextNode(e.val()));
            break;
        case "text":
            ele.appendChild(document.createTextNode(e.text()));
            break;
    }
    return MODAhtmlDecode(ele.innerHTML);
}

function MODAhtmlDecode(text) {
    var temp = document.createElement("div");
    temp.innerHTML = text;
    var output = temp.innerText || temp.textContent;
    temp = null;
    return output;
}
//抓去物件的attr內的資料
function getObjAtr(e, attrName) {
    return e.attr(attrName);
}
////禁止用瀏覽器的上一步功能
function disableBack() {

    if (window.history && window.history.pushState) {
        $(window).on("popstate", function () {
            alert("請使用左邊選單");
            window.history.pushState("forward", null, "");
            window.history.forward(1);
        });
    }
    window.history.pushState("forward", null, "");
    window.history.forward(1);
}
disableBack();
var copyurl ;
///複製功能 title 標題   txt複製文字
function txtCopy(title, txt) {
    var copytxt = txt;
    //if (txt.indexOf("http") < 0) {
    //    copytxt = copyurl + txt;
    //}
    navigator.clipboard.writeText(copytxt)
        .then(() => {
            Swal.fire({
                icon: "warning",
                title: title + "  內容已經複製"
            });
        })
        .catch(err => {
            console.log('Something went wrong', err);
        })
}
function txtHerf(href) {
    window.open(href);
}
function mandatory(key) {
    location.href = '/Common/GetFile?fileID=' + key + "&ft=1";
}
function imgerror(e) {
    var filePath = "/images/error/file.png";
    e[0].src = filePath;
}
//抓取顯示筆數
function diCountFunction(preset, perPageShow, backperPageShow) {
    if (perPageShow != "undefined") {
        return perPageShow;
    }
    if (perPageShow == "undefined" && backperPageShow != 0) {
        return backperPageShow;
    }
    return preset;
}
//取cookies函式  
function getCookie(key) {
    var name = key + "=";
    var ca = document.cookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i].trim();
        if (c.indexOf(name) == 0) {
            var EA = c.substring(name.length, c.length);
            return EA;
        }
    }
    return "";
}
//兩個引數，一個是cookie的名子，一個是值
function SetCookie(name, value) {
    var d = new Date();
    var strjson = JSON.stringify(value);
    d.setTime(d.getTime() + (1 * 2 * 60 * 60 * 1000)); //以1 hours 計算
    var expires = "expires=" + d.toGMTString();
    document.cookie = name + "=" + strjson + ";" + expires + ";cookie_flags: 'max-age=7200;secure;samesite=none';path=/;";
}
function RemoveCookie(name) {
    var d = new Date();
    var strjson = JSON.stringify("");
    d.setTime(d.getTime() + (1 * 1 * 1 * 1 * -1)); //立刻過期
    var expires = "expires=" + d.toGMTString();
    document.cookie = name + "=" + strjson + ";" + expires + ";cookie_flags: 'max-age=7200;secure;samesite=none';path=/;";

}
function htmlempty(e) {
    $(getHtmlId(e)).empty();
}
function getHtmlId(e) {
    var _id = xssStr("#".concat(e));
    return _id;
}
function xssStr(txt) {
    return txt.replace(/</g, '&;lt;').replace(/>/g, '&;gt;');
}
//sort
function sortTh(p, obj) {
    var classes = ['sortIcon sortAsc', 'sortIcon sortDesc'];
    obj.find('.sortIcon').each(function () {
        this.className = classes[($.inArray(this.className, classes) + 1) % classes.length];
    });
    obj.siblings().find('.sortIcon').attr('class', 'sortIcon sortNo');
    var sorttype = "asc";
    if (obj.find('.sortIcon').hasClass("sortDesc")) { sorttype = "desc"; }
    var sortType = $("[name=sortType]");
    sortType.attr("data-title", obj.attr("data-title"));
    sortType.attr("data-sort", sorttype);
    //
    searchFunction(p);
}
//
/**
 * 以下為上傳物件所需
 * 
 */
//updateFile
function updateFile(e) {
    var filename = getObjAtr(e, "data-name");
    var fth = getObjAtr(e, "data-fth");
    var file = e.parent().find(".updatefile");
    var formdata = new FormData();
    formdata.append("file", file[0].files[0]);
    console.log(formdata);
    var url = "/Common/UpdateFile";
    $.ajax({
        url: url + "?" + $.param({ fth: fth, filename: filename }),
        type: "POST",
        data: formdata,
        contentType: false,//必須false才會自動加上正確的Content-Type
        processData: false,//必須false才會避開jQuery對 formdata 的預設處理.XMLHttpRequest會對 formdata 進行正確的處理.
        success: function (data) {
            alert("更新檔案成功!");
        },
        error: function (data) {
            alert("上傳失敗!" + data.Message);
        }
    });
};
//FileTable 刷新
function fileTable(table_id, data, itme, lan, needcopy) {
    var tr = "";
    needcopy = getCookie("needCopy") != null ? getCookie("needCopy") : needcopy;
    $.each(data, function (i, item) {
        tr += "<tr class='file_tr" + itme + " file_tr" + lan + "' ' data-gid=" + item.groupID + ">";
        tr += "<td><img height='50' src='" + item.filePath + "'  onerror='imgerror($(this))' /></td>";
        tr += "<td><div class='py-1'>" + item.fileOriginName + "</div>";
        //是否需要複製按鈕
        if (needcopy == "true") {
            //是否為圖檔
            tr += "<div class='py-1'>" + item.filePath
            tr += "<input type=\"button\" value=\"複製\" onclick=\"txtCopy('','" + item.filePath + "')\" class=\"btn btn-sm btn-purple\"></div>";
        }
        else {
            tr += "<div class='py-1'>" + item.filePath
        }
        tr += "</div> </td>";
        tr += "<td><input type='text' data-name='" + item.fileNewName + "' value='" + item.fileTitle + "' class='form-control filetitle' /></td>";
        tr += "<td><input type='button' value='刪除' onclick='deleteSessionFile($(this))'  class='btn btn-danger delfile' data-name='" + item.fileNewName + "' /></td>";
        tr += "</tr>";
    });

    $(getHtmlId(table_id)).append(tr);
}
//重新整理 reloadfilesTable
function reloadfilesTable() {
    var reloadHref = '/Common/GetSessionFiles';
    $.ajax({
        url: reloadHref,
        type: 'post',
        dataType: "json",
        dataFilter: function (data, type) {
            var jsonStr = xssStr(JSON.stringify(data));
            var _data = JSON.parse(jsonStr);
            return _data;
        },
        success: function (data) {
            if (data.statusCode == 200) {
                var retureData = data.content;
                var mapGroupID = new Set(retureData.map(x => x.groupID).filter(x => x.length < 10));
                var Lang = ["zh-tw", "en"];
                for (const entry of mapGroupID.keys()) {

                    for (var i = 0; i < Lang.length; i++) {
                        var _GroupID = xssStr(entry);
                        var _lang = xssStr(Lang[i]);
                        var filesClass = _GroupID.concat(_lang);
                        htmlempty("filesTable".concat(xssStr(filesClass)));
                        fileTable("filesTable" + filesClass,
                            data.content.filter(x => x.groupID == _GroupID && x.lan == _lang),
                            filesClass,
                            _lang,
                            "true");
                    }

                }
            }
        }
    });
}
//取得上傳物件的排序
function fileinfo(lan) {
    var filesort = 1;
    var fileinfo = [];
    $(".file_tr" + lan).each(function () {
        fileinfo.push(
            {
                filenewname: $(this).find(".filetitle").attr("data-name"),
                filetitle: $(this).find(".filetitle").val(),
                filesort: filesort,
                groupid: $(this).attr("data-gid"),
                lan: lan
            });
        filesort++;
    });
    return fileinfo;
}
//chart textLab
var chartLabel = {
    display: 'auto',
    color: '#005599',
    backgroundColor: 'rgb(210,210,210,0.8)',
    labels: {
        title: {
            font: { weight: 'bold' }
        }
    },
    anchor: 'end',
    align: 'end',
    offset: 4
}



