The timetable plug in is a built in plugin of HAP+ which allows you to display the timetable of user.

# See the App_Data folder of the timetable plugin, run the reportdef file in SIMS
# Set the employeeID in AD to be the student's UPN
# Run HAP+
# Login as an Administrator
# Go to timetable.aspx
# At the top click on Options and run the Conversion script to reduce the file size and make HAP+ like it better
# Test by impersonating a student, either by UPN or by username (if employeeID is filled)

You can run a SIMS report to generate an XML file with timetables for each student, which the system can process.  If it returns 0 results it will switch to using Exchange Sync.

You can add optional Exchange calendars to display
{code:xml}
  <Timetable>
    <optionalCalendars><calendar calendar="office@crickhowell-hs.powys.sch.uk" roles="![Students](Students)" color="#ad3a3a" /></optionalCalendars>
  </Timetable>
{code:xml}