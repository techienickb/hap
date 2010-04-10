<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BookingPopup.ascx.cs" Inherits="CHS_Extranet.BookingSystem.BookingPopup" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Assembly="System.Web.Ajax" Namespace="System.Web.UI" TagPrefix="asp" %>
                <div id="modalBackground" class="modalBackground" style="display: none;"></div>
                <div id="modalPopup" style="display: none;">
                    <asp:Panel runat="server" ID="Popup" Width="500px" CssClass="popupContent">
                        <asp:PlaceHolder runat="server" ID="manybookings" Visible="false">
                            <h1>Too Many Bookings</h1>
                            <p>You have exceeded the allowed bookings for this week.  Try next week instead. Or contact IT</p>
                        </asp:PlaceHolder>
                        <asp:PlaceHolder id="bookingform" runat="server">
                            <h1>New Booking</h1>
                            <div>
                                <div style="font-weight: bold;">Booking for <asp:Label runat="server" ID="lesson" /> on <asp:Label runat="server" ID="date" /> <asp:Label runat="server" ID="inLab" Text="in" /> <asp:Label runat="server" ID="room" /><asp:HiddenField runat="server" ID="bookingvars" /></div>
                                Year: 
                                <asp:DropDownList runat="server" ID="BookYear" Width="60px"><asp:ListItem Value="">--</asp:ListItem><asp:ListItem>7</asp:ListItem><asp:ListItem>8</asp:ListItem><asp:ListItem>9</asp:ListItem><asp:ListItem>10</asp:ListItem><asp:ListItem>11</asp:ListItem><asp:ListItem>12</asp:ListItem><asp:ListItem>13</asp:ListItem></asp:DropDownList>
                                Subject: 
                                <asp:TextBox ID="BookLesson" Width="100px" runat="server" /><asp:RequiredFieldValidator ControlToValidate="BookLesson" runat="server" ErrorMessage="*" />
                                <asp:Panel runat="server" ID="ltbooking" style="display: none;">
                                    Room: <asp:TextBox ID="BookLTRoom" runat="server" Width="40px" />
                                    Number of Laptops: <asp:RadioButton ID="lt16" Text="16" runat="server" Checked="true" GroupName="lt" /><asp:RadioButton ID="lt32" Text="32" runat="server" GroupName="lt" />
                                    <asp:CheckBox runat="server" ID="headphones" Text="Headphones?" TextAlign="Right" Checked="false" />
                                </asp:Panel>
                                <asp:PlaceHolder runat="server" ID="bookingadmin1">
                                    <div style="padding-top: 4px">Select Username: <asp:DropDownList runat="server" ID="userlist" /></div>
                                </asp:PlaceHolder>
                            </div>
                        </asp:PlaceHolder>
                        <div class="buttons">
                            <asp:Button runat="server" Text="Book" ID="book" OnClick="book_Click" />
                            <asp:Button ID="ok_btn" runat="server" Text="Cancel" CausesValidation="false" OnClientClick="$get('modalBackground').style.display = $get('modalPopup').style.display = 'none'; return false;" />
                        </div>
                    </asp:Panel>
                </div>
                <script type="text/javascript">
                    function setIDs() {
                        lessonID = "<%=lesson.ClientID %>";
                        roomID = "<%=room.ClientID %>";
                        bookingvarsID = "<%=bookingvars.ClientID %>";
                        ltbookingID = "<%=ltbooking.ClientID %>";
                        inID = "<%=inLab.ClientID %>";
                    }
                    setIDs();
                </script>