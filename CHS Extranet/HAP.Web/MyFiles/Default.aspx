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
	<input type="text" id="renamebox" />
	<div id="properties" title="Properties">
		<div id="propcont">Loading...</div>
	</div>
	<div id="preview" title="Preview" style="height: 500px; overflow: auto; width: 700px;">
		<div id="previewcont">Loading...</div>
	</div>
	<div id="progressstatus" title="Progress">
		<div class="progress"></div>
	</div>
	<div id="googlesignin" title="Sign into Google Docs">
		<div>Once you have signed into Google, HAP+ will upload the selected file to your Google Docs</div>
		<div>
			<label for="googleuser">Username: </label>
			<input type="text" id="googleuser" />
		</div>
		<div>
			<label for="googlepass">Password: </label>
			<input type="password" id="googlepass" />
		</div>
		<div class="progress"></div>
	</div>
	<div class="contextMenu" id="contextMenu">
	  <ul>
		<li id="con-open">Open</li>
		<li id="con-delete">Delete</li>
		<li id="con-rename">Rename</li>
		<li id="con-preview">Preview</li>
		<li id="con-properties">Properties</li>
		<li id="con-google">Send to Google Docs</li>
	  </ul>
	</div>
	<div id="uploaders" title="Upload">
		<input type="file" id="uploadedfiles" runat="server" multiple="multiple" />
		<asp:Button runat="server" style="display: none;" id="uploadbtn" 
			onclick="uploadbtn_Click" /><asp:HiddenField runat="server" id="p" />
	</div>
	<div id="uploadprogress" class="tile-border-color" style="border-width: 1px; border-style: solid; border-bottom: 0;">
		<div class="tile-color ui-widget-header">Upload Progress</div>
		<div id="progresses">
		</div>
	</div>
	<div id="myfilescontent">
	<div id="toolbar" style="padding: 4px; margin-bottom: 4px;" class="ui-widget-header">
		<div style="float: right;">
			<span style="color: #fff;" id="search">Search:
			<input type="text" id="filter" />
			</span>
			<button class="dropdown" id="view">View</button>
		</div>
		<button id="backup"></button><span id="newfolderspan"><input type="text" id="newfoldertext" style="margin-right: 6px;" /><button id="newfolder">New Folder</button></span> <button id="upload">Upload</button> <label id="uploadto" />
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
		var subdrop = false;
		var showView = 0;
		var viewMode = 0;
		var keys = { shift: false, ctrl: false };
		var lazytimer = null;
		var temp = null;
		var table = null;
		var uploads = new Array();
		var curitem = null;
		var curpath = null;
		$(window).hashchange(function () {
			$("#filter").val("");
			if (window.location.href.split('#')[1] != "" && window.location.href.split('#')[1]) {
				curpath = window.location.href.split("#")[1];
				if (typeof (window.FileReader) != 'undefined') $("#MyFiles").attr("dropzone", "copy<%=DropZoneAccepted %>");
				if (viewMode == 1) { $("#MyFiles").addClass("details"); $("#MyFilesHeaddings").show(); }
				else if (viewMode == 2) $("#MyFiles").addClass("small");
				else if (viewMode == 3) $("#MyFiles").addClass("medium");
				else if (viewMode == 4) $("#MyFiles").addClass("large");
			}
			else {
				curitem = curpath = null;
				$("#toolbar").slideUp();
				if (typeof (window.FileReader) != 'undefined') {
					$("#MyFiles").removeAttr("dropzone").attr("dropzone", "copy<%=DropZoneAccepted %>").unbind("dragover").unbind("dragleave").unbind("dragend");
				}
				$("#MyFiles").removeClass("details").removeClass("small").removeClass("medium").removeClass("large");
				$("#MyFilesHeaddings").hide();
			}
			Load();
		});
		function OnError(xhr, ajaxOptions, thrownError) {
			console.log(thrownError);
			console.log(ajaxOptions);
			console.log(xhr);
			alert(thrownError);
		}
		function Copy(index, target) {
			temp = { "index": index, "target": target };
			var a = '"' + SelectedItems()[index].Data.Path + '"';
			$.ajax({
				type: 'POST',
				url: '<%=ResolveUrl("~/api/MyFiles/Copy")%>',
				dataType: 'json',
				data: '{ "OldPath" : "' + SelectedItems()[index].Data.Path.replace(/\\/gi, '/') + '", "NewPath": "' + (target.replace(/\//gi, '\\') + '\\' + SelectedItems()[index].Data.Path.substr(SelectedItems()[index].Data.Path.lastIndexOf('\\'))).replace(/\\\\\\/gi, "\\").replace(/\\\\/gi, "\\").replace(/\\/gi, '/') + '" }',
				contentType: 'application/json',
				success: function (data) {
					temp.index++;
					$("#progressstatus").dialog("title", "Copying item " + (temp.index + 1) + " of " + SelectedItems().length + " items");
					$("#progressstatus .progress").progressbar({ value: (temp.index / SelectedItems().length) * 100 });
					if (temp.index < SelectedItems().length) Move(temp.index, temp.target);
					else { temp = null; Load(); setTimeout(function() { $("#progressstatus").dialog("close"); }, 500); }
				},
				error: function (xhr, ajaxOptions, thrownError) {
					console.log(thrownError);
					console.log(ajaxOptions);
					console.log(xhr);
					if (confirm("An Error has Occured While Copying " + SelectedItems()[temp.index].Data.Name + ", do you want to Continue?\n\nError Details:\n\n" + thrownError)) {
						temp.index++;
						$("#progressstatus").dialog("title", "Copying item " + (temp.index + 1) + " of " + SelectedItems().length + " items");
						$("#progressstatus .progress").progressbar({ value: (temp.index / SelectedItems().length) * 100 });
						if (temp.index < SelectedItems().length) Copy(temp.index, temp.target);
						else { temp = null; Load(); setTimeout(function() { $("#progressstatus").dialog("close"); }, 500); }
					} else { temp = null; Load(); setTimeout(function() { $("#progressstatus").dialog("close"); }, 500); }
				}
			});
		}
		function Move(index, target) {
			temp = { "index": index, "target": target };
			var a = '"' + SelectedItems()[index].Data.Path + '"';
			$.ajax({
				type: 'POST',
				url: '<%=ResolveUrl("~/api/MyFiles/Move")%>',
				dataType: 'json',
				data: '{ "OldPath" : "' + SelectedItems()[index].Data.Path.replace(/\\/gi, '/') + '", "NewPath": "' + (target.replace(/\//gi, '\\') + '\\' + SelectedItems()[index].Data.Path.substr(SelectedItems()[index].Data.Path.lastIndexOf('\\'))).replace(/\\\\\\/gi, "\\").replace(/\\\\/gi, "\\").replace(/\\/gi, '/') + '" }',
				contentType: 'application/json',
				success: function (data) {
					temp.index++;
					$("#progressstatus").dialog("title", "Moving item " + (temp.index + 1) + " of " + SelectedItems().length + " items");
					$("#progressstatus .progress").progressbar({ value: (temp.index / SelectedItems().length) * 100 });
					if (temp.index < SelectedItems().length) Move(temp.index, temp.target);
					else { temp = null; Load(); setTimeout(function() { $("#progressstatus").dialog("close"); }, 500); }
				},
				error: function (xhr, ajaxOptions, thrownError) {
					console.log(thrownError);
					console.log(ajaxOptions);
					console.log(xhr);
					if (confirm("An Error has Occured While Moving " + SelectedItems()[temp.index].Data.Name + ", do you want to Continue?\n\nError Details:\n\n" + thrownError)) {
						temp.index++;
						$("#progressstatus").dialog("title", "Moving item " + (temp.index + 1) + " of " + SelectedItems().length + " items");
						$("#progressstatus .progress").progressbar({ value: (temp.index / SelectedItems().length) * 100 });
						if (temp.index < SelectedItems().length) Move(temp.index, temp.target);
						else { temp = null; Load(); setTimeout(function() { $("#progressstatus").dialog("close"); }, 500); }
					} else { temp = null; Load(); setTimeout(function() { $("#progressstatus").dialog("close"); }, 500); }
				}
			});
		}
		function Delete(index) {
			temp = index;
			var a = '"' + SelectedItems()[index].Data.Path.replace(/\\/g, "/") + '"';
			$.ajax({
				type: 'DELETE',
				url: '<%=ResolveUrl("~/api/MyFiles/Delete")%>',
				dataType: 'json',
				data: '[' + a + ']',
				contentType: 'application/json',
				success: function (data) {
					if (data[0].match(/i could not delete/gi)) alert(data[0]);
					temp++;
					$("#progressstatus").dialog("title", "Deleting item " + (temp + 1) + " of " + SelectedItems().length + " items");
					$("#progressstatus .progress").progressbar({ value: (temp / SelectedItems().length) * 100 });
					if (temp < SelectedItems().length) Delete(temp);
					else { temp = null; Load(); setTimeout(function() { $("#progressstatus").dialog("close"); }, 500); }
				},
				error: OnError
			});
		}
		function Upload(file, path) {
			this.File = file;
			this.Path = path;
			this.Start = function() {
				if ("<%=DropZoneAccepted %>".toLowerCase().indexOf(this.File.type.toLowerCase()) == -1 && "<%=DropZoneAccepted %>" != "") {
					alert(this.File.name + " is an restricted file type\n\n\You can only upload:\n\n <%=AcceptedExtensions %>");
					uploads.pop(this);
					return false;
				}
				// Validate file size
				if(this.File.size > <%=maxRequestLength%>) {
					alert(this.File.name + " is Too Big to Upload!");
					uploads.pop(this);
					return false;
				}

				$("#progresses").append('<div id="upload-' + this.File.name.replace(/[\\'\. \[\]\(\)\-]/g, "_") + '"><div class="progressbar" style="display: inline-block; width: 100px; height: 20px; vertical-align: middle; overflow: hidden;"></div> ' + this.File.name + '</div>');
				$("#upload-" + this.File.name.replace(/[\\'\. \[\]\(\)\-]/g, "_") + " .progressbar").progressbar({ value: 0 });
				$.ajax({
					type: 'GET',
					url: '<%=ResolveUrl("~/api/MyFiles/Exists/")%>' + this.Path.replace(/\\/gi, "/").replace(/\.\.\/Download\//gi, "") + '/' + this.File.name,
					dataType: 'json',
					context: this,
					contentType: 'application/json',
					success: function (data) {
						if (data.Name == null || confirm("The file " + this.File.name + " already exists\n\nDo you want to overwrite it?")) this.ContinueUpload(this.File.name);
						else { 
							$("#upload-" + this.File.name.replace(/[\\'\. \[\]\(\)\-]/g, "_")).remove();
							if (uploads.length == 1) $("#uploadprogress").slideUp('slow');
							uploads.pop(this);
						}
					}, error: OnError
				});
				return true;
			};
			this.xhr = new XMLHttpRequest();
			this.ContinueUpload = function(a) {
				var id = a.replace(/[\\'\. \[\]\(\)\-]/g, "_");
				this.xhr = new XMLHttpRequest();
				this.xhr.id = id;
				this.xhr.upload.addEventListener("progress", this.onProgress, false);
				this.xhr.addEventListener("progress", this.onProgress, false);
				this.xhr.onprogress = this.onProgress;
				this.xhr.open('POST', '<%=ResolveUrl("~/api/myfiles-upload/")%>' + this.Path.replace(/\\/g, '/') + '/', true);
				this.xhr.onreadystatechange = function () {
					if (this.readyState == 4) {
						var item = null;
						for (var i = 0; i < uploads.length; i ++) if (uploads[i].File.name.replace(/[\\'\. \[\]\(\)\-]/g, "_") == this.id) item = uploads[i];
						if (this.status != 200) alert("Upload of " + item.File.name + " has Failed!");
						$("#upload-" + this.id + " .progressbar").progressbar("value", 100 );
						$("#upload-" + id).delay(1000).slideUp('slow', function() { $("#upload-" + id).remove(); if (uploads.length == 0) $("#uploadprogress").slideUp('slow'); });
						if (curpath.substr(0, curpath.length - 1).replace(/\//g, "\\") == item.Path) Load();
						uploads.pop(item);
					}
				};
				this.xhr.setRequestHeader('X_FILENAME', this.File.name);
				this.xhr.send(this.File);
			};
			this.onProgress = function (e) {
				var percent = parseInt((e.loaded / e.total) * 100);
				for (var i = 0; i < uploads.length; i++) if (uploads[i].File.size == e.total) uploads[i].updateProgress(percent);
			};
			this.updateProgress = function (e) {
				$("#upload-" + this.File.name.replace(/[\\'\. \[\]\(\)\-]/g, "_") + " .progressbar").progressbar("value", e);
			};
		}
		function Drive(data) {
			this.Data = data;
			this.Id = "";
			this.Render = function () {
				this.Id = this.Data.Path.substr(0, 1);
				$("#MyFiles").append('<a id="' + this.Id + '" href="#' + this.Data.Path + '" class="Drive"><span class="icon">' + this.Data.Path.substr(0, 1) + '</span><span class="label">' + this.Data.Name + '</span><span class="progress"><label>' + this.Data.Space + '%</label><i style="width: ' + this.Data.Space + '%"></i></span></a>');
			};
			this.Refresh = function () {
				$("#" + this.Id).attr("href", "#" + this.Data.Path).html('<span class="icon">' + this.Data.Path.substr(0, 1) + '</span><span class="label">' + this.Data.Name + '</span><span class="progress"><label>' + this.Data.Space + '%</label><i style="width: ' + this.Data.Space + '%"></i></span>');
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
				if (this.Data.Actions == 0 || this.Data.Actions == 3) $("#" + this.Id).draggable({ helper: function () { return $('<div id="dragobject"><img /><span></span></div>'); }, start: function (event, ui) {
					var item = null;
					for (var x = 0; x < items.length; x++) if (items[x].Id == $(this).attr("id")) item = items[x];
					if (!item.Selected) for (var x = 0; x < items.length; x++) if (items[x].Selected) { items[x].Selected = false; items[x].Refresh(); }
					item.Selected = true;
					item.Refresh();
					$("#dragobject").show();
					$("#dragobject img").show().attr("src", item.Data.Icon);
					$("#dragobject span").text("").hide();
				}
				});
				if (this.Data.Type == 'Directory' && this.Data.Actions == 0) $("#" + this.Id).droppable({ accept: '.Selectable', activeClass: 'droppable-active', hoverClass: 'droppable-hover', drop: function (ev, ui) {
					var item = null;
					for (var x = 0; x < items.length; x++) if (items[x].Id == $(this).attr("id")) item = items[x];
					var s = "";
					for (var i = 0; i < SelectedItems().length; i++) s += SelectedItems()[i].Data.Name + "\n";
					$("#progressstatus").dialog({ autoOpen: true, modal: true, title: ((keys.ctrl) ? "Copying" : "Moving") + " 1 of " + SelectedItems().length + " items" });
					$("#progressstatus .progress").progressbar({ value: (1 / SelectedItems().length) * 100 });
					if (keys.ctrl) Copy(0, item.Data.Path);
					else if (confirm("Are you sure you want to move:\n\n" + s)) Move(0, item.Data.Path);
					else $("#progressstatus").dialog("close");
				}, over: function (event, ui) {
					var item = null;
					for (var x = 0; x < items.length; x++) if (items[x].Id == $(this).attr("id")) item = items[x];
					$("#dragobject span").text(((keys.ctrl) ? "Copy" : "Move") + " To " + item.Data.Name).show();
				}, out: function (event, ui) {
					$("#dragobject span").text("").hide();
				}
				});
				if (typeof (window.FileReader) != 'undefined' && this.Data.Type == 'Directory' && this.Data.Actions == 0) {
					$("#" + this.Id).attr("dropzone", "copy<%=DropZoneAccepted %>").bind("dragover", function () {
						var item = null;
						for (var x = 0; x < items.length; x++) if (items[x].Id == $(this).attr("id")) item = items[x];
						$("#uploadto").text("Upload To " + item.Data.Name);
						return false;
					}).bind("dragleave", function () {
						$("#uploadto").text("");
						return false;
					}).bind("dragend", function () {
						$("#uploadto").text("");
						return false;
					});
					$("#" + this.Id)[0].ondrop = function (event) {
						if (event.target.files != null || event.dataTransfer != null) {
							event.preventDefault();
							subdrop = true;
							$("#uploadprogress").slideDown('slow');
							var item = null;
							for (var x = 0; x < items.length; x++) if (items[x].Id == $(this).attr("id")) item = items[x];
							for (var i = 0; i < (event.target.files || event.dataTransfer.files).length; i++) {
								var file = new Upload((event.target.files || event.dataTransfer.files)[i], item.Data.Path);
								uploads.push(file);
								file.Start();
							}
							if (uploads.length == 0) $("#uploadprogress").slideUp('slow');
							$("#uploadto").text("");
							return false;
						}
					};
				}
				$("#" + this.Id).bind("click", this.Click).contextMenu('contextMenu', {
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
						if (curitem.Actions != 0) { $("#con-delete", menu).remove(); $("#con-google", menu).remove(); $("#con-rename", menu).remove(); }
						if (SelectedItems().length > 1) { $("#con-open", menu).remove(); $("#con-rename", menu).remove(); $("#con-properties", menu).remove(); $("#con-preview", menu).remove(); $("#con-google", menu).remove(); }
						else {
							var remgoogle = false;
							if (SelectedItems()[0].Data.Extension != ".txt" && SelectedItems()[0].Data.Extension != ".xlsx" && SelectedItems()[0].Data.Extension != ".docx" && SelectedItems()[0].Data.Extension != ".xls" && SelectedItems()[0].Data.Extension != ".csv" && SelectedItems()[0].Data.Extension != ".png" && SelectedItems()[0].Data.Extension != ".gif" && SelectedItems()[0].Data.Extension != ".jpg" && SelectedItems()[0].Data.Extension != ".jpeg" && SelectedItems()[0].Data.Extension != ".bmp") {
								$("#con-preview", menu).remove();
								if (SelectedItems()[0].Data.Extension != ".ppt" && SelectedItems()[0].Data.Extension != ".pptx" && SelectedItems()[0].Data.Extension != ".pps" && SelectedItems()[0].Data.Extension != ".doc" && SelectedItems()[0].Data.Extension != ".rtf")
									$("#con-google", menu).remove();
							}
						}
						return menu;
					},
					bindings: {
						'con-open': function (t) {
							if (SelectedItems().length > 1) { alert("This only works on 1 item"); return false; }
							if (SelectedItems()[0].Data.Type == 'Directory') window.location.href = "#" + SelectedItems()[0].Data.Path;
							else window.location.href = SelectedItems()[0].Data.Path;
						},
						'con-delete': function (t) {
							$("#progressstatus").dialog({ autoOpen: true, modal: true, title: "Deleting 1 of " + SelectedItems().length + " items" });
							$("#progressstatus .progress").progressbar({ value: (1 / SelectedItems().length) * 100 });
							var s = "";
							for (var i = 0; i < SelectedItems().length; i++) s += SelectedItems()[i].Data.Name + "\n";
							if (confirm("Are you sure you want to delete:\n\n" + s)) Delete(0);
							else $("#progressstatus").dialog("close");
						},
						'con-rename': function (t) {
							if (SelectedItems().length > 1) { alert("This only works on 1 item"); return false; }
							var item = SelectedItems()[0];
							$("#renamebox").val(item.Data.Name).css("display", "block").css("top", $("#" +item.Id).position().top).css("left", $("#" + item.Id).position().left).focus().select();
						},
						'con-properties': function (t) {
							if (SelectedItems().length > 1) { alert("This only works on 1 item"); return false; }
							$("#properties").dialog({ autoOpen: true, modal: true, buttons: {
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
						},
						'con-preview': function (t) {
							if (SelectedItems().length > 1) { alert("This only works on 1 item"); return false; }
							$("#preview").dialog({ autoOpen: true, height: 600, width: 900, modal: true, buttons: {
								"OK": function () {
									$("#previewcont").html("Loading...");
									$(this).dialog("close");
								}
							}
							});
							$.ajax({
								type: 'GET',
								url: '<%=ResolveUrl("~/api/MyFiles/Preview/")%>' + SelectedItems()[0].Data.Path.replace(/\\/gi, "/").replace(/\.\.\/Download\//gi, ""),
								dataType: 'json',
								contentType: 'application/json',
								success: function (data) {
									$("#previewcont").html(data);
								}, error: OnError
							});
						},
						'con-google' : function (t) {
							if (SelectedItems().length > 1) { alert("This only works on 1 item"); return false; }
							$("#googlesignin").dialog({ autoOpen: true, modal: true, buttons: { 
								"Signin": function() { 
									$("#googleuser").addClass("loading");
									$("#googlepass").addClass("loading");
									$("#googlesignin .progress").height(16).width(16).addClass("loading");
									$.ajax({
										type: 'POST',
										url: '<%=ResolveUrl("~/api/MyFiles/SendTo/Google/")%>' + SelectedItems()[0].Data.Path.replace(/\\/gi, "/").replace(/\.\.\/Download\//gi, ""),
										dataType: 'json',
										data: '{ "username" : "' + $("#googleuser").val() + '", "password": "' + $("#googlepass").val() + '" }',
										contentType: 'application/json',
										success: function (data) {
											$("#googlesignin").dialog("close");
											$("#googleuser").val("").removeClass("loading");
											$("#googlepass").val("").removeClass("loading");
											$("#googlesignin .progress").height(0).width(0).removeClass("loading");
											window.open(data, "googledocs");
										},
										error: OnError
									});
								}, "Close": function() { $(this).dialog("close"); } } 
							});
						}
					}
				});
			};
			this.Refresh = function () {
				$("#" + this.Id).attr("href", (this.Data.Path.match(/\.\./i) ? this.Data.Path.replace(/\\/g, "/") : '#' + this.Data.Path)).attr("title", this.Data.Name);
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
			if (typeof (window.FileReader) != 'undefined') $("#MyFiles").unbind("dragover").unbind("dragleave").unbind("dragend").unbind("drop");
			if (curpath == null) {
				$.ajax({
					type: 'GET',
					url: '<%=ResolveUrl("~/api/MyFiles/Drives")%>',
					dataType: 'json',
					contentType: 'application/json;',
					success: function (data) {
						items = new Array();
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
					$("#toolbar").slideDown();
					$("#MyFilesHeaddings .name").css("width", $("#MyFiles > a .label").width() + $("#MyFiles > a img").width() + 4);
					$("#MyFilesHeaddings .type").css("width", $("#MyFiles > a .type").width() + 2);
					$("#MyFilesHeaddings .extension").css("width", $("#MyFiles > a .extension").width() + 2);
					$("#MyFilesHeaddings .size").css("width", $("#MyFiles > a .size").width() + 2);
				}, error: OnError
			});
			$.ajax({
				type: 'GET',
				url: '<%=ResolveUrl("~/api/MyFiles/info/")%>' + curpath.replace(/\\/gi, "/"),
				dataType: 'json',
				contentType: 'application/json',
				success: function (data) {
					curitem = data;
					if (curitem.Actions == 0) {
						$("#newfolderspan").animate({ opacity: 1.0 }, 500, function () { $("#newfolderspan").show(); });
						$("#upload").animate({ opacity: 1.0 }, 500, function () { $("#upload").show(); });
					}
					else {
						$("#newfolderspan").animate({ opacity: 0 }, 500, function () { $("#newfolderspan").hide(); });
						$("#upload").animate({ opacity: 0 }, 500, function () { $("#upload").hide(); });
					}
					if (typeof (window.FileReader) != 'undefined' && curitem.Actions == 0) {
						$("#MyFiles").attr("dropzone", "copy<%=DropZoneAccepted %>").bind("dragover", function () {
							$("#uploadto").text("Upload To " + curitem.Name);
							return false;
						}).bind("dragleave", function () {
							$("#uploadto").text("");
							return false;
						}).bind("dragend", function () {
							$("#uploadto").text("");
							return false;
						});
						$("#MyFiles")[0].ondrop = function (event) {
							if (event.target.files != null || event.dataTransfer != null) {
								event.preventDefault();
								if (subdrop) { subdrop = false; return; }
								$("#uploadprogress").slideDown('slow');
								for (var i = 0; i < (event.target.files || event.dataTransfer.files).length; i++) {
									var file = new Upload((event.target.files || event.dataTransfer.files)[i], curitem.Location.substr(0, curitem.Location.length - 1).replace(/:/g, ""));
									uploads.push(file);
									file.Start();
								}
								$("#uploadto").text("");
							}
						};
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
			$("#properties").dialog({ autoOpen: false });
			$("#preview").dialog({ autoOpen: false });
			$("#progressstate").dialog({ autoOpen: false });
			$("#uploaders").dialog({ autoOpen: false });
			$("#googlesignin").dialog({ autoOpen: false });
			$("#Views").animate({ height: 'toggle' });
			$("#Tree").dynatree({ imagePath: "../images/setup/", selectMode: 1, minExpandLevel: 1, noLink: false, children: [{ icon: "../myfiles-i.png", title: "My Drives", href: "#", isFolder: true, isLazy: true}], fx: { height: "toggle", duration: 200 },
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
									res.push({ title: data[i].Name + " (" + data[i].Path + ")", actions: data[i].Actions, icon: "../drive.png", href: "#" + data[i].Path, isFolder: true, isLazy: true, noLink: false, key: data[i].Path });
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
										res.push({ title: data[i].Name, href: "#" + data[i].Path, actions: data[i].Actions, isFolder: true, isLazy: true, noLink: false, key: data[i].Path });
									}
								node.setLazyNodeStatus(DTNodeStatus_Ok);
								node.addChild(res);
							}, error: OnError
						});
					}
				},
				onRender: function (dtnode, nodeSpan) {
					if (dtnode.data.href != "#") {
						$(nodeSpan).children("a").attr("href", dtnode.data.href).bind("click", function () { window.location.href = $(this).attr("href"); });
						if (dtnode.data.actions != null && dtnode.data.actions == 0) {
							$(nodeSpan).children("a").droppable({ accept: '.Selectable', activeClass: 'droppable-active', hoverClass: 'droppable-hover', drop: function (ev, ui) {
								var item = null;
								for (var x = 0; x < items.length; x++) if (items[x].Id == $(this).attr("id")) item = items[x];
								var s = "";
								for (var i = 0; i < SelectedItems().length; i++) s += SelectedItems()[i].Data.Name + "\n";
								$("#progressstatus").dialog({ autoOpen: true, modal: true, title: ((keys.ctrl) ? "Copying" : "Moving") + " 1 of " + SelectedItems().length + " items" });
								$("#progressstatus .progress").progressbar({ value: (1 / SelectedItems().length) * 100 });
								if (keys.ctrl) Copy(0, $(this).attr("href").substr(1));
								else if (confirm("Are you sure you want to move:\n\n" + s)) Move(0, $(this).attr("href").substr(1));
								else $("#progressstatus").dialog("close");
							}, over: function (event, ui) {
								$("#dragobject span").text(((keys.ctrl) ? "Copy" : "Move") + " To " + $(this).text()).show();
								temp = $(this);
								if (lazytimer == null) lazytimer = setTimeout(function () { $("#Tree").dynatree("getTree").getNodeByKey(temp.attr("href").substr(1)).toggleExpand(); clearTimeout(lazytimer); lazytimer = null; }, 1000);
							}, out: function (event, ui) {
								$("#dragobject span").text("").hide();
								if (lazytimer != null) {
									clearTimeout(lazytimer);
									lazytimer = null;
									temp = null;
								}
							}
							});
							if (typeof (window.FileReader) != 'undefined') {
								$(nodeSpan).children("a").attr("dropzone", "copy<%=DropZoneAccepted %>").bind("dragover", function () {
									$("#uploadto").text("Upload To " + $(this).text());
									return false;
								}).bind("dragleave", function () {
									$("#uploadto").text("");
									return false;
								}).bind("dragend", function () {
									$("#uploadto").text("");
									return false;
								});
								$(nodeSpan).children("a")[0].ondrop = function (event) {
									if (event.target.files != null || event.dataTransfer != null) {
										event.preventDefault();
										$("#uploadprogress").slideDown('slow');
										var files;
										if (event.target.files != null) files = event.target.files;
										else if (event.dataTransfer != null && event.dataTransfer.files != null) files = event.dataTransfer.files;
										if (files == null)  { $("#uploadto").text(""); return;  }
										for (var i = 0; i < files.length; i++) {
											var file = new Upload(files[i], $(this).attr("href").substr(1));
											uploads.push(file);
											file.Start();
										}
										$("#uploadto").text("");
									}
								};
							}
						}
					}
				}
			});
			$("#filter").val("");
			$("button").button().click(function () { return false; });
			$("button.dropdown").button({ icons: { secondary: "ui-icon-carat-1-s"} });
			$("#backup").click(function () { history.go(-1); return false; }).button({ icons: { primary: "ui-icon-circle-arrow-w" }, text: false }).css("height", "26px");
			$("#newfolder").click(function () {
				if ($("#newfolder span").text() == "New Folder") {
					$("#newfolder span").text("Create");
					$("#newfoldertext").val("").css("margin", "0 4px").animate({ width: 150, opacity: 1.0 }).focus();
				} else {
					if (temp != null) { clearTimeout(temp); temp == null; }
					$("#newfoldertext").addClass("loading");
					$.ajax({
						type: 'GET',
						url: '<%=ResolveUrl("~/api/MyFiles/Exists/")%>' + curpath.replace(/\\/gi, "/") + $("#newfoldertext").val() + '/',
						dataType: 'json',
						context: this,
						contentType: 'application/json',
						success: function (data) {
							if (data.Name != null) {
								$("#newfoldertext").removeClass("loading");
								alert("The folder " + data.Name + " already exists!")
							} else {
								$.ajax({
									type: 'POST',
									url: '<%=ResolveUrl("~/api/MyFiles/New/")%>' + curpath.replace(/\\/gi, "/") + $("#newfoldertext").val(),
									dataType: 'json',
									contentType: 'application/json',
									success: function (data) {
										$("#newfoldertext").removeClass("loading");
										$("#newfoldertext").animate({ width: 0, opacity: 0.0 }).css("margin", "0");
										$("#newfolder span").text("New Folder");
										Load();
									},
									error: OnError
								});
							}
						}, error: OnError
					});
				}
			});
			$("#newfoldertext").focusout(function () {
				temp = setTimeout(function () { 
					$("#newfoldertext").removeClass("loading");
					$("#newfoldertext").animate({ width: 0, opacity: 0.0 }).css("margin", "0");
					$("#newfolder span").text("New Folder");
				}, 1000);
			}).focusin(function() {
				if (temp != null) { clearTimeout(temp); temp == null; }
			}).trigger("focusout").keydown(function (event) {
				var keycode = (event.keyCode ? event.keyCode : (event.which ? event.which : event.charCode));
				if (keycode == 9 || keycode == 59 || keycode == 188 || keycode == 190 || keycode == 192 || keycode == 111 || keycode == 220 || keycode == 191 || keycode == 106 || (keycode == 56 && keys.shift) || (keycode == 50 && keys.shift)) { event.preventDefault(); return; }
				else if (keycode == 27) { $("#newfoldertext").blur(); event.preventDefault(); }
				else if (keycode == 13) { $("#newfolder").trigger("click"); return false; }
			});
			$("#renamebox").focusout(function() {
				if (temp == "esc") { temp = null; return; }
				else {
					if (SelectedItems()[0].Data.Name == $(this).val()) { $("#renamebox").css("display", "none"); return; }
					$("#renamebox").css("display", "none");
					$("#progressstatus").dialog({ autoOpen: true, modal: true, title: "Checking..." });
					$("#progressstatus .progress").progressbar({ value: 0 });
					$.ajax({
						type: 'GET',
						url: '<%=ResolveUrl("~/api/MyFiles/Exists/")%>' + (SelectedItems()[0].Data.Path.substr(0, SelectedItems()[0].Data.Path.lastIndexOf('\\')) + "\\" + $("#renamebox").val() + (SelectedItems()[0].Data.Extension == null ? '\\' : SelectedItems()[0].Data.Extension)).replace(/\\\\/gi, "\\").replace(/\\/gi, "/"),
						dataType: 'json',
						context: this,
						contentType: 'application/json',
						success: function (data) {
							if (data.Name != null) {
								$("#progressstatus").dialog({ autoOpen: true, modal: true, title: "Waiting..." });
								$("#progressstatus .progress").progressbar({ value: 10 });
								confirm(data.Name + " already exists!");
								$("#progressstatus").dialog("close");
							} else {
								$("#progressstatus").dialog({ autoOpen: true, modal: true, title: "Renaming..." });
								$("#progressstatus .progress").progressbar({ value: 50 });
								$.ajax({
									type: 'POST',
									url: '<%=ResolveUrl("~/api/MyFiles/Move")%>',
									data: '{ "OldPath": "' + SelectedItems()[0].Data.Path.replace(/\\/gi, "/") + '", "NewPath": "' + (SelectedItems()[0].Data.Path.substr(0, SelectedItems()[0].Data.Path.lastIndexOf('\\')) + "\\" + $("#renamebox").val() + (SelectedItems()[0].Data.Extension == null ? '\\' : SelectedItems()[0].Data.Extension)).replace(/\\\\/gi, "\\").replace(/\\/gi, "/") + '" }',
									dataType: 'json',
									contentType: 'application/json',
									success: function (data) {
										$("#progressstatus").dialog({ autoOpen: true, modal: true, title: "Waiting..." });
										$("#progressstatus .progress").progressbar({ value: 100 });
										temp = null; 
										setTimeout(function() { $("#progressstatus").dialog("close"); }, 500);
										Load();
									},
									error: OnError
								});
							}
						}, error: OnError
					});
				}
			}).keyup(function(event) { 
				var keycode = (event.keyCode ? event.keyCode : (event.which ? event.which : event.charCode));
				if (keycode == 27) { temp = "esc"; $("#renamebox").css("display", "none"); event.preventDefault(); }
				else temp = null;
			}).keydown(function (event) {
				var keycode = (event.keyCode ? event.keyCode : (event.which ? event.which : event.charCode));
				if (keycode == 9 || keycode == 59 || keycode == 188 || keycode == 190 || keycode == 192 || keycode == 111 || keycode == 220 || keycode == 191 || keycode == 106 || (keycode == 56 && keys.shift) || (keycode == 50 && keys.shift)) { event.preventDefault(); return; }
				else if (keycode == 13) { event.preventDefault(); $("#renamebox").blur(); }
			});
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
					$("#MyFiles").addClass("details").removeClass("small").removeClass("medium").removeClass("large").css("padding-top", $("#toolbar").height() + 34);
					$("#MyFilesHeaddings").css("display", "block");
					$("#MyFilesHeaddings .name").css("width", $("#MyFiles > a .label").width() + $("#MyFiles > a img").width() + 4);
					$("#MyFilesHeaddings .type").css("width", $("#MyFiles > a .type").width() + 2);
					$("#MyFilesHeaddings .extension").css("width", $("#MyFiles > a .extension").width() + 2);
					$("#MyFilesHeaddings .size").css("width", $("#MyFiles > a .size").width() + 2);

					$("#renamebox").removeClass("small").removeClass("medium").removeClass("large").addClass("details");
				}
				else if ($(this).text() == "Small Icons") {
					viewMode = 2;
					$("#MyFiles").addClass("small").removeClass("details").removeClass("medium").removeClass("large").css("padding-top", $("#toolbar").height() + 10);
					$("#MyFilesHeaddings").css("display", "none");
					$("#renamebox").removeClass("details").removeClass("medium").removeClass("large").addClass("small");
				}
				else if ($(this).text() == "Medium Icons") {
					viewMode = 3;
					$("#MyFiles").addClass("medium").removeClass("small").removeClass("large").removeClass("details").css("padding-top", $("#toolbar").height() + 10);
					$("#MyFilesHeaddings").css("display", "none");
					$("#renamebox").removeClass("details").removeClass("small").removeClass("large").addClass("medium");
				}
				else if ($(this).text() == "Large Icons") {
					viewMode = 4;
					$("#MyFiles").addClass("large").removeClass("medium").removeClass("small").removeClass("details").css("padding-top", $("#toolbar").height() + 10);
					$("#MyFilesHeaddings").css("display", "none");
					$("#renamebox").removeClass("details").removeClass("small").removeClass("medium").addClass("large");
				}
				else {
					viewMode = 0;
					$("#MyFiles").removeClass("details").removeClass("small").removeClass("medium").removeClass("large").css("padding-top", $("#toolbar").height() + 10);
					$("#renamebox").removeClass("details").removeClass("small").removeClass("medium").removeClass("large");
					$("#MyFilesHeaddings").css("display", "none");
				}
			});
			$("#uploadprogress").css("margin-left", $("#myfilescontent").width() - $("#uploadprogress").width()).slideUp('slow');
			$("#upload").click(function () { $("#uploaders").dialog({ autoOpen: true, modal: true, buttons: { 
				"Upload": function() { 
					$("#uploadprogress").slideDown('slow');
					if ($("#<%=uploadedfiles.ClientID %>")[0].files != null) {
						for (var i = 0; i < $("#<%=uploadedfiles.ClientID %>")[0].files.length; i++) {
							var file = new Upload(($("#<%=uploadedfiles.ClientID %>")[0].files)[i], curitem.Location.substr(0, curitem.Location.length - 1).replace(/:/g, ""));
							uploads.push(file);
							file.Start();
						}
						if (uploads.length == 0) $("#uploadprogress").slideUp('slow');
						$("#<%=uploadedfiles.ClientID %>").html($("#<%=uploadedfiles.ClientID %>").html());
					} else {
						$("#<%=p.ClientID %>").val(curpath);
						$("#<%=uploadbtn.ClientID %>").trigger("click");
						$("#uploadprogress").slideDown('slow');
						$("#uploadprogress").html("Uploading...<br />This page will refrersh when it has finished");
					}
					$("#uploadto").text("");
					$(this).dialog("close");
				}, "Close": function() { $(this).dialog("close"); } } 
			}); });
			$("#<%=uploadedfiles.ClientID %>").attr("accept", "<%=DropZoneAccepted.Replace("f:", "") %>");
			$("#toolbar").css("width", $("#myfilescontent").width() - 10).css("top", $("#myfilescontent").offset().top);
			$("#Tree").css("top", $("#myfilescontent").offset().top + $("#toolbar").height() + 10);
			$("#MyFiles").css("margin-left", $("#Tree").width() + 5).css("padding-top", $("#toolbar").height() + 10);
			$("#Views").css("top", $("#view").offset().top + $("#view").parent().height()).css("left", $("#view").offset().left - ($("#Views").width() - $("#view").width()) + 1);
			$("#MyFilesHeaddings").css("margin-left", $("#Tree").width() + 5).css("top", $("#myfilescontent").offset().top + $("#toolbar").height() + 10);
			$(window).scroll(function (event) {
				if ($(this).scrollTop() >= $("#myfilesheader").offset().top + $("#myfilesheader").height()) {
					$("#toolbar").css("position", "fixed").css("top", 0);
					$("#Tree").css("position", "fixed").css("top", $("#toolbar").height() + 10);
					$("#MyFilesHeaddings").css("position", "fixed").css("top", $("#toolbar").height() + 8);
					$("#Views").css("position", "fixed").css("top", $("#toolbar").height() + 4);
				}
				else {
					$("#toolbar").css("position", "absolute").css("top", $("#myfilesheader").offset().top + $("#myfilesheader").height());
					$("#Tree").css("position", "absolute").css("top", $("#myfilesheader").offset().top + $("#myfilesheader").height() + $("#toolbar").height() + 10);
					$("#Views").css("position", "absolute").css("top", $("#myfilesheader").offset().top + $("#myfilesheader").height() + $("#toolbar").height() + 4);
					$("#MyFilesHeaddings").css("position", "absolute").css("top", $("#myfilesheader").offset().top + $("#myfilesheader").height() + $("#toolbar").height() + 8);
				}
			});
			$(window).trigger("hashchange");
			
		});
		$(document).bind('keydown', function (event) { var keycode = (event.keyCode ? event.keyCode : (event.which ? event.which : event.charCode)); keys.shift = (keycode == 16); keys.ctrl = (keycode == 17); });
		$(document).bind('keyup', function (event) { keys.shift = keys.ctrl = false; });
	</script>
</asp:Content>
