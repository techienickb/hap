/*HAP.Web.JS.js - Copyright © 2012 nb development - Version 1 */
if (hap == null)
    var hap = {
        root: "/hap/",
        common: {
            jsonError: function (xhr, ajaxOptions, thrownError) {
                try {
                if (xhr.responseText.match(/\<!doctype html\>/gi)) window.location.reload();
                else {
                    console.log(xhr.responseXML.documentElement.children[2]);
                    alert(xhr.responseXML.documentElement.children[1].children[0].textContent);
                }
                } catch (e) { alert(thrownError); }
            },
            resolveUrl: function (virtual) {
                return virtual.replace(/~\//g, hap.root);
            },
            getLocal: function (e) {
                for (var i = 0; i < hap.localization.length; i++)
                    if (hap.localization[i].name == e) return unescape(hap.localization[i].value.replace(/\\\\/g, "\\"));
            }
        },
        help: {
            Init: function () {
                if (document.getElementById("helpbox") != null) return;
                $('<div id="helpbox" title="Help"><div class="content">Loading</div></div>').appendTo(document.body);
                $("#helpbox").dialog({ autoOpen: false });
            },
            Load: function (path) {
                $("#helpbox").dialog({ autoOpen: true, modal: true, height: 600, width: 990, buttons: { "Close": function () { $(this).dialog("close"); } } });
                $("#helpbox .content").html("Loading...");
                $.ajax({
                    type: 'GET',
                    url: hap.common.resolveUrl('~/api/Help/' + path),
                    dataType: 'json',
                    contentType: 'application/json',
                    success: function (data) {
                        $("#helpbox .content").html(data);
                    },
                    error: hap.common.jsonError
                });
            }
        },
        localization: []
    };
$(function () { hap.help.Init(); });