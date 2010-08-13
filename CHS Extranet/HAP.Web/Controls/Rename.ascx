<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Rename.ascx.cs" Inherits="HAP.Web.Controls.Rename" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
    <asp:Button runat="server" Text="Rename" style="display: none;" id="renameclicker" />
    <asp:Panel runat="server" ID="renamebox" Width="300px" style="display: none;" CssClass="modalPopup">
        <h1>Rename</h1>
        <p>
            Rename: <asp:Label runat="server" ID="filename" />
            <br />
            To:
            <asp:TextBox ID="newname" runat="server" ValidationGroup="Rename" />
            <asp:RequiredFieldValidator ValidationGroup="Rename" runat="server" ErrorMessage="*" ControlToValidate="newname" />
        </p>
        <div class="modalButtons">
            <asp:Button runat="server" ID="yesren" Text="Rename" ValidationGroup="Rename" onclick="yesren_Click" />
            <asp:Button runat="server" ID="cancel" Text="Cancel" />
        </div>
    </asp:Panel>
    <asp:ModalPopupExtender runat="server" TargetControlID="renameclicker" PopupControlID="renamebox" BackgroundCssClass="modalBackground" CancelControlID="cancel" />
    <asp:HiddenField runat="server" ID="renameitem" />
<script type="text/javascript">
    var renameitem = $get('<%=renameitem.ClientID %>');
    var renameitemname = $get('<%=filename.ClientID %>');
    var renameclick = $get('<%=renameclicker.ClientID %>');
</script>