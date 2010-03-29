<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Delete.ascx.cs" Inherits="CHS_Extranet.Controls.Delete" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Assembly="System.Web.Ajax" Namespace="System.Web.UI" TagPrefix="asp" %>
    <asp:Button runat="server" Text="Delete" style="display: none;" id="deleteclicker" />
    <asp:Panel runat="server" ID="deletebox" Width="300px" style="display: none;" CssClass="modalPopup">
        <h1>Are you sure you want to delete?</h1>
        <p><asp:Label runat="server" ID="filename" /></p>
        <div class="modalButtons">
            <asp:Button runat="server" ID="yesdel" UseSubmitBehavior="true" Text="Yes" onclick="yesdel_Click" />
            <asp:Button runat="server" ID="cancel" Text="Cancel" />
        </div>
    </asp:Panel>
    <asp:ModalPopupExtender runat="server" TargetControlID="deleteclicker" PopupControlID="deletebox" BackgroundCssClass="modalBackground" CancelControlID="cancel" />
<asp:HiddenField runat="server" ID="deleteitem" />
<script type="text/javascript">
    var deleteitem = $get('<%=deleteitem.ClientID %>');
    var deleteitemname = $get('<%=filename.ClientID %>');
    var deleteclick = $get('<%=deleteclicker.ClientID %>');
</script>