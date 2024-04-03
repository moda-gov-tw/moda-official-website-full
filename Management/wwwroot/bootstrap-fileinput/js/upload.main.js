var objFile;                    //   上傳參數
var _initialPreview = [];       //   已上傳物件路徑
var _initialPreviewConfig = []; //   已上傳物件
function _AutoUpload(e) {
    var obj = {
        getdataUrl: e.attr("data-getdata"),
        popUrl: e.attr("data-popup-url"),
        updateUrl: e.attr("data-upload"),
        deleteUrl: e.attr("data-delete"),
        sortUrl: e.attr("data-sort"),
        id: e.attr("data-id"),
        maxFileCount: e.attr("data-maxfilecount"),
        maxFileSize: e.attr("data-maxfilesize"),
        allowedFileExtensions: e.attr("data-allowedfileextensions")
    };
    _LoadFunction2(obj);
}
//
function _LoadFunction2(obj) {
    _initialPreview = [];
    _initialPreviewConfig = [];
    _objFile(obj);  //物件設定
    pop(obj); //pop視窗
}

//
function _LoadFunction(obj) {
    _initialPreview = [];
    _initialPreviewConfig = [];
    $.ajax({
        url: obj.getdataUrl,
        data: { id: obj.id},
        type: 'post',
        dataType: 'json',
        success: function (data) {
            if (data.statusCode == 200) {
                var list = data.content;
                _FileData(list); //<--畫面呈現之前的上傳的物件-->
                _objFile(obj);  //物件設定
                pop(obj); //pop視窗
            }
        }
    });
}
// loadFileData
function _FileData(list) {
    if (list.length > 0) {
        list.forEach(function (item, i) {
            var url = document.location.protocol + '//' + document.location.host + item.filePath;
            _initialPreview.push(url);
            _initialPreviewConfig.push({
                caption: item.fileName2,
                size: item.fileSize,
                width: "120px",
                key: item.id
            });
        });
    }
}
//物件設定
function _objFile(obj) {
    var allowedFileExtensions = obj.allowedFileExtensions == null ? null : obj.allowedFileExtensions.split(',');
    objFile = SetFileModel(
        {
            uploadUrl: obj.updateUrl,
            deleteUrl: obj.deleteUrl,
            sortUrl: obj.sortUrl,
            initialPreview: _initialPreview,
            initialPreviewConfig: _initialPreviewConfig,
            maxFileSize: obj.maxFileSize,
            maxFileCount: obj.maxFileCount,
            allowedFileExtensions: allowedFileExtensions,
         
        });
}
//pop



/**
目前版本 fileinput 5.01
官方資料 https://plugins.krajee.com/file-input
 參數資料 :
language 可以參考bootstrap-fileinput/js/locales 有哪些語系<預設繁體中文>
theme 版型 可以參考bootstrap-fileinput/js/theme 有哪些版型
uploadUrl 檔案上傳路徑
deleteUrl 刪除檔案路徑
overwriteInitial false 不允许覆盖初始的预览，所以添加文件时不会覆盖
initialPreviewAsData 初始預覽數據
minFileCount  : 最小檔案數量
maxFileCount  : 最大檔案數量
allowedFileExtensions : 檔案類型  ["jpg", "gif", "png", "txt","zip", "rar", "gz", "tgz", "md", "ini", "text"]
initialCaption : 上傳 文字客製化 <基本上不用特別設定>
maxFileSize : 單檔上傳檔案限制:單位 KB 如果不设置，最大值
initialPreview:[] 初始loading進來的資料 裡面塞路徑
initialPreviewConfig 初始loading進來的資料 [{caption: "檔案名稱" ,  size:檔案大小kb , width:120px, url 檔案路徑, key:  } ]
 */
function SetFileModel(obj) {
    return {
        language: obj.language == null ? "zh-TW" : obj.language,
        theme: obj.theme == null ? "explorer-fas" : obj.theme,
        uploadUrl: obj.uploadUrl,
        deleteUrl: obj.deleteUrl,
        sortUrl: obj.sortUrl == null ? "" : obj.sortUrl,
        overwriteInitial: obj.overwriteInitial == null ? false : obj.overwriteInitial,
        initialPreviewAsData: obj.initialPreviewAsData == null ? true : obj.initialPreviewAsData,
        minFileCount: obj.minFileCount == null ? 1 : obj.minFileCount,
        maxFileCount: obj.maxFileCount == null ? 10 : obj.maxFileCount,
        allowedFileExtensions: obj.allowedFileExtensions == null ? ['jpeg','jpg', 'png', 'gif', 'bmp', 'txt', 'doc', 'docx', 'ppt', 'pptx', 'xls', 'xlsx', 'pdf', 'rar', 'zip', 'mp3','odt','odp','ods','csv','svg','tif','tiff','mp4'] : obj.allowedFileExtensions,
        maxFileSize: obj.maxFileSize == null ? 25600 : obj.maxFileSize,
        initialPreview: obj.initialPreview == null ? [] : obj.initialPreview,
        initialPreviewConfig: obj.initialPreviewConfig == null ? [] : obj.initialPreviewConfig,
    }
}



