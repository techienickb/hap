<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Announcement.ascx.cs" Inherits="HAP.Web.Announcement" %>
<asp:Literal runat="server" ID="AnnouncementText" />
<asp:PlaceHolder runat="server" ID="adminbox">
    <div id="EditAnn"><a href="#" class="EditAnnLink" title="Edit Announcement"><img runat="server" src="~/images/icons/edit.png" alt="Edit Announcement" /></a></div>
    <div id="editannouncement" title="Edit Announcement">
        <textarea id="htmlannouncement" rows="6" cols="100"><asp:Literal runat="server" ID="htmlannounce" /></textarea><br />
        <asp:CheckBox ID="ShowAnnouncement" runat="server" Text="Announcement     " TextAlign="Left" /><br />
    </div>
    <script type="text/javascript" src="<%:ResolveClientUrl("~/Scripts/jquery.wysiwyg.js")%>"></script>
    <script type="text/javascript">
        $("#editannouncement").hide();
        var wysiwyginit = false;
        $("#EditAnn").click(function () {
            $("#editannouncement").hapPopup({
                buttons:
                    [{
                        Text: "Save", Click: function () {
                            $.ajax({
                                url: "api/announcement/save", type: 'POST', data: '{ "content": "' + escape($("textarea").wysiwyg("getContent")) + '", "show": "' + $("#<%=ShowAnnouncement.ClientID%>").is(":checked") + '" }', dataType: "json", contentType: 'application/JSON', success: function (data) {
                                    $("#editannouncement").hapPopup("close");
                                    document.location.reload();
                                }, error: hap.common.jsonError
                            });
                            return false;
                        }
                    },
                    {
                        Text: "Close", Click: function () {
                            $(this).parents(".hapPopup").hide();
                            return false;
                        }
                    }
                ]
            });
            if (!wysiwyginit) $("textarea").wysiwyg({ css: hap.common.resolveUrl('~/style/editor.css') });
            wysiwyginit = true;
            return false;
        });
    </script>
</asp:PlaceHolder>