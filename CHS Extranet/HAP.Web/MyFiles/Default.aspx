<%@ Page Title="" Language="C#" MasterPageFile="~/masterpage.master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="HAP.Web.MyFiles.Default" %>
<asp:Content ContentPlaceHolderID="head" runat="server">
	<script src="../Scripts/jquery.ba-hashchange.min.js" type="text/javascript"></script>
	<script src="../Scripts/jquery.dynatree.js" type="text/javascript"></script>
	<script src="../Scripts/jquery.contextmenu.js" type="text/javascript"></script>
	<link href="../style/ui.dynatree.css" rel="stylesheet" type="text/css" />
	<link href="../style/MyFiles.css" rel="stylesheet" type="text/css" />
    <script src="//js.live.net/v5.0/wl.js"></script>
    <script>$("link[media='handheld'], link[media='screen and (max-device-width: 780px)'], style[media='screen and (max-device-width: 780px)']").remove(); var oldie = false;</script>
    <!--[if (lt IE 11)]><script>oldie = true;</script><![endif]-->
</asp:Content>
<asp:Content ContentPlaceHolderID="title" runat="server"><asp:HyperLink runat="server" NavigateUrl="~/MyFiles/"><hap:LocalResource runat="server" StringPath="myfiles/myfiles" /></asp:HyperLink></asp:Content>
<asp:Content ContentPlaceHolderID="header" runat="server">
	<a style="float: right;" id="help" href="#" onclick="return false;"><hap:LocalResource StringPath="help" runat="server" />?</a>
    <div id="toolbar" data-role="header">
		<div style="float: right;">
            <a class="dropdown" id="de" href="#"><hap:LocalResource runat="server" StringPath="myfiles/directedit" /></a>
			<a class="dropdown" id="view" href="#"><hap:LocalResource runat="server" StringPath="myfiles/view" /></a>
		</div>
		<div style="float: left;">
			<a href="#" id="backup"><span></span></a>
		</div>
		<div style="float: left; margin-left: 3px;" id="maintools">
			<input type="text" id="newfoldertext" /><a id="newfolder" href="#"><hap:LocalResource runat="server" StringPath="myfiles/newfolder" /></a>
            <a href="#" id="toolbar-cut"><hap:LocalResource runat="server" StringPath="myfiles/cut" /></a>
            <a href="#" id="toolbar-copy"><hap:LocalResource runat="server" StringPath="myfiles/copy/copy" /></a>
            <div id="toolbar-paste-container" class="menucontainer"><a href="#" id="toolbar-paste"><hap:LocalResource runat="server" StringPath="myfiles/paste" /></a>|<a style="padding: 0 5px;" href="#" id="toolbar-clear">X</a></div>
            <a href="#" id="toolbar-delete"><hap:LocalResource runat="server" StringPath="myfiles/delete/delete" /></a>
            <a href="#" id="toolbar-zip"><hap:LocalResource runat="server" StringPath="myfiles/zip/zip" /></a>
            <a href="#" id="toolbar-unzip"><hap:LocalResource runat="server" StringPath="myfiles/unzip/unzip" /></a>
            <a href="#" id="toolbar-open"><hap:LocalResource runat="server" StringPath="myfiles/open" /></a>
            <a href="#" id="toolbar-download"><hap:LocalResource runat="server" StringPath="myfiles/download" /></a>
            <a href="#" id="upload"><hap:LocalResource runat="server" StringPath="myfiles/upload/upload" /></a>
            <label id="uploadto"></label>
		</div>
	</div>
