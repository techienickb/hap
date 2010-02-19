<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="upload.aspx.cs" Inherits="CHS_Extranet.upload" %>
<%@ Register Assembly="System.Web.Silverlight" Namespace="System.Web.UI.SilverlightControls" TagPrefix="asp" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>File Uploader</title>
    <link href="/extranet/basestyle.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
        <asp:Silverlight ID="Xaml1" runat="server" Source="~/ClientBin/FileUpload.xap" 
            MinimumVersion="2.0.31005.0" Width="100%" Height="350px" InitParameters="UploadPage=FileUpload.ashx" />
        <input type="button" value="Done" onclick="done();" />
        <script type="text/javascript">
            function done() {
                window.opener.location.href = window.opener.location.href;
                if (window.opener.progressWindow) window.opener.progressWindow.close();
                window.close();
            }
        </script>
    </div>
    </form>
</body>
</html>
