<%@ Page Title="" Language="C#" MasterPageFile="~/masterpage.master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="HAP.Web.MyFiles.Default" %>
<asp:Content ContentPlaceHolderID="head" runat="server">
	<script src="../Scripts/jquery-1.7.1.min.js" type="text/javascript"></script>
	<script src="../Scripts/jquery-ui-1.8.16.custom.min.js" type="text/javascript"></script>
	<script src="../Scripts/jquery.ba-hashchange.min.js" type="text/javascript"></script>
	<script src="../Scripts/jquery.dynatree.js" type="text/javascript"></script>
	<script src="../Scripts/jquery.contextmenu.js" type="text/javascript"></script>
	<script src="../Scripts/hap.web.js.js" type="text/javascript"></script>
	<link href="../style/ui.dynatree.css" rel="stylesheet" type="text/css" />
	<link href="../style/MyFiles.css" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ContentPlaceHolderID="body" runat="server">
	<div style="overflow: hidden; clear: both; position: relative; height: 120px" id="myfilesheader">
		<div class="tiles" style="position: absolute; left: 0; margin-top: 45px;">
			<a class="button" href="../"><hap:LocalResource StringPath="homeaccessplus" runat="server" Seperator=" " StringPath2="home" /></a>
		</div>
		<div class="tiles" style="position: absolute; right: 0; text-align: right; margin-top: 45px;">
			<a class="button" id="bug" href="#" onclick="return false;">Got an Issue?</a>
			<a class="button" id="help" href="#" onclick="return false;"><hap:LocalResource StringPath="help" runat="server" /></a>
		</div>
		<div style="text-align: center;">
			<img src="../images/myfiles.png" alt="My Files" />
		</div>
	</div>
	<input type="text" id="renamebox" />
	<hap:WrappedLocalResource runat="server" title="#myfiles/properties" id="properties" Tag="div">
		<div id="propcont"><hap:LocalResource StringPath="loading" runat="server" />...</div>
	</hap:WrappedLocalResource>
	<hap:WrappedLocalResource runat="server" title="#myfiles/preview" id="preview" style="height: 500px; overflow: auto; width: 700px;" Tag="div">
		<div id="previewcont"><hap:LocalResource StringPath="loading" runat="server" />...</div>
	</hap:WrappedLocalResource>
	<hap:WrappedLocalResource runat="server" title="#myfiles/progress" id="progressstatus" Tag="div">
		<div class="progress"></div>
	</hap:WrappedLocalResource>
	<hap:WrappedLocalResource runat="server" id="googlesignin" title="#myfiles/sendto/googlesignin" Tag="div">
		<div><hap:LocalResource StringPath="myfiles/sendto/googlesignin2" runat="server" /></div>
		<div>
			<label for="googleuser"><hap:LocalResource StringPath="username" runat="server" />: </label>
			<input type="text" id="googleuser" />
		</div>
		<div>
			<label for="googlepass"><hap:LocalResource StringPath="password" runat="server" />: </label>
			<input type="password" id="googlepass" />
		</div>
		<div class="progress"></div>
	</hap:WrappedLocalResource>
	<div class="contextMenu" id="contextMenu">
	  <ul>
		<li id="con-open"><hap:LocalResource StringPath="myfiles/open" runat="server" /></li>
		<li id="con-download"><hap:LocalResource StringPath="myfiles/download" runat="server" /></li>
		<li id="con-delete"><hap:LocalResource StringPath="myfiles/delete/delete" runat="server" /></li>
		<li id="con-rename"><hap:LocalResource StringPath="myfiles/rename" runat="server" /></li>
		<li id="con-preview"><hap:LocalResource StringPath="myfiles/preview" runat="server" /></li>
		<li id="con-properties"><hap:LocalResource StringPath="myfiles/properties" runat="server" /></li>
		<li id="con-google"><hap:LocalResource StringPath="myfiles/sendto/googledocs" runat="server" /></li>
	  </ul>
	</div>
	<hap:WrappedLocalResource runat="server" id="uploaders" title="#myfiles/upload/upload" Tag="div">
		<input type="file" multiple="multiple" id="uploadedfiles" />
		<iframe style="width: 300px; height: 180px"></iframe>
	</hap:WrappedLocalResource>
	<div id="uploadprogress" class="tile-border-color" style="border-width: 1px; border-style: solid; border-bottom: 0;">
		<div class="tile-color ui-widget-header"><hap:LocalResource StringPath="myfiles/upload/uploadprogress" runat="server" /></div>
		<div id="progresses">
		</div>
	</div>
	<div id="myfilescontent">
	<div id="toolbar" style="padding: 4px; margin-bottom: 4px;" class="ui-widget-header">
		<div style="float: right;">
			<span style="color: #fff;" id="search"><hap:LocalResource runat="server" StringPath="search" />:
			<input type="text" id="filter" />
			</span>
			<button class="dropdown" id="view"><hap:LocalResource runat="server" StringPath="myfiles/view" /></button>
		</div>
		<div style="float: left;">
			<button id="backup"></button>
		</div>
		<div style="float: left; margin-left: 3px;">
			<input type="text" id="newfoldertext" />
			<button id="newfolder"><hap:LocalResource runat="server" StringPath="myfiles/newfolder" /></button>
			<button id="upload"><hap:LocalResource runat="server" StringPath="myfiles/upload/upload" /></button>
			<label id="uploadto"></label>
		</div>
	</div>
	<div id="Views" class="tile-border-color">
		<button id="tiles"><hap:LocalResource runat="server" StringPath="myfiles/tiles" /></button>
		<button id="smallicons"><hap:LocalResource runat="server" StringPath="myfiles/smallicons" /></button>
		<button id="mediumicons"><hap:LocalResource runat="server" StringPath="myfiles/mediumicons" /></button>
		<button id="largeicons"><hap:LocalResource runat="server" StringPath="myfiles/largeicons" /></button>
		<button id="details"><hap:LocalResource runat="server" StringPath="myfiles/details" /></button>
	</div>
	<div id="Tree" class="tile-border-color">
	</div>
	<div id="MyFilesHeaddings">
		<span class="name"><hap:LocalResource runat="server" StringPath="name" /></span><span class="type"><hap:LocalResource runat="server" StringPath="myfiles/type" /></span><span class="extension"><hap:LocalResource runat="server" StringPath="myfiles/extension" /></span><span class="size"><hap:LocalResource runat="server" StringPath="myfiles/size" /></span></div>
	<div id="MyFiles" class="tiles">
	</div>
	</div>
	<hap:CompressJS runat="server" tag="div">
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
		function Copy(index, target) {
			temp = { "index": index, "target": target };
			var a = '"' + SelectedItems()[index].Data.Path + '"';
			$.ajax({
				type: 'POST',
				url: hap.common.resolveUrl('~/api/MyFiles/Copy') + '?' + window.JSON.stringify(new Date()),
				dataType: 'json',
				data: '{ "OldPath" : "' + SelectedItems()[index].Data.Path.replace(/\\/gi, '/') + '", "NewPath": "' + (target.replace(/\//gi, '\\') + '\\' + SelectedItems()[index].Data.Path.substr(SelectedItems()[index].Data.Path.lastIndexOf('\\'))).replace(/\\\\\\/gi, "\\").replace(/\\\\/gi, "\\").replace(/\\/gi, '/') + '" }',
				contentType: 'application/json',
				success: function (data) {
					temp.index++;
					$("#progressstatus").dialog("title", hap.common.getLocal("myfiles/copy/copyingitem1") + " " + (temp.index + 1) + " " + hap.common.getLocal("of") + " " + SelectedItems().length + " " + hap.common.getLocal("items"));
					$("#progressstatus .progress").progressbar({ value: (temp.index / SelectedItems().length) * 100 });
					if (temp.index < SelectedItems().length) Move(temp.index, temp.target);
					else { temp = null; Load(); setTimeout(function() { $("#progressstatus").dialog("close"); }, 500); }
				},
				error: function (xhr, ajaxOptions, thrownError) {
					console.log(xhr.responseXML.documentElement.children[2]);
					if (confirm(hap.common.getLocal("myfiles/copy/error1") + " " + SelectedItems()[temp.index].Data.Name + ", " + hap.common.getLocal("myfiles/copy/error2") + "\n\n" + hap.common.getLocal("errordetails") + ":\n\n" + xhr.responseXML.documentElement.children[1].children[0].textContent)) {
						temp.index++;
						$("#progressstatus").dialog("title", hap.common.getLocal("myfiles/copy/copyingitem1") + " " + (temp.index + 1) + " " + hap.common.getLocal("of") + " " + SelectedItems().length + " " + hap.common.getLocal("items"));
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
				url: hap.common.resolveUrl('api/MyFiles/Move') + '?' + window.JSON.stringify(new Date()),
				dataType: 'json',
				data: '{ "OldPath" : "' + SelectedItems()[index].Data.Path.replace(/\\/gi, '/') + '", "NewPath": "' + (target.replace(/\//gi, '\\') + '\\' + SelectedItems()[index].Data.Path.substr(SelectedItems()[index].Data.Path.lastIndexOf('\\'))).replace(/\\\\\\/gi, "\\").replace(/\\\\/gi, "\\").replace(/\\/gi, '/') + '" }',
				contentType: 'application/json',
				success: function (data) {
					temp.index++;
					$("#progressstatus").dialog("title", hap.common.getLocal("myfiles/move/movingitem1") + " " + (temp.index + 1) + " " + hap.common.getLocal("of") + " " + SelectedItems().length + " " + hap.common.getLocal("items"));
					$("#progressstatus .progress").progressbar({ value: (temp.index / SelectedItems().length) * 100 });
					if (temp.index < SelectedItems().length) Move(temp.index, temp.target);
					else { temp = null; Load(); setTimeout(function() { $("#progressstatus").dialog("close"); }, 500); }
				},
				error: function (xhr, ajaxOptions, thrownError) {
					console.log(xhr.responseXML.documentElement.children[2]);
					if (confirm(hap.common.getLocal("myfiles/move/error1") + " " + SelectedItems()[temp.index].Data.Name + ", " + hap.common.getLocal("myfiles/move/error2") + "\n\n" + hap.common.getLocal("errordetails") + ":\n\n" + xhr.responseXML.documentElement.children[1].children[0].textContent)) {
						temp.index++;
						$("#progressstatus").dialog("title", hap.common.getLocal("myfiles/move/movingitem1") + " " + (temp.index + 1) + " " + hap.common.getLocal("of") + " " + SelectedItems().length + " " + hap.common.getLocal("items"));
						$("#progressstatus .progress").progressbar({ value: (temp.index / SelectedItems().length) * 100 });
						if (temp.index < SelectedItems().length) Move(temp.index, temp.target);
						else { temp = null; Load(); setTimeout(function() { $("#progressstatus").dialog("close"); }, 500); }
					} else { temp = null; Load(); setTimeout(function() { $("#progressstatus").dialog("close"); }, 500); }
				}
			});
		}
		function Delete(index) {
			temp = index;
			var a = '"' + SelectedItems()[index].Data.Path.replace(/\.\.\/download\//gi, "").replace(/\\/g, "/") + '"';
			$.ajax({
				type: 'POST',
				url: hap.common.resolveUrl('~/api/MyFiles/Delete') + '?' + window.JSON.stringify(new Date()),
				dataType: 'json',
				data: '[' + a + ']',
				contentType: 'application/json',
				success: function (data) {
					if (data[0].match(/i could not delete/gi)) alert(data[0]);
					temp++;
					$("#progressstatus").dialog("title", hap.common.getLocal("myfiles/delete/deletingitem1") + " " + (temp.index + 1) + " " + hap.common.getLocal("of") + " " + SelectedItems().length + " " + hap.common.getLocal("items"));
					$("#progressstatus .progress").progressbar({ value: (temp / SelectedItems().length) * 100 });
					if (temp < SelectedItems().length) Delete(temp);
					else { temp = null; Load(); setTimeout(function() { $("#progressstatus").dialog("close"); }, 500); }
				},
				error: hap.common.jsonError
			});
		}
		function Upload(file, path) {
			this.File = file;
			this.Path = path;
			this.Start = function() {
				if (this.File.name.indexOf('.') == -1) {
					alert(hap.common.getLocal("myfiles/upload/folderwarning").replace(/\%/g, this.File.name));
					uploads.pop(this);
					return false;
				}
				else if ("<%=DropZoneAccepted %>".toLowerCase().indexOf(this.File.name.substr(this.File.name.lastIndexOf('.')).toLowerCase()) == -1 && "<%=DropZoneAccepted %>" != "") {
					alert(this.File.name + " " + hap.common.getLocal("myfiles/upload/filetypewarning") + "\n\n <%=AcceptedExtensions %>");
					uploads.pop(this);
					return false;
				}
				if(this.File.size > <%=maxRequestLength%>) {
					alert(this.File.name + " " + hap.common.getLocal("myfiles/upload/filesizewarning"));
					uploads.pop(this);
					return false;
				}

				$("#progresses").append('<div id="upload-' + this.File.name.replace(/[\\'\. \[\]\(\)\-]/g, "_") + '"><div class="progressbar" style="display: inline-block; width: 100px; height: 20px; vertical-align: middle; overflow: hidden;"></div> ' + this.File.name + '</div>');
				$("#upload-" + this.File.name.replace(/[\\'\. \[\]\(\)\-]/g, "_") + " .progressbar").progressbar({ value: 0 });
				$.ajax({
					type: 'GET',
					url: hap.common.resolveUrl('~/api/MyFiles/Exists/') + this.Path.replace(/\\/gi, "/").replace(/\.\.\/Download\//gi, "") + '/' + this.File.name + '?' + window.JSON.stringify(new Date()),
					dataType: 'json',
					context: this,
					contentType: 'application/json',
					success: function (data) {
						if (data.Name == null || confirm(hap.common.getLocal("myfiles/upload/fileexists1") + " " + this.File.name + " " + hap.common.getLocal("myfiles/upload/fileexists2"))) this.ContinueUpload(this.File.name);
						else { 
							$("#upload-" + this.File.name.replace(/[\\'\. \[\]\(\)\-]/g, "_")).remove();
							if (uploads.length == 1) $("#uploadprogress").slideUp('slow');
							uploads.pop(this);
						}
					}, error: hap.common.jsonError
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
				this.xhr.open('POST', hap.common.resolveUrl('~/api/myfiles-upload/') + this.Path.replace(/\\/g, '/') + '/', true);
				this.xhr.onreadystatechange = function () {
					if (this.readyState == 4) {
						var item = null;
						for (var i = 0; i < uploads.length; i ++) if (uploads[i].File.name.replace(/[\\'\. \[\]\(\)\-]/g, "_") == this.id) item = uploads[i];
						if (this.status != 200) alert(hap.common.getLocal("myfiles/upload/upload") + " " + hap.common.getLocal("of") + " " + item.File.name + " " + hap.common.getLocal("myfiles/upload/failed") + "\n\n" + this.responseText.substr(this.responseText.indexOf('<title>') + 7, this.responseText.indexOf('</title>') - (7 + this.responseText.indexOf('<title>'))));
						$("#upload-" + this.id + " .progressbar").progressbar("value", 100 );
						$("#upload-" + id).delay(1000).slideUp('slow', function() { $("#upload-" + id).remove(); if (uploads.length == 0) $("#uploadprogress").slideUp('slow'); });
						if (curpath.substr(0, curpath.length - 1).replace(/\//g, "\\") == item.Path || curpath.replace(/\//g, "\\") == item.Path) Load();
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
				this.Id = (this.Data.Name + this.Data.Extension).replace(/\s|[^a-z|^A-Z|^0-9|\-]/gi, "_");
				var label = this.Data.Name;

				var h = '<a id="' + this.Id + '" title="' + this.Data.Name + '" ';
				if (this.Data.Type == 'Directory') h += 'class="Folder Selectable" ';
				else h += 'class="Selectable" ';
				h += 'href="' + (this.Data.Path.match(/\.\./i) ? this.Data.Path.replace(/\\/g, "/") : '#' + this.Data.Path) + '"><img class="icon" src="' + this.Data.Icon + '" alt="" /><span class="label">' + label + '</span><span class="type">';
				if (this.Data.Type == 'Directory') h += hap.common.getLocal("myfiles/filefolder");
				else h += this.Data.Type + '</span><span class="extension">' + this.Data.Extension + '</span><span class="size">' + this.Data.Size;
				h += '</span></a>';
				$("#MyFiles").append(h);
				if (this.Data.Actions == 0) $("#" + this.Id).draggable({ helper: function () { return $('<div id="dragobject"><img /><span></span></div>'); }, start: function (event, ui) {
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
					$("#progressstatus").dialog({ autoOpen: true, modal: true, title: hap.common.getLocal("myfiles/" + ((keys.ctrl) ? "copy/copying" : "move/moving")) + " 1 " + hap.common.getLocal("of") + " " + SelectedItems().length + " " + hap.common.getLocal("items") });
					$("#progressstatus .progress").progressbar({ value: (1 / SelectedItems().length) * 100 });
					if (keys.ctrl) Copy(0, item.Data.Path);
					else if (confirm(hap.common.getLocal("myfiles/move/question1") + "\n\n" + s)) Move(0, item.Data.Path);
					else $("#progressstatus").dialog("close");
				}, over: function (event, ui) {
					var item = null;
					for (var x = 0; x < items.length; x++) if (items[x].Id == $(this).attr("id")) item = items[x];
					$("#dragobject span").text(hap.common.getLocal("myfiles/" + ((keys.ctrl) ? "copy/copy" : "move/move")) + " " + hap.common.getLocal("to") + " " + item.Data.Name).show();
				}, out: function (event, ui) {
					$("#dragobject span").text("").hide();
				}
				});
				if (typeof (window.FileReader) != 'undefined' && this.Data.Type == 'Directory' && this.Data.Actions == 0) {
					$("#" + this.Id).attr("dropzone", "copy<%=DropZoneAccepted %>").bind("dragover", function () {
						var item = null;
						for (var x = 0; x < items.length; x++) if (items[x].Id == $(this).attr("id")) item = items[x];
						$("#uploadto").text(hap.common.getLocal("myfiles/upload/upload") + " " + hap.common.getLocal("to") + " " + item.Data.Name);
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
				$("#" + this.Id).bind("click", this.Click).bind("dblclick", this.DoubleClick).contextMenu('contextMenu', {
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
						if (curitem.Actions == 3) { $("#con-download", menu).remove(); if (SelectedItems().length != 1 || SelectedItems()[0].Data.Type != 'Directory') ("#con-open", menu).remove(); $("#con-properties", menu).remove(); }
						if (SelectedItems().length > 1) { $("#con-download", menu).remove(); $("#con-open", menu).remove(); $("#con-rename", menu).remove(); $("#con-properties", menu).remove(); $("#con-preview", menu).remove(); $("#con-google", menu).remove(); }
						else {
							var remgoogle = false;
							if (SelectedItems()[0].Data.Extension != ".zip") $("#con-download", menu).remove();
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
							if (SelectedItems().length > 1) { alert(hap.common.getLocal("myfiles/only1")); return false; }
							if (SelectedItems()[0].Data.Type == 'Directory') window.location.href = "#" + SelectedItems()[0].Data.Path;
							else if (SelectedItems()[0].Data.Extension == ".zip") window.location.href="#" + (curitem.Location.replace(/:/gi, "").replace(/\//gi, "/") + "\\").replace(/\\\\/gi, "\\") + SelectedItems()[0].Data.Name + ".zip";
							else window.location.href = SelectedItems()[0].Data.Path;
						},
						'con-download': function (t) {
							if (SelectedItems().length > 1) { alert(hap.common.getLocal("myfiles/only1")); return false; }
							if (SelectedItems()[0].Data.Type == 'Directory') window.location.href = "#" + SelectedItems()[0].Data.Path;
							else window.location.href = SelectedItems()[0].Data.Path;
						},
						'con-delete': function (t) {
							$("#progressstatus").dialog({ autoOpen: true, modal: true, title: hap.common.getLocal("myfiles/delete/deletingitem1") + " 1 " + hap.common.getLocal("of") + " " + SelectedItems().length + " " + hap.common.getLocal("items") });
							$("#progressstatus .progress").progressbar({ value: (1 / SelectedItems().length) * 100 });
							var s = "";
							for (var i = 0; i < SelectedItems().length; i++) s += SelectedItems()[i].Data.Name + "\n";
							if (confirm(hap.common.getLocal("myfiles/delete/question1") + ":\n\n" + s)) Delete(0);
							else $("#progressstatus").dialog("close");
						},
						'con-rename': function (t) {
							if (SelectedItems().length > 1) { alert(hap.common.getLocal("myfiles/only1")); return false; }
							var item = SelectedItems()[0];
							$("#renamebox").val(item.Data.Name).css("display", "block").css("top", $("#" +item.Id).position().top).css("left", $("#" + item.Id).position().left).focus().select();
						},
						'con-properties': function (t) {
							if (SelectedItems().length > 1) { alert(hap.common.getLocal("myfiles/only1")); return false; }
							$("#properties").dialog({ autoOpen: true, modal: true, buttons: {
								"OK": function () {
									$(this).dialog("close");
									$("#propcont").html("Loading...");
								}
							}
							});
							$.ajax({
								type: 'GET',
								url: hap.common.resolveUrl('~/api/MyFiles/Properties/') + SelectedItems()[0].Data.Path.replace(/\\/gi, "/").replace(/\.\.\/Download\//gi, "") + '?' + window.JSON.stringify(new Date()),
								dataType: 'json',
								contentType: 'application/json',
								success: function (data) {
									var s = '<div><img src="' + data.Icon + '" alt="" style="width: 32px; float: left; margin-right: 40px;" />' + data.Name + '</div>';
									s += '<hr style="height: 1px; border-width: 1px" />';
									if (data.Type == "File Folder") {
										s += '<div><label>' + hap.common.getLocal("myfiles/type") + ': </label>' + data.Type + '</div>';
										s += '<div><label>' + hap.common.getLocal("myfiles/location") + ': </label><a href="' + hap.common.resolveUrl("~/api/myfiles-permalink/" + data.Location.replace(/\\/g, "/") + "/") + '">' + data.Location + '</a></div>';
										s += '<div><label>' + hap.common.getLocal("myfiles/size") + ': </label>' + data.Size + '</div>';
										s += '<div><label>' + hap.common.getLocal("myfiles/contains") + ': </label>' + data.Contents + '</div>';
										s += '<hr style="height: 1px; border-width: 1px" />';
										s += '<div><label>' + hap.common.getLocal("myfiles/created") + ': </label>' + data.DateCreated + '</div>';
									} else {
										s += '<div><label>' + hap.common.getLocal("myfiles/typeoffile") + ': </label>' + data.Type + ' (' + data.Extension + ')</div>';
										s += '<hr style="height: 1px; border-width: 1px" />';
										s += '<div><label>' + hap.common.getLocal("myfiles/location") + ': </label>' + data.Location + '</div>';
										s += '<div><label>' + hap.common.getLocal("myfiles/size") + ': </label>' + data.Size + '</div>';
										s += '<hr style="height: 1px; border-width: 1px" />';
										s += '<div><label>' + hap.common.getLocal("myfiles/created") + ': </label>' + data.DateCreated + '</div>';
										s += '<div><label>' + hap.common.getLocal("myfiles/modified") + ': </label>' + data.DateModified + '</div>';
										s += '<div><label>' + hap.common.getLocal("myfiles/accessed") + ': </label>' + data.DateAccessed + '</div>';
									}
									$("#propcont").html(s);
								}, error: hap.common.jsonError
							});
						},
						'con-preview': function (t) {
							if (SelectedItems().length > 1) { alert(hap.common.getLocal("myfiles/only1")); return false; }
							$("#preview").dialog({ autoOpen: true, height: 600, width: 900, modal: true, buttons: {
								"OK": function () {
									$("#previewcont").html(hap.common.getLocal("loading") + "...");
									$(this).dialog("close");
								}
							}
							});
							$.ajax({
								type: 'GET',
								url: hap.common.resolveUrl('~/api/MyFiles/Preview/') + SelectedItems()[0].Data.Path.replace(/\\/gi, "/").replace(/\.\.\/Download\//gi, "") + '?' + window.JSON.stringify(new Date()),
								dataType: 'json',
								contentType: 'application/json',
								success: function (data) {
									$("#previewcont").html(data);
								}, error: hap.common.jsonError
							});
						},
						'con-google' : function (t) {
							if (SelectedItems().length > 1) { alert(hap.common.getLocal("myfiles/only1")); return false; }
							$("#googlesignin").dialog({ autoOpen: true, modal: true, buttons: { 
								"Signin": function() { 
									$("#googleuser").addClass("loading");
									$("#googlepass").addClass("loading");
									$("#googlesignin .progress").height(16).width(16).addClass("loading");
									$.ajax({
										type: 'POST',
										url: hap.common.resolveUrl('~/api/MyFiles/SendTo/Google/') + SelectedItems()[0].Data.Path.replace(/\\/gi, "/").replace(/\.\.\/Download\//gi, "") + '?' + window.JSON.stringify(new Date()),
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
										error: hap.common.jsonError
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
				if (this.Data.Type == 'Directory') h += hap.common.getLocal("myfiles/filefolder");
				else h += this.Data.Type + '</span><span class="extension">' + this.Data.Extension + '</span><span class="size">' + this.Data.Size;
				h += '</span>';

				$("#" + this.Id).html(h);
				if (this.Show) $("#" + this.Id).removeAttr("style");
				else $("#" + this.Id).css("display", "none");
			};
			this.Selected = false;
			this.ClickTimer = null;
			this.DoubleClick = function (e) {
				$('#jqContextMenu').css("display", "none");
				$('#jqContextMenuShadow').css("display", "none");
				var item = null;
				for (var x = 0; x < items.length; x++) if (items[x].Id == $(this).attr("id")) item = items[x];
				if (item.Data.Type != 'Directory' && item.Data.Actions == 3) return;
				if (item.Data.Type != 'Directory') alert(hap.common.getLocal("myfiles/downloadwarning"));
				var item = null;
				for (var x = 0; x < items.length; x++)
					if (items[x].Id == $(this).attr("id")) { item = items[x]; break; }
				window.location.href = (item.Data.Path.match(/\.\./i) ? item.Data.Path.replace(/\\/g, "/") : '#' + item.Data.Path);
			}
			this.Click = function (e) {
				e.preventDefault();
				$('#jqContextMenu').css("display", "none");
				$('#jqContextMenuShadow').css("display", "none");
				var item = null;
				for (var x = 0; x < items.length; x++) if (items[x].Id == $(this).attr("id")) item = items[x];
				clearTimeout(item.ClickTimer);
				item.Clicks++;
				item.ClickTimer = setTimeout("for (var x = 0; x < items.length; x++) if (items[x].Id == '" + item.Id + "') items[x].Clicks = 0;", 500);
				if (item.Clicks == 1) {
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
					url: hap.common.resolveUrl('~/api/MyFiles/Drives') + '?' + window.JSON.stringify(new Date()),
					dataType: 'json',
					contentType: 'application/json;',
					success: function (data) {
						items = new Array();
						$("#MyFiles").html("");
						for (var i = 0; i < data.length; i++)
							items.push(new Drive(data[i]));
						for (var i = 0; i < items.length; i++)
							items[i].Render();
					}, error: hap.common.jsonError
				});
			} else {
			$.ajax({
				type: 'GET',
				url: hap.common.resolveUrl("~/api/MyFiles/") + curpath.replace(/\\/gi, "/") + '?' + window.JSON.stringify(new Date()),
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
				}, error: hap.common.jsonError
			});
			$.ajax({
				type: 'GET',
				url: hap.common.resolveUrl("~/api/MyFiles/info/") + curpath.replace(/\\/gi, "/") + '?' + window.JSON.stringify(new Date()),
				dataType: 'json',
				contentType: 'application/json',
				success: function (data) {
					curitem = data;
					if (curitem.Actions == 0) {
						$("#newfolder").animate({ opacity: 1.0 }, 500, function () { $("#newfolder").show(); });
						$("#newfoldertext").blur();
						$("#upload").animate({ opacity: 1.0 }, 500, function () { $("#upload").show(); });
					}
					else {
						$("#newfolder").animate({ opacity: 0 }, 500, function () { $("#newfolder").hide(); });
						$("#newfoldertext").blur();
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
									var file = new Upload((event.target.files || event.dataTransfer.files)[i], (curpath.length == 2 ? curitem.Location.substr(0, curitem.Location.length -1 ) : curitem.Location).replace(/:/g, ""));
									uploads.push(file);
									file.Start();
								}
								$("#uploadto").text("");
							}
						};
					}
				}, error: hap.common.jsonError
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
			$("#Tree").dynatree({ imagePath: "../images/setup/", selectMode: 1, minExpandLevel: 1, noLink: false, children: [{ icon: "../myfiles-i.png", title: hap.common.getLocal("myfiles/mydrives"), href: "#", isFolder: true, isLazy: true}], fx: { height: "toggle", duration: 200 },
				onLazyRead: function (node) {
					if (node.data.href == "#") {
						$.ajax({
							type: 'GET',
							url: hap.common.resolveUrl("~/api/MyFiles/Drives") + '?' + window.JSON.stringify(new Date()),
							dataType: 'json',
							contentType: 'application/json',
							success: function (data) {
								res = [];
								for (var i = 0; i < data.length; i++)
									res.push({ title: data[i].Name + " (" + data[i].Path + ")", actions: data[i].Actions, icon: "../drive.png", href: "#" + data[i].Path, isFolder: true, isLazy: true, noLink: false, key: data[i].Path });
								node.setLazyNodeStatus(DTNodeStatus_Ok);
								node.addChild(res);
							}, error: hap.common.jsonError
						});
					} else {
						$.ajax({
							type: 'GET',
							url: hap.common.resolveUrl("~/api/MyFiles/") + node.data.href.substr(1).replace(/\\/g, "/") + '?' + window.JSON.stringify(new Date()),
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
							}, error: hap.common.jsonError
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
								$("#progressstatus").dialog({ autoOpen: true, modal: true, title: hap.common.getLocal("myfiles/" + ((keys.ctrl) ? "copy/copying" : "move/moving")) + " 1 " + hap.common.getLocal("of") + " " + SelectedItems().length + " " + hap.common.getLocal("items") });
								$("#progressstatus .progress").progressbar({ value: (1 / SelectedItems().length) * 100 });
								if (keys.ctrl) Copy(0, $(this).attr("href").substr(1));
								else if (confirm(hap.common.getLocal("myfiles/move/question1") + "\n\n" + s)) Move(0, $(this).attr("href").substr(1));
								else $("#progressstatus").dialog("close");

							}, over: function (event, ui) {
								$("#dragobject span").text(hap.common.getLocal("myfiles/" + ((keys.ctrl) ? "copy/copy" : "move/move")) + " " + hap.common.getLocal("to") + " " + $(this).text()).show();
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
									$("#uploadto").text(hap.common.getLocal("myfiles/upload/upload") + " " + hap.common.getLocal("to") + " " + $(this).text());
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
				if ($("#newfolder span").text() == hap.common.getLocal("myfiles/newfolder")) {
					$("#newfolder span").text(hap.common.getLocal("myfiles/create"));
					$("#newfoldertext").val("").show().animate({ width: 150, opacity: 1.0 }).focus();
				} else {
					if (temp != null) { clearTimeout(temp); temp == null; }
					$("#newfoldertext").addClass("loading");
					$.ajax({
						type: 'GET',
						url: hap.common.resolveUrl("~/api/MyFiles/Exists/") + curpath.replace(/\\/gi, "/") + $("#newfoldertext").val() + '/' + '?' + window.JSON.stringify(new Date()),
						dataType: 'json',
						context: this,
						contentType: 'application/json',
						success: function (data) {
							if (data.Name != null) {
								$("#newfoldertext").removeClass("loading");
								alert(hap.common.getLocal("myfiles/folderexists1") + " " + data.Name + " " + hap.common.getLocal("myfiles/folderexists2"));
							} else {
								$.ajax({
									type: 'POST',
									url: hap.common.resolveUrl("~/api/MyFiles/New/") + curpath.replace(/\\/gi, "/") + $("#newfoldertext").val() + '?' + window.JSON.stringify(new Date()),
									dataType: 'json',
									contentType: 'application/json',
									success: function (data) {
										$("#newfoldertext").removeClass("loading");
										$("#newfoldertext").animate({ width: 0, opacity: 0.0 }).css("margin", "0");
										$("#newfolder span").text(hap.common.getLocal("myfiles/newfolder"));
										Load();
									},
									error: hap.common.jsonError
								});
							}
						}, error: hap.common.jsonError
					});
				}
			});
			$("#newfoldertext").focusout(function () {
				temp = setTimeout(function () { 
					$("#newfoldertext").removeClass("loading");
					$("#newfoldertext").animate({ width: 0, opacity: 0.0 }, 500, function() { $("#newfoldertext").hide() }).css("margin", "0");
					$("#newfolder span").text(hap.common.getLocal("myfiles/newfolder"));
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
					$("#progressstatus").dialog({ autoOpen: true, modal: true, title: hap.common.getLocal("myfiles/checking") + "..." });
					$("#progressstatus .progress").progressbar({ value: 0 });
					$.ajax({
						type: 'GET',
						url: hap.common.resolveUrl("~/api/MyFiles/Exists/") + (SelectedItems()[0].Data.Path.substr(0, SelectedItems()[0].Data.Path.lastIndexOf('\\')) + "\\" + $("#renamebox").val() + (SelectedItems()[0].Data.Extension == null ? '\\' : SelectedItems()[0].Data.Extension)).replace(/\\\\/gi, "\\").replace(/\\/gi, "/") + '?' + window.JSON.stringify(new Date()),
						dataType: 'json',
						context: this,
						contentType: 'application/json',
						success: function (data) {
							if (data.Name != null) {
								$("#progressstatus").dialog({ autoOpen: true, modal: true, title: hap.common.getLocal("myfiles/waiting") + "..." });
								$("#progressstatus .progress").progressbar({ value: 10 });
								confirm(data.Name + " " + hap.common.getLocal("myfiles/folderexists2"));
								$("#progressstatus").dialog("close");
							} else {
								$("#progressstatus").dialog({ autoOpen: true, modal: true, title: hap.common.getLocal("myfiles/renaming") + "..." });
								$("#progressstatus .progress").progressbar({ value: 50 });
								$.ajax({
									type: 'POST',
									url: hap.common.resolveUrl("~/api/MyFiles/Move") + '?' + window.JSON.stringify(new Date()),
									data: '{ "OldPath": "' + SelectedItems()[0].Data.Path.replace(/\\/gi, "/") + '", "NewPath": "' + (SelectedItems()[0].Data.Path.substr(0, SelectedItems()[0].Data.Path.lastIndexOf('\\')) + "\\" + $("#renamebox").val() + (SelectedItems()[0].Data.Extension == null ? '\\' : SelectedItems()[0].Data.Extension)).replace(/\\\\/gi, "\\").replace(/\\/gi, "/") + '" }',
									dataType: 'json',
									contentType: 'application/json',
									success: function (data) {
										$("#progressstatus").dialog({ autoOpen: true, modal: true, title: hap.common.getLocal("myfiles/waiting") + "..." });
										$("#progressstatus .progress").progressbar({ value: 100 });
										temp = null; 
										setTimeout(function() { $("#progressstatus").dialog("close"); }, 500);
										Load();
									},
									error: hap.common.jsonError
								});
							}
						}, error: hap.common.jsonError
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
			$("#help").button({icons: { secondary: 'ui-icon-help' }}).click(function() {
				hap.help.Load("myfiles/index");
				return false;
			});
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
			});
			$("#Views button").click(function () {
				if ($(this).attr("id") == "details") {
					viewMode = 1;
					$("#MyFiles").addClass("details").removeClass("small").removeClass("medium").removeClass("large").css("padding-top", $("#toolbar").height() + 34);
					$("#MyFilesHeaddings").css("display", "block");
					$("#MyFilesHeaddings .name").css("width", $("#MyFiles > a .label").width() + $("#MyFiles > a img").width() + 4);
					$("#MyFilesHeaddings .type").css("width", $("#MyFiles > a .type").width() + 2);
					$("#MyFilesHeaddings .extension").css("width", $("#MyFiles > a .extension").width() + 2);
					$("#MyFilesHeaddings .size").css("width", $("#MyFiles > a .size").width() + 2);

					$("#renamebox").removeClass("small").removeClass("medium").removeClass("large").addClass("details");
				}
				else if ($(this).attr("id") == "smallicons") {
					viewMode = 2;
					$("#MyFiles").addClass("small").removeClass("details").removeClass("medium").removeClass("large").css("padding-top", $("#toolbar").height() + 10);
					$("#MyFilesHeaddings").css("display", "none");
					$("#renamebox").removeClass("details").removeClass("medium").removeClass("large").addClass("small");
				}
				else if ($(this).attr("id") == "mediumicons") {
					viewMode = 3;
					$("#MyFiles").addClass("medium").removeClass("small").removeClass("large").removeClass("details").css("padding-top", $("#toolbar").height() + 10);
					$("#MyFilesHeaddings").css("display", "none");
					$("#renamebox").removeClass("details").removeClass("small").removeClass("large").addClass("medium");
				}
				else if ($(this).attr("id") == "largeicons") {
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
			$("#upload").click(function () { 
				if ($("#uploadedfiles")[0].files != null) {
					$("#uploaders iframe").remove();
					$("#uploaders").dialog({ autoOpen: true, resizable: false, modal: true, buttons: { 
						"Upload": function() { 
							$("#uploadprogress").slideDown('slow');
							for (var i = 0; i < $("#uploadedfiles")[0].files.length; i++) {
								var file = new Upload(($("#uploadedfiles")[0].files)[i], (curpath.length == 2 ? curitem.Location.substr(0, curitem.Location.length -1 ) : curitem.Location).replace(/:/g, ""));
								uploads.push(file);
								file.Start();
							}
							if (uploads.length == 0) $("#uploadprogress").slideUp('slow');
							$("#uploadedfiles").html($("#uploadedfiles").html());
							$("#uploadto").text("");
							$(this).dialog("close");
						}, "Close": function() { $(this).dialog("close"); } } 
					});
				} else {
					$("#uploaders iframe").attr("src", "../uploadh.aspx?path=" + curpath).css("display", "block");
					$("#uploaders input").hide();
					$("#uploaders").dialog({ autoOpen: true, modal: true, width: 320, height: 280, resizable: false });
				}
			});
			$("#uploadedfiles").attr("accept", "<%=DropZoneAccepted.Replace("f:", "") %>");
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
			$("#bug").click(function() { hap.help.Load("myfiles/bug"); return false; });
		});
		$(document).bind('keydown', function (event) { var keycode = (event.keyCode ? event.keyCode : (event.which ? event.which : event.charCode)); keys.shift = (keycode == 16); keys.ctrl = (keycode == 17); });
		$(document).bind('keyup', function (event) { keys.shift = keys.ctrl = false; });
		function closeUpload() { $("#uploaders").dialog("close"); };
		</script>
	</hap:CompressJS>
	<% if (FirstTime) { %> <script type="text/javascript">$(function () { $("#help").trigger("click"); });</script><%}  %>
</asp:Content>
