<%@ Page Title="" Language="C#" MasterPageFile="~/masterpage.master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="HAP.Web.MyFiles.Default" %>
<asp:Content ContentPlaceHolderID="head" runat="server">
    <script src="../Scripts/jquery-1.6.2.min.js" type="text/javascript"></script>
    <script src="../Scripts/jquery-ui-1.8.16.custom.min.js" type="text/javascript"></script>
    <script src="../Scripts/jquery.ba-hashchange.min.js" type="text/javascript"></script>
    <script src="../Scripts/jquery.dynatree.js" type="text/javascript"></script>
    <link href="../style/ui.dynatree.css" rel="stylesheet" type="text/css" />
    <link href="../style/MyFiles.css" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ContentPlaceHolderID="body" runat="server">
    <div id="Tree" class="tile-border-color">
        <ul id="tree">
            <li><a href="#">My Drives</a>
                <ul>
                    
                </ul>
            </li>
        </ul>
    </div>
    <div id="MyFiles" class="tiles">
    </div>
    <script type="text/javascript">
        var items = new Array();
        function Item(data) {
            this.Data = data;
            this.Id = "";
            this.Render = function () {
                this.Id = this.Data.Path.replace(/\:/g, "").replace(/\\/g, "%20");
                var label = this.Data.Name + (this.Data.Path.match(/\\/g) ? "" : " (" + this.Data.Path + ")");

                $("#MyFiles").append('<a href="#' + this.Data.Path + '"' + (this.Data.Path.match(/\\/g) ? "" : ' class="drive"') + '><span class="icon" style="background-image: url(' + this.Data.Icon + ');"></span><span class="label">' + label + '</span>' + (this.Data.Path.match(/\\/g) ? '' : '<span class="progress"><label>' + this.Data.Space + '%</label><i style="width: ' + this.Data.Space + '%"></i></span>') + '</a>');
                $("#Tree > ul > li > ul").append('<li id="nav-' + this.Id + '"><a href="#' + this.Data.Path + '">' + this.Data.Name + '</a></li>');
            };
            this.Refresh = function () {
                $("#" + this.Id).attr("href", "#" + this.Data.Path);
                var label = this.Data.Name + (this.Data.Path.match(/\\/g) ? "" : " (" + this.Data.Path + ")");
                $("#" + this.Id).html('<span class="icon" style="background-image: url(' + this.Data.Icon + ');"></span><span class="label">' + label + '</span>' + (this.Data.Path.match(/\\/g) ? '' : '<span class="progress"><label>' + this.Data.Space + '%</label><i style="width: ' + this.Data.Space + '%"></i></span>'));
                $('#nav-' + this.Id + ' > a').attr("href", '#' + this.Data.Path);
                $('#nav-' + this.Id + ' > a').html(this.Data.Name);
            };
        }
        $(function () {
            $.ajax({
                type: 'GET',
                url: '<%=ResolveUrl("~/api/MyFiles/Drives")%>',
                dataType: 'json',
                contentType: 'application/json; charset=utf-8',
                success: function (data) {
                    items = new Array();
                    $("#MyFiles").html("");
                    for (var i = 0; i < data.length; i++)
                        items.push(new Item(data[i]));
                    for (var i = 0; i < items.length; i++)
                        items[i].Render();
                    $("#Tree").dynatree({ imagePath: "../images/setup/", selectMode: 1, noLink: true });
                }
            });
        });
    </script>
</asp:Content>
