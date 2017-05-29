<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="OverviewCalendar.ascx.cs" Inherits="HAP.Web.BookingSystem.OverviewCalendar" %>
<%@ Register Assembly="HAP.Web" Namespace="HAP.Web.BookingSystem" TagPrefix="hap" %>
    
    <div ID="OverviewBox" style="display: none;">
        <div class="popupContent" style="width: 600px;">
            <h1>Month Overview</h1>
            <div>
                <iframe src="OverviewCalendar.aspx" frameborder="0" marginwidth="0" marginheight="0" scrolling="no" width="100%" height="400px"></iframe>
            </div>
            <div class="modalButtons">
                <input type="button" value="Close" onclick="hideOverview();" />
            </div>
        </div>
    </div>