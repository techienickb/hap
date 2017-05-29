var curticket;
var st = "";
var userlist2 = null;
var userlist = null;
var newticket_pc = null;
var uploads = [];

function genAttachments() {
    var a = [];
    for (var i = 0; i < uploads.length; i++)
        a.push('{ "Name": "' + escape(uploads[i].d.name) + '", "CType": "' + escape(uploads[i].d.type) + '" }');
    return '[' + a.join(', ') + ']';
}

function loadTicket() {
    uploads = [];
    $("#ticket-uploads").remove();
    $("#tabs a.selected").removeClass("selected");
    $("#newticket-link").removeClass("active");
    $("#tabs").removeClass("ticketactive");
    $("#HDmain > div").hide();
    if (curticket != null) {
        $("#tabs").addClass("ticketactive");
        $("#currentticket").show();
        $("#curtick-loading").show();
        var stringticket = curticket.match(/\//gi) ? curticket.split(/\//g)[1] : curticket;
        $("#ticket_" + stringticket).addClass("selected").removeClass("unread");
        $('<div id="ticket-' + curticket.replace(/\//gi, "_") + '" class="ticket">Loading Ticket ' + stringticket + '...</div>').appendTo("#curtick-loading");
        if (curticket.match(/\//gi)) { $("#HDcontrols, #HDlowercontrols").hide(); $("#currentticket").addClass("nocontrol"); }
        else { $("#HDcontrols, #HDlowercontrols").show(); $("#currentticket").removeClass("nocontrol"); }
        $.ajax({
            type: 'GET',
            url: hap.common.formatJSONUrl("~/api/HelpDesk/" + (curticket.match(/\//gi) ? 'A' : '') + "Ticket/" + curticket),
            dataType: 'json',
            contentType: 'application/json',
            success: function (data) {
                renderTicket(data);
            }, error: hap.common.jsonError
        });
    }
}

function renderTicket(data) {
    $("#ticket-uploads").remove();
    if (data.FAQ) { $("#HDcontrols, #HDlowercontrols").hide(); $("#currentticket").addClass("nocontrol"); }
    $("#ticket-note, #ticket-AwareI").val("");
    $("#ticket-hidenote").prop("checked", false).prev().removeClass("on");
    $(".ui-tabs-selected a span").html("Ticket: " + data.Subject);
    $("#ticket-Subject").html(data.Subject).prev().html("Ticket " + (curticket.match(/\//gi) ? curticket.split(/\//g)[1] : curticket) + ": ");
    $("#ticket-Username").html(data.Username);
    $("#ticket-Date").html(data.Date);
    $("#ticket-Priority").html(data.Priority);
    $("#ticket-Status").html(data.Status);
    $("#ticket-AssignedTo").html(hap.hdadmin ? (data.AssignedTo.length > 0 ? data.AssignedTo : "----") : data.AssignedTo);
    $("#curtick-loading").hide();
    $("#curtick-loading .ticket").remove();
    origshowto = data.ShowTo;
    $("ticket-ShowToI").val(data.ShowTo);
    var h = "";
    for (var i = 0; i < data.Notes.length; i++) {
        var d = replaceURLWithHTMLLinks(unescape(data.Notes[i].NoteText).replace(/\+/g, ' '));
        d = d.replace(/#(\d+)/gi, '<a href="#ticket-$1">#$1</a>');
        h += data.Notes[i].DisplayName + ' ' + data.Notes[i].Date + '<br /><pre>' + d + '</pre>';
        if (data.Notes[i].Attachments.length > 0) {
            h += 'Attachments: ';
            var a = [];
            for (var x = 0; x < data.Notes[i].Attachments.length; x++)
                a.push('<a href="' + hap.common.formatJSONUrl("~/api/helpdesk/Get/" + data.Notes[i].Id + "/" + escape(data.Notes[i].Attachments[x].Name)) + '" target="_blank">' + data.Notes[i].Attachments[x].Name + '</a>');
            h += a.join(", ");
            h += "<br />";
        }
    }
    $("#notes").html(h);
    $("button").button();
}

function replaceURLWithHTMLLinks(text) {
    var exp = /(\b(https?|ftp|file):\/\/[-A-Z0-9+&@#\/%?=~_|!:,.;]*[-A-Z0-9+&@#\/%=~_|])/ig;
    return text.replace(exp, "<a href='$1'>$1</a>");
}

function assignTicket() {
    $("#curtick-loading").show();
    var url = hap.common.resolveUrl("~/api/HelpDesk/AdminTicket/") + curticket + '?' + window.JSON.stringify(new Date());
    var data = '{ "Note": "", "State": "", "Priority": "", "ShowTo": "", "FAQ": "", "AssignTo": "' + userlist2.val() + '", "Subject": "" }';
    $.ajax({
        type: 'PUT',
        url: url,
        dataType: 'json',
        data: data,
        contentType: 'application/json',
        error: hap.common.jsonError,
        success: function (data) {
            Update();
            renderTicket(data);
        }
    });
}
function updateTicket() {
    $("#curtick-loading").show();
    var s = $("#ticket-Status").text();
    if (s == "User Attention Needed" && !hap.hdadmin) s = "Investigating";
    var data = '{ "Note": "' + escape($("#ticket-note").val()) + '", "State": "' + s + '", "Priority": "' + $("#ticket-Priority").text() + '", "ShowTo": "' + $("#ticket-ShowToI").text() + '", "FAQ": "false", "AssignTo": "' + userlist2.val() + '", "Subject": "' + $("#ticket-Subject").text() + '"' + (hap.hdadmin ? (', "HideNote": ' + $("#ticket-hidenote").is(":checked")) : '') + ', "Attachments": ' + genAttachments() + ' }';
    var url = (hap.hdadmin) ? hap.common.formatJSONUrl("~/api/HelpDesk/AdminTicket/" + curticket) : hap.common.formatJSONUrl("~/api/HelpDesk/Ticket/" + curticket);
    $.ajax({
        type: 'PUT',
        url: url,
        dataType: 'json',
        data: data,
        contentType: 'application/json',
        error: hap.common.jsonError,
        success: function (data) {
            renderTicket(data);
            Update();
        }
    });
    return false;
}

function setFAQ() {
    $("#curtick-loading").show();
    var s = $("#ticket-Status").text();
    var data = '{ "Note": "' + escape($("#ticket-note").val()) + '", "State": "' + + '", "Priority": "' + $("#ticket-Priority").text() + '", "ShowTo": "' + $("#ticket-ShowToI").val() + '", "FAQ": "true", "AssignTo": "", "Subject": "' + $("#ticket-Subject").text() + '" }';
    var url = (hap.hdadmin) ? hap.common.formatJSONUrl("~/api/HelpDesk/Ticket/" + curticket) : hap.common.formatJSONUrl("~/api/HelpDesk/AdminTicket/" + curticket);
    $.ajax({
        type: 'PUT',
        url: url,
        dataType: 'json',
        data: data,
        contentType: 'application/json',
        error: hap.common.jsonError,
        success: function (data) {
            renderTicket(data);
            Update();
        }
    });
    return false;
}

function fileTicket() {
    if ($("#newticket-subject").val().length == 0) { alert("Ticket Subject needs to be entered"); return; }
    var data = '{ "Subject": "' + escape($("#newticket-subject").val()) + '", "Room": "' + $("#newticket-room").val() + '", "Note": "' + escape($("#newticket-note").val()) + '"';
    var url = hap.common.formatJSONUrl("~/api/HelpDesk/Ticket");
    if (hap.hdadmin) {
        data += ', "Priority": "' + $("#priorityradioes input:checked").val() + '", "User": "' + userlist.val() + '", "ShowTo": "' + $("#newticket-showto").val() + '"';
        url = hap.common.resolveUrl("~/api/HelpDesk/AdminTicket");
    }
    data += ', "Attachments": ' + genAttachments() + ' }';
    $.ajax({
        type: 'POST',
        url: url,
        dataType: 'json',
        data: data,
        contentType: 'application/json',
        success: function (data) {
            $("#newticket-subject").val("");
            $("#newticket-room").val(newticket_pc.val());
            $("#newticket-note").val("");
            if (hap.hdadmin) {
                $("#priorityradioes input:checked").removeAttr("checked");
                $("#newticket-showto").val("");
            }
            location.href = "#ticket-" + data.Id;
            Update();
        }, error: hap.common.jsonError
    });
    return false;
}

function upload(d) {
    this.onProgress = function (e) {
        for (var i = 0; i < uploads.length; i++) if (uploads[i].d.size == e.total) uploads[i].updateProgress(parseInt((e.loaded / e.total) * 100));
    };
    this.updateProgress = function (e) {
        $("#upload-" + this.xhr.id + " progress").val(e);
    }
    this.xhr = new XMLHttpRequest();
    this.xhr.id = this.id = d.name.replace(/[^\w]/gi, "");
    this.d = d;
    this.xhr.upload.addEventListener("progress", this.onProgress, false);
    this.xhr.addEventListener("progress", this.onProgress, false);
    this.xhr.onprogress = this.onProgress;
    this.xhr.open('POST', hap.common.resolveUrl('~/api/helpdesk-upload'), true);
    this.xhr.onreadystatechange = function () {
        if (this.readyState == 4) {
            if ($("#ticket-uploads").length == 0) (curticket == null ?  $("#newticket-file") : $("#ticket-file")).before('<span id="ticket-uploads">Pending attachments: </span>');
            $("#ticket-uploads").append(unescape(this.responseText) + ", ");
            for (var i = 0; i < uploads.length; i++) if (uploads[i].d.name == unescape(this.responseText)) {
                var u = uploads[i];
                if (u.d.type.match(/^image\//gi)) {
                    var reader = new FileReader();
                    reader.onload = function (e) {
                        $("#upload-" + u.id).append('<img src="' + e.target.result + '" width="150px" />');
                    }
                    reader.readAsDataURL(u.d);
                }
                $("#upload-" + u.id).append("Uploaded");
                $("#upload-" + u.id + " progress").remove();
            }
        }
    };
    this.xhr.setRequestHeader('X_FILENAME', d.name);
    this.xhr.send(d);
}

$(window).hashchange(function () {
    if (window.location.href.split('#')[1] != "" && window.location.href.split('#')[1]) {
        curticket = window.location.href.split('#')[1].substr(7);
        loadTicket();
    }
    else curticket = null;
});
$(function () {
    $("#newticket-room").val(newticket_pc.val());
    $("#HDtop div span").click(function () {
        var next = $(this).next();
        if (!hap.hdadmin && $(this).parent().hasClass("hdadmin") || $("#currentticket").hasClass("nocontrol")) return;
        if (next.is("input") || next.is("select")) next.val($(this).hide().text()).show().focus();
    }).next("input[type=text],select").hide().focusout(function () {
        $(this).prev().show().text($(this).hide().val());
    });
    if (!hap.hdadmin) { $("#HDtop .hdadmin, #ticket-hidenote").hide(); if ($("#ticket-hidenote").prev().is(".hapswitch")) $("#ticket-hidenote").prev().hide(); }
    $("#updateticket, #assignticket").dialog({ autoOpen: false });
    $("#tabs > div, #HDmain > div").hide();
    $("#toolbar a:not(#header_migrate)").click(function () {
        if ($(this).index() < 5) $("#updateticket, #assignticket").dialog("close");
        if ($(this).index() > 3) { $("#HDmain > div").hide(); window.location.href = "#"; $("#newticket-link, #stats-link").removeClass("active"); $("#tabs").addClass("ticketactive"); }
        else { $("#tabs > div, #newticket, #stats").hide(); $("#toolbar a").removeClass("active"); }
        $($(this).addClass("active").attr("href")).show();
        return false;
    });
    $("button, .button, input[type=submit]").button();
    if (window.location.href.split('#')[1] != "" && window.location.href.split('#')[1]) curticket = window.location.href.split('#')[1].substr(7);
    else curticket = null;
    $("#toolbar a").first().click();
    $("#ticket-note").keyup(function () {
        if ($("#ticket-note").val().match(/@\S+/gi)) {
            var showto = origshowto;
            for (var i = 0; i < $("#ticket-note").val().match(/@\S+/gi).length; i++) {
                if (origshowto.match($("#ticket-note").val().match(/@\S+/gi)[i].substr(1)) || $("#ticket-note").val().match(/@\S+/gi)[i].substr(1).toLowerCase() == $("#ticket-Username").val().toLowerCase() || $("#ticket-note").val().match(/@\S+/gi)[i].substr(1).toLowerCase() == $("#ticket-AssignedTo").val().toLowerCase()) continue;
                if (showto != "") showto += ", ";
                showto += $("#ticket-note").val().match(/@\S+/gi)[i].substr(1);
            }
            $("#ticket-ShowToI").val(showto);
        }
    });
    $("#ticket-file, #newticket-file").click(function () {
        if ($("#uploadbox").length == 0) {
            $("#hapContent").append('<div title="Add File" id="uploadbox"><div><p>Drag a file into this box or click below</p><input type="file" multiple="multiple" id="uploadfiles" /></div></div>');
            $("#uploadbox").hapPopup();
            $("#uploadbox .hapPopup-content").css("height", parseInt($("#uploadbox .hapPopup-content").css("max-height").replace(/px/gi, "")) - 4 + "px").css("text-align", "center").css("border", "padding 1px #ccc").css("margin", "2px").on("dragover", function () {
                $("#uploadbox .hapPopup-content").css("border", "dashed 3px #0060a6").css("padding", "0");
                return false;
            }).on("dragleave", function () {
                $("#uploadbox .hapPopup-content").css("border", "dashed 1px #ccc").css("padding", "2px");
                return false;
            }).on("dragend", function () {
                $("#uploadbox .hapPopup-content").css("border", "dashed 1px #ccc").css("padding", "2px");
                return false;
            }).attr("dropzone", "copy f:image/png f:image/gif f:image/jpeg f:image/jpg")[0].ondrop = function (event) {
                event.stopPropagation();
                event.preventDefault();
                $("#uploadbox .hapPopup-content").css("border", "dashed 1px #ccc").css("padding", "2px");
                var files = (event.target.files || event.dataTransfer.files);
                for (var i = 0; i < files.length; i++) {
                    var file = files[i];
                    if (file.type.match(/application\/x-zip/gi) || file.type.match(/^image\//gi)) {
                        $("#uploadbox .hapPopup-content").append('<div style="display: inline-block; width: 150px; height: 150px;" id="upload-' + file.name.replace(/[^\w]/gi, "") + '">' + file.name + '<br /><progress value="0" max="100"></progress></div>');
                        uploads.push(new upload(file));

                    } else alert("This file type cannot be uploaded");
                }
                return false;
            };
        } else {
            $("#uploadbox .hapPopup-content").html('<div><p>Drag a file into this box or click below</p><input type="file" multiple="multiple" id="uploadfiles" /></div>');
            $("#uploadbox").hapPopup();
        }
        $("#uploadfiles").change(function () {
            for (var i = 0; i < this.files.length; i++) {
                var file = this.files[i];
                if (file.type.match(/application\/x-zip/gi) || file.type.match(/^image\//gi)) {
                    $("#uploadbox .hapPopup-content").append('<div style="display: inline-block; width: 150px; height: 150px;" id="upload-' + file.name.replace(/[^\w]/gi, "") + '">' + file.name + '<br /><progress value="0" max="100"></progress></div>');
                    uploads.push(new upload(file));

                } else alert("This file type cannot be uploaded");
            }
            $(this).val("");
        });
        return false;
    });
    loadTicket();
    Update();
});
var origshowto;
function isRead(o) {
    for (var i = 0; i < o.split(/,/g).length; i++)
        if ($.trim(o.split(/,/g)[i].toLowerCase()) == hap.user.toLowerCase()) return true;
    return false;
}
function Update() {
    $.ajax({
        type: 'GET',
        url: hap.common.formatJSONUrl("~/api/HelpDesk/Tickets/Open" + (hap.hdadmin ? '' : ("/" + hap.user))),
        dataType: 'json',
        contentType: 'application/json',
        success: function (data) {
            var x = "";
            for (var i = 0; i < data.length; i++) x += '<div><a href="#ticket-' + data[i].Id + '" class="' + data[i].Priority.replace(/ /g, "-") + ' ' + (isRead(data[i].ReadBy) ? '' : 'unread') + '" id="ticket_' + data[i].Id + '">' + data[i].Subject + ' <span>' + data[i].Id + ' - ' + data[i].Username + (data[i].AssignedTo == '' ? '' : (' -> ' + data[i].AssignedTo)) + ' - ' + data[i].Date + '</span></a></div>';
            if (data.length == 0) x = "No Tickets";
            $("#opentickets").html(x);
            $("#ticket_" + curticket).addClass("selected").removeClass("unread");
        }, error: hap.common.jsonError
    });
    $.ajax({
        type: 'GET',
        url: hap.common.formatJSONUrl("~/api/HelpDesk/Tickets/Closed" + (hap.hdadmin ? '' : ("/" + hap.user))),
        dataType: 'json',
        contentType: 'application/json',
        success: function (data) {
            var x = "";
            for (var i = 0; i < data.length; i++) x += '<div><a href="#ticket-' + data[i].Id + '" class="' + data[i].Priority.replace(/ /g, "-") + ' ' + (isRead(data[i].ReadBy) ? '' : 'unread') + '" id="ticket_' + data[i].Id + '">' + data[i].Subject + ' <span>' + data[i].Id + ' - ' + data[i].Username + (data[i].AssignedTo == '' ? '' : (' -> ' + data[i].AssignedTo)) + ' - ' + data[i].Date + '</span></a></div>';
            if (data.length == 0) x = "No Tickets";
            $("#closedtickets").html(x);
            $("#ticket_" + curticket).addClass("selected").removeClass("unread");
        }, error: hap.common.jsonError
    });
    $.ajax({
        type: 'GET',
        url: hap.common.formatJSONUrl("~/api/HelpDesk/FAQs"),
        dataType: 'json',
        contentType: 'application/json',
        success: function (data) {
            var x = "";
            for (var i = 0; i < data.length; i++) {
                x += '<div><a href="#ticket-' + data[i].Id + '" class="' + data[i].Priority.replace(/ /g, "-") + ' ' + (isRead(data[i].ReadBy) ? '' : 'unread') + '" id="ticket_' + data[i].Id + '">' + data[i].Subject + '</a></div>'
            }
            if (data.length == 0) x = "No FAQs";
            $("#faqs").html(x);
            $("#ticket_" + curticket).addClass("selected").removeClass("unread");
        }, error: hap.common.jsonError
    });
    if (hap.hdadmin) {
        $("#spinner").spinner().spinner("value", 7);
        $("#stats-content").fadeOut();
        $("#refreshstats").click(function () {
            $("#stats-loading").fadeIn();
            $("#stats-content").fadeOut();
            $.ajax({
                type: 'GET',
                url: hap.common.formatJSONUrl("~/api/HelpDesk/Stats/" + $("#spinner").spinner("value")),
                dataType: 'json',
                contentType: 'application/json',
                success: function (data) {
                    parseStats(data);
                }, error: hap.common.jsonError
            });
            return false;
        });
        setTimeout(function () {
            $.ajax({
                type: 'GET',
                url: hap.common.formatJSONUrl("~/api/HelpDesk/Stats"),
                dataType: 'json',
                contentType: 'application/json',
                success: function (data) {
                    parseStats(data);
                }, error: hap.common.jsonError
            });
        }, 500);
    }
}
function parseStats(data) {
    $("#stats-loading").fadeOut();
    $("#stats-content").fadeIn();
    var s = "<div>Closed Tickets: <b>" + data.ClosedTickets + "</b></div><div>New Tickets: <b>" + data.NewTickets + "</b></div><div>Open Tickets: <b>" + data.OpenTickets + "</b></div>";
    if (data.HighestUser != null) s += "<div>Highest User: <b>" + data.HighestUser.Username + "</b> with <b>" + data.HighestUser.Tickets + "</b> tickets</div>";
    $("#stats-body").html(s);
}