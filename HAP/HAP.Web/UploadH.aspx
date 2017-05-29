<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UploadH.aspx.cs" Inherits="HAP.Web.UploadH" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>File Uploader</title>
    <link href="~/style/basestyle.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <div>File 1: <asp:FileUpload ID="FileUpload1" runat="server" /></div>
        <div>File 2: <asp:FileUpload ID="FileUpload2" runat="server" /></div>
        <div>File 3: <asp:FileUpload ID="FileUpload3" runat="server" /></div>
        <div>File 4: <asp:FileUpload ID="FileUpload4" runat="server" /></div>
        <div>File 5: <asp:FileUpload ID="FileUpload5" runat="server" /></div>
        <asp:Literal runat="server" ID="message" />
        <div style="text-align: right;">
            <asp:Button ID="uploadbtn" runat="server" Text="Upload" onclick="uploadbtn_Click" />
            <asp:Button ID="uploadbtnClose" runat="server" Text="Upload + Close" onclick="uploadbtn_Click" />
            <input type="button" value="Cancel" onclick="done();" />
        </div>
        <script type="text/javascript">
            function done() {
                if (window.opener) {
                    window.opener.location.href = window.opener.location.href;
                    if (window.opener.progressWindow) window.opener.progressWindow.close();
                    window.close();
                } else {
                    if (window.parent.hap) { window.parent.Load(); window.parent.closeUpload(); }
                }
            }
        </script>
        <asp:Literal runat="server" ID="closeb" Visible="false">
            <script type="text/javascript">done();</script>
        </asp:Literal>
    </div>
    </form>
</body>
</html>
