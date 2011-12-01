<%@ Page Title="" Language="C#" MasterPageFile="~/masterpage.master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="HAP.Web.MyFiles.Default" %>
<asp:Content ContentPlaceHolderID="head" runat="server">
	<script src="../Scripts/jquery-1.7.1.min.js" type="text/javascript"></script>
	<script src="../Scripts/jquery-ui-1.8.16.custom.min.js" type="text/javascript"></script>
	<script src="../Scripts/jquery.ba-hashchange.min.js" type="text/javascript"></script>
	<script src="../Scripts/jquery.dynatree.js" type="text/javascript"></script>
	<script src="../Scripts/jquery.contextmenu.js" type="text/javascript"></script>
	<link href="../style/ui.dynatree.css" rel="stylesheet" type="text/css" />
	<link href="../style/MyFiles.css" rel="stylesheet" type="text/css" />
	<meta name="DownloadOptions" content="noopen" />
</asp:Content>
<asp:Content ContentPlaceHolderID="body" runat="server">
	<div style="overflow: hidden; clear: both; position: relative; height: 120px" id="myfilesheader">
		<div class="tiles" style="position: absolute; left: 0; margin-top: 45px;">
			<a class="button" href="../">Home Access Plus+ Home</a>
		</div>
		<div class="tiles" style="float: right; text-align: right; margin-top: 45px;">
		</div>
		<div style="text-align: center;">
			<img src="../images/myfiles.png" alt="My Files" />
		</div>
	</div>
	<div id="properties" title="Properties">
		<div id="propcont">Loading...</div>
	</div>
	<div class="contextMenu" id="contextMenu">
	  <ul>
		<li id="con-properties">Properties</li>
	  </ul>
	</div>
	<div id="myfilescontent">
	<div id="toolbar" style="padding: 4px; margin-bottom: 4px;" class="ui-widget-header">
		<div style="float: right;">
			<span style="color: #fff;" id="search">Search:
			<input type="text" id="filter" />
			</span>
			<button class="dropdown" id="view">View</button>
		</div>
		<button class="dropdown">Organise</button> <button id="backup">...</button> <button>New Folder</button>
	</div>
	<div id="Views" class="tile-border-color">
		<button>Tiles</button>
		<button>Small Icons</button>
		<button>Medium Icons</button>
		<button>Large Icons</button>
		<button>Details</button>
	</div>
	<div id="Tree" class="tile-border-color">
	</div>
	<div id="MyFilesHeaddings">
		<span class="name">Name</span><span class="type">Type</span><span class="extension">Extension</span><span class="size">Size</span></div>
	<div id="MyFiles" class="tiles">
	</div>
	</div>
	<script type="text/javascript">
		var items = new Array();
		var showView = 0;
		var viewMode = 0;
		var keys = { shift: false, ctrl: false };
		var lazytimer = null;
		var temp = null;
		var table = null;
		var curpath = null;
		$(window).hashchange(function () {
			$("#filter").val("");
			if (window.location.href.split('#')[1] != "" && window.location.href.split('#')[1]) {
				curpath = window.location.href.split("#")[1];
				if ($("#backup").width() < 23) {
					$("#backup").removeAttr("style");
					$("#search").animate({ opacity: 1.0 });
					$("#backup").animate({ width: 25, opacity: 1.0 });
				}
			}
			else {
				curpath = null;
				if ($("#backup").width() != 0) {
					$("#backup").animate({ width: 0, opacity: 0.0 }, 200, function () {
						$("#backup").css("display", "none");
					});
					$("#search").animate({ opacity: 0.0 });
				}
			}
			Load();
		});
		function OnError(xhr, ajaxOptions, thrownError) {
			console.log(thrownError);
			console.log(ajaxOptions);
			console.log(xhr);
			alert(thrownError);
		}
		function Drive(data) {
			this.Data = data;
			this.Id = "";
			this.Render = function () {
				this.Id = this.Data.Path.substr(0, 1);
				$("#MyFiles").append('<a id="' + this.Id + '" href="#' + this.Data.Path + '" class="Drive"><span class="icon">' + this.Data.Path.substr(0, 1) + '</span><span class="label">' + this.Data.Name + '</span><span class="progress"><label>' + this.Data.Space + '%</label><i style="width: ' + this.Data.Space + '%"></i></span></a>');
			};
			this.Refresh = function () {
				$("#" + this.Id).attr("href", "#" + this.Data.Path);
				$("#" + this.Id).html('<span class="icon">' + this.Data.Path.substr(0, 1) + '</span><span class="label">' + this.Data.Name + '</span><span class="progress"><label>' + this.Data.Space + '%</label><i style="width: ' + this.Data.Space + '%"></i></span>');
				$('#nav-' + this.Id + ' > a').attr("href", '#' + this.Data.Path);
				$('#nav-' + this.Id + ' > a > span').html(this.Data.Name);
			};
		}
		function SelectedItems() {
			var val = new Array();
			for (var i = 0; i < items.length; i++)
				if (items[i].Selected) val.push(items[i]);
			return val;
		}
		function Item(data) {
			this.Data = data;
			this.Id = "";
			this.Show = true;
			this.Clicks = 0;
			this.Render = function () {
			    this.Id = (this.Data.Name + this.Data.Extension).replace(/[\\'\. \[\]\(\)\-]/g, "_");
			    var label = this.Data.Name;

			    var h = '<a id="' + this.Id + '" title="' + this.Data.Name + '" ';
			    if (this.Data.Type == 'Directory') h += 'class="Folder Selectable" ';
			    else h += 'class="Selectable" ';
			    h += 'href="' + (this.Data.Path.match(/\.\./i) ? this.Data.Path.replace(/\\/g, "/") : '#' + this.Data.Path) + '"><img class="icon" src="' + this.Data.Icon + '" alt="" /><span class="label">' + label + '</span><span class="type">';
			    if (this.Data.Type == 'Directory') h += 'File Folder';
			    else h += this.Data.Type + '</span><span class="extension">' + this.Data.Extension + '</span><span class="size">' + this.Data.Size;
			    h += '</span></a>';
			    $("#MyFiles").append(h);
			    $("#" + this.Id).draggable({ helper: function () { return $('<div id="dragobject"><img /><span></span></div>'); }, start: function (event, ui) {
			        var item = null;
			        for (var x = 0; x < items.length; x++) if (items[x].Id == $(this).attr("id")) item = items[x];
			        if (!item.Selected) for (var x = 0; x < items.length; x++) if (items[x].Selected) { items[x].Selected = false; items[x].Refresh(); }
			        item.Selected = true;
			        item.Refresh();
			        $("#dragobject").show();
			        $("#dragobject img").attr("src", item.Data.Icon);
			        $("#dragobject span").text("");
			        $("#dragobject span").hide();
			    }
			    });
			    if (this.Data.Type == 'Directory') $("#" + this.Id).droppable({ accept: '.Selectable', activeClass: 'droppable-active', hoverClass: 'droppable-hover', drop: function (ev, ui) {

			    }, over: function (event, ui) {
			        var item = null;
			        for (var x = 0; x < items.length; x++) if (items[x].Id == $(this).attr("id")) item = items[x];
			        $("#dragobject span").text(((keys.ctrl) ? "Copy" : "Move") + " To " + item.Data.Name);
			        $("#dragobject span").show();
			    }, out: function (event, ui) {
			        $("#dragobject span").text("");
			        $("#dragobject span").hide();
			    }
			    });
			    $("#" + this.Id).bind("click", this.Click);
			    $("#" + this.Id).contextMenu('contextMenu', {
			        onContextMenu: function (e) {
			            var element = $(e.target);
			            if (!element.is("a")) element = element.parent("a");
			            var item = null;
			            for (var x = 0; x < items.length; x++) if (items[x].Id == element.attr("id")) item = items[x];
			            if (!item.Selected) for (var x = 0; x < items.length; x++) if (items[x].Selected) { items[x].Selected = false; items[x].Refresh(); }
			            item.Selected = true;
			            item.Refresh();
			            return true;
			        },
			        onShowMenu: function (e, menu) {
			            if (SelectedItems().length > 1) $("#con-properties", menu).remove();
			            return menu;
			        },
			        bindings: {
			            'con-properties': function (t) {
			                if (SelectedItems().length > 1) { alert("This only works on 1 item"); return false; }
			                $("#properties").dialog({ autoOpen: true, buttons: {
			                    "OK": function () {
			                        $(this).dialog("close");
			                        $("#propcont").html("Loading...");
			                    }
			                }
			                });
			                $.ajax({
			                    type: 'GET',
			                    url: '<%=ResolveUrl("~/api/MyFiles/Properties/")%>' + SelectedItems()[0].Data.Path.replace(/\\/gi, "/").replace(/\.\.\/Download\//gi, ""),
			                    dataType: 'json',
			                    contentType: 'application/json',
			                    success: function (data) {
			                        var s = '<div><img src="' + data.Icon + '" alt="" style="width: 32px; float: left; margin-right: 40px;" />' + data.Name + '</div>';
			                        s += '<hr style="height: 1px; border-width: 1px" />';
			                        if (data.Type == "File Folder") {
			                            s += '<div><label>Type: </label>' + data.Type + '</div>';
			                            s += '<div><label>Location: </label>' + data.Location + '</div>';
			                            s += '<div><label>Size: </label>' + data.Size + '</div>';
			                            s += '<div><label>Contains: </label>' + data.Contents + '</div>';
			                            s += '<hr style="height: 1px; border-width: 1px" />';
			                            s += '<div><label>Created: </label>' + data.DateCreated + '</div>';
			                        } else {
			                            s += '<div><label>Type of file: </label>' + data.Type + ' (' + data.Extension + ')</div>';
			                            s += '<hr style="height: 1px; border-width: 1px" />';
			                            s += '<div><label>Location: </label>' + data.Location + '</div>';
			                            s += '<div><label>Size: </label>' + data.Size + '</div>';
			                            s += '<hr style="height: 1px; border-width: 1px" />';
			                            s += '<div><label>Created: </label>' + data.DateCreated + '</div>';
			                            s += '<div><label>Modified: </label>' + data.DateModified + '</div>';
			                            s += '<div><label>Accessed: </label>' + data.DateAccessed + '</div>';
			                        }
			                        $("#propcont").html(s);
			                    }, error: OnError
			                });
			            }
			        }
			    });
			};
			this.Refresh = function () {
			    $("#" + this.Id).attr("href", (this.Data.Path.match(/\.\./i) ? this.Data.Path.replace(/\\/g, "/") : '#' + this.Data.Path));
			    $("#" + this.Id).attr("title", this.Data.Name);
			    if (this.Selected) $("#" + this.Id).addClass("Selected");
			    else $("#" + this.Id).removeClass("Selected");
			    var label = this.Data.Name;
			    var h = '<img class="icon" src="' + this.Data.Icon + '" alt="" /><span class="label">' + label + '</span><span class="type">';
			    if (this.Data.Type == 'Directory') h += 'File Folder';
			    else h += this.Data.Type + '</span><span class="extension">' + this.Data.Extension + '</span><span class="size">' + this.Data.Size;
			    h += '</span>';

			    $("#" + this.Id).html(h);
			    if (this.Show) $("#" + this.Id).removeAttr("style");
			    else $("#" + this.Id).css("display", "none");
			};
			this.Selected = false;
			this.ClickTimer = null;
			this.Click = function (e) {
				e.preventDefault();
				$('#jqContextMenu').css("display", "none");
				$('#jqContextMenuShadow').css("display", "none");
				var item = null;
				for (var x = 0; x < items.length; x++) if (items[x].Id == $(this).attr("id")) item = items[x];
				clearTimeout(item.ClickTimer);
				item.Clicks++;
				item.ClickTimer = setTimeout("for (var x = 0; x < items.length; x++) if (items[x].Id == '" + item.Id + "') items[x].Clicks = 0;", 300);
				if (item.Clicks === 1) {
					var i = -1, z = -1;
					for (var x = 0; x < items.length; x++) {
						if (items[x].Id == $(this).attr("id")) { z = x; }
						else if (items[x].Selected && keys.shift) { if (i == -1) i = x; }
						else if (items[x].Selected && !keys.ctrl) { items[x].Selected = false; items[x].Refresh(); }
					}
					item.Selected = !item.Selected;
					item.ClickCount = 1;
					if (i != -1 && z != -1 && keys.shift) {
						if (i > z) { var ti = i; i = z; z = ti; }
						for (var x = i; x < z; x++) {
							items[x].Selected = true;
							items[x].Refresh();
						}
					}
					item.Refresh();
				} else {
					if (item.Data.Type != 'Directory') alert("You are about to download this file, if you wish to edit this file, please remember to\nSave it to your computer, and upload it back once you have finished!");
					var item = null;
					for (var x = 0; x < items.length; x++)
						if (items[x].Id == $(this).attr("id")) { item = items[x]; break; }
					window.location.href = (item.Data.Path.match(/\.\./i) ? item.Data.Path.replace(/\\/g, "/") : '#' + item.Data.Path);
				}
				return false;
			};
		}
		function Load() {
			$(".context-menu").remove();
			if (curpath == null) {
			    $.ajax({
			        type: 'GET',
			        url: '<%=ResolveUrl("~/api/MyFiles/Drives")%>',
			        dataType: 'json',
			        contentType: 'application/json;',
			        success: function (data) {
			            items = new Array();
			            $("MyFiles").removeAttr("class");
			            $("#MyFiles").html("");
			            for (var i = 0; i < data.length; i++)
			                items.push(new Drive(data[i]));
			            for (var i = 0; i < items.length; i++)
			                items[i].Render();
			        }, error: OnError
			    });
			} else {
				$.ajax({
					type: 'GET',
					url: '<%=ResolveUrl("~/api/MyFiles/")%>' + curpath.replace(/\\/gi, "/"),
					dataType: 'json',
					contentType: 'application/json',
					success: function (data) {
						items = new Array();
						$("#MyFiles").html("");
						for (var i = 0; i < data.length; i++)
							items.push(new Item(data[i]));
						for (var i = 0; i < items.length; i++) items[i].Render();
					}, error: OnError
				});
			}
		}
		function searchres(data) {
			var re = new RegExp("(" + $("#filter").val().replace(/\*/g, ")(.*)(").replace(/ /g, ")(.*)(").replace(/\(\)/g, "") + ")", "i");
			return data.match(re);
		}
		$(function () {
			$("#properties").dialog({ autoOpen: false });
			$("#Views").animate({ height: 'toggle' });
			$("#Tree").dynatree({ imagePath: "../images/setup/", selectMode: 1, minExpandLevel: 1, noLink: false, children: [{ title: "My Drives", href: "#", isFolder: true, isLazy: true}], fx: { height: "toggle", duration: 200 },
				onLazyRead: function (node) {
					if (node.data.href == "#") {
						$.ajax({
							type: 'GET',
							url: '<%=ResolveUrl("~/api/MyFiles/Drives")%>',
							dataType: 'json',
							contentType: 'application/json',
							success: function (data) {
								res = [];
								for (var i = 0; i < data.length; i++)
									res.push({ title: data[i].Name, href: "#" + data[i].Path, isFolder: true, isLazy: true, noLink: false, key: data[i].Path });
								node.setLazyNodeStatus(DTNodeStatus_Ok);
								node.addChild(res);
							}, error: OnError
						});
					} else {
						$.ajax({
							type: 'GET',
							url: '<%=ResolveUrl("~/api/MyFiles/")%>' + node.data.href.substr(1).replace(/\\/g, "/"),
							dataType: 'json',
							contentType: 'application/json',
							success: function (data) {
								res = [];
								for (var i = 0; i < data.length; i++)
									if (data[i].Type == "Directory") {
										res.push({ title: data[i].Name, href: "#" + data[i].Path, isFolder: true, isLazy: true, noLink: false, key: data[i].Path });
									}
								node.setLazyNodeStatus(DTNodeStatus_Ok);
								node.addChild(res);
							}, error: OnError
						});
					}
				},
				onRender: function (dtnode, nodeSpan) {
					if (dtnode.data.href != "#") {
						$(nodeSpan).children("a").attr("href", dtnode.data.href);
						$(nodeSpan).children("a").bind("click", function () { window.location.href = $(this).attr("href"); });
						$(nodeSpan).children("a").droppable({ accept: '.Selectable', activeClass: 'droppable-active', hoverClass: 'droppable-hover', drop: function (ev, ui) {

						}, over: function (event, ui) {
							$("#dragobject span").text(((keys.ctrl) ? "Copy" : "Move") + " To " + $(this).text());
							$("#dragobject span").show();
							temp = $(this);
							if (lazytimer == null) lazytimer = setTimeout(function () { $("#Tree").dynatree("getTree").getNodeByKey(temp.attr("href").substr(1)).toggleExpand(); clearTimeout(lazytimer); lazytimer = null; }, 1000);
						}, out: function (event, ui) {
							$("#dragobject span").text("");
							$("#dragobject span").hide();
							if (lazytimer != null) {
								clearTimeout(lazytimer);
								lazytimer = null;
								temp = null;
							}
						}
						});
					}
				}
			});
			$("#filter").val("");
			if (window.location.href.split('#')[1] != "" && window.location.href.split('#')[1]) {
				curpath = window.location.href.split("#")[1];
				if ($("#backup").width() < 23) {
					$("#backup").removeAttr("style");
					$("#search").animate({ opacity: 1.0 });
					$("#backup").animate({ width: 25, opacity: 1.0 });
				}
			}
			else {
				curpath = null;
				if ($("#backup").width() != 0) {
					$("#backup").animate({ width: 0, opacity: 0.0 }, 200, function () {
						$("#backup").css("display", "none");
					});
					$("#search").animate({ opacity: 0.0 });
				}
			}
			Load();
			$("button").button();
			$("button").click(function () { return false; });
			$("button.dropdown").button({ icons: { secondary: "ui-icon-carat-1-s"} });
			$("#backup").click(function () { history.go(-1); return false; });
			$(".button").button();
			$("#view").click(function () {
				if (showView == 0) {
					showView = 1;
					$("#Views").animate({ height: 'toggle' });
				}
				return false;
			});
			$("#filter").keyup(function () {
				if ($("#filter").val().length == 0) for (var x = 0; x < items.length; x++) { items[x].Show = true; items[x].Refresh(); }
				else {
					for (var x = 0; x < items.length; x++) { if (searchres(items[x].Data.Name + items[x].Data.Extension)) items[x].Show = true; else items[x].Show = false; items[x].Refresh(); }
				}
			});
			$(document).click(function () {
				if (showView == 1) { $("#Views").animate({ height: 'toggle' }); showView = 0; }
				//else if (showView == 1) showView = 2;
			});
			$("#Views button").click(function () {
				if ($(this).text() == "Details") {
					viewMode = 1;
					$("#MyFiles").addClass("details");
					$("#MyFiles").removeClass("small");
					$("#MyFiles").removeClass("medium");
					$("#MyFiles").removeClass("large");
					$("#MyFilesHeaddings").css("display", "block");
					$("#MyFiles").css("padding-top", $("#toolbar").height() + 34);
					$("#MyFilesHeaddings .name").css("width", $("#MyFiles > a .label").width() + $("#MyFiles > a img").width() + 4);
					$("#MyFilesHeaddings .type").css("width", $("#MyFiles > a .type").width() + 2);
					$("#MyFilesHeaddings .extension").css("width", $("#MyFiles > a .extension").width() + 2);
					$("#MyFilesHeaddings .size").css("width", $("#MyFiles > a .size").width() + 2);
				}
				else if ($(this).text() == "Small Icons") {
					viewMode = 2;
					$("#MyFiles").addClass("small");
					$("#MyFiles").removeClass("details");
					$("#MyFiles").removeClass("medium");
					$("#MyFiles").removeClass("large");
					$("#MyFilesHeaddings").css("display", "none");
					$("#MyFiles").css("padding-top", $("#toolbar").height() + 10);
				}
	            else if ($(this).text() == "Medium Icons") {
	                viewMode = 2;
	                $("#MyFiles").addClass("medium");
	                $("#MyFiles").removeClass("small");
	                $("#MyFiles").removeClass("large");
	                $("#MyFiles").removeClass("details");
	                $("#MyFilesHeaddings").css("display", "none");
	                $("#MyFiles").css("padding-top", $("#toolbar").height() + 10);
	            }
	            else if ($(this).text() == "Large Icons") {
	                viewMode = 2;
	                $("#MyFiles").addClass("large");
	                $("#MyFiles").removeClass("medium");
	                $("#MyFiles").removeClass("small");
	                $("#MyFiles").removeClass("details");
	                $("#MyFilesHeaddings").css("display", "none");
	                $("#MyFiles").css("padding-top", $("#toolbar").height() + 10);
	            }
				else {
					viewMode = 0;
					$("#MyFiles").removeClass("details");
					$("#MyFiles").removeClass("small");
					$("#MyFiles").removeClass("medium");
					$("#MyFiles").removeClass("large");
					$("#MyFilesHeaddings").css("display", "none");
					$("#MyFiles").css("padding-top", $("#toolbar").height() + 10);
				}
			});
			$("#toolbar").css("width", $("#myfilescontent").width() - 10);
			$("#toolbar").css("top", $("#myfilescontent").offset().top);
			$("#Tree").css("top", $("#myfilescontent").offset().top + $("#toolbar").height() + 10);
			$("#MyFiles").css("margin-left", $("#Tree").width() + 5);
			$("#MyFiles").css("padding-top", $("#toolbar").height() + 10);
			$("#Views").css("top", $("#view").offset().top + $("#view").parent().height());
			$("#Views").css("left", $("#view").offset().left - ($("#Views").width() - $("#view").width()) + 1);
			$("#MyFilesHeaddings").css("margin-left", $("#Tree").width() + 5);
			$("#MyFilesHeaddings").css("top", $("#myfilescontent").offset().top + $("#toolbar").height() + 10);
			$(window).scroll(function (event) {
				if ($(this).scrollTop() >= $("#myfilesheader").offset().top + $("#myfilesheader").height()) {
					$("#toolbar").css("position", "fixed"); $("#toolbar").css("top", 0);
					$("#Tree").css("position", "fixed"); $("#Tree").css("top", $("#toolbar").height() + 10);
					$("#MyFilesHeaddings").css("position", "fixed"); $("#MyFilesHeaddings").css("top", $("#toolbar").height() + 8);
					$("#Views").css("position", "fixed"); $("#Views").css("top", $("#toolbar").height() + 4);
				}
				else {
					$("#toolbar").css("position", "absolute"); $("#toolbar").css("top", $("#myfilesheader").offset().top + $("#myfilesheader").height());
					$("#Tree").css("position", "absolute"); $("#Tree").css("top", $("#myfilesheader").offset().top + $("#myfilesheader").height() + $("#toolbar").height() + 10);
					$("#Views").css("position", "absolute"); $("#Views").css("top", $("#myfilesheader").offset().top + $("#myfilesheader").height() + $("#toolbar").height() + 4);
					$("#MyFilesHeaddings").css("position", "absolute"); $("#MyFilesHeaddings").css("top", $("#myfilesheader").offset().top + $("#myfilesheader").height() + $("#toolbar").height() + 8);
				}
			});
		});
		$(document).bind('keydown', function (e) { keys.shift = (e.keyCode == 16); keys.ctrl = (e.keyCode == 17); });
		$(document).bind('keyup', function (e) { keys.shift = keys.ctrl = false; });
	</script>
</asp:Content>
