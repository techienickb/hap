<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Display.aspx.cs" Inherits="CHS_Extranet.BookingSystem.Display" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Display Board System</title>
    <style type="text/css">
        html { overflow: hidden; overflow-y: hidden; padding: 0; margin: 0; background: rgb(66, 0, 0); font-family: Calibri; font-size: 12pt; border: solid 0 rgb(66, 0, 0); }
        body { overflow: hidden; overflow-y: hidden; margin: 0; padding: 0; margin: 0; }
        form { padding: 0; margin: 0; height: 100%; }
        img { border: none; }
        p { font-size:13px; line-height:1.5em; color:#4e4e4e; margin:0.25em 0em 15px 0em; font-family:Segoe UI, Arial,Verdana,Helvetica,sans-serif; }
        a:link { font-size:13px; color:#6d051f; text-decoration:none; }
        a:hover { color:#96203e; text-decoration:underline; }
        a:visited { font-size:13px; color:#6d051f; text-decoration:none; }
        a:visited:hover { text-decoration:underline; }
        a:active { font-size:13px; color:#f58220; /*text-decoration:underline;*/ }
        h1 { font-size:30px; color:#fff; margin: 0; padding: 0; font-weight:lighter; /*font-size:160%;*/ }
        h2 { font-size:22px; font-weight:lighter; color:#f58220; padding:0; margin: 0; }
        h3 { font-size:32px; color: rgb(229,185,183); }
        
        .Outter { width: 340px; float: left; border-right: solid 2px #FFF; height: 600px; }
        .Content { padding: 2px 0 0px 0; }
        .Content div { padding: 11px 0 11px 20px; }
        .Current { background: #812d42; }
        #Right { float: right; text-align: right; border-left: solid 2px #FFF; border-right: 0; }
        #Right .Content div { padding: 11px 20px 11px 0; }
        h1.Head { text-align: center; color: rgb(195,214,155); padding: 6px 0; margin: 0; height: 30px; }
        h1.Head span { float: left; width: 340px; font-size: 30px; text-align: center; font-weight: bold; border-right: solid 2px #FFF; }
        h1.Head span.s { float: right; border-right: 0; border-left: solid 2px #FFF; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <asp:Panel runat="server" ID="ICT1_ICT2" Visible="false">
        <div class="Outter">
            <div class="Content">
                <asp:Repeater runat="server" ID="ICT1">
                    <ItemTemplate>
                        <div<%#currentLesson == Container.ItemIndex ? " class=\"Current\"" : ""%>>
                            <h1><b><%#(Container.ItemIndex + 1) %>:</b> <%#Eval("Name")%></h1>
                            <h2><%#getName(Container.DataItem) %></h2>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
        </div>
        <div id="Right" class="Outter">
            <div class="Content">
                <asp:Repeater runat="server" ID="ICT2">
                    <ItemTemplate>
                        <div<%#currentLesson == Container.ItemIndex ? " class=\"Current\"" : ""%>>
                            <h1><b><%#(Container.ItemIndex + 1) %>:</b> <%#Eval("Name")%></h1>
                            <h2><%#getName(Container.DataItem) %></h2>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
        </div>
        <h1 style="padding: 100px 0; text-align: center;">Food & Drink are not to be taken into any ICT ROOM!</h1>
        <h1 style="padding: 100px 0; text-align: center;">Bags are not to be left in the ICT Rooms!</h1>
        <img src="/Extranet/images/swish.png" alt="" style="position: absolute; bottom: -50px; left: 0; right: 0;" />
        <div style="position: absolute; bottom: 26px; left: 10px; right: 10px; text-align: center;">
            <img src="/Extranet/images/rounded-logo.png" alt="" style="position: absolute; left: 0; bottom: 0" />
            <span style="position: absolute; right: 50px; bottom: 30px; color: rgb(88,0,0);">Slide 1</span>
            <span style="color: rgb(88,0,0); display: block; padding-bottom: 30px;"><%=DateTime.Now.ToString("dddd, dd MMMM yyyy")%> (Week <%=bs.WeekNumber.ToString() %>)</span>
        </div>
        </asp:Panel>
        <asp:Panel runat="server" ID="defaultview" Visible="false">
            <h1><asp:Label runat="server" ID="roomlabel" /></h1>
            <div class="Outter">
                <div class="Content">
                    <asp:Repeater runat="server">
                        <ItemTemplate>
                            <div<%#currentLesson == Container.ItemIndex ? " class=\"Current\"" : ""%>>
                                <h1><b><%#(Container.ItemIndex + 1) %>:</b> <%#Eval("Name")%></h1>
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
