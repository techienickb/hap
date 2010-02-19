<%@ Page Title="Crickhowell High School - IT - Extranet" Language="C#" MasterPageFile="~/chs.master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="CHS_Extranet.Default" %>
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
                        <img src="/images/loading.gif" alt="" style="vertical-align: text-bottom;" /> Loading...
                    </div> 
                </ProgressTemplate> 
            </asp:UpdateProgress> 
        </asp:Panel> 
        <asp:modalpopupextender ID="ModalProgress" runat="server" 
            TargetControlID="panelUpdateProgress" BackgroundCssClass="modalMask" 
            PopupControlID="panelUpdateProgress" />
        <asp:UpdatePanel runat="server" ID="upl" ChildrenAsTriggers="true">
            <ContentTemplate>
                <h1>The Extranet</h1>
                <asp:Image runat="server" ID="userimage" ImageAlign="Right" />
                <h2>Hello, <asp:Literal runat="server" ID="welcomename" /></h2>
                <h3>My Details:</h3>
                <asp:Panel runat="server" ID="viewmode">
                    <ul>
                        <li><b>Name: </b><asp:Literal runat="server" ID="name" /></li>
                        <li><asp:Literal runat="server" ID="form" /></li>
                        <li><b>Email Address: </b><asp:Literal runat="server" ID="email" /></li>
                    </ul>
                    <asp:Button runat="server" Text="Update My Details" ID="updatemydetails" onclick="updatemydetails_Click" />
                    <p id="HomeButtons">
                        <asp:LinkButton runat="server" ID="mycomputer" onclick="mycomputer_Click">
                            <img src="images/icons/net.png" alt="" />
                            Browse My Computer
                            <i>Access your school my documents</i>
                        </asp:LinkButton>
                        <asp:LinkButton runat="server" ID="rdapp" OnClick="rdapp_Click">
                            <img src="images/icons/remotedesktop.png" alt="" />
                            Access a School Computer
                            <i>Run school applications at home</i>
                        </asp:LinkButton>
                        <asp:LinkButton runat="server" ID="learnres" OnClick="learnres_Click">
                            <img src="images/icons/school.png" alt="" />
                            Access Learning Resources
                            <i>Launch RM Learning Resources</i>
                        </asp:LinkButton>
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
