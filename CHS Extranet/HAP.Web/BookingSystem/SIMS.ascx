<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SIMS.ascx.cs" Inherits="HAP.Web.BookingSystem.SIMS" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:LinkButton ID="importfs" runat="server" CausesValidation="false" style="float: right; padding: 0 5px;">Import from SIMS</asp:LinkButton>
        <asp:ModalPopupExtender runat="server" TargetControlID="importfs" PopupControlID="simsimport" BackgroundCssClass="modalBackground2" OkControlID="ok_btn" />
        <asp:Panel runat="server" ID="simsimport" style="display: none;" CssClass="modalPopup" Width="300px">
            <h1>Import SIMS Data</h1>
            <asp:FileUpload ID="importfile" runat="server" Width="100%" />
            <div class="modalButtons">
                <asp:Button runat="server" Text="Import" CausesValidation="false" ID="import" onclick="import_Click" />
                <asp:Button ID="ok_btn" runat="server" Text="Close" />
            </div>
        </asp:Panel>
        <asp:Literal ID="results" runat="server"></asp:Literal>