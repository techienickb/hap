<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Unzip.aspx.cs" Inherits="CHS_Extranet.Unzip" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Unzip</title>
    <link href="/extranet/basestyle.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
    <h1>Unzip</h1>
    <p>Unzip <asp:Literal runat="server" ID="item" /></p>
    <p>
        <asp:RadioButton ID="unziphere" Checked="true" GroupName="unzipto" Text="Unzip Here" runat="server" />
        <br />
        <asp:RadioButton runat="server" ID="unziptox" GroupName="unzipto" Text="Unzip to new folder called '{0}'" />
    </p>
    <p>
        <asp:HiddenField runat="server" ID="fullname" />
        <asp:Button runat="server" ID="ok" Text="Unzip" onclick="ok_Click" />
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
