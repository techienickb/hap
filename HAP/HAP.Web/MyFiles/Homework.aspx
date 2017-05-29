<%@ Page Title="" Language="C#" MasterPageFile="~/masterpage.master" AutoEventWireup="true" CodeBehind="Homework.aspx.cs" Inherits="HAP.Web.MyFiles.Homework" %>
<asp:Content ContentPlaceHolderID="head" runat="server">
	<script src="../Scripts/jquery.dynatree.js" type="text/javascript"></script>
	<link href="../style/ui.dynatree.css" rel="stylesheet" type="text/css" />
	<link href="../style/MyFiles.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript" src="../Scripts/jquery.wysiwyg.js"></script>
    <link rel="stylesheet" type="text/css" href="../style/jquery.wysiwyg.css" />
    <style type="text/css">
        label { display: inline-block; min-width: 140px; padding-right: 10px; text-align: right; }
    </style>
</asp:Content>
<asp:Content ContentPlaceHolderID="title" runat="server"><asp:HyperLink runat="server" NavigateUrl="~/MyFiles/"><hap:LocalResource StringPath="myfiles/homework/homework" runat="server" /></asp:HyperLink></asp:Content>
<asp:Content ContentPlaceHolderID="header" runat="server">
    <a id="new" href="#" onclick="return false;"><hap:LocalResource StringPath="myfiles/homework/new" runat="server" /></a>
