[Documentation.pdf](Documentation_Documentation.pdf)

# Installation/Configuration [Videos](Videos)
[Videos](Videos)
[User Card Lock Down Video](User-Card-Lock-Down-Video)
**[Demo Video](Demo-Video)**

# Problem with + in folder/file name
Run: 
{{
%windir%\system32\inetsrv\appcmd set config "Default Web Site" -section:system.webServer/security/requestFiltering -allowDoubleEscaping:true}}

# Problem with the App Pool Permissions
Run
{{
icacls C:\inetpub\wwwroot\Hap /grant "IIS AppPool\HAP":(RX)}}

# Changing the Admin Password
It is recommended to change the HAP+ password first before you change the AD admin password, but if you don't these are the steps to reset the password:
# Open ~/app_data/hapConfig.xml in notepad
# Set firstrun="false" to firstrun="true"
# Save and Close
# Open ~/web.config in notepad
# Find this:
{code:xml}
<location path="setup.aspx">
    <system.web>
      <authorization>
        <allow roles="Domain Admins" />
        <deny users="*" />
      </authorization>
    </system.web>
  </location>
{code:xml}
# Change it to:
{code:xml}
<location path="setup.aspx">
    <system.web>
      <authorization>
        <allow users="*" />
      </authorization>
    </system.web>
  </location>
{code:xml}
# Run HAP+
# Change the AD Password
# Save
# Follow the warning at the top of the setup page to re-secure the setup page

# [Timetable Plugin](Timetable-Plugin)
[Timetable Plugin](Timetable-Plugin)

# SIMS.net -> Static Booking System
* SIMS.net Report to Import and Run: [Booking System Export.RptDef](Documentation_Booking System Export.RptDef)
* [Video Tutorial on What to Do](http://www.youtube.com/watch?v=mEn3IhEV7_8)

# SIMS.net Photohandler (~/p.ashx)
* Import report [HAP+ Student Pictures.RptDef](Documentation_HAP+ Student Pictures.RptDef)
* Run report and save into the app_data folder as report.xml
* Enable the photohandler with ~/p.ashx

# [Developer References](Developer-References)

# [HAP and NLB](HAP-and-NLB)