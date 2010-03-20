<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Announcement.ascx.cs" Inherits="CHS_Extranet.Announcement" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit.HTMLEditor" TagPrefix="asp" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Assembly="System.Web.Ajax" Namespace="System.Web.UI" TagPrefix="asp" %>

        <div id="EditAnn">
        <asp:LinkButton runat="server" CssClass="EditAnnLink" ToolTip="Edit Announcement" ID="EditAnnouncement">
            <img runat="server" src="~/images/icons/edit.png" alt="Edit Announcement" />
        </asp:LinkButton>
        <asp:Literal runat="server" ID="AnnouncementText" />
        </div>
        <asp:Panel runat="server" ID="AnnouncementEditor" style="display: none;" CssClass="modalPopup" Width="700px">
            <asp:Editor ID="Editor1" runat="server" />
            <asp:CheckBox ID="ShowAnnouncement" runat="server" Text="Show Announcement" />
            <br />
            <asp:Button runat="server" Text="Save" ID="saveann" OnClick="saveann_Click" />
            <asp:Button ID="ok_btn" runat="server" Text="Close" />
        </asp:Panel>
        <asp:ModalPopupExtender runat="server" TargetControlID="EditAnnouncement" PopupControlID="AnnouncementEditor" BackgroundCssClass="modalBackground" OkControlID="ok_btn" />