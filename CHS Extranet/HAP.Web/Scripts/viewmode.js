function setCookie(c_name, value, expiredays) {
    var exdate = new Date();
    exdate.setDate(exdate.getDate() + expiredays);
    document.cookie = c_name + "=" + escape(value) + ((expiredays == null) ? "" : ";expires=" + exdate.toGMTString() + ";path=/");
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
    if (e.innerHTML == "Rename") {
        renameitem.value = e.href.substring(e.href.lastIndexOf('#') + 1).replace(/%20/gi, " ").replace(/\^/gi, "&").replace(/%5E/gi, "&");
        if (e.href.match(/#F!/i)) renameitemname.innerHTML = e.href.substring(e.href.lastIndexOf('#') + 3).replace(/%20/gi, " ").replace(/\^/gi, "&").replace(/%5E/gi, "&");
        else renameitemname.innerHTML = e.href.substring(e.href.lastIndexOf('#') + 1).replace(/%20/gi, " ").replace(/\^/gi, "&").replace(/%5E/gi, "&");
        renameclick.click();
    } else if (e.innerHTML == "Delete") {
        deleteitem.value = e.href.substring(e.href.lastIndexOf('#') + 1).replace(/%20/gi, " ").replace(/\^/gi, "&").replace(/%5E/gi, "&");
        if (e.href.match(/#F!/i)) deleteitemname.innerHTML = e.href.substring(e.href.lastIndexOf('#') + 3).replace(/%20/gi, " ").replace(/\^/gi, "&").replace(/%5E/gi, "&");
        else deleteitemname.innerHTML = e.href.substring(e.href.lastIndexOf('#') + 1).replace(/%20/gi, " ").replace(/\^/gi, "&").replace(/%5E/gi, "&");
        deleteclick.click();
    }
    else if (e.innerHTML == "Move")
        window.open(e.href, 'HAPMove', 'toolbar=0,status=0,statusbar=0,menubar=0,menu=0,address=0,addressbar=0,width=500,height=500', true);
    else if (e.innerHTML == "Unzip") {
        unzipitem.value = e.href.substring(e.href.lastIndexOf('#') + 1).replace(/%20/gi, " ").replace(/\^/gi, "&").replace(/%5E/gi, "&");
        if (e.href.match(/#F!/i)) unzipitemname.innerHTML = e.href.substring(e.href.lastIndexOf('#') + 3).replace(/%20/gi, " ").replace(/\^/gi, "&").replace(/%5E/gi, "&");
        else unzipitemname.innerHTML = e.href.substring(e.href.lastIndexOf('#') + 1).replace(/%20/gi, " ").replace(/\^/gi, "&").replace(/%5E/gi, "&");
        unzipclick.click();
    }
    else if (e.innerHTML == "Zip") {
        zipitem.value = e.href.substring(e.href.lastIndexOf('#') + 1).replace(/%20/gi, " ").replace(/\^/gi, "&").replace(/%5E/gi, "&");
        if (e.href.match(/#F!/i)) zipitemname.innerHTML = e.href.substring(e.href.lastIndexOf('#') + 3).replace(/%20/gi, " ").replace(/\^/gi, "&").replace(/%5E/gi, "&");
        else zipitemname.innerHTML = e.href.substring(e.href.lastIndexOf('#') + 1).replace(/%20/gi, " ").replace(/\^/gi, "&").replace(/%5E/gi, "&");
        zipclick.click();
    }
    else if (e.innerHTML == "HTML Preview")
        window.open(e.href, 'HAPPreview', 'toolbar=0,status=0,statusbar=0,menubar=0,menu=0,address=0,addressbar=0,width=800,scrollbars=0,height=600', true);
    else window.open(e.href, 'HAPUpload', 'toolbar=0,status=0,statusbar=0,menubar=0,menu=0,address=0,addressbar=0,width=600,height=400', true);
    return false;
}

function changeversion(e) {
    setCookie('mycompv', e, 30);
    if (e == "sl") window.location.href = appdir + "/mycomputersl.aspx";
    else document.getElementById('versionquest').style.display = 'none';
    return false;
}

var hasSilverlight = Boolean(window.Silverlight);
var hasSilverlight4 = hasSilverlight && Silverlight.isInstalled('4.0');
if (!hasSilverlight4) document.getElementById("versionquest").style.display = document.getElementById('mypcsl').style.display =  "none";
else {
    var version = getCookie('mycompv');
    if (version != null && version != "") {
        changeversion(version);
    } else document.getElementById('versionquest').style.display = 'block';
}


getViewMode();