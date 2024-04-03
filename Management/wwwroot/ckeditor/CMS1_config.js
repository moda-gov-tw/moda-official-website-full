CKEDITOR.editorConfig = function (config) {

    // 設定工具列
    config.toolbarGroups = [
        { name: 'document', groups: ['mode', 'document', 'doctools'] },
        { name: 'clipboard', groups: ['undo', 'clipboard'] },
        { name: 'editing', groups: ['find', 'selection', 'spellchecker', 'editing'] },
        { name: 'forms', groups: ['forms'] },
        { name: 'styles', groups: ['styles'] },
        { name: 'colors', groups: ['colors'] },
        { name: 'basicstyles', groups: ['basicstyles', 'cleanup'] },
        '/',
        { name: 'paragraph', groups: ['align', 'list', 'indent', 'blocks', 'bidi', 'paragraph'] },
        { name: 'links', groups: ['links'] },
        { name: 'insert', groups: ['insert'] },
        { name: 'others', groups: ['others'] },
        { name: 'about', groups: ['about'] },
        { name: 'tools', groups: ['tools'] }
    ];

    config.removeButtons = 'Save,NewPage,Preview,Print,Templates,Replace,Scayt,Form,Checkbox,Radio,TextField,Textarea,Select,Button,HiddenField,ImageButton,RemoveFormat,CopyFormatting,About,Smiley,SpecialChar,Flash,Cut,Copy,Paste,Find';

    // 工具列可收合
    config.toolbarCanCollapse = true;
    // 工具列預設展開
    config.toolbarStartupExpanded = true;
    // 加入中文字體
    config.font_names = '新細明體;標楷體;微軟正黑體' + config.font_names;
    //a標籤可以強制 div & a 
    CKEDITOR.dtd.a.div = 1;
    CKEDITOR.dtd.a.p = 1;
    // 允許內容修改
    config.allowedContent = true;
    config.fillEmptyBlocks = false;
    config.autoParagraph = false;
    config.extraAllowedContent = '*(*);*{*}';


    // 設定段落樣式
    config.enterMode = CKEDITOR.ENTER_P;
    config.shiftEnterMode = CKEDITOR.ENTER_BR;
    // 貼上時清除格式
    config.forcePasteAsPlainText = true;
    // Word貼上不要提示「需清理內容為純文字」
    config.pasteFromWordPromptCleanup = false;
    //忽略字體格式
    config.pasteFromWordRemoveFontStyles = true;
    config.pasteFromWordRemoveStyles = true;
    // 無障礙設定
    config.extraPlugins = 'language';
    config.language_list = ['en:英語', 'es:西班牙語', 'de:德語', 'fr:法語'];
    config.extraPlugins = 'image';
    
    if (location.href.includes('state'))
    { config.contentsCss = ['/Admin/css/CKEditorState.css']; }
    config.extraPlugins = 'markdown';
}