<%@ Page Title="Crickhowell High School - IT - Home Access Plus+" Language="C#" MasterPageFile="~/masterpage.master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="HAP.Web.Default" %>
<%@ Register TagName="announcement" TagPrefix="hap" Src="~/Announcement.ascx" %>
<%@ Register TagName="version" TagPrefix="hap" Src="~/UpdateChecker.ascx" %>
<asp:Content runat="server" ContentPlaceHolderID="head">
    <link rel="stylesheet" type="text/css" href="style/jquery.wysiwyg.css" />
    <script src="Scripts/jquery.ba-hashchange.min.js"></script>
    <script src="Scripts/jquery.mousewheel.js"></script>
    <script src="Scripts/jquery.event.move.js"></script>
    <script src="Scripts/jquery.event.swipe.js"></script>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="viewport"><meta name="viewport" content="width=device-width, initial-scale=1.0, maximum-scale=1.0, user-scalable=0" /></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div id="hapHomeContainer">
        <div id="hapHomeSide" class="tile-color"></div>
        <div id="hapHome">
            <hap:announcement runat="server" />
            <hap:version runat="server" />
            <div id="title">
                <h1><a href="http://hap.codeplex.com" target="_blank"><hap:LocalResource StringPath="homeaccessplus" runat="server" /></a></h1>
                <h2>Access your School from Home</h2>
            </div>
            <div id="HomeButtonsHeader">
                <asp:Repeater ID="homepageheaders" runat="server">
                    <ItemTemplate><h1 id="header-<%#Eval("Name").ToString().Replace(" ", "").Replace("'", "").Replace("+", "").Replace(".", "").Replace(",", "").Replace("&", "").Replace("/", "").Replace("\\", "") %>"><a href="#-<%#Eval("Name").ToString().Remove(1).ToLower() %><%#Eval("Name").ToString().Replace(" ", "").Replace("'", "").Replace("+", "").Replace(".", "").Replace(",", "").Replace("&", "").Replace("/", "").Replace("\\", "").Remove(0, 1) %>" title="Show the <%#Eval("Name") %> Tab"><%#Eval("Name") %></a></h1></ItemTemplate>
                </asp:Repeater>
            </div>
            <div id="HomeButtonsContainer">
                <div id="HomeButtonsOutter">
                    <div id="HomeButtons" class="tiles">
                        <asp:Repeater ID="homepagelinks" runat="server">
                            <ItemTemplate>
                                <div class="panel<%#(((bool)Eval("HideHomePageLink")) ? " no-scroll" : "") %>" id="panel-<%#Eval("Name").ToString().Replace(" ", "").Replace("'", "").Replace("+", "").Replace(".", "").Replace(",", "").Replace("&", "").Replace("/", "").Replace("\\", "") %>">
                                    <%#Eval("SubTitle").ToString() == "#me" ? "" : Eval("SubTitle").ToString() %>
                                    <div <%#Eval("SubTitle").ToString() == "#me" ? "class=\"me\" " : "" %>id="<%#((string)Eval("Name")).Replace(" ", "").Replace("'", "").Replace(",", "").Replace(".", "").Replace("*", "").Replace("&", "").Replace("/", "").Replace("\\", "") %>">
                                        <script type="text/javascript">
                                            $(document).ready(function () {
                                                hap.livetiles.Init([
                                                <%#gettiles((HAP.Web.Configuration.LinkGroup)Container.DataItem)%>
                                                ]);
                                            });
                                        </script>
                                    </div>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>
                    <p>&nbsp;</p>
                </div>
                <a id="rightscoll" href="#right"></a>
                <a id="leftscroll" href="#left"></a>
            </div>
            <% if (User.IsInRole("Domain Admins")) { %>
            <div id="sidebaredit" style="margin-top: -40px">
                <h1>Tile Editor</h1>
                <input type="hidden" id="edit-data" />
                <label for="edit-name">Name: </label><input type="text" id="edit-name" /><br />
                <label for="edit-description">Description: </label><input type="text" id="edit-description" /><br />
                <label for="edit-url">Url: </label><input type="text" id="edit-url" /><br />
                <label for="edit-target">Target: </label><input type="text" id="edit-target" /><br />
                <label for="edit-icon">Icon: </label><input type="text" id="edit-icon" /><br />
                <label for="edit-width">Width: </label><select id="edit-width"><option>1</option><option>2</option><option>3</option></select><br />
                <label for="edit-height">Height: </label><select id="edit-height"><option>1</option><option>2</option><option>3</option></select><br />
                <div class="buttons">
                    <button id="edit-save">Save</button><button id="edit-cancel">Cancel</button>
                </div>
            </div>
            <div id="contextbar">
                <a href="#edit" id="edit">Edit Mode</a>
                <a href="#add-link" id="add-link">Add Link</a>
                <a href="#edit-link" id="edit-link">Edit Link</a>
                <a href="#del-link" id="delete-link">Delete Link</a>
            </div>
            <script>
                $("#add-link").click(function () {
                    $("#sidebaredit").addClass("show");
                    $("#contextbar").removeClass("show");
                    $("#sidebaredit input").val("");
                    return false;
                });
                $("#edit-link").click(function () {
                    var tile = null;
                    for (var i = 0; i < hap.livetiles.Tiles.length; i++)
                        if (hap.livetiles.Tiles[i].id == $("#HomeButtons .selected").attr("id")) { tile = hap.livetiles.Tiles[i]; break; }
                    if (tile != null) {
                        $("#sidebaredit").addClass("show");
                        $("#contextbar").removeClass("show");
                        $("#edit-name, #edit-data").val(tile.data.Name);
                        $("#edit-description").val(tile.data.Description);
                        $("#edit-url").val(tile.data.Url);
                        $("#edit-target").val(tile.data.Target);
                        $("#edit-width").val(tile.data.Width);
                        $("#edit-height").val(tile.data.Height);
                        $("#edit-icon").val("~/" + tile.data.Icon.split(/\/64\/64\//gi)[1]);
                    }
                    return false;
                });
                $("#edit-save").click(function () {
                    if ($("#edit-data").val().length == 0)
                    {
                        $.ajax({
                            type: 'POST',
                            url: 'API/Setup/AddLink',
                            data: '{ "group": "' + $("#HomeButtonsHeader .active a").html() + '", "name": "' + $("#edit-name").val() + '", "desc": "' + $("#edit-description").val() + '", "icon": "' + $("#edit-icon").val() + '", "url": "' + $("#edit-url").val() + '", "target": "' + $("#edit-target").val() + '", "showto": "Inherit", "width": "' + $("#edit-width").val() + '", "height": "' + $("#edit-height").val() + '" }',
                            contentType: 'application/json',
                            dataType: 'json',
                            success: function (data) {
                                location.reload();
                            },
                            error: hap.common.jsonError
                        });
                    } else {
                        $.ajax({
                            type: 'POST',
                            url: 'API/Setup/UpdateLink',
                            data: '{ "group": "' + $("#HomeButtonsHeader .active a").html() + '", "origname": "' + $("#edit-data").val() + '", "name": "' + $("#edit-name").val() + '", "desc": "' + $("#edit-description").val() + '", "icon": "' + $("#edit-icon").val() + '", "url": "' + $("#edit-url").val() + '", "target": "' + $("#edit-target").val() + '", "showto": "Inherit", "width": "' + $("#edit-width").val() + '", "height": "' + $("#edit-height").val() + '" }',
                            contentType: 'application/json',
                            dataType: 'json',
                            success: function (data) {
                                $("#contextbar").addClass("show");
                                $("#sidebaredit").removeClass("show");
                                setTimeout(function () { location.reload(); }, 100);
                            },
                            error: hap.common.jsonError
                        });
                    }
                    return false;
                });
                $("#edit-cancel").click(function () {
                    $("#contextbar").addClass("show");
                    $("#sidebaredit").removeClass("show");
                    $("#contextbar > #edit").trigger("click");
                    return false;
                })
                $("#del-link").click(function () {
                    if (confirm("Are you sure you want to delete this link?"))
                        $.ajax({
                            type: 'POST',
                            url: 'API/Setup/RemoveLink',
                            data: '{ "group": "' + $("#HomeButtonsHeader .active a").html() + '", "name": "' + "" + '" }',
                            contentType: 'application/json',
                            dataType: 'json',
                            success: function (data) { location.reload(); },
                            error: hap.common.jsonError
                        });
                    return false;
                });
            </script>
            <% } %>
            <script type="text/javascript">
                var scrollpos = sliding = startClientX = startPixelOffset = pixelOffset = maxscroll = 0;
                hap.load = hap.loadtypes.help;
                $(document).ready(function () {
                    $("button").button();
                    maxscroll = $("#HomeButtons > div.no-scroll").length == 0 ? ($("#HomeButtons > div").length - 1) : ($("#HomeButtons > div.no-scroll").first().index() - 1);
                    $("#HomeButtons").mousewheel(function(event, delta) {
                        if (delta > 0) scrollpos--;
                        else scrollpos++;
                        scrollpos = Math.min(Math.max(scrollpos, 0), maxscroll);
                        location.href = $($("#HomeButtonsHeader h1")[scrollpos]).children("a")[0].href;
                        return false;
                    }).on("swipeleft", function (e) {
                        scrollpos++;
                        scrollpos = Math.min(Math.max(scrollpos, 0), maxscroll);
                        location.href = $($("#HomeButtonsHeader h1")[scrollpos]).children("a")[0].href;
                        return false;
                    }).on("swiperight", function (e) {
                        scrollpos--;
                        scrollpos = Math.min(Math.max(scrollpos, 0), maxscroll);
                        location.href = $($("#HomeButtonsHeader h1")[scrollpos]).children("a")[0].href;
                        return false;
                    });
                    $("#HomeButtons").css("width", (($("#HomeButtons > div").length * $("#HomeButtonsOutter").width()) + 200) + "px");
                    if ($("#HomeButtons > div").length == 1) $("#rightscoll, #leftscroll").hide();
                    $("#HomeButtons .panel").css("width", $("#HomeButtonsOutter").width() + "px");
                    $("#leftscroll").click(function () {
                        scrollpos--;
                        scrollpos = Math.min(Math.max(scrollpos, 0), maxscroll);
                        location.href = $($("#HomeButtonsHeader h1")[scrollpos]).children("a")[0].href;
                        return false;
                    });
                    $("#rightscoll").click(function () {
                        scrollpos++;
                        scrollpos = Math.min(Math.max(scrollpos, 0), maxscroll);
                        location.href = $($("#HomeButtonsHeader h1")[scrollpos]).children("a")[0].href;
                        return false;
                    });
                    $(window).resize(function() {
                        $("#HomeButtons").css("width", (($("#HomeButtons > div").length * $("#HomeButtonsOutter").width()) + 200) + "px");
                        $("#HomeButtons .panel").css("width", $("#HomeButtonsOutter").width() + "px");
                        $("#HomeButtonsOutter").scrollLeft(scrollpos * ($("#HomeButtonsOutter").width() - 20) + (scrollpos * 20));
                        $("#rightscoll, #leftscroll").css("height", $("#HomeButtonsOutter").height() + "px").css("line-height", $("#HomeButtonsOutter").height() + "px");
                    });
                    $("#HomeButtonsOutter").animate({ scrollLeft: (scrollpos * ($("#HomeButtonsOutter").width() - 20) + (scrollpos * 20)) });
                    $("#HomeButtonsHeader h1:first").addClass("active");
                    $('input[type=submit]').button();
                    $("#rightscoll, #leftscroll").css("height", $("#HomeButtonsOutter").height() + "px").css("line-height", $("#HomeButtonsOutter").height() + "px");
                    $(window).trigger("hashchange");
                    if (hap.admin) {
                        $(document).on("contextmenu", function (e) {
                            if ($("#contextbar").hasClass("show")) $("#contextbar").removeClass("show");
                            else $("#contextbar").addClass("show");
                            return false;
                        });
                        $("#contextbar > #edit").click(function () {
                            var e = $($("#HomeButtonsHeader h1")[scrollpos]).children("a")[0].href.split('#')[1].substr(($($("#HomeButtonsHeader h1")[scrollpos]).children("a")[0].href.split('#')[1].substr(0, 1) == '-' ? 1 : 0), 1).toUpperCase() + $($("#HomeButtonsHeader h1")[scrollpos]).children("a")[0].href.split('#')[1].substr(($($("#HomeButtonsHeader h1")[scrollpos]).children("a")[0].href.split('#')[1].substr(0, 1) == '-' ? 2 : 1));
                            if ($("#contextbar").hasClass("editmode")) {
                                $("#contextbar").removeClass("editmode"); $("#contextbar").removeClass("selectactive"); $(".selected").removeClass("selected");
                                $("#" + e).sortable("destroy");
                            }
                            else {
                                $("#" + e).sortable({
                                    update: function (e, u) {
                                        var s = "";
                                        for (var y = 0; y < $(this).sortable('toArray').toString().substr(1).split(/,/gi).length; y++)
                                        {
                                            var tile = null;
                                            for (var i = 0; i < hap.livetiles.Tiles.length; i++)
                                                if (hap.livetiles.Tiles[i].id == $(this).sortable('toArray').toString().substr(1).split(/,/gi)[y]) { tile = hap.livetiles.Tiles[i]; break; }
                                            s += ",link";
                                            s += tile.data.Name;
                                        }
                                        $.ajax({
                                            type: 'POST',
                                            url: 'API/Setup/UpdateLinkOrder',
                                            data: '{"group": "linkgroup' + $("#HomeButtonsHeader .active a").html() + '", "links": "' + s.substr(1) + '"}',
                                            contentType: 'application/json',
                                            dataType: 'json',
                                            error: hap.common.jsonError
                                        });
                                    }
                                });
                                $("#contextbar").addClass("editmode");
                            }
                            return false;
                        });
                    }
                });
                $(window).hashchange(function () {
                    if (window.location.href.split('#')[1] != "" && window.location.href.split('#')[1]) {
                        var e = window.location.href.split('#')[1].substr((window.location.href.split('#')[1].substr(0, 1) == '-' ? 1 : 0), 1).toUpperCase() + window.location.href.split('#')[1].substr((window.location.href.split('#')[1].substr(0, 1) == '-' ? 2 : 1));
                        if ($("#" + e)[0]) {
                            scrollpos = $("#panel-" + e).index();
                            $("#HomeButtonsOutter").animate({ scrollLeft: (scrollpos * ($("#HomeButtonsOutter").width() - 20) + (scrollpos * 20)) });
                            $("#HomeButtonsHeader h1").removeClass("active");
                            $("#header-" + e).addClass("active");
                            if ($(".editmode").length > 0) $("#contextbar > #edit").trigger("click");
                        }
                    }
                });
            </script>
        </div>
    </div>
</asp:Content>
