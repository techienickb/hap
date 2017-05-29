Hi all,

Please note, that the new version of firefox is causing issues with home access plus+.  This issue only effects the silverlight version of the my computer browser, and the default uploader.

To fix the problem, either downgrade to 3.6.3 or:

# Type "about:config" into FF's address bar
# Accept the warning (if applicable)
# Search for the entry "dom.ipc.plugins.enabled.npctrl.dll"
# Change its value from "true" to "false" (double-click)
# Restart the browser

I have filed a bug report with Mozilla about this, and so has a fair few other people, as this is a big issue.