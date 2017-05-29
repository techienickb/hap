/*HAP.Web.JS.js - Copyright © 2014 nb development - Version 5 */
if (hap == null) {
    var hap = {
        root: "/hap/",
        user: "",
        admin: false,
        bsadmin: false,
        hdadmin: false,
        errorTimeout: null,
        common: {
            jsonError: function (xhr, ajaxOptions, thrownError) {
                try {
                    if (xhr.responseText.match(/\<!doctype html/gi) && thrownError != "Not Found" && thrownError != "Internal Server Error") window.location.reload();
                    else {
                        if (jQuery.parseJSON(xhr.responseText).Message == "Length of the data to decrypt is invalid." || jQuery.parseJSON(xhr.responseText).Message.match(/invalid length for a base\-64/gi)) hap.help.Load("impmsg");
                        else {
                            if (document.getElementById("errorlist") == null) $("#hapContent").append('<div id="errorlist"></div>');
                            $("<div class=\"ui-state-error ui-corner-all\" style=\"padding: 3px 10px 3px 10px\"><span class=\"ui-icon ui-icon-alert\" style=\"float: left; margin-right: 5px; margin-top: 2px;\"></span><a href=\"#\" onclick=\"this.nextSibling.className = (this.nextSibling.className == 'cont') ? '' : 'cont'; return false;\">" + jQuery.parseJSON(xhr.responseText).Message + "</a><div class=\"cont\">This error has been logged on the server's event log</div></div>").appendTo("#errorlist");
                            if (hap.errorTimeout == null) hap.errorTimeout = setTimeout("hap.common.clearError();", 10000);

                            try { console.log(xhr.responseText); } catch (ex) { };
                        }
                    }
                } catch (e) { if (thrownError != "") alert(thrownError); }
            },
            formatJSONUrl: function (url) {
                var d = new Date().valueOf();
                return hap.common.resolveUrl(url) + '?' + d;
            },
            clearError: function () {
                $($("#errorlist").children()[0]).animate({ height: 0 }, 300, function () { $($("#errorlist").children()[0]).remove(); });
                if ($("#errorlist").children().length > 0) hap.errorTimeout = setTimeout("hap.common.clearError();", 10000);
                else hap.errorTimeout = null;
            },
            resolveUrl: function (virtual) {
                return virtual.replace(/~\//g, hap.root);
            },
            keepAlive: function () {
                setInterval(function () {
                    $.ajax({
                        url: hap.common.formatJSONUrl("~/api/test/"), type: 'GET', success: function (data) {
                        }, error: hap.common.jsonError
                    });
                }, 60000);
            },
            getLocal: function (e) {
                for (var i = 0; i < hap.localization.length; i++)
                    if (hap.localization[i].name == e) return unescape(hap.localization[i].value.replace(/\\\\/g, "\\"));
            },
            makeSwitchs: function () {
                var e = $("input[type='checkbox']");
                for (var i = 0; i < e.length; i++) {
                    var o = $(e[i]);
                    if (o.hasClass("noswitch")) continue;
                    if (o.hasClass("hapswitch")) continue;
                    o.before('<span class="hapswitch" data-for="' + o.attr("id") + '"><span></span><i></i></span>');
                    o.addClass("hapswitch").change(function () {
                        $(this).prev().removeClass("on").addClass($(this).is(":checked") ? 'on' : '');
                    }).prev().click(function () {
                        $(this).next().prop("checked", $(this).next().is(":checked") ? false : true);
                        $(this).removeClass("on").addClass($(this).next().is(":checked") ? 'on' : '');
                    });
                    if (o.is(":checked")) o.prev().addClass('on');
                }

            }
        },
        header: {
            StopClose: false,
            StopUClose: false,
            WaitInit: false,
            Init: function () {

                if (window.location.pathname.toLowerCase() != hap.root.toLowerCase() && window.location.pathname.toLowerCase() != hap.common.resolveUrl('~/login.aspx').toLowerCase() && window.location.pathname.toLowerCase() != hap.common.resolveUrl('~/kerberos.aspx').toLowerCase()) {
                    $("#hapTitleMore").click(function () { return false; });
                    $.ajax({
                        url: hap.common.formatJSONUrl('~/api/livetiles/'), type: 'GET', dataType: "json", contentType: 'application/JSON', success: function (data) {
                            $("#hapContent").click(function () { if ($("#hapHeaderMore").css('display') == 'block' && !hap.header.StopClose) $("#hapHeaderMore").animate({ height: 'toggle' }); hap.header.StopClose = false; }).append('<div id="hapHeaderMore" class="tile-color"><div class="tiles"></div></div>');
                            $("#hapHeaderMore").click(function () { hap.header.StopClose = true; }).mouseleave(function () { if (!hap.header.WaitInit) $("#hapHeaderMore").animate({ height: 'toggle' }); });
                            $("#hapTitleMore").click(function () { hap.header.WaitInit = true; $("#hapHeaderMore").animate({ height: 'toggle' }, 500, 'linear', function () { hap.header.WaitInit = false; }); return false; }).trigger("click");
                            for (var i = 0; i < data.length; i++) {
                                if (data[i].Group == 'Me') continue;
                                var s = "<div>" + unescape(data[i].GroupName) + "<div>";
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
                $('<div id="helpbox" title="Help"><div class="content">Loading</div></div>').appendTo(document.body).hide();
                /*$("#helpbox").dialog({ autoOpen: false });*/
            },
            Load: function (path) {
                $("#helpbox").hapPopup();
                $("#helpbox .content").html("Loading...");
                $.ajax({
                    type: 'GET',
                    url: hap.common.resolveUrl('~/api/Help/' + path),
                    dataType: 'json',
                    contentType: 'application/json',
                    success: function (data) {
                        $("#helpbox .content").html(data);
                        $("#helpbox .content button").button();
                    },
                    error: hap.common.jsonError
                });
            }
        },
        localization: [],
        livetiles: {
            Init: function (data) {
                if (data.length > 0) {
                    if ($("#" + data[0].Data.Group).is(".me")) this.ShowMe(data);
                    else for (var i = 0; i < data.length; i++) {
                        var tile = new this.LiveTile(data[i].Type, data[i].Data);
                        for (var x = 0; x < this.TileHandlers.length; x++)
                            if (data[i].Type.match(this.TileHandlers[x].type)) { this.TileHandlers[x].func(data[i].Type, data[i].Data, tile); break; }
                        tile.Render();
                        this.Tiles.push(tile);
                    }
                }
            },
            TileHandlers: [],
            RegisterTileHandler: function (type, func) {
                this.TileHandlers.push({ "type": type, "func": func });
            },
            Tiles: [],
            ShowMe: function (data) {
                for (var i = 0; i < data.length; i++)
                    if (data[i].Data.Name == "Me") {
                        $("#" + data[i].Data.Group).append('<div id="me-me"></div>').parent().addClass("me");
                        $.ajax({
                            url: hap.common.formatJSONUrl("~/api/livetiles/me"), type: 'GET', context: this.id, dataType: "json", contentType: 'application/JSON', success: function (data) {
                                if (data.Photo != "" && data.Photo != null) $("#me-me").append('<img src="' + hap.common.resolveUrl(data.Photo) + '" style="float: right;" />');
                                $("#me-me").append('<div id="me-name">' + data.Name + '</div><div id="me-email">' + data.Email + '</div>');
                            }, error: hap.common.jsonError
                        });
                    } else if (data[i].Data.Name == "Password") {
                        $("#" + data[i].Data.Group).append('<div id="me-password"><h1>Change My Password</h1><div><label for="me-password-current">Current Password: </label><input type="password" id="me-password-current" value="" /></div><div><label for="me-password-new">New Password: </label><input type="password" id="me-password-new" value="" /></div><div><label for="me-password-confirm">Confirm Password: </label><input type="password" id="me-password-confirm" value="" /></div><input type="button" id="me-setpassword" value="Change Password" /></div>');
                        $("#me-setpassword").button().click(function () {
                            if ($("#me-password-current").val().length == 0 || $("#me-password-new").val().length == 0 || $("#me-password-confirm").val().length == 0 || $("#me-password-confirm").val() != $("#me-password-new").val()) return false;
                            $.ajax({
                                url: hap.common.formatJSONUrl("~/api/livetiles/me/password"), type: 'POST', dataType: "json", contentType: 'application/JSON', data: '{ "oldpassword": "' + $("#me-password-current").val() + '", "newpassword": "' + $("#me-password-new").val() + '" }', success: function (data) {
                                    alert("Password Updated");
                                    $("#me-password-current, #me-password-new, #me-password-confirm").val("");
                                }, error: hap.common.jsonError
                            });
                            return false;
                        });
                    }
            },
            RegisterDefaultTiles: function () {
                hap.livetiles.RegisterTileHandler("exchange.unread", function (type, initdata, t) {
                    t.html = '<a id="' + t.id + '" href="' + hap.common.resolveUrl(initdata.Url) + '" target="' + initdata.Target + '" title="' + initdata.Description + '"' + ' class="tile width' + initdata.Width + ' height' + initdata.Height + '"' + (initdata.Color == '' ? '' : ' style="background-color: ' + initdata.Color.Base + ';" onmouseover="this.style.backgroundColor = \'' + initdata.Color.Light + '\';" onmouseout="this.style.backgroundColor = \'' + initdata.Color.Base + '\';" onmousedown="this.style.backgroundColor = \'' + initdata.Color.Dark + '\';"') + '><span><i style="background-image: url(' + hap.common.resolveUrl(initdata.Icon) + ');"></i><label></label></span>' + initdata.Name + '</a>';
                    setTimeout("hap.livetiles.UpdateExchangeMail('" + t.id + "');", 100);
                });
                hap.livetiles.RegisterTileHandler("exchange.appointments", function (type, initdata, t) {
                    t.html = '<a id="' + t.id + '" href="' + hap.common.resolveUrl(initdata.Url) + '" target="' + initdata.Target + '" title="' + initdata.Description + '"' + ' class="tile width' + initdata.Width + ' height' + initdata.Height + '"' + (initdata.Color == '' ? '' : ' style="background-color: ' + initdata.Color.Base + ';" onmouseover="this.style.backgroundColor = \'' + initdata.Color.Light + '\';" onmouseout="this.style.backgroundColor = \'' + initdata.Color.Base + '\';" onmousedown="this.style.backgroundColor = \'' + initdata.Color.Dark + '\';"') + '><span><i style="background-image: url(' + hap.common.resolveUrl(initdata.Icon) + ');"></i><label></label></span>' + initdata.Name + '</a>';
                    t.Render();
                    $("#" + t.id).addClass("appointment");
                    setTimeout("hap.livetiles.UpdateExchangeAppointments('" + t.id + "');", 100);
                });
                hap.livetiles.RegisterTileHandler(/exchange.calendarinfo\:/gi, function (type, initdata, t) {
                    t.html = '<a id="' + t.id + '" href="' + hap.common.resolveUrl(initdata.Url) + '" target="' + initdata.Target + '" title="' + initdata.Description + '"' + ' class="tile width' + initdata.Width + ' height' + initdata.Height + '"' + (initdata.Color == '' ? '' : ' style="background-color: ' + initdata.Color.Base + ';" onmouseover="this.style.backgroundColor = \'' + initdata.Color.Light + '\';" onmouseout="this.style.backgroundColor = \'' + initdata.Color.Base + '\';" onmousedown="this.style.backgroundColor = \'' + initdata.Color.Dark + '\';"') + '><span><i style="background-image: url(' + hap.common.resolveUrl(initdata.Icon) + ');"></i><label></label></span>' + initdata.Name + '</a>';
                    t.Render();
                    $("#" + t.id).data("name", initdata.Name).data("mailbox", type.split(/exchange.calendarinfo\:/gi)[1]).addClass("appointment").click(function () {
                        $.ajax({
                            url: hap.common.formatJSONUrl("~/api/livetiles/exchange/calendarinfo"), context: t, type: 'POST', dataType: 'json', data: '{ "Mailbox" : "' + $('#' + t.id).data("mailbox") + '" }', contentType: 'application/JSON', success: function (data) {
                                var s = "";
                                var url = $("#" + this.id).attr("href");
                                var id = "#" + this.id;
                                for (var i = 0; i < data.length; i++)
                                    s += '<span style="font-size: 20px; display: block;">' + data[i].Subject + '</span>From: ' + data[i].Start + " To: " + data[i].End + "<br />" + unescape(data[i].Body).replace('\n', '') + "<hr />";
                                $("<div href=\"" + url + "\"/>").html(s).dialog({ width: 800, height: 500, title: $(id).data("name"), autoOpen: true, buttons: { "Open": function() { window.location.href = url; }, "Close": function () { $(this).dialog("close"); } } });
                            }, error: hap.common.jsonError
                        });
                        return false;
                    });
                    setTimeout("hap.livetiles.UpdateExchangeCalendarInfo('" + t.id + "', '" + t.type.split(/exchange.calendarinfo\:/gi)[1] + "');", 100);
                });
                hap.livetiles.RegisterTileHandler(/exchange.calendar\:/gi, function (type, initdata, t) {
                    t.html = '<a id="' + t.id + '" href="' + hap.common.resolveUrl(initdata.Url) + '" target="' + initdata.Target + '" title="' + initdata.Description + '"' + ' class="tile width' + initdata.Width + ' height' + initdata.Height + '"' + (initdata.Color == '' ? '' : ' style="background-color: ' + initdata.Color.Base + ';" onmouseover="this.style.backgroundColor = \'' + initdata.Color.Light + '\';" onmouseout="this.style.backgroundColor = \'' + initdata.Color.Base + '\';" onmousedown="this.style.backgroundColor = \'' + initdata.Color.Dark + '\';"') + '><span><i style="background-image: url(' + hap.common.resolveUrl(initdata.Icon) + ');"></i><label></label></span>' + initdata.Name + '</a>';
                    t.Render();
                    $("#" + t.id).addClass("appointment");
                    setTimeout("hap.livetiles.UpdateExchangeCalendar('" + t.id + "', '" + type.split(/exchange.calendar\:/gi)[1] + "');", 100);
                });
                hap.livetiles.RegisterTileHandler(/^me/gi, function (type, initdata, t) {
                    t.html = '<a id="' + t.id + '" href="' + hap.common.resolveUrl(initdata.Url) + '" target="' + initdata.Target + '" title="' + initdata.Description + '"' + ' class="tile width' + initdata.Width + ' height' + initdata.Height + '"' + (initdata.Color == '' ? '' : ' style="background-color: ' + initdata.Color.Base + ';" onmouseover="this.style.backgroundColor = \'' + initdata.Color.Light + '\';" onmouseout="this.style.backgroundColor = \'' + initdata.Color.Base + '\';" onmousedown="this.style.backgroundColor = \'' + initdata.Color.Dark + '\';"') + '><span><i style="background-image: url(' + hap.common.resolveUrl(initdata.Icon) + ');"></i><label></label></span>' + initdata.Name + '</a>';
                    t.Render();
                    $("#" + t.id).addClass("me");
                    $.ajax({
                        url: hap.common.formatJSONUrl("~/api/livetiles/me"), type: 'GET', context: t.id, dataType: "json", contentType: 'application/JSON', success: function (data) {
                            $("#" + this + " span label").html("<b>" + data.Name + "</b><br />" + data.Email);
                            if (data.Photo != "" && data.Photo != null) $("#" + this + " span i").css("background-image", "url(" + hap.common.resolveUrl(data.Photo) + ")");
                            setInterval("$('#" + this + " span i').animate({ height: 'toggle' });", 8000);
                        }, error: hap.common.jsonError
                    });
                });
                hap.livetiles.RegisterTileHandler("bookings", function (type, initdata, t) {
                    t.html = '<a id="' + t.id + '" href="' + hap.common.resolveUrl(initdata.Url) + '" target="' + initdata.Target + '" title="' + initdata.Description + '"' + ' class="tile width' + initdata.Width + ' height' + initdata.Height + '"' + (initdata.Color == '' ? '' : ' style="background-color: ' + initdata.Color.Base + ';" onmouseover="this.style.backgroundColor = \'' + initdata.Color.Light + '\';" onmouseout="this.style.backgroundColor = \'' + initdata.Color.Base + '\';" onmousedown="this.style.backgroundColor = \'' + initdata.Color.Dark + '\';"') + '><span><i style="background-image: url(' + hap.common.resolveUrl(initdata.Icon) + ');"></i><label class="text"></label></span>' + initdata.Name + '</a>';
                    t.Render();
                    setInterval("$('#" + t.id + " > span > i').animate({ height: 'toggle' });", 8000);
                    setTimeout("hap.livetiles.UpdateBookings('" + t.id + "');", 100);
                });
                hap.livetiles.RegisterTileHandler("helpdesk", function (type, initdata, t) {
                    t.html = '<a id="' + t.id + '" href="' + hap.common.resolveUrl(initdata.Url) + '" target="' + initdata.Target + '" title="' + initdata.Description + '"' + ' class="tile helpdesk width' + initdata.Width + ' height' + initdata.Height + '"' + (initdata.Color == '' ? '' : ' style="background-color: ' + initdata.Color.Base + ';" onmouseover="this.style.backgroundColor = \'' + initdata.Color.Light + '\';" onmouseout="this.style.backgroundColor = \'' + initdata.Color.Base + '\';" onmousedown="this.style.backgroundColor = \'' + initdata.Color.Dark + '\';"') + '><span><i style="background-image: url(' + hap.common.resolveUrl(initdata.Icon) + ');"></i><label class="count"></label><label class="text"></label></span>' + initdata.Name + '</a>';
                    t.Render();
                    setInterval("$('#" + t.id + " > span > i, #" + t.id + " > span > .count').animate({ height: 'toggle' });", 10000);
                    setTimeout("hap.livetiles.UpdateTickets('" + t.id + "');", 100);
                });
                hap.livetiles.RegisterTileHandler(/^uptime\:/gi, function (type, initdata, t) {
                    t.html = '<a id="' + t.id + '" href="' + hap.common.resolveUrl(initdata.Url) + '" target="' + initdata.Target + '" title="' + initdata.Description + '"' + ' class="tile width' + initdata.Width + ' height' + initdata.Height + '"' + (initdata.Color == '' ? '' : ' style="background-color: ' + initdata.Color.Base + ';" onmouseover="this.style.backgroundColor = \'' + initdata.Color.Light + '\';" onmouseout="this.style.backgroundColor = \'' + initdata.Color.Base + '\';" onmousedown="this.style.backgroundColor = \'' + initdata.Color.Dark + '\';"') + '><span><i style="background-image: url(' + hap.common.resolveUrl(initdata.Icon) + ');"></i><label></label></span>' + initdata.Name + '</a>';
                    t.Render();
                    $("#" + t.id).addClass("me");
                    setTimeout("hap.livetiles.UpdateUptime('" + t.id + "', '" + type.substr(7) + "');", 100);
                });
                hap.livetiles.RegisterTileHandler("myfiles", function (type, initdata, t) {
                    t.html = '<a id="' + t.id + '" href="' + hap.common.resolveUrl(initdata.Url) + '" target="' + initdata.Target + '" title="' + initdata.Description + '" class="tile me' + ' width' + initdata.Width  + ' height' + initdata.Height + '"' + (initdata.Color == '' ? '' : ' style="background-color: ' + initdata.Color.Base + ';" onmouseover="this.style.backgroundColor = \'' + initdata.Color.Light + '\';" onmouseout="this.style.backgroundColor = \'' + initdata.Color.Base + '\';" onmousedown="this.style.backgroundColor = \'' + initdata.Color.Dark + '\';"') + '><span><i style="background-image: url(' + hap.common.resolveUrl(initdata.Icon) + ');"></i><label></label></span>' + initdata.Name + '</a>';
                    t.Render();
                    $.ajax({
                        url: hap.common.formatJSONUrl("~/api/myfiles/drives"), type: 'GET', context: t.id, dataType: "json", contentType: 'application/JSON', success: function (data) {
                            var s = "";
                            for (var i = 0; i < (data.length > 3 ? 3 : data.length) ; i++)
                                s += "<b>" + data[i].Name + "</b>" + (data[i].Space == -1 ? "<br /><br />" : '<br /><span class="progress"><label>' + data[i].Space + '%</label><u style="width: ' + data[i].Space + '%"></u></span>');
                            $("#" + this + " span label").html(s);
                            setInterval("$('#" + this + " > span > i').animate({ height: 'toggle' });", 6000);
                        }, error: hap.common.jsonError
                    });
                });
            },
            LiveTile: function (type, initdata) {
                this.data = initdata;
                this.type = type;
                this.id = (this.data.Group + this.data.Name).replace(/['\/\\\&\.\,\?\!\£\$\%\^\*\(\)@]*/gi, "").replace(/\s/gi, '_');
                this.html = '<a id="' + this.id + '" href="' + hap.common.resolveUrl(this.data.Url) + '" target="' + this.data.Target + '" title="' + this.data.Description + '"' + 'class="tile width' + this.data.Width + ' height' + this.data.Height + '"' + (this.data.Color == '' ? '' : ' style="background-color: ' + this.data.Color.Base + ';" onmouseover="this.style.backgroundColor = \'' + this.data.Color.Light + '\';" onmouseout="this.style.backgroundColor = \'' + this.data.Color.Base + '\';" onmousedown="this.style.backgroundColor = \'' + this.data.Color.Dark + '\';"') + '><span><i style="background-image: url(' + hap.common.resolveUrl(this.data.Icon) + ');"></i><label></label></span>' + this.data.Name + '</a>';
                this.Render = function () {
                    if ($("#" + this.id).length > 0) return false;
                    else {
                        $("#" + this.data.Group).append($(this.html).attr("data-idname", this.data.Name));
                        if (hap.admin) {
                            $("#" + this.id).on("contextmenu", function (e) {
                                if ($(".editmode").length > 0) {
                                    var b = $(this).hasClass("selected");
                                    $(".selected").removeClass("selected");
                                    $("#sidebaredit").removeClass("show");
                                    $("#contextbar").removeClass("selectactive");
                                    if (!b) { $("#contextbar").addClass("selectactive"); $(this).addClass("selected"); }
                                    return false;
                                }
                            });
                        }
                        return true;
                    }
                }
            },
            UpdateExchangeCalendarInfo: function (tileid, mailbox) {
                $("#" + tileid + " span label").html($.datepicker.formatDate('D <b>d</b>', new Date()));
                $.ajax({
                    url: hap.common.formatJSONUrl("~/api/livetiles/exchange/calendarinfo"), type: 'POST', dataType: 'json', data: '{ "Mailbox" : "' + mailbox + '" }', context: { tile: tileid, mb: mailbox }, contentType: 'application/JSON', success: function (data) {
                        var s = "";
                        for (var i = 0; i < data.length; i++)
                            s += data[i].Start + " - " + data[i].End + "<br />" + data[i].Subject + "<br />";
                        $("#" + this.tile + " span i").html(s);
                        if (data.length > 0) $("#" + this.tile + " span i").attr("style", "background-image: url();");
                        setTimeout("hap.livetiles.UpdateExchangeCalendarInfo('" + this.tile + "', '" + this.mb + "');", 100000);
                    }, error: hap.common.jsonError
                });
            },
            UpdateExchangeCalendar: function (tileid, mailbox) {
                $("#" + tileid + " span label").html($.datepicker.formatDate('D <b>d</b>', new Date()));
                $.ajax({
                    url: hap.common.formatJSONUrl("~/api/livetiles/exchange/calendar"), type: 'POST', dataType: 'json', data: '{ "Mailbox" : "' + mailbox + '" }', context: { tile: tileid, mb: mailbox }, contentType: 'application/JSON', success: function (data) {
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
                    url: hap.common.formatJSONUrl("~/api/livetiles/exchange/appointments"), type: 'GET', context: tileid, dataType: "json", contentType: 'application/JSON', success: function (data) {
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
                    url: hap.common.formatJSONUrl("~/api/BookingSystem/Search"),
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
                        $('#' + this + " span label").html(d);
                        setTimeout("hap.livetiles.UpdateBookings('" + this + "');", 110000);
                    },
                    error: hap.common.jsonError
                });
            },
            UpdateTickets: function (tileid) {
                $.ajax({
                    type: 'GET',
                    url: hap.common.formatJSONUrl("~/api/HelpDesk/Tickets/Open" + (hap.hdadmin ? '' : ('/' + hap.user))),
                    dataType: 'json',
                    context: tileid,
                    contentType: 'application/json',
                    success: function (data) {
                        var x = "";
                        var y = 0;
                        data.reverse();
                        for (var i = 0; i < data.length; i++) {
                            x += '<b>' + (i + 1) + '</b>:' + data[i].Subject + '<br />';
                            var read = false;
                            for (var a = 0; a < data[i].ReadBy.split(",").length; a++)
                                if ($.trim(data[i].ReadBy.split(",")[a].toLowerCase()) == hap.user.toLowerCase()) read = true;
                            if (!read) y++;
                        }
                        if (data.length == 0) x = "No Open Tickets";
                        $("#" + this + " span label").html(x);
                        if (y > 0) $("#" + this + " span i").animate({ width: 180 }, 500, function () { $(this).parent().children("label.count").html(y); });
                        else {
                            $("#" + this + " span .count").html("");
                            $("#" + this + " span i").animate({ width: 227 });
                        }
                        setTimeout("hap.livetiles.UpdateTickets('" + this + "');", 500000);
                    },
                    error: hap.common.jsonError
                });
            },
            UpdateExchangeMail: function (tileid) {
                $.ajax({
                    url: hap.common.formatJSONUrl("~/api/livetiles/exchange/unread"), type: 'GET', context: tileid, dataType: "json", contentType: 'application/JSON', success: function (data) {
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
                    url: hap.common.formatJSONUrl("api/livetiles/uptime/" + server), type: 'GET', context: con, dataType: "json", contentType: 'application/JSON', success: function (data) {
                        $("#" + this.tile + " span i").html(data);
                        setTimeout("hap.livetiles.UpdateUptime('" + this.tile + "', '" + this.server + "');", 5000);
                    }, error: hap.common.jsonError
                });
            }
        }
    };
    hap.livetiles.RegisterDefaultTiles();
    $(document).ajaxStart(function () {
        $("#hapLoader").addClass("go");
    }).ajaxStop(function () {
        $("#hapLoader").removeClass("go");
    });
    $(function () {
        hap.header.Init();
        if (hap.load > hap.loadtypes.none) {
            hap.help.Init();
            hap.common.keepAlive();
        }
        hap.common.makeSwitchs();
    });
    $.fn.hapPopup = function (e) {
        if (!e) e = { buttons: [{ Text: "Close", Click: function () { $(this).parents(".hapPopup").hide(); return false; } }] };
        else if (e == "close") { $(this).hide(); return this; }
        this.show();
        if (!this.hasClass("hapPopup")) {
            this.addClass("hapPopup").contents().wrapAll('<table class="hapPopup-table" cellpadding=0 border=0 cellspacing=0><tr><td><div class="hapPopup-wrapper"><div class="hapPopup-wrapperinner"><div class="hapPopup-content"></div></div></div></td></tr></table>');
            if (this.attr("title")) this.find(".hapPopup-wrapperinner").prepend('<h1 class="hapPopup-title">' + this.attr("title") + '</h1>');
            if (e) {
                if (e.buttons) {
                    var a = $('<div class="hapPopup-buttonset"></div>');
                    for (var i = 0; i < e.buttons.length; i++)
                        $("<button>" + e.buttons[i].Text + "</button>").click(e.buttons[i].Click).appendTo(a);
                    this.find(".hapPopup-wrapperinner").append(a);
                    this.find('button').button();
                }
            }
            this.find('.hapPopup-content').css('max-height', parseInt($("#hapContent").css("min-height").replace(/px/gi, "")) - 200);
        }
        return this;
    };
}