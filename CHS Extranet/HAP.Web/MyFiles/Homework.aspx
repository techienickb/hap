<%@ Page Title="" Language="C#" MasterPageFile="~/masterpage.master" AutoEventWireup="true" CodeBehind="Homework.aspx.cs" Inherits="HAP.Web.MyFiles.Homework" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
	<script src="../Scripts/jquery-1.7.1.min.js" type="text/javascript"></script>
	<script src="../Scripts/jquery-ui-1.8.16.custom.min.js" type="text/javascript"></script>
	<script src="../Scripts/jquery.dynatree.js" type="text/javascript"></script>
	<link href="../style/ui.dynatree.css" rel="stylesheet" type="text/css" />
	<link href="../style/MyFiles.css" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
	<div style="overflow: hidden; clear: both; position: relative; height: 120px" id="myfilesheader">
		<div class="tiles" style="position: absolute; left: 0; margin-top: 45px;">
			<a class="button" href="../">Home Access Plus+ Home</a>
		</div>
		<div class="tiles" style="float: right; text-align: right; margin-top: 45px;">
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
	    $(function () {
	        $(".button").button();
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
	            $("#tree").dynatree({ imagePath: "../images/setup/", selectMode: 1, isLazy: false, minExpandLevel: 1, children: c, fx: { height: "toggle", duration: 200 },
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
	    });
	</script>
</asp:Content>
