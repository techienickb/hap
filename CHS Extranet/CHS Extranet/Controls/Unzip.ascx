<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Unzip.ascx.cs" Inherits="CHS_Extranet.Controls.Unzip" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Assembly="System.Web.Ajax" Namespace="System.Web.UI" TagPrefix="asp" %>
    <asp:Button runat="server" Text="Unzip" style="display: none;" id="unzipclicker" />
    <asp:Panel runat="server" ID="unzipbox" Width="300px" style="display: none;" CssClass="modalPopup">
        <h1>Unzip</h1>
        <p>Unzip <asp:Label runat="server" ID="filename" /></p>
        <p>
            <asp:RadioButton ID="unziphere" Checked="true" GroupName="unzipto" Text="Unzip Here" runat="server" />
            <br />
            <asp:RadioButton runat="server" ID="unziptox" GroupName="unzipto" Text="Unzip to new folder" />
        </p>
        <div class="modalButtons">
            <asp:Button runat="server" ID="ok" Text="Unzip" CausesValidation="false" onclick="ok_Click" />
            <asp:Button runat="server" ID="cancel" Text="Cancel" />
        </div>
    </asp:Panel>
    <asp:ModalPopupExtender runat="server" TargetControlID="unzipclicker" PopupControlID="unzipbox" BackgroundCssClass="modalBackground" CancelControlID="cancel" />
<asp:HiddenField runat="server" ID="unzipitem" />
<script type="text/javascript">
    var unzipitem = $get('<%=unzipitem.ClientID %>');
    var unzipitemname = $get('<%=filename.ClientID %>');
    var unzipclick = $get('<%=unzipclicker.ClientID %>');
</script>