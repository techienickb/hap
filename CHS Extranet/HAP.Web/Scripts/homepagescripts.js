function showtab(e, sender) {
    for (var x = 0; x < document.getElementById('tabs').getElementsByTagName('div').length; x++) {
        if (document.getElementById('tabs').getElementsByTagName('div')[x].className == "tab")
            document.getElementById('tabs').getElementsByTagName('div')[x].removeAttribute("style");
    }
    for (var x = 0; x < document.getElementById('tabheaders').getElementsByTagName('a').length; x++) {
        document.getElementById('tabheaders').getElementsByTagName('a')[x].removeAttribute("class");
    }
    document.getElementById(e + "_tab").setAttribute("style", "display: block;");
    sender.setAttribute('class', 'active');
}
document.getElementById('tabheaders').getElementsByTagName('a')[0].onclick();