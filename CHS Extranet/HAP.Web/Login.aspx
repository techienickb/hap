<%@ Page Title="{0} - Home Access Plus+ - Login" Language="C#" MasterPageFile="~/Masterpage.master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="HAP.Web.Login" %>
<asp:Content ContentPlaceHolderID="body" runat="server">
    <div id="loginform">
        <img src="images/haplogo.png" alt="Home Access Plus+ - Access your school from Home" />
        <div>
            <asp:Label runat="server" AssociatedControlID="username">Username: </asp:Label>
            <asp:TextBox runat="server" ID="username" Width="300px" />
            <asp:RequiredFieldValidator runat="server" ControlToValidate="username" ErrorMessage="*" ForeColor="Red" />
        </div>
        <div>
            <asp:Label runat="server" AssociatedControlID="password">Password: </asp:Label>
            <asp:TextBox TextMode="Password" runat="server" ID="password" Width="300px" />
            <asp:RequiredFieldValidator runat="server" ControlToValidate="password" ErrorMessage="*" ForeColor="Red" />
        </div>
        <asp:Literal runat="server" ID="message" />
        <div class="submit">
            <asp:Button runat="server" UseSubmitBehavior="true" ID="login" Text="Login" onclick="login_Click" Font-Size="200%" />
        </div>
        <div class="base">
            Connected to <%=HAP.Web.Configuration.hapConfig.Current.School.Name %>
        </div>
        <div class="copyright">
            &copy; <%=DateTime.Now.Year %> nb development. All rights reserved.
        </div>
    </div>
    <script type="text/javascript">
        $(document).ready(function () {
            $("#<%=login.ClientID %>").button();
        });
        $('input[type=text]').keyup(function (e) {
            if (e.keyCode == 13) {
                $("#<%=login.ClientID %>").trigger('click');
            }
        });
    </script>
</asp:Content>
