<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="rename.aspx.cs" Inherits="CHS_Extranet.Rename" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Confirm Rename</title>
    <link href="/extranet/basestyle.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <h1>Rename</h1>
        <p>
            Rename: <asp:Label runat="server" ID="filename" />
            <br />
            To:
            <asp:TextBox ID="newname" runat="server" />
        </p>
        <asp:HiddenField runat="server" ID="fullname" />
        <asp:Button runat="server" ID="yesren" UseSubmitBehavior="true" Text="Rename" onclick="yesren_Click" />
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
