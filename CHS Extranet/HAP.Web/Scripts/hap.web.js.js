/*HAP.Web.JS.js - Copyright © 2012 nb development - Version 1 */
if (hap == null) {
    var hap = {
        root: "/hap/",
        user: "",
        admin: false,
        common: {
            jsonError: function (xhr, ajaxOptions, thrownError) {
                try {
                    if (xhr.responseText.match(/\<!doctype html/gi)) window.location.reload();
                    else {
                        if (document.getElementById("errorlist") == null) $("#hapContent").append('<div id="errorlist"></div>');
                        $("<div class=\"ui-state-error ui-corner-all\" style=\"padding: 3px 10px 3px 10px\"><span class=\"ui-icon ui-icon-alert\" style=\"float: left; margin-right: 5px; margin-top: 2px;\"></span><a href=\"#\" onclick=\"this.nextSibling.className = (this.nextSibling.className == 'cont') ? '' : 'cont'; return false;\">" + jQuery.parseJSON(xhr.responseText).Message + "</a><div class=\"cont\">This error has been logged on the server's event log</div></div>").appendTo("#errorlist");
                        setTimeout("hap.common.clearError();", 10000);
                        try { console.log(xhr.responseText); } catch (ex) { };
                    }
                } catch (e) { if (thrownError != "") alert(thrownError); }
            },
            clearError: function () {
                $($("#errorlist").children()[0]).animate({ height: 0 }, 500, function () { $(this).remove(); });
            },
            resolveUrl: function (virtual) {
                return virtual.replace(/~\//g, hap.root);
            },
            keepAlive: function () {
                setInterval(function () {
                    $.ajax({
                        url: hap.common.resolveUrl("~/api/test/?1=" + JSON.stringify(new Date())), type: 'GET', success: function (data) {
                        }, error: hap.common.jsonError
                    });
                }, 60000);
            },
            getLocal: function (e) {
                for (var i = 0; i < hap.localization.length; i++)
                    if (hap.localization[i].name == e) return unescape(hap.localization[i].value.replace(/\\\\/g, "\\"));
            }
        },
        header: {
            StopClose: false,
            StopUClose: false,
            WaitInit: false,
            Init: function () {

                if (window.location.pathname.toLowerCase() != hap.root.toLowerCase() && window.location.pathname.toLowerCase() != hap.common.resolveUrl('~/login.aspx').toLowerCase()) {
                    $("#hapTitleMore").click(function () { return false; });
                    $.ajax({
                        url: hap.common.resolveUrl('~/api/livetiles/') + '?' + window.JSON.stringify(new Date()), type: 'GET', dataType: "json", contentType: 'application/JSON', success: function (data) {
                            $("#hapContent").click(function () { if ($("#hapHeaderMore").css('display') == 'block' && !hap.header.StopClose) $("#hapHeaderMore").animate({ height: 'toggle' }); hap.header.StopClose = false; }).append('<div id="hapHeaderMore" class="tile-color"><div class="tiles"></div></div>');
                            $("#hapHeaderMore").click(function () { hap.header.StopClose = true; }).mouseleave(function () { if (!hap.header.WaitInit) $("#hapHeaderMore").animate({ height: 'toggle' }); });
                            $("#hapTitleMore").click(function () { hap.header.WaitInit = true; $("#hapHeaderMore").animate({ height: 'toggle' }, 500, 'linear', function () { hap.header.WaitInit = false; }); return false; }).trigger("click");
                            for (var i = 0; i < data.length; i++) {
                                if (data[i].Group == 'Me') continue;
                                var s = "<div>" + data[i].Group + "<div>";
                                if (i == 0) s += '<a href="' + hap.common.resolveUrl("~/") + '" title="' + hap.common.getLocal("homeaccessplus") + " " + hap.common.getLocal("home") + '" style="background-image: url(' + hap.common.resolveUrl("~/images/icons/metro/hap-logo-64.png") + ');">' + hap.common.getLocal("home") + '</a>';
                                for (var i2 = 0; i2 < data[i].Tiles.length; i2++) {
                                    if (data[i].Tiles[i2].Url.substr(0, 1) != "#")
                                        s += '<a href="' + hap.common.resolveUrl(data[i].Tiles[i2].Url) + '" style="' + (data[i].Tiles[i2].Icon == "" ? "" : 'background-image: url(' + hap.common.resolveUrl(data[i].Tiles[i2].Icon) + '); ') + (data[i].Tiles[i2].Color.substr(0, 1) == " " ? ('background-color: ' + $.parseJSON(data[i].Tiles[i2].Color).Base + ';" onmouseout="this.style.backgroundColor = \'' + $.parseJSON(data[i].Tiles[i2].Color).Base + '\';" onmouseover="this.style.backgroundColor = \'' + $.parseJSON(data[i].Tiles[i2].Color).Light + '\';" onmousedown="this.style.backgroundColor = \'' + $.parseJSON(data[i].Tiles[i2].Color).Dark + '\';"') : '"') + (data[i].Tiles[i2].Target == "" ? "" : ' target="' + data[i].Tiles[i2].Target + '"') + ' title="' + data[i].Tiles[i2].Description + '">' + data[i].Tiles[i2].Name + '</a>';
                                }
                                s += "</div></div>";
                                $("#hapHeaderMore > .tiles").append(s);
                            }
                        }, error: hap.common.jsonError
                    });
                    $("#hapTitleMore").attr("title", hap.common.getLocal("more"));
                    $("#hapHeader").css('right', $("#hapUserTitle").width() + 30 + 'px');
                } else {
                    $("#hapHeader").hide();
                }
                $("#hapUserTitle").click(function () { $("#hapUserMenu").animate({ height: 'toggle' }); return false; }).trigger("click");
                $("#hapUserMenu").click(function () { hap.header.StopUClose = true; });
                $("#hapContent").click(function () { if (!hap.header.StopUClose && $("#hapUserMenu").css('display') == 'block') $("#hapUserMenu").animate({ height: 'toggle' }); hap.header.StopUClose = false; });
            }
        },
        loadtypes : {
            none: 0,
            help: 1,
            full: 2
        },
        load: 2,
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
        localization: [],
        livetiles: {
            Init: function (data) {
                if ($("#" + data[0].Data.Group).is(".me")) this.ShowMe(data);
                else for (var i = 0; i < data.length; i++) this.Tiles.push(new this.LiveTile(data[i].Type, data[i].Data));
            },
            Tiles: [],
            ShowMe: function (data) {
                for (var i = 0; i < data.length; i++)
                    if (data[i].Data.Name == "Me") {
                        $("#" + data[i].Data.Group).append('<div id="me-me"></div>').parent().addClass("me");
                        $.ajax({
                            url: "api/livetiles/me", type: 'GET', context: this.id, dataType: "json", contentType: 'application/JSON', success: function (data) {
                                if (data.Photo != "" && data.Photo != null) $("#me-me").append('<img src="' + hap.common.resolveUrl(data.Photo) + '" style="float: right;" />');
                                $("#me-me").append('<div id="me-name">' + data.Name + '</div><div id="me-email">' + data.Email + '</div>');
                            }, error: hap.common.jsonError
                        });
                    } else if (data[i].Data.Name == "Password") {
                        $("#" + data[i].Data.Group).append('<div id="me-password"><h1>Change My Password</h1><div><label for="me-password-current">Current Password: </label><input type="password" id="me-password-current" value="" /></div><div><label for="me-password-new">New Password: </label><input type="password" id="me-password-new" value="" /></div><div><label for="me-password-confirm">Confirm Password: </label><input type="password" id="me-password-confirm" value="" /></div><input type="button" id="me-setpassword" value="Change Password" /></div>');
                        $("#me-setpassword").button().click(function () {
                            if ($("#me-password-current").val().length == 0 || $("#me-password-new").val().length == 0 || $("#me-password-confirm").val().length == 0 || $("#me-password-confirm").val() != $("#me-password-new").val()) return false;
                            $.ajax({
                                url: "api/livetiles/me/password", type: 'POST', dataType: "json", contentType: 'application/JSON', data: '{ "oldpassword": "' + $("#me-password-current").val() + '", "newpassword": "' + $("#me-password-new").val() + '" }', success: function (data) {
                                    alert("Password Updated");
                                    $("#me-password-current, #me-password-new, #me-password-confirm").val("");
                                }, error: hap.common.jsonError
                            });
                            return false;
                        });
                    }
            },
            LiveTile: function (type, initdata, size) {
                this.id = (initdata.Group + initdata.Name).replace(/[\s'\/\\\&\.\,\*]*/gi, "");
                if (type == "exchange.appointments" || type.match(/exchange.calendar\:/gi) || type == "bookings" || type == "helpdesk") size = "large";
                this.html = '<a id="' + this.id + '" href="' + hap.common.resolveUrl(initdata.Url) + '" target="' + initdata.Target + '" title="' + initdata.Description + '"' + (size == 'large' ? ' class="large"' : '') + (initdata.Color == '' ? '' : ' style="background-color: ' + initdata.Color.Base + ';" onmouseover="this.style.backgroundColor = \'' + initdata.Color.Light + '\';" onmouseout="this.style.backgroundColor = \'' + initdata.Color.Base + '\';" onmousedown="this.style.backgroundColor = \'' + initdata.Color.Dark + '\';"') + '><span><i style="background-image: url(' + hap.common.resolveUrl(initdata.Icon) + ');"></i><label></label></span>' + initdata.Name + '</a>';
                $("#" + initdata.Group).append(this.html);
                if (type == "exchange.unread") {
                    setTimeout("hap.livetiles.UpdateExchangeMail('" + this.id + "');", 100);
                } else if (type == "exchange.appointments") {
                    $("#" + this.id).addClass("appointment");
                    setTimeout("hap.livetiles.UpdateExchangeAppointments('" + this.id + "');", 100);
                } else if (type.match(/exchange.calendar\:/gi)) {
                    $("#" + this.id).addClass("appointment");
                    setTimeout("hap.livetiles.UpdateExchangeCalendar('" + this.id + "', '" + type.split(/exchange.calendar\:/gi)[1] + "');", 100);
                } else if (type == "me") {
                    $("#" + this.id).addClass("me").click(function () {
                        scrollpos = $("#HomeButtons > .me").index();
                        $("#HomeButtonsOutter").animate({ scrollLeft: (scrollpos * ($("#HomeButtonsOutter").width() - 20) + (scrollpos * 20)) });
                        $("#HomeButtonsHeader h1").removeClass("active");
                        $("#HomeButtonsHeader h1")[scrollpos].className = "active";
                        return false;
                    });
                    $.ajax({
                        url: "api/livetiles/me" + '?' + window.JSON.stringify(new Date()), type: 'GET', context: this.id, dataType: "json", contentType: 'application/JSON', success: function (data) {
                            $("#" + this + " span label").html("<b>" + data.Name + "</b><br />" + data.Email);
                            if (data.Photo != "" && data.Photo != null) $("#" + this + " span i").css("background-image", "url(" + hap.common.resolveUrl(data.Photo) + ")");
                            setInterval("$('#" + this + " span i').animate({ height: 'toggle' });", 8000);
                        }, error: hap.common.jsonError
                    });
                } else if (type == "myfiles") {
                    $("#" + this.id).addClass("me");
                    $.ajax({
                        url: "api/myfiles/drives", type: 'GET', context: this.id, dataType: "json", contentType: 'application/JSON', success: function (data) {
                            var s = "";
                            for (var i = 0; i < (data.length > 3 ? 3 : data.length) ; i++)
                                s += "<b>" + data[i].Name + "</b>" + (data[i].Space == -1 ? "<br /><br />" : '<br /><span class="progress"><label>' + data[i].Space + '%</label><u style="width: ' + data[i].Space + '%"></u></span>');
                            $("#" + this + " span label").html(s);
                            setInterval("$('#" + this + " > span > i').animate({ height: 'toggle' });", 6000);
                        }, error: hap.common.jsonError
                    });
                } else if (type == "bookings") {
                    $("#" + this.id).addClass("appointment");
                    setTimeout("hap.livetiles.UpdateBookings('" + this.id + "');", 100);
                } else if (type == "helpdesk") {
                    $("#" + this.id).addClass("me");
                    setInterval("$('#" + this.id + " > span > i').animate({ height: 'toggle' });", 10000);
                    setTimeout("hap.livetiles.UpdateTickets('" + this.id + "');", 100);
                } else if (type.match(/uptime\:/gi)) {
                    $("#" + this.id).addClass("me");
                    setTimeout("hap.livetiles.UpdateUptime('" + this.id + "', '" + type.substr(7) + "');", 100);
                }
            },
            UpdateExchangeCalendar: function (tileid, mailbox) {
                $("#" + tileid + " span label").html($.datepicker.formatDate('D <b>d</b>', new Date()));
                $.ajax({
                    url: "api/livetiles/exchange/calendar" + '?' + window.JSON.stringify(new Date()), type: 'POST', dataType: 'json', data: '{ "Mailbox" : "' + mailbox + '" }', context: { tile: tileid, mb: mailbox }, contentType: 'application/JSON', success: function (data) {
                        var s = "";
                        for (var i = 0; i < data.length; i++)
                            s += data[i] + "<br />";
                        $("#" + this.tile + " span i").html(s);
                        if (data.length > 0) $("#" + this.tile + " span i").attr("style", "background-image: url();");
                        setTimeout("hap.livetiles.UpdateExchangeCalendar('" + this.tile + "', '" + this.mb + "');", 100000);
                    }, error: hap.common.jsonError
                });
            },
            UpdateExchangeAppointments: function (tileid) {
                $("#" + tileid + " span label").html($.datepicker.formatDate('D <b>d</b>', new Date()));
                $.ajax({
                    url: "api/livetiles/exchange/appointments" + '?' + window.JSON.stringify(new Date()), type: 'GET', context: tileid, dataType: "json", contentType: 'application/JSON', success: function (data) {
                        var s = "";
                        for (var i = 0; i < data.length; i++)
                            s += data[i] + "<br />";
                        $("#" + this + " span i").html(s);
                        if (data.length > 0) $("#" + this + " span i").attr("style", "background-image: url();");
                        setTimeout("hap.livetiles.UpdateExchangeAppointments('" + this + "');", 100000);
                    }, error: hap.common.jsonError
                });
            },
            UpdateBookings: function (tileid) {
                $.ajax({
                    type: 'POST',
                    url: hap.common.resolveUrl("~/api/BookingSystem/Search") + '?' + window.JSON.stringify(new Date()),
                    dataType: 'json',
                    context: tileid,
                    data: '{ "Query": "' + hap.user + '" }',
                    contentType: 'application/json',
                    success: function (data) {
                        var d = "";
                        for (var i = 0; i < data.length; i++) {
                            var item = data[i];
                            d += (item.Date.match(/[0|1][0-9]\w\w\w/g) ? item.Date.substr(2, item.Date.length - 2) : item.Date) + ": " + item.Name + " in " + item.Room + "<br />";
                        }
                        if (data.length > 0) $("#" + this + " span i").attr("style", "background-image: url();");
                        $('#' + this + " span i").html(d);
                        setTimeout("hap.livetiles.UpdateBookings('" + this + "');", 110000);
                    },
                    error: hap.common.jsonError
                });
            },
            UpdateTickets: function (tileid) {
                $.ajax({
                    type: 'GET',
                    url: hap.common.resolveUrl("~/api/HelpDesk/Tickets/Open") + (hap.admin ? '' : ('/' + hap.user)) + '?' + window.JSON.stringify(new Date()),
                    dataType: 'json',
                    context: tileid,
                    contentType: 'application/json',
                    success: function (data) {
                        var x = "";
                        for (var i = 0; i < data.length; i++) x += '<b>' + (i + 1) + '</b>:' + data[i].Subject + '<br />';
                        if (data.length == 0) x = "No Open Tickets";
                        $("#" + this + " span label").html(x);
                        setTimeout("hap.livetiles.UpdateTickets('" + this + "');", 500000);
                    },
                    error: hap.common.jsonError
                });
            },
            UpdateExchangeMail: function (tileid) {
                $.ajax({
                    url: "api/livetiles/exchange/unread" + '?' + window.JSON.stringify(new Date()), type: 'GET', context: tileid, dataType: "json", contentType: 'application/JSON', success: function (data) {
                        if (data > 0) {
                            $("#" + this + " span i").animate({ width: 60 }, 500, function () { $(this).parent().children("label").html(data); });
                        } else {
                            $("#" + this + " span label").html("");
                            $("#" + this + " span i").animate({ width: 108 });
                        }
                        setTimeout("hap.livetiles.UpdateExchangeMail('" + this + "');", 30000);
                    }, error: hap.common.jsonError
                });
            },
            UpdateUptime: function (tileid, server) {
                var con = { tile: tileid, server: server };
                $.ajax({
                    url: "api/livetiles/uptime/" + server, type: 'GET', context: con, dataType: "json", contentType: 'application/JSON', success: function (data) {
                        $("#" + this.tile + " span i").html(data);
                        setTimeout("hap.livetiles.UpdateUptime('" + this.tile + "', '" + this.server + "');", 5000);
                    }, error: hap.common.jsonError
                });
            }
        }
    };
    $(function () {
        hap.header.Init();
        if (hap.load > hap.loadtypes.none) {
            hap.help.Init();
            hap.common.keepAlive();
        }
    });
}