<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="newfolder.aspx.cs" Inherits="CHS_Extranet.newfolder" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>New Folder</title>
    <link href="/extranet/basestyle.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <h1>New Folder</h1>
        <p><b>Folder Name: </b><asp:TextBox ID="foldername" runat="server" /></p>
        <asp:Button runat="server" ID="yes" UseSubmitBehavior="true" Text="Create" onclick="yes_Click" />
        <input type="button" value="Cancel" onclick="window.close();" />
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
