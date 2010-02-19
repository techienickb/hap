<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="delete.aspx.cs" Inherits="CHS_Extranet.delete" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Confirm Deletion</title>
    <link href="/Extranet/basestyle.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <h1>Are you sure you want to delete?</h1>
        <p><asp:Label runat="server" ID="filename" /></p>
        <asp:HiddenField runat="server" ID="fullname" />
        <asp:Button runat="server" ID="yesdel" UseSubmitBehavior="true" Text="Yes" onclick="yesdel_Click" />
        <input type="button" value="No" onclick="window.close();" />
        <asp:PlaceHolder runat="server" id="closeandrefresh" visible="false">
            <script type="text/javascript">
                window.opener.location.href = window.opener.location.href;
                if (window.opener.progressWindow) window.opener.progressWindow.close();
                window.close();
            </script>
        </asp:PlaceHolder>
    </div>
    </form>
</body>
</html>
