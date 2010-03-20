function setCookie(c_name, value, expiredays) {
    var exdate = new Date();
    exdate.setDate(exdate.getDate() + expiredays);
    document.cookie = c_name + "=" + escape(value) + ((expiredays == null) ? "" : ";expires=" + exdate.toGMTString());
}
function getCookie(c_name) {
    if (document.cookie.length > 0) {
        var c_start = document.cookie.indexOf(c_name + "=");
        if (c_start != -1) {
            c_start = c_start + c_name.length + 1;
            var c_end = document.cookie.indexOf(";", c_start);
            if (c_end == -1) c_end = document.cookie.length;
            return unescape(document.cookie.substring(c_start, c_end));
        }
    }
    return "";
}

function getViewMode() {
    var viewmode = getCookie('viewmode');
    if (viewmode != null && viewmode != "") {
        changeview(viewmode);
    }
}

function view() {
    document.getElementById('viewbox').className = (document.getElementById('viewbox').className == "show" ? "" : "show");
    return false;
}
function changeview(e) {
    document.getElementById('browser').className = e;
    setCookie('viewmode', e, 30);
    return false;
}
function popup(e) {
    if (e.innerHTML == "Rename")
        window.open(e.href, 'CHSRename', 'toolbar=0,status=0,statusbar=0,menubar=0,menu=0,address=0,addressbar=0,width=400,height=200', true);
    else if (e.innerHTML == "Delete")
        window.open(e.href, 'CHSDelete', 'toolbar=0,status=0,statusbar=0,menubar=0,menu=0,address=0,addressbar=0,width=400,height=200', true);
    else if (e.innerHTML == "New Folder")
        window.open(e.href, 'CHSNew', 'toolbar=0,status=0,statusbar=0,menubar=0,menu=0,address=0,addressbar=0,width=400,height=200', true);
    else if (e.innerHTML == "Move")
        window.open(e.href, 'CHSMove', 'toolbar=0,status=0,statusbar=0,menubar=0,menu=0,address=0,addressbar=0,width=500,height=500', true);
    else if (e.innerHTML == "Unzip")
        window.open(e.href, 'CHSUnzip', 'toolbar=0,status=0,statusbar=0,menubar=0,menu=0,address=0,addressbar=0,width=400,height=300', true);
    else if (e.innerHTML == "HTML Preview")
        window.open(e.href, 'CHSPreview', 'toolbar=0,status=0,statusbar=0,menubar=0,menu=0,address=0,addressbar=0,width=800,scrollbars=0,height=600', true);
    else window.open(e.href, 'CHSUpload', 'toolbar=0,status=0,statusbar=0,menubar=0,menu=0,address=0,addressbar=0,width=600,height=400', true);
    return false;
}

getViewMode();