<%@ Page Language="C#" AutoEventWireup="true"  CodeBehind="Display.aspx.cs" Inherits="HAP.Web.BookingSystem.Display" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Display Board System</title>
    <link href="~/StyleSheet.css" rel="stylesheet" type="text/css" media="screen" />
    <style type="text/css">
        .Outter { width: 340px; float: left; overflow: hidden; }
        #header { width: 985px; margin: 0 auto; background-color: #fff; height: 170px;  }
        body { margin: 0; background: transparent; width: auto; }
        .Content { padding: 2px 0 0px 0; overflow: hidden; }
        .Content div { padding: 11px 0 11px 20px; }
        .Current { background: #ffb900; }
        #Right { float: right; text-align: right; border-right: 0; }
        #Right .Content div { padding: 11px 20px 11px 0; }
        h1, h2 { margin: 0; font-weight: normal; }
        h2 { color: Gray; }
        h1.Head { text-align: center; padding: 6px 0; margin: 0; height: 30px; }
        h1.Head span { float: left; width: 340px; font-size: 30px; text-align: center; font-weight: bold;}
        h1.Head span.s { float: right; border-right: 0; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <asp:Panel runat="server" ID="ICT1_ICT2" Visible="false">
        <div id="header">
            <a href="index.htm" id="logo"><span>Home</span></a>
        </div>
        <div class="Outter">
            <div class="Content">
                <h1 style="text-align: center;">ICT2</h1>
                <asp:Repeater runat="server" ID="ICT1">
                    <ItemTemplate>
                        <div<%#currentLesson == Eval("Lesson").ToString() ? " class=\"Current\"" : ""%>>
                            <h1><b><%#Eval("Lesson").ToString().Replace("Lesson ", "").Trim()%>:</b> <%#Eval("Name")%></h1>
                            <h2><%#getName(Container.DataItem) %></h2>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
        </div>
        <div id="Right" class="Outter">
            <div class="Content">
                <h1 style="text-align: center;">ICT2</h1>
                <asp:Repeater runat="server" ID="ICT2">
                    <ItemTemplate>
                        <div<%#currentLesson == Eval("Lesson").ToString() ? " class=\"Current\"" : ""%>>
                            <h1><%#Eval("Name")%> <b><%#Eval("Lesson").ToString().Replace("Lesson ", "").Trim()%>:</b></h1>
                            <h2><%#getName(Container.DataItem) %></h2>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
        </div>
        <h1 style="padding: 60px 0; text-align: center;">Food & Drink are not to be taken into any ICT ROOM!</h1>
        <h1 style="padding: 60px 0; text-align: center;">Bags are not to be left in the ICT Rooms!</h1>
        </asp:Panel>
        <asp:Panel runat="server" ID="defaultview" Visible="false">
            <style type="text/css">body { background: #fff; }</style>
            <h1 style="text-align: center;"><asp:Label runat="server" ID="roomlabel" Visible="false" /></h1>
            <div class="Outter">
                <div class="Content">
                    <asp:Repeater runat="server">
                        <ItemTemplate>
                            <div<%#currentLesson == Eval("Lesson").ToString() ? " class=\"Current\"" : ""%>>
                                <h1><b><%#Eval("Lesson").ToString().Replace("Lesson ", "").Trim() %>:</b> <%#Eval("Name")%></h1>
                                <h2><%#getName(Container.DataItem) %></h2>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
            </div>
        </asp:Panel>
    </form>
</body>
</html>
