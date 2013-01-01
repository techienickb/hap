<%@ Page Title="{0} - Home Access Plus+ - Login" Language="C#" MasterPageFile="~/Masterpage.master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="HAP.Web.Login" %>
<asp:Content runat="server" ContentPlaceHolderID="viewport"><meta name="viewport" content="width=device-width" /></asp:Content>
<asp:Content ContentPlaceHolderID="body" runat="server">
    <div id="logincontainer">
        <div id="loginformside" class="tile-color"></div>
        <div id="loginform">
            <h1><a href="https://hap.codeplex.com/" target="_blank"><hap:LocalResource StringPath="homeaccessplus" runat="server" /></a></h1>
            <h2>Access your School From Home</h2>
            <div>
                <asp:Label runat="server" AssociatedControlID="username">Username: </asp:Label>
                <asp:TextBox runat="server" ID="username" Width="300px" AutoCompleteType="None" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="username" ErrorMessage="*" ForeColor="Red" />
            </div>
            <div>
                <asp:Label runat="server" AssociatedControlID="password">Password: </asp:Label>
                <asp:TextBox TextMode="Password" runat="server" ID="password" Width="300px" />
            </div>
            <asp:Literal runat="server" ID="message" />
            <div class="submit">
                <asp:LinkButton runat="server" UseSubmitBehavior="true" ID="login" Text="Login" onclick="login_Click" Font-Size="200%" />
            </div>
            <div class="copyright">
                &copy; <%=DateTime.Now.Year %> nb development. All rights reserved.
            </div>
        </div>
        <script type="text/javascript">
            hap.load = hap.loadtypes.none;
            $(document).ready(function () {
                $("#<%=login.ClientID %>").button();
                $("#<%=username.ClientID %>").focus();
            });
            $("#<%=username.ClientID%>").focusout(function () {
                $("#<%=username.ClientID%>").val($.trim($("#<%=username.ClientID%>").val()));
            });
            $('input[type=text],input[type=password]').keyup(function (e) {
                if (e.keyCode == 13) {
                    $("#<%=username.ClientID%>").val($.trim($("#<%=username.ClientID%>").val()));
                    location.href = $("#<%=login.ClientID %>").attr("href");
                }
            });
        </script>
    </div>
</asp:Content>
