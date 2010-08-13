<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Zip.ascx.cs" Inherits="HAP.Web.Controls.Zip" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
    <asp:Button runat="server" Text="Zip" style="display: none;" id="zipclicker" />
    <asp:Panel runat="server" ID="zipbox" Width="300px" style="display: none;" CssClass="modalPopup">
        <h1>Are you sure you want to ZIP up this Folder?</h1>
        <p><asp:Label runat="server" ID="filename" /></p>
        <div class="modalButtons">
            <asp:Button runat="server" ID="ok" Text="Ok" onclick="ok_Click" />
            <asp:Button runat="server" ID="cancel" Text="Cancel" />
        </div>
    </asp:Panel>
    <asp:ModalPopupExtender runat="server" TargetControlID="zipclicker" PopupControlID="zipbox" BackgroundCssClass="modalBackground" CancelControlID="cancel" />
<asp:HiddenField runat="server" ID="zipitem" />
<script type="text/javascript">
    var zipitem = $get('<%=zipitem.ClientID %>');
    var zipitemname = $get('<%=filename.ClientID %>');
    var zipclick = $get('<%=zipclicker.ClientID %>');
</script>