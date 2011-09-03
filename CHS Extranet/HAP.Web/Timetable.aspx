<%@ Page Title="Crickhowell High School - Home Access Plus+ - Timetable" Language="C#" MasterPageFile="~/Masterpage.master" AutoEventWireup="true" CodeBehind="Timetable.aspx.cs" Inherits="HAP.Web.Timetable" %>
<asp:Content ContentPlaceHolderID="body" runat="server">
    <asp:PlaceHolder runat="server" ID="adminconverter">
        <div>
            <asp:Button runat="server" Text="Convert SIMS.net Export" ID="convert" 
            onclick="convert_Click" /> (Export needs to be saved into the App_Data Folder as timetableexport.xml)
        </div>
        <div>
            <asp:Label ID="Label1" runat="server" AssociatedControlID="upn" Text="UPN: " /><asp:TextBox runat="server" ID="upn" />
            <asp:Button runat="server" ID="impersonate" Text="Impersonate Student" 
                onclick="impersonate_Click" />
        </div>
    </asp:PlaceHolder>
    <asp:Literal runat="server" ID="message" />
    <ul id="timetable">
    <asp:Repeater ID="tt" runat="server" DataSourceID="ttds">
        <ItemTemplate>
            <li>
                <span class="day"><%#Eval("Day") %></span>
                <ul>
            <asp:Repeater runat="server" DataSource='<%#Eval("Lessons") %>'>
                <ItemTemplate>
                    <li>
                        <span class="period"><%#Eval("Period").ToString().Split(new char[] { ':' })[1] %></span>
                        <span class="subject"><%#Eval("Description")%></span>
                        <span class="teacher"><%#Eval("Teacher") %></span>
                        <span class="room"><%#Eval("Room") %></span>
                    </li>
                </ItemTemplate>
            </asp:Repeater>
                </ul>
            </li>
        </ItemTemplate>
    </asp:Repeater>
    </ul>
    <asp:ObjectDataSource ID="ttds" runat="server" SelectMethod="getRecords" TypeName="HAP.Data.Timetables.Timetables">
        <SelectParameters>
            <asp:Parameter Name="UPN" Type="String" />
        </SelectParameters>
    </asp:ObjectDataSource>
    <script type="text/javascript">
        $(document).ready(function () { $("input[type='submit']").button(); });
    </script>
</asp:Content>