</asp:Content>
<asp:Content ContentPlaceHolderID="body" runat="server">
	<div id="o" style="overflow: hidden;">
		<div id="tree" style="float: left;"></div>
	</div>
	<script type="text/javascript">
	    var active;
	    function load() {
	        $("#uploadhomework").dialog({ autoOpen: false });
	        $.getJSON(hap.common.formatJSONUrl("~/api/homework/my"), function (data) {
	            var c = [], a = data.length > 0 ? { title: data[0].TeacherName, isFolder: true, children: [] } : null;
	            for (var i = 0; i < data.length; i++) {
	                if (a.title != data[i].TeacherName) {
	                    c.push(a);
	                    a = { title: data[i].TeacherName, isFolder: true, children: [] };
	                }
	                a.children.push({ title: data[i].Name, isFolder: false, d: data[i], key: data[i].Teacher + "/" + data[i].Start.replace(/\:/gi, ".") + "/" + data[i].End.replace(/\:/gi, ".") + "/" + data[i].Name });
	            }
	            if (a != null) {
	                c.push(a);
	                $("#tree").dynatree({
	                    imagePath: "../images/setup/", selectMode: 1, isLazy: false, minExpandLevel: 1, children: c, fx: { height: "toggle", duration: 200 },
	                    onActivate: function (node) {
	                        $("#o .homework").remove();
	                        active = null;
	                        if (!node.data.isFolder) {
	                            active = node.data.d;
	                            $("#o").append('<div class="homework" style="overflow: hidden;"><h1>' + node.data.title + '</h1><div>Valid from: ' + node.data.d.Start + ' until: ' + node.data.d.End + ' issue by: ' + node.data.d.TeacherName + '</div><div>' + node.data.d.Description + '</div><button onclick="showuploader(); return false;">Upload Homework</button>' + (node.data.d.Mine ? ('<button onclick="showedit(); return false; ">' + hap.common.getLocal("myfiles/homework/edit") + '</button><button onclick="doRemove(); return false;">' + hap.common.getLocal("myfiles/homework/remove") + '</button>') : '') + '</div>');
	                        }
	                    }
	                });
	            } else { 
	                $("#tree").hide();
	                $("#o .homework").remove();
	            }
	        });
	    }
	    $(function () {
	        $(".button").button();
	        load();
	    });
	</script>
    <hap:WrappedLocalResource runat="server" Tag="div" title="#myfiles/homework/upload" ID="uploadhomework">
        <div id="dropzone">
            Drop File Here or use the file chooser
        </div>
		<input type="file" multiple="multiple" id="uploadedfiles" />
		<iframe style="width: 300px; height: 180px"></iframe>
    </hap:WrappedLocalResource>
	<hap:WrappedLocalResource runat="server" title="#myfiles/progress" id="progressstatus" Tag="div">
		<div class="progress"></div>
	</hap:WrappedLocalResource>
	<div id="uploadprogress" class="tile-border-color" style="border-width: 1px; border-style: solid; border-bottom: 0;">
		<div class="tile-color ui-widget-header"><hap:LocalResource StringPath="myfiles/upload/uploadprogress" runat="server" /></div>
		<div id="progresses">
		</div>
	</div>
    <script type="text/javascript">
        var uploads = new Array();
        function showuploader() {
            if ($("#uploadedfiles")[0].files != null) {
                $("#uploadhomework iframe").remove();
                $("#uploadhomework").dialog({
                    autoOpen: true, resizable: false, modal: true, buttons: {
                        "Upload": function () {
                            $("#uploadprogress").slideDown('slow');
                            for (var i = 0; i < $("#uploadedfiles")[0].files.length; i++) {
                                var file = new Upload(($("#uploadedfiles")[0].files)[i], (active.Path.length == 2 ? active.Path.substr(0, active.Path.length - 1) : active.Path).replace(/:/g, ""));
                                uploads.push(file);
                                file.Start();
                            }
                            if (uploads.length == 0) $("#uploadprogress").slideUp('slow');
                            $("#uploadedfiles").html($("#uploadedfiles").html());
                            $("#uploadto").text("");
                            $(this).dialog("close");
                        }, "Close": function () { $(this).dialog("close"); }
                    }
                });
            } else {
                $("#uploadhomework iframe").attr("src", "../uploadh.aspx?path=" + active.Path + "&teacher=" + active.Teacher + "&name=" + active.name + "&start=" + active.Start + "&end=" + active.End).css("display", "block");
                $("#uploadhomework input").hide();
                $("#uploadhomework").dialog({ autoOpen: true, modal: true, width: 320, height: 280, resizable: false });
            }
        }
        $("#uploadprogress").css("margin-left", $("#myfilescontent").width() - $("#uploadprogress").width()).slideUp('slow');
        $("#uploaders").dialog({ autoOpen: false });

        function Upload(file, path) {
            this.File = file;
            this.Path = path;
            this.Start = function() {
                if (this.File.name.indexOf('.') == -1) {
                    alert(hap.common.getLocal("myfiles/upload/folderwarning").replace(/\%/g, this.File.name));
                    uploads.pop(this);
                    return false;
                }
                else if ("<%=AcceptedExtensions %>".toLowerCase().indexOf(this.File.name.substr(this.File.name.lastIndexOf('.')).toLowerCase()) == -1 && "<%=DropZoneAccepted %>" != "") {
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
                    url: hap.common.resolveUrl('~/api/Homework/Exists/') + active.Teacher + "/" + active.Name + "/" + active.Start.replace(/\:/gi, ".").replace(/\//gi, '-') + "/" + active.End.replace(/\:/gi, ".").replace(/\//gi, '-') + "/" + active.Path.replace(/\\/g, '/') + active.Name + '/' + hap.user + " - " + this.File.name,
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
                this.xhr.open('POST', hap.common.resolveUrl('~/api/homework-upload/') + active.Teacher + "/" + active.Name + "/" + active.Start.replace(/\:/gi, ".").replace(/\//gi, '-') + "/" + active.End.replace(/\:/gi, ".").replace(/\//gi, '-') + "/" + this.Path.replace(/\\/g, '/'), true);
                this.xhr.onreadystatechange = function () {
                    if (this.readyState == 4) {
                        var item = null;
                        for (var i = 0; i < uploads.length; i ++) if (uploads[i].File.name.replace(/[\\'\. \[\]\(\)\-]/g, "_") == this.id) item = uploads[i];
                        if (this.status != 200) alert(hap.common.getLocal("myfiles/upload/upload") + " " + hap.common.getLocal("of") + " " + item.File.name + " " + hap.common.getLocal("myfiles/upload/failed") + "\n\n" + this.responseText.substr(this.responseText.indexOf('<title>') + 7, this.responseText.indexOf('</title>') - (7 + this.responseText.indexOf('<title>'))));
                        $("#upload-" + this.id + " .progressbar").progressbar("value", 100 );
                        $("#upload-" + id).delay(1000).slideUp('slow', function() { $("#upload-" + id).remove(); if (uploads.length == 0) $("#uploadprogress").slideUp('slow'); });
                        load();
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
    </script>
    <div id="addhomework" title="Add Homework">
        <label for="title"><hap:LocalResource runat="server" StringPath="myfiles/homework/title" />:</label><input type="text" id="title" required="required" /><br />
        <label for="start"><hap:LocalResource runat="server" StringPath="myfiles/homework/start" />:</label><input type="date" id="start" style="width: 105px" required="required" /><input type="time" id="starttime" maxlength="5" style="width: 40px;" required="required" /><br />
        <label for="end"><hap:LocalResource runat="server" StringPath="myfiles/homework/end" />:</label><input type="date" id="end" style="width: 105px" required="required" /><input type="time" id="endtime" maxlength="5" style="width: 40px;" required="required" /><br />
        <label for="description"><hap:LocalResource runat="server" StringPath="myfiles/homework/description" />:</label>
        <textarea id="description" cols="60" rows="6"></textarea>
        <div style="display: inline-block; width: 49%; clear:both;">
            <label for="students"><hap:LocalResource runat="server" StringPath="myfiles/homework/students" /></label><br />
            <select id="students" multiple="multiple" style="float:left;">
            </select>
            <button id="add" class="students">+</button><br />
            <button id="del" class="students">-</button>
        </div>
        <div style="display:inline-block; width: 49%; clear:both;">
            <label for="teachers"><hap:LocalResource runat="server" StringPath="myfiles/homework/teachers" /></label><br />
            <select id="teachers" multiple="multiple" style="float:left;">
            </select>
            <button id="add1" class="teachers">+</button><br />
            <button id="del1" class="teachers">-</button>
        </div>
        <script type="text/javascript">
            var control;
            $("#add, #add1").click(function () {
                control = this.className;
                $("#userfinder").dialog({
                    autoOpen: true, buttons: {
                        "Add": function () {
                            $("#results option:selected").clone().appendTo($("#" + control));
                            return false;
                        },
                        "Close": function () {
                            $(this).dialog("close");
                            return false;
                        }
                    }
                });
                return false;
            });
            $("#del, #del1").click(function () {
                $("#" + this.className + " option:selected").remove();
                return false;
            });
        </script>
        <div>
            <label for="path"><hap:LocalResource runat="server" StringPath="myfiles/homework/path" /></label><br />
            <input type="text" id="path" style="width: 100%;" />
        </div>
    </div>
    <div id="userfinder" title="User Finder">
        <label for="userquery" style="text-align:center;">Enter a Username, part of a Username, or Name to begin your Search</label><br />
        <input type="text" id="userquery" /><br />
        <select id="results" multiple="multiple"></select>
    </div>
    <script type="text/javascript">
        $("#userquery").keyup(function () {
            if ($(this).val().length > 0) {
                $("#results option").remove();
                $.ajax({
                    url: "../api/homework/search", type: 'POST',
                    data: '{ "query": "' + $(this).val() + '*" }',
                    dataType: "json", contentType: 'application/JSON',
                    success: function (data) {
                        for (var i = 0; i < data.length; i++) {
                            var x = '<option value="' + data[i].split('|')[0] + '">' + data[i].split('|')[1] + ' - ' + data[i].split('|')[0] + '</option>';
                            $("#results").append(x);
                        }
                    }, error: hap.common.jsonError
                });
            }
        });
    </script>
    <script type="text/javascript">
        $(function () {
            $("#start, #end").datepicker({ dateFormat: 'dd/mm/yy' });
            $("textarea").wysiwyg({ css: hap.common.resolveUrl('~/style/editor.css') });
            $("#addhomework, #userfinder").dialog({ autoOpen: false });
            $("#teachers option, #students option, #results option").remove();
            $("#new").click(function () {
                $("#addhomework").dialog({
                    autoOpen: true, modal: true, buttons: {
                        "Save": function () {
                            var nodes = [];
                            for (var i = 0; i < $("#students option").size(); i++) {
                                nodes.push('{ "Value": "' + $($("#students option")[i]).val() + '", "Type": "User", "Mode": "Student", "Method": "Add" }');
                            }
                            for (var i = 0; i < $("#teachers option").size(); i++) {
                                nodes.push('{ "Value": "' + $($("#teachers option")[i]).val() + '", "Type": "User", "Mode": "Teacher", "Method": "Add" }');
                            }
                            nodes = '[' + nodes.join() + ']';
                            $.ajax({
                                url: hap.common.resolveUrl("~/api/homework/add1"), type: 'POST',
                                data: '{ "name": "' + escape($("#title").val()) + '", "description": "' + escape($("textarea").wysiwyg("getContent")) + '", "start": "' + $("#start").val() + " " + $("#starttime").val() + '", "end": "' + $("#end").val() + " " + $("#endtime").val() + '", "path": "' + $("#path").val().replace(/\\/gi, "/").replace(/\:/gi, "") + '", "nodes":' + nodes + '}',
                                dataType: "json", contentType: 'application/JSON',
                                success: function (data) {
                                    $("#addhomework").dialog("close");
                                    load();
                                }, error: hap.common.jsonError
                            });
                            $(this).dialog("close");
                            return false;
                        },
                        "Cancel": function () { $(this).dialog("close"); return false; }
                    }, width: 500, title: "Add Homework", minWidth: 500
                });
                $("textarea").wysiwyg("setContent", "Initial Content");
                $("#starttime, #endtime").val("09:00");
                $("#start").val($.datepicker.formatDate('dd/mm/yy', new Date()));
                $("#path").val("N:\\HAP Homeworks\\");
                $("#end").val("");
                $("#title").val("");
                $("#students option").remove();
                $("#teachers option").remove();
                return false;
            });
        });
        function showedit() {
            $("#addhomework").dialog({
                autoOpen: true, modal: true, buttons: {
                    "Save": function () {
                        var nodes = [];
                        for (var i = 0; i < $("#students option").size(); i++) {
                            nodes.push('{ "Value": "' + $($("#students option")[i]).val() + '", "Type": "User", "Mode": "Student", "Method": "Add" }');
                        }
                        for (var i = 0; i < $("#teachers option").size(); i++) {
                            nodes.push('{ "Value": "' + $($("#teachers option")[i]).val() + '", "Type": "User", "Mode": "Teacher", "Method": "Add" }');
                        }
                        nodes = '[' + nodes.join() + ']';
                        $.ajax({
                            url: hap.common.resolveUrl("~/api/homework/edit"), type: 'POST',
                            data: '{ "teacher": "' + active.Teacher + '", "name": "' + active.Name + '", "start": "' + active.Start + '", "end": "' + active.end + '", "newname": "' + escape($("#title").val()) + '", "description": "' + escape($("textarea").wysiwyg("getContent")) + '", "newstart": "' + $("#start").val() + " " + $("#starttime").val() + '", "newend": "' + $("#end").val() + " " + $("#endtime").val() + '", "path": "' + $("#path").val() + '" }',
                            dataType: "json", contentType: 'application/JSON',
                            success: function (data) {
                                $("#addhomework").dialog("close");
                                load();
                            }, error: hap.common.jsonError
                        });
                        $(this).dialog("close");
                        return false;
                    },
                    "Cancel": function () { $(this).dialog("close"); return false; }
                }, width: 500, title: "Edit Homework", minWidth: 500
            });
            $("textarea").wysiwyg("setContent", active.Description);
            $("#starttime").val(active.Start.split(" ")[1]);
            $("#start").val(active.Start.split(" ")[0]);
            $("#end").val(active.End.split(" ")[0]);
            $("#endtime").val(active.End.split(" ")[1]);
            $("#path").val(active.Path);
            $("#title").val(active.Name);
            for (var i = 0; i < active.Nodes.length; i++) {
                var x = '<option value="' + active.Nodes[i].Value + '">' + active.Nodes[i].Value + '</option>';
                if (active.Nodes[i].Type == "User" && active.Nodes[i].Method == 'add' && active.Nodes[i].Mode == 'Student')                
                    $("#students").append(x);
                else if (active.Nodes[i].Type == "User" && active.Nodes[i].Method == 'add' && active.Nodes[i].Mode == 'Teacher') 
                    $("#teachers").append(x);
            }
            return false;
        }

        function doRemove() {
            if (!confirm(hap.common.getLocal('myfiles/homework/confirmremove'))) return;
            $.ajax({
                url: hap.common.resolveUrl("~/api/homework/remove"), type: 'POST',
                data: '{ "teacher": "' + active.Teacher + '", "name": "' + active.Name + '", "start": "' + active.Start + '", "end": "' + active.End + '" }',
                dataType: "json", contentType: 'application/JSON',
                success: function (data) {
                    load();
                }, error: hap.common.jsonError
            });
            return false;
        }
    </script>
</asp:Content>
