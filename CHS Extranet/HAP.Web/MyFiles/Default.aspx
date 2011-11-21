<%@ Page Title="" Language="C#" MasterPageFile="~/masterpage.master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="HAP.Web.MyFiles.Default" %>
<asp:Content ContentPlaceHolderID="head" runat="server">
	<script src="../Scripts/jquery-1.6.2.min.js" type="text/javascript"></script>
	<script src="../Scripts/jquery-ui-1.8.16.custom.min.js" type="text/javascript"></script>
	<script src="../Scripts/jquery.ba-hashchange.min.js" type="text/javascript"></script>
	<script src="../Scripts/jquery.dynatree.js" type="text/javascript"></script>
	<script src="../Scripts/jquery.dataTables.js" type="text/javascript"></script>
	<script src="../Scripts/jquery.contextmenu.js" type="text/javascript"></script>
	<link href="../style/ui.dynatree.css" rel="stylesheet" type="text/css" />
	<link href="../style/MyFiles.css" rel="stylesheet" type="text/css" />
	<meta name="DownloadOptions" content="noopen" />
</asp:Content>
<asp:Content ContentPlaceHolderID="body" runat="server">
	<div style="overflow: hidden; clear: both; position: relative; height: 120px">
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
	<div id="toolbar" style="padding: 4px; margin-bottom: 4px;" class="ui-widget-header">
		<div style="float: right;">
			<span style="color: #fff;" id="search">Search:
			<input type="text" id="filter" />
			</span>
			<button class="dropdown" id="view">View</button>
		</div>
		<button class="dropdown">Organise</button> <button id="backup">...</button> <button>Open</button> <button>New Folder</button>
	</div>
	<div id="Views" class="tile-border-color">
		<button>Tiles</button>
		<button>Details</button>
	</div>
	<div id="Tree" class="tile-border-color">
	</div>
	<div id="MyFiles" class="tiles">
	</div>
	<div id="MyFilesTable">
	<table id="MyFiles-Table">
		<thead>
			<tr><th>Name</th><th>Type</th><th width="105px">Date modified</th><th>Size</th></tr>
		</thead>
		<tbody></tbody>
	</table>
	</div>
	<script type="text/javascript">
		var items = new Array();
		var showView = 0;
		var viewMode = 0;
		var keys = { shift: false, ctrl: false };
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
			    this.Id = (this.Data.Name + this.Data.Extension).replace(/ /g, "_").replace(/\\/g, "-").replace(/\./g, "");
			    var label = this.Data.Name;

			    var h = '<a id="' + this.Id + '" ';
			    if (this.Data.Type == 'Directory') h += 'class="Folder Selectable" ';
			    else h += 'class="Selectable" ';
			    h += 'href="' + (this.Data.Path.match(/\.\./i) ? this.Data.Path.replace(/\\/g, "/") : '#' + this.Data.Path) + '"><img class="icon" src="' + this.Data.Icon + '" alt="" /><span class="label">' + label + '</span><span class="type">';
			    if (this.Data.Type == 'Directory') h += 'File Folder';
			    else h += this.Data.Type + '<br />' + this.Data.Size;
			    h += '</span></a>';
			    $("#MyFiles").append(h);
			    $("#" + this.Id).bind("click", this.Click);
			    $("#" + this.Id).contextMenu('context-menu-' + this.Id, {
			        'Delete': {
			            click: function (element) {  // element is the jquery obj clicked on when context menu launched
			                alert('Menu item 1 clicked');
			            }
			        },
			        'Rename': {
			            click: function (element) { alert('second clicked'); }
			        },
			        'Properties': {
			            click: function (element) {
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
			                return false;
			            }
			        }
			    },
				{
				    showMenu: function (element) {
				        var item = null;
				        for (var x = 0; x < items.length; x++) if (items[x].Id == $(element).attr("id")) item = items[x];
				        item.Selected = true;
				        item.Refresh();
				    },
				    hideMenu: function (element) { }
				});
			};
			this.RenderTable = function () {
				this.Id = (this.Data.Name + this.Data.Extension).replace(/ /g, "_").replace(/\\/g, "-").replace(/\./g, "");
				var label = this.Data.Name;

				var h = '<tr><td><a id="' + this.Id + '" ';
				if (this.Data.Type == 'Directory') h += 'class="Folder Selectable" ';
				else h += 'class="Selectable" ';
				h += 'href="' + (this.Data.Path.match(/\.\./i) ? this.Data.Path.replace(/\\/g, "/") : '#' + this.Data.Path) + '"><img class="icon" src="' + this.Data.Icon + '" alt="" /><span class="label">' + label + '</span></a></td>';
				h += '<td>';
				if (this.Data.Type == 'Directory') h += 'File Folder';
				else h += this.Data.Type;
				h += '</td>';
				h += '<td>' + this.Data.ModifiedTime + '</td><td>';
				if (this.Data.Type == 'Directory') h += '&nbsp;';
				else h += this.Data.Size;
				h += '</td></tr>';
				$("#MyFiles-Table tbody").append(h);
				$("#" + this.Id).bind("click", this.Click);
			};
			this.Refresh = function () {
				if (viewMode == 1) {
					var label = this.Data.Name;
					var h = '<td><a id="' + this.Id + '" ';
					if (this.Data.Type == 'Directory') h += 'class="Folder Selectable" ';
					else h += 'class="Selectable" ';
					h += 'href="' + (this.Data.Path.match(/\.\./i) ? this.Data.Path.replace(/\\/g, "/") : '#' + this.Data.Path) + '"><img class="icon" src="' + this.Data.Icon + '" alt="" /><span class="label">' + label + '</span></a></td>';
					h += '<td>';
					if (this.Data.Type == 'Directory') h += 'File Folder';
					else h += this.Data.Type;
					h += '</td>';
					h += '<td>' + this.Data.ModifiedTime + '</td><td>';
					if (this.Data.Type == 'Directory') h += '&nbsp;';
					else h += this.Data.Size;
					h += '</td>';
					$("#" + this.Id).parent().parent().html(h);
					$("#" + this.Id).click(this.Click);
					$("#" + this.Id).click(function () { return false; });

					if (this.Selected) $("#" + this.Id).addClass("Selected");
					else $("#" + this.Id).removeClass("Selected");
					if (this.Show) $("#" + this.Id).parent().parent().removeAttr("style");
					else $("#" + this.Id).parent().parent().css("display", "none");
				}
				else {
					$("#" + this.Id).attr("href", (this.Data.Path.match(/\.\./i) ? this.Data.Path.replace(/\\/g, "/") : '#' + this.Data.Path));
					if (this.Selected) $("#" + this.Id).addClass("Selected");
					else $("#" + this.Id).removeClass("Selected");
					var label = this.Data.Name;
					var h = '<img class="icon" src="' + this.Data.Icon + '" alt="" /><span class="label">' + label + '</span><span class="type">';
					if (this.Data.Type == 'Directory') h += 'File Folder';
					else h += this.Data.Type + '<br />' + this.Data.Size;
					h += '</span>';

					$("#" + this.Id).html(h);
					if (this.Show) $("#" + this.Id).removeAttr("style");
					else $("#" + this.Id).css("display", "none");
				}
			};
			this.Selected = false;
			this.ClickTimer = null;
			this.Click = function (e) {
				e.preventDefault();
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
						$("MyFiles").css("display", "block");
						$("MyFilesTable").css("display", "none");
						$("#MyFiles").html("");
						$("#MyFiles-Table tbody").html("");
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
						if (table != null) $("#MyFiles-Table").dataTable().fnDestroy();
						$("#MyFiles-Table tbody").html("");
						if (viewMode == 1) {
							$("#MyFiles").css("display", "none");
							$("#MyFilesTable").css("display", "block");
						}
						else {
							$("#MyFiles").css("display", "block");
							$("#MyFilesTable").css("display", "none");
						}
						for (var i = 0; i < data.length; i++)
							items.push(new Item(data[i]));
						for (var i = 0; i < items.length; i++)
							if (viewMode == 0) items[i].Render();
							else items[i].RenderTable();
						if (viewMode == 1) {
							$("#MyFiles-Table").dataTable({ "bJQueryUI": true, bPaginate: false, bLengthChange: false, bSort: false, bInfo: false, bFilter: false });
							if (table != null) $("#MyFiles-Table").dataTable().fnAdjustColumnSizing();
							table = $("#MyFiles-Table").dataTable();
						}
					}, error: OnError
				});
			}
		}
		function searchres(data) {
			var re = new RegExp("(" + $("#filter").val().replace(/\*/g, ")(.*)(").replace(/ /g, ")(.*)(").replace(/\(\)/g, "") + ")", "i");
			return data.match(re);
		}
		$(function () {
			$("#MyFilesTable").css("display", "none");
			$("#properties").dialog({ autoOpen: false });
			$("#Views").animate({ height: 'toggle' });
			$("#Tree").dynatree({ imagePath: "../images/setup/", selectMode: 1, noLink: false, minExpandLevel: 1, children: [{ title: "My Drives", href: "#", isFolder: true, isLazy: true}], fx: { height: "toggle", duration: 200 },
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
					if (dtnode.data.href != "#")
						$(nodeSpan).children("a").attr("href", dtnode.data.href);
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
			$("#Views").css("top", $("#view").position().top + $("#view").parent().height() + 2);
			$("#Views").css("left", $("#view").position().left - ($("#Views").width() - $("#view").width()));
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
			$("#hapContent").click(function () {
				if (showView == 2) { $("#Views").animate({ height: 'toggle' }); showView = 0; }
				else if (showView == 1) showView = 2;
			});
			$("#Views button").click(function () {
				$("#MyFiles").html("");
				if (table != null) $("#MyFiles-Table").dataTable().fnDestroy();
				$("#MyFiles-Table tbody").html("");
				if ($(this).text() == "Details") {
					viewMode = 1;
					$("#MyFiles").css("display", "none");
					$("#MyFilesTable").css("display", "block");
				}
				else {
					viewMode = 0;
					$("#MyFiles").css("display", "block");
					$("#MyFilesTable").css("display", "none");
				}
				for (var i = 0; i < items.length; i++)
					if (viewMode == 0) items[i].Render();
					else items[i].RenderTable();
				if (viewMode == 1) {
					$("#MyFiles-Table").dataTable({ "bJQueryUI": true, bPaginate: false, bLengthChange: false, bSort: false, bInfo: false, bFilter: false });
					if (table != null) $("#MyFiles-Table").dataTable().fnAdjustColumnSizing();
					table = $("#MyFiles-Table").dataTable();
				}
			});
		});
		$(document).bind('keydown', function (e) { keys.shift = (e.keyCode == 16); keys.ctrl = (e.keyCode == 17); });
		$(document).bind('keyup', function (e) { keys.shift = keys.ctrl = false; });
	</script>
</asp:Content>
