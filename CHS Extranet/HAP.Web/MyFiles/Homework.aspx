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
<asp:Content ContentPlaceHolderID="body" runat="server">
	<div style="overflow: hidden; clear: both; position: relative; height: 120px" id="myfilesheader">
		<div class="tiles" style="position: absolute; left: 0; margin-top: 45px;">
			<a class="button" href="../">Home Access Plus+ Home</a>
		</div>
		<div class="tiles" style="float: right; text-align: right; margin-top: 45px;">
            <a class="button" id="new" href="#">New</a>
			<a class="button" href="./">My Files</a>
		</div>
		<div style="text-align: center;">
			<img src="../images/myfiles.png" alt="My Files" />
		</div>
	</div>
	<div id="o" style="overflow: hidden;">
		<div id="tree" style="float: left;"></div>
	</div>
	<script type="text/javascript">
	    var active;
	    function load() {
	        $.getJSON("../api/homework/my", function (data) {
	            var c = [], a = { title: data[0].TeacherName, isFolder: true, children: [] };
	            for (var i = 0; i < data.length; i++) {
	                if (a.title != data[i].TeacherName) {
	                    c.push(a);
	                    a = { title: data[i].TeacherName, isFolder: true, children: [] };
	                }
	                a.children.push({ title: data[i].Name, isFolder: false, d: data[i], key: data[i].Teacher + "/" + data[i].Start.replace(/\:/gi, ".") + "/" + data[i].End.replace(/\:/gi, ".") + "/" + data[i].Name });
	            }
	            c.push(a);
	            $("#tree").dynatree({
	                imagePath: "../images/setup/", selectMode: 1, isLazy: false, minExpandLevel: 1, children: c, fx: { height: "toggle", duration: 200 },
	                onActivate: function (node) {
	                    $("#o .homework").remove();
	                    active = null;
	                    if (!node.data.isFolder) {
	                        active = node.data.d;
	                        $("#o").append('<div class="homework" style="overflow: hidden;"><h1>' + node.data.title + '</h1><div>Valid from: ' + node.data.d.Start + ' until: ' + node.data.d.End + ' issue by: ' + node.data.d.TeacherName + '</div><div>' + node.data.d.Description + '</div><button onclick="return false; showuploader();">Upload Homework</button></div>');
	                    }
	                }
	            });
	        });
	    }
	    $(function () {
	        $(".button").button();
	        load();
	    });
	</script>

    <div id="addhomework" title="Add Homework">
        <label for="title">Title:</label><input type="text" id="title" required="required" /><br />
        <label for="start">Start:</label><input type="date" id="start" style="width: 105px" required="required" /><input type="time" id="starttime" maxlength="5" style="width: 40px;" required="required" /><br />
        <label for="end">End:</label><input type="date" id="end" style="width: 105px" required="required" /><input type="time" id="endtime" maxlength="5" style="width: 40px;" required="required" /><br />
        <label for="description">Description:</label>
        <textarea id="description" cols="60" rows="6"></textarea>
        <div style="display: inline-block; width: 49%; clear:both;">
            <label for="students">Students</label><br />
            <select id="students" multiple="multiple" style="float:left;">
            </select>
            <button id="add" class="students">+</button><br />
            <button id="del" class="students">-</button>
        </div>
        <div style="display:inline-block; width: 49%; clear:both;">
            <label for="teachers">Additional Teachers</label><br />
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
                            $.ajax({
                                url: "api/homework/add1", type: 'POST',
                                data: '{ "name": "' + escape($("#title").val()) + '", "description": "' + escape($("textarea").wysiwyg("getContent")) + '", "start": "' + $("#start").val() + " " + $("#starttime").val() + '", "end": "' + $("#end").val() + " " + $("#endtime").val() + '" }',
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
                    }, width: 500
                });
                $("textarea").wysiwyg("setContent", "Initial Content");
                $("#starttime, #endtime").val("09:00");
                $("#start").val($.datepicker.formatDate('dd/mm/yy', new Date()));
                return false;
            });
        });
    </script>
</asp:Content>
