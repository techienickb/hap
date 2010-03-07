<%@ Page Title="Crickhowell High School - IT - Home Access Plus+" Language="C#" MasterPageFile="~/chs.master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="CHS_Extranet.Default" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit.HTMLEditor" TagPrefix="asp" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Assembly="System.Web.Ajax" Namespace="System.Web.UI" TagPrefix="asp" %>

<asp:Content runat="server" ContentPlaceHolderID="head">
    <link href="mycomputer.css" rel="stylesheet" type="text/css" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div id="maincol">
        <asp:AjaxScriptManager runat="server" />
        <script type="text/javascript">
            var ModalProgress = '<%= ModalProgress.ClientID %>';
            Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(beginReq);
            Sys.WebForms.PageRequestManager.getInstance().add_endRequest(endReq);
            function beginReq(sender, args) {
                // shows the Popup
                $find(ModalProgress).show();
            }

            function endReq(sender, args) {
                //  shows the Popup
                $find(ModalProgress).hide();
            } 
        </script>
        <asp:UpdatePanelAnimationExtender ID="UpdatePanelAnimationExtender" 
            runat="server" Enabled="True" TargetControlID="upl">
            <Animations>
                <OnUpdated><FadeIn minimumOpacity=".2" /></OnUpdated>
                <OnUpdating><FadeOut minimumOpacity=".2" /></OnUpdating>
            </Animations>
        </asp:UpdatePanelAnimationExtender>
        <asp:Panel ID="panelUpdateProgress" runat="server" CssClass="updateProgress">
            <asp:UpdateProgress ID="UpdateProg1" DisplayAfter="0" runat="server"> 
                <ProgressTemplate> 
                    <div style="position: relative; top: 30%; text-align: center; background: #fff;"> 
                        <img runat="server" src="~/images/loading.gif" alt="" style="vertical-align: text-bottom;" /> Loading...
                    </div> 
                </ProgressTemplate> 
            </asp:UpdateProgress> 
        </asp:Panel>
        <div id="EditAnn">
        <asp:LinkButton runat="server" CssClass="EditAnnLink" ToolTip="Edit Announcement" ID="EditAnnouncement">
            <img runat="server" src="~/images/icons/edit.png" alt="Edit Announcement" />
        </asp:LinkButton>
        <asp:Literal runat="server" ID="Announcement" />
        </div>
        <asp:Panel runat="server" ID="AnnouncementEditor" style="display: none;" CssClass="modalPopup" Width="700px">
            <asp:Editor ID="Editor1" runat="server" />
            <asp:CheckBox ID="ShowAnnouncement" runat="server" Text="Show Announcement" />
            <br />
            <asp:Button runat="server" Text="Save" ID="saveann" OnClick="saveann_Click" />
            <asp:Button ID="ok_btn" runat="server" Text="Close" />
        </asp:Panel>
        <asp:ModalPopupExtender ID="ModalPopupExtender1" runat="server" TargetControlID="EditAnnouncement" PopupControlID="AnnouncementEditor" BackgroundCssClass="modalBackground" OkControlID="ok_btn" />
        <asp:modalpopupextender ID="ModalProgress" runat="server" 
            TargetControlID="panelUpdateProgress" BackgroundCssClass="modalMask" 
            PopupControlID="panelUpdateProgress" />
        <asp:UpdatePanel runat="server" ID="upl" ChildrenAsTriggers="true">
            <ContentTemplate>                
                <asp:Image runat="server" ID="userimage" ImageAlign="Right" />
                <h2>Hello, <asp:Literal runat="server" ID="welcomename" />, welcome to Home Access Plus+</h2>
                <h3>My Details:</h3>
                <asp:Panel runat="server" ID="viewmode">
                    <ul>
                        <li><b>Name: </b><asp:Literal runat="server" ID="name" /></li>
                        <li><asp:Literal runat="server" ID="form" /></li>
                        <li><b>Email Address: </b><asp:Literal runat="server" ID="email" /></li>
                    </ul>
                    <asp:Button runat="server" Text="Update My Details" ID="updatemydetails" onclick="updatemydetails_Click" />
                    <p id="HomeButtons">
                        <asp:Repeater ID="homepagelinks" runat="server">
                            <ItemTemplate>
                                <asp:HyperLink runat="server" ID="mycomputer" NavigateUrl='<%#Eval("LinkLocation")%>'>
                                    <img runat="server" src='<%#Eval("Icon")%>' alt="" />
                                    <%#Eval("Name") %>
                                    <i><%#Eval("Description") %></i>
                                </asp:HyperLink>
                            </ItemTemplate>
                        </asp:Repeater>
                    </p>
                </asp:Panel>
                <asp:Panel runat="server" ID="editmode" Visible="false">
                    <ul>
                        <li><b>First Name: </b><asp:TextBox ID="txtfname" runat="server" /></li>
                        <li><b>Last Name: </b><asp:TextBox ID="txtlname" runat="server" /></li>
                        <li><asp:Literal runat="server" ID="formlabel" /><asp:TextBox ID="txtform" runat="server" Columns="4" /></li>
                    </ul>
                    <asp:Button runat="server" Text="Save these Details" ID="editmydetails" onclick="editmydetails_Click" />
                </asp:Panel>
            </ContentTemplate>
        </asp:UpdatePanel> 
    </div>
</asp:Content>
