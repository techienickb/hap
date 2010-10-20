<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Upload.ascx.cs" Inherits="HAP.Web.Controls.Upload" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
	<asp:Hyperlink runat="server" ID="uploadclicker" Text="Upload" NavigateUrl="javascript:ShowUploader();" />
	<asp:Panel runat="server" ID="uploadbox" Width="600px" style="display: none;" CssClass="modalPopup">
		<h1>Upload File</h1>
		<object type="application/x-silverlight-2" data="data:application/x-silverlight-2," style="height: 315px; width: 100%;">
			<param name="MinRuntimeVersion" value="4.0.50401.0" />
			<param name="source" value="<%=Request.ApplicationPath%>/ClientBin/HAP.Silverlight.xap" />
			<param name="InitParams" runat="server" id="InitParams" />
			<param name="autoUpgrade" value="true" />
			<param name="onLoad" value="onSilverlightLoaded" />
			<param name="onerror" value="onSilverlightError" />
			<param name="minRuntimeVersion" value="4.0.50303.0" />
			<a href="http://www.microsoft.com/getsilverlight/" target="_blank" style="text-decoration: none;"><img src="<%=Request.ApplicationPath%>/images/Silverlight-Prompt.png" alt="Get Microsoft Silverlight" /></a>
			<p>Using the Silverlight for Uploading allows for: (compared to standard HTML uploading)</p>
			<ul>
				<li>Faster Uploads</li>
				<li>Larger Uploads</li>
				<li>Multiple Uploads Queuing</li>
				<li>Upload Progress Bars</li>
				<li>Time to Upload</li>
				<li>Image Previews</li>
			</ul>
			<p>If you do not wish to install Microsoft's Silverlight, please use the HTML Uploader by clicking on the button.</p>
		</object>
		
		<div class="modalButtons">
			<input type="button" value="Switch to HTML Uploader" onclick="window.open('<%=Request.ApplicationPath%>/uploadh.aspx?path=<%=((HAP.Web.routing.IMyComputerDisplay)Page).RoutingDrive %>/<%=((HAP.Web.routing.IMyComputerDisplay)Page).RoutingPath %>', 'HAPUpload', 'toolbar=0,status=0,statusbar=0,menubar=0,menu=0,address=0,addressbar=0,width=600,height=400', true);" />
			<asp:Button runat="server" ID="cancel" Text="Close" />
		</div>
	</asp:Panel>
	<asp:ModalPopupExtender ID="uploadmodalbox" TargetControlID="uploadclicker" runat="server" PopupControlID="uploadbox" BackgroundCssClass="modalBackground" CancelControlID="cancel" OnCancelScript="window.location.href=window.location.href;" />