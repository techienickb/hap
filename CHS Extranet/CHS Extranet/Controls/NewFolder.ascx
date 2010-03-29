<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="NewFolder.ascx.cs" Inherits="CHS_Extranet.Controls.NewFolder" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Assembly="System.Web.Ajax" Namespace="System.Web.UI" TagPrefix="asp" %>
    <asp:LinkButton runat="server" Text="New Folder" ID="newfolderlink" />
    <asp:Panel runat="server" ID="newfolderbox" Width="300px" style="display: none;" CssClass="modalPopup">
        <h1>New Folder</h1>
        <p><b>Folder Name: </b><asp:TextBox ID="foldername" runat="server" ValidationGroup="NewFolderGroup" />
            <asp:RequiredFieldValidator runat="server" ErrorMessage="*" ValidationGroup="NewFolderGroup" ControlToValidate="foldername" /></p>
        <div class="modalButtons">
            <asp:Button runat="server" ID="yes" UseSubmitBehavior="true" ValidationGroup="NewFolderGroup" Text="Create" onclick="yes_Click" />
            <asp:Button runat="server" ID="cancel" Text="Cancel" />
        </div>
    </asp:Panel>
    <asp:ModalPopupExtender runat="server" TargetControlID="newfolderlink" ID="newfolderpopup" PopupControlID="newfolderbox" BackgroundCssClass="modalBackground" CancelControlID="cancel" />