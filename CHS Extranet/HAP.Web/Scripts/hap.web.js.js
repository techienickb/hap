/*HAP.Web.JS.js - Copyright © 2012 nb development - Version 1 */
if (hap == null)
    var hap = {
        root: "/hap/",
        user: "",
        common: {
            jsonError: function (xhr, ajaxOptions, thrownError) {
                try {
                    if (xhr.responseText.match(/\<!doctype html\>/gi)) window.location.reload();
                    else {
                        console.log(xhr.responseXML.documentElement.children[2]);
                        if (xhr.responseXML.documentElement.children[1].children[0].textContent != "") alert(xhr.responseXML.documentElement.children[1].children[0].textContent);
                    }
                } catch (e) { if (thrownError != "") alert(thrownError); }
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
        localization: [],
        livetiles : {
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
                            alert("Feature Coming Soon...");
                            return false;
                        });
                    }
            },
            LiveTile: function (type, initdata, size) {
                this.id = (initdata.Group + initdata.Name).replace(/[\s'\/\\\&\.\,\*]*/gi, "");
                if (type == "exchange.appointments" || type == "bookings" || type == "helpdesk") size = "large";
                this.html = '<a id="' + this.id + '" href="' + hap.common.resolveUrl(initdata.Url) + '" target="' + initdata.Target + '" title="' + initdata.Description + '"' + (size == 'large' ? ' class="large"' : '') + (initdata.Color == '' ? '' : ' style="background-color: ' + initdata.Color + ';"') + '><span><i style="background-image: url(' + hap.common.resolveUrl(initdata.Icon) + ');"></i><label></label></span>' + initdata.Name + '</a>';
                $("#" + initdata.Group).append(this.html);
                if (type == "exchange.unread") {
                    setTimeout("hap.livetiles.UpdateExchangeMail('" + this.id + "');", 100);
                } else if (type == "exchange.appointments") {
                    $("#" + this.id).addClass("appointment");
                    setTimeout("hap.livetiles.UpdateExchangeAppointments('" + this.id + "');", 100);
                } else if (type == "me") {
                    $("#" + this.id).addClass("me").click(function () {
                        scrollpos = $("#HomeButtons > .me").index();
                        $("#HomeButtonsOutter").animate({ scrollLeft: (scrollpos * ($("#HomeButtonsOutter").width() - 20) + (scrollpos * 20)) });
                        $("#HomeButtonsHeader h1").removeClass("active");
                        $("#HomeButtonsHeader h1")[scrollpos].className = "active";
                        return false;
                    });
                    $.ajax({
                        url: "api/livetiles/me", type: 'GET', context: this.id, dataType: "json", contentType: 'application/JSON', success: function (data) {
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
                            for (var i = 0; i < (data.length > 3 ? 3 : data.length); i++)
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
            UpdateExchangeAppointments: function (tileid) {
                $("#" + tileid + " span label").html($.datepicker.formatDate('D <b>d</b>', new Date()));
                $.ajax({
                    url: "api/livetiles/exchange/appointments", type: 'GET', context: tileid, dataType: "json", contentType: 'application/JSON', success: function (data) {
                        var s = "";
                        for (var i = 0; i < data.length; i++)
                            s += data[i] + "<br />";
                        $("#" + this + " span i").html(s);
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
                        if (data.length == 0) $("#" + this + " span i").attr("style", "background-image: url(" + hap.common.resolveUrl('~/images/icons/white/bookingsystem.png') + ");");
                        $('#' + this + " span i").html(d);
                        setTimeout("hap.livetiles.UpdateBookings('" + this + "');", 110000);
                    },
                    error: hap.common.jsonError
                });
            },
            UpdateTickets: function (tileid) {
                $.ajax({
                    type: 'GET',
                    url: hap.common.resolveUrl("~/api/HelpDesk/Tickets/Open") + '?' + window.JSON.stringify(new Date()),
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
                    url: "api/livetiles/exchange/unread", type: 'GET', context: tileid, dataType: "json", contentType: 'application/JSON', success: function (data) {
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
$(function () { hap.help.Init(); });