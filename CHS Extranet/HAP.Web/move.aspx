<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="move.aspx.cs" Inherits="HAP.Web.move" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Move</title>
    <link href="<%=Request.ApplicationPath %>/basestyle.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
    <h1>Move</h1>
    <p>Move <asp:Literal runat="server" ID="moveitem" /> to:</p>
    <div style="height: 300px; overflow: auto;">
        <asp:TreeView ID="TreeView1" runat="server" ImageSet="Arrows" 
            onselectednodechanged="TreeView1_SelectedNodeChanged">
            <HoverNodeStyle Font-Underline="True" ForeColor="#000" />
            <NodeStyle HorizontalPadding="5px" NodeSpacing="0px" ForeColor="#000" VerticalPadding="0px" />
            <ParentNodeStyle Font-Bold="False" />
            <SelectedNodeStyle Font-Underline="True" BackColor="#d9d9d9" ForeColor="#000" 
                HorizontalPadding="0px" VerticalPadding="0px" />
        </asp:TreeView>
    </div>
    <p>
        <asp:HiddenField runat="server" ID="fullname" />
        <asp:Button runat="server" Enabled="false" ID="ok" Text="Move" onclick="ok_Click" />
        <input type="button" value="Cancel" onclick="window.close();" />
    </p>
    <asp:PlaceHolder runat="server" id="closeandrefresh" visible="false">
        <script type="text/javascript">
            window.opener.location.href = window.opener.location.href;
            if (window.opener.progressWindow) window.opener.progressWindow.close();
            window.close();
        </script>
    </asp:PlaceHolder>
    </form>
</body>
</html>
