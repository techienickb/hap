<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Announcement.ascx.cs" Inherits="HAP.Web.Controls.Announcement" %>
<asp:Literal runat="server" ID="AnnouncementText" />
<asp:PlaceHolder runat="server" ID="adminbox">
    <div id="EditAnn"><a href="#" class="EditAnnLink" title="Edit Announcement"><img runat="server" src="~/images/icons/edit.png" alt="Edit Announcement" /></a></div>
    <div id="editannouncement" title="Edit Announcement">
        <asp:TextBox runat="server" ID="htmlannouncement" TextMode="MultiLine" Rows="6" Columns="50" /><br />
        <asp:CheckBox ID="ShowAnnouncement" runat="server" Text="Show Announcement" /><br />
    </div>
    <script type="text/javascript" src="<%:ResolveClientUrl("~/Scripts/jquery.wysiwyg.js")%>"></script>
    <script type="text/javascript">
        $(function () { $("#editannouncement").dialog({ autoOpen: false }); $("textarea").wysiwyg({ css: hap.common.resolveUrl('~/style/editor.css') }); });
        $("#EditAnn").click(function () {
            $("#editannouncement").dialog({
                autoOpen: true,
                width: 500,
                buttons: {
                    "Save": function () {
                        $.ajax({
                            url: "api/announcement/save", type: 'POST', data: '{ "content": "' + escape($("textarea").wysiwyg("getContent")) + '", "show": "' + $("#<%=ShowAnnouncement.ClientID%>").is(":checked") + '" }', dataType: "json", contentType: 'application/JSON', success: function (data) {
                                $("#editannouncement").dialog("close");
                                document.location.reload();
                            }, error: hap.common.jsonError
                        });
                    },
                    "Close": function () {
                        $(this).dialog("close");
                    }
                }
            });
            return false;
        });
    </script>
</asp:PlaceHolder>