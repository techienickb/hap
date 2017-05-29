<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UpdateChecker.ascx.cs" Inherits="HAP.Web.UpdateChecker" %>
<h1 id="updatecheck" class="Announcement">
    Checking for Update...
</h1>
<script>
    $.getJSON(hap.common.formatJSONUrl("~/api/setup/checkupdate"), function (data) {
        if (data.Current != null) {
            $("#updatecheck").html("A new version of Home Access Plus+ is available (Your Version " + data.Current + ", Latest Version " + data.Next + ")")
        } else $("#updatecheck").remove();
    });
</script>