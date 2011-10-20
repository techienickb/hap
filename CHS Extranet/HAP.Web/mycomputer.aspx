<%@ Page Title="Crickhowell High School - IT - Extranet - My Computer" Language="C#" MasterPageFile="~/masterpage.master" AutoEventWireup="true" CodeBehind="mycomputer.aspx.cs" Inherits="HAP.Web.mycomputer" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Src="~/Controls/NewFolder.ascx" TagName="NewFolder" TagPrefix="hap" %>
<%@ Register Src="~/Controls/Delete.ascx" TagName="Delete" TagPrefix="hap" %>
<%@ Register Src="~/Controls/Rename.ascx" TagName="Rename" TagPrefix="hap" %>
<%@ Register Src="~/Controls/Unzip.ascx" TagName="Unzip" TagPrefix="hap" %>
<%@ Register Src="~/Controls/Zip.ascx" TagName="Zip" TagPrefix="hap" %>
<%@ Register Src="~/Controls/Upload.ascx" TagName="Upload" TagPrefix="hap" %>

<asp:Content runat="server" ContentPlaceHolderID="head">
	<link href="<%=ResolveClientUrl("~/style/mycomputer.css")%>" rel="stylesheet" type="text/css" />
	<meta name="DownloadOptions" content="noopen" />
	<script type="text/javascript" src="<%=ResolveClientUrl("~/Scripts/rightclick.js")%>"></script>
	<script type="text/javascript">
		SimpleContextMenu.setup({ 'preventDefault': true, 'preventForms': false });
		SimpleContextMenu.attach('container', 'CM1');
		function onSilverlightError(sender, args) {
			alert(args.ErrorMessage);
		}
		var slCtl = null;
		function onSilverlightLoaded(sender, args) {
			slCtl = sender.getHost();
		}
		var appdir = "<%=Request.ApplicationPath %>";
	</script>
</asp:Content>

<asp:Content ContentPlaceHolderID="body" runat="server">
	<div id="maincol">
		<h1>My Computer</h1>
		<a href="<%=ResolveClientUrl("~")%>">Home Access Plus+ Home</a>, <a href="<%=ResolveClientUrl("~/MyComputerSL.aspx")%>" onclick="return changeversion('sl');" id="mypcsl" title="Try the Extended silverlight version of the My School Computer Browser">Extended Version</a>
		<script type="text/javascript" src="<%=ResolveClientUrl("~/Scripts/Silverlight.js")%>"></script>
		<div id="versionquest" style="display: none;">
			<div class="modalBackground" style="width: 100%; height: 100%; position: absolute; position: fixed; z-index: 2000; top: 0; left: 0; right: 0; bottom: 0;">
			</div>
			<div style="width: 100%; height: 100%; position: absolute; position: fixed; z-index: 2001; top: 0; left: 0; right: 0; bottom: 0;">
				<table cellpadding="0" cellspacing="0" border="0" style="width: 100%; height: 100%;">
					<tr valign="middle" align="center">
						<td valign="middle" align="center">
							<div class="modalPopup" style="width: 300px; text-align: left;">
								<h1>My Computer Version:</h1>
								Please select a version to use:
								<div id="HomeButtons">
									<a href="<%=ResolveClientUrl("~/MyComputerSL.aspx")%>" onclick="return changeversion('sl');">Extended Version<i>Contains drag and drop features</i></a>
									<a href="<%=ResolveClientUrl("~/mycomputer.aspx")%>" onclick="return changeversion('html');">Basic Version<i>Basic HTML icons</i></a>
								</div>
							</div>
						</td>
					</tr>
				</table>
			</div>
		</div>
		<div style="position: relative;">
			<div id="bar">
				<hap:NewFolder runat="server" ID="newfolderlink" Visible="false" />
				<hap:Upload runat="server" id="newfileuploadlink" Visible="false" />
				<a class="right" href="<%=ResolveClientUrl("~/mycomputer.aspx")%>" onclick="return view();"><span>View</span></a>
			</div>
			<div id="viewbox">
				<a href="#" onclick="return changeview('Icons');">Icons</a>
				<a href="#" onclick="return changeview('List');">List</a>
				<a href="#" onclick="return changeview('Tile');">Tiles</a>
			</div>
		</div>
		<asp:Repeater runat="server" ID="breadcrumbrepeater">
			<HeaderTemplate><div id="breadcrumbs"></HeaderTemplate>
			<ItemTemplate><a href="<%#Eval("Path") %>"><%#Eval("Name") %></a></ItemTemplate>
			<SeparatorTemplate><span>&nbsp;</span></SeparatorTemplate>
			<FooterTemplate></div></FooterTemplate>
		</asp:Repeater>
		<div id="browser">
			<asp:Repeater runat="server" ID="browserrepeater">
				<ItemTemplate>
					<a href="<%#Eval("Path") %>"<%#((bool)Eval("RightClick")) ? " class=\"container\"" : ""%>>
						<img src="<%#ResolveClientUrl("~/" + Eval("Image").ToString()) %>" alt="" />
						<%#Eval("Name") %>
						<%#Eval("Description") %>
					</a>
				</ItemTemplate>
			</asp:Repeater>
		</div>
		<script type="text/javascript" src="<%=Request.ApplicationPath %>/scripts/viewmode.js">
		</script>
		<hap:Delete runat="server" id="DeleteBox" />
		<hap:Rename runat="server" id="RenameBox" />
		<hap:Zip runat="server" id="ZipBox" />
		<hap:Unzip runat="server" id="UnzipBox" />
		<asp:PlaceHolder runat="server" ID="postbackmove" Visible="false">
			<script type="text/javascript">
				function getPosition(obj) {
					var topValue = 0;
					while (obj) {
						topValue += obj.offsetTop;
						obj = obj.offsetParent;
					}
					return topValue;
				}
				self.scrollTo(0, getPosition($get('bar')));
			</script>
		</asp:PlaceHolder>
	</div>
	<div id="CM1" class="SimpleContextMenu">
		<a href="#" onclick="return popup(this);">Delete</a>
		<a href="#" onclick="return popup(this);" runat="server" id="rckmove">Move</a>
		<a href="#" onclick="return popup(this);">Rename</a>
		<a href="#" onclick="return popup(this);" style="display: none;">HTML Preview</a>
		<a href="#" onclick="return popup(this);" style="display: none;">Zip</a>
		<a href="#" onclick="return popup(this);" style="display: none;">Unzip</a>
	</div>
</asp:Content>