</asp:Content>
<asp:Content ContentPlaceHolderID="body" runat="server">
    <script>
        var ad = "<%=HAP.MyFiles.DirectEditToken.ConvertToToken(HttpContext.Current.Request.Cookies["token"].Value + "|" + HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName].Value + "|" + HttpContext.Current.Request.Url.ToString().ToLower().Replace("myfiles/default.aspx", "api/myfiles/destatus/confirm")) %>";
        var dechecktimer, detimeout, run1 = true;
        $(function () {
            dechecktimer = setInterval(function () {
                $.ajax({
                    type: 'GET',
                    url: hap.common.formatJSONUrl('~/api/MyFiles/DEStatus/Query'),
                    dataType: 'json',
                    contentType: 'application/json',
                    success: function (data) {
                        if (data == true)
                        {
                            clearInterval(dechecktimer);
                            clearTimeout(detimeout);
                            detimeout = dechecktimer = null;
                            $("#hapdirectedit").prop("checked", true);
                        }
                        else if (run1) {
                            if (window.navigator.userAgent.match(/firefox/gi))
                            $('<iframe />', {
                                'id': 'decheck',
                                'src': 'hap://' + ad,
                                'style': 'display: none; position: absolute; top: 0; left: 0;'
                            }).appendTo("body");
                            run1 = false;
                        }
                    },
                    error: hap.common.jsonError
                });
            }, 1000);
            detimeout = setTimeout(function () {
                clearInterval(dechecktimer);
            }, 10005);
        });
    </script>
	<input type="text" id="renamebox" />
	<hap:WrappedLocalResource runat="server" title="#myfiles/properties" id="properties" Tag="div">
		<div id="propcont"><hap:LocalResource StringPath="loading" runat="server" />...</div>
	</hap:WrappedLocalResource>
	<hap:WrappedLocalResource runat="server" title="#myfiles/zip/question1" id="zipquestion" Tag="div">
		<div id="zipcont"><hap:LocalResource StringPath="myfiles/zip/question2" runat="server" /> <input type="text" id="zipfilename" />.zip</div>
	</hap:WrappedLocalResource>
	<hap:WrappedLocalResource runat="server" title="#myfiles/preview" id="preview" style="height: 500px; overflow: auto; width: 700px;" Tag="div">
		<div id="previewcont"><hap:LocalResource StringPath="loading" runat="server" />...</div>
	</hap:WrappedLocalResource>
	<hap:WrappedLocalResource runat="server" title="#myfiles/progress" id="progressstatus" Tag="div">
		<div class="progress"></div>
	</hap:WrappedLocalResource>
    <hap:WrappedLocalResource runat="server" ID="loadingbox" title="#myfiles/sendto/skydrive" Tag="div">
		<hap:LocalResource StringPath="myfiles/upload/uploading" runat="server" />...
	</hap:WrappedLocalResource>
	<hap:WrappedLocalResource runat="server" id="googlesignin" title="#myfiles/sendto/googlesignin" Tag="div" style="width: 400px;">
		<div><hap:LocalResource StringPath="myfiles/sendto/googlesignin2" runat="server" /></div>
		<div>
			<label for="googleuser"><hap:LocalResource StringPath="username" runat="server" />: </label>
			<input type="text" id="googleuser" style="width: 300px;" />
		</div>
		<div>
			<label for="googlepass"><hap:LocalResource StringPath="password" runat="server" />: </label>
			<input type="password" id="googlepass" />
		</div>
		<div class="progress"></div>
	</hap:WrappedLocalResource>
    <div id="questionboxes"></div>
	<div class="contextMenu" id="contextMenu">
        <ul>
            <li id="con-open"><hap:LocalResource StringPath="myfiles/open" runat="server" /></li>
            <li id="con-download"><hap:LocalResource StringPath="myfiles/download" runat="server" /></li>
            <li id="con-delete"><hap:LocalResource StringPath="myfiles/delete/delete" runat="server" /></li>
            <li id="con-rename"><hap:LocalResource StringPath="myfiles/rename" runat="server" /></li>
            <li id="con-preview"><hap:LocalResource StringPath="myfiles/preview" runat="server" /></li>
            <li id="con-properties"><hap:LocalResource StringPath="myfiles/properties" runat="server" /></li>
            <li id="con-unzip"><hap:LocalResource ID="LocalResource1" StringPath="myfiles/unzip/unzip" runat="server" /></li>
            <li id="con-zip"><hap:LocalResource ID="LocalResource2" StringPath="myfiles/zip/zip" runat="server" /></li>
            <li id="con-google"><hap:LocalResource StringPath="myfiles/sendto/googledocs" runat="server" /></li>
            <% if (!string.IsNullOrEmpty(config.MyFiles.LiveAppId)) { %>
            <li id="con-skydrive"><hap:LocalResource StringPath="myfiles/sendto/skydrive" runat="server" /></li>
            <% } %>
        </ul>
	</div>
	<hap:WrappedLocalResource runat="server" id="uploaders" title="#myfiles/upload/upload" Tag="div">
		<input type="file" multiple="multiple" id="uploadedfiles" />
        <label for="uploadfilesrandom"><hap:LocalResource runat="server" StringPath="myfiles/upload/random" /></label>
        <input type="checkbox" id="uploadfilesrandom" />
		<iframe style="width: 300px; height: 180px"></iframe>
	</hap:WrappedLocalResource>
	<div id="uploadprogress" class="tile-border-color" style="border-width: 1px; border-style: solid; border-bottom: 0;">
		<div class="tile-color ui-widget-header"><hap:LocalResource StringPath="myfiles/upload/uploadprogress" runat="server" /></div>
		<div id="progresses">
		</div>
	</div>
	<div id="myfilescontent">
	<div id="Views" class="hapmenu tile-border-color">
		<a href="#" id="tiles"><hap:LocalResource runat="server" StringPath="myfiles/tiles" /></a>
		<a href="#" id="smallicons"><hap:LocalResource runat="server" StringPath="myfiles/smallicons" /></a>
		<a href="#" id="mediumicons"><hap:LocalResource runat="server" StringPath="myfiles/mediumicons" /></a>
		<a href="#" id="largeicons"><hap:LocalResource runat="server" StringPath="myfiles/largeicons" /></a>
		<a href="#" id="details"><hap:LocalResource runat="server" StringPath="myfiles/details" /></a>
	</div>
    <div id="directedit" class="hapmenu tile-border-color">
        <label for="hapdirectedit">Enable <hap:LocalResource runat="server" StringPath="myfiles/directedit" />: </label>
        <input type="checkbox" id="hapdirectedit" />
    </div>
    <div id="search">
		<input type="text" id="filter" />
	</div>
	<div id="Tree" class="tile-border-color">
	</div>
	<div id="MyFilesHeaddings">
		<a class="name" href="javascript:void(Load('Default', Sortorder == 'Default' ? (Sortdirection == 1 ? 2 : 1) : 1));"><hap:LocalResource runat="server" StringPath="name" /></a><a class="modified" href="javascript:void(Load('UnderlyingModified', Sortorder == 'UnderlyingModified' ? (Sortdirection == 1 ? 2 : 1) : 1));"><hap:LocalResource runat="server" StringPath="myfiles/modified" /></a><a class="type" href="javascript:void(Load('Type', Sortorder == 'Type' ? (Sortdirection == 1 ? 2 : 1) : 1));"><hap:LocalResource runat="server" StringPath="myfiles/type" /></a><a class="extension" href="javascript:void(Load('Extension', Sortorder == 'Extension' ? (Sortdirection == 1 ? 2 : 1) : 1));"><hap:LocalResource runat="server" StringPath="myfiles/extension" /></a><a href="javascript:void(Load('UnderlyingSize', Sortorder == 'UnderlyingSize' ? (Sortdirection == 1 ? 2 : 1) : 1));" class="size"><hap:LocalResource runat="server" StringPath="myfiles/size" /></a></div>
	<div id="MyFiles" class="tiles" data-role="content">
	</div>
	</div>
	<hap:CompressJS runat="server" tag="script">
	<script type="text/javascript">
		var items = new Array();
		var subdrop = false;
		var showView = 0;
		var showDE = 0;
		var viewMode = 0;
		var keys = { shift: false, ctrl: false };
		var lazytimer = null;
		var temp, clipboard, table, curitem, curpath, Sortorder, Sortdirection = null;
		var uploads = new Array();
		$(window).hashchange(function () {
			$("#filter").val(hap.common.getLocal("search"));
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
				$("#search input").fadeOut();
				if (typeof (window.FileReader) != 'undefined') {
					$("#MyFiles").removeAttr("dropzone").attr("dropzone", "copy<%=DropZoneAccepted %>").unbind("dragover").unbind("dragleave").unbind("dragend");
				}
				$("#MyFiles").removeClass("details").removeClass("small").removeClass("medium").removeClass("large");
				$("#MyFilesHeaddings").hide();
			}
			Load();
		});
	    function Zip(zipfile, index) {
	        temp = { Index: index, File: zipfile };
	        var a = '"' + SelectedItems()[index].Data.Path.replace(/\.\.\/download\//gi, "").replace(/\\/g, "/") + '"';
	        $.ajax({
	            type: 'POST',
	            url: hap.common.formatJSONUrl('~/api/MyFiles/Zip'),
	            dataType: 'json',
	            data: '{ "Zip": "' + zipfile.replace(/\\/g, "/").replace("//", "/") + '", "Paths": [' + a.replace(/\\/g, "/") + '] }',
	            contentType: 'application/json',
	            success: function (data) {
	                temp.Index++;
	                $("#progressstatus").dialog("option", "title", hap.common.getLocal("myfiles/zip/zipingitem1") + " " + (temp.Index + 1) + " " + hap.common.getLocal("of") + " " + SelectedItems().length + " " + hap.common.getLocal("items"));
	                $("#progressstatus .progress").progressbar({ value: (temp.Index / SelectedItems().length) * 100 });
	                if (temp.Index < SelectedItems().length) Zip(temp.File, temp.Index);
	                else { temp = null; Load(); setTimeout(function() { $("#progressstatus").dialog("close"); }, 500); }
	            },
	            error: hap.common.jsonError
	        });
	    }
		function UnZip(overwrite) {
			if (overwrite == null) overwrite = false;
			var a = SelectedItems()[0].Data.Path.replace(/\.\.\/download\//gi, "").replace(/\\/gi, "/");
			a = (a.match(/\.zip\\/gi) ? (a.split(/\.zip\\/gi)[0] + ".zip") : a);
			a = '"' + a + '"';
			$.ajax({
				type: 'POST',
				url: hap.common.formatJSONUrl('~/api/MyFiles/UnZip'),
				dataType: 'json',
				data: '{ "ZipFile" : ' + a + ', "Overwrite": "' + overwrite + '" }',
				contentType: 'application/json',
				success: function (data) {
					$("#progressstatus").dialog("option", "title", hap.common.getLocal("myfiles/unzip/unzipping") + " " + SelectedItems()[0].Data.Name);
					$("#progressstatus .progress").progressbar({ value: 80 });
					temp = null; 
					Load(); 
					setTimeout(function() { $("#progressstatus").dialog("close"); }, 500);
				},
				error: function (xhr, ajaxOptions, thrownError) {
				    if (jQuery.parseJSON(xhr.responseText).Message == "Destination Folder Exists")
					{
						if (confirm(hap.common.getLocal("myfiles/folderexists1") + " " + SelectedItems()[0].Data.Name + " " + hap.common.getLocal("myfiles/folderexists2") + "\n" + hap.common.getLocal("myfiles/merge")))
							Unzip(true);
						else {
							$("#progressstatus").dialog("option", "title", hap.common.getLocal("myfiles/unzip/unzipping") + " " + SelectedItems()[0].Data.Name);
							$("#progressstatus .progress").progressbar({ value: 50 });
							temp = null; 
							Load(); 
							setTimeout(function() { $("#progressstatus").dialog("close"); }, 500);
						}
					}
				    else if (jQuery.parseJSON(xhr.responseText).Message == "File Exists in Destination")
					{
						if (confirm(hap.common.getLocal("myfiles/fileexists1") + " " + SelectedItems()[0].Data.Name + " " + hap.common.getLocal("myfiles/fileexists2")))
							Unzip(true);
						else {
							$("#progressstatus").dialog("option", "title", hap.common.getLocal("myfiles/unzip/unzipping") + " " + SelectedItems()[0].Data.Name);
							$("#progressstatus .progress").progressbar({ value: 50 });
							temp = null; 
							Load(); 
							setTimeout(function() { $("#progressstatus").dialog("close"); }, 500);
						}
					}
					else if (confirm(hap.common.getLocal("myfiles/unzip/error1") + " " + SelectedItems()[0].Data.Name + ", " + hap.common.getLocal("myfiles/unzip/error2") + "\n\n" + hap.common.getLocal("errordetails") + ":\n\n" + xhr.responseXML.documentElement.children[1].children[0].textContent)) {
						$("#progressstatus").dialog("option", "title", hap.common.getLocal("myfiles/unzip/unzipping") + " " + SelectedItems()[0].Data.Name);
						$("#progressstatus .progress").progressbar({ value: 50 });
						temp = null; 
						Load(); 
						setTimeout(function() { $("#progressstatus").dialog("close"); }, 500);
					} else { 
					    hap.common.jsonError(xhr, ajaxOptions, thrownError);
						$("#progressstatus").dialog("option", "title", hap.common.getLocal("myfiles/unzip/unzipping") + " " + SelectedItems()[0].Data.Name);
						$("#progressstatus .progress").progressbar({ value: 50 });
						temp = null; 
						Load(); 
						setTimeout(function() { $("#progressstatus").dialog("close"); }, 500);
					}
				}
			});
		}
		function Copy(index, target, overwrite) {
			temp = { "index": index, "target": target };
			if (overwrite == null) overwrite = false;
			$.ajax({
				type: 'POST',
				url: hap.common.formatJSONUrl('~/api/MyFiles/Copy'),
				dataType: 'json',
				data: '{ "OldPath" : "' + SelectedItems()[index].Data.Path.replace(/\.\.\/download\//gi, "").replace(/\\/gi, "/") + '", "NewPath": "' + (target.replace(/\\/gi, "/") + '/' + SelectedItems()[index].Data.Path.replace(/\\/gi, "/").substr(SelectedItems()[index].Data.Path.replace(/\\/gi, "/").lastIndexOf('/')).replace(/\//gi, "")) + '", "Overwrite": "' + overwrite + '" }',
				contentType: 'application/json',
				success: function (data) {
					temp.index++;
					$("#progressstatus").dialog("option", "title", hap.common.getLocal("myfiles/copy/copyingitem1") + " " + (temp.index + 1) + " " + hap.common.getLocal("of") + " " + SelectedItems().length + " " + hap.common.getLocal("items"));
					$("#progressstatus .progress").progressbar({ value: (temp.index / SelectedItems().length) * 100 });
					if (temp.index < SelectedItems().length) Copy(temp.index, temp.target);
					else { temp = null; Load(); setTimeout(function() { $("#progressstatus").dialog("close"); }, 500); }
				},
				error: function (xhr, ajaxOptions, thrownError) {
					if (jQuery.parseJSON(xhr.responseText).Message == "Destination Folder Exists")
					{
					    if (confirm(hap.common.getLocal("myfiles/folderexists1") + " " + SelectedItems()[temp.index].Data.Name + " " + hap.common.getLocal("myfiles/folderexists2") + "\n" + hap.common.getLocal("myfiles/merge")))
							Copy(temp.index, temp.target, true);
						else {
							temp.index++;
							$("#progressstatus").dialog("option", "title", hap.common.getLocal("myfiles/copy/copyingitem1") + " " + (temp.index + 1) + " " + hap.common.getLocal("of") + " " + SelectedItems().length + " " + hap.common.getLocal("items"));
							$("#progressstatus .progress").progressbar({ value: (temp.index / SelectedItems().length) * 100 });
							if (temp.index < SelectedItems().length) Copy(temp.index, temp.target);
							else { temp = null; Load(); setTimeout(function() { $("#progressstatus").dialog("close"); }, 500); }
						}
					}
					else if (jQuery.parseJSON(xhr.responseText).Message == "File Exists in Destination")
					{
						if (confirm(hap.common.getLocal("myfiles/upload/fileexists1") + " " + SelectedItems()[temp.index].Data.Name + " " + hap.common.getLocal("myfiles/upload/fileexists2")))
							Copy(temp.index, temp.target, true);
						else {
							temp.index++;
							$("#progressstatus").dialog("option", "title", hap.common.getLocal("myfiles/copy/copyingitem1") + " " + (temp.index + 1) + " " + hap.common.getLocal("of") + " " + SelectedItems().length + " " + hap.common.getLocal("items"));
							$("#progressstatus .progress").progressbar({ value: (temp.index / SelectedItems().length) * 100 });
							if (temp.index < SelectedItems().length) Copy(temp.index, temp.target);
							else { temp = null; Load(); setTimeout(function() { $("#progressstatus").dialog("close"); }, 500); }
						}
					}
					else if (confirm(hap.common.getLocal("myfiles/copy/error1") + " " + SelectedItems()[temp.index].Data.Name + ", " + hap.common.getLocal("myfiles/copy/error2") + "\n\n" + hap.common.getLocal("errordetails") + ":\n\n" + jQuery.parseJSON(xhr.responseText).Message)) {
						temp.index++;
						$("#progressstatus").dialog("option", "title", hap.common.getLocal("myfiles/copy/copyingitem1") + " " + (temp.index + 1) + " " + hap.common.getLocal("of") + " " + SelectedItems().length + " " + hap.common.getLocal("items"));
						$("#progressstatus .progress").progressbar({ value: (temp.index / SelectedItems().length) * 100 });
						if (temp.index < SelectedItems().length) Copy(temp.index, temp.target);
						else { temp = null; Load(); setTimeout(function() { $("#progressstatus").dialog("close"); }, 500); }
					} else { hap.common.jsonError(xhr, ajaxOptions, thrownError); temp = null; Load(); setTimeout(function() { $("#progressstatus").dialog("close"); }, 500); }
				}
			});
		}
		function CopyClipboard(index, target, overwrite) {
			temp = { "index": index, "target": target };
			if (overwrite == null) overwrite = false;
			var a = '"' + clipboard.items[index].Data.Path + '"';
			$.ajax({
				type: 'POST',
				url: hap.common.formatJSONUrl('~/api/MyFiles/Copy'),
				dataType: 'json',
				data: '{ "OldPath" : "' + clipboard.items[index].Data.Path.replace(/\.\.\/download\//gi, "").replace(/\\/gi, "/") + '", "NewPath": "' + (target.replace(/\\/gi, '/') + '/' + clipboard.items[index].Data.Path.replace(/\\/gi, "/").substr(clipboard.items[index].Data.Path.replace(/\\/gi, "/").lastIndexOf('/')).replace(/\//gi, '')) + '", "Overwrite": "' + overwrite + '" }',
				contentType: 'application/json',
				success: function (data) {
					temp.index++;
					$("#progressstatus").dialog("option", "title", hap.common.getLocal("myfiles/copy/copyingitem1") + " " + (temp.index + 1) + " " + hap.common.getLocal("of") + " " + clipboard.items.length + " " + hap.common.getLocal("items"));
					$("#progressstatus .progress").progressbar({ value: (temp.index / clipboard.items.length) * 100 });
					if (temp.index < clipboard.items.length) CopyClipboard(temp.index, temp.target);
					else { clipboard = null; temp = null; Load(); setTimeout(function() { $("#progressstatus").dialog("close"); }, 500); }
				},
				error: function (xhr, ajaxOptions, thrownError) {
				    if (jQuery.parseJSON(xhr.responseText).Message == "Destination Folder Exists")
				    {
				        if (confirm(hap.common.getLocal("myfiles/folderexists1") + " " + clipboard.items[temp.index].Data.Name + " " + hap.common.getLocal("myfiles/folderexists2") + "\n" + hap.common.getLocal("myfiles/merge")))
				            CopyClipboard(temp.index, temp.target, true);
				        else {
				            temp.index++;
				            $("#progressstatus").dialog("option", "title", hap.common.getLocal("myfiles/copy/copyingitem1") + " " + (temp.index + 1) + " " + hap.common.getLocal("of") + " " + clipboard.items.length + " " + hap.common.getLocal("items"));
				            $("#progressstatus .progress").progressbar({ value: (temp.index / clipboard.items.length) * 100 });
				            if (temp.index < clipboard.items.length) CopyClipboard(temp.index, temp.target);
				            else { temp = null; Load(); setTimeout(function() { $("#progressstatus").dialog("close"); }, 500); }
				        }
				    }
				    else if (jQuery.parseJSON(xhr.responseText).Message == "File Exists in Destination")
				    {
				        if (confirm(hap.common.getLocal("myfiles/upload/fileexists1") + " " + clipboard.items[temp.index].Data.Name + " " + hap.common.getLocal("myfiles/upload/fileexists2")))
				            CopyClipboard(temp.index, temp.target, true);
				        else {
				            temp.index++;
				            $("#progressstatus").dialog("option", "title", hap.common.getLocal("myfiles/copy/copyingitem1") + " " + (temp.index + 1) + " " + hap.common.getLocal("of") + " " + clipboard.items.length + " " + hap.common.getLocal("items"));
				            $("#progressstatus .progress").progressbar({ value: (temp.index / clipboard.items.length) * 100 });
				            if (temp.index < clipboard.items.length) CopyClipboard(temp.index, temp.target);
				            else { temp = null; Load(); setTimeout(function() { $("#progressstatus").dialog("close"); }, 500); }
				        }
				    }
				    else if (confirm(hap.common.getLocal("myfiles/copy/error1") + " " + clipboard.items.Data.Name + ", " + hap.common.getLocal("myfiles/copy/error2") + "\n\n" + hap.common.getLocal("errordetails") + ":\n\n" + xhr.responseXML.documentElement.children[1].children[0].textContent)) {
				        temp.index++;
				        $("#progressstatus").dialog("option", "title", hap.common.getLocal("myfiles/copy/copyingitem1") + " " + (temp.index + 1) + " " + hap.common.getLocal("of") + " " + clipboard.items.length + " " + hap.common.getLocal("items"));
				        $("#progressstatus .progress").progressbar({ value: (temp.index / clipboard.items.length) * 100 });
				        if (temp.index < clipboard.items.length) CopyClipboard(temp.index, temp.target);
				        else { clipboard = null; temp = null; Load(); setTimeout(function() { $("#progressstatus").dialog("close"); }, 500); }
				    } else { hap.common.jsonError(xhr, ajaxOptions, thrownError); clipboard = null; temp = null; Load(); setTimeout(function() { $("#progressstatus").dialog("close"); }, 500); }
				}
			});
		}
		function Move(index, target, overwrite) {
			temp = { "index": index, "target": target };
			if (overwrite == null) overwrite = false;
			var a = '"' + SelectedItems()[index].Data.Path + '"';
			$.ajax({
				type: 'POST',
				url: hap.common.formatJSONUrl('~/api/MyFiles/Move'),
				dataType: 'json', 
				data: '{ "OldPath" : "' + SelectedItems()[index].Data.Path.replace(/\.\.\/download\//gi, "").replace(/\\/gi, "/") + '", "NewPath": "' + (target.replace(/\\/gi, "/") + '/' + SelectedItems()[index].Data.Path.replace(/\\/gi, "/").substr(SelectedItems()[index].Data.Path.replace(/\\/gi, "/").lastIndexOf('/')).replace(/\//gi, "")) + '", "Overwrite": "' + overwrite + '" }',
				contentType: 'application/json',
				success: function (data) {
					temp.index++;
					$("#progressstatus").dialog("option", "title", hap.common.getLocal("myfiles/move/movingitem1") + " " + (temp.index + 1) + " " + hap.common.getLocal("of") + " " + SelectedItems().length + " " + hap.common.getLocal("items"));
					$("#progressstatus .progress").progressbar({ value: (temp.index / SelectedItems().length) * 100 });
					if (temp.index < SelectedItems().length) Move(temp.index, temp.target);
					else { temp = null; Load(); setTimeout(function() { $("#progressstatus").dialog("close"); }, 500); }
				},
				error: function (xhr, ajaxOptions, thrownError) {
				    if (jQuery.parseJSON(xhr.responseText).Message == "Destination Folder Exists")
				        {
				            if (confirm(hap.common.getLocal("myfiles/folderexists1") + " " + SelectedItems()[temp.index].Data.Name + " " + hap.common.getLocal("myfiles/folderexists2") + "\n" + hap.common.getLocal("myfiles/merge")))
				                Move(temp.index, temp.target, true);
				            else {
				                temp.index++;
				                $("#progressstatus").dialog("option", "title", hap.common.getLocal("myfiles/copy/copyingitem1") + " " + (temp.index + 1) + " " + hap.common.getLocal("of") + " " + SelectedItems().length + " " + hap.common.getLocal("items"));
				                $("#progressstatus .progress").progressbar({ value: (temp.index / SelectedItems().length) * 100 });
				                if (temp.index < SelectedItems().length) Move(temp.index, temp.target);
				                else { temp = null; Load(); setTimeout(function() { $("#progressstatus").dialog("close"); }, 500); }
				            }
				        }
				    else if (jQuery.parseJSON(xhr.responseText).Message == "File Exists in Destination")
				        {
				            if (confirm(hap.common.getLocal("myfiles/fileexists1") + " " + SelectedItems()[temp.index].Data.Name + " " + hap.common.getLocal("myfiles/fileexists2")))
				                Move(temp.index, temp.target, true);
				            else {
				                temp.index++;
				                $("#progressstatus").dialog("option", "title", hap.common.getLocal("myfiles/copy/copyingitem1") + " " + (temp.index + 1) + " " + hap.common.getLocal("of") + " " + SelectedItems().length + " " + hap.common.getLocal("items"));
				                $("#progressstatus .progress").progressbar({ value: (temp.index / SelectedItems().length) * 100 });
				                if (temp.index < SelectedItems().length) Move(temp.index, temp.target);
				                else { temp = null; Load(); setTimeout(function() { $("#progressstatus").dialog("close"); }, 500); }
				            }
				        }
				    else if (confirm(hap.common.getLocal("myfiles/move/error1") + " " + SelectedItems()[temp.index].Data.Name + ", " + hap.common.getLocal("myfiles/move/error2") + "\n\n" + hap.common.getLocal("errordetails") + ":\n\n" + xhr.responseXML.documentElement.children[1].children[0].textContent)) {
				        temp.index++;
				        $("#progressstatus").dialog("option", "title", hap.common.getLocal("myfiles/move/movingitem1") + " " + (temp.index + 1) + " " + hap.common.getLocal("of") + " " + SelectedItems().length + " " + hap.common.getLocal("items"));
				        $("#progressstatus .progress").progressbar({ value: (temp.index / SelectedItems().length) * 100 });
				        if (temp.index < SelectedItems().length) Move(temp.index, temp.target);
				        else { temp = null; Load(); setTimeout(function() { $("#progressstatus").dialog("close"); }, 500); }
				    } else { hap.common.jsonError(xhr, ajaxOptions, thrownError); temp = null; Load(); setTimeout(function() { $("#progressstatus").dialog("close"); }, 500); }
				}
			});
		}
		function MoveClipboard(index, target, overwrite) {
			temp = { "index": index, "target": target };
			if (overwrite == null) overwrite = false;
			var a = '"' + clipboard.items[index].Data.Path + '"';
			$.ajax({
				type: 'POST',
				url: hap.common.formatJSONUrl('~/api/MyFiles/Move'),
				dataType: 'json',
				data: ('{ "OldPath" : "' + clipboard.items[index].Data.Path.replace(/\.\.\/download\//gi, "") + '", "NewPath": "' + (target + '\\' + clipboard.items[index].Data.Path.substr(clipboard.items[index].Data.Path.lastIndexOf('/'))) + '", "Overwrite": "' + overwrite + '" }').replace(/\\/gi, "/"),
				contentType: 'application/json',
				success: function (data) {
					temp.index++;
					$("#progressstatus").dialog("option", "title", hap.common.getLocal("myfiles/move/movingitem1") + " " + (temp.index + 1) + " " + hap.common.getLocal("of") + " " + clipboard.items.length + " " + hap.common.getLocal("items"));
					$("#progressstatus .progress").progressbar({ value: (temp.index / clipboard.items.length) * 100 });
					if (temp.index < clipboard.items.length) Move(temp.index, temp.target);
					else { clipboard = null; temp = null; Load(); setTimeout(function() { $("#progressstatus").dialog("close"); }, 500); }
				},
				error: function (xhr, ajaxOptions, thrownError) {
				    if (jQuery.parseJSON(xhr.responseText).Message == "Destination Folder Exists")
					{
						if (confirm(hap.common.getLocal("myfiles/folderexists1") + " " + clipboard.items[temp.index].Data.Name + " " + hap.common.getLocal("myfiles/folderexists2") + "\n" + hap.common.getLocal("myfiles/merge")))
							MoveClipboard(temp.index, temp.target, true);
						else {
							temp.index++;
							$("#progressstatus").dialog("option", "title", hap.common.getLocal("myfiles/move/movingitem1") + " " + (temp.index + 1) + " " + hap.common.getLocal("of") + " " + clipboard.items.length + " " + hap.common.getLocal("items"));
							$("#progressstatus .progress").progressbar({ value: (temp.index / clipboard.items.length) * 100 });
							if (temp.index < clipboard.items.length) MoveClipboard(temp.index, temp.target);
							else { temp = null; Load(); setTimeout(function() { $("#progressstatus").dialog("close"); }, 500); }
						}
					}
				    else if (jQuery.parseJSON(xhr.responseText).Message == "File Exists in Destination")
					{
						if (confirm(hap.common.getLocal("myfiles/fileexists1") + " " + clipboard.items[temp.index].Data.Name + " " + hap.common.getLocal("myfiles/fileexists2")))
							MoveClipboard(temp.index, temp.target, true);
						else {
							temp.index++;
							$("#progressstatus").dialog("option", "title", hap.common.getLocal("myfiles/move/movingitem1") + " " + (temp.index + 1) + " " + hap.common.getLocal("of") + " " + clipboard.items.length + " " + hap.common.getLocal("items"));
							$("#progressstatus .progress").progressbar({ value: (temp.index / clipboard.items.length) * 100 });
							if (temp.index < clipboard.items.length) MoveClipboard(temp.index, temp.target);
							else { temp = null; Load(); setTimeout(function() { $("#progressstatus").dialog("close"); }, 500); }
						}
					}
					else if (confirm(hap.common.getLocal("myfiles/move/error1") + " " + clipboard.items[temp.index].Data.Name + ", " + hap.common.getLocal("myfiles/move/error2") + "\n\n" + hap.common.getLocal("errordetails") + ":\n\n" + xhr.responseXML.documentElement.children[1].children[0].textContent)) {
						temp.index++;
						$("#progressstatus").dialog("option", "title", hap.common.getLocal("myfiles/move/movingitem1") + " " + (temp.index + 1) + " " + hap.common.getLocal("of") + " " + clipboard.items.length + " " + hap.common.getLocal("items"));
						$("#progressstatus .progress").progressbar({ value: (temp.index / clipboard.items.length) * 100 });
						if (temp.index < clipboard.items.length) Move(temp.index, temp.target);
						else { clipboard = null; temp = null; Load(); setTimeout(function() { $("#progressstatus").dialog("close"); }, 500); }
					} else { hap.common.jsonError(xhr, ajaxOptions, thrownError); clipboard = null; temp = null; Load(); setTimeout(function() { $("#progressstatus").dialog("close"); }, 500); }
				}
			});
		}
		function Delete(index) {
			temp = index;
			var a = '"' + SelectedItems()[index].Data.Path.replace(/\.\.\/download\//gi, "").replace(/\\/g, "/") + '"';
			$.ajax({
				type: 'POST',
				url: hap.common.formatJSONUrl('~/api/MyFiles/Delete'),
				dataType: 'json',
				data: '[' + a + ']',
				contentType: 'application/json',
				success: function (data) {
				    if (data[0].match(/i could not delete/gi)) {
				        $('#hapContent').append('<div title="Alert" id="hapWarning">' + data[0] + '</div>');
				        $("#hapWarning").hapPopup({
				            buttons: [
                                { Text: "Ok", Click: function () { $(this).parents(".hapPopup").remove(); return false; } }
				            ]
				        });
				    }
					temp++;
					if (temp < SelectedItems().length) {
					    $("#progressstatus").dialog("option", "title", hap.common.getLocal("myfiles/delete/deletingitem1") + " " + (temp + 1) + " " + hap.common.getLocal("of") + " " + SelectedItems().length + " " + hap.common.getLocal("items"));
					    $("#progressstatus .progress").progressbar({ value: (temp / SelectedItems().length) * 100 });
					}
					if (temp < SelectedItems().length) Delete(temp);
					else { temp = null; Load(); setTimeout(function() { $("#progressstatus").dialog("close"); }, 500); }
				},
				error: hap.common.jsonError
			});
		}
		function Upload(file, path) {
			this.File = file;
			this.Path = path;
			this.FileName = "";
			this.FormatFileName = function() {
			    return this.File.name.substr(0, this.File.name.lastIndexOf('.')) + Math.floor((Math.random()*100)+1) + this.File.name.substr(this.File.name.lastIndexOf('.'));
			};
			this.Start = function() {
			    this.FileName = ($("#uploadfilesrandom").is(":checked") ? this.FormatFileName() : this.File.name) + "";
			    if (this.File.name.indexOf('.') == -1) {
			        $('#hapContent').append('<div title="Warning" id="hapWarning">' + hap.common.getLocal("myfiles/upload/folderwarning").replace(/\%/g, this.FileName) + '</div>');
			        $("#hapWarning").hapPopup({
			            buttons: [
                            { Text: "Ok", Click: function () { $(this).parents(".hapPopup").remove(); return false; } }
			            ]
			        });
					uploads.pop(this);
					return false;
				}
				else if ("<%=AcceptedExtensions %>".toLowerCase().indexOf(this.FileName.substr(this.FileName.lastIndexOf('.')).toLowerCase()) == -1 && "<%=DropZoneAccepted %>" != "") {
				    $('#hapContent').append('<div title="Warning" id="hapWarning">' + this.File.name + " " + hap.common.getLocal("myfiles/upload/filetypewarning") + '<br /><br /><%=AcceptedExtensions%></div>');
				    $("#hapWarning").hapPopup({
				        buttons: [
                            { Text: "Ok", Click: function () { $(this).parents(".hapPopup").remove(); return false; } }
				        ]
				    });
				    uploads.pop(this);
					return false;
				}
			    if (this.File.size > parseInt('<%=maxRequestLength%>')) {
			        $('#hapContent').append('<div title="Warning" id="hapWarning">' + this.File.name + " " + hap.common.getLocal("myfiles/upload/filesizewarning") + '</div>');
			        $("#hapWarning").hapPopup({
			            buttons: [
                            { Text: "Ok", Click: function () { $(this).parents(".hapPopup").remove(); return false; } }
			            ]
			        });
					uploads.pop(this);
					return false;
				}
		        $("#progresses").append('<div id="upload-' + this.FileName.replace(/[\\'\. \[\]\(\)\-]/g, "_") + '"><div class="progressbar" style="display: inline-block; width: 100px; height: 20px; vertical-align: middle; overflow: hidden;"></div> ' + this.FileName + '</div>');
		        $("#upload-" + this.FileName.replace(/[\\'\. \[\]\(\)\-]/g, "_") + " .progressbar").progressbar({ value: 0 });
				$.ajax({
					type: 'GET',
					url: hap.common.formatJSONUrl('~/api/MyFiles/Exists/' + this.Path.replace(/\\/gi, "/").replace(/\.\.\/Download\//gi, "") + '/' + this.FileName),
					dataType: 'json',
					context: this,
					contentType: 'application/json',
					success: function (data) {
					    if (data.Name == null || confirm(hap.common.getLocal("myfiles/upload/fileexists1") + " " + this.FileName + " " + hap.common.getLocal("myfiles/upload/fileexists2"))) this.ContinueUpload(this.FileName);
						else { 
					        $("#upload-" + this.FileName.replace(/[\\'\. \[\]\(\)\-]/g, "_")).remove();
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
			    $("#upload-" + id + " .progress").progressbar({ value: 0 });
				this.xhr = new XMLHttpRequest();
				this.xhr.id = id;
				this.xhr.upload.addEventListener("progress", this.onProgress, false);
				this.xhr.addEventListener("progress", this.onProgress, false);
				this.xhr.onprogress = this.onProgress;
				this.xhr.open('POST', hap.common.resolveUrl('~/api/myfiles-upload/') + this.Path.replace(/\\/g, '/') + '/', true);
				this.xhr.onreadystatechange = function () {
					if (this.readyState == 4) {
					    var uitem = null;
					    for (var i = 0; i < uploads.length; i ++) if (uploads[i].FileName.replace(/[\\'\. \[\]\(\)\-]/g, "_") == this.id) uitem = uploads[i];
					    if (this.status != 200) {
					        $('#hapContent').append('<div title="' + hap.common.getLocal("myfiles/upload/upload") + " " + hap.common.getLocal("of") + " " + uitem.FileName + " " + hap.common.getLocal("myfiles/upload/failed") + '" id="hapWarning">' + this.responseText.substr(this.responseText.indexOf('<title>') + 7, this.responseText.indexOf('</title>') - (7 + this.responseText.indexOf('<title>'))) + '</div>');
					        $("#hapWarning").hapPopup({
					            buttons: [
                                    { Text: "Ok", Click: function () { $(this).parents(".hapPopup").remove(); return false; } }
					            ]
					        });
					    }
						$("#upload-" + id + " .progressbar").progressbar({ value: 100 });
						$("#upload-" + id).delay(1000).slideUp('slow', function() { $("#upload-" + id).remove(); if (uploads.length == 0) $("#uploadprogress").slideUp('slow'); });
						if (curpath.substr(0, curpath.length - 1).replace(/\//g, "\\") == uitem.Path || curpath.replace(/\//g, "\\") == uitem.Path) Load();
						uploads.pop(uitem);
					}
				};
				this.xhr.setRequestHeader('X_FILENAME', this.FileName);
				this.xhr.send(this.File);
			};
			this.onProgress = function (e) {
				var percent = parseInt((e.loaded / e.total) * 100);
				for (var i = 0; i < uploads.length; i++) if (uploads[i].File.size == e.total) uploads[i].updateProgress(percent);
			};
			this.updateProgress = function (e) {
			    $("#upload-" + this.FileName.replace(/[\\'\. \[\]\(\)\-]/g, "_") + " .progressbar").progressbar({ value: e });
			};
		}
		function Drive(data) {
			this.Data = data;
			this.Id = "";
			this.Render = function () {
				this.Id = this.Data.Path.substr(0, 1);
				$("#MyFiles").append('<a id="' + this.Id + '" href="#' + this.Data.Path + '" class="Drive"><span class="icon">' + this.Data.Path.substr(0, 1) + '</span><span class="label">' + this.Data.Name + '</span>' + (this.Data.Space < 0 ? '' : '<span class="progress"><label>' + this.Data.Space + '%</label><i style="width: ' + this.Data.Space + '%"></i></span>') + '</a>');
			};
			this.Refresh = function () {
				$("#" + this.Id).attr("href", "#" + this.Data.Path).html('<span class="icon">' + this.Data.Path.substr(0, 1) + '</span><span class="label">' + this.Data.Name + '</span>' + (this.Data.Space < 0 ? '' : '<span class="progress"><label>' + this.Data.Space + '%</label><i style="width: ' + this.Data.Space + '%"></i></span>'));
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
		function RefreshToolbar() {
			var cut, copy, paste, download, open, unzip, zip;
			zip = cut = copy = del = open = SelectedItems().length > 0;
			paste = curitem.Permissions.CreateFiles && clipboard != null;
			zip = zip ? curitem.Actions == 0 : zip;
			zip = zip ? curitem.Permissions.CreateFiles : zip;
			unzip = download = open = SelectedItems().length == 1;
			unzip = unzip ? curitem.Actions == 0 : unzip;
			unzip = unzip ? curitem.Permissions.CreateDirs : unzip;
			for (var i = 0; i < SelectedItems().length; i++) {
				var item = SelectedItems()[i];
				if (cut && item.Data.Permissions.DeleteSubDirsOrFiles) cut = true;
				else cut = false;
				if (copy && item.Data.Permissions.ReadData) copy = true;
				else copy = false;
				if (item.Data.Type == "Directory" || item.Data.Permissions.Execute == false || item.Data.Permissions.ReadData == false) download = false;
				if (item.Data.Traverse == false || (item.Data.Type != "Directory" && item.Data.Extension.toLowerCase() != ".zip")) open = false;
				if (item.Data.Path.match(/\.zip/gi) && unzip && curitem.Permissions.CreateDirs && curitem.Actions == 0) unzip = true;
				else unzip = false;
				if (del && item.Data.Permissions.DeleteSubDirsOrFiles && curitem.Actions == 0) del = true;
				else del = false;
			}
			if (curpath.match(/\.zip/gi)) { cut = copy = pase = del = download = zip = unzip = false; }
			if (cut && $("#toolbar-cut").css("display") == "none") { $("#toolbar-cut").css("display", "").animate({ width: 22 }); $("#toolbar-delete").css("display", "").animate({ width: 43 }); }
			if (!cut && $("#toolbar-cut").css("display") != "none") { $("#toolbar-cut").animate({ width: 0 }, { duration: 500, complete: function() { $("#toolbar-cut").css("display", "none") } }); $("#toolbar-delete").animate({ width: 0 }, { duration: 500, complete: function() { $("#toolbar-delete").css("display", "none") } }); }
			if (copy && $("#toolbar-copy").css("display") == "none") $("#toolbar-copy").css("display", "").animate({ width: 35 });
			if (!copy && $("#toolbar-copy").css("display") != "none") $("#toolbar-copy").animate({ width: 0 }, { duration: 500, complete: function() { $("#toolbar-copy").css("display", "none") } });
			if (paste && $("#toolbar-paste").css("display") == "none") { $("#toolbar-paste-container").show(); $("#toolbar-paste").css("display", "").animate({ width: 38 }); $("#toolbar-clear").css("display", "").animate({ width: 10 }); }
			if (!paste && $("#toolbar-paste").css("display") != "none") { $("#toolbar-paste").animate({ width: 0 }, { duration: 500, complete: function() { $("#toolbar-paste").css("display", "none"); $("#toolbar-paste-container").hide(); } }); $("#toolbar-clear").animate({ width: 0 }, { duration: 500, complete: function() { $("#toolbar-clear").css("display", "none") } }); }
			if (download && $("#toolbar-download").css("display") == "none") $("#toolbar-download").css("display", "").animate({ width: 64 });
			if (!download && $("#toolbar-download").css("display") != "none") $("#toolbar-download").animate({ width: 0 }, { duration: 500, complete: function() { $("#toolbar-download").css("display", "none") } });
			if (open && $("#toolbar-open").css("display") == "none") $("#toolbar-open").css("display", "").animate({ width: 36 });
			if (!open && $("#toolbar-open").css("display") != "none") $("#toolbar-open").animate({ width: 0 }, { duration: 500, complete: function() { $("#toolbar-open").css("display", "none") } });
			if (unzip && $("#toolbar-unzip").css("display") == "none") $("#toolbar-unzip").css("display", "").animate({ width: 40 });
			if (!unzip && $("#toolbar-unzip").css("display") != "none") $("#toolbar-unzip").animate({ width: 0 }, { duration: 500, complete: function() { $("#toolbar-unzip").css("display", "none") } });
			if (zip && $("#toolbar-zip").css("display") == "none") $("#toolbar-zip").css("display", "").animate({ width: 22 });
			if (!zip && $("#toolbar-zip").css("display") != "none") $("#toolbar-zip").animate({ width: 0 }, { duration: 500, complete: function () { $("#toolbar-zip").css("display", "none") } });
			$(window).trigger("resize");
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
				h += 'href="' + (this.Data.Path.match(/\.\./i) ? this.Data.Path.replace(/\\/g, "/") : '#' + this.Data.Path) + '"><img class="icon" src="' + this.Data.Icon + '" alt="" /><span class="label">' + label + '</span><span class="modified">' + this.Data.ModifiedTime + '</span><span class="type">';
				if (this.Data.Type == 'Directory') h += hap.common.getLocal("myfiles/filefolder");
				else h += this.Data.Type + '</span><span class="extension">' + this.Data.Extension + '</span><span class="size">' + this.Data.Size;
				h += '</span></a>';
				$("#MyFiles").append(h);
				if (this.Data.Permissions.CreateFiles && this.Data.Actions == 0) $("#" + this.Id).draggable({ helper: function () { return $('<div id="dragobject"><img /><span></span></div>'); }, start: function (event, ui) {
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
				if (this.Data.Type == 'Directory' && this.Data.Permissions.CreateFiles && this.Data.Actions == 0) $("#" + this.Id).droppable({ accept: '.Selectable', activeClass: 'droppable-active', hoverClass: 'droppable-hover', drop: function (ev, ui) {
					var item = null;
					for (var x = 0; x < items.length; x++) if (items[x].Id == $(this).attr("id")) item = items[x];
					var s = "";
					for (var i = 0; i < SelectedItems().length; i++) s += SelectedItems()[i].Data.Name + "\n";
					$("#progressstatus").dialog({ autoOpen: true, modal: true, title: hap.common.getLocal("myfiles/" + ((keys.ctrl) ? "copy/copying" : "move/moving")) + " 1 " + hap.common.getLocal("of") + " " + SelectedItems().length + " " + hap.common.getLocal("items") });
					$("#progressstatus .progress").progressbar({ value: (1 / SelectedItems().length) * 100 });
					if (keys.ctrl) Copy(0, item.Data.Path);
					else if (confirm(hap.common.getLocal("myfiles/move/question1") + "\n\n" + s)) Move(0, item.Data.Path);
					else $("#progressstatus").dialog("close");
					keys.ctrl = keys.shift = false;
				}, over: function (event, ui) {
					var item = null;
					for (var x = 0; x < items.length; x++) if (items[x].Id == $(this).attr("id")) item = items[x];
					$("#dragobject span").text(hap.common.getLocal("myfiles/" + ((keys.ctrl) ? "copy/copy" : "move/move")) + " " + hap.common.getLocal("to") + " " + item.Data.Name).show();
				}, out: function (event, ui) {
					$("#dragobject span").text("").hide();
				}
				});
				if (typeof (window.FileReader) != 'undefined' && this.Data.Type == 'Directory' && this.Data.Permissions.CreateFiles && this.Data.Actions == 0) {
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
							$("#uploadprogress").show().slideDown('slow');
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
					    if (curitem.Permissions.DeleteSubDirsOrFiles == false || curitem.Actions > 0) $("#con-delete", menu).remove();
					    if (curitem.Permissions.CreateFiles == false || curitem.Actions > 0) { $("#con-rename", menu).remove(); $("#con-zip", menu).remove(); $("#con-unzip", menu).remove(); }
					    if (curitem.Actions == 3) { $("#con-download", menu).remove(); if (SelectedItems().length != 1 || SelectedItems()[0].Data.Type != 'Directory') $("#con-open", menu).remove(); $("#con-properties", menu).remove(); $("#con-zip", menu).remove(); $("#con-unzip", menu).remove(); }
					    if (SelectedItems().length > 1) { $("#con-unzip", menu).remove(); $("#con-download", menu).remove(); $("#con-open", menu).remove(); $("#con-rename", menu).remove(); $("#con-properties", menu).remove(); $("#con-preview", menu).remove(); $("#con-google", menu).remove(); $("#con-skydrive", menu).remove(); }
					    else {
					        if (!SelectedItems()[0].Data.Path.match(/\.zip/gi)) { $("#con-open", menu).remove(); }
					        var remgoogle = false;
					        if (SelectedItems()[0].Data.Extension != ".txt" && SelectedItems()[0].Data.Extension != ".xlsx" && SelectedItems()[0].Data.Extension != ".docx" && SelectedItems()[0].Data.Extension != ".xls" && SelectedItems()[0].Data.Extension != ".csv" && SelectedItems()[0].Data.Extension != ".png" && SelectedItems()[0].Data.Extension != ".gif" && SelectedItems()[0].Data.Extension != ".jpg" && SelectedItems()[0].Data.Extension != ".jpeg" && SelectedItems()[0].Data.Extension != ".bmp") {
					            $("#con-preview", menu).remove();
					            if (SelectedItems()[0].Data.Extension != ".ppt" && SelectedItems()[0].Data.Extension != ".pptx" && SelectedItems()[0].Data.Extension != ".pps" && SelectedItems()[0].Data.Extension != ".doc" && SelectedItems()[0].Data.Extension != ".rtf") $("#con-google", menu).remove();
					        }
					        if (SelectedItems()[0].Data.Extension != ".zip") $("#con-unzip", menu).remove();
					    }
					    return menu;
					},
					bindings: {
						'con-open': function (t) {
						    if (SelectedItems().length > 1) {
						        $('#hapContent').append('<div title="' + hap.common.getLocal("myfiles/only1") + '" id="hapWarning">' + hap.common.getLocal("myfiles/only1") + '</div>');
						        $("#hapWarning").hapPopup({
						            buttons: [
                                        { Text: "Ok", Click: function () { $(this).parents(".hapPopup").remove(); return false; } }
						            ]
						        });
						        return false;
						    }
							if (SelectedItems()[0].Data.Type == 'Directory') window.location.href = "#" + SelectedItems()[0].Data.Path;
							else if (SelectedItems()[0].Data.Extension == ".zip") window.location.href="#" + (curitem.Location.replace(/:/gi, "").replace(/\//gi, "/") + "\\").replace(/\\\\/gi, "\\") + SelectedItems()[0].Data.Name + ".zip";
							else window.location.href = SelectedItems()[0].Data.Path;
						},
						'con-download': function (t) {
						    if (SelectedItems().length > 1) {
						        $('#hapContent').append('<div title="Question" id="hapWarning">' + hap.common.getLocal("myfiles/only1") + '</div>');
						        $("#hapWarning").hapPopup({
						            buttons: [
                                        { Text: "Ok", Click: function () { $(this).parents(".hapPopup").remove(); return false; } }
						            ], item: item
						        });
						        return false;
						    }
							if (SelectedItems()[0].Data.Type == 'Directory') window.location.href = "#" + SelectedItems()[0].Data.Path;
							else {
							    if ($("#hapdirectedit").is(":checked") && SelectedItems()[0].Data.Extension.match(/(xls|doc|ppt)/gi)) {
							        $('#hapContent').append('<div title="Question" id="hapWarning">' + hap.common.getLocal("myfiles/directeditquestion") + '</div>');
							        $("#hapWarning").hapPopup({
							            buttons: [
                                            {
                                                Text: "Download", Click: function () {
                                                    $(this).parents(".hapPopup").remove();
                                                    $('#hapContent').append('<div title="Warning" id="hapWarning">' + hap.common.getLocal("myfiles/downloadwarning") + '</div>');
                                                    $("#hapWarning").hapPopup({
                                                        buttons: [
                                                            { Text: "Ok", Click: function () { $(this).parents(".hapPopup").remove(); window.open("" + (SelectedItems()[0].Data.Path.match(/\.\./i) ? SelectedItems()[0].Data.Path.replace(/\\/g, "/") : '#' + SelectedItems()[0].Data.Path)); return false; } },
                                                            { Text: "Cancel", Click: function () { $(this).parents(".hapPopup").remove(); return false; } }
                                                        ], item: SelectedItems()[0]
                                                    });
                                                    return false;
                                                }
                                            },
                                            { Text: "DirectEdit", Click: function () { $(this).parents(".hapPopup").remove(); location.href = "" + SelectedItems()[0].Data.Path.replace(/\.\.\/Download\//gi, "../myfiles/directedit/"); return false; } },
                                            { Text: "Cancel", Click: function () { $(this).parents(".hapPopup").remove(); return false; } }
							            ], item: SelectedItems()[0]
							        });
							    }
							    else {
							        $('#hapContent').append('<div title="Warning" id="hapWarning">' + hap.common.getLocal("myfiles/downloadwarning") + '</div>');
							        $("#hapWarning").hapPopup({
							            buttons: [
                                            { Text: "Ok", Click: function () { $(this).parents(".hapPopup").remove(); window.open("" + (SelectedItems()[0].Data.Path.match(/\.\./i) ? SelectedItems()[0].Data.Path.replace(/\\/g, "/") : '#' + SelectedItems()[0].Data.Path)); return false; } },
                                            { Text: "Cancel", Click: function () { $(this).parents(".hapPopup").remove(); return false; } }
							            ], item: SelectedItems()[0]
							        });
							    }
							}
						},
						'con-delete': function (t) {
							$("#progressstatus").dialog({ autoOpen: true, modal: true, title: hap.common.getLocal("myfiles/delete/deletingitem1") + " 1 " + hap.common.getLocal("of") + " " + SelectedItems().length + " " + hap.common.getLocal("items") });
							$("#progressstatus .progress").progressbar({ value: (1 / SelectedItems().length) * 100 });
							var s = "";
							for (var i = 0; i < SelectedItems().length; i++) s += SelectedItems()[i].Data.Name + "\n";
							if (confirm(hap.common.getLocal("myfiles/delete/question1") + "\n\n" + s)) Delete(0);
							else $("#progressstatus").dialog("close");
						},
						'con-rename': function (t) {
						    if (SelectedItems().length > 1) {
						        $('#hapContent').append('<div title="' + hap.common.getLocal("myfiles/only1") + '" id="hapWarning">' + hap.common.getLocal("myfiles/only1") + '</div>');
						        $("#hapWarning").hapPopup({
						            buttons: [
                                        { Text: "Ok", Click: function () { $(this).parents(".hapPopup").remove(); return false; } }
						            ]
						        });
						        return false;
						    }
							var item = SelectedItems()[0];
							$("#renamebox").val(item.Data.Name).css("display", "block").css("top", $("#" +item.Id).position().top).css("left", $("#" + item.Id).position().left).focus().select();
						},
						'con-properties': function (t) {
						    if (SelectedItems().length > 1) {
						        $('#hapContent').append('<div title="' + hap.common.getLocal("myfiles/only1") + '" id="hapWarning">' + hap.common.getLocal("myfiles/only1") + '</div>');
						        $("#hapWarning").hapPopup({
						            buttons: [
                                        { Text: "Ok", Click: function () { $(this).parents(".hapPopup").remove(); return false; } }
						            ]
						        });
						        return false;
						    }
							$("#properties").dialog({ autoOpen: true, modal: true, buttons: {
								"OK": function () {
									$(this).dialog("close");
									$("#propcont").html("Loading...");
								}
							}
							});
							$.ajax({
								type: 'GET',
								url: hap.common.formatJSONUrl('~/api/MyFiles/Properties/' + SelectedItems()[0].Data.Path.replace(/\\/gi, "/").replace(/\.\.\/Download\//gi, "")),
								dataType: 'json',
								contentType: 'application/json',
								success: function (data) {
									var s = '<div><img src="' + data.Icon + '" alt="" style="width: 32px; float: left; margin-right: 40px;" />' + ((data.Type == "File Folder") ? '<a href="' + hap.common.resolveUrl("~/api/myfiles-permalink/" + data.Location.replace(/\\/g, "/") + "/" + data.Name.replace(/&/g, "^")) + '">' + data.Name + '</a>' : data.Name) + '</div>';
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
										s += '<div><label>' + hap.common.getLocal("myfiles/location") + ': </label><a href="' + hap.common.resolveUrl("~/api/myfiles-permalink/" + data.Location.replace(/\\/g, "/") + "/") + '">' + data.Location + '</a></div>';
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
						    if (SelectedItems().length > 1) {
						        $('#hapContent').append('<div title="' + hap.common.getLocal("myfiles/only1") + '" id="hapWarning">' + hap.common.getLocal("myfiles/only1") + '</div>');
						        $("#hapWarning").hapPopup({
						            buttons: [
                                        { Text: "Ok", Click: function () { $(this).parents(".hapPopup").remove(); return false; } }
						            ]
						        });
						        return false;
						    }
							$("#preview").dialog({ autoOpen: true, height: 600, width: 900, modal: true, buttons: {
								"OK": function () {
									$("#previewcont").html(hap.common.getLocal("loading") + "...");
									$(this).dialog("close");
								}
							}
							});
							$.ajax({
								type: 'GET',
								url: hap.common.formatJSONUrl('~/api/MyFiles/Preview/' + SelectedItems()[0].Data.Path.replace(/\\/gi, "/").replace(/\.\.\/Download\//gi, "")),
								dataType: 'json',
								contentType: 'application/json',
								success: function (data) {
									$("#previewcont").html(data);
								}, error: hap.common.jsonError
							});
						},
						'con-unzip' : function (t) {
							$("#progressstatus").dialog({ autoOpen: true, modal: true, title: hap.common.getLocal("myfiles/unzip/unzipping") + ": " + SelectedItems()[0].Data.Name });
							$("#progressstatus .progress").progressbar({ value: 10 });
							UnZip();
						},
						'con-zip' : function (t) {
							$("#zipquestion").dialog({ autoOpen: true, modal: true, buttons: { 
								"ZIP": function() { 
									$(this).dialog("close");
									$("#progressstatus").dialog({ autoOpen: true, modal: true, title: hap.common.getLocal("myfiles/zip/zipingitem1") + " 1 " + hap.common.getLocal("of") + " " + SelectedItems().length + " " + hap.common.getLocal("items") });
									$("#progressstatus .progress").progressbar({ value: 1 });
									Zip(curpath + "\\" + $("#zipfilename").val() + ".zip", 0);
								}, "Close": function() { $(this).dialog("close"); } } 
							}); 
							$("#zipfilename").val(SelectedItems()[0].Data.Name).focus();
						},
						'con-google' : function (t) {
						    if (SelectedItems().length > 1) {
						        $('#hapContent').append('<div title="' + hap.common.getLocal("myfiles/only1") + '" id="hapWarning">' + hap.common.getLocal("myfiles/only1") + '</div>');
						        $("#hapWarning").hapPopup({
						            buttons: [
                                        { Text: "Ok", Click: function () { $(this).parents(".hapPopup").remove(); return false; } }
						            ]
						        });
                                return false;
						    }
							$("#googlesignin").dialog({ autoOpen: true, modal: true, buttons: { 
								"Signin": function() { 
									$("#googleuser").addClass("loading");
									$("#googlepass").addClass("loading");
									$("#googlesignin .progress").height(16).width(16).addClass("loading");
									$.ajax({
										type: 'POST',
										url: hap.common.formatJSONUrl('~/api/MyFiles/SendTo/Google/' + SelectedItems()[0].Data.Path.replace(/\\/gi, "/").replace(/\.\.\/Download\//gi, "")),
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
						},
						'con-skydrive' : function (t) {
						    if (SelectedItems().length > 1) {
						        $('#hapContent').append('<div title="' + hap.common.getLocal("myfiles/only1") + '" id="hapWarning">' + hap.common.getLocal("myfiles/only1") + '</div>');
						        $("#hapWarning").hapPopup({
						            buttons: [
                                        { Text: "Ok", Click: function () { $(this).parents(".hapPopup").remove(); return false; } }
						            ]
						        });
						        return false;
						    }
						    WL.init({ client_id: '<%=config.MyFiles.LiveAppId%>', redirect_uri: hap.common.resolveUrl("~/myfiles/oauth.aspx"), scope: 'wl.skydrive_update', response_type: 'token' });
						    WL.login({scope: 'wl.skydrive_update' }, function () { 
						        $("#loadingbox").dialog({ autoOpen: true, modal: true });
						        $.ajax({
						            type: 'POST',
						            url: hap.common.formatJSONUrl('~/api/MyFiles/SendTo/SkyDrive/' + SelectedItems()[0].Data.Path.replace(/\\/gi, "/").replace(/\.\.\/Download\//gi, "")),
						            dataType: 'json',
						            data: '{ "accessToken" : "' + WL.getSession().access_token + '" }',
						            contentType: 'application/json',
						            success: function (data) {
						                $("#loadingbox").dialog("close");
						            },
						            error: hap.common.jsonError
                                });
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

				var item = null;
				for (var x = 0; x < items.length; x++)
				    if (items[x].Id == $(this).attr("id")) { item = items[x]; break; }

				if (item.Data.Type != 'Directory') {
				    if ($("#hapdirectedit").is(":checked") && item.Data.Extension.match(/(xls|doc|ppt)/gi)) {
				        $('#hapContent').append('<div title="Question" id="hapWarning">' + hap.common.getLocal("myfiles/directeditquestion") + '</div>');
				        $("#hapWarning").hapPopup({
				            buttons: [
                                {
                                    Text: "Download", Click: function () {
                                        $(this).parents(".hapPopup").remove();
                                        $('#hapContent').append('<div title="Warning" id="hapWarning">' + hap.common.getLocal("myfiles/downloadwarning") + '</div>');
                                        $("#hapWarning").hapPopup({
                                            buttons: [
                                                { Text: "Ok", Click: function () { $(this).parents(".hapPopup").remove(); window.open("" + (item.Data.Path.match(/\.\./i) ? item.Data.Path.replace(/\\/g, "/") : '#' + item.Data.Path)); return false; } },
                                                { Text: "Cancel", Click: function () { $(this).parents(".hapPopup").remove(); return false; } }
                                            ], item: item
                                        });
                                        return false;
                                    }
                                },
				                { Text: "DirectEdit", Click: function () { $(this).parents(".hapPopup").remove(); location.href = "" + item.Data.Path.replace(/\.\.\/Download\//gi, "../myfiles/directedit/"); return false; } },
                                { Text: "Cancel", Click: function () { $(this).parents(".hapPopup").remove(); return false; } }
				            ], item: item
				        });
				    }
				    else {
				        $('#hapContent').append('<div title="Warning" id="hapWarning">' + hap.common.getLocal("myfiles/downloadwarning") + '</div>');
				        $("#hapWarning").hapPopup({
				            buttons: [
                                { Text: "Ok", Click: function () { $(this).parents(".hapPopup").remove(); window.open("" + (item.Data.Path.match(/\.\./i) ? item.Data.Path.replace(/\\/g, "/") : '#' + item.Data.Path)); return false; } },
                                { Text: "Cancel", Click: function () { $(this).parents(".hapPopup").remove(); return false; } }
				            ], item: item
				        });
				    }
				}
				else window.location.href = (item.Data.Path.match(/\.\./i) ? item.Data.Path.replace(/\\/g, "/") : '#' + item.Data.Path);
			};
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
					RefreshToolbar();
				} else {

				}
				return false;
			};
		}
	    function Load(sortorder, sortdirection) {
	        Sortorder = sortorder;
	        Sortdirection = sortorder == null ? 1 : sortdirection;
	        var sort = (sortorder == null ? '' : ('Sort/' + sortorder + '/' + sortdirection + '/'));
			$(".context-menu").remove();
			if (typeof (window.FileReader) != 'undefined') $("#MyFiles").unbind("dragover").unbind("dragleave").unbind("dragend").unbind("drop");
			if (curpath == null) {
				$.ajax({
					type: 'GET',
					url: hap.common.formatJSONUrl('~/api/MyFiles/Drives'),
					dataType: 'json',
					contentType: 'application/json;',
					success: function (data) {
						items = new Array();
						$("#MyFiles").html("");
						$(window).trigger("resize");
						for (var i = 0; i < data.length; i++)
							items.push(new Drive(data[i]));
						for (var i = 0; i < items.length; i++)
							items[i].Render();
					}, error: hap.common.jsonError
				});
			} else {
			$.ajax({
				type: 'GET',
				url: hap.common.formatJSONUrl("~/api/MyFiles/" + sort + curpath.replace(/\\/gi, "/")),
				dataType: 'json',
				contentType: 'application/json',
				success: function (data) {
					items = new Array();
					$("#MyFiles").html("");
					for (var i = 0; i < data.length; i++)
						items.push(new Item(data[i]));
					for (var i = 0; i < items.length; i++) items[i].Render();
					$(window).trigger("resize");
					$("#toolbar").slideDown();
					$("#search input").fadeIn();
					$("#MyFilesHeaddings .name").css("width", $("#MyFiles > a .label").width() + $("#MyFiles > a img").width() + 4);
					$("#MyFilesHeaddings .type").css("width", $("#MyFiles > a .type").width() + 2);
					$("#MyFilesHeaddings .extension").css("width", $("#MyFiles > a .extension").width() + 2);
					$("#MyFilesHeaddings .modified").css("width", $("#MyFiles > a .modified").width() + 2);
					$("#MyFilesHeaddings .size").css("width", $("#MyFiles > a .size").width() + 2);
				}, error: hap.common.jsonError
			});
			$.ajax({
				type: 'GET',
				url: hap.common.formatJSONUrl("~/api/MyFiles/info/" + curpath.replace(/\\/gi, "/")),
				dataType: 'json',
				contentType: 'application/json',
				success: function (data) {
				    curitem = data;
				    $("#filter").val(hap.common.getLocal("search") + ": " + curitem.Name);
				    if (curitem.Permissions.CreateDirs) {
				        $("#newfolder").animate({ opacity: 1.0 }, 500, function () { $("#newfolder").show(); });
				        $("#newfoldertext").blur();
				    }
				    else {
				        $("#newfolder").animate({ opacity: 0 }, 500, function () { $("#newfolder").hide(); });
				        $("#newfoldertext").blur();
				    }
				    if (curitem.Permissions.CreateFiles)
                        $("#upload").animate({ opacity: 1.0 }, 500, function () { $("#upload").show(); });
                    else $("#upload").animate({ opacity: 0 }, 500, function () { $("#upload").hide(); });
				    if (typeof (window.FileReader) != 'undefined' && curitem.Permissions.CreateFiles) {
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
								$("#uploadprogress").show().slideDown('slow');
								for (var i = 0; i < (event.target.files || event.dataTransfer.files).length; i++) {
									var file = new Upload((event.target.files || event.dataTransfer.files)[i], (curpath.length == 2 ? curitem.Location.substr(0, curitem.Location.length -1 ) : curitem.Location).replace(/:/g, ""));
									uploads.push(file);
									file.Start();
								}
								$("#uploadto").text("");
							}
						};
					}
					setTimeout(function () { RefreshToolbar(); }, 500);
				}, error: hap.common.jsonError
			});
			}
		}
		function searchres(data) {
			var re = new RegExp("(" + $("#filter").val().replace(/\*/g, ")(.*)(").replace(/ /g, ")(.*)(").replace(/\(\)/g, "") + ")", "i");
			return data.match(re);
		}
		var directedit = false;
		$(function () {
		    $(window).resize(function () {
		        $("#Tree").css("top", $("#search").position().top + $("#search").height()).css("height", $("#hapContent").height() - $("#search").height()); 
		        $("#MyFiles").css("min-height", parseInt($("#hapContent").css("min-height").replace(/px/g, "")) - 2); 
		        $("#Views,#directedit").css("left", $('#view').position().left);
		    });
		    if (navigator.appVersion.indexOf("Win") == -1) {
		        $("#de").remove();
		    }
		    $("#properties").dialog({ autoOpen: false });
		    $("#loadingbox").dialog({ autoOpen: false });
			$("#preview").dialog({ autoOpen: false });
			$("#zipquestion").dialog({ autoOpen: false });
			$("#progressstate").dialog({ autoOpen: false });
			$("#uploaders").dialog({ autoOpen: false });
			$("#googlesignin").dialog({ autoOpen: false });
			$("#Views,#directedit").animate({ height: 'toggle' });
			$("#Tree").dynatree({ imagePath: "../images/setup/", debugLevel: 0, selectMode: 1, minExpandLevel: 1, noLink: false, children: [{ icon: "../myfiles-i.png", title: hap.common.getLocal("myfiles/mydrives"), href: "#", isFolder: true, isLazy: true}], fx: { height: "toggle", duration: 200 },
				onLazyRead: function (node) {
					if (node.data.href == "#") {
						$.ajax({
							type: 'GET',
							url: hap.common.formatJSONUrl("~/api/MyFiles/Drives"),
							dataType: 'json',
							contentType: 'application/json',
							success: function (data) {
								res = [];
								for (var i = 0; i < data.length; i++)
									res.push({ title: data[i].Name + " (" + data[i].Path.substr(0, 1) + ")", actions: data[i].Actions, icon: "../drive.png", href: "#" + data[i].Path, isFolder: true, isLazy: true, noLink: false, key: data[i].Path });
								node.setLazyNodeStatus(DTNodeStatus_Ok);
								node.addChild(res);
							}, error: hap.common.jsonError
						});
					} else {
						$.ajax({
							type: 'GET',
							url: hap.common.formatJSONUrl("~/api/MyFiles/" + node.data.href.substr(1).replace(/\\/g, "/")),
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
										$("#uploadprogress").show().slideDown('slow');
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
			}).dynatree("getRoot").visit(function(node){ node.expand(true); });
			$("#filter").val(hap.common.getLocal("search"));
			$("button").button().click(function () { return false; });
			$("button.dropdown").button({ icons: { secondary: "ui-icon-carat-1-s"} });
			$("#backup").click(function () { history.go(-1); return false; });
			$("#newfolder").click(function () {
				if ($("#newfolder").text() == hap.common.getLocal("myfiles/newfolder")) {
					$("#newfolder").text(hap.common.getLocal("myfiles/create"));
					$("#newfoldertext").val("").show().animate({ width: 150, opacity: 1.0 }).focus();
				} else {
					if (temp != null) { clearTimeout(temp); temp == null; }
					$("#newfoldertext").addClass("loading");
					$.ajax({
						type: 'GET',
						url: hap.common.formatJSONUrl("~/api/MyFiles/Exists/" + curpath.replace(/\\/gi, "/") + (curpath.replace(/\\/gi, "/").match(/\\$/g) ? '' : '/') + $("#newfoldertext").val() + '/').replace(/\/\//gi, "/"),
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
									url: hap.common.formatJSONUrl("~/api/MyFiles/New/" + curpath.replace(/\\/gi, "/") + (curpath.replace(/\\/gi, "/").match(/\\$/g) ? '' : '/') + $("#newfoldertext").val()).replace(/\/\//gi, "/"),
									dataType: 'json',
									contentType: 'application/json',
									success: function (data) {
										$("#newfoldertext").removeClass("loading");
										$("#newfoldertext").animate({ width: 0, opacity: 0.0 }).css("margin", "0");
										$("#newfolder").text(hap.common.getLocal("myfiles/newfolder"));
										Load();
									},
									error: hap.common.jsonError
								});
							}
						}, error: hap.common.jsonError
					});
				}
				return false;
			});
			$("#newfoldertext").focusout(function () {
				temp = setTimeout(function () { 
					$("#newfoldertext").removeClass("loading");
					$("#newfoldertext").animate({ width: 0, opacity: 0.0 }, 500, function() { $("#newfoldertext").hide() });
					$("#newfolder").text(hap.common.getLocal("myfiles/newfolder"));
				}, 1000);
			}).focusin(function() {
				if (temp != null) { clearTimeout(temp); temp == null; }
			}).trigger("focusout").keydown(function (event) {
				var keycode = (event.keyCode ? event.keyCode : (event.which ? event.which : event.charCode));
				if (keycode == 9 || keycode == 59 || keycode == 188 || keycode == 190 || keycode == 192 || keycode == 111 || keycode == 220 || keycode == 191 || keycode == 106 || (keycode == 56 && keys.shift) || (keycode == 50 && keys.shift)) { event.preventDefault(); return; }
				else if (keycode == 27) { $("#newfoldertext").blur(); event.preventDefault(); }
				else if (keycode == 13) { $("#newfolder").trigger("click"); return false; }
			});
			$("#toolbar-cut").animate({ width: 0 }, { duration: 500, complete: function() { $("#toolbar-cut").css("display", "none") } }).click(function () {
				var its = [];
				for (var i = 0; i < SelectedItems().length; i++)
					its.push(SelectedItems()[i]);
				clipboard = { mode: "cut", items: its };
				RefreshToolbar();
				return false;
			});
			$("#toolbar-copy").animate({ width: 0 }, { duration: 500, complete: function() { $("#toolbar-copy").css("display", "none") } }).click(function() {
				var its = [];
				for (var i = 0; i < SelectedItems().length; i++)
					its.push(SelectedItems()[i]);
				clipboard = { mode: "copy", items: its }; 
				RefreshToolbar();
				return false;
			});
			$("#toolbar-paste").animate({ width: 0 }, { duration: 500, complete: function() { $("#toolbar-paste-container").hide(); $("#toolbar-paste").css("display", "none") } }).click(function () {
				var s = "";
				for (var i = 0; i < clipboard.items.length; i++) s += clipboard.items[i].Data.Name + "\n";
				$("#progressstatus").dialog({ autoOpen: true, modal: true, title: hap.common.getLocal("myfiles/" + ((clipboard.mode == 'copy') ? "copy/copying" : "move/moving")) + " 1 " + hap.common.getLocal("of") + " " + clipboard.items.length + " " + hap.common.getLocal("items") });
				$("#progressstatus .progress").progressbar({ value: (1 / clipboard.items.length) * 100 });
				if (clipboard.mode == 'copy') CopyClipboard(0, (curpath.length == 2 ? curitem.Location.substr(0, curitem.Location.length -1 ) : curitem.Location).replace(/:/g, ""));
				else if (confirm(hap.common.getLocal("myfiles/move/question1") + "\n\n" + s)) MoveClipboard(0, (curpath.length == 2 ? curitem.Location.substr(0, curitem.Location.length -1 ) : curitem.Location).replace(/:/g, ""));
				else $("#progressstatus").dialog("close");
				return false;
			});
			$("#toolbar-unzip").animate({ width: 0 }, { duration: 500, complete: function() { $("#toolbar-unzip").css("display", "none") } }).click(function () {
				$("#progressstatus").dialog({ autoOpen: true, modal: true, title: hap.common.getLocal("myfiles/unzip/unzipping") + ": " + SelectedItems()[0].Data.Name });
				$("#progressstatus .progress").progressbar({ value: 10 });
				UnZip();
				return false;
			});
			$("#toolbar-zip").animate({ width: 0 }, { duration: 500, complete: function() { $("#toolbar-zip").css("display", "none") } }).click(function () {
				$("#zipquestion").dialog({ autoOpen: true, modal: true, buttons: { 
					"ZIP": function() { 
						$(this).dialog("close");
						$("#progressstatus").dialog({ autoOpen: true, modal: true, title: hap.common.getLocal("myfiles/zip/zipingitem1") + " 1 " + hap.common.getLocal("of") + " " + SelectedItems().length + " " + hap.common.getLocal("items") });
						$("#progressstatus .progress").progressbar({ value: 1 });
						Zip(curpath + "\\" + $("#zipfilename").val() + ".zip", 0);
					}, "Close": function() { $(this).dialog("close"); } } 
				}); 
				$("#zipfilename").val(SelectedItems()[0].Data.Name).focus();
				return false;
			});
			$("#toolbar-clear").animate({ width: 0 }, { duration: 500, complete: function() { $("#toolbar-clear").css("display", "none") } }).click(function () {
				clipboard = null;
				RefreshToolbar();
				return false;
			});
			$("#toolbar-open").animate({ width: 0 }, { duration: 500, complete: function() { $("#toolbar-open").css("display", "none") } }).click(function () {
			    if (SelectedItems().length > 1) {
			        $('#hapContent').append('<div title="' + hap.common.getLocal("myfiles/only1") + '" id="hapWarning">' + hap.common.getLocal("myfiles/only1") + '</div>');
			        $("#hapWarning").hapPopup({
			            buttons: [
                            { Text: "Ok", Click: function () { $(this).parents(".hapPopup").remove(); return false; } }
			            ]
			        });
			        return false;
			    }
			    if (SelectedItems()[0].Data.Type == 'Directory') window.location.href = "#" + SelectedItems()[0].Data.Path;
			    else if (SelectedItems()[0].Data.Extension == ".zip") window.location.href = "#" + (curitem.Location.replace(/:/gi, "").replace(/\//gi, "/") + "\\").replace(/\\\\/gi, "\\") + SelectedItems()[0].Data.Name + ".zip";
			    else window.location.href = SelectedItems()[0].Data.Path;
				return false;
			});
			$("#toolbar-download").animate({ width: 0 }, { duration: 500, complete: function() { $("#toolbar-download").css("display", "none") } }).click(function () {
			    if (SelectedItems()[0].Data.Type == 'Directory') window.location.href = "#" + SelectedItems()[0].Data.Path;
			    else {
			        if ($("#hapdirectedit").is(":checked") && SelectedItems()[0].Data.Extension.match(/(xls|doc|ppt)/gi)) {
			            $('#hapContent').append('<div title="Question" id="hapWarning">' + hap.common.getLocal("myfiles/directeditquestion") + '</div>');
			            $("#hapWarning").hapPopup({
			                buttons: [
                                {
                                    Text: "Download", Click: function () {
                                        $(this).parents(".hapPopup").remove();
                                        $('#hapContent').append('<div title="Warning" id="hapWarning">' + hap.common.getLocal("myfiles/downloadwarning") + '</div>');
                                        $("#hapWarning").hapPopup({
                                            buttons: [
                                                {
                                                    Text: "Ok", Click: function () {
                                                        $(this).parents(".hapPopup").remove();
                                                        window.open("" + (SelectedItems()[0].Data.Path.match(/\.\./i) ? SelectedItems()[0].Data.Path.replace(/\\/g, "/") : '#' + SelectedItems()[0].Data.Path)); return false;
                                                    }
                                                },
                                                { Text: "Cancel", Click: function () { $(this).parents(".hapPopup").remove(); return false; } }
                                            ], item: SelectedItems()[0]
                                        });
                                        return false;
                                    }
                                },
                                { Text: "DirectEdit", Click: function () { $(this).parents(".hapPopup").remove(); location.href = "" + SelectedItems()[0].Data.Path.replace(/\.\.\/Download\//gi, "../myfiles/directedit/"); return false; } },
                                { Text: "Cancel", Click: function () { $(this).parents(".hapPopup").remove(); return false; } }
			                ], item: SelectedItems()[0]
			            });
			        }
			        else {
			            $('#hapContent').append('<div title="Warning" id="hapWarning">' + hap.common.getLocal("myfiles/downloadwarning") + '</div>');
			            $("#hapWarning").hapPopup({
			                buttons: [
                                {
                                    Text: "Ok", Click: function () {
                                        $(this).parents(".hapPopup").remove();
                                        if (SelectedItems()[0].Data.Path.match(/\.zip\//gi)) window.open(SelectedItems()[0].Data.Path.split(/\.zip\//gi)[0] + ".zip");
                                        else window.open(SelectedItems()[0].Data.Path);
                                        return false;
                                    }
                                },
                                { Text: "Cancel", Click: function () { $(this).parents(".hapPopup").remove(); return false; } }
			                ], item: SelectedItems()[0]
			            });
			        }
			    }
				return false;
			});
			$("#toolbar-delete").animate({ width: 0 }, { duration: 500, complete: function() { $("#toolbar-delete").css("display", "none") } }).click(function () {
				$("#progressstatus").dialog({ autoOpen: true, modal: true, title: hap.common.getLocal("myfiles/delete/deletingitem1") + " 1 " + hap.common.getLocal("of") + " " + SelectedItems().length + " " + hap.common.getLocal("items") });
				$("#progressstatus .progress").progressbar({ value: (1 / SelectedItems().length) * 100 });
				var s = "";
				for (var i = 0; i < SelectedItems().length; i++) s += SelectedItems()[i].Data.Name + "\n";
				if (confirm(hap.common.getLocal("myfiles/delete/question1") + "\n\n" + s)) Delete(0);
				else $("#progressstatus").dialog("close");
				return false;
			});
			$("#renamebox").focusout(function() {
				if (temp == "esc") { temp = null; return; }
				else {
				    temp = "esc";
					if (SelectedItems()[0].Data.Name == $(this).val()) { $("#renamebox").css("display", "none"); return; }
					$("#renamebox").css("display", "none");
					$("#progressstatus").dialog({ autoOpen: true, modal: false, title: hap.common.getLocal("myfiles/checking") + "..." });
					$("#progressstatus .progress").progressbar({ value: 0 });
					var p = SelectedItems()[0].Data.Path;
					if (p.match(/^..\/download\//gi)) p = p.split(/^..\/download\//gi)[1].replace(/\//g, '\\');

					$.ajax({
						type: 'GET',
						url: hap.common.formatJSONUrl("~/api/MyFiles/Exists/" + (p.substr(0, p.lastIndexOf('\\')) + "\\" + $("#renamebox").val() + (SelectedItems()[0].Data.Extension == null ? '\\' : SelectedItems()[0].Data.Extension)).replace(/\\\\/gi, "\\").replace(/\\/gi, "/")),
						dataType: 'json',
						context: this,
						contentType: 'application/json',
						success: function (data) {
							if (data.Name != null) {
							    $("#progressstatus").dialog({ autoOpen: true, modal: false, title: hap.common.getLocal("myfiles/waiting") + "..." });
								$("#progressstatus .progress").progressbar({ value: 10 });
								confirm(data.Name + " " + hap.common.getLocal("myfiles/folderexists2"));
								$("#progressstatus").dialog("close");
							} else {
							    $("#progressstatus").dialog({ autoOpen: true, modal: false, title: hap.common.getLocal("myfiles/renaming") + "..." });
								$("#progressstatus .progress").progressbar({ value: 50 });
								$.ajax({
									type: 'POST',
									url: hap.common.formatJSONUrl("~/api/MyFiles/Move"),
									data: '{ "OldPath": "' + p.replace(/\\/gi, "/") + '", "NewPath": "' + (p.substr(0, p.lastIndexOf('\\')) + "\\" + $("#renamebox").val() + (SelectedItems()[0].Data.Extension == null ? '\\' : SelectedItems()[0].Data.Extension)).replace(/\\\\/gi, "\\").replace(/\\/gi, "/") + '" }',
									dataType: 'json',
									contentType: 'application/json',
									success: function (data) {
										$("#progressstatus").dialog({ autoOpen: true, modal: false, title: hap.common.getLocal("myfiles/waiting") + "..." });
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
			$("#help").click(function() {
				hap.help.Load("myfiles/index");
				return false;
			});
			$("#view").click(function () {
			    if (showDE == 1) { $("#directedit").animate({ height: 'toggle' }); showDE = 0; }
				if (showView == 0) {
					showView = 1;
					$("#Views").animate({ height: 'toggle' });
				}
				return false;
			});
			$("#de").click(function () {
			    if (showView == 1) { $("#Views").animate({ height: 'toggle' }); showDE = 0; }
			    if (showDE == 0) {
			        showDE = 1;
			        $("#directedit").animate({ height: 'toggle' });
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
			    if (showDE == 1) { $("#directedit").animate({ height: 'toggle' }); showDE = 0; }
			});
			$("#Views a").click(function () {
				if ($(this).attr("id") == "details") {
					viewMode = 1;
					$("#MyFiles").addClass("details").removeClass("small").removeClass("medium").removeClass("large");
					$("#MyFilesHeaddings").css("display", "block");
					$("#MyFilesHeaddings .name").css("width", $("#MyFiles > a .label").width() + $("#MyFiles > a img").width() + 4);
					$("#MyFilesHeaddings .type").css("width", $("#MyFiles > a .type").width() + 2);
					$("#MyFilesHeaddings .modified").css("width", $("#MyFiles > a .modified").width() + 2);
					$("#MyFilesHeaddings .extension").css("width", $("#MyFiles > a .extension").width() + 2);
					$("#MyFilesHeaddings .size").css("width", $("#MyFiles > a .size").width() + 2);

					$("#renamebox").removeClass("small").removeClass("medium").removeClass("large").addClass("details");
				}
				else if ($(this).attr("id") == "smallicons") {
					viewMode = 2;
					$("#MyFiles").addClass("small").removeClass("details").removeClass("medium").removeClass("large");
					$("#MyFilesHeaddings").css("display", "none");
					$("#renamebox").removeClass("details").removeClass("medium").removeClass("large").addClass("small");
				}
				else if ($(this).attr("id") == "mediumicons") {
					viewMode = 3;
					$("#MyFiles").addClass("medium").removeClass("small").removeClass("large").removeClass("details");
					$("#MyFilesHeaddings").css("display", "none");
					$("#renamebox").removeClass("details").removeClass("small").removeClass("large").addClass("medium");
				}
				else if ($(this).attr("id") == "largeicons") {
					viewMode = 4;
					$("#MyFiles").addClass("large").removeClass("medium").removeClass("small").removeClass("details");
					$("#MyFilesHeaddings").css("display", "none");
					$("#renamebox").removeClass("details").removeClass("small").removeClass("medium").addClass("large");
				}
				else {
					viewMode = 0;
					$("#MyFiles").removeClass("details").removeClass("small").removeClass("medium").removeClass("large");
					$("#renamebox").removeClass("details").removeClass("small").removeClass("medium").removeClass("large");
					$("#MyFilesHeaddings").css("display", "none");
				}
				return false;
			});
			$("#uploadprogress").css("margin-left", $("#myfilescontent").width() - $("#uploadprogress").width()).slideUp('slow');
			$("#upload").click(function () { 
				if ($("#uploadedfiles")[0].files != null) {
					$("#uploaders iframe").remove();
					$("#uploaders").dialog({ autoOpen: true, resizable: false, modal: true, buttons: { 
						"Upload": function() { 
							$("#uploadprogress").show().slideDown('slow');
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
					$("#uploaders input, #uploaders label").hide();
					$("#uploaders").dialog({ autoOpen: true, modal: true, width: 320, height: 280, resizable: false });
				}
				return false;
			});
			$("#uploadedfiles").attr("accept", "<%=DropZoneAccepted.Replace("f:", "") %>");
		    $("#search").css("top", $("#hapHeader").position().top + $("#hapHeader").height());
		    $("#search input").focus(function () { if ($(this).val().match(hap.common.getLocal("search"))) $(this).val(""); }).blur(function () { if ($(this).val() == "") $(this).val(hap.common.getLocal("search") + ": " + curitem.Name); });
		    $(window).trigger("resize");
			$("#MyFiles").css("margin-left", $("#Tree").width() + 5);
			$("#MyFilesHeaddings").css("margin-left", $("#Tree").width() + 5).css("top", $("#hapHeader").position().top + $("#hapHeader").height());
			$(window).trigger("hashchange");
		});
	    $(document).bind('keydown', function (event) { 
	        var keycode = (event.keyCode ? event.keyCode : (event.which ? event.which : event.charCode)); 
	        keys.shift = (keycode == 16); keys.ctrl = (keycode == 17); 
	        if (keycode == 46 && SelectedItems().length > 0 && !$("input").is(":focus")) { 
	            event.preventDefault();
	            $("#progressstatus").dialog({ autoOpen: true, modal: true, title: hap.common.getLocal("myfiles/delete/deletingitem1") + " 1 " + hap.common.getLocal("of") + " " + SelectedItems().length + " " + hap.common.getLocal("items") });
	            $("#progressstatus .progress").progressbar({ value: (1 / SelectedItems().length) * 100 });
	            var s = "";
	            for (var i = 0; i < SelectedItems().length; i++) s += SelectedItems()[i].Data.Name + "\n";
	            if (confirm(hap.common.getLocal("myfiles/delete/question1") + "\n\n" + s)) Delete(0);
	            else $("#progressstatus").dialog("close");
	        }
	    });
	    $(document).bind('keyup', function (event) { keys.shift = keys.ctrl = false; });
	    function closeUpload() { $("#uploaders").dialog("close"); };
		</script>
	</hap:CompressJS>
	<% if (FirstTime) { %> <script type="text/javascript">$(function () { $("#help").trigger("click"); });</script><%}  %>
    <% if (W8AppCap) { %> <script type="text/javascript">$(function () { hap.help.Load("myfiles/w8app"); });</script><%}  %>
</asp:Content>
