<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ChangePassword.ascx.cs" Inherits="HAP.Web.Controls.ChangePassword" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
        <asp:Button runat="server" ID="ChangePass" Text="Change Password" />
        <asp:Panel runat="server" ID="ChangePassBox" style="display: none;" CssClass="modalPopup" Width="400px">
            <h1>Change Password</h1>
            <asp:UpdatePanel ID="UpdatePanel1" runat="server" ChildrenAsTriggers="true" UpdateMode="Conditional"><ContentTemplate>
            <div>
                <asp:Label runat="server" AssociatedControlID="currentpass">Current Password: </asp:Label>
                <asp:TextBox runat="server" TextMode="Password" ID="currentpass" ValidationGroup="changepass" />
                <asp:RequiredFieldValidator runat="server" ErrorMessage="*" ForeColor="Red" ControlToValidate="currentpass" ValidationGroup="changepass" />
                <br />
                <asp:Label runat="server" AssociatedControlID="newpass">New Password: </asp:Label>
                <asp:TextBox runat="server" TextMode="Password" ID="newpass" ValidationGroup="changepass" />
                <asp:RequiredFieldValidator runat="server" ErrorMessage="*" ForeColor="Red" ControlToValidate="newpass" ValidationGroup="changepass" />
                <br />
                <asp:Label runat="server" AssociatedControlID="confpass">Confirm Password: </asp:Label>
                <asp:TextBox runat="server" TextMode="Password" ID="confpass" ValidationGroup="changepass" />
                <asp:CompareValidator runat="server" ErrorMessage="*" ForeColor="Red" ControlToValidate="confpass" ControlToCompare="newpass" ValidationGroup="changepass" />
                <br />
                <asp:Label runat="server" ForeColor="Red" ID="errormess" />
            </div>
            <asp:Button runat="server" Text="Save" style="float: left; margin-top: 5px;" onclick="ChangePass_Click" ID="savepass" ValidationGroup="changepass" />
            </ContentTemplate></asp:UpdatePanel>
            <div class="modalButtons">
                <img src="bookingsystem/loading.gif" id="ploadingPopup" alt="" style="display: none; margin-left: 30px; float: left;" />
                <asp:Button ID="ok_btn" runat="server" Text="Close" />
            </div>
        </asp:Panel>
        <asp:ModalPopupExtender runat="server" TargetControlID="ChangePass" PopupControlID="ChangePassBox" BackgroundCssClass="modalBackground" CancelControlID="ok_btn" />
        <script type="text/javascript">
            function endRequestHandler(sender, args) {
                $get('ploadingPopup').style.display = "none";
            }
            function beginRequestHandler(sender, args) {
                $get('ploadingPopup').style.display = "block";
            }
            Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(beginRequestHandler);
            Sys.WebForms.PageRequestManager.getInstance().add_endRequest(endRequestHandler);
</script>