<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="xls.aspx.cs" Inherits="HAP.Web.xls" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Excel Preview</title>
    <link href="<%=Request.ApplicationPath%>/basestyle.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
    <div style="position: absolute; top: 0; left: 0; right: 0; bottom: 0; overflow: auto; width: 100%; padding-bottom: 60px;">
        <asp:GridView ID="GridView1" runat="server" BorderColor="Gray" BorderStyle="Solid" BorderWidth="1px" AutoGenerateColumns="true">
        </asp:GridView>
    </div>
    <input type="button" value="Close" onclick="window.close();" style="position: absolute; right: 25px; bottom: 25px;" />
    </form>
</body>
</html>
