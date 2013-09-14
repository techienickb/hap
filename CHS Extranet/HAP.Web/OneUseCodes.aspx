<%@ Page Title="" Language="C#" MasterPageFile="~/masterpage.master" AutoEventWireup="true" CodeBehind="OneUseCodes.aspx.cs" Inherits="HAP.Web.OneUseCodes" %>
<asp:Content runat="server" ContentPlaceHolderID="viewport"><meta name="viewport" content="width=device-width, initial-scale=1.0, maximum-scale=1.0, user-scalable=0" /></asp:Content>
<asp:Content ContentPlaceHolderID="body" runat="server">
    <p>One Use Codes can be used to Sign in from Untrusted locations</p>
    <p>You need to generate codes when you are at School, and using a School Computer, and you will be asked to log in again</p>
    <asp:Button runat="server" ID="ExpireAllCodes" Text="Expire All Codes" OnClick="ExpireAllCodes_Click" />
    <table>
        <thead>
            <tr>
                <th>Code</th><th>Expires</th>
                <%if (User.IsInRole("Domain Admins")) { %>
                <th>Username</th>
                <%} %>
            </tr>
        </thead>
        <tbody>
            <asp:Repeater runat="server" ID="repeater">
                <ItemTemplate>
                    <tr>
                        <td><%#Eval("Code") %></td>
                        <td><%#Eval("Expires").ToString() %></td>
                        <td><%#(User.IsInRole("Domain Admins") ? Eval("Username") : "") %></td>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>
            <tr>
                <td colspan="3"><asp:Button runat="server" ID="gencodes" Text="Generate Codes" OnClick="gencodes_Click" /></td>
            </tr>
        </tbody>
    </table>
</asp:Content>
